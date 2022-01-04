using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.FrequentlyAskedQuestions
{
    public static class Interview
    {
        /// <summary>
        /// Find The First Common Ancestor of two given nodes with parent link
        /// </summary>

        public static Node FindCommonAncestor(Node first, Node second)
        {
            var NodeSet = new HashSet<Node>();
            if (first == second) return first;
            if (first == null && second != null) return null;
            if (first != null && second == null) return null;

            while (first.parent != null || second.parent != null)
            {
                first = first.parent;
                second = second.parent;
                if (NodeSet.Contains(first))
                    return first;
                else
                    NodeSet.Add(first);
                if (NodeSet.Contains(second))
                    return second;
                else
                    NodeSet.Add(second);
            }
            Node tempNode;
            if (first.parent == null) tempNode = second;
            else tempNode = first;

            while (tempNode.parent != null)
            {
                tempNode = tempNode.parent;
                if (NodeSet.Contains(tempNode))
                    return tempNode;
            }

            return null;
        }



        /// <summary>
        /// Find The First Common Ancestor of two given nodes without parent link
        /// Explanation : Traverse the tree from root (Post Order) and if you find the target bubble it up in temp
        /// if bubbling overwrites the first target then it means that the two nodes are under the same subtree.
        /// if not check the public booleans to confirm that both targets have been identified.
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(LogN)</SpaceComplexity>
        /// </summary>
        public static Node FindCommonAncestorWithoutParentLink(Node root, Node First, Node Second)
        {
            first = First; //first target
            second = Second; //second target
            if (first == second) return first;
            if (first == root || second == root) return root;

            Node LCA = Helper(root);
            if (firstFound && secondFound) return LCA; // if both targets have been identified return the commono node
            //otherwise return null
            return null;
        }

        public static Node first;
        public static Node second;
        public static bool firstFound;
        public static bool secondFound;

        public static Node Helper(Node node)
        {
            Node temp = null;
            if (node == null)
                return null;
            if (node == first)
            {
                firstFound = true;
                temp = node;
            }
            if (node == second)
            {
                secondFound = true;
                temp = node;
            }

            Node nodeLeft = Helper(node.left);
            Node nodeRight = Helper(node.right);

            if (temp != null) //if current node is a target bubble it
                return temp;

            if (nodeLeft != null && nodeRight != null) //if targets are under different subtrees then return the common node
                return node;

            return (nodeLeft != null) ? nodeLeft : nodeRight; //if you have found only one of them bubble it until you find the second
        }

        /// <summary>
        /// Find The First Common Ancestor of two given nodes without parent link in BST
        /// Explanation : TFor Binary search tree, while traversing the tree from top to bottom the first node which lies 
        /// in between the two numbers n1 and n2 is the LCA of the nodes, i.e. the first node n with the lowest depth 
        /// which lies in between n1 and n2 (n1<=n<=n2) n1 < n2, So just recursively traverse the BST in, 
        /// if node’s value is greater than both n1 and n2 then our LCA lies in the left side of the node,
        /// if it’s is smaller than both n1 and n2, then LCA lies on the right side.
        /// Otherwise, the root is LCA (assuming that both n1 and n2 are present in BST).
        /// <TimeComplexity>O(H) where H is the height of the tree</TimeComplexity>
        /// <SpaceComplexity>O(H) where H is the height of the tree</SpaceComplexity>
        /// <Hint>In order to get rid of SpaceComplexity you can follow an iterative solution --> O(1)</Hint>
        /// </summary>

        //public static Node FindCommonAncestorBST(Node root, Node First, Node Second)
        public static Node FindCommonAncestorBST()
        {
            Node root = new Node(20);
            root.left = new Node(8);
            root.right = new Node(22);
            root.left.left = new Node(4);
            root.left.right = new Node(19);
            root.left.right.left = new Node(10);
            root.right.right = new Node(70);

            Node First = new Node(19);
            Node Second = new Node(10);
            first = (First.data < Second.data) ? First : Second; //smaller ancestor
            second = (First.data < Second.data) ? Second : First; //greater ancestor
            if (first == second) return first;
            if (first == root || second == root) return root;

            if (first.data <= root.data && root.data <= second.data) return root;

            Node LCA = HelperBST(root);

            return LCA; // if both targets have been identified return the commono node
                        //otherwise return null

        }

        public static Node HelperBST(Node node)
        {
            if (node.data > first.data && node.data > second.data)
                return HelperBST(node.left);
            else if (node.data < first.data && node.data < second.data)
                return HelperBST(node.right);
            else if (first.data == node.data)
                return first;
            else if (second.data == node.data)
                return second;
            else if (first.data < node.data && node.data < second.data)
                return node;
            return null;
        }

        /// <summary>
        /// Print path from root to a given node in a binary tree
        /// <TimeComplexity>O(N) worst case the destination is the rightmost leaf</TimeComplexity>
        /// <SpaceComplexity>O(logn+K) logn for stack in memory and K the path in Stack</SpaceComplexity>
        /// </summary>
        public static void PathToNode(Node root, Node dest)
        {
            root = new Node(20);
            root.left = new Node(8);
            root.right = new Node(22);
            root.left.left = new Node(4);
            root.left.right = new Node(19);
            root.left.right.left = new Node(10);
            root.right.right = new Node(70);

            dest = root.left.right.left;

            if (root == null) return;

            PathToNodeHelper(root, dest, false);
            string pathToFollow = string.Empty;
            while (path.Count > 0)
            {
                pathToFollow += path.Pop().data.ToString() + " -> ";
            }
            Console.WriteLine(pathToFollow);
        }
        public static Stack<Node> path = new Stack<Node>();
        public static bool PathToNodeHelper(Node root, Node dest, bool flag)
        {

            if (root == null) return false;
            if (root == dest)
            {
                //Console.WriteLine("Destination path -> "+root.data);
                path.Push(root);
                return true;
            }
            flag = PathToNodeHelper(root.left, dest, flag);
            if (flag)
            {
                //Console.WriteLine("Destination path -> " + root.data);
                path.Push(root);
                return true;
            }
            flag = PathToNodeHelper(root.right, dest, flag);
            if (flag)
            {
                //Console.WriteLine("Destination path -> " + root.data);
                path.Push(root);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Print every path from root to leafs without using recursion
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(logn)</SpaceComplexity>
        /// </summary>
        public static void PrintRootToLeafPathsNoRecursion(Node root)
        {
            root = new Node(20);
            root.left = new Node(8);
            root.right = new Node(22);
            root.left.left = new Node(4);
            root.left.right = new Node(19);
            root.left.right.left = new Node(10);
            root.right.right = new Node(70);

            if (root == null) return;
            string tempPath = string.Empty;

            Dictionary<Node, Node> map = new Dictionary<Node, Node>();
            map.Add(root, null); //root has null parent

            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Node tempNode = stack.Pop();

                if (tempNode.right != null)
                {
                    stack.Push(tempNode.right);
                    map.Add(tempNode.right, tempNode);
                }

                if (tempNode.left != null)
                {
                    stack.Push(tempNode.left);
                    map.Add(tempNode.left, tempNode);
                }

                if (tempNode.left == null & tempNode.right == null)
                {
                    PrintTopToBottomPath(tempNode, map);
                }
            }

        }
        public static void PrintTopToBottomPath(Node currentNode, Dictionary<Node, Node> map)
        {
            Stack<Node> path = new Stack<Node>();
            path.Push(currentNode);
            while (currentNode != null)
            {
                currentNode = map[currentNode];
                if (currentNode != null)
                    path.Push(currentNode);
            }
            string pathToFollow = string.Empty;
            var sb = new StringBuilder();
            while (path.Count > 0)
            {
                Node tempToAdd = path.Pop();
                sb.Append(tempToAdd.data.ToString());
                sb.Append(" -> ");
            }
            Console.WriteLine("Path to follow : " + sb.ToString());
        }

        /// <summary>
        /// Print all nodes at odd levels of the tree
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(logn)</SpaceComplexity>
        /// </summary>
        public static void PrintNodesAtOddLevels()
        {
            var root = new Node(20);
            root.left = new Node(8);
            root.right = new Node(22);
            root.left.left = new Node(4);
            root.left.right = new Node(19);
            root.left.right.left = new Node(10);
            root.right.right = new Node(70);

            if (root == null)
                Console.WriteLine("Empty tree");

            PrintNodesAtOddLevelsHelper(root, 1);
        }
        public static void PrintNodesAtOddLevelsHelper(Node root, int level)
        {
            if (root == null) return;

            if (level % 2 > 0)
                Console.WriteLine(root.data);

            PrintNodesAtOddLevelsHelper(root.left, level + 1);
            PrintNodesAtOddLevelsHelper(root.right, level + 1);
        }

        /// <summary>
        /// Print all nodes at odd levels of the tree
        /// <TimeComplexity>O(MN)</TimeComplexity>
        /// <SpaceComplexity>O(N)</SpaceComplexity>
        /// </summary>
        public static bool CheckIfSubtree()
        {
            //tree 1
            var root1 = new Node(26);
            root1.right = new Node(3);
            root1.right.right = new Node(3);
            root1.left = new Node(10);
            root1.left.left = new Node(4);
            root1.left.left.right = new Node(30);
            root1.left.right = new Node(6);

            //tree 2
            var root2 = new Node(10);
            root2.right = new Node(6);
            root2.left = new Node(4);
            root2.left.right = new Node(30);

            //check if tree2 is subtree of tree1

            return IsSubtree(root1, root2);
        }

        public static bool IsSubtree(Node root1, Node root2)
        {
            if (root2 == null)
                return true;
            if (root1 == null)
                return false;
            if (AreIdentical(root1, root2)) return true;

            return IsSubtree(root1.left, root2) || IsSubtree(root1.right, root2);
        }

        public static bool AreIdentical(Node root1, Node root2)
        {
            if (root1 == null && root2 == null)
                return true;
            if (root1 == null || root2 == null)
                return false;

            return (root1.data == root2.data && AreIdentical(root1.left, root2.left) && AreIdentical(root1.right, root2.right));

        }

        public static List<string> PreRoot1 = new List<string>();
        public static List<string> PreRoot2 = new List<string>();
        public static List<string> InRoot1 = new List<string>();
        public static List<string> InRoot2 = new List<string>();

        /// <summary>
        /// Check if a binary tree is subtree of another binary tree
        /// Approach : PreOrder and InOrder both binary trees and create 2 arrays for each tree. If each of the 2 arrays
        /// of the second binary tree are subarrays of each of the 2 arrays of the first tree then the second is a subtree 
        /// of the first. To Avoid error in hierarchy and left/right children whenever you find a null node add a special char
        /// to the array example -> "%"
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(N)</SpaceComplexity>
        /// </summary>
        public static bool CheckIfSubtreeOptimized()
        {
            //tree1
            var root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);

            //tree 2
            var root2 = new Node(8);
            root2.left = new Node(4);
            root2.right = new Node(19);
            root2.right.left = new Node(10);

            if (root1 == null && root2 != null) return false;
            if (root1 != null && root2 == null) return true;

            PreOrder(root1, true);
            PreOrder(root2);
            InOrder(root1, true);
            InOrder(root2);

            return IsSubArray(PreRoot1, PreRoot2) && IsSubArray(InRoot1, InRoot2);
        }
        public static bool IsSubArray(List<string> arr1, List<string> arr2)
        {
            int counter = 0;
            for (var i = 0; i < arr1.Count; i++)
            {
                if (arr1[i] == arr2[0]) break;
                counter++;
            }
            bool breaks = false;
            if (counter < arr1.Count)
            {
                int pos2 = 0;
                while (pos2 < arr2.Count)
                {
                    if (arr1[counter] == arr2[pos2])
                    {
                        counter++;
                        pos2++;
                    }
                    else
                    {
                        breaks = true;
                        break;
                    }
                }
            }
            return (!breaks);
        }
        public static void PreOrder(Node root, bool IsRoot1 = false)
        {
            if (root != null)
            {
                if (IsRoot1)
                    PreRoot1.Add(root.data.ToString());
                else
                    PreRoot2.Add(root.data.ToString());
            }
            else
            {
                if (IsRoot1)
                    PreRoot1.Add("%");
                else
                    PreRoot2.Add("%");
                return;
            }
            PreOrder(root.left, IsRoot1);
            PreOrder(root.right, IsRoot1);
        }
        public static void InOrder(Node root, bool IsRoot1 = false)
        {
            if (root == null)
            {
                if (IsRoot1)
                    InRoot1.Add("%");
                else
                    InRoot2.Add("%");
                return;
            }

            InOrder(root.left, IsRoot1);
            if (root != null)
            {
                if (IsRoot1)
                    InRoot1.Add(root.data.ToString());
                else
                    InRoot2.Add(root.data.ToString());
            }
            InOrder(root.right, IsRoot1);
        }

        /// <summary>
        /// Check if a binary tree contains duplicate binary trees with size 2 or more
        /// Approach : Preorder the tree and create a global stringBuilder. When you find a duplicated node in sb then keep the 
        /// index of this. Keep traversing the tree and moving the index to check if the lower layers are continuation of the subtree
        /// <Hint>Google Interview Question</Hint>
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(LogN)</SpaceComplexity>
        /// </summary>
        public static bool CheckIfTreeContainsDuplicateSubTrees()
        {
            //tree1
            var root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);
            root1.right.left = new Node(21);
            root1.right.right = new Node(70);
            root1.right.right.left = new Node(8);
            root1.right.right.left.left = new Node(4);
            root1.right.right.left.right = new Node(19);
            root1.right.right.left.right.left = new Node(10);

            CheckIfTreeContainsDuplicateSubTreesHelp(root1,0);
            CheckIfTreeContainsDuplicateSubTreesHelper(root1);
            return (maxLevel >= 2) ? true : false;
        }
        public static StringBuilder sbChecker = new StringBuilder();
        public static bool flag = false;
        public static int counter = 0;
        public static int maxLevel = 0;
        public static int indexFound;

        /// This helper has a problem because it counts empty nodes ("%") as extra layers but identifies duplicated trees.
        public static void CheckIfTreeContainsDuplicateSubTreesHelper(Node root)
        {
            if (root == null)
            {
                if (flag)
                {
                    string sbCheck = sbChecker.ToString();
                    if (sbCheck[indexFound+1]=='%')
                    {
                        indexFound++;
                        counter++;
                        if (counter > maxLevel)
                            maxLevel++;
                    }
                    else
                    {
                        flag = false;
                        counter = 0;
                    }
                }
                else
                    sbChecker.Append("%");
                return;
            }
            if (!sbChecker.ToString().Contains(root.data.ToString()))
            {
                sbChecker.Append(root.data.ToString());
                flag = false;
                counter = 0;
            }
            else
            {
                if (!flag)
                {
                    indexFound = sbChecker.ToString().IndexOf(root.data.ToString());
                    flag = true;
                    counter++;
                    if (counter > maxLevel)
                        maxLevel++;
                }
                else
                {
                    int nextIndex = sbChecker.ToString().IndexOf(root.data.ToString());
                    if (nextIndex == indexFound + 1)
                    {
                        indexFound++;
                        counter++;
                        if (counter > maxLevel)
                            maxLevel++;
                    }
                }
            }
            CheckIfTreeContainsDuplicateSubTreesHelper(root.left);
            CheckIfTreeContainsDuplicateSubTreesHelper(root.right);
        }

        /// This helper handles the previous problem by passing the currentLayer to the nextNode and updates the maxlayer 
        /// only when the current layer exceeds the max
        public static void CheckIfTreeContainsDuplicateSubTreesHelp(Node root,int layer)
        {
            if (root == null)
            {
                if (flag)
                {
                    string sbCheck = sbChecker.ToString();
                    if (sbCheck[indexFound + 1] == '%')
                    {
                        indexFound++;
                        if (layer > maxLevel)
                            maxLevel++;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                    sbChecker.Append("%");
                return;
            }
            else if (!sbChecker.ToString().Contains(root.data.ToString()))
            {
                sbChecker.Append(root.data.ToString());
                flag = false;
                CheckIfTreeContainsDuplicateSubTreesHelp(root.left, 0);
                CheckIfTreeContainsDuplicateSubTreesHelp(root.right, 0);
            }
            else
            {
                if (!flag)
                {
                    indexFound = sbChecker.ToString().IndexOf(root.data.ToString());
                    flag = true;
                    if (layer > maxLevel)
                        maxLevel++;
                }
                else
                {
                    int nextIndex = sbChecker.ToString().IndexOf(root.data.ToString());
                    if (nextIndex == indexFound + 1)
                    {
                        indexFound++;
                        
                        if (layer > maxLevel)
                            maxLevel++;
                    }
                }
                CheckIfTreeContainsDuplicateSubTreesHelp(root.left, layer+1);
                CheckIfTreeContainsDuplicateSubTreesHelp(root.right, layer+1);
            }
        }

        /// <summary>
        /// Serialize a Binary Tree
        /// Approach : Create a stack and push the root. While the stack != empty pop the top item, push right and left children
        /// (respectively so you can respect the order) and keep every popped node in an array. Mark every null node with a special char
        /// like "#"
        /// Example : 
        /// Input :
        ///             20
        ///         8       22
        ///      4    19      70
        ///         10
        /// Output : "20,8,4,#,#,19,10,#,#,#,22,#,70,#,#"
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(N)</SpaceComplexity>
        /// </summary>
        public static string SerializeTree(Node root1)
        {
            //if (root1==null) return;
            //tree1
            root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);

            string[] sb = new string[15];
            Stack<Node> stack = new Stack<Node>();

            stack.Push(root1);
            int index = 0;
            while (stack.Count>0)
            {
                Node tempNode = stack.Pop();
                if (tempNode == null)
                    sb[index]="#";
                else
                {
                    sb[index] = tempNode.data.ToString();
                    //sb.Append(tempNode.data.ToString());
                    stack.Push(tempNode.right);
                    stack.Push(tempNode.left);
                }
                index++;
            }
            //string result = sb.ToString();
            string result = string.Join(",", sb);
            return result;
        }

        /// <summary>
        /// Deserialize a Binary Tree
        /// Approach : Keep a global index so you can iterate on the input list with the serialized binary tree while you're recursing
        /// in order to create the tree with respect to its initial structure. Create each node (inputList[index] -> index++) and recurse
        /// for left and right children until index>=inputList.Count. You have successfully re-created the initial tree
        /// 
        /// Example : 
        /// Input : "20,8,4,#,#,19,10,#,#,#,22,#,70,#,#"
        /// Output : 
        ///             20
        ///         8       22
        ///      4    19      70
        ///         10
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(logN)</SpaceComplexity>
        /// </summary>
        public static void Deserialize (string data) 
        {
            if (data == null || data == string.Empty)
                return;

            string[] sb = data.Split(",");
            var root = DeserializeHelper(sb);
            PreOrder(root);
        }

        public static int indexHelper = 0 ;

        public static Node DeserializeHelper(string[] data)
        {
            if (indexHelper >= data.Length) return null ;
            if (data[indexHelper] == "#") return null;
            var node = new Node(Convert.ToInt32(data[indexHelper].ToString()));
            indexHelper++;
            node.left = DeserializeHelper(data);
            indexHelper++;
            node.right = DeserializeHelper(data);

            return node;
        }

        /// <summary>
        /// Find Path For Rat In A Maze
        /// Approach : Find a path from (0,0) to destination (ex. (n-1,n-1)) when the rat in the maze can move only right and down
        /// Consider that 0's are dead blocks and you cannot step on them. 1's are valid blocks that can form a path.
        /// Example : 
        /// Input : 
        /// | 1 | 0 | 0 | 0 |
        /// | 1 | 1 | 0 | 1 |
        /// | 0 | 1 | 0 | 0 |
        /// | 1 | 1 | 1 | 1 |
        /// Output : 
        /// List<Tuple<int,int>> --> [(0,0),(1,0),(1,1),(2,1),(3,1),(3,2),(3,3)] path to reach the destination
        /// <TimeComplexity>O(2^(n^2))</TimeComplexity>
        /// <SpaceComplexity>O(n^2) for the output path if we use a [,] to re create the path</SpaceComplexity>
        /// </summary>
        public static void RatInMaze()
        {
            /*int[,] grid = new int[4, 4]
            {
                { 1,0,0,0},
                {1,1,0,1 },
                {0,1,0,0 },
                {1,1,1,1 }
            };*/
            int[,] grid = new int[4, 4]
            {
                { 1,1,1,1},
                {1,1,0,1 },
                {0,1,0,1 },
                {1,1,1,1 }
            };
            //source = [0,0] dest = [n-1,n-1]
            RatInMazeHelper(0, 0, new Tuple<int, int>(3, 3), grid);
        }

        public static int squaresSize = 4;
        public static List<Tuple<int, int>> paths = new List<Tuple<int, int>>();
        public static List<List<Tuple<int, int>>> differentPaths = new List<List<Tuple<int, int>>>();
        
        public static bool RatInMazeHelper(int row,int col, Tuple<int,int> dest, int[,] grid)
        {
            if (row >= squaresSize || col>=squaresSize)
                return false;
            if (row==dest.Item1 && col==dest.Item2)
            {
                //paths.Add(new Tuple<int, int>(row, col)); don't add it to the path cause you'll always be 1 entity behind at the backtrack.
                var tempPath = new List<Tuple<int, int>>();
                foreach (var p in paths)
                {
                    tempPath.Add(p);
                }
                tempPath.Add(new Tuple<int, int>(row, col));
                differentPaths.Add(tempPath);
                return true;
            }
            else
            {
                if (grid[row, col] == 1)
                {
                    paths.Add(new Tuple<int, int>(row, col));
                    bool flag1 = RatInMazeHelper(row, col + 1, dest, grid);
                    bool flag2 = RatInMazeHelper(row + 1, col, dest, grid);
                    if (paths.Count>0)
                        paths.RemoveAt(paths.Count - 1); //backtrack
                    return (flag1 || flag2);
                }
                else
                    return false;
            }
        }

        
        public static void LeftSideOfTree()
        {
            var root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            //root1.right.right.right = new Node(71);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.right = new Node(1000);
            root1.right.left = new Node(1001);
            root1.right.left.left = new Node(1002);

            //LeftSideOfTreeHelper(root1, 0);
            LeftSideOfTreeHelper(root1.left, 0);
            Console.WriteLine(root1.data);
            RightSideOfTreeHelper(root1.right, 0);
        }

        public static int MaxLayer = -1;
        public static void LeftSideOfTreeHelper(Node node, int layer)
        {
            if (node == null)
                return;
            if (layer>MaxLayer)
            {
                MaxLayer = layer;
                Console.WriteLine(node.data); 
            }
            layer++;
            LeftSideOfTreeHelper(node.left, layer);
            LeftSideOfTreeHelper(node.right, layer);
        }

        public static int RightMaxLayer = -1;
        public static void RightSideOfTreeHelper(Node node, int layer)
        {
            if (node == null)
                return;
            if (layer > RightMaxLayer)
            {
                RightMaxLayer = layer;
                Console.WriteLine(node.data);
            }
            layer++;
            RightSideOfTreeHelper(node.right, layer);
            RightSideOfTreeHelper(node.left, layer);
            
        }
    }
    
    public class Node 
    {
        public int data;
        public Node left;
        public Node right;
        public Node parent;

        public Node(int data)
        {
            this.data = data;
            this.left = this.right = null;
        }
    }
}
