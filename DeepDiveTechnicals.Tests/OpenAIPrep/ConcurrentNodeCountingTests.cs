using System.Collections.Concurrent;
using System.Collections.Immutable;

using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

using static DeepDiveTechnicals.OpenAIPrep.ConcurrentNodeCounting;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class ConcurrentNodeCountingTests
    {
        [Fact]
        public async Task ConcurrentNodeCounting_CountsExpectedResult_ManagesDiamondGraphs()
        {
            // Diamond: R -> A -> C, and R -> B -> C
            var r = new Node("R");
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");

            r.Neighbors.Add(a);
            r.Neighbors.Add(b);
            a.Neighbors.Add(c);
            b.Neighbors.Add(c);

            var counter = new ConcurrentNodeCounting(); // per request
            var reachable = await counter.CountNodeAsync(r, []);

            reachable.Count.Should().Be(4);
            Console.WriteLine(string.Join(",", reachable.OrderBy(x => x.Id))); // A,B,C,R
            Console.WriteLine(reachable.Count); // 4
        }

        [Fact]
        public async Task ConcurrentNodeCounting_CycleGraph_ThrowsCycleDetection()
        {
            // Diamond: R -> A -> C, and R -> B -> C
            var r = new Node("R");
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");

            r.Neighbors.Add(a);
            r.Neighbors.Add(b);
            a.Neighbors.Add(c);
            b.Neighbors.Add(c);
            c.Neighbors.Add(a);

            var counter = new ConcurrentNodeCounting(); // per request
            var task = counter.CountNodeAsync(r, []);

            await Assert.ThrowsAsync<CycleDetectionException>(async () => await task);
        }
    }
}
