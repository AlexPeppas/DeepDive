using DeepDiveTechnicals.Common.Helpers;
using DeepDiveTechnicals.DataStructures.LinkedList;
using DeepDiveTechnicals.DataStructures.StacksAndQueues;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using static DeepDiveTechnicals.Common.Models.GraphStructs;
using static DeepDiveTechnicals.Common.Models.TreeStructs;
using static DeepDiveTechnicals.DataStructures.TreesAndGraphs.TreesAndGraphs;

namespace DeepDiveTechnicals.DataStructures.TreesAndGraphs
{
    public class NodeStruct
    {
        public int data;
        public NodeStruct left;
        public NodeStruct right;
        public NodeStruct parent; //pointer which points to previous node "problem 4.6"

        public NodeStruct(int data)
        {
            this.data = data;
            this.left = null;
            this.right = null;
        }
    }

    public class GraphNode
    {
        public string name;
        public bool visited = false;
        public GraphNode[] adjacents;
        public State state; // property for 4.1 problem which demands intermidiate statuses

    }

    public class GraphStruct
    {
        public GraphNode[] GraphNodes;
    }



    public static class TreeBasicTraversals
    {
        public static void PreOrder(NodeStruct head)
        {
            if (head != null)
            {
                Console.WriteLine(head.data);
                PreOrder(head.left);
                PreOrder(head.right);
            }
        }

        public static void PostOrder(NodeStruct head)
        {
            if (head != null)
            {
                PostOrder(head.left);
                PostOrder(head.right);
                Console.WriteLine(head.data);
            }
        }

        public static void InOrder(NodeStruct head)
        {
            if (head != null)
            {
                InOrder(head.left);
                Console.WriteLine(head.data);
                InOrder(head.right);
            }
        }
    }

    public static class GraphSearch
    {
        public static void DFS(GraphNode root)
        {
            if (root == null) return;
            //doSomething(root)
            root.visited = true;
            foreach (var neighbor in root.adjacents)
            {
                if (neighbor.visited == false)
                    DFS(neighbor);
            }
        }

        public static void BFS(GraphNode root)
        {
            CustomQueue<GraphNode> queue = new CustomQueue<GraphNode>();
            root.visited = true;
            queue.Add(root); //Enqueue

            while (!queue.IsEmpty())
            {
                var currentNode = queue.Remove();
                foreach (var neighbor in currentNode.adjacents)
                {
                    if (neighbor.visited == false)
                    {
                        //doSomething(neighbor)
                        neighbor.visited = true;
                        queue.Add(neighbor);
                    }
                }
            }
        }
    }


    public static class TreesAndGraphs
    {
        /// <summary>
        /// Problem : 4.1
        /// Description : Route Between Nodes: Given a directed graph, design an algorithm to find out whether there is a
        ///route between two nodes.
        /// </summary>
        /// 
        public static bool RouteBetweenNodes(GraphStruct graph, GraphNode source, GraphNode destination)
        {
            if (source == destination) return true;

            for (var i = 0; i < graph.GraphNodes.Length; i++)
            {
                graph.GraphNodes[i].state = State.unvisited;
            }

            CustomQueue<GraphNode> queue = new CustomQueue<GraphNode>();

            queue.Add(source);
            GraphNode temp;
            while (!queue.IsEmpty())
            {
                temp = queue.Remove();
                foreach (var neighbor in temp.adjacents)
                {
                    if (neighbor.state == State.unvisited)
                    {
                        if (neighbor == destination) return true;
                        else
                        {
                            neighbor.state = State.visiting;
                            queue.Add(neighbor);
                        }
                    }
                }
                temp.state = State.visited;
            }
            return false;
        }

        public static bool RouteBetweenNodesV2(int nodesNo)
        {
            var graph = GraphHelpers.CreateRandomGraph(nodesNo);
            Debug.WriteLine($"GRAPH: **** \n {graph}");

            var src = graph.Nodes[1];
            var dst = graph.Nodes[3];

            ApplyDFS(src, dst, out var exists);
            return exists;
        }

