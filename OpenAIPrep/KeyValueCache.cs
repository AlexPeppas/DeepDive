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
        
        private record CacheEntry(byte[] Value, DateTime ExpiresAt, LRUNode? NodePointer);
        private readonly ConcurrentDictionary<string, CacheEntry> _internalStore = new();
        
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockersPerKeyForEntries = new();
        private readonly object _globalLRULock = new(); // Global critical lock LRU shuffling operations

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
            var expiresAt = DateTime.UtcNow + ttl; // first the TTL update to avoid race condition of PUT and then immediate eviction 
            if (_internalStore.TryGetValue(key, out var entry))
            {
                entry = new CacheEntry(Value: Encoding.UTF8.GetBytes(value), expiresAt, entry.NodePointer);
            }
            else
            {
                entry = new CacheEntry(Value: Encoding.UTF8.GetBytes(value), expiresAt, null); // here we could go with OCC and version tagging but keep simple for this draft.
            }

            UpdateLRUHead(key, entry);
        }

        /// <summary>
        ///  Lazy expiration eviction upon Get.
        ///  Thread-safe for multi-threading environments per-key
        ///  Global lock per LRU operation for the entire process
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async ValueTask<string?> Get(string key)
        {
            if (_internalStore.TryGetValue(key, out var entry))
            {
                var expiresAt = entry.ExpiresAt;
                if (DateTime.UtcNow < expiresAt)
                {
                    // if not expired try lock
                    var locker = _lockersPerKeyForEntries.GetOrAdd(key, new SemaphoreSlim(1, 1));
                    var leaseAqcuired = false;
                    try
                    {
                        if (await locker.WaitAsync(TimeSpan.FromSeconds(5)))
                        {
                            leaseAqcuired = true;
                            if (_internalStore.TryGetValue(key, out entry))
                            {
                                if (DateTime.UtcNow < expiresAt)
                                {
                                    return Encoding.UTF8.GetString(entry.Value); // for the sake of simplicity we'll not handle release lock and acquire global lock for LRU purge/dettach here
                                }
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

                            // ONLY and if ONLY we acquired the lease and the 2nd entry get was not null means that we successfully SERVED
                            // Thus after we release the per-key lock, we attempt the LRU update on a global lock without mixing and nesting lockings from keyLock -> global and vice-versa
                            if (entry is not null)
                            {
                                UpdateLRUHead(key, entry); // update LRU ONLY if read successfully
                            }
                        }
                    }
                }
                else
                {
                    // expired but not housekeeped, purge under global lock to ensure we update the LRU
                    lock (_globalLRULock)
                    {
                        Purge(key, out var purgedEntry);
                        if (purgedEntry is not null && purgedEntry.NodePointer is not null)
                        {
                            Dettach(purgedEntry.NodePointer);
                        }
                    }
                }
            }
            return null;
        }

        private void UpdateLRUHead(string key, CacheEntry entry)
        {
            lock(_globalLRULock)
            {
                if (entry.NodePointer is not null)
                {
                    var lruNode = entry.NodePointer;
                    if (lruNode.Head is not null && lruNode.Tail is not null)
                    {
                        lruNode.Head.Tail = lruNode.Tail;
                        lruNode.Tail.Head = lruNode.Head;

                        // move to top
                        MoveLRUToTop(lruNode);
                    }
                    else if (lruNode.Head is null && lruNode.Tail is null) // first entry
                    {
                        _globalTail = lruNode;
                        _globalHead = lruNode;
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

                    _internalStore[key] = entry;
                }
                else
                {
                    var lruNode = new LRUNode(key);

                    // new addition
                    if (_maxItems < _internalStore.Count)
                    {
                        throw new Exception("Cache Size overflow");
                    }
                    else if (_maxItems == _internalStore.Count)
                    {
                        // set new tail
                        var newTail = _globalHead?.Tail;
                        newTail?.Head = null;

                        // we need to pop
                        Purge(_globalHead?.Key ?? string.Empty, out var purgedEntry);
                        Dettach(purgedEntry!.NodePointer!);

                        _globalHead = newTail;
                    }

                    // move new entry to top
                    _internalStore[key] = new CacheEntry(entry!.Value, entry.ExpiresAt, lruNode);
                    MoveLRUToTop(lruNode);
                }
            }
        }

        private void Purge(string key, out CacheEntry? purgedEntry)
        {
            if (string.IsNullOrEmpty(key))
            {
                purgedEntry = null;
                return; 
            }

            _internalStore.TryRemove(key, out purgedEntry);
            _lockersPerKeyForEntries.TryRemove(key, out _);
        }

        private void MoveLRUToTop(LRUNode lruNode)
        {           
            lruNode.Head = _globalTail;
            lruNode.Tail = null;
            _globalTail?.Tail = lruNode;
            _globalTail = lruNode;    
        }

        private async Task ExpirationManagerDaemon()
        {
            do
            {
                foreach (var (key, entry) in _internalStore)
                {
                    if (entry.ExpiresAt <= DateTime.UtcNow)
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
            var locker = _lockersPerKeyForEntries.GetOrAdd(key, new SemaphoreSlim(1, 1));
            var leaseAqcuired = false;
            CacheEntry? purgedEntry = null;
            try
            {
                if (await locker.WaitAsync(TimeSpan.FromSeconds(5)))
                {
                    leaseAqcuired = true;
                    Purge(key, out purgedEntry);
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

                    // keep the global lock for the LRU shuffling
                    if (purgedEntry is not null && purgedEntry.NodePointer is not null)
                    {
                        lock (_globalLRULock)
                        {
                            Dettach(purgedEntry.NodePointer);
                        }
                    }
                    

                }
            }
        }

        private void Dettach(LRUNode node)
        {             
            var tail = node.Tail;
            var head = node.Head;

            tail?.Head = head;
            head?.Tail = tail;

            if (node == _globalHead)
            {
                _globalHead = tail;
            }
            if (node == _globalTail)
            {
                _globalTail = head;
            }

            node.Dispose();
        }

        private record LRUNode(string Key) : IDisposable
        {
            public LRUNode? Tail { get; set; } = null;

            public LRUNode? Head { get; set; } = null;

            public void Dispose()
            {
                this.Tail = null;
                this.Head = null;
            }
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
