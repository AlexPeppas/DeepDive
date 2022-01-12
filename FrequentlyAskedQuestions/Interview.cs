using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

            return LCA; // if both targets have been identified return the common node
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

        public static Node firstNode;
        public static Node secondNode;
        
        public static void FirstCommonAncestor()
        {
            Node root = new Node(20);
            root.left = new Node(8);
            root.right = new Node(22);
            root.left.left = new Node(4);
            root.left.right = new Node(19);
            root.left.right.right = new Node(21);
            root.right.right = new Node(10);
            root.right.right.right = new Node(70);

            Node First = new Node(4);
            Node Second = new Node(21);
            firstNode = First;
            secondNode = Second;
           

            var nodeReturned = FirstCommonAncestorHelper(root);
            if (nodeReturned == null)
                Console.WriteLine("Not in the same tree");
            else
                Console.WriteLine(nodeReturned);
        }

        public static bool firstNodeFound = false;
        public static bool secondNodeFound = false;

        public static Node FirstCommonAncestorHelper(Node node)
        {
            if (node == null)
                return null;
            Node temp = null;
            if (node.data == firstNode.data)
            {
                temp = node;
                firstNodeFound = true;
            }
            else if (node.data == secondNode.data)
            {
                temp = node;
                secondNodeFound = true;
            }
         
            Node nodeLeft = FirstCommonAncestorHelper(node.left);
            Node nodeRight = FirstCommonAncestorHelper(node.right);

            if (nodeLeft != null && nodeRight != null)
                return node;

            if (temp != null)
                return temp;
            return (nodeLeft != null) ? nodeLeft : nodeRight;
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
        /// Check if a binary tree contains duplicate binary trees with size 2 or more
        /// Approach : Preorder the tree and create a global stringBuilder. When you find a duplicated node in sb then keep the 
        /// index of this. Keep traversing the tree and moving the index to check if the lower layers are continuation of the subtree
        /// This is an optimization of the previous 2 verisons because it doesnt count null nodes "#" as extra layer and it has a precise
        /// reset mechanism where if maxLayer<2 every time we reset we drawback to maxLayer = 0
        /// <Hint>Google Interview Question</Hint>
        /// <TimeComplexity>O(N)</TimeComplexity>
        /// <SpaceComplexity>O(LogN)</SpaceComplexity>
        /// </summary>
        public static bool TreeWithDuplicateSubTreesDepth2()
        {
            var root1 = new Node(7);
            root1.right = new Node(22);
            root1.right.right = new Node(7);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(9);
            root1.left.right.left = new Node(1);
            root1.right.left = new Node(21);

            root1.right.right.left = new Node(8);
            root1.right.right.left.left = new Node(4);
            root1.right.right.left.right = new Node(9);
            root1.right.right.left.right.left = new Node(1);

            TreeWithDuplicateSubTreesDepth2Helper(root1);
            return (maxLayer >= 2) ? true : false;
        }
        public static int currLayer = 0;
        public static int maxLayer = 0;
        public static bool underRevision = false;
        public static StringBuilder sb = new StringBuilder();
        public static int index = 0;

        public static void TreeWithDuplicateSubTreesDepth2Helper(Node node)
        {
            if (node == null)
            {
                if (underRevision)
                {
                    string toCheck = sb.ToString()[index].ToString();
                    if (toCheck == "#")
                    {
                    //doNothing
                    }    

                    else
                    {
                        currLayer = 0;
                        underRevision = false;
                        if (maxLayer < 2) maxLayer = 0;
                    }
                }
                sb.Append("#");
                return;
            }
            else if (!sb.ToString().Contains(node.data.ToString()))
            {
                currLayer = 0;
                if (underRevision) underRevision = false;
                if (maxLayer < 2) maxLayer = 0;
                sb.Append(node.data.ToString());
                TreeWithDuplicateSubTreesDepth2Helper(node.left);
                TreeWithDuplicateSubTreesDepth2Helper(node.right);
            }
            else if (underRevision)
            {
                string toCheck = sb.ToString()[index].ToString();
                if (node.data.ToString()==toCheck)
                {
                    if (currLayer > maxLayer) maxLayer = currLayer;
                    
                    currLayer++;
                    index++;
                    TreeWithDuplicateSubTreesDepth2Helper(node.left);
                    index++;
                    TreeWithDuplicateSubTreesDepth2Helper(node.right);
                }
                else
                {   // RESET
                    currLayer = 0;
                    underRevision = false;
                    if (maxLayer < 2) maxLayer = 0;
                    TreeWithDuplicateSubTreesDepth2Helper(node.left);
                    TreeWithDuplicateSubTreesDepth2Helper(node.right);
                }
                sb.Append(node.data.ToString());
            }
            else //it is contained
            {
                underRevision = true;
                index = sb.ToString().IndexOf(node.data.ToString());
                sb.Append(node.data.ToString());
                currLayer = 0; //initialize the root of the possible duplicated subTree
                index++;
                TreeWithDuplicateSubTreesDepth2Helper(node.left);
                index++;
                TreeWithDuplicateSubTreesDepth2Helper(node.right);
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

        /// <summary>
        /// Print Left and Right side of a Tree
        /// </summary>
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

        /// <summary>
        /// Clone A Graph
        /// Approach : Use a queue to store each node and its adjacents to perform a BFS on the incoming graph.
        /// Use a HashSet to check if the Node has already been checked. If it has already been traversed do not
        /// add it to the queue. For each incoming node wether it has been added to the hashet or not store his 
        /// adjacents list to the clone node so you can transfer the structure.
        /// <TimeComplexity>O(N)</TimeComplexity>
        
        public static void GraphClone()
        {
            #region Graph Seeding
            Graph graph = new Graph(5);
            graph.nodes[0] = new GraphNode("A",2);
            graph.nodes[1] = new GraphNode("B", 3);
            graph.nodes[2] = new GraphNode("C", 2);
            graph.nodes[3] = new GraphNode("D", 3);
            graph.nodes[4] = new GraphNode("E", 2);

            //graph.nodes[0].adjacents[0].data = graph.nodes[1].data;
            graph.nodes[0].adjacents[0] = graph.nodes[1];
            graph.nodes[0].adjacents[1] = graph.nodes[2];

            graph.nodes[1].adjacents[0] = graph.nodes[0];
            graph.nodes[1].adjacents[1]= graph.nodes[3];
            graph.nodes[1].adjacents[2]= graph.nodes[4];

            graph.nodes[2].adjacents[0]= graph.nodes[0];
            graph.nodes[2].adjacents[1]= graph.nodes[3];

            graph.nodes[3].adjacents[0]= graph.nodes[1];
            graph.nodes[3].adjacents[1]= graph.nodes[2];
            graph.nodes[3].adjacents[2]= graph.nodes[4];

            graph.nodes[4].adjacents[0]= graph.nodes[1];
            graph.nodes[4].adjacents[1]= graph.nodes[3];
            #endregion
            if (graph == null) return;
            GraphNode root = graph.nodes[0];
            HashSet<GraphNode> setAdded = new HashSet<GraphNode>();
            Queue<GraphNode> queue = new Queue<GraphNode>();

            Graph cloneGraph = new Graph(graph.nodes.Length);
        
            queue.Enqueue(root);

            int cloneIndex = 0;
            while (queue.Count>0)
            {
                GraphNode currentNode = queue.Dequeue();
                GraphNode cloneCurrentNode = new GraphNode(currentNode.data, currentNode.adjacents.Length);
                
                if (!setAdded.Contains(currentNode))
                {
                    
                    setAdded.Add(currentNode);
                }
                int NeighborCounter = 0;
                foreach (var neighb in currentNode.adjacents)
                {
                    cloneCurrentNode.adjacents[NeighborCounter] = neighb;
                    NeighborCounter++;
                    
                    if (!setAdded.Contains(neighb) && !queue.Contains(neighb))
                        queue.Enqueue(neighb);
                }
                cloneGraph.nodes[cloneIndex] = cloneCurrentNode;
                cloneIndex++;
            }
            PrintGraph(cloneGraph);
        }

        public static void PrintGraph(Graph cloneGraph)
        {
            foreach (var node in cloneGraph.nodes)
            {
                Console.WriteLine($"NODE : {node.data} AND ADJACENT NODES" + Environment.NewLine + "{");
                foreach (var item in node.adjacents)
                {

                    Console.WriteLine("   " + item.data + ",");
                }
                Console.WriteLine("}");
            }
        }

        /// <summary>
        /// Print All Permutations of a String
        /// Approach : Build the base case and then recurse over the incoming string (based on decrement index -- st.Length--) until you finally 
        /// build the list of all available permutaitons. Each permutation is the output of the previous layer and just append the current letter to 
        /// every possible position (for every item in the list of strings).
        
        public static void PrintAllPermutations(string st)
        {
            // abcd
            // d -> cd, dc -> bcd, cbd, cdb, bdc, dbc, dcb -> ...
            var coms = PrintAllPermutationsHelper(st,0,new List<string>());
        }
        
        public static List<string> PrintAllPermutationsHelper(string st, int index,List<string> combs)
        {
            if (index==st.Length)
            {
                combs.Add(" ");
                return combs;
            }
            combs = PrintAllPermutationsHelper(st, index + 1, combs);
            combs = Builder(combs, st[index].ToString());
            return combs;
        }

        public static List<string> Builder (List<string> combs,string letter)
        {
            if (combs.FirstOrDefault()==" ")
            {
                combs[0] = letter;
                return combs;
            }
            
            List<string> outPut = new List<string>();
            foreach (var item in combs)
            {
                int itemIndex = 0;
                foreach (var letters in item)
                {
                    StringBuilder sb = new StringBuilder();
                    string temp = item.Substring(0, itemIndex);
                    sb.Append(temp);
                    sb.Append(letter);
                    sb.Append(item.Substring(itemIndex));
                    outPut.Add(sb.ToString());

                    itemIndex++;
                }
                outPut.Add(item + letter);
            }
            return outPut;
        }
        /*
         * 1,1,1,1
         * 0,1,0,1
         * 1,1,0,1
         * 0,1,1,1
         */
        public static void RatWithTuples()
        {
            int[,] maze = new int[4, 4]
            {
                {1,1,1,1 },
                {0,1,0,1 },
                {1,1,0,1 },
                {0,1,1,1 }
            };
            Tuple<int, int> source = new Tuple<int, int>(0, 0);
            Tuple<int, int> dest = new Tuple<int, int>(3, 3);
            RatWithTuplesHelper(maze, source, dest, new List<Tuple<int, int>>());
        }
        public static List<List<Tuple<int, int>>> allPaths = new List<List<Tuple<int, int>>>();
        public static void RatWithTuplesHelper(int[,] maze, Tuple<int,int> src, Tuple<int, int> dest, List<Tuple<int, int>> path)
        {
            if (src.Item1 > dest.Item1 || src.Item2 > dest.Item2) return;
            if (src.Item1 < 0 || src.Item2 < 0) return;
            if (src.Item1 == dest.Item1 && src.Item2 == dest.Item2)
            {
                
                var clonePath = new List<Tuple<int, int>>();
                foreach (var item in path)
                {
                    clonePath.Add(item);
                }
                clonePath.Add(dest);
                allPaths.Add(clonePath);
            }
            else
            {
                if (maze[src.Item1,src.Item2] == 1)
                {
                    path.Add(new Tuple<int, int>(src.Item1, src.Item2));
                    RatWithTuplesHelper(maze, new Tuple<int, int>(src.Item1, src.Item2+1), dest, path);
                    RatWithTuplesHelper(maze, new Tuple<int, int>(src.Item1+1, src.Item2), dest, path);
                    if (path.Count > 0) path.RemoveAt(path.Count - 1);
                }
            }
        }

        /// <summary>
        /// Write a method to sort an array of strings so that all tne anagrnms are next to each other.
        /// <Time>O(n*s*logs) slogs for sorting each string and n for sorting each string</Time>
        /// <Space>O(n+m) where n is the output list and m is the number of records of hashmap because 
        /// m can have just one key and every other word of the list be containted in its list value cause it may be 
        /// an array of anagrams</Space>
        /// </summary>
        public static List<string> GroupAnagrams(List<string> input)
        {
            input = new List<string> { "dog", "cat", "pizza", "god", "odg", "tac", "taco" };
            var map = new Dictionary<string, List<string>>();
            foreach (var item in input)
            {
                string sortedItem = SortString(item);
                if (map.ContainsKey(sortedItem))
                    map[sortedItem].Add(item);
                else
                    map.Add(sortedItem, new List<string> { item });
            }
            var outputList = new List<string>();
            foreach (var item in map)
            {
                foreach (var val in item.Value)
                {
                    outputList.Add(val);
                }
            }
            return outputList;
        }
        public static string SortString(string input)
        {
            var temp = input.ToCharArray();
            Array.Sort(temp);
            return new string(temp);
        }

        /// <summary>
        /// Approach the classic Lowest Common Ancestor on a typical Binary Tree. What if one of the given nodes is not contained in the tree ? Then there is no a common ancestor 
        /// so handle this scenario with two public booleans firstFound and secondFound
        /// </summary>
        public static void LowestCommonAncestor()
        {
            var root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);

            fNod = root1.left.right.left; //10
            sCnod = root1.left; //8
            var LCA = LowestComHelper(root1);
            if (sFound && fFound) Console.WriteLine($"Lowest Common Ancestor -> {LCA.data}");

            fNod = root1.left.right.left; //10
            sCnod = root1.left.left; // 4
            sFound = false;
            fFound = false;
            LCA = LowestComHelper(root1);
            if (sFound && fFound) Console.WriteLine($"Lowest Common Ancestor -> {LCA.data}");

            fNod = root1.left.right.left; //10
            sCnod = root1.right.right; // 70
            sFound = false;
            fFound = false;
            LCA = LowestComHelper(root1);
            if (sFound && fFound) Console.WriteLine($"Lowest Common Ancestor -> {LCA.data}");
            /*
             *          20
             *       8      22
             *    4    19      70
                     10
             */

        }

        public static Node fNod;
        public static Node sCnod;
        public static bool fFound = false;
        public static bool sFound = false;

        public static Node LowestComHelper(Node node)
        {
            if (node == null)
            {
                return null;
            }
            Node temp = null;
            if (node == fNod)
            {
                temp = fNod;
                fFound = true;
            }
            else if (node == sCnod)
            {
                temp = sCnod;
                sFound = true;
            }
            Node nodeLeft = LowestComHelper(node.left);
            Node nodeRight = LowestComHelper(node.right);
            if (nodeLeft == null && nodeRight == null && temp == null)
            {
                return null;
            }
            else if (temp != null)
            {
                return temp;
            }
            else if (nodeLeft != null && nodeRight != null) return node;
            return (nodeLeft == null) ? nodeRight : nodeLeft;

        }

        /// <summary>
        /// Perform Cycle Detection in Directed Graph
        /// Approach : Use a queue and a stringbuilder. BFS on graph and build the stringbuild foreach current node and neighbor. If the node is included in stringbuilder
        /// keep the index and traverse on its neighbors (typical BFS) if you build a sequence which is already included in you SB then you're in a cycle.
        /// </summary>
        public static bool CyclesInDirectedGraph()
        {
            #region Graph Seeding
            Graph graph = new Graph(4);
            graph.nodes[0] = new GraphNode("A", 2);
            graph.nodes[1] = new GraphNode("B", 1);
            graph.nodes[2] = new GraphNode("C", 2);
            graph.nodes[3] = new GraphNode("D", 1);
            
            //nodeA
            graph.nodes[0].adjacents[0] = graph.nodes[1]; //B
            graph.nodes[0].adjacents[1] = graph.nodes[2]; //C
            //nodeB
            graph.nodes[1].adjacents[0] = graph.nodes[2]; //C
            //nodeC
            graph.nodes[2].adjacents[0] = graph.nodes[3]; //D
            graph.nodes[2].adjacents[1] = graph.nodes[0]; //A
            //nodeD
            //graph.nodes[3].adjacents[0] = graph.nodes[3]; //D

            #endregion

            GraphNode node = graph.nodes[0];
            Queue<GraphNode> queue = new Queue<GraphNode>();
            StringBuilder sb = new StringBuilder();
            queue.Enqueue(node);
            bool contains = false;
            bool underRevision = false;
            int index = 0;
            int count = 0;
            while (queue.Count>0)
            {
                var tempNode = queue.Dequeue();
                if (!sb.ToString().Contains(tempNode.data))
                {
                    sb.Append(tempNode.data);
                    underRevision = false;
                }
                else
                {
                    if (!underRevision)
                    {
                        count = 0;
                        index = sb.ToString().IndexOf(tempNode.data);
                        underRevision = true;
                        sb.Append(tempNode.data);
                    }
                    else//if (underRevision)
                    {
                        if (sb.ToString()[index+1].ToString()==tempNode.data)
                        {
                            index++;
                            count++;
                            if (count > 2)
                            { contains = true; break; }
                        }
                        else
                        {
                            if (count > 2)
                            { contains = true; break; }
                            //RESET
                            index = 0;
                            count = 0;
                            underRevision = false;
                        }
                        sb.Append(tempNode.data);
                    }
                }
                foreach (var neighbor in tempNode.adjacents)
                {
                    if (neighbor == null) break;
                    if (neighbor == tempNode)
                    {
                        contains = true;
                        break;
                    }
                    if (!queue.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
            return contains;
        }

        /// <summary>
        /// Perform Cycle Detection in Directed Graph
        /// Approach : Use a stack and perform DFS with recursion on the root. If you end up adding a node in the stack which is already contained while you're in recursion (memory stack)
        /// then you have entered a cycle.
        /// </summary>
        public static bool CyclesInDirectedGraphDFS()
        {
            #region Graph Seeding
            Graph graph = new Graph(4);
            graph.nodes[0] = new GraphNode("A", 2);
            graph.nodes[1] = new GraphNode("B", 1);
            graph.nodes[2] = new GraphNode("C", 2);
            graph.nodes[3] = new GraphNode("D", 1);

            //nodeA
            graph.nodes[0].adjacents[0] = graph.nodes[1]; //B
            graph.nodes[0].adjacents[1] = graph.nodes[2]; //C
            //nodeB
            graph.nodes[1].adjacents[0] = graph.nodes[2]; //C
            //nodeC
            graph.nodes[2].adjacents[0] = graph.nodes[3]; //D
            graph.nodes[2].adjacents[1] = graph.nodes[0]; //A
            //nodeD
            //graph.nodes[3].adjacents[0] = graph.nodes[3]; //D

            #endregion

            GraphNode node = graph.nodes[0];
            Stack<GraphNode> stack = new Stack<GraphNode>();
            CyclesHelperDetection(node, stack);
            return cycleDetected;
        }
        public static bool cycleDetected = false;
        public static void CyclesHelperDetection(GraphNode node , Stack<GraphNode> stack)
        {
            if (cycleDetected) return;
            if (node == null)
                return;
            if (!stack.Contains(node))
            {
                stack.Push(node);
                foreach (var neighbor in node.adjacents)
                {
                    if (neighbor != null)
                    {
                        CyclesHelperDetection(neighbor, stack);
                        if (cycleDetected) return;
                        if (stack.Count>0) stack.Pop(); // backTrack
                    }
                }
            }
            else
            {
                cycleDetected = true;
                return;
            }
        }

        /// <summary>
        /// Sort a Nearly Sorted (K-Sorted) Array
        /// Approach : For each value keep it in a current var and its previous value as well. Check the next k elements and find this which is previous <= valueToSwap < current
        /// When you target the minimum valueToSwap of all k elements swap it and add it to the hashset. When you find it again while continuing with the while loop, ignore it.
        /// <Time>O(nk)</Time>
        /// </summary>
        public static void KSortedArray()
        {

            List<int> karray = new List<int>
            {6, 5, 3, 2, 8, 10, 9};
            HashSet<int> set = new HashSet<int>();
            int k = 3;
            for (int i=0;i<karray.Count-1;i++) //do not check the last item it must be sorted by the previous 4
            {
                int current = karray[i];
                if (set.Contains(current)) continue;
                int prev;
                prev = (i == 0) ? -1 : karray[i - 1];
                int cloneK = 1;
                int min = current;
                int position = -1;
                while (cloneK<=k)
                {
                    if (cloneK+i<karray.Count)
                    {
                        int temp = karray[i+cloneK];
                        if (temp<min && temp>= prev)
                        {
                            min = temp;
                            position = i + cloneK;
                        }
                    }
                    else
                    {
                        //out of index
                        break;
                    }
                    cloneK++;
                }
                if (position != -1)
                {
                    //swap
                    int swapTemp = current;
                    set.Add(swapTemp);
                    set.Add(karray[position]);
                    karray[i] = karray[position];
                    karray[position] = swapTemp;
                }
            }
        }

        /// <summary>
        /// ASKED BY TWITTER
        /// 1. (Boggle–like a question) In a 2D array (M x N, in the given ex. 3×3) of numbers,
        /// find the strictly increasing path from the specified origin cell (1,0) to the specified destination cell (0, 2).
        /// The array may contain duplicates, and the solution should work with the dups. 
        /// <Approach>
        /// Start from (1,0) in matrix. Every time split in three different directions. Right , Upwards, Downwoards if the 
        /// next neighbor is > current. If it is add the current value to the path and jump on the neighbor.
        /// Keep doing until you hit the destination or build one of the base cases (out of index i, out of index j, or no neighbor
        /// satisfy the greate (neighbor>current) condition).
        /// If you have multiple paths with equal maxSum then keep them all in a List<List<Tuple<int,int>>> so you can provide all 
        /// possible paths.
        /// Upon backtrack POP the top of the list when you finish with all neighbor jumps. Handle the scenario that you have backtracked 
        /// to the first position which is going to be at index=0 (DO NOT POP).
        /// </Approach>
        /// </summary>
        public static void StrictlyIncreasingPathTwitter()
        {
            int[,] matrix = new int[3, 3]
            {
                { 0,0,8},
                { 1,4,7},
                { 4,5,6}
            };
            StrictlyIncreasingPathTwitterHelper(matrix, position, new List<Tuple<int, int>>());
        }
        public static Tuple<int, int> position = new Tuple<int, int>(1, 0);
        public static Tuple<int, int> destination = new Tuple<int, int>(0, 2);
        public static List<List<Tuple<int, int>>> LongestPaths = new List<List<Tuple<int, int>>>();
        public static int pathMaxSum = 0;

        public static void StrictlyIncreasingPathTwitterHelper(int[,] matrix,Tuple<int,int> position, List<Tuple<int,int>> positions)
        {
            if (position.Item1 == destination.Item1 && position.Item2 == destination.Item2)
            {
                if (pathMaxSum == 0)
                {
                    List<Tuple<int, int>> tempList = new List<Tuple<int, int>>();
                    foreach (var item in positions)
                    {
                        pathMaxSum += matrix[item.Item1, item.Item2];
                        tempList.Add(item);
                    }
                    pathMaxSum += matrix[destination.Item1, destination.Item2];
                    tempList.Add(destination);
                    LongestPaths.Add(tempList);
                }
                else
                {
                    int max = 0;
                    List<Tuple<int,int>> tempList = new List<Tuple<int, int>>();
                    foreach (var item in positions)
                    {
                        max += matrix[item.Item1, item.Item2];
                        tempList.Add(item);
                    }
                    max+= matrix[destination.Item1, destination.Item2];
                    tempList.Add(destination);
                    if (max>=pathMaxSum)
                    {
                        pathMaxSum = max;
                        LongestPaths.Add(tempList);
                    }
                    else
                    {
                        //ignore this path;
                    }
                }
            }
            if (position.Item1 >= matrix.GetLength(0) || position.Item1 < 0)
                return; //out of bounds row
            if (position.Item2 >= matrix.GetLength(1) || position.Item2 < 0)
                return; // out of bounds col
            if (position.Item1 + 1 < matrix.GetLength(0))
            {
                if (matrix[position.Item1, position.Item2] < matrix[position.Item1 + 1, position.Item2])
                {
                    positions.Add(position);
                    StrictlyIncreasingPathTwitterHelper(matrix, new Tuple<int, int>(position.Item1 + 1, position.Item2), positions);
                }
            }
            if (position.Item2 + 1 < matrix.GetLength(1))
            {
                if (matrix[position.Item1, position.Item2] < matrix[position.Item1, position.Item2+1])
                {
                    positions.Add(position);
                    StrictlyIncreasingPathTwitterHelper(matrix, new Tuple<int, int>(position.Item1, position.Item2+1), positions);
                }
            }
            if (position.Item1-1>=0)
            {
                if (matrix[position.Item1, position.Item2] < matrix[position.Item1-1, position.Item2])
                {
                    positions.Add(position);
                    StrictlyIncreasingPathTwitterHelper(matrix, new Tuple<int, int>(position.Item1-1, position.Item2), positions);
                }
            }
            if (positions.Count>0)
                positions.RemoveAt(positions.Count - 1);
            return;
        }
        /// <summary>
        /// ASKED BY MICROSOFT
        /// Start from (0,0) in a grid. Your robot starts moving forward. Every time it hits a blocking position (=-1) or
        /// out of bounds rotate 90degrees and keep moving.
        /// When you detect a cycle throw an exception with cycle detection and print how many blocks in the grid it has visited.
        /// </summary>
        public static void FindPathRobotInGridWithCycle()
        {
            int[,] maze = new int[3, 4]
            {
                { 1,1,-1,1},
                {-1,1,-1,-1},
                { 1,1,1,1}
            };
            var roboPos = new RoboPosition(0,0);
            FindPathRobotInGridWithCycleHelper(roboPos.row, roboPos.col, maze);
        }

        public static void FindPathRobotInGridWithCycleHelper(int row, int col, int[,] maze)
        {
            //var roboPosition = new RoboPosition(row, col);
            if (row >= maze.GetLength(0) || row < 0)
            {
                if (Angle < 270)
                    Angle += 90;
                else
                    Angle = 0;
                return;
            }//out of row bounds
            if (col >= maze.GetLength(1) || col < 0)
            {
                if (Angle < 270)
                    Angle += 90;
                else
                    Angle = 0;
                return;
            }//out of col bounds
            if (maze[row, col] == -1)
            {
                if (Angle < 270)
                    Angle += 90;
                else
                    Angle = 0;
                return ;
            }
            /*if (RoboPositionWithDirection.ContainsKey(roboPosition))
            {
                if (RoboPositionWithDirection[roboPosition].Contains((Directions)Angle))
                    throw new Exception($"Cycle Detected. Max Squares Visited -> {maxSquaresVisited}");
                RoboPositionWithDirection[roboPosition].Add((Directions)Angle);
            }
            else RoboPositionWithDirection.Add(roboPosition, new List<Directions> { (Directions)Angle });*/
            if (RoboPositionWithDirection.ContainsKey(new Tuple<int,int>(row,col)))
            {
                if (RoboPositionWithDirection[new Tuple<int, int>(row, col)].Contains((Directions)Angle))
                    throw new Exception($"Cycle Detected. Max Squares Visited -> {maxSquaresVisited}");
                RoboPositionWithDirection[new Tuple<int, int>(row, col)].Add((Directions)Angle);
            }
            else RoboPositionWithDirection.Add(new Tuple<int, int>(row, col), new List<Directions> { (Directions)Angle });
            maxSquaresVisited++;
            for (int i=0;i<4;i++)
            { //try 4 directions
                if (Angle == 0)
                    FindPathRobotInGridWithCycleHelper(row - 1, col, maze);
                else if (Angle == 90)
                    FindPathRobotInGridWithCycleHelper(row, col + 1, maze);
                else if (Angle == 180)
                    FindPathRobotInGridWithCycleHelper(row + 1, col, maze);
                else if (Angle == 270)
                    FindPathRobotInGridWithCycleHelper(row, col - 1, maze);
            }
        }
        public class RoboPosition
        {
            public int row;
            public int col;

            public RoboPosition(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }
        public static int Angle = 90;
        public static int maxSquaresVisited = 0;
        public enum Directions
        {
            Upward = 0,
            Forward = 90,
            Downward = 180,
            Backward = 270
        };
        /*public static Dictionary<RoboPosition, List<Directions>> RoboPositionWithDirection
        = new Dictionary<RoboPosition, List<Directions>>();*/
        public static Dictionary<Tuple<int, int>, List<Directions>> RoboPositionWithDirection
        = new Dictionary<Tuple<int, int>, List<Directions>>();

    }
    /// <summary>
    /// LRU CACHE DESIGN
    /// </summary>
    public class LRU
    {
        public Dictionary<int, DoubleLinkedListNode> LRUmap;
        private int maxSize;

        public LRU(int maxSize)
        {
            LRUmap = new Dictionary<int, DoubleLinkedListNode>();
            this.maxSize = maxSize;
        }

        public string Get(int key)
        {
            if (!LRUmap.ContainsKey(key))
            {
                return null;
            }
            else
            {
                var tempNode = LRUmap[key];
                string value = tempNode.Value;
                //cases
                if (tempNode.Child == null) // first node of the linked list
                {
                    var newTail = tempNode.Parent;
                    tempNode.Parent = null;

                    newTail.Child = null;
                    newTail.Parent = DoubleLinkedListStruct.Tail.Parent;
                    if (newTail.Parent == null) newTail.Parent = tempNode; //just two nodes in the linked list
                    DoubleLinkedListStruct.Tail = newTail;
                    
                    var prevHead = DoubleLinkedListStruct.Head;
                    DoubleLinkedListStruct.Head.Parent = tempNode; //update the parent of the head pointer
                    DoubleLinkedListStruct.Head = tempNode; //update the head pointer 
                    DoubleLinkedListStruct.Head.Child = prevHead; //double link it
                }
                else if (tempNode.Parent == null)
                {
                    //if it's null then it's already at the end of the double linked list
                    //which means it has been most recently accessed so do nothing
                }
                else
                {
                    tempNode.Child.Parent = tempNode.Parent;
                    tempNode.Parent.Child = tempNode.Child;

                    var prevHead = DoubleLinkedListStruct.Head;
                    DoubleLinkedListStruct.Head.Parent = tempNode; //update the parent of the head pointer
                    DoubleLinkedListStruct.Head = tempNode; //update the head pointer 
                    DoubleLinkedListStruct.Head.Child = prevHead; //double link it
                }
                return value;
            }
        }

        public void Add (int key, string value)
        {
            if (!LRUmap.ContainsKey(key))
            {
                if (LRUmap.Count() >= this.maxSize)
                {
                    //if it exceeds its size delete the least recently used node from cache
                    //and update double linked list by removing the current tail and updating with the new.
                    var nodeToDelete = DoubleLinkedListStruct.Tail;
                    var nodesKey = nodeToDelete.key;
                    LRUmap.Remove(nodesKey);
                    DoubleLinkedListStruct.Tail.Child.Parent = null;
                    DoubleLinkedListStruct.Tail = DoubleLinkedListStruct.Tail.Child;
                }

                var newNode = new DoubleLinkedListNode(value,key);
                //DoubleLinkedListStruct.Nodes.Add(newNode);
                LRUmap.Add(key, newNode);
                if (DoubleLinkedListStruct.Head == null)
                {
                    DoubleLinkedListStruct.Head = newNode;
                    if (DoubleLinkedListStruct.Tail == null) //struct is empty
                        DoubleLinkedListStruct.Tail = newNode;
                }
                else
                {
                    var prevHead = DoubleLinkedListStruct.Head;
                    DoubleLinkedListStruct.Head.Parent = newNode; //update the parent of the head pointer
                    DoubleLinkedListStruct.Head = newNode; //update the head pointer 
                    DoubleLinkedListStruct.Head.Child = prevHead; //double link it
                }
            }
            else throw new Exception("This key is already contained");
        }

        public bool Contains(int key)
        {
            return (LRUmap.ContainsKey(key)) ? true : false;
        }

        public int Count()
        {
            return LRUmap.Count();
        }
    }

    //PROPOSED CLASS FOR THE FINAL IMPLEMENTATION AS AN OPTIMIZATION
    public static class DoubleLinkedListStruct2 
    {
        public static DLNode Head;

        public static DLNode Tail;

        public class DLNode
        {
            public int key;
            public string Value;
            public DoubleLinkedListNode Parent;
            public DoubleLinkedListNode Child;
            public DLNode(string Value, int key)
            {
                this.Value = Value;
                this.key = key;
                this.Parent = null;
                this.Child = null;
            }
        }
    }

    //CURRENT ARCHITECTURE USED
    public static class DoubleLinkedListStruct
    {
        //public static HashSet<DoubleLinkedListNode> Nodes;
        public static DoubleLinkedListNode Head;
        public static DoubleLinkedListNode Tail;
    }

    public class DoubleLinkedListNode
    {
        public int key;
        public string Value;
        public DoubleLinkedListNode Parent;
        public DoubleLinkedListNode Child;
        public DoubleLinkedListNode(string Value,int key)
        {
            this.Value = Value;
            this.key = key;
            this.Parent = null;
            this.Child = null;
        }
    }


    public class Graph
    {
        public GraphNode[] nodes;
        public Graph(int nodesNo)
        {
            nodes = new GraphNode[nodesNo];
        }
        
    }
    public class GraphNode 
    {
        public string data;
        public GraphNode[] adjacents;

        public GraphNode(string data,int nodesNo)
        {
            this.data = data;
            this.adjacents = new GraphNode[nodesNo];
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