        private static void ApplyDFS(GNode current, GNode dest, out bool routeExists)
        {
            if (current is null)
            {
                routeExists = false;
                return;
            }

            current.State = GState.Visited;

            if (ReferenceEquals(current, dest))
            {
                routeExists = true;
                return;
            }

            foreach (var node in current.Adjacents)
            {
                if (node.State == GState.Unvisited)
                {
                    ApplyDFS(node, dest, out routeExists);
                    if (routeExists is true)
                    {
                        return;
                    }
                }
            }

            routeExists = false;
        }

        public enum State { unvisited = 0, visiting, visited };

        /// <summary>
        /// Problem : 4.2
        /// Description : Given a sorted (increasing order) array with unique integer elements, write an
        /// algorithm to create a binary search tree with minimal height.
        ///
        /// Explanation : 
        /// To create a tree of minimal height we need to match the number of nodes in the left subtree to the number of nodes in the right subtree
        /// as much as possible. This means that we want the root to be the middle of the array, since half the elements (placed left) should be less than the root
        /// and half (placed right) should be greater than the root.
        /// </summary>
        public static NodeStruct ConstructMinimalTree(int[] array)
        {
            NodeStruct root = MinimalTreeInternal(array, 0, array.Length); // this will return the first array[mid] which is going to be the root of the BST
            return root;
        }

        private static NodeStruct MinimalTreeInternal(int[] array, int start, int end)
        {
            if (end < start)
                return null;

            int mid = (start + end) / 2;
            NodeStruct node = new NodeStruct(array[mid]);
            node.left = MinimalTreeInternal(array, start, mid - 1);
            node.right = MinimalTreeInternal(array, mid + 1, end);
            return node;
        }

        public static GTreeNode ConstructMinimalTreeV2(int[] array)
        {
            // 1, 3, 4, 6, 8, 9, 12, 15, 21
            // VISUALIZE THE TREE
            /*                      8
                        3                   15      21
                    1       4          9
                              6           12
            */
            var root = MinimalTreeInternalV2(array, 0, array.Length); // this will return the first array[mid] which is going to be the root of the BST
            // PrintTree(root);
            return root;
        }

        /*private static void PrintTree(GTreeNode root, string indent = "", bool isLeft = true)
        {
            if (root == null)
            {
                return;
            }

            // Print the current node (right-most first for better visual alignment)
            if (root.Right != null)
            {
                PrintTree(root.Right, indent + (isLeft ? "│   " : "    "), false);
            }

            // Print current node's data
            Debug.WriteLine(indent + (isLeft ? "└── " : "┌── ") + root.Data);

            // Print the left subtree
            if (root.Left != null)
            {
                PrintTree(root.Left, indent + (isLeft ? "    " : "│   "), true);
            }
        }

        private static void PrintTree(GTreeNode root, int midPointer, bool leftIndent, bool comesFromLeft, bool isFirst)
        {
            if (root is null)
            {
                return;
            }

            if (midPointer<0)
                return;

            if (isFirst)
            {
                var nodeVisual = root.Data.ToString().PadLeft(midPointer,' ');
                Debug.WriteLine(nodeVisual);
            }

            var tempMidPointer = midPointer;

            if (comesFromLeft && !isFirst)
            {
                if (leftIndent)
                {
                    tempMidPointer -= 2;
                    var nodeVisual = root.Data.ToString().PadLeft(tempMidPointer, ' ');
                    Debug.WriteLine(nodeVisual);
                }
                else
                {
                    tempMidPointer -= 1;
                    var nodeVisual = root.Data.ToString().PadLeft(tempMidPointer, ' ');
                    Debug.WriteLine(nodeVisual);
                }
            }
            else if (!comesFromLeft && !isFirst)
            {
                if (leftIndent)
                {
                    tempMidPointer += 1;
                    var nodeVisual = root.Data.ToString().PadLeft(tempMidPointer, ' ');
                    Debug.WriteLine(nodeVisual);
                }
                else
                {
                    tempMidPointer += 2;
                    var nodeVisual = root.Data.ToString().PadLeft(tempMidPointer + 2, ' ');
                    Debug.WriteLine(nodeVisual);
                }
            }

            PrintTree(root.Left, tempMidPointer, true, true, false);
            PrintTree(root.Right, tempMidPointer, false, false, false);
        }*/

