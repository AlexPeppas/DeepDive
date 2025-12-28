using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepDiveTechnicals.Common.Models
{
    public static class TreeStructs
    {
        public sealed class GTreeNode
        {
            private GTreeNode(object data, GTreeNode left = null, GTreeNode right = null, GTreeNode parent = null)
            {
                Data = data;
                Left = left;
                Right = right;
                Parent = parent;
                Status = GTreeNodeStatus.Unvisisted;
            }

            public static GTreeNode Init(object data, GTreeNode left = null, GTreeNode right = null, GTreeNode parent = null)
            {
                return new GTreeNode(data, left, right, parent);
            }

            public object Data { get; internal init; }

            public GTreeNode Left { get; internal set; }

            public GTreeNode Right { get; internal set; }

            public GTreeNode Parent { get; internal set; }

            public GTreeNodeStatus Status { get; set; }
        }

        public enum GTreeNodeStatus 
        {
            Unvisisted,
            Visisted
        }
    }
}
