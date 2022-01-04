using DeepDiveTechnicals.DataStructures.LinkedList;
using DeepDiveTechnicals.DataStructures.StacksAndQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public enum State { unvisited = 0, visiting, visited };

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

        /// <summary>
        /// Problem : 4.2
        /// Description : Given a sorted (increasing order) array with unique integer elements, write an
        ///algorithm to create a binary search tree with minimal height.
        ///
        /// Explanation : 
        /// To create a tree of minimal height we need to match the number of nodes in the left subtree to the number of nodes in the right subtree
        /// as much as possible. This means that we want the root to be the middle of the array, since half the elements (placed left) should be less than the root
        /// and half (placed right) should be greater than the root.
        /// </summary>
        public static NodeStruct MinimalTree(int[] array)
        {
            NodeStruct root = MinimalTree(array, 0, array.Length); // this will return the first array[mid] which is going to be the root of the BST
            return root;
        }
        public static NodeStruct MinimalTree(int[] array, int start, int end)
        {
            if (end < start)
                return null;

            int mid = (start + end) / 2;
            NodeStruct node = new NodeStruct(array[mid]);
            node.left = MinimalTree(array, start, mid - 1);
            node.right = MinimalTree(array, mid + 1, end);
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

            if (!ValidateBSTInOrderTraversal(root.left)) return false;

            if (root.data < minimumV && minimumV != null)
                return false;
            else
                minimumV = root.data;

            if (!ValidateBSTInOrderTraversal(root.right)) return false;

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
        /// Output: f, e, a, b, d, c
        /// </summary>
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

            projects.RemoveAll(item => projectsToAdd.Contains(item));

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

        /// <summary>
        /// Problem : 4.8
        /// Description : Design an algorithm and write code to find the first common ancestor
        /// of two nodes in a binary tree.Avoid storing additional nodes in a data structure.NOTE: This is not
        /// necessarily a binary search tree.
        /// </summary>
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
    }
}