        private static GTreeNode MinimalTreeInternalV2(int[] array, int start, int end)
        {
            if (end <= start)
                return null;

            var mid = (start + end) / 2;

            var node = GTreeNode.Init(array[mid]);
            node.Left = MinimalTreeInternalV2(array, start, mid-1);
            node.Right = MinimalTreeInternalV2(array, mid + 1, end);

            return node;
        }

        /// <summary>
        /// Problem : 4.3
        /// Description : List of Depths: Given a binary tree, design an algorithm which creates a linked list of all the nodes
        /// at each depth(e.g., if you have a tree with depth D, you'll have D linked lists).
        /// </summary>
        /// 

        /*Commentary : 
        We can implement a simple modification of the pre-order traversal algorithm, where we pass in level +
        1 to the next recursive call. The code below provides an implementation using depth-first search.
        */
        public static List<LinkedListStruct> LinkedListStructList { get; set; }

        public static List<LinkedListStruct> ListOfDepthsDFS(NodeStruct root)
        {
            ListOfDepthsDFS(root, 1);
            return LinkedListStructList;
        }
        public static void ListOfDepthsDFS(NodeStruct root, int level)
        {
            if (root == null) return; //base case

            if (LinkedListStructList.Count < level)
            {
                LinkedListStructList.Add(new DataStructures.LinkedList.LinkedListStruct(root.data));
            }
            else
            {
                LinkedListStructList[level - 1].Next = new LinkedListStruct(root.data);
            }
            ListOfDepthsDFS(root.left, level++);
            ListOfDepthsDFS(root.right, level++);

        }

        /// <summary>
        /// Problem : 4.4
        /// Description : Implement a function to check if a binary tree is balanced. For the purposes of
        ///this question, a balanced tree is defined to be a tree such that the heights of the two subtrees of any
        ///node never differ by more than one.
        /// </summary>

        /*Commentary : On each node. we recurse through its entire subtree. This means
        that getHeight is called repeatedly on the same nodes. The algorithm isO(N log N) since each node is
        "touched" once per node above it.
        */
        public static bool CheckBalanced(NodeStruct root)
        {
            if (root == null) return true;
            int heightDiff = GetHeight(root.left) - GetHeight(root.right);
            if (Math.Abs(heightDiff) > 1)
                return false;
            else
                return (CheckBalanced(root.left) && CheckBalanced(root.right));
        }

        public static int GetHeight(NodeStruct node)
        {
            if (node == null) return -1;

            return Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
        }

        /// <summary>
        /// Problem : 4.5
        /// Description : Implement a function to check if a binary tree is a binary search tree.
        /// </summary>
        public static bool ValidateBST(NodeStruct root)
        {
            return ValidateBST(root, int.MaxValue, int.MinValue);
        }
        public static bool ValidateBST(NodeStruct node, int min, int max)
        {
            if (node == null) return true;

            if ((min != int.MaxValue && node.data <= min) || (max != int.MinValue && node.data > max))
                return false;


            return (ValidateBST(node.left, min, node.data) && ValidateBST(node.right, node.data, max));

        }
        /*
         * We can either approach it with a in-order traversal. If we know that the tree does not contain duplicates then the in order traversal
         * should produce a sorted array (asc). To avoid the usage of an array we just keep the top(lastVisited) value. If the current node is lower than the min
         * and it breaks the sorting rule then it is not a BST. If we finish traversal with no break then we have a valid BST
         */
        public static int? minimumV = null;
        public static bool ValidateBSTInOrderTraversal(NodeStruct root)
        {
            if (root == null) return true;

            if (!ValidateBSTInOrderTraversal(root.left)) 
                return false;

            if (root.data < minimumV && minimumV != null)
                return false;
            else
                minimumV = root.data;

            if (!ValidateBSTInOrderTraversal(root.right)) 
                return false;

            return true;
        }

        /// <summary>
        /// Problem : 4.6
        /// Description : Write an algorithm to find the "next" node (i.e., in-order successor) of a given node in a
        ///binary search tree.You may assume that each node has a link to its parent.
        /// </summary>
        public static NodeStruct Successor(NodeStruct node)
        {
            if (node.right != null)
            {
                return leftMostChild(node.right);
            }
            else
            {
                NodeStruct q = node;
                NodeStruct x = q.parent;
                while (x != null && x.left != q)
                {
                    q = x;
                    x = x.parent;
                }
                return x;
            }

        }
        /*
         *  But wait-what if we traverse all the way up the tree before finding a left child?This will happen only when
            we hit the very end of the in-order traversal. That is, if we're already on the far right of the tree, then there is
            no in-order successor. We should return null.

         */
        public static NodeStruct leftMostChild(NodeStruct node)
        {
            if (node == null)
                return null;
            while (node.left != null)
            {
                node = node.left;
            }
            return node;
        }

