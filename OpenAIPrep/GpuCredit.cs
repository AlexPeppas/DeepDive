using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public interface IGpuCredit
    {
        void Add(DateTime time, int credits, DateTime expires);

        bool Cost(DateTime time, int credits);

        int Balance { get; }
    }

    public sealed record RemainingCredits
    {
        public long StartTime { get; set; }
        public int Credits { get; set; } 
        public long Due { get; set; }
    }

    public sealed class GpuCredit : IGpuCredit
    {
        private readonly PriorityQueue<RemainingCredits, long> _minHeapCreditsDue;
        private readonly Queue<RemainingCredits> _creditsLedgeChronological;
        private readonly SemaphoreSlim _criticalLock;
        private int _currentBalance = 0;
        private long _latestTicks = 0;

        private readonly CancellationTokenSource _globalCTS;

        public GpuCredit()
        {
            _creditsLedgeChronological = new();
            _minHeapCreditsDue = new();
            _criticalLock = new SemaphoreSlim(1,1);

            _globalCTS = new CancellationTokenSource();
            _ = Task.Run(BGSweeper, _globalCTS.Token);
        }

        public int Balance => _currentBalance;

        public void Add(DateTime time, int credits, DateTime expires)
        {
            if (time.Ticks < _latestTicks)
            {
                throw new ArgumentOutOfRangeException("Ledge is chronologically oredred, it does not support back-dating");
            }

            if (time >= expires)
            {
                throw new ArgumentException($"{nameof(expires)} cannot be less than {nameof(time)}");
            }

            if (credits <=0)
            {
                Console.WriteLine("no-op");
                return;
            }

            var acquired = false;
            try
            {
                if (_criticalLock.Wait(TimeSpan.FromSeconds(5)))
                {
                    acquired = true;
                    var remainingCredits = new RemainingCredits { Credits = credits, Due = expires.Ticks, StartTime = time.Ticks };
                    _creditsLedgeChronological.Enqueue(remainingCredits);
                    _minHeapCreditsDue.Enqueue(remainingCredits, remainingCredits.Due); // min-heapify based on Due
                    _currentBalance += credits;
                    _latestTicks = time.Ticks;
                }
            }
            finally
            {
                if (acquired)
                {
                    _criticalLock.Release(1);
                }
            }
        }

        public bool Cost(DateTime time, int credits)
        {
            if (_creditsLedgeChronological is null || _creditsLedgeChronological.Count == 0)
            {
                return false;
            }

            if (_currentBalance < credits)
            {
                // fail early
                return false;
            }

            // no backdating
            if (time.Ticks < _latestTicks)
            {
                throw new ArgumentOutOfRangeException("Chronological only; no back-dating.");
            }

            var credited = false;
            var lockAcquired = false;

            // begin lock
            if (_criticalLock.Wait(TimeSpan.FromSeconds(5)))
            {
                lockAcquired = true;
                try
                {
                    var creditsCollected = 0;
                    var pendingTransactions = new List<Action>();
                    var twoPhaseCommit = new List<RemainingCredits>();
                    do
                    {
                        if (_creditsLedgeChronological.TryPeek(out var remainingCredits) && (remainingCredits.StartTime <= time.Ticks)) // within time window
                        {
                            if (remainingCredits.Due < time.Ticks)
                            {
                                // expired. lazy clean
                                _creditsLedgeChronological.TryDequeue(out remainingCredits);
                                continue;
                            }

                            if (remainingCredits.Credits + creditsCollected >= credits)
                            {
                                var toBeCredited = (credits - creditsCollected);
                                pendingTransactions.Add(() => remainingCredits.Credits -= toBeCredited);
                                pendingTransactions.Add(() => _currentBalance -= toBeCredited);

                                creditsCollected = credits;
                                credited = true;
                                if (remainingCredits.Credits - toBeCredited == 0) // node credits drained fully
                                {
                                    _creditsLedgeChronological.TryDequeue(out remainingCredits);
                                }
                            }
                            else
                            {
                                // not enough so we'll try to deduct in an ACID style.
                                var toBeCredited = remainingCredits.Credits;
                                pendingTransactions.Add(() => remainingCredits.Credits -= toBeCredited);
                                pendingTransactions.Add(() => _currentBalance -= toBeCredited);
                                creditsCollected += toBeCredited;
                                _creditsLedgeChronological.TryDequeue(out remainingCredits);
                            }
                        }
                        else
                        {
                            break;
                        }
                    } 
                    while (_creditsLedgeChronological.Count > 0 && !credited);

                    if (credited)
                    {
                        // commit
                        foreach (var trx in pendingTransactions)
                        {
                            trx();
                        }
                    }  
                }
                catch
                {
                }
                finally
                {
                    if (lockAcquired)
                    {
                        _criticalLock.Release(1);
                    }
                }
            }

            return credited;
        }

        private async Task BGSweeper()
        {
            var fallbackSleep = TimeSpan.FromSeconds(5);

            do
            {
                var acquired = false;
                try
                {
                    if (await _criticalLock.WaitAsync(TimeSpan.FromSeconds(5), _globalCTS.Token))
                    {
                        acquired = true;
                        if (_minHeapCreditsDue.TryPeek(out var credits, out var due))
                        {
                            if (due < DateTime.UtcNow.Ticks)
                            {
                                //expired
                                _minHeapCreditsDue.TryDequeue(out _, out _);
                                _currentBalance -= credits.Credits;
                            }
                            else
                            {
                                // not yet-expired and topmost root of min-heap sleep precisely until its due window.
                                var sleepTime = due - DateTime.UtcNow.Ticks;
                                acquired = false; // reset signal to guard against over-releasing
                                _criticalLock.Release(1);
                                await Task.Delay(TimeSpan.FromTicks(sleepTime), _globalCTS.Token);
                            }
                        }
                        else
                        {
                            acquired = false; // reset signal to guard against over-releasing
                            _criticalLock.Release(1);
                            // empty sleep default
                            await Task.Delay(fallbackSleep, _globalCTS.Token);
                        }
                    }
                }
                finally
                {
                    if (acquired)
                    {
                        _criticalLock.Release(1);
                    }
                }
            }
            while (true);
        }
    }
}
