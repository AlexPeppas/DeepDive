using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class WebCrawlerBoundedMultiThreading
    {
        private readonly int _maxThreads;
        
        private readonly ConcurrentQueue<Uri> _urisToBeCrawled = new();
        private readonly HashSet<string> _urlsCrawled = new();
        
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _crawlLock;
        private readonly CancellationTokenSource _globalCancelSignal;
        
        private readonly HtmlParser _htmlParser;

        public WebCrawlerBoundedMultiThreading(int maxThreads)
        {
            _maxThreads = maxThreads;
            _crawlLock = new();
            _globalCancelSignal = new CancellationTokenSource();
            _htmlParser = new HtmlParser();
        }

        public async Task<List<Uri>> OrchestrateCrawlingAsync(string url, CancellationToken cts)
        {
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cts, _globalCancelSignal.Token);

            _urisToBeCrawled.Enqueue(new Uri(url));

            var workersSpawn = new List<Task>();
            /// spawn as many instances as our max bounded threads
            for (var thread = 0; thread < _maxThreads; thread++)
            {
                workersSpawn.Add(Task.Run(async () => await BGCrawler(url, thread, linkedToken.Token), linkedToken.Token));
            }
            try
            {
                await Task.WhenAll(workersSpawn);
            }
            catch(Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                Debug.WriteLine("Cancellation was requested");
                return [];
            }
            
            return [.. _urlsCrawled.Select(crawled => new Uri(crawled))];
        }

        /// <summary>
        ///  Competing consuming long polling workers pattern
        /// </summary>
        private async Task BGCrawler(string url, int index, CancellationToken cts)
        {
            var maxStaleAttempts = 3;
            var currentAttempt = 0;
            var host = new Uri(url).Host;
            var backOffSleep = TimeSpan.FromSeconds(2);

            while (currentAttempt < maxStaleAttempts)
            {
                while (_urisToBeCrawled.Count > 0)
                {
                    currentAttempt = 0; // reset attempt
                    if (_urisToBeCrawled.TryDequeue(out var uriBeingCrawled))
                    {
                        var lockAcquired = false;
                        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
                        try
                        {
                            if (!_urlsCrawled.Contains(uriBeingCrawled.AbsoluteUri))
                            {
                                if(_crawlLock.TryGetValue(uriBeingCrawled.AbsoluteUri, out _))
                                {
                                    // a competing instance is already crawling this
                                    continue;
                                }
                                
                                if (_crawlLock.TryAdd(uriBeingCrawled.AbsoluteUri, semaphore))
                                {
                                    if (await semaphore.WaitAsync(TimeSpan.FromSeconds(5), cts))
                                    {
                                        lockAcquired = true;
                                        var parsed = await _htmlParser.ParseAsync(uriBeingCrawled, cts);
                                        _urlsCrawled.Add(uriBeingCrawled.AbsoluteUri);

                                        foreach (var newUrl in parsed)
                                        {
                                            if (newUrl.Host == host && !_urlsCrawled.Contains(newUrl.AbsoluteUri)) // if newUrl has same host and not already crawled - cycle prevention
                                            {
                                                _urisToBeCrawled.Enqueue(newUrl); // even if we add a uri that just has been added to the urlsCrawled hashset we'll skip it when one of the competing workers consumes it. Lazy clean.
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            throw;
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException && ex is not TaskCanceledException)
                        {
                            Debug.WriteLine("Non-parseable uri, or someone else acquired lock first. Skip and continue");
                        }
                        finally
                        {
                            if (lockAcquired)
                            {
                                semaphore.Release(1);
                            }
                        }
                    }
                }

                Debug.WriteLine($"Worker_{index} going stale for 1sec and attempt: {currentAttempt}");
                await Task.Delay((int)Math.Pow(backOffSleep.Seconds,currentAttempt), cts);
                currentAttempt++;
            }
        }
    }

    public sealed class HtmlParser
    {
        private readonly Dictionary<string, List<Uri>> _uris = new Dictionary<string, List<Uri>>()
        {
            ["http://a.com/index"] = new List<Uri> { new("http://a.com/a"), new("http://a.com/b") , new("http://b.com/x") },
            ["http://a.com/a"] = new List<Uri> { new("http://a.com/b")},
            ["http://a.com/b"] = new List<Uri> { new("http://a.com/c"), new("http://a.com/d"), new("http://a.com/e"), new("http://b.com/x"), new("http://b.com/xx") },
            ["http://a.com/c"] = new List<Uri> { new("http://a.com/b") },
            ["http://a.com/c"] = [],
            ["http://a.com/e"] = new List<Uri> { new("http://a.com/f"), new("http://a.com/e"), new("http://a.com/c"), new("http://a.com/c?utf8=true"), new("http://a.com/c?fcbStream=true&groupSession=false"), new("http://b.com/x"), new("http://b.com/xx") },
            ["http://a.com/f"] = [],
            ["http://a.com/c?utf8=true"] = [],
            ["http://a.com/c?fcbStream=true&groupSession=false"] = [],
        };

        public async Task<List<Uri>> ParseAsync(Uri uri, CancellationToken cts)
        {
            await Task.Yield();
            if (_uris.TryGetValue(uri.AbsoluteUri, out var uris))
            {
                return uris;
            }

            return [];
        }
    }
}
