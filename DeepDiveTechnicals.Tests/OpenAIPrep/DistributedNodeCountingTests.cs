using DeepDiveTechnicals.OpenAIPrep;

using FluentAssertions;

namespace DeepDiveTechnicals.Tests.OpenAIPrep
{
    public sealed class DistributedNodeCountingTests
    {
        [Fact]
        public async Task DistributedGraph_NoCycles_ReturnsDistinctCount()
        {
            var node1 = new NodeRuntime { NodeId = "1", Neighbors = [] };
            var node2 = new NodeRuntime { NodeId = "2", Neighbors = [] };
            var node3 = new NodeRuntime { NodeId = "3", Neighbors = [] };
            var node5 = new NodeRuntime { NodeId = "5", Neighbors = [] };
            var node4 = new NodeRuntime { NodeId = "4", Neighbors = [node5] };
            var node0 = new NodeRuntime { NodeId = "0", Neighbors = new List<NodeRuntime> { node1, node2, node3, node4 } };

            var nodeCountingExperiment = new DistributedNodeCounting();
            var count = await nodeCountingExperiment.StartCount("request1", node0);

            Assert.Equal(5, count.Count);
        }

        [Fact]
        public async Task DistributedGraph_NoCycles_DuplicateMessage_ReturnsDistinctCount()
        {
            var node1 = new NodeRuntime { NodeId = "1", Neighbors = [] };
            var node2 = new NodeRuntime { NodeId = "2", Neighbors = [] };
            var node3 = new NodeRuntime { NodeId = "3", Neighbors = [] };
            var node5 = new NodeRuntime { NodeId = "5", Neighbors = [] };
            var node4 = new NodeRuntime { NodeId = "4", Neighbors = [node5] };
            var node0 = new NodeRuntime { NodeId = "0", Neighbors = new List<NodeRuntime> { node1, node2, node3, node4 } };
            node1.Neighbors.Add(node3);
            node3.Neighbors.Add(node2);

            var nodeCountingExperiment = new DistributedNodeCounting();
            var count = await nodeCountingExperiment.StartCount("request1",node0);

            Assert.Equal(5, count.Count);
        }

        [Fact]
        public async Task DistributedGraph_NoCycles_MultipleRequests_Idempotent_ReturnsDistinctCount()
        {
            var node1 = new NodeRuntime { NodeId = "1", Neighbors = [] };
            var node2 = new NodeRuntime { NodeId = "2", Neighbors = [] };
            var node3 = new NodeRuntime { NodeId = "3", Neighbors = [] };
            var node5 = new NodeRuntime { NodeId = "5", Neighbors = [] };
            var node4 = new NodeRuntime { NodeId = "4", Neighbors = [node5] };
            var node0 = new NodeRuntime { NodeId = "0", Neighbors = new List<NodeRuntime> { node1, node2, node3, node4 } };
            node1.Neighbors.Add(node3);
            node3.Neighbors.Add(node2);

            var nodeCountingExperiment = new DistributedNodeCounting();
            var count1 = await nodeCountingExperiment.StartCount("request1", node0);
            var count2 = await nodeCountingExperiment.StartCount("request2", node0);
            count1 = await nodeCountingExperiment.StartCount("request1", node0); // should be cached and Idempotent

            Assert.Equal(5, count1.Count);
            Assert.Equal(5, count2.Count);
        }

        [Fact]
        public async Task DistributedGraph_CycleDetection_Throws()
        {
            var node1 = new NodeRuntime { NodeId = "1", Neighbors = [] };
            var node2 = new NodeRuntime { NodeId = "2", Neighbors = [] };
            var node3 = new NodeRuntime { NodeId = "3", Neighbors = [] };
            var node5 = new NodeRuntime { NodeId = "5", Neighbors = [] };
            var node4 = new NodeRuntime { NodeId = "4", Neighbors = [node5] };
            var node0 = new NodeRuntime { NodeId = "0", Neighbors = new List<NodeRuntime> { node1, node2, node3, node4 } };
            node1.Neighbors.Add(node3);
            node3.Neighbors.Add(node2);
            node2.Neighbors.Add(node0);

            var nodeCountingExperiment = new DistributedNodeCounting();
            await Assert.ThrowsAsync<CycleDetectionException>(async () => await nodeCountingExperiment.StartCount("request1", node0));
        }
    }
}