        /// <summary>
        /// Problem : 4.7
        /// Description : You are given a list of projects and a list of dependencies (which is a list of pairs of
        /// projects, where the second project is dependent on the first project). All of a project's dependencies
        /// must be built before the project is. Find a build order that will allow the projects to be built.If there
        /// is no valid build order, return an error.
        /// EXAMPLE
        /// Input:
        /// projects: a, b, c, d, e, f
        /// dependencies: (a, d), (f, b), (b, d), (f, a), (d, c)
        /// c-->d, d-->a,b b-->f a-->f
        /// Output: f, e, a, b, d, c
        /// </summary>
        
        public static bool TryBuildOrderV2(List<string> projects, List<Tuple<string,string>> dependencies, out HashSet<string> order)
        {
            order = new HashSet<string>();

            var graph = PrepareGraphFromDeps(dependencies, projects.Count);

            var exists = true;
            foreach(var node in graph.Nodes)
            {
                if (node.State == GState.Unvisited)
                {
                    exists &= TraverseBuildOrderWithLoopDetection(node, node, order);
                }
            }

            if (exists)
            {
                foreach(var proj in projects)
                {
                    // non-dependent stuff
                    if (!order.Contains(proj))
                    {
                        order = order.Prepend(proj).ToHashSet();
                    }
                }
            }

            return exists;
        }

        private static GGraph PrepareGraphFromDeps(List<Tuple<string, string>> dependencies, int graphNodes)
        {
            var graph = GGraph.Init(graphNodes);
            var projToNode = new Dictionary<string,GNode>();

            foreach(var pair in dependencies)
            {
                GNode masterNode;
                GNode dependentNode;

                if (!projToNode.ContainsKey(pair.Item1))
                {
                    masterNode = GNode.Init(pair.Item1, 0);
                    projToNode.Add(pair.Item1, masterNode);
                    graph.Nodes.Add(masterNode);
                }
                else
                {
                    masterNode = projToNode[pair.Item1];
                }

                if (!projToNode.ContainsKey(pair.Item2))
                {
                    dependentNode = GNode.Init(pair.Item2, 0);
                    projToNode.Add(pair.Item2, dependentNode);
                    graph.Nodes.Add(dependentNode);
                }
                else
                {
                    dependentNode = projToNode[pair.Item2];
                }

                dependentNode.Adjacents.Add(masterNode);
            }

            return graph;
        }

        private static bool TraverseBuildOrderWithLoopDetection(GNode started, GNode node, HashSet<string> order)
        {
            node.State = GState.Visited;

            foreach (var neighbor in node.Adjacents)
            {
                if (ReferenceEquals(started, neighbor))
                {
                    Debug.WriteLine("Cycle detected");
                    order = [];
                    return false;
                }

                if (neighbor.State == GState.Unvisited)
                {
                    if (!TraverseBuildOrderWithLoopDetection(started, neighbor, order))
                    {
                        return false; // cycle detected break
                    }
                }
            }

            order.Add(node.Data.ToString());

            return true;
        }

        public static List<string> BuildOrder()
        {
            var projects = new List<string>
            { "a","b","c","d","e","f","g","k"};

            var dependencies = new List<List<string>>
            {
                new List<string>{"d","a"},
                new List<string>{"e","a"},
                new List<string>{"f","a"},
                new List<string>{"g","f"},
                new List<string>{"b","e"},
                new List<string>{"b","d"},

            };

            return BuildOrderDict(projects, dependencies);
        }

        public static List<string> buildOrder = new List<string>();

