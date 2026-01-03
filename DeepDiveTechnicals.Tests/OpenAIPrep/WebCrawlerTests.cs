using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

namespace DeepDiveTechnicals.Tests
{
    public sealed class WebCrawlerTests
    {
        [Fact]
        public async Task Crawling_Bounded4CompetingWorkers()
        {
            var crawler = new WebCrawlerBoundedMultiThreading(4);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var urls = await crawler.OrchestrateCrawlingAsync("http://a.com/index", cts.Token);
            urls.Should().NotBeNull();
            urls.Count.Should().Be(9);
        }

        [Fact]
        public async Task Crawling_Bounded4CompetingWorkers_Cancelled()
        {
            var crawler = new WebCrawlerBoundedMultiThreading(4);
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
            var urls = await crawler.OrchestrateCrawlingAsync("http://a.com/index", cts.Token);
            urls.Should().NotBeNull();
            urls.Should().BeEmpty();
        }
    }
}
