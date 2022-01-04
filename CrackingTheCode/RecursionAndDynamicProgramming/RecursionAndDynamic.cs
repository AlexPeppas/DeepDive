using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.RecursionAndDynamicProgramming
{
    public static class RecursionAndDynamic
    {
        /// <summary>
        /// WarmUp : 
        /// Description : Compute the nth Fibonnaci Number recursively
        /// </summary>
        /// 
        /*
         * f0 , f1 , f2 , f3 , f4 , f5 , f6 , f7
           0  , 1  , 1  , 2  ,  3 , 5 , 8 , 13 
        */
        public static int Fibo(int nth)
        {
            if (nth == 0)
                return 0;
            if (nth == 1)
                return 1;
            else
                return FiboRecursively(0, 1, nth, 0);
        }

        public static int FiboRecursively(int f0, int f1, int nth, int layer)
        {
            if (layer == nth)
                return f1; 
            else
            {
                layer++;
                int temp;
                temp = f1;
                f1 = f1 + f0;
                f0 = temp;
                return FiboRecursively(f0, f1, nth, layer);
            }
        }

        public static List<int> cache = new List<int> { 0, 1 };

        public static void FibMain ()
        {
            Console.WriteLine("FIB OF 7 : " + FibMemo(7));
            Console.WriteLine("FIB OF 6 : " + cache[6]);
            Console.WriteLine("FIB OF 5 : " + cache[5]);
            Console.WriteLine("FIB OF 8 : " + FibMemo(8));
        }
        public static int FibMemo (int n)
        {
            if (n == 0)
                return 0;
            if (n == 1)
                return 1;
            if (n >= cache.Count)
            {
                for (int st=cache.Count-1;st<=n;st++)
                {
                    cache.Add(cache[st] + cache[st - 1]);
                }
            }
            return cache[n];
        }

        /// <summary>
        /// Problem : 8.1
        /// Description : A child is running up a staircase with n steps and can hop either 1 step, 2 steps, or 3
        ///steps at a time.Implement a method to count how many possible ways the child can run up the stairs.
        /// </summary>
        public static int TripleStep(int n)
        {
            int[] memo = new int[n + 1];
            Array.Fill(memo, -1);
            return TripleStep(n, memo);
        }

        private static int TripleStep (int n , int[] memo)
        {
            if (n < 0)
                return 0;
            if (n == 0)
                return 1;
            else if (memo[n] > -1)
                return memo[n];
            else
            {
                memo[n] = TripleStep(n - 1, memo) + TripleStep(n - 2, memo) + TripleStep(n - 3, memo);
                return memo[n];
            }
        }

        /// <summary>
        /// Problem : 8.2
        /// Description : Imagine a robot sitting on the upper left corner of grid with r rows and c columns.
        /// The robot can only move in two directions, right and down, but certain cells are "off limits" such that
        /// the robot cannot step on them.Design an algorithm to find a path for the robot from the top left to
        /// the bottom right.
        /// </summary>
        /// Time Complexity O(row*col)
        public class Point 
        {
            
            public int row;
            public int column;

            public Point(int r, int c)
            {
                row = r;
                column = c;
            }
        }
        public static List<Point> RobotInAGrid ()
        {
            bool[,] maze = new bool[3, 4] {
            {true,true,true,true},
            {false,false,false,true},
            {false,false,false,true}
            };
            if (maze == null || maze.Length == 0) return null;
            List<Point> path = new List<Point>();
            HashSet<Point> hset = new HashSet<Point>();
            if (GetPath(maze,maze.GetLength(0)-1,maze.GetLength(1)-1,path,hset))
                return path;
            return null;
        }
        public static bool GetPath(bool[,] maze,int row, int col,List<Point> path, HashSet<Point> hset)
        {
            if (col<0 || row<0 || !maze[row,col])
                return false;
            Point point = new Point(row, col);

            if (hset.Contains(point))
                return false;

            bool isAtOrigin = (row == 0) && (col == 0);

            if (isAtOrigin || GetPath(maze,row-1,col,path,hset) || GetPath(maze,row,col-1,path,hset))
            {
                path.Add(point);
                return true;
            }

            hset.Add(point);
            return false;
        }

        /// <summary>
        /// Problem : 8.3
        /// Description : A magic index in an array A[ 1 .•. n-1] is defined to be an index such that A[ i]
        /// i.Given a sorted array of distinct integers, write a method to find a magic index, if one exists, in array A.
        /// FOLLOW UP
        /// What if the values are not distinct?
        /// </summary>
        
        public static int MagicIndex(List<int> sortedList)
        {
            sortedList.Add(-5);
            sortedList.Add(-3);
            sortedList.Add(-2);
            sortedList.Add(-1);
            sortedList.Add(4);
            sortedList.Add(7);
            sortedList.Add(9);
            sortedList.Add(10);

            if (sortedList.Count == 0) return 0;
            return BinarySearchInMagicIndex(sortedList, 0, sortedList.Count - 1);
        }

        public static int MagicIndexDuplicates(List<int> sortedList)
        {
            sortedList.Add(-1);
            sortedList.Add(0);
            sortedList.Add(0);
            sortedList.Add(3);
            sortedList.Add(3);
            sortedList.Add(3);
            sortedList.Add(3);
            sortedList.Add(3);
            sortedList.Add(3);
            sortedList.Add(7);
            sortedList.Add(12);
            sortedList.Add(39);

            if (sortedList.Count == 0) return 0;
            return BinarySearchInMagicIndexDuplicates(sortedList, 0, sortedList.Count - 1);
        }
        public static int BinarySearchInMagicIndex(List<int> list, int start , int end)
        {
            if (start > end)
                return -1;
            int mid = (start + end) / 2;
            if (list[mid] ==start)
                return start;

            if (list[start] > start)
                return BinarySearchInMagicIndex(list, start, mid - 1);
            else
                return BinarySearchInMagicIndex(list, mid + 1, end);
        }

        //if array contains duplicate integers
        /*
        * When we see that A [mid] < mid, we cannot conclude which side the magic index is on. It could be on
        the right side, as before. Or, it could be on the left side (as it, in fact, is).
        Could it be anywhere on the left side? Not exactly. Since A[ 5] = 3, we know that A[ 4] couldn't be a magic
        index. A[ 4] would need to be 4 to be the magic index, but A[ 4] must be less than or equal to A[ 5].
        In fact, when we see that A[ 5] = 3, we'll need to recursively search the right side as before. But, to search
        the left side, we can skip a bunch of elements and only recursively search elements A [ 0] through A [ 3].
        A [ 3] is the first element that could be a magic index.
        The general pattern is that we compare mid Index and midValue for equality first. Then, if they are not
        equal, we recursively search the left and right sides as follows:
        • Left side: search indices start through Math. min (midlndex - 1, midValue ).
        Right side: search indices Math. max(midlndex + 1, midValue) through end.
         */
        public static int BinarySearchInMagicIndexDuplicates(List<int> list, int start, int end)
        {
            if (start > end)
                return -1;
            int midIndex= (start + end) / 2;
            int midValue = list[midIndex];
            if (midValue == midIndex)
                return midIndex;

            //left search
            int endTemp = Math.Min(midValue, midIndex-1);
            int left = BinarySearchInMagicIndexDuplicates(list, start, endTemp);
            if (left > -1)
                return left;

            //right search
            int startTemp = Math.Max(midValue, midIndex + 1);
            int right = BinarySearchInMagicIndexDuplicates(list, startTemp, end);
            return right;
        }

        /// <summary>
        /// Problem : 8.4
        /// Description : Write a method to return all subsets of a set.
        /// </summary>
        /// case : 0 -> {}
        /// case : 1 -> {a1} {}
        /// case : 2 -> {a1} {} {a1,a2} {a2}
        /// case : 3 -> {}{a1}{a2}{a1,a2} {a3} {a1,a2,a3} {a1,a3} {a2,a3}
        /// case : n -> case(n-1)+ case(n-1)+n
        ///         Recursion O(n2^n)
        ///         

        public static List<List<int>> PowerSet(List<int> set,int index)
        {
            //{1,2,3} , 0
            List<List<int>> allSubsets;
            if (set.Count == index)
            {
                allSubsets = new List<List<int>>(); // add empty subset
                //baseCase
            }
            else
            {
                allSubsets = PowerSet(set, index + 1);
                int item = set[index];
                List<List<int>> allSubsetsClone = new List<List<int>>();
                foreach (var subSet in allSubsets)
                {
                    List<int> tempSubset = new List<int>();
                    tempSubset.AddRange(subSet);
                    tempSubset.Add(item);
                    allSubsetsClone.Add(tempSubset);
                }
                allSubsets.AddRange(allSubsetsClone);
            }
            return allSubsets;
        }

        /// <summary>
        /// Problem : 8.5
        /// Description : Write a recursive function to multiply two positive integers without using
        /// the* operator (or / operator). You can use addition, subtraction, and bit shifting, but you should
        /// minimize the number of those operations.
        /// TODO
        /// </summary>
        public static void RecursiveMultiply()
        { }

        /// <summary>
        /// Problem : 8.7
        /// Description : Write a method to compute all permutations of a string of unique characters.
        /// </summary>
        /// a1 -> a1
        /// a1a2 -> a2a1, a1a2
        /// a1a2a3 -> a3a2a1, a2a3a1, a2a1a3, a3a1a2, a1a3a2, a1a2a3
        /// </summary>
        public static List<string> PermutationsWithoutDupsS(string set)
        {
            //"123"
            if (set == null)
                return null;

            List<string> permutations = new List<string>();
            
            if (set.Length == 0)
                permutations.Add(" "); //base case 
            else
            {
                char first = set[0];
                string remaining = set.Substring(1);
                List<string> words = PermutationsWithoutDupsS(remaining);
                foreach (var word in words)
                {
                    for (int i=0; i<word.Length;i++)
                    {
                        string st = insertCharAt(word, first, i);
                        permutations.Add(st);
                    }
                }
            }
            return permutations;
        }
        public static string insertCharAt(string word, char first, int i)
        {
            var s = new StringBuilder();
            s.Append(word.Substring(0, i));
            s.Append(first);
            s.Append(word.Substring(i));
            return s.ToString();
        }


        //TOFIX
        public static List<List<string>> PermutationsWithoutDups(List<string> set, int index) 
        {
            //[a3,a2,a1] 0 
            List<List<string>> output = new List<List<string>> { };
            if (set.Count == index+1)
            {
                List<string> temp = new List<string> { set[index] };
                output.Add(temp);
            }
            else
            {
                output = PermutationsWithoutDups(set, index + 1);
                string item = set[index];
                List<List<string>> cache = new List<List<string>> { };
                foreach (var subset in output)
                {
                    foreach (var currentString in subset)
                    {
                        int i = 0;
                        List<string> temp = new List<string>();
                        StringBuilder sBuilder = new StringBuilder();
                        while (i<currentString.Length)
                        {
                            temp = new List<string>();
                            sBuilder = new StringBuilder();
                            sBuilder.Append(currentString.Substring(0, i));
                            sBuilder.Append(item);
                            sBuilder.Append(currentString.Substring(i + 1, currentString.Length));
                            i++;
                            temp.Add(sBuilder.ToString());
                            cache.Add(temp);
                        }
                        temp.Add(sBuilder.Append(item).ToString());
                        cache.Add(temp);
                    }

                }
                return cache;
            }
            return output;

        }

        /// <summary>
        /// Problem : 8.9
        /// Description : Write a method to compute all permutations of a string of unique characters.
        /// </summary>
        ///Implement an algorithm to print all valid(i.e., properly opened and closed) combinations
        ///of n pairs of parentheses.
        ///EXAMPLE
        ///Input: 3
        ///Output: ((() ) ) , (() () ) , (() ) () , () (() ) , () () ()
        /// </summary>
        /// 3 (), 0
        public static HashSet<string> set = new HashSet<string>();
        public static List<string> Parens(int pairs, int index)
        {
            List<string> parens = new List<string>();
            if (index == pairs)
            {
                parens.Add(" ");
            }
            else
            {
                List<string> words = Parens(pairs, index + 1);
                foreach (var phrase in words)
                {
                    if (phrase == " ")
                        parens.Add("()");
                    else
                    {
                        int counter = 0;
                        foreach (var word in phrase)
                        {
                            string st = string.Empty;
                            string stafter = string.Empty;
                            if (word == '(')
                            { 
                                st = insertParensPair(phrase, counter);
                                stafter = insertParensPair(phrase, counter+1);
                            }
                            if (!set.Contains(st) && st != string.Empty)
                            {
                                parens.Add(st);
                                set.Add(st);
                            }
                            if (!set.Contains(stafter) && stafter != string.Empty)
                            {
                                parens.Add(stafter);
                                set.Add(stafter);
                            }
                            counter++;
                        }
                    }
                }
            }
            return parens;
        }

        public static string insertParensPair(string phrase, int index)
        {
            var s = new StringBuilder();
            s.Append(phrase.Substring(0, index));
            s.Append("()");
            s.Append(phrase.Substring(index));
            return s.ToString();
        }

        /// <summary>
        /// Problem : 8.10
        /// Description : Implement the "paint fill" function that one might see on many image editing programs.
        /// That is, given a screen(represented by a two-dimensional array of colors), a point, and a new color,
        /// fill in the surrounding area until the color changes from the original color.
        /// </summary>
        /// r: red, y:yellow, b:black, g:green COLORS
        /// 
        public static void PaintHelper ()
        {
            char[,] image = new char[4, 6]
            {
                { 'b','b','g','g','g','b'},
                { 'b','b','g','g','g','y'},
                { 'b','b','g','g','g','b'},
                { 'y','y','g','b','g','y'}
            };
            char colorToChange = 'r';
            char colorStartedAs = 'g';
            PaintFill(image, 1, 3, colorToChange, colorStartedAs);

           
            for (var i=0;i<image.GetLength(0);i++)
            {
                StringBuilder newBoard = new StringBuilder();
                for (var j=0;j<image.GetLength(1);j++)
                {
                    newBoard.Append(image[i, j]);
                }
                Console.WriteLine(newBoard.ToString() + System.Environment.NewLine);
            }
        }
        public static char[,] PaintFill (char[,] image, int row , int column, char color, char colorStartedAs)
        {
            int rowLength = image.GetLength(0);
            int colLength = image.GetLength(1);
            
            if (row < 0 || row >= rowLength)
                return image;
            if (column < 0 || column >= colLength)
                return image;
            if (image[row,column]== colorStartedAs)
            {
                image[row, column] = color;
                PaintFill(image, row, column + 1, color, colorStartedAs);
                PaintFill(image, row+1, column, color, colorStartedAs);
                PaintFill(image, row, column - 1, color, colorStartedAs);
                PaintFill(image, row-1, column, color, colorStartedAs);
                PaintFill(image, row - 1, column-1, color, colorStartedAs);
                PaintFill(image, row + 1, column-1, color, colorStartedAs);
                PaintFill(image, row + 1, column+1, color, colorStartedAs);
                PaintFill(image, row - 1, column+1, color, colorStartedAs);
            }
            return image;
        }

        /// <summary>
        /// Problem : 8.12
        /// Description : Write an algorithm to print all ways of arranging eight queens on an 8x8 chess board
        /// so that none of them share the same row, column, or diagonal.In this case, "diagonal" means all
        /// diagonals, not just the two that bisect the board.
        /// </summary>
        /// 
        public static void EightQueens()
        {
            EightQueensHelper(0, new int[Grid]);
            Console.WriteLine("Pause");
        }
        public static int Grid = 8;
        public static List<int[]> theGrid = new List<int[]>();
        public static void EightQueensHelper(int row, int[] places)
        {

            if (row == Grid)
            {
                int[] tempArray = new int[Grid];
                for (var i=0;i<places.Length;i++)
                {
                    tempArray[i] = places[i];
                }
                theGrid.Add(tempArray);
            }
            else
            {
                for (int col = 0; col < Grid; col++)
                {
                    if (CheckValid(places, row, col))
                    {

                        places[row] = col;
                        EightQueensHelper(row + 1, places);
                    }
                }
            }
        }

        public static bool CheckValid (int[] columns, int row1, int col1)
        {
            for (int row2=0;row2<row1;row2++)
            {
                int col2 = columns[row2];

                if (col1 == col2)
                    return false;
                int colDistance = Math.Abs(col2 - col1);

                int rowDistance = row1 - row2;
                if (colDistance == rowDistance)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Produce N first pairs of Valid Parens ()
        /// </summary>
        /// <param name="Produce N first pairs of Valid Parens ()"></param>
        public static void FirstNValidPairsParens(int pairs)
        {
            var outputs = FirstNValidPairsParensHelper(3, new List<List<string>>());
            int layer = 0;
            foreach (var ch in outputs)
            {
                Console.WriteLine("Layer :" + layer);
                foreach (var paren in ch)
                {
                    if (layer == 0)
                        Console.WriteLine("");
                    else
                        Console.WriteLine(paren);
                }
                layer++;
                Console.WriteLine(Environment.NewLine);
            }
        }

        public static List<List<string>> FirstNValidPairsParensHelper(int pairs, List<List<string>> overallMatrix)
        {
            if (pairs == 0)
            {
                overallMatrix.Add(new List<string> { "" });
                return overallMatrix;
            }

            
            overallMatrix = FirstNValidPairsParensHelper(pairs - 1, overallMatrix);
            List<string> tempLayer = FirstNValidPairsParensBuilder(overallMatrix[pairs-1]);
            overallMatrix.Add(tempLayer);
            return overallMatrix;
        }

        public static List<string> FirstNValidPairsParensBuilder(List<string> layer)
        {
            var set = new HashSet<string>();
            if (layer[0]=="")
            {
                
                layer[0] = "()";
                return layer;
            }
            List<string> tempLayer = new List<string>();
            foreach (var seq in layer)
            {  
                for (var ch=0;ch<seq.Length;ch++)
                {
                    if (seq[ch]=='(')
                    {
                        StringBuilder sb = new StringBuilder();

                        string temp = seq.Substring(0, ch);
                        sb.Append(temp);
                        sb.Append("()");
                        sb.Append(seq.Substring(ch));
                        //tempLayer.Add(sb.ToString());
                        if (!set.Contains(sb.ToString()))
                            set.Add(sb.ToString());

                        sb = new StringBuilder();

                        temp = seq.Substring(0, ch+1);
                        sb.Append(temp);
                        sb.Append("()");
                        sb.Append(seq.Substring(ch + 1));
                        //tempLayer.Add(sb.ToString());
                        if (!set.Contains(sb.ToString()))
                            set.Add(sb.ToString());
                    }
                }
            }
            foreach (var kes in set)
            {
                tempLayer.Add(kes);
            }
            return tempLayer;
        }
        
    }
}