        public static List<string> BuildOrderDict(List<string> projects, List<List<string>> dependencies)
        {
            var projectsToAdd = projects.Where(it => !dependencies.Any(dep => dep.Contains(it))).ToList();
            buildOrder.AddRange(projectsToAdd);

            projects.RemoveAll(projectsToAdd.Contains);

            var dict = new Dictionary<string, bool>();
            foreach (var proj in projects)
            {
                dict.Add(proj, false);
            }
            var projToScan = new List<string>();
            var temp = new List<string>();
            do {
                projToScan = dict.Where(it => !it.Value).Select(it => it.Key).ToList();
                foreach (var proj in projToScan)
                {
                    var flg = false;
                    foreach (var item in dependencies)
                    {
                        if (item[0] == proj && !dict[item[1]])
                            flg = true;
                    }
                    if (!flg)
                    {
                        if (!dict[proj]) //dont add it again
                        {
                            dict[proj] = true;
                            buildOrder.Add(proj);
                        }
                    }
                }
                temp = dict.Where(it => !it.Value).Select(it => it.Key).ToList();
            } while (!projToScan.SequenceEqual(temp));

            if (dict.ContainsValue(false))
                throw new Exception("There is no build order");

            return buildOrder;

        }


        public static int Solution(int[] V, int[] A, int[] B)
        {
            var map = new Dictionary<int, Tuple<int, List<int>>>();
            HashSet<int> keysToDelete = new HashSet<int>();
            for (int i=0;i<B.GetLength(0);i++)
            {
                if (!map.ContainsKey(B[i]))
                    map.Add(B[i], new Tuple<int, List<int>>(1, new List<int> { A[i] }));
                else
                {
                    int freq = map[B[i]].Item1;
                    List<int> relations = map[B[i]].Item2;
                    if (!relations.Contains(A[i]))
                    {
                        relations.Add(A[i]);
                        freq++;
                    }
                    if (freq > 2)
                        keysToDelete.Add(B[i]);
                    map[B[i]] = new Tuple<int, List<int>>(freq, relations);
                }
            }
            var cloneMap = new Dictionary<int, Tuple<int, List<int>>>();
            foreach (var item in map)
            {
                if (keysToDelete.Contains(item.Key) || keysToDelete.Any(key => item.Value.Item2.Contains(key)))
                {
                    //don't include
                }
                else
                    cloneMap.Add(item.Key, item.Value);
            }

            for (int i=0; i<V.Length;i++)
            {
                if (!B.Contains(i))
                    cloneMap.Add(i, new Tuple<int, List<int>>(0, new List<int>()));
            }


            //var positions = new Tuple<int, int, int, int>(0,0,0,0);
            var scores = new Tuple<int, int, int, int>(-1, -1, -1, -1);
            foreach (var proj in cloneMap)
            {
                if (proj.Value.Item1 > 0 && proj.Value.Item1 < 2)
                {
                    if (V[proj.Value.Item2[0]] + V[proj.Key] > scores.Item1 + scores.Item2)
                    {
                        //positions = new Tuple<int, int, int, int>(proj.Value.Item2[0], proj.Value.Item2[1], positions.Item3, positions.Item4);
                        scores = new Tuple<int, int, int, int>(V[proj.Value.Item2[0]], V[proj.Key], scores.Item3, scores.Item4);
                    }
                }
                else if (proj.Value.Item1 >= 2) continue;
                else
                {
                    int min = Math.Min(scores.Item3, scores.Item4);
                    if (V[proj.Key] > min)
                        scores = new Tuple<int, int, int, int>(scores.Item1, scores.Item2, Math.Max(scores.Item3, scores.Item4), V[proj.Key]);
                }
            }
            int independentScores = 0;
            if (scores.Item3 > -1)
                independentScores += scores.Item3;
            if (scores.Item4 > -1)
                independentScores += scores.Item4;
            int dependentScores = scores.Item1 + scores.Item2;
            return Math.Max(independentScores, dependentScores);
        }
        /// <summary>
        /// Problem : 4.8
        /// Description : Design an algorithm and write code to find the first common ancestor
        /// of two nodes in a binary tree.Avoid storing additional nodes in a data structure.NOTE: This is not
        /// necessarily a binary search tree.
        /// </summary>
        public static bool TryFindCommonAncestorV2(out GTreeNode common)
        {
            var (first, second) = BuildTreeParentsV2();

            common = null;

            if (first is null || second is null) 
            {
                return false;
            }

            if (ReferenceEquals(first, second))
            {
                common = first;
                return true;
            }

            common = FindCommonAncestorInternal(first, second);

            return common is not null;
        }

