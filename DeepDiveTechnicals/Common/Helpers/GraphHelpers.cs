using System;

using static DeepDiveTechnicals.Common.Models.GraphStructs;

namespace DeepDiveTechnicals.Common.Helpers
{
    public static class GraphHelpers
    {
        private const int MaxNodeValue = 100;

        public static GGraph CreateRandomGraph(int nodesNo)
        {
            var graph = GGraph.Init(nodesNo);

            var random = new Random();

            // init nodes
            for (int i = 0; i < nodesNo; i++)
            {                
                var adjacentsNo = random.Next(nodesNo);
                var currentNode = GNode.Init(random.Next(MaxNodeValue), adjacentsNo);
                graph.Nodes[i] = currentNode;
            }

            // init adjacent relationships
            foreach (var node in graph.Nodes)
            {
                var rows = node.Adjacents.Count;
                for (int i = 0; i < rows; i++) 
                {
                    node.Adjacents[i] = graph.Nodes[random.Next(graph.Nodes.Count)];
                }
            }

            return graph;
        }
    }
}
