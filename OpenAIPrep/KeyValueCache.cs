using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class KeyValueCache
    {
        /// <summary>
        /// If we do have large objects that live on the LOH (large-object-heap) we do not want to cache the entire graph and overhead, instead churn CPU cycles for ser/deser and store the byte[]
        /// </summary>
        // THESE ALL CAN BE COLLAPSED IN A SINGLE DICTIONARY AND A Record(byte[] Value, DT ExpiresAt, LLNode node)
        private readonly ConcurrentDictionary<string, byte[]> _internalStore = new();
        private readonly ConcurrentDictionary<string, DateTime> _expirationPerKey = new();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockersPerKey = new();
        private readonly ConcurrentDictionary<string, LRUNode> _entryNodes = new();
        
        private readonly object _globalLock = new(); // Global critical lock for head & tail
        private LRUNode? _globalTail = null;
        private LRUNode? _globalHead = null;

        private int _maxItems = 0;

        public KeyValueCache(int maxItems = int.MaxValue)
        {
            if (maxItems < 0 )
            {
                throw new ArgumentOutOfRangeException("Cannot set cache with negative max items");
            }

            _maxItems = maxItems;

            Task.Run(async () => await ExpirationManagerDaemon());
        }

        public void Put(string key, string value, TimeSpan ttl)
        {
            // add or update
            _expirationPerKey[key] = DateTime.UtcNow + ttl; // first the TTL update to avoid race condition of PUT and then immediate eviction 
            _internalStore[key] = Encoding.UTF8.GetBytes(value); // here we could go with OCC and version tagging but keep simple for this draft.
            UpdateLRUHead(key);
        }

        private void UpdateLRUHead(string key)
        {
            if (_entryNodes.TryGetValue(key, out var lruNode))
            {
                if (lruNode.Head is not null && lruNode.Tail is not null)
                {
                    lruNode.Head.Tail = lruNode.Tail;
                    lruNode.Tail.Head = lruNode.Head;

                    // move to top
                    MoveLRUToTop(lruNode);
                }
                else if (lruNode.Head is null && lruNode.Tail is null) // first entry
                {
                    lock(_globalLock)
                    {
                        _globalTail = lruNode;
                        _globalHead = lruNode;
                    }
                }
                else if (lruNode.Head is null)
                {
                    // it's the bottomost node;
                    lruNode.Tail!.Head = null;

                    // move to top
                    MoveLRUToTop(lruNode);

                }
                else // lruNode.Head is null
                {
                    // it's the topmost node, do nothing
                }
            }
            else
            {
                lruNode = new LRUNode(key);

                // new addition
                if (_maxItems < _entryNodes.Count)
                {
                    throw new Exception("Cache Size overflow");
                }
                else if (_maxItems == _entryNodes.Count)
                {
                    // we need to pop
                    Purge(_globalHead?.Key ?? string.Empty);
                    
                    lock (_globalLock)
                    {
                        // set new tail
                        var newTail = _globalHead?.Tail;
                        newTail?.Head = null;
                    
                        _globalHead = newTail;
                    }
                    
                    // move new entry to top
                    _entryNodes[key] = lruNode!;
                    MoveLRUToTop(lruNode);
                }
                else
                {
                    // add on top, still within limits
                    // move to top
                    _entryNodes[key] = lruNode!;
                    MoveLRUToTop(lruNode);
                }
            }
        }

        private void Purge(string key)
        {
            if (string.IsNullOrEmpty(key))
            { return; }

            _entryNodes.TryRemove(key, out _);
            _internalStore.TryRemove(key, out _);
            _expirationPerKey.TryRemove(key, out _);
            _lockersPerKey.TryRemove(key, out _);
        }

        private void MoveLRUToTop(LRUNode lruNode)
        {
            lock(_globalLock)
            {
                lruNode.Head = _globalTail;
                lruNode.Tail = null;
                _globalTail?.Tail = lruNode;
                _globalTail = lruNode;
            }
        }

        public async ValueTask<string?> Get(string key)
        {
            if (_expirationPerKey.TryGetValue(key, out var expiresAt))
            {
                if (DateTime.UtcNow < expiresAt)
                {
                    // if not expired try lock
                    var locker = _lockersPerKey.GetOrAdd(key, new SemaphoreSlim(1,1));
                    var leaseAqcuired = false;
                    try
                    {
                        if (await locker.WaitAsync(TimeSpan.FromSeconds(5)))
                        {
                            leaseAqcuired = true;
                            if (_internalStore.TryGetValue(key, out var store))
                            {
                                UpdateLRUHead(key); // update LRU ONLY if read successfully
                                return Encoding.UTF8.GetString(store);
                            }
                        }
                    }
                    catch
                    {
                        // catch rare conditions or timeout. Ideally we would want to retry but better have concrete scenarios like
                        // - 500, throw for timeout
                        // etc
                    }
                    finally
                    {
                        if (leaseAqcuired)
                        {
                            locker.Release(1);
                        }
                    }
                }
                else
                {
                    // expired but not housekeeped, purge
                    Purge(key);
                }
            }
            return null;
        }

        private async Task ExpirationManagerDaemon()
        {
            do
            {
                foreach (var (key, expiresAt) in _expirationPerKey)
                {
                    if (expiresAt <= DateTime.UtcNow)
                    {
                        await RecordEviction(key);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5));
                
            }
            while (true);
            
        }

        private async Task RecordEviction(string key)
        {
            var locker = _lockersPerKey.GetOrAdd(key, new SemaphoreSlim(1, 1));
            var leaseAqcuired = false;
            try
            {
                if (await locker.WaitAsync(TimeSpan.FromSeconds(5)))
                {
                    leaseAqcuired = true;
                    Purge(key);
                }
            }
            catch
            {

            }
            finally
            {
                if (leaseAqcuired)
                {
                    locker.Release(1);
                }
            }
        }

        private record LRUNode(string Key)
        {
            public LRUNode? Tail { get; set; } = null;

            public LRUNode? Head { get; set; } = null;
        }
    }

    public sealed class KeyValueCacheTests
    {
        public static async Task KeyValueCache_VariousGetSet_Expiration_Success()
        {
            var cache = new KeyValueCache(maxItems: 3);

            cache.Put("k1", "123", TimeSpan.FromSeconds(10));

            var value = await cache.Get("k1");

            cache.Put("k2", "456", TimeSpan.FromHours(1));
            cache.Put("k3", "789", TimeSpan.FromHours(1));
            cache.Put("k2", "455", TimeSpan.FromSeconds(15));
            cache.Put("k4", "000", TimeSpan.FromSeconds(1));

            Thread.Sleep(TimeSpan.FromSeconds(5));
            value = await cache.Get("k1");
        }
    }
}