        private static GTreeNode FindCommonAncestorInternal(GTreeNode first, GTreeNode second)
        {
            if (first is not null)
            {
                if (first.Status == GTreeNodeStatus.Visisted)
                {
                    return first;
                }

                first.Status = GTreeNodeStatus.Visisted;
                first = first.Parent;
            }

            if (second is not null)
            {
                if (second.Status == GTreeNodeStatus.Visisted)
                {
                    return second;
                }

                second.Status = GTreeNodeStatus.Visisted;
                second = second.Parent;
            }

            if (second is null &&  first is null)
            {
                throw new Exception("this should not happen or the tree structure is invalid.");
            }

            return FindCommonAncestorInternal(first, second);
        }

        public static (GTreeNode, GTreeNode) BuildTreeParentsV2()
        {
            var first = GTreeNode.Init(
                1, parent: GTreeNode.Init(
                    2, parent: GTreeNode.Init(
                        3, parent: GTreeNode.Init(4))));

            var second = GTreeNode.Init(
                5, parent: GTreeNode.Init(
                    6, parent: first.Parent.Parent));

            return (first, second);
        }

        public class CustomNode
        {
            public int data;
            public CustomNode parent;
            public CustomNode left;
            public CustomNode right;
            public bool visited;

            public CustomNode(int data)
            {
                this.data = data;
                visited = false;
                parent = null;
            }
        }
        public static CustomNode first;
        public static CustomNode second;
        public static void FirstCommonNode()
        {
            BuildTree();
            //optimization
            FirstCommonAncestorOptimized(first, second);

            //firstApproach
            FirstCommonAncestor(first);
            FirstCommonAncestor(second);
        }
        public static void BuildTree()
        {
            first = new CustomNode(1);
            first.parent = new CustomNode(2);
            first.parent.parent = new CustomNode(3);
            first.parent.parent.parent = new CustomNode(4);

            second = new CustomNode(5);
            second.parent = new CustomNode(6);
            second.parent.parent = first.parent.parent;
            second.parent.parent.parent = first.parent.parent.parent;
        }
        public static void FirstCommonAncestor(CustomNode node)
        {
            node.visited = true;

            if (node.parent == null)
                return; //base case 
            else
            {
                node = node.parent;
                if (!node.visited)
                    node.visited = true;
                else
                {
                    Console.WriteLine("Intersection Node : " + node.data);
                    return;
                }
            }
            FirstCommonAncestor(node);
        }

        public static void FirstCommonAncestorOptimized(CustomNode firstNode, CustomNode secondNode)
        {
            firstNode.visited = true;
            secondNode.visited = true;

            if (firstNode.parent == null && secondNode.parent == null)
                return; //base case 
            else if (firstNode.parent != null && secondNode.parent == null)
            {
                firstNode = firstNode.parent;
                if (!firstNode.visited)
                    firstNode.visited = true;
                else
                {
                    Console.WriteLine("Intersection Node : " + firstNode.data);
                    return;
                }
                FirstCommonAncestor(firstNode);
            }
            else if (firstNode.parent == null && secondNode.parent != null)
            {
                secondNode = secondNode.parent;
                if (!secondNode.visited)
                    secondNode.visited = true;
                else
                {
                    Console.WriteLine("Intersection Node : " + secondNode.data);
                    return;
                }
                FirstCommonAncestor(secondNode);
            }
            else
            {
                firstNode = firstNode.parent;
                if (!firstNode.visited)
                    firstNode.visited = true;
                else
                {
                    Console.WriteLine("Intersection Node : " + firstNode.data);
                    return;
                }
                secondNode = secondNode.parent;
                if (!secondNode.visited)
                    secondNode.visited = true;
                else
                {
                    Console.WriteLine("Intersection Node : " + secondNode.data);
                    return;
                }
                FirstCommonAncestorOptimized(firstNode, secondNode);
            }
        }

        public static CustomNode FirstCommonAncestorPreOrder()
        {
            while (first.parent != null)
            {
                first.visited = true;
                first = first.parent;
                if (commonAncestorPointer != null)
                    break;
                if (first.left.visited && !first.right.visited)
                    Traverse(first.right, second);
                else if (!first.left.visited && first.right.visited)
                    Traverse(first.left, second);
                else if (first.left.visited && first.right.visited)
                {
                    if (commonAncestorPointer == null)
                        throw new Exception("No Common Ancestor");
                    else
                        break;
                }
            }
            return commonAncestorPointer;
        }

