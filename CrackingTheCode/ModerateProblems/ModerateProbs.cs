using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.CrackingTheCode
{

    public static class ModerateProbs
    {
        /// <summary>
        /// Problem : 16.1
        /// Description : Write a function to swap a number in place (that is, without temporary variables).
        /// </summary>
        public static Tuple<int, int> NumberSwapper(int a, int b)
        {
            //Approach to swap elements (in position) without temps
            // var a -> b and var b - > a
            // a = 3 , b = 5
            // a = 3 + 5 = 8
            // b = a - b = 8 - 5 = 3
            // a = a - b = 8 - 3 = 5

            a = a + b;
            b = a - b;
            a = a - b;

            return new Tuple<int, int>(a, b);
        }

        /// <summary>
        /// Problem : 16.2
        /// Description : Design a method to find the frequency of occurrences of any given word in a
        /// book.What if we were running this algorithm multiple times?
        /// </summary>
        /// Solution :
        /// Just do pre-processing with cost of extra time and memory, design a HashMap and the lookup
        /// will be executed in O(1). If you have to check this for a single time then you have to go 
        /// through every word of the text and this will cost O(n) time.
        /// 

        /// <summary>
        /// Problem : 16.6
        /// Description : Given two arrays of integers, compute the pair of values (one value in each
        /// array) with the smallest(non-negative) difference.Return the difference.
        /// EXAMPLE
        /// Input: {l, 3, 15, 11, 2}, {23, 127, 235, 19, 8}
        /// Output: 3. That is, the pair(11, 8).
        /// Explanation : Sort two int[] arrays and set pointer1 to arr1[0] and pointer2 to arr2[0].
        /// Start moving pointers alternatively if pointer1<pointer2 .
        /// increase pointer1 to decrease the difference and vice versa.
        /// Sorting O(ALogA+BloGB) , Searching O(A+B) A = arr1.Count and B = arr2.Count
        /// So Overall Runtime -> O(ALogA + BLogB)
        public static void SmallestDifference(List<int> l1, List<int> l2)
        {
            l1.Sort(); //nlogn n = l1.Count-1
            l2.Sort(); //mlogm m = l2.Count-1

            //1,2,3,11,15     8,19,23,127,235

            if (l1.Count == 0 || l1 == null || l2.Count == 0 || l2 == null)
            {
                Console.WriteLine("Invalid Input");
                return;
            }

            int pointer1 = l1[0];
            int pointer2 = l2[0];

            int finalMinPointer1 = pointer1;
            int finalMinPointer2 = pointer2;

            int min = Math.Abs(pointer1 - pointer2);

            int pos1 = 0;
            int pos2 = 0;

            while (pos1 < l1.Count - 1 && pos2 < l2.Count - 1)
            {
                if (pointer1 <= pointer2)
                {
                    pos1++;
                    pointer1 = l1[pos1];
                }
                else
                {
                    pos2++;
                    pointer2 = l2[pos2];
                }
                int tempMin = Math.Abs(pointer1 - pointer2);
                if (tempMin < min)
                {
                    min = tempMin;
                    finalMinPointer1 = pointer1;
                    finalMinPointer2 = pointer2;
                }
                if (min == 0) break;
            }

            Console.WriteLine("The minimum pair is : " + min + $" and it's produced by ({finalMinPointer1},{finalMinPointer2})");
        }

        /// <summary>
        /// Problem : 16.10
        /// Description : Given a list of people with their birth and death years, implement a method to
        /// compute the year with the most number of people alive.You may assume that all people were born
        /// between 1900 and 2000 (inclusive). If a person was alive during any portion of that year, they should
        /// be included in that year's count. For example, Person (birth= 1908, death= 1909) is included in the
        /// counts for both 1908 and 1909.
        /// Time : O(nlogn) where n count of persons
        /// 

        public static int LivingPeople(List<Person> persons)
        {
            //birth: 01 10 10 12 13 20 23 75 83 90
            //death: 15 72 82 90 94 98 98 98 98 99

            var births = SortIt(persons, true);
            var deaths = SortIt(persons, false);

            int maxAlive = 0;
            int yearOccured = 0;
            int birthIndex = 0;
            int deathIndex = 0;
            int currentAlive = 0;

            while (birthIndex < births.Count)
            {
                if (births[birthIndex] <= deaths[deathIndex])
                {
                    currentAlive++;
                    if (currentAlive > maxAlive)
                    {
                        maxAlive = currentAlive;
                        yearOccured = births[birthIndex];
                    }
                    birthIndex++;
                }
                else if (births[birthIndex] > deaths[deathIndex])
                {
                    currentAlive--;
                    deathIndex++;
                }
            }
            return yearOccured;
        }

        public class Person
        {
            public int birthDate;
            public int deathDate;
        }

        public static List<int> SortIt(List<Person> persons, bool births)
        {
            var list = (births) ? persons.OrderBy(it => it.birthDate)?.Select(it => it.birthDate).ToList() : persons.OrderBy(it => it.deathDate)?.Select(it => it.deathDate).ToList();
            return list;
        }

        /// <summary>
        /// Problem : 16.11
        /// Description : You are building a diving board by placing a bunch of planks of wood end-to-end.
        /// There are two types of planks, one of length shorter and one of length longer.You must use
        /// exactly K planks of wood.Write a method to generate all possible lengths for the diving board.
        /// Runtime O(totalPlanks^2)
        public static void DivingBoard(int totalPlanks, int shorter, int longer)
        {
            var memoSet = SetPlanksMemo(totalPlanks, 0, shorter, longer, new HashSet<int>(), new Dictionary<Tuple<int, int>, HashSet<int>>());
            foreach (var item in memoSet)
            {
                Console.WriteLine("Available combinations with Memoiazation lead to length -> " + item);
            }

            var set = SetPlanks(totalPlanks, 0, shorter, longer, new HashSet<int>());
            foreach (var item in set)
            {
                Console.WriteLine("Available combinations lead to length -> " + item);
            }


        }

        public static HashSet<int> SetPlanks(int totalPlanks, int totalLength, int shorter, int longer, HashSet<int> lengths)
        {
            if (totalPlanks == 0)
            {
                lengths.Add(totalLength);
            }
            else
            {
                SetPlanks(totalPlanks - 1, totalLength + shorter, shorter, longer, lengths);
                SetPlanks(totalPlanks - 1, totalLength + longer, shorter, longer, lengths);
            }
            return lengths;
        }

        public static HashSet<int> SetPlanksMemo(int totalPlanks, int totalLength, int shorter, int longer, HashSet<int> lengths,
            Dictionary<Tuple<int, int>, HashSet<int>> complexHash)
        {

            if (complexHash.ContainsKey(new Tuple<int, int>(totalPlanks, totalLength)))
                return complexHash[new Tuple<int, int>(totalPlanks, totalLength)];
            if (totalPlanks == 0)
            {
                lengths.Add(totalLength);
                complexHash.Add(new Tuple<int, int>(totalPlanks, totalLength), lengths);
            }
            else
            {
                SetPlanksMemo(totalPlanks - 1, totalLength + shorter, shorter, longer, lengths, complexHash);
                SetPlanksMemo(totalPlanks - 1, totalLength + longer, shorter, longer, lengths, complexHash);
            }
            return lengths;
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
        public static void MasterMind(string guess)
        {
            //YRGB 2 pseudo-hit, 1 hit
            int position = 0;
            int pseudo = 0;
            int hit = 0;
            foreach (var ch in guess)
            {
                if (ComputersChoice.Contains(ch))
                {
                    bool flg = false;
                    int first = ComputersChoice.IndexOf(ch);
                    if (first == position) hit++;
                    else
                    {

                        while (first != -1)
                        {
                            first = ComputersChoice.IndexOf(ch, first + 1);
                            if (first == position) flg = true;
                        }
                    }
                    if (flg) hit++; else pseudo++;
                }
                position++;
            }
            Console.WriteLine("Hits : " + hit + " Pseudo-Hits : " + pseudo);
        }
        public static string ComputersChoice = "RGGB";

        /// <summary>
        /// Problem : 16.16
        /// Description : Given an array of integers, write a method to find indices m and n such that if you sorted
        /// elements m through n, the entire array would be sorted.Minimize n - m(that is, find the smallest
        /// such sequence).
        /// EXAMPLE
        /// Input: 1, 2, 4, 7, 10, 11, 7, 12, 6, 7, 16, 18, 19
        /// Output: (3, 9)
        /// TODO WITH BOOK APPROACH WHICH IS OPTIMIZED. THIS IS MY APPROACH USING RECURSION 
        /// </summary>
        /// 
        public static int MinIndex = -1;
        public static int MaxIndex = 100;
        public static List<int> GlobalLs = new List<int>();
        public static void SubSort()
        {
            List<int> ls = new List<int>
            { 1,2,4, 7, 10, 11, 7, 12, 6, 7, 16, 18, 19};
            foreach (var item in ls)
            {
                GlobalLs.Add(item);
            }
            GlobalLs.Sort();

            SubSortHelperMinIndex(ls, 0);
            SubSortHelperMaxIndex(ls, ls.Count - 1);
            Console.WriteLine($"Our Range is [{MinIndex},{MaxIndex})." + Environment.NewLine + $"Start at {MinIndex} and finish at {MaxIndex - 1}");
        }
        public static void SubSortHelperMinIndex(List<int> ls, int indexStart)
        {
            if (indexStart > ls.Count) return;
            List<int> temp = new List<int>();
            temp = ls.Skip(indexStart).ToList();
            temp.Sort();
            List<int> tempFinal = new List<int>();
            tempFinal.AddRange(ls.Take(indexStart));
            tempFinal.AddRange(temp);
            if (GlobalLs.SequenceEqual(tempFinal))
            {
                if (indexStart > MinIndex)
                    MinIndex = indexStart;
                indexStart++;
                SubSortHelperMinIndex(ls, indexStart);
            }
            else
                return;
        }

        public static void SubSortHelperMaxIndex(List<int> ls, int indexEnd)
        {
            if (indexEnd > ls.Count) return;
            List<int> temp = new List<int>();
            temp = ls.Take(indexEnd).ToList();
            temp.Sort();
            List<int> tempFinal = new List<int>();

            tempFinal.AddRange(temp);
            tempFinal.AddRange(ls.Skip(indexEnd));
            if (GlobalLs.SequenceEqual(tempFinal))
            {
                if (indexEnd < MaxIndex)
                    MaxIndex = indexEnd;
                indexEnd--;
                SubSortHelperMaxIndex(ls, indexEnd);
            }
            else
                return;
        }
        //TODO
        public static void SubSortHelperOptimized(List<int> ls)
        {
            ///left: 1, 2, 4, 7, 10, 11 -> max = 11
            ///middle: 8, 12
            ///right: 5, 6, 16, 18, 19 -> min = 5
            ///
            ///min(middle) > end(left)
            ///max(middle) < start(right)
        }

        /// <summary>
        /// Problem : 16.17
        /// Description : You are given an array of integers (both positive and negative). Find the
        /// contiguous sequence with the largest sum. Return the sum.
        /// EXAMPLE
        /// Input: 2, -8, 3, -2, 4, -10
        /// Output: 5 (i.e • , { 3, -2, 4} )
        /// </summary>
        public static void ContiguousSequence()
        {
            //2 3 -8 -1 2 4 -2 3
            int[] arr = new int[8] { 2, 3, -8, -1, 2, 4, -2, 3 };
            int sum = 0;
            int maxSum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
                if (sum > maxSum)
                    maxSum = sum;
                else if (sum < 0)
                    sum = 0;
            }
            Console.WriteLine($"The largest sum of the contiguous sequence is {maxSum}");
        }

        /// <summary>
        /// Problem : 16.18
        /// Description : You are given two strings, pattern and value. The pattern string consists of
        /// just the letters a and b, describing a pattern within a string. For example, the string catcatgocatgo
        /// matches the pattern aabab(where cat is a and go is b). It also matches patterns like a, ab, and b.
        /// Write a method to determine if value matches pattern.
        /// </summary>
        /// TODO
        public static void PatternMatching()
        {

        }

        /// <summary>
        /// Problem : 16.19
        /// Description : You have an integer matrix representing a plot of land, where the value at that location
        /// represents the height above sea level.A value of zero indicates water.A pond is a region of water
        /// connected vertically, horizontally, or diagonally. The size of the pond is the total number of
        /// connected water cells.Write a method to compute the sizes of all ponds in the matrix.
        /// EXAMPLE
        /// Input:
        /// 0 2 1 0
        /// 0 1 0 1
        /// 1 1 0 1
        /// 0 1 0 1
        /// Output: 2, 4, 1 (in any order)
        /// <Time>O(WH) where W width and H height</Time>
        /// Another way to compute this is to think about how many times each cell is "touched" by either call. Each cell
        /// will be touched once by the c omputePondSizes function.Additionally, a cell might be touched once by
        /// each of its adjacent cells.This is still a constant number of touches per cell. Therefore, the overall runtime is
        /// O(N2) on an NxN matrix or, more generally, O(WH).
        public static List<int> PondSizes()
        {
            int[,] land = new int[5, 4]
            {
                {0,2,1,0 },
                {0,1,0,1 },
                {1,1,0,1 }
                ,{0,1,0,1}
                ,{0,0,0,0}
            };
            int rowSize = land.GetLength(0);
            int colSize = land.GetLength(1);
            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < colSize; j++)
                {
                    if (land[i, j] == 0)
                    {
                        int size = CalculatePondSize(i, j, land);
                        Ponds.Add(size);
                    }
                }
            }

            return Ponds;
        }

        public static List<int> Ponds = new List<int>();

        public static int CalculatePondSize(int row, int col, int[,] land)
        {
            int rowSize = land.GetLength(0) - 1;
            int colSize = land.GetLength(1) - 1;
            if (row > rowSize || row < 0 || col > colSize || col < 0 || land[row, col] != 0)
                return 0;

            land[row, col] = -1;
            int size = 1;
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    size += CalculatePondSize(row + dr, col + dc, land);
                }
            }

            return size;
        }

        /// <summary>
        /// Problem : 16.20
        /// Description : T9: On old cell phones, users typed on a numeric keypad and the phone would provide a list of words
        /// that matched these numbers.Each digit mapped to a set of O - 4 letters.Implement an algorithm
        /// to return a list of matching words, given a sequence of digits.You are provided a list of valid words
        /// (provided in whatever data structure you'd like). The mapping is shown in the diagram below:
        /// </summary>
        /// <Time>O(4^n) where n is the length of the string and n is the available letters for each number in the input</Time>
        //This is the mobile mapping for each number
        public static Dictionary<int, char[]> NumToWord = new Dictionary<int, char[]>
        {
            {8 , new char[3]{'t','u','v' } },{7, new char[4]{'p','q','r','s' } }, {3, new char[3]{'d','e','t'} }
        };
        //The list with the possible words
        public static List<string> availableWords = new List<string>();
        //The set with the valid words
        public static HashSet<string> validWords = new HashSet<string> { "tree", "used" };
        public static List<string> T9Mobile(string input)
        {

            char[] words = NumToWord[Convert.ToInt32(input[0].ToString())];
            for (var i = 0; i < words.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(words[i]);
                //T9MobileHelper(input.Substring(1),sb,1);
                T9MobileHelper(input.ToCharArray(), sb, 1);
            }
            var itemsToReturn = availableWords.Where(word => validWords.Contains(word))?.ToList();
            return itemsToReturn;
        }

        /*public static void T9MobileHelper(string input, StringBuilder sb, int index)
        {
            if (index > input.Length-1)
            {
                availableWords.Add(sb.ToString());
                return;
            }
            
            char[] words = NumToWord[Convert.ToInt32(input[0].ToString())];
            for (var i = 0; i < words.Length; i++)
            {
                sb.Append(words[i]);
                T9MobileHelper(input.Substring(index), sb, index + 1);
                sb.Remove(sb.Length - 1, sb.Length - 1);
            }
        }*/
        public static void T9MobileHelper(char[] input, StringBuilder sb, int index)
        {
            if (index > input.Length - 1)
            {
                availableWords.Add(sb.ToString());
                return;
            }

            char[] words = NumToWord[Convert.ToInt32(input[index].ToString())];
            for (var i = 0; i < words.Length; i++)
            {
                sb.Append(words[i]);
                T9MobileHelper(input, sb, index + 1);
                string temp = sb.ToString();
                sb.Clear();
                sb.Append(temp.Substring(0, temp.Length - 1));
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
        /// <Time>Time for Iterative solution is O(A+B) 
        /// but sorting will cost O(AlogA) + O(BlogB) which will be our final time complexity cause its greater than A+B</Time>
        
        public static List<Tuple<int, int>> SumSwap()
        {
            //1,1,1,2,2,4 -> 11
            //6,3,3,3, -> 15
            int[] arr1 = new int[6] { 1, 2, 4, 2, 1, 1 };
            int[] arr2 = new int[4] { 3, 6, 3, 3 };

            //sum1 - a + b = sum2 + a - b
            // a - b = (sum1-sum2)/2
            SumSwapHelperHash(arr1, arr2);

            arr1 = arr1.OrderBy(it => it).ToArray();
            arr2 = arr2.OrderBy(it => it).ToArray();
            return SumSwapHelper(arr1, arr2);
        }

        //Optimal Solution with Hashset
        //This runs in O(A+B)
        public static Tuple<int,int> SumSwapHelperHash(int[] arr1, int[] arr2)
        {
            int target = FindTarget(arr1, arr2);
            if (target == -1) return new Tuple<int, int>(-1,-1);

            HashSet<int> localSet = new HashSet<int>();
            foreach ( var val in arr1)
            {
                localSet.Add(val);
            }

            // a- b = target --> a = target + b
            foreach (var val in arr2)
            {
                int lookUpTarget = target + val; //target + b;
                if (localSet.Contains(lookUpTarget))
                    return new Tuple<int, int>(lookUpTarget, val);
            }
            return new Tuple<int, int>(-1, -1);
        }

        //Alternate Solution with Iterate
        public static List<Tuple<int, int>> SumSwapHelper(int[] arr1, int[] arr2)
        {
            int? target= FindTarget(arr1, arr2);

            if (target == -1) return new List<Tuple<int, int>>();

            int indexa = 0;
            int indexb = 0;
            var output = new List<Tuple<int, int>>();

            while (indexa<arr1.Length && indexb<arr2.Length)
            {
                //1,1,1,2,2,4 -> 11
                //3,3,3,6 -> 15
                int difference = arr1[indexa] - arr2[indexb];
                if (difference == target)
                    output.Add(new Tuple<int, int>(indexa, indexb));
                else if (difference < target)
                    indexa++;
                else
                    indexb++;
            }
            return output;
        }

        public static int FindTarget(int[] arr1, int[] arr2)
        {
            //sum1 - a + b = sum2 + a - b
            // a - b = (sum1-sum2)/2
            int sum1 = arr1.Sum();
            int sum2 = arr2.Sum();
            if ((sum1 - sum2) % 2 != 0) return -1;
            return (sum1 - sum2) / 2;
        }

        /// <summary>
        /// Problem : 16.24
        /// Description : Design an algorithm to find all pairs of integers within an array which sum to a
        /// specified value.
        /// </summary>
        /// Solution : Sort the array and iterate over it. For each element apply Binary Search for the complement
        /// item1 + item2 = target --> item1 = target - item2 or complement = target- currentItem
        /// <Time>O(nlogn)</Time>
        /// Discuss tradeoff for hashset optimization which will cost additionaly O(N) space but can avoid duplicate checks.
        public static void PairsWithSum (List<int> input,int target)
        {
            //3,4,12,5,2,2
            //--> 2,2,3,4,5,12 and target = 9
            //--> (4,5), (5,4) pairs (Remove Dups if needed? Discuss with interviewer)

            var output = new List<Tuple<int, int>>();
            var set = new HashSet<int>();
            input= input.OrderBy(it => it)?.ToList();
            //it1 + it2 = 9 --> 9-it2 = it1
            for (int i = 0; i < input.Count; i++)
            {
                if (set.Contains(input[i]))
                    continue;
                else if (input[i] >= target)
                    break;
                else
                {
                    int secondItem = PairsWithSumHelper(input, 0, input.Count - 1, target - input[i]);
                    if (secondItem != -1)
                        output.Add(new Tuple<int, int>(input[i], secondItem));
                    else
                        set.Add(input[i]);
                }
            }
        }
        
        public static int PairsWithSumHelper(List<int> input , int start, int finish, int target)
        {
            if (start > finish)
                return -1;
            int mid = (start + finish) / 2;
            
            if (input[mid] == target)
                return input[mid];
            else
            {
                if (input[mid] > target)
                    return PairsWithSumHelper(input, start, mid - 1, target);
                else
                    return PairsWithSumHelper(input, mid + 1, finish, target);
            }
        }
        /// <summary>
        /// Problem : 16.26
        /// Description : Given an array of integers, write a method to find indices m and n such that if you sorted
        /// elements m through n, the entire array would be sorted.Minimize n - m(that is, find the smallest
        /// such sequence).
        /// EXAMPLE
        /// Input: 1, 2, 4, 7, 10, 11, 7, 12, 6, 7, 16, 18, 19
        /// Output: (3, 9)
        /// </summary>
        public static HashSet<char> NewSet = new HashSet<char> { '*', '-', '+', '/' };
        public static int Calculator(string input)
        {
            if (input.Length == 0) return 0;

            // [2 * 3] +[ 5 / 6 * 3 ]- 15
            //2*3+5/6*3+15
            //15
            //3 +
            //6 *
            //5 /
            //3 +
            //2 *
            Stack<int> numbersStack = new Stack<int>();
            Stack<char> signStack = new Stack<char>();

            foreach(var ch in input)
            {
                if (NewSet.Contains(ch))
                    signStack.Push(ch);
                else
                    numbersStack.Push(Convert.ToInt32(ch));
            }
            
            int finalNumber = numbersStack.Pop();
            int tempNum;
            char tempSign;
            var sequentialFlag = false;
            for (var i=0;i<numbersStack.Count;i++)
            {
                tempNum = numbersStack.Pop();
                tempSign = signStack.Pop();
                if (tempSign == '+' && (signStack.Peek() == '+' || signStack.Peek() =='-'))
                    finalNumber += tempNum;
                else if (tempSign == '-' && (signStack.Peek() == '+' || signStack.Peek() == '-'))
                    finalNumber -= tempNum;
                else
                {
                    while (tempSign != '+' || tempSign != '-')
                    {
                        if (tempSign == '*')
                        {
                            tempNum = tempNum * numbersStack.Pop();
                        }
                        else // '/'
                        {
                            tempNum = tempNum / numbersStack.Pop();
                        }
                        tempSign = signStack.Pop();
                    }
                    //finalNumber +=
                }
            }
            return finalNumber;
        }

        /// <summary>
        /// <Author>Hackerrank - https://www.hackerrank.com/challenges/queens-attack-2/problem?isFullScreen=true </Author>
        /// You will be given a square chess board with one queen and a number of obstacles placed on it. Determine how many squares the queen can attack.
        /// A queen is standing on an chessboard.The chess board's rows are numbered from  to , going from bottom to top. Its columns are numbered from  to , going from left to right. Each square is referenced by a tuple, , describing the row, , and column, , where the square is located.
        /// The queen is standing at position.In a single move, she can attack any square in any of the eight directions (left, right, up, down, and the four diagonals). In the diagram below, the green circles denote all the cells the queen can attack from :
        /// There are obstacles on the chessboard, each preventing the queen from attacking any square beyond it on that path. For example, an obstacle at location  in the diagram above prevents the queen from attacking cells , , and :
        /// 
        /// 5 3
        /// 4 3
        /// 5 5
        /// 4 2
        /// 2 3
        /// Sample Output 1
        /// 10
        /// Explanation 1
        /// The queen is standing at position (4,3) on a (5,5) chessboard with k = 3 obstacles:
        /// The number of squares she can attack from that position is 10 .
        /// </summary>
        public static int totalBlocks = 0;
        public static HashSet<Tuple<int, int>> Hset = new HashSet<Tuple<int, int>>();
        public static int queensAttack(int n, int k, int r_q, int c_q, List<List<int>> obstacles)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                Hset.Add(new Tuple<int, int>(obstacles[i][0], obstacles[i][1]));
            }

            UpperLeftHelper(n, r_q - 1, c_q - 1, obstacles);
            UpperHelper(n, r_q - 1, c_q, obstacles);
            UpperRightHelper(n, r_q - 1, c_q + 1, obstacles);
            LeftHelper(n, r_q, c_q - 1, obstacles);
            RightHelper(n, r_q, c_q + 1, obstacles);
            LowerLeftHelper(n, r_q + 1, c_q - 1, obstacles);
            LowerHelper(n, r_q + 1, c_q, obstacles);
            LowerRightHelper(n, r_q + 1, c_q + 1, obstacles);


            return totalBlocks;
        }

        public static void UpperLeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            UpperLeftHelper(n, row - 1, col - 1, obstacles);
            return;
        }
        public static void UpperHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            UpperHelper(n, row - 1, col, obstacles);
            return;
        }
        public static void UpperRightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            UpperRightHelper(n, row - 1, col + 1, obstacles);
            return;
        }
        public static void LeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            LeftHelper(n, row, col - 1, obstacles);
            return;
        }
        public static void RightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            RightHelper(n, row, col + 1, obstacles);
            return;
        }
        public static void LowerLeftHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            LowerLeftHelper(n, row + 1, col - 1, obstacles);
            return;
        }
        public static void LowerHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            LowerHelper(n, row + 1, col, obstacles);
            return;
        }
        public static void LowerRightHelper(int n, int row, int col, List<List<int>> obstacles)
        {
            if (ShouldReturn(row, col, n, obstacles)) return;
            totalBlocks++;
            LowerRightHelper(n, row + 1, col + 1, obstacles);
            return;
        }

        public static bool ShouldReturn(int row, int col, int n, List<List<int>> obstacles)
        {
            if (row < 1 || row > n)
                return true;
            if (col < 1 || col > n)
                return true;
            return isObstacle(row, col, obstacles);
        }

        public static bool isObstacle(int r, int c, List<List<int>> obstacles)
        {
            if (Hset.Contains(new Tuple<int, int>(r, c)))
                return true;
            return false;
        }

        /// <summary>
        /// HackerRank https://www.hackerrank.com/challenges/simple-text-editor/problem?isFullScreen=true
        /// Implement a simple text editor. The editor initially contains an empty string, S. Perform Q operations of the following 4 types:
        /// 1 append(W) - Append string W to the end of S.
        /// 2 delete - Delete the last k characters of S.
        /// 3 print - Print the kth character of s.
        /// 4 undo - Undo the last (not previously undone) operation of type 1 or 2, reverting S to the state it was in prior to that operation.
        /// Example
        /// S = 'abcde'
        /// ops = ["1fg","36","25","4","37","4","34"]
        /// operation
        /// index S       ops[index] explanation

        /// 0       abcde   1 fg append fg
        /// 1       abcdefg 3 6         print the 6th letter - f
        /// 2       abcdefg 2 5         delete the last 5 letters
        /// 3       ab      4           undo the last operation, index 2
        /// 4       abcdefg 3 7         print the 7th characgter - g
        /// 5       abcdefg 4           undo the last operation, index 0
        /// 6       abcde   3 4         print the 4th character - d
        /// </summary>

        public static void TextEditor(string input, List<string> operations)
        {
            //precompute to reform the operations in a desired more readable format
            var textEditorOperations = new List<Tuple<int, string>>();
            foreach (var operation in operations)
            {
                if (operation[0].ToString()=="4")
                    textEditorOperations.Add(new Tuple<int, string>(4, null));
                else
                    textEditorOperations.Add(new Tuple<int, string>(Convert.ToInt32(operation[0].ToString()), operation.Substring(1)));
            }

            var stack = new Stack<string>();
            stack.Push(input);
            //Use a stack to keep all the previous states. Can be done with a linked list as well.
            foreach (var operation in textEditorOperations)
            {
                if (operation.Item1 == 4) //Undo
                {
                    if (stack.Count > 0)
                        stack.Pop();
                }
                else
                { 
                    string peek = stack.Peek();

                    if (operation.Item1 == 1) //Append
                    {
                        stack.Push(peek + operation.Item2);
                    }
                    else if (operation.Item1 == 2) //Delete
                    {
                        stack.Push(peek.Substring(0, peek.Length - Convert.ToInt32(operation.Item2)));
                    }
                    else if (operation.Item1 == 3) //Print
                    {
                        Console.WriteLine(peek[Convert.ToInt32(operation.Item2)-1]);
                    } 
                    else //Not Valid
                    {
                        Console.WriteLine($"There is no such condition {operation.Item1}");
                    }
                }
            }
        }
    }
}
