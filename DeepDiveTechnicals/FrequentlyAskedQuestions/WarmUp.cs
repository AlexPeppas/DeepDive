using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.FrequentlyAskedQuestions
{
    public class WarmUp
    {
        public static Node firstLCA = null;
        public static Node secondLCA = null;
        public static bool firstLCAFound = false;
        public static bool secondLCAFound = false;
        public static void LCA()
        {
            var root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);
            /*
           *          20
           *       8      22
           *    4    19      70
                   10
           */
            firstLCA = new Node(4);
            secondLCA = new Node(10);
            var LCA = LCAHelper(root1);
            if (firstLCAFound && secondLCAFound)
                Console.WriteLine("First Common Ancestor " + LCA.data);
            else
                Console.WriteLine("There is no Common Ancestor");

            firstLCAFound = false;
            secondLCAFound = false;
            firstLCA = new Node(70);
            secondLCA = new Node(10);
            LCA = LCAHelper(root1);
            if (firstLCAFound && secondLCAFound)
                Console.WriteLine("First Common Ancestor " + LCA.data);
            else
                Console.WriteLine("There is no Common Ancestor");

            firstLCAFound = false;
            secondLCAFound = false;
            firstLCA = new Node(22);
            secondLCA = new Node(70);
            LCA = LCAHelper(root1);
            if (firstLCAFound && secondLCAFound)
                Console.WriteLine("First Common Ancestor " + LCA.data);
            else
                Console.WriteLine("There is no Common Ancestor");
        }
        public static Node LCAHelper(Node node)
        {
            Node tempNode = null;
            if (node == null)
            {
                return null;
            }
            else if (node.data == firstLCA.data)
            {
                tempNode = node;
                firstLCAFound = true;
            }
            else if (node.data == secondLCA.data)
            {
                tempNode = node;
                secondLCAFound = true;
            }
            var leftNode = LCAHelper(node.left);
            var rightNode = LCAHelper(node.right);
            if (tempNode != null)
                return tempNode;
            if (leftNode != null && rightNode != null)
                return node;
            return (leftNode != null) ? leftNode : rightNode;
        }

        public static int[] QuickSort()
        {
            int[] arr = new int[6] { 1, 3, 9, 7, 5, 4 };

            QuickSortHelper(0, arr.GetLength(0) - 1, arr);
            return arr;
        }

        public static void QuickSortHelper(int low, int high, int[] arr)
        {
            if (low >= high)
                return;

            int pivot = arr[(low + high) / 2];
            var index = Partition(arr, pivot, low, high); // build items around partition (middle element)

            QuickSortHelper(low, index - 1, arr); //continue for the left and right sub-arrays
            QuickSortHelper(index, high, arr);
        }

        public static int Partition(int[] arr, int partitionKey, int leftPntr, int rightPntr)
        {
            while (leftPntr <= rightPntr)
            {
                while (arr[leftPntr] < partitionKey)
                {
                    leftPntr++;
                }

                while (arr[rightPntr] > partitionKey)
                {
                    rightPntr--;
                }
                if (leftPntr <= rightPntr)
                {
                    //swap
                    int temp = arr[rightPntr];
                    arr[rightPntr] = arr[leftPntr];
                    arr[leftPntr] = temp;
                    rightPntr--;
                    leftPntr++;
                }
            }

            return leftPntr;
        }

        public static void TripleStep(int k)
        {
            /*       _5
             *     _4
             *   _3
             * _2
           * _1
             */
            // O(n) time
            var memo = new int[k + 1];
            Array.Fill(memo, -1);
            TripleStepHelper(k, memo);
        }
        public static int TripleStepHelper(int k, int[] memo)
        {
            if (k < 0)
                return 0;
            if (k == 0 || k == 1)
            {
                memo[k] = 1;
                return memo[k];
            }
            if (memo[k] > 0)
                return memo[k];
            memo[k] = TripleStepHelper(k - 1, memo) + TripleStepHelper(k - 2, memo) + TripleStepHelper(k - 3, memo);
            return memo[k];
        }

        ///Implement an algorithm to print all valid(i.e., properly opened and closed) combinations
        ///of n pairs of parentheses.
        ///EXAMPLE
        ///Input: 3
        ///Output: ((() ) ) , (() () ) , (() ) () , () (() ) , () () ()
        /// </summary>
        public static void SubSetOfValidParensPairs(int k)
        {
            //()
            //()(), (())
            //((() ) ) , (() () ) , (() ) () , () (() ) , () () ()

            HashSet<string> set = new HashSet<string>();
            set.Add("()"); //1st pair
            int index = 2; //iterate for second and go on
            while (index <= k)
            {
                var tempSet = new HashSet<string>();
                foreach (var sentence in set)
                {
                    for (int i = 0; i < sentence.Length; i++)
                    {
                        if (sentence[i].ToString() == "(") //add before and after a pair
                        {
                            StringBuilder firstAddition = new StringBuilder();
                            string temp = sentence.Substring(0, i);
                            firstAddition.Append(temp);
                            firstAddition.Append("()");
                            firstAddition.Append(sentence.Substring(i));

                            StringBuilder secondAddition = new StringBuilder();
                            temp = sentence.Substring(0, i + 1);
                            secondAddition.Append(temp);
                            secondAddition.Append("()");
                            secondAddition.Append(sentence.Substring(i + 1));

                            //make sure not to store duplicates
                            if (!tempSet.Contains(firstAddition.ToString()))
                                tempSet.Add(firstAddition.ToString());
                            if (!temp.Contains(secondAddition.ToString()))
                                tempSet.Add(secondAddition.ToString());
                        }
                    }
                }
                set = tempSet;
                index++;
            }
        }

        //First Common Ancestor Classic Iterative Approach
        public static void LCAClassic(Node root1)
        {

            root1 = new Node(20);
            root1.right = new Node(22);
            root1.right.right = new Node(70);
            root1.left = new Node(8);
            root1.left.left = new Node(4);
            root1.left.right = new Node(19);
            root1.left.right.left = new Node(10);
            /*
           *          20
           *       8      22
           *    4    19      70
                   10
           */
            Node firstNode = root1.left.left;
            Node secondNode = root1.left.right.left;

            var common = LCAHelper(root1, firstNode, secondNode);
            if (firstNodeFound && secondNodeFound)
                Console.WriteLine($"Lowest Common Ancestor between Node : {firstNode.data} and Node : {secondNode.data} is : {common.data}");

            firstNode = root1.right;
            secondNode = root1.right.right;

            common = LCAHelper(root1, firstNode, secondNode);
            if (firstNodeFound && secondNodeFound)
                Console.WriteLine($"Lowest Common Ancestor between Node : {firstNode.data} and Node : {secondNode.data} is : {common.data}");

            firstNode = root1.left.right;
            secondNode = root1.right;

            common = LCAHelper(root1, firstNode, secondNode);
            if (firstNodeFound && secondNodeFound)
                Console.WriteLine($"Lowest Common Ancestor between Node : {firstNode.data} and Node : {secondNode.data} is : {common.data}");
        }
        public static bool firstNodeFound = false;
        public static bool secondNodeFound = false;
        public static Node LCAHelper(Node node, Node first, Node second)
        {
            Node tempNode = null;
            if (node == null)
                return null;
            if (node.data == first.data)
            { tempNode = first; firstNodeFound = true; }
            if (node.data == second.data)
            { tempNode = second; secondNodeFound = true; }

            var leftNeighbor = LCAHelper(node.left, first, second);
            var rightNeighbor = LCAHelper(node.right, first, second);

            if (tempNode != null)
                return tempNode;
            if (leftNeighbor == null && rightNeighbor == null)
                return null;
            if (leftNeighbor != null && rightNeighbor != null)
                return node;
            return (leftNeighbor != null) ? leftNeighbor : rightNeighbor;
        }

        public class MazePoint
        {
            public int row;
            public int col;

            public MazePoint(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }

        public static void RatInMaze()
        {
            //0,0,0,0
            //0,1,0,0
            //0,0,1,0
            var maze = new int[3, 4]
            {
                {0,0,0,0},
                {0,1,0,0},
                {0,0,1,0}
            };
            MazeHelper(maze, new List<MazePoint>(), 0, 0);
            MazeHelperCacheOptimization(maze, new List<MazePoint>(), 0, 0); //fix cache approach
        }
        public static List<List<MazePoint>> possiblePaths = new List<List<MazePoint>>();
        public static void MazeHelper(int[,] maze, List<MazePoint> path, int row, int col)
        {
            if (row >= maze.GetLength(0) || row < 0)
                return; //out of bounds
            if (col >= maze.GetLength(1) || col < 0)
                return; // out of bounds
            if (maze[row, col] == 1)
                return; //blocking point
            if (row == maze.GetLength(0) - 1 && col == maze.GetLength(1) - 1)
            {
                List<MazePoint> clonePath = new List<MazePoint>();
                foreach (var item in path)
                {
                    clonePath.Add(item);
                }
                clonePath.Add(new MazePoint(row, col));
                possiblePaths.Add(clonePath);
                return;
            }
            path.Add(new MazePoint(row, col));
            MazeHelper(maze, path, row, col + 1);
            MazeHelper(maze, path, row + 1, col);
            path.RemoveAt(path.Count - 1);

        }
        public static Dictionary<MazePoint, List<MazePoint>> cache = new Dictionary<MazePoint, List<MazePoint>>();

        //cache approach needs fixing
        public static void MazeHelperCacheOptimization(int[,] maze, List<MazePoint> path, int row, int col)
        {
            if (cache.ContainsKey(new MazePoint(row, col)))
            {
                List<MazePoint> clonePath = new List<MazePoint>();
                for (var i = 0; i < path.Count; i++)
                {
                    clonePath.Add(path[i]);
                }
                clonePath.AddRange(cache[new MazePoint(row, col)]);
                possiblePaths.Add(clonePath);
                return;
            }
            if (row >= maze.GetLength(0) || row < 0)
                return; //out of bounds
            if (col >= maze.GetLength(1) || col < 0)
                return; // out of bounds
            if (maze[row, col] == 1)
                return; //blocking point
            if (row == maze.GetLength(0) - 1 && col == maze.GetLength(1) - 1)
            {
                List<MazePoint> clonePath = new List<MazePoint>();
                for (var i = 0; i < path.Count; i++)
                {
                    clonePath.Add(path[i]);
                    if (!cache.ContainsKey(path[i]))
                        cache.TryAdd(path[i], new List<MazePoint>());
                    for (var j = i; j < path.Count; j++)
                    {
                        cache[path[i]].Add(path[j]);
                    }
                }
                clonePath.Add(new MazePoint(row, col));
                possiblePaths.Add(clonePath);
                return;
            }
            path.Add(new MazePoint(row, col));
            MazeHelperCacheOptimization(maze, path, row, col + 1);
            MazeHelperCacheOptimization(maze, path, row + 1, col);
            path.RemoveAt(path.Count - 1);

        }

        public static void NQueens(int gridVar)
        {
            /*
             * 0,0,1,0
             * 1,0,0,0
             * 0,0,0,1
             * 0,1,0,0
             */
            List<int> places = new List<int> { -1, -1, -1, -1 };

            gridVar = 4;
            QueenHelper(places, gridVar, 0);
        }

        public static List<List<int>> validBoards = new List<List<int>>();

        public static void QueenHelper(List<int> places, int grid, int row)
        {
            if (row == grid)
            {
                var placesClone = new List<int>(places);
                
                validBoards.Add(placesClone);
                return;
            }

            for (int col = 0; col < grid; col++)
            {
                if (ValidSpot(places, row, col))
                {
                    places[row] = col;
                    QueenHelper(places, grid, row + 1);
                }
            }
        }

        public static bool ValidSpot(List<int> places, int row1, int col1)
        {
            for (var row2 = 0; row2 < row1; row2++)
            {
                int col2 = places[row2];

                if (col2 == col1)
                    return false;

                int rowDistance = row1 - row2;
                int colDistance = Math.Abs(col1 - col2);

                if (rowDistance == colDistance)
                    return false;
            }
            return true;
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
        public static void CyclesHelperDetection(GraphNode node, Stack<GraphNode> stack)
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
                        if (stack.Count > 0) stack.Pop(); // backTrack
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
        /// Problem : 1.6
        /// Description : Implement a method to perform basic string compression using the counts
        ///of repeated characters.For example, the string aabcccccaaa would become a2blc5a3.If the
        ///"compressed" string would not become smaller than the original string, your method should return
        ///the original string. You can assume the string has only uppercase and lowercase letters(a - z).
        /// </summary>
        public static string StringCompression(string input)
        {
            StringBuilder sb = new StringBuilder();
            int previousCount = 1;
            string previousLetter = input[0].ToString();
            bool lastAppended = false;

            for (int i = 1; i < input.Length; i++)
            {

                if (input[i].ToString() == previousLetter)
                {
                    previousCount++;
                    lastAppended = false;
                }
                else
                {
                    lastAppended = true;
                    sb.Append(previousLetter);
                    sb.Append(previousCount);
                    previousLetter = input[i].ToString();
                    previousCount = 1;
                }
            }
            if (!lastAppended)
            {
                sb.Append(previousLetter);
                sb.Append(previousCount);
            }

            string expectedOutput = sb.ToString();

            if (expectedOutput.Length >= input.Length)
                return input;
            else
                return expectedOutput;
        }

        /// <summary>
        /// Problem : 1.9
        /// Description : Assume you have a method isSubstring which checks if one word is a substring
        /// of another.Given two strings, 51 and 52, write code to check if 52 is a rotation of 51 using only one
        /// call to i5Sub5tring(e.g., "waterbottle" is a rotation of" erbottlewat").
        /// </summary>
        /// waterbottlewaterbottle
        public static bool IsRotation(string input, string rotation)
        {
            input += input;
            if (input.Contains(rotation))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Problem : 10.1
        /// Description : You are given two sorted arrays, A and B, where A has a large enough buffer at the
        /// end to hold B.Write a method to merge B into A in sorted order.
        /// </summary>
        public static void MergeArrays(int[] largeA, int[] smallB, int lastItemIndex)
        {
            int currentIndex = lastItemIndex + 1;
            int pointerA = 0;
            int pointerB = 0;

            while (pointerA <= lastItemIndex && pointerB < smallB.GetLength(0))
            {
                if (largeA[pointerA] <= smallB[pointerB])
                {
                    largeA[currentIndex] = largeA[pointerA];
                    pointerA++;
                    currentIndex++;
                }
                else
                {
                    largeA[currentIndex] = smallB[pointerB];
                    pointerB++;
                    currentIndex++;
                }
            }

            if (pointerB < smallB.GetLength(0))
            {
                while (pointerB < smallB.GetLength(0))
                {
                    largeA[currentIndex] = smallB[pointerB];
                    pointerB++;
                    currentIndex++;
                }
            }
            else if (pointerA <= lastItemIndex)
            {
                while (pointerA <= lastItemIndex)
                {
                    largeA[currentIndex] = largeA[pointerA];
                    pointerA++;
                    currentIndex++;
                }
            }
            Array.Fill(largeA, -1, 0, lastItemIndex + 1);
        }

        /// <summary>
        /// Problem : 10.2
        /// Description : Write a method to sort an array of strings so that all tne anagrnms are next to each other.
        /// </summary>
        public static void GroupOfAnagrams(List<string> input)
        {
            input = input.Distinct().ToList();

            var dict = new Dictionary<string, List<string>>();
            foreach (var item in input)
            {
                var itemArray = item.ToCharArray();
                Array.Sort(itemArray);
                string key = new string(itemArray);

                if (!dict.ContainsKey(key))
                    dict.Add(key, new List<string> { item });
                else
                    dict[key].Add(item);
            }
            var output = new List<string>();
            foreach (var pair in dict)
            {
                output.AddRange(pair.Value);
            }
        }

        /// <summary>
        /// Problem : 10.3
        /// Description : Given a sorted array of n integers that has been rotated an unknown
        /// number of times, write code to find an element in the array.You may assume that the array was
        /// originally sorted in increasing order.
        /// EXAMPLE
        /// lnput:find5in{ 15, 16, 19, 20, 25, 1, 3, 4, 5, 7, 10, 14}
        /// Output: 8 (the index of 5 in the array)
        /// </summary>
        public static int RotatedArray(List<int> input, int target)
        {
            int start = 0;
            int last = input.Count - 1;

            int index = RotatedHelper(start, last, input, target);
            return index;
        }

        public static int RotatedHelper(int start, int last, List<int> input, int target)
        {
            int mid = (start + last) / 2;
            if (start == last)
            {
                if (input[start] == target)
                    return last;
                else
                    return -1;
            }
            if (last < start) return -1;
            if (input[mid] < target)
            {
                if (input[last] == target)
                    return last;
                if (input[last] < target)
                    return RotatedHelper(start, mid - 1, input, target);
                else
                    return RotatedHelper(mid + 1, last, input, target);
            }
            else if (input[mid] > target)
            {
                if (input[start] == target)
                    return start;
                if (input[start] > target)
                    return RotatedHelper(mid + 1, last, input, target);
                else
                    return RotatedHelper(start, mid - 1, input, target);
            }
            else return mid;
        }

        /// <summary>
        /// Problem : Google Interview
        /// Description : Given a string ex "1234" which identifies a word (encoded a = "1" , b = "2" etc..)
        /// find all the possible ways to translate it, ex,
        /// "1" + "2" + "3" + "4" --> a b c d
        /// "12" + "3" + "4" --> l c d
        /// "1" + "23" + "4" --> a w d
        /// output : 3 ways
        /// </summary>
        private static Dictionary<int, char> alphabetDict = new Dictionary<int, char>()
        {
            {1,'a' },{2,'b'},{3,'c' },{4,'d'},{12,'l'},{23,'w'}
        };
        public static List<string> decoded = new List<string>();
        public static void Decode(string input)
        {
            if (input.Length == 1)
                decoded.Add(alphabetDict[Convert.ToInt32(input[0].ToString())].ToString());
            else
                DecodeHelper(input, 0, new StringBuilder());
        }

        public static void DecodeHelper(string input, int index, StringBuilder sb)
        {
            if (index < input.Length)
            {
                if (index + 1 < input.Length)
                {
                    var currentItem = Convert.ToInt32(input[index].ToString() + input[index + 1].ToString());
                    if (currentItem <= 26)
                    {
                        char currentChar = alphabetDict[currentItem];
                        sb.Append(currentChar.ToString());
                        DecodeHelper(input, index + 2, sb);
                        sb.Remove(sb.Length - 1, 1);
                    }
                }
                sb.Append(alphabetDict[Convert.ToInt32(input[index].ToString())].ToString());
                DecodeHelper(input, index + 1, sb);
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                decoded.Add(sb.ToString());
                return;
            }
        }

        public static int MaxSubsetSum(int[] arr)
        {
            var maxArr = new List<Tuple<int, int>>(); //actual value, accumulative sum
            maxArr.Add(new Tuple<int, int>(arr[0], arr[0]));
            maxArr.Add(new Tuple<int, int>(arr[1], arr[1]));
            //-2 , 1 ,3 ,-4 , 5
            int totalMax = Math.Max(Math.Max(maxArr[0].Item1, maxArr[0].Item2), Math.Max(maxArr[1].Item1, maxArr[1].Item2));
            for (int i = 2; i < arr.Count(); i++)
            {
                int max = int.MinValue;
                for (int j = i - 2; j >= 0; j--)
                {
                    var temp = Math.Max(max, Math.Max(maxArr[j].Item1, maxArr[j].Item2));
                    if (temp > max)
                        max = temp;
                }
                maxArr.Add(new Tuple<int, int>(arr[i], max+arr[i]));
                if (Math.Max(maxArr[i].Item1, maxArr[i].Item2) > totalMax) totalMax = Math.Max(maxArr[i].Item1, maxArr[i].Item2);
            }
            return totalMax;
        }

        /// <summary>
        /// Problem : Max Array Sum (SubSets) Hackerrank : https://www.hackerrank.com/challenges/max-array-sum/problem?isFullScreen=true&h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=dynamic-programming
        /// Description : Given an array of integers, find the subset of non-adjacent elements with the maximum sum. Calculate the sum of that subset. It is possible that the maximum sum is 0, the case when all elements are negative.
        /// Example
        /// [-2 , 1 ,3 ,-4 , 5]
        /// The following subsets with more than 1 element exist.These exclude the empty subset and single element subsets which are also valid.
        /// Subset      Sum
        /// [-2, 3, 5]   6
        /// [-2, 3]      1
        /// [-2, -4]    -6
        /// [-2, 5]      3
        /// [1, -4]     -3
        /// [1, 5]       6
        /// [3, 5]       8
        /// The maximum subset sum is 8. Note that any individual element is a subset as well.
        /// Approach : Index 0 and Index 1 
        /// </summary>

        public static int MaxSubsetSumCache(int[] arr)
        {
            var cacheSum = new List<int>(); //actual value, accumulative sum
            cacheSum.Add(arr[0]);
            cacheSum.Add(arr[1]);
            int dummyMax = Math.Max(arr[0], arr[2]);
            dummyMax = Math.Max(dummyMax, arr[2] + arr[0]);
            cacheSum.Add(dummyMax);                         
            //-2 , 1 ,3 ,-4 , 5
            int totalMax = Math.Max(cacheSum[0], cacheSum[1]);
            totalMax = Math.Max(totalMax, cacheSum[2]);

            for (int i = 3; i < arr.Count(); i++)
            {
                int max1 = Math.Max(arr[i], cacheSum[i-2]);
                max1 = Math.Max(max1, arr[i] + cacheSum[i-2]);

                int max2 = Math.Max(arr[i], cacheSum[i - 3]);
                max2 = Math.Max(max2, arr[i] + cacheSum[i - 3]);

                int currentMax = Math.Max(max1, max2);
                if (currentMax > totalMax) totalMax = currentMax;
                cacheSum.Add(currentMax);
            }
            return totalMax;
        }

        /// <summary>
        /// Problem : 8.1
        /// Description : A child is running up a staircase with n steps and can hop either 1 step, 2 steps, or 3
        ///steps at a time.Implement a method to count how many possible ways the child can run up the stairs.
        /// </summary>
        public static int FourStepMemo(int n)
        {
            int[] memo = new int[n + 1];
            Array.Fill(memo, -1);
            return FourStepMemoHelper(n, memo);
        }
        public static int FourStepMemoHelper(int n, int[] memo)
        {
            if (n == -1 || n == -2 || n == -3)
                return 0;
            if (n == 0)
                return 1;
            else if (memo[n] > -1)
                return memo[n];
            else
            {
                memo[n] = FourStepMemoHelper(n - 1, memo) + FourStepMemoHelper(n - 2, memo) + FourStepMemoHelper(n - 3, memo) + FourStepMemoHelper(n - 4, memo);
                return memo[n];
            }
        }

        /// <summary>
        /// Problem : Facebook Career Page
        /// Description : You are given two strings s and t. You can select any substring of string s and rearrange the characters of the selected substring. Determine the minimum length of the substring of s such that string t is a substring of the selected substring.
        /// Input
        /// s and t are non-empty strings that contain less than 1,000,000 characters each
        /// Output
        /// Return the minimum length of the substring of s.If it is not possible, return -1
        /// Example
        /// s = "dcbefebce"
        /// t = "fd"
        /// output = 5
        /// Explanation:
        /// Substring "dcbef" can be rearranged to "cfdeb", "cefdb", and so on.String t is a substring of "cfdeb". Thus, the minimum length required is 5.
        public static void MinLengthSubstring(string s, string t)
        {
            var stringtHistogram = new Dictionary<string, int>();
            long generalHistoSum = 0; //avoid the loop in dictionary on the s stirng subsequent loop

            foreach (var chart in t)
            {
                string key = chart.ToString();
                generalHistoSum++;
                if (stringtHistogram.ContainsKey(key))
                {
                    stringtHistogram[key]++;
                }
                else
                    stringtHistogram.Add(key, 1);
            }

            long minLength = 0;
            bool firstLetterContained = false;

            foreach (var chars in s)
            {
                if (firstLetterContained)
                    minLength++;

                string key = chars.ToString();
                if (stringtHistogram.ContainsKey(key))
                {
                    if (!firstLetterContained)
                    {
                        firstLetterContained = true;
                        minLength++;
                    }
                    if (stringtHistogram[key] > 0)
                    {
                        stringtHistogram[key]--;
                        generalHistoSum--;
                        if (generalHistoSum == 0)
                            break;
                    }
                }
            }

            if (generalHistoSum == 0)
                Console.WriteLine($"Minimum length is {minLength}");
            else
                Console.WriteLine($"Not a substring");
        }

        /// </summary>

        /// <summary>
        /// Problem : 2.2
        /// Description : Implement an algorithm to find the kth to last element of a singly linked list.
        /// </summary>
        public static LinkedListNode KthElementLinkedList(LinkedListNode head, int k)
        {
            var p1 = head;
            var p2 = head;
            while (k > 0)
            {
                k--;
                p2 = p2.next;
                if (p2 == null) throw new Exception("No Kth Element available");
            }
            while (p2.next != null)
            {
                p1 = p1.next;
                p2 = p2.next;
            }
            return p1;
        }

        public static int maxLayer = 0;
        public static void LeftView()
        {
            var head = new TreeNode(8);
            head.left = new TreeNode(3);
            head.right = new TreeNode(10);
            head.left.left = new TreeNode(1);
            head.left.right = new TreeNode(6);
            head.left.right.left = new TreeNode(4);
            head.left.right.right = new TreeNode(7);
            head.right.right = new TreeNode(14);
            head.right.right.left = new TreeNode(13);
            head.right.right.right = new TreeNode(13);
            head.right.right.right.right = new TreeNode(444);
            LeftViewHelper(head, 1);
            Console.WriteLine($"Number of visible nodes : {maxLayer}");
        }

        public static void LeftViewHelper(TreeNode node, int currentLayer)
        {
            if (node == null)
            {
                return;
            }
            if (currentLayer > maxLayer)
            {
                maxLayer = currentLayer;
                Console.WriteLine(node.data);
            }

            LeftViewHelper(node.left, currentLayer + 1);
            LeftViewHelper(node.right, currentLayer + 1);
        }

        public static void RotationalCipher(string input, int rotationFactor)
        {
            StringBuilder output = new StringBuilder();

            if (rotationFactor >= 26)
                rotationFactor %= 26;

            foreach (char chr in input)
            {
                int currentASCII = Convert.ToInt32(chr);
                if (currentASCII >= 48 && currentASCII <= 57) //[0-9]
                {
                    if (currentASCII + rotationFactor > 57)
                    {
                        int remaining = 57 - currentASCII;
                        char toAdd = (char)(48 + (rotationFactor - remaining - 1));
                        output.Append(toAdd);
                    }
                    else
                    {
                        char toAdd = (char)(currentASCII + rotationFactor);
                        output.Append(toAdd);
                    }

                }
                else if (currentASCII >= 97 && currentASCII <= 122) //[a,z]
                {
                    if (currentASCII + rotationFactor > 122)
                    {
                        int remaining = 122 - currentASCII;
                        char toAdd = (char)(97 + (rotationFactor - remaining - 1));
                        output.Append(toAdd);
                    }
                    else
                    {
                        char toAdd = (char)(currentASCII + rotationFactor);
                        output.Append(toAdd);
                    }
                }
                else if (currentASCII >= 65 && currentASCII <= 90) //[A,Z]
                {
                    if (currentASCII + rotationFactor > 90)
                    {
                        int remaining = 90 - currentASCII;
                        char toAdd = (char)(65 + (rotationFactor - remaining - 1));
                        output.Append(toAdd);
                    }
                    else
                    {
                        char toAdd = (char)(currentASCII + rotationFactor);
                        output.Append(toAdd);
                    }
                }
                else //Ignore Special Characters
                {
                    output.Append(chr);
                }
            }
        }

        /// <summary>
        /// Problem : 16.21
        /// Description : Given two arrays of integers, find a pair of values (one value from each array) that you
        /// can swap to give the two arrays the same sum.
        /// EXAMPLE
        /// lnput:{ 4, 1, 2, 1, 1, 2}
        /// and{3, 6, 3, 3}
        /// Output: {1, 3}
        /// </summary>
        /// <Time>Time for HashSet solution is O(A+B)</Time>
        public static void SumSwap()
        {
            List<int> array1 = new List<int> { 4, 1, 2, 1, 1, 2 };
            List<int> array2 = new List<int> { 3, 6, 3, 3 };

            HashSet<int> set = new HashSet<int>();
            foreach (var item in array1)
            {
                if (!set.Contains(item))
                    set.Add(item);
            }

            int sum1 = array1.Sum();
            int sum2 = array2.Sum();

            foreach (var item in array2)
            {
                int target = (sum1 - sum2) / 2 + item;
                if (set.Contains(target))
                {
                    Console.WriteLine($"A valid pair is consisted of {item} from array2 and {target} from array1");
                }
            }
        }

        public static int totalBlocks = 0;
        public static void QueensAttack(int n, int k, int r_q, int c_q, List<List<int>> obstacles)
        {
            UpperLeftHelper(n, r_q, c_q, obstacles);
            UpperHelper(n, r_q, c_q, obstacles);
            UpperRightHelper(n, r_q, c_q, obstacles);
            LeftHelper(n, r_q, c_q, obstacles);
            RightHelper(n, r_q, c_q, obstacles);
            LowerLeftHelper(n, r_q, c_q, obstacles);
            LowerHelper(n, r_q, c_q, obstacles);
            LowerRightHelper(n, r_q, c_q, obstacles);

            totalBlocks -= 8;
            Console.WriteLine($"Total blocks available for attack {totalBlocks}");
        }

        public static void UpperLeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            UpperLeftHelper(n, row - 1, col - 1, obstacles);
            return;
        }
        public static void UpperHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            UpperHelper(n, row - 1, col, obstacles);
            return;
        }
        public static void UpperRightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            UpperRightHelper(n, row - 1, col + 1, obstacles);
            return;
        }
        public static void LeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            LeftHelper(n, row, col - 1, obstacles);
            return;
        }
        public static void RightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            RightHelper(n, row, col + 1, obstacles);
            return;
        }
        public static void LowerLeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            LowerLeftHelper(n, row + 1, col - 1, obstacles);
            return;
        }
        public static void LowerHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            LowerHelper(n, row + 1, col, obstacles);
            return;
        }
        public static void LowerRightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (row < 0 || row > n)
                return;
            if (col < 0 || col > n)
                return;
            if (obstacles[row].Contains(col))
                return;
            totalBlocks++;
            LowerRightHelper(n, row + 1, col + 1, obstacles);
            return;
        }

        /// <summary>
        /// Problem : 16.15
        /// Description : The computer has four slots, and each slot will contain a ball that is red (R), yellow (Y), green (G) or
        /// blue(B). For example, the computer might have RGGB(Slot #1 is red, Slots #2 and #3 are green, Slot
        /// #4 is blue).
        /// You, the user, are trying to guess the solution. You might, for example, guess YRGB.
        /// When you guess the correct color for the correct slot, you get a "hit:' If you guess a color that exists
        /// but is in the wrong slot, you get a "pseudo-hit:' Note that a slot that is a hit can never count as a
        ///pseudo-hit.
        /// For example, if the actual solution is RGBY and you guess GGRR , you have one hit and one pseudohit
        /// Write a method that, given a guess and a solution, returns the number of hits and pseudo-hits
        /// </summary>
        public static void RYGB(string input, string guess)
        {
            //RGBBY
            //string key,<Tuple<int,List<int>> value --> R,1[0], G,1[1], B,2[2,3], Y,1[4]
            //GGRRY
            var colorFrequencyPositions = new Dictionary<string, Tuple<int, List<int>>>();
            int index = 0;
            foreach (var item in input)
            {
                if (colorFrequencyPositions.ContainsKey(item.ToString()))
                {
                    var tempTuple = colorFrequencyPositions[item.ToString()];
                    tempTuple.Item2.Add(index);
                    colorFrequencyPositions[item.ToString()] = new Tuple<int, List<int>>(tempTuple.Item1 + 1, tempTuple.Item2);
                }
                else
                {
                    colorFrequencyPositions.Add(item.ToString(), new Tuple<int, List<int>>(1, new List<int> { index }));
                }
                index++;
            }

            index = 0; //reset
            int pseudohits = 0;
            int hits = 0;
            foreach (var item in guess)
            {
                if (colorFrequencyPositions.ContainsKey(item.ToString()))
                {
                    int frequency = colorFrequencyPositions[item.ToString()].Item1;
                    List<int> listIndexes = colorFrequencyPositions[item.ToString()].Item2;
                    if (colorFrequencyPositions[item.ToString()].Item2.Contains(index))
                    {
                        hits++;
                        if (frequency > 0)
                            colorFrequencyPositions[item.ToString()] = new Tuple<int, List<int>>(frequency - 1, listIndexes);
                        else
                        {
                            colorFrequencyPositions[item.ToString()] = new Tuple<int, List<int>>(frequency, listIndexes);
                            pseudohits--;
                        }
                    }
                    else
                    {
                        if (frequency > 0)
                        {
                            colorFrequencyPositions[item.ToString()] = new Tuple<int, List<int>>(frequency - 1, listIndexes);
                            pseudohits++;
                        }
                    }
                }
                index++;
            }
            Console.WriteLine($"Number of pseudo-hits {pseudohits} and number of actual hits {hits}");
        }

        /// <summary>
        /// Find all palindrome substrings
        /// </summary>
        public static HashSet<string> palindromSets = new HashSet<string>();
        public static void AllPalindrom (string input)
        {
            PalindromHelper(input, 0);
        }
        
        public static void PalindromHelper(string input, int index)
        {
            if (index==input.Length-1)
            {
                palindromSets.Add(input[index].ToString());
                return;
            }
            PalindromHelper(input, index + 1);

            HashSet<string> cloneSets = new HashSet<string>();
            foreach (var set in palindromSets)
            {
                var tempSet = SubstringBuilder(input[index].ToString(),set);
                foreach (var item in tempSet)
                {
                    cloneSets.Add(item);
                }
            }
            palindromSets = cloneSets;

        }

        public static HashSet<string> SubstringBuilder (string letter,string word)
        {
            HashSet<string> tempSet = new HashSet<string>();
            for (int i=0; i<word.Length;i++)
            {
                StringBuilder sb = new StringBuilder();
                string tempFirst = word.Substring(0, i);
                if (tempFirst != null || !string.IsNullOrEmpty(tempFirst))
                    sb.Append(tempFirst);
                sb.Append(letter);
                sb.Append(word.Substring(i));
                tempSet.Add(sb.ToString());
            }
            StringBuilder finalSb = new StringBuilder();
            finalSb.Append(word);
            finalSb.Append(letter);
            tempSet.Add(finalSb.ToString());
            return tempSet;
        }
        
        /// <summary>
        /// FaceBook
        /// https://www.facebookrecruiting.com/portal/coding_practice_question/?problem_id=840934449713537&ppid=454615229006519&practice_plan=0
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int numberOfWays(int[] arr, int k)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (var item in arr)
            {
                if (map.ContainsKey(item))
                    map[item]++;
                else
                    map.Add(item, 1);
            }
            int totalPairs = 0;
            int totalToDividePairs = 0;
            foreach (var pair in map)
            {
                int key = k - pair.Key;
                if (pair.Key*2==k)
                {
                    totalPairs += pair.Value * (pair.Value - 1) / 2;
                }
                else if (map.ContainsKey(key))
                {
                    totalToDividePairs += map[key];
                }
            }
            return totalPairs+ (totalToDividePairs/2);
        }

        /// <summary>
        /// FaceBook https://www.facebookrecruiting.com/portal/coding_practice_question/?problem_id=559324704673058&ppid=454615229006519&practice_plan=0
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int matchingPairs(string s, string t)
        {
            int pointer = 0;

            int validPairs = 0;
            var map = new Dictionary<char, Tuple<int, int>>();
            while (pointer < s.Length)
            {
                if (s[pointer] == t[pointer])
                    validPairs++;
                else
                {
                    if (s.Contains(t[pointer]))
                        map.Add(t[pointer], new Tuple<int, int>(pointer, s.IndexOf(t[pointer])));
                }
                pointer++;
            }
            bool foundPerfectSwap = false;
            if (map.Count > 0)
            {
                foreach (var pair in map)
                {
                    var tPointer = pair.Value.Item1;
                    var sPointer = pair.Value.Item2;
                    if (map.ContainsKey(t[sPointer]))
                    {
                        if (map[t[sPointer]].Item2 == tPointer)
                        {
                            foundPerfectSwap = true;
                            break;
                        }
                    }
                }
                if (!foundPerfectSwap)
                    validPairs += 1;
                else
                    validPairs += 2;
            }
            
            return validPairs;
        }

        /// <summary>
        /// GeeksForGeeks : https://practice.geeksforgeeks.org/problems/search-in-a-rotated-array4618/1
        /// </summary>
        public static void SearchInRotatedArray()
        {
            List<int> rotatedArray = new List<int> { 5, 6, 7, 8, 1, 2, 3 };
            int target = 8;
            SearchInRotatedArrayHelper(rotatedArray, 0, rotatedArray.Count - 1, target);
            if (index != -1)
                Console.WriteLine($"Target's {target} index is : {index}");

            index = -1;
            target = 3;
            SearchInRotatedArrayHelper(rotatedArray, 0, rotatedArray.Count - 1, target);
            if (index != -1)
                Console.WriteLine($"Target's {target} index is : {index}");

            index = -1;
            target = 6;
            SearchInRotatedArrayHelper(rotatedArray, 0, rotatedArray.Count - 1, target);
            if (index != -1)
                Console.WriteLine($"Target's {target} index is : {index}");

            index = -1;
            target = 113;
            SearchInRotatedArrayHelper(rotatedArray, 0, rotatedArray.Count - 1, target);
            if (index != -1)
                Console.WriteLine($"Target's {target} index is : {index}");
            else
                Console.WriteLine($"Target not found {target}");
        }

        public static int index = -1;

        public static void SearchInRotatedArrayHelper(List<int> input, int start, int end, int target)
        {
            if (start > end)
                return;

            int mid = (start + end) / 2;

            if (input[mid] == target)
            {
                index = mid;
                return;
            }

            //if (input[mid] < target)
            //{

                if (target <= input[end]) //search right
                {
                    SearchInRotatedArrayHelper(input, mid + 1, end, target);
                    if (index == -1)
                        SearchInRotatedArrayHelper(input, start, mid - 1, target); //search left
                    else
                        return;
                }
                else
                {
                    SearchInRotatedArrayHelper(input, start, mid - 1, target);
                    if (index == -1)
                        SearchInRotatedArrayHelper(input, mid + 1, end, target);
                    else
                        return;
                }
            //}
            //else { }
            
        }

        /// <summary>
        /// GeeksForGeeks : https://www.geeksforgeeks.org/connect-nodes-at-same-level/
        /// Input Tree
        ///  A
        /// / \
       /// B   C
      /// / \   \
      /// D E   F

        /// Output Tree
        ///  A--->NULL
        /// / \
        ///B-->C-->NULL
      /// / \   \
     /// D-->E-->F-->NULL
        /// </summary>
        public static void ConnectNodesSameLevel()
        {
            ComplexTreeNode head = new ComplexTreeNode(1) ;
            head.left = new ComplexTreeNode(2);
            head.right = new ComplexTreeNode(3);
            head.left.left = new ComplexTreeNode(4);
            head.left.right = new ComplexTreeNode(5);
            head.right.right = new ComplexTreeNode(6);

            var levelToNodes = new Dictionary<int, List<ComplexTreeNode>>();
            levelToNodes.Add(0, new List<ComplexTreeNode> { head });
            ConnectedNodesHelper(head, 1,levelToNodes);

            foreach (var item in levelToNodes)
            {
                int index = 0;
                while (index < item.Value.Count-1)
                {
                    item.Value[index].next = item.Value[index + 1];
                    index++;
                }
                item.Value[index].next = null; //last neighbor
            }

            PrintTree(head);
        }

        public static void ConnectedNodesHelper(ComplexTreeNode node, int level, Dictionary<int,List<ComplexTreeNode>> levelToNodes)
        {
            if (node == null)
                return;
            if (levelToNodes.ContainsKey(level))
                levelToNodes[level].Add(node);
            else
                levelToNodes.Add(level, new List<ComplexTreeNode> { node });

            ConnectedNodesHelper(node.left, level + 1, levelToNodes);
            ConnectedNodesHelper(node.right, level + 1, levelToNodes);
        }

        public static void PrintTree(ComplexTreeNode node)
        {
            if (node == null)
                return;
            //-999 signifies NULL next neighbor
            Console.WriteLine($"Current node {node.data} and his next neighbor is {node.next?.data ?? -999}");

            PrintTree(node.left);
            PrintTree(node.right);
        }

        /// <summary>
        /// GeekForGeeks : https://www.geeksforgeeks.org/lowest-common-ancestor-binary-tree-set-1/
        /// </summary>
        public static void FindLCA ()
        {
            ComplexTreeNode head = new ComplexTreeNode(1);
            head.left = new ComplexTreeNode(2);
            head.right = new ComplexTreeNode(3);
            head.left.left = new ComplexTreeNode(4);
            head.left.right = new ComplexTreeNode(5);
            head.left.right.right = new ComplexTreeNode(7);
            head.right.right = new ComplexTreeNode(6);

            var firstNode = head.right.right; //6
            var secondNode = head.left.right; //5

            var LCA = FindLCAHelper(head, firstNode, secondNode);

            if (firstFound && secondFound)
                Console.WriteLine($"Lowest Common Ancestor in input tree is {LCA.data}"); //1

            //reset
            firstNode = head.left; //2 
            secondNode= head.left.left; //4 
            firstFound = false;
            secondFound = false;
            LCA = FindLCAHelper(head, firstNode, secondNode);

            if (firstFound && secondFound)
                Console.WriteLine($"Lowest Common Ancestor in input tree is {LCA.data}"); //2
        }

        private static bool firstFound = false;
        private static bool secondFound = false;

        public static ComplexTreeNode FindLCAHelper(ComplexTreeNode node, ComplexTreeNode firstNode, ComplexTreeNode secondNode)
        {
            if (node == null)
                return null;

            ComplexTreeNode tempNode = null;

            if (node == firstNode)
            {
                tempNode = firstNode;
                firstFound = true;
            }
            else if (node == secondNode)
            {   
                tempNode = secondNode;
                secondFound = true;
            }

            var leftNeighbors = FindLCAHelper(node.left, firstNode, secondNode);
            var rightNeighbors = FindLCAHelper(node.right, firstNode, secondNode);

            if (tempNode == null && leftNeighbors == null && rightNeighbors == null)
                return null;
            if (leftNeighbors != null && rightNeighbors != null)
                return node;
            if (tempNode != null)
                return tempNode;
            return (leftNeighbors ?? rightNeighbors);
        }

        /// <summary>
        /// GeekForGeeks : https://www.geeksforgeeks.org/detect-and-remove-loop-in-a-linked-list/
        /// </summary>
        public static void RemoveLoopFromLinkedList()
        {
            //1 - 2 - 3 - 4 - 5 - 2
            var head = new LinkedListNode(1);
            head.next = new LinkedListNode(2);
            head.next.next = new LinkedListNode(3);
            head.next.next.next = new LinkedListNode(4);
            head.next.next.next.next = new LinkedListNode(5);
            head.next.next.next.next.next = head.next;

            bool flag;
            LinkedListNode pointOfCollision;

            (flag,pointOfCollision) = CircleDetection(head);

            if (!flag)
                Console.WriteLine("There is no Loop");
            else
            {
                Console.WriteLine($"Point of collision was {pointOfCollision.data} and his neighbor should be null {pointOfCollision.next}");
            }

        }

        public static Tuple<bool, LinkedListNode> CircleDetection (LinkedListNode node)
        {
            var head = node;
            var slowPointer = node;
            var fastPointer = node;

            while (fastPointer != null || fastPointer?.next != null)
            {
                fastPointer = fastPointer.next.next;
                slowPointer = slowPointer.next;

                if (slowPointer == fastPointer)
                {
                    var collision = RemoveLoop(slowPointer,head);
                    collision.next = null; //remove loop
                    return new Tuple<bool, LinkedListNode>(true, collision);
                }
            }
            return new Tuple<bool, LinkedListNode>(false, null);
        }

        public static LinkedListNode RemoveLoop (LinkedListNode node, LinkedListNode head)
        {
            var tempNode = node;
            while (head != node)
            {
                tempNode = node;
                node = node.next;
                head = head.next;
            }
            return tempNode;

        }

        /// <summary>
        /// Given two strings your function should return YES if a can be identical to b after operations,
        /// available operations : 
        /// convert small letter to capital letter (from a)
        /// delete small letter (from a)
        /// a is consisted of small and capital letters (ASCII) and b only from capital letters.
        /// </summary>
        public static string output = "NO";
        public static string Abbreviation(string a, string b)
        {
            if (a.Length < b.Length) return output;
            indexTarget = b.Length - 1;
            AbbrevHelper(a, b, 0);
            return output;
        }
        public static int indexTarget = 0;
        public static int charSum = 0;
        public static bool violation = false;
        //A-Z 65-90
        //a-z 97-122
        public static void AbbrevHelper(string input, string target, int indexInput)
        {
            if (indexInput < input.Length-1)
                AbbrevHelper(input, target, indexInput + 1);
            if (indexInput < indexTarget && (target.Length - 1 - charSum < indexInput)) { output = "NO"; return; } //speedUp
            if (violation) return;
            if (charSum == target.Length)
            {
                int inputAscii = input[indexInput];
                if (inputAscii <= 90) //capital letter
                    output = "NO";
                return;
            }
            if (input[indexInput] == target[indexTarget])
            {
                indexTarget--;
                charSum++;
                if (charSum == target.Length)
                    output = "YES";
                return;
            }
            else
            {
                int inputAscii = input[indexInput];
                int targetAscii = target[indexTarget];
                if (inputAscii > 90) //not capital
                {
                    int inputAsciiToCapital = 65 + (inputAscii - 97);
                    if (inputAsciiToCapital == targetAscii) // same letter toUpper()
                    {
                        indexTarget--;
                        charSum++;
                        if (charSum == target.Length)
                            output = "YES";
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else //reset
                {
                    violation = true;
                    output = "NO";
                    return;
                }
            }
        }

        public class LinkedListNode
        {
            public int data;
            public LinkedListNode next;
            public LinkedListNode(int data)
            {
                this.data = data;
                this.next = null;
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

        public class ComplexTreeNode
        {
            public int data;
            public ComplexTreeNode left;
            public ComplexTreeNode right;
            public ComplexTreeNode next;

            public ComplexTreeNode(int data)
            {
                this.data = data;
                this.left = null;
                this.right = null;
                this.next = null;
            }
        }
    }
}