        public static CustomNode commonAncestorPointer;

        public static void Traverse(CustomNode child, CustomNode target)
        {
            if (child == null)
                return;
            if (child == target)
            {
                commonAncestorPointer = child;
                return;
            }
            Traverse(child.left, target);
            Traverse(child.right, target);
        }

        /// <summary>
        /// Problem : 4.9
        /// Description : A binary search tree was created by traversing through an array from left to right
        /// and inserting each element.Given a binary search tree with distinct elements, print all possible
        /// arrays that could have led to this tree.
        /// EXAMPLE
        /// Input:
        /// 2,root 1,root.left 3,root.right
        /// Output: {2, 1, 3}, {2, 3, 1}
        /// </summary>
        /// 
        /// TO DO AGAIN
        public static List<LinkedListStruct> BSTSequences(CustomNode node)
        {
            List<LinkedListStruct> result = new List<LinkedListStruct>();

            if (node == null)
            {
                result.Add(new LinkedListStruct());
                return result;
            }

            LinkedListStruct prefix = new LinkedListStruct();
            prefix.Data = node.data;

            //Recurse on left and right subtrees
            List<LinkedListStruct> leftSequence = BSTSequences(node.left);
            List<LinkedListStruct> rightSequence = BSTSequences(node.right);

            //Weave together each list from the left and right sides
            foreach (var leftitem in leftSequence)
            {
                foreach (var rightitem in rightSequence)
                {
                    List<LinkedListStruct> weaved = new List<LinkedListStruct>();
                    WeaveLists(leftitem, rightitem, weaved, prefix);
                    result.AddRange(weaved);
                }
            }
            return result;
        }

        //weave lists together in all possible ways. 
        //This algorithm works by removing the head from one list, recursing and then doing the same thing with the other list.
        public static void WeaveLists(LinkedListStruct leftItem, LinkedListStruct rightItem, List<LinkedListStruct> results, LinkedListStruct prefix)
        {
            //One list is empty. Add remainder to [a cloned] prefix and store result

            if (leftItem.Next == null || rightItem.Next == null)
            {
            }
        }

        /// <summary>
        /// Problem : 4.10
        /// Description : Tl and T2 are two very large binary trees, with Tl much bigger than T2. Create an
        /// algorithm to determine ifT2 is a subtree of Tl.
        /// A tree T2 is a subtree of Tl if there exists a node n in Tl such that the subtree of n is identical to T2.
        /// That is, if you cut off the tree at node n, the two trees would be identical.
        /// 
        private static (GTreeNode, GTreeNode) BuildTreeAndSubtreeV2()
        {
            var second = GTreeNode.Init(3, left: GTreeNode.Init(2, left: GTreeNode.Init(1)), right: GTreeNode.Init(6, right: GTreeNode.Init(5)));
            var first = GTreeNode.Init(4, left: second);

            return (first, second);
        }

        public static bool CheckSubtreeV2()
        {
            var (t1Root, t2Root) = BuildTreeAndSubtreeV2();

            if (t1Root is null)
            {
                return false; // empty tree
            }

            if (t2Root is null)
            {
                return true; // empty sub tree 
            }

            return CheckSubTreeNodesInternal(t1Root.Left, t2Root) || CheckSubTreeNodesInternal(t1Root.Right, t2Root);
        }

        private static bool CheckSubTreeNodesInternal(GTreeNode t1Node, GTreeNode t2Node)
        {
            if (t1Node is null)
            {
                return false;
            }

            if (t1Node.Data == t2Node.Data)
            {
                var match = CheckSubTreeBodiesInternal(t1Node, t2Node);
                if (match)
                {
                    return true;
                }
            }

            return CheckSubTreeNodesInternal(t1Node.Left, t2Node) || CheckSubTreeNodesInternal(t1Node.Right, t2Node);
        }

