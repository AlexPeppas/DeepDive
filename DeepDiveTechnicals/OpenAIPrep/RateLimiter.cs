using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class RateLimiter
    {
        private sealed record RequestsStructure
        {
            public int Etag { get; set; } // Monotonically incremental version style for OCC TRX

            public ConcurrentQueue<long> RequestTicks { get; set; }

            public SemaphoreSlim Gate { get; set; }
        }

        private readonly CancellationTokenSource _globalCancellationTokenSource;

        private readonly ConcurrentDictionary<string, RequestsStructure> _requestsPerUser = new();
        private readonly PriorityQueue<(string UserId, int Etag), long> _minHeapUserInteractivity = new(); // ticks is going to play the priority role. The lower ticks the fastest it needs to get deqeueued
       
        private readonly TimeSpan _window;
        private readonly int _maxRequests;
        private readonly BGMinHeapSweeper _bgSweeper;

        public RateLimiter(int maxRequests, TimeSpan window)
        {
            if (window < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException("Window cannot be smaller than 1s");
            }

            _window = window;
            _maxRequests = maxRequests;
            var globalToken = new CancellationTokenSource();
            _globalCancellationTokenSource = globalToken;
            _bgSweeper = new BGMinHeapSweeper(_minHeapUserInteractivity, _requestsPerUser, interactiveEligibility: 3 * window); // 3*window since the last tick request means the user is eligible for cleanup
            _ = Task.Run(async () => await _bgSweeper.KickStartSweeping(globalToken.Token));
        }

        // returns true if request is allowed, false if rate-limited
        public async ValueTask<bool> Allow(string userId)
        {
            if (_globalCancellationTokenSource.IsCancellationRequested)
            {
                throw new SystemException("CANCELLED");
            }

            if (!_requestsPerUser.TryGetValue(userId, out var userRequests))
            {
                var nowTicks = DateTime.UtcNow.Ticks;
                var initQueuedTicks = new ConcurrentQueue<long>();
                initQueuedTicks.Enqueue(nowTicks);
                var initUserTicks = new RequestsStructure { RequestTicks = initQueuedTicks, Gate = new SemaphoreSlim(1, 1), Etag = 0 };

                // IF a racing task has already initiated the user struct, update the existing userTicks instead of overwritting it.
                var result = _requestsPerUser.AddOrUpdate(
                    userId,
                    addValue: initUserTicks,
                    updateValueFactory: (userId, existingUserTicks) => { existingUserTicks.RequestTicks.Enqueue(nowTicks); existingUserTicks.Etag++; return existingUserTicks; });

                /// Now ticks acts as our priority. The next nowTicks for the same user will be lower in the min-heap. Thus the old will be popped faster. 
                /// Only and if only if our BG sweeper finds the same Etag it will purge from the user state
                _minHeapUserInteractivity.Enqueue((userId, result.Etag), nowTicks);

                if (_bgSweeper.TryStartWorking())
                {
                    // if sleeping, kick-start
                    _ = Task.Run(async () => await _bgSweeper.KickStartSweeping(_globalCancellationTokenSource.Token));
                }
                return await ValueTask.FromResult(true);
            }
            else
            {
                var totalReqsInCurrentWindow = 0;
                var now = DateTime.UtcNow;
                var acquired = false;
                try
                {
                    acquired = await userRequests.Gate.WaitAsync(TimeSpan.FromSeconds(5));
                    if (acquired)
                    {
                        var requestTicks = userRequests.RequestTicks;
                        do
                        {
                            if (requestTicks.TryPeek(out var peekTicks))
                            {
                                if (now.Ticks - peekTicks <= _window.Ticks)// not expired, within window
                                {
                                    totalReqsInCurrentWindow = requestTicks.Count;
                                    if (totalReqsInCurrentWindow < _maxRequests)
                                    {
                                        userRequests.RequestTicks.Enqueue(now.Ticks); // enqueue new
                                        userRequests.Etag++;
                                        _minHeapUserInteractivity.Enqueue((userId, userRequests.Etag), now.Ticks); // add on the min-heap

                                        if (_bgSweeper.TryStartWorking())
                                        {
                                            // if sleeping, kick-start
                                            _ = Task.Run(async () => await _bgSweeper.KickStartSweeping(_globalCancellationTokenSource.Token));
                                        }

                                    }
                                    else if (totalReqsInCurrentWindow >= _maxRequests)
                                    {
                                        return false; // overflow
                                    }

                                    return true;
                                }

                                //else expired, dequeue
                                requestTicks.TryDequeue(out _);
                            }
                        }
                        while (requestTicks.Count > 0);

                        if (requestTicks.Count == 0) // drained queue with all expired, enqueue the fresh request
                        {
                            userRequests.RequestTicks.Enqueue(now.Ticks); // enqueue new
                            userRequests.Etag++;
                            _minHeapUserInteractivity.Enqueue((userId, userRequests.Etag), now.Ticks);

                            if (_bgSweeper.TryStartWorking())
                            {
                                // if sleeping, kick-start
                                _ = Task.Run(async () => await _bgSweeper.KickStartSweeping(_globalCancellationTokenSource.Token));
                            }

                        }
                    }
                }
                finally
                {
                    if (acquired)
                    { 
                        userRequests.Gate.Release(1);
                    }
                }

                return await ValueTask.FromResult(totalReqsInCurrentWindow < _maxRequests);
            }
        }

        public async Task ShutDownAsync(CancellationToken cts)
        {
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_globalCancellationTokenSource.Token, cts);
            linkedToken.Cancel(throwOnFirstException: false);

            _requestsPerUser.Clear();
        }

        private sealed class BGMinHeapSweeper
        {
            private readonly ConcurrentDictionary<string, RequestsStructure> _requestsPerUser;
            private readonly PriorityQueue<(string UserId, int Etag), long> _minHeapUserInteractivity;
            private readonly TimeSpan _interactiveEligibility;

            /// pass the singleton and make the semantics easier for better state managemenet - SOLID
            public BGMinHeapSweeper(
                PriorityQueue<(string UserId, int Etag), long> minHeapUserInteractivity,
                ConcurrentDictionary<string, RequestsStructure> requestsPerUser,
                TimeSpan interactiveEligibility)
            {
                _minHeapUserInteractivity = minHeapUserInteractivity;
                _interactiveEligibility = interactiveEligibility;
                _requestsPerUser = requestsPerUser;
            }

            public enum State
            {
                Idle = 0,
                Working = 1
            }

            private int _internalState = (int)State.Idle;

            /// <summary>
            /// Readonly view of the state
            /// </summary>
            public State SweeperState => (State)Volatile.Read(ref _internalState);

            public bool TryStartWorking()
            {
                return Interlocked.CompareExchange(
                    ref _internalState,
                    (int)State.Working, // new value
                    (int)State.Idle) // existingValue    
                        == (int)State.Idle; 
            }

            /// <summary>
            ///  Volatile write guarantess that every other write happened before this line is first visible to all other consumer threads before this happened.
            ///  In low-level multi-threading we know that JIT/CPU/Compiler can perform read/write reorderings for efficiency
            ///  Given the following example,
            ///  var a = new Confg() // write 1
            ///  _state = State.Working // plain write 2
            ///  Another thread may see _state as working but still see a as null before JIT reordered them for efficiency which in multi-threading can be cruicial for strong consistency.
            ///  Volatile ensures that every preceding line, has been written before this is executed.
            /// </summary>
            private void SetWorking()
            {
                Volatile.Write(ref _internalState, (int)State.Working);
            }

            private void SetIdle()
            {
                Volatile.Write(ref _internalState, (int)State.Idle);
            }

            public async Task KickStartSweeping(CancellationToken cts)
            {
                SetWorking();
                
                do
                {
                    var now = DateTime.UtcNow;
                    if (_minHeapUserInteractivity.TryPeek(out var user, out var lastTicks))
                    {
                        if(now.Ticks <= _interactiveEligibility.Ticks + lastTicks)
                        {
                            // not expired; sleep for the peek's next window which by design will always be the lowest, any new request added has > ticks than this
                            var due = _interactiveEligibility.Ticks + lastTicks;
                            var remaining = due - now.Ticks;
                            if (remaining > 0) // ensure no exotic errors due to clock jitter
                            {
                                // sleep until we have the eligibility for purge due to inactivity for the topmost/lowest request.
                                await Task.Delay(TimeSpan.FromTicks(remaining), cts);
                            }
                        }
                        else
                        {
                            // expired. Dequeue and check if needs purge from main list.
                            if (_minHeapUserInteractivity.TryDequeue(out user, out _))
                            {
                                var (userId, userEtag) = user;
                                if (_requestsPerUser.TryGetValue(userId, out var userRequests))
                                {
                                    if (userEtag == userRequests.Etag) // check if ETAG otherwise do not Purge from main user state
                                    {
                                        var acquired = false;
                                        try 
                                        {
                                            if (await userRequests.Gate.WaitAsync(TimeSpan.FromSeconds(5), cts))
                                            {
                                                acquired = true;
                                                if (userEtag == userRequests.Etag) // if lock successfully acquired re-check etag, OCC LOCK.
                                                {
                                                    _requestsPerUser.TryRemove(userId, out _); // remove thread-safely the user from state due to inactivity.
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            if (acquired)
                                            {
                                                userRequests.Gate.Release(1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                // move to the next or sleep
                } while (_minHeapUserInteractivity.Count > 0);

                // we cleaned the entire min-heap, time to exit and we'll be waiting for the next kickstart request to wake up
                SetIdle();
            }
        }
    }

    public class RateLimiterTests
    {
        public async Task TryAllow_VariousScenarios_Succeeds()
        {
            var rateLimiter = new RateLimiter(5, TimeSpan.FromSeconds(10));

            var user1 = await rateLimiter.Allow("user1");
            var user2 = new RetryPolicy(3).WithExceptionTypes([typeof(ArgumentOutOfRangeException)]).ExecuteAsync(async () => await rateLimiter.Allow("user2"));
            
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
            user1 = await rateLimiter.Allow("user1");
        }
    }

    internal sealed class RetryPolicy
    {
        private readonly int MaxRetries = 3;
        private IEnumerable<Type> ExceptionTypes = [];
        public RetryPolicy(int retries)
        {
            if (retries >= 0)
            {
                MaxRetries = retries;
            }
        }

        public RetryPolicy WithExceptionTypes(IEnumerable<Type> exceptionsToHandle)
        {
            ExceptionTypes = exceptionsToHandle;
            return this;
        }

        public async Task<T> ExecuteAsync<T>(Func<T> del)
        {
            var currentAttempt = 0;

            do
            {
                try
                {
                    return del();
                }
                catch (Exception ex) when (ExceptionTypes.Contains(ex.GetType()))
                {
                    Console.WriteLine("Retrying");
                    currentAttempt++;
                }
                catch(Exception genericEx)
                {
                    _ = genericEx;
                }
            }
            while (currentAttempt <= MaxRetries);

            throw new Exception("Max retries, exhausted");
        }
    }
}
