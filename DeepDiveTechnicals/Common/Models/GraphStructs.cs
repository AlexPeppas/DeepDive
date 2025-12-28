using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.Common.Models
{
    public static class GraphStructs
    {
        public enum GState 
        {
            Unvisited,
            Visited
        }

        public sealed class GNode 
        {
            public static GNode Init(object data, int adjacentsNo)
            {
                return new GNode(data, adjacentsNo);
            }

            private GNode(object data, int adjacentsNo)
            {
                Data = data;
                Adjacents = new List<GNode>();
                State = GState.Unvisited;
            }

            public List<GNode> Adjacents { get; set; }

            public object Data { get; set; }

            public GState State { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder($"node{Data}[");
                var count = 0;

                foreach (var adjacent in Adjacents)
                {
                    sb.Append(adjacent.Data.ToString()); 
                    count++;

                    if (count <= Adjacents.Count-1)
                    {
                        sb.Append(',');
                    }
                }

                sb.Append(']');
                return sb.ToString();
            }
        }

        public sealed class GGraph
        {
            private GGraph(int nodesNo)
            {
                Nodes = new List<GNode>();
            }

            public static GGraph Init(int nodesNo)
            {
                return new GGraph(nodesNo);
            }

            public List<GNode> Nodes { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                var count = 0;

                foreach (var node in Nodes)
                {
                    sb.Append(node.ToString());
                    count++;

                    if (count <= Nodes.Count - 1)
                    {
                        sb.Append(',');
                    }
                }

                return sb.ToString();
            }
        }
    }
}