        private static bool CheckSubTreeBodiesInternal(GTreeNode t1Node, GTreeNode t2Node)
        {
            if (t1Node is not null && t2Node is not null)
            {
                if (t1Node.Data == t2Node.Data)
                {
                    return CheckSubTreeBodiesInternal(t1Node.Left, t2Node.Left) && CheckSubTreeBodiesInternal(t1Node.Right, t2Node.Right);
                }
                else
                {
                    return false;
                }
            }

            if (t1Node is null && t2Node is null)
            {
                return true; //match
            }

            return false; //misalignment
        }

        public static bool CheckSubtree(CustomNode t1, CustomNode t2)
        {
            if (t2 == null) return true; //null tree is always a subtree
            if (t1 == null) return false; //big tree empty & subtree still not found
            if (t1.data == t2.data && CheckBodies(t1, t2))
                return true;
            return CheckSubtree(t1.left, t2) || CheckSubtree(t1.right, t2);
        }

        public static bool CheckBodies(CustomNode t1, CustomNode t2)
        {
            if (t1 == null && t2 == null)
                return true;
            if (t1 == null || t2 == null)
                return false;
            if (t1.data != t2.data)
                return false;

            return CheckBodies(t1.left, t2.left) && CheckBodies(t1.right, t2.right);
        }

        public static bool CheckSubTreeSimpleApproach(CustomNode t1, CustomNode t2)
        {
            StringBuilder st1 = new StringBuilder();
            StringBuilder st2 = new StringBuilder();

            GetOrderString(t1, st1);
            GetOrderString(t2, st2);

            return st1.ToString().IndexOf(st2.ToString()) != -1;
        }

        public static void GetOrderString(CustomNode t1, StringBuilder sb)
        {
            if (t1 == null)
            {
                sb.Append("X");
                return;
            }
            sb.Append(t1.data + " ");
            GetOrderString(t1.left, sb);
            GetOrderString(t1.right, sb);
        }

        /// <summary>
        /// Problem : 4.2
        /// </summary>
        public static void MinimalTree()
        {
            List<int> sortedList = new List<int> { 0, 1, 3, 5, 6, 8, 9, 12 };
        }
        public static CustomNode CreateBinarySearchTree(List<int> sortedList, int start, int end)
        {
            if (start > end) return null;
            int mid = (start+end)/ 2;
            CustomNode node = new CustomNode(sortedList[mid]);
            node.left = CreateBinarySearchTree(sortedList, start, mid - 1);
            node.left = CreateBinarySearchTree(sortedList, mid+1, end);

            return node;
        }

        /// <summary>
        /// Problem : 4.8
        /// </summary>
        public static void FirstCommonAncestor()
        {
            CustomNode root = new CustomNode(1);
        }
        public static CustomNode firstAncestor;
        public static CustomNode secondAncestor;

        public static CustomNode FCAHelper(CustomNode node)
        {
            if (node == null)
                return null;
            CustomNode temp = null;
            if (node == firstAncestor)
            {
                temp = firstAncestor;
            }
            else if (node == secondAncestor)
            {
                temp = secondAncestor;
            }

            CustomNode nodeLeft = FCAHelper(node.left);
            CustomNode nodeRight = FCAHelper(node.right);

            if (nodeLeft == null && nodeRight == null)
                return null;
            else if (temp != null)
                return temp;
            else if (nodeLeft != null && nodeRight != null)
                return node;
            return (nodeLeft != null) ? nodeLeft : nodeRight;
        }

        public static void TreeFromList(List<int> input)
        {
            // 1 2 3 4 5 6 7 8 9 10
            // 1 2 3 4 5
            input = input.OrderBy(it => it)?.ToList();
            var head = BSTHelper(input, 0, input.Count - 1);
            PreOrderLogging(head);
        }

        public static void PreOrderLogging(TreeNode head)
        {
            if (head == null)
                return;
            Console.WriteLine(head.data);
            PreOrderLogging(head.left);
            PreOrderLogging(head.right);
        }

        public static TreeNode BSTHelper(List<int> input, int start, int end)
        {
            if (start > end)
                return null;

            int mid = (start + end) / 2;

            var node = new TreeNode(input[mid]);

            node.left = BSTHelper(input,start,mid-1);

            node.right = BSTHelper(input, mid + 1, end);

            return node;
        }
    }

    public class TreeNode 
    {
        public int data;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int data)
        {
            this.data = data;
            this.left = null;
            this.right = null;
        }
    }
}
