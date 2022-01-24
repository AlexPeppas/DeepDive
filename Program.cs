using DeepDiveTechnicals.CrackingTheCode;
using DeepDiveTechnicals.CrackingTheCode.ObjectOrientedDesign;
using DeepDiveTechnicals.DataStructures.ArraysAndStrings;
using DeepDiveTechnicals.DataStructures.LinkedList;
using DeepDiveTechnicals.DataStructures.TreesAndGraphs;
using DeepDiveTechnicals.DynamicProgramming;
using DeepDiveTechnicals.Services;
using DeepDiveTechnicals.SortingSearching;
using Lucene.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals
{
    class Program
    {
        public enum Map
        {
            comp = 0
        };

        public static void Main(string[] args)
        {
    
            #region My Custom Problems
            MyCustomProblems.RatInMazeCached();
            #endregion

            #region Hard Problems
            HardProblems.ReSpace(); // HARD. Used a Trie. Nice Technique
            HardProblems.BiNodeFunc();
            HardProblems.BabyNames();
            HardProblems.LettersAndNumber(); //HARD. Nice Technique
            HardProblems.MissingNumberNonBinary(); //Do the binary approach
            HardProblems.Shuffle();
            HardProblems.RandomSet();
            #endregion

            #region Twitter 1st
            FrequentlyAskedQuestions.Interview.TwitterMain(new List<Tuple<int, long>>(), 2);
            //input larget dataSet and binWidth (for histogram chunks) = 2 etc. [0,1] [2,3] [4,5]
            #endregion

            #region Microsoft Final Problem
            FrequentlyAskedQuestions.Interview.FindPathRobotInGridWithCycle(); //HARD MICROSOFT
            #endregion

            #region Hashing 
            long hashWithShift = new Hashing().UniqueHashShifting(
            "Tesla Community in California, Please help stop the Solar Tax by "
            + " GavinNewsom  ! Make your voice heard! All the information is posted here!"
            + " @Tesla @TeslaSolar @elonmusk Please RT and get the word out.");
            long hash =new Hashing().UniqueHash("01012022210324510005000 This is my very first unique hash of my very first tweet. Hello World I would like to recommend and adjust my previous tweets and also add some pictures and gifs and pngs. I will retweet about crypto and politics");
            long hashNoShift = new Hashing().UniqueHash("Hello World. This is a large set of data to be hashed so I am trying to overflow my long variable. Author AlexP and Date : 15/01/2022.");
            #endregion

            #region LRU CACHE DESIGN 
            FrequentlyAskedQuestions.LRU cache = new FrequentlyAskedQuestions.LRU(3);
            cache.Add(1, "Alex");
            cache.Add(2, "Lina");
            var temp =cache.Get(1);
            cache.Add(3, "Ermioni");
            temp = cache.Get(2);
            cache.Add(4, "Spyros");
            cache.Add(5, "Katerina");

            #endregion

            #region Frequently Asked Questions
            FrequentlyAskedQuestions.Interview.BinaryTreeToDLL(); // AMAZON Medium
            FrequentlyAskedQuestions.Interview.KLargestElements(2); // AMAZON Easy
            FrequentlyAskedQuestions.WarmUp.LCA();
            FrequentlyAskedQuestions.Interview.StrictlyIncreasingPathTwitter(); //HARD TWITTER
            FrequentlyAskedQuestions.Interview.KSortedArray();
            FrequentlyAskedQuestions.Interview.CyclesInDirectedGraphDFS();
            FrequentlyAskedQuestions.Interview.CyclesInDirectedGraph();
            FrequentlyAskedQuestions.Interview.LowestCommonAncestor();
            FrequentlyAskedQuestions.Interview.TreeWithDuplicateSubTreesDepth2();
            var groupOfAnagrams = FrequentlyAskedQuestions.Interview.GroupAnagrams(new List<string>());
            FrequentlyAskedQuestions.Interview.RatWithTuples();
            FrequentlyAskedQuestions.Interview.PrintAllPermutations("abc");
            FrequentlyAskedQuestions.Interview.GraphClone();
            FrequentlyAskedQuestions.Interview.FirstCommonAncestor();
            FrequentlyAskedQuestions.Interview.LeftSideOfTree();
            FrequentlyAskedQuestions.Interview.RatInMaze();
            FrequentlyAskedQuestions.Interview.SerializeTree(null);
            FrequentlyAskedQuestions.Interview.CheckIfTreeContainsDuplicateSubTrees();
            FrequentlyAskedQuestions.Interview.CheckIfSubtreeOptimized();
            FrequentlyAskedQuestions.Interview.PrintNodesAtOddLevels();
            FrequentlyAskedQuestions.Interview.PrintRootToLeafPathsNoRecursion(new FrequentlyAskedQuestions.Node(0));
            FrequentlyAskedQuestions.Interview.PathToNode(new FrequentlyAskedQuestions.Node(0),new FrequentlyAskedQuestions.Node(1));
            FrequentlyAskedQuestions.Interview.FindCommonAncestorBST();
            RecursionAndDynamicProgramming.RecursionAndDynamic.FirstNValidPairsParens(3);
            #endregion

            #region Moderate Problems
            ModerateProbs.PairsWithSum(new List<int> { 3, 4, 12, 5, 2, 2 }, 9);
            ModerateProbs.SumSwap();
            ModerateProbs.T9Mobile("8733");
            ModerateProbs.PondSizes();
            ModerateProbs.SubSort();
            ModerateProbs.ContiguousSequence();
            ModerateProbs.MasterMind("YRGG");
            ModerateProbs.DivingBoard(6, 5, 10);
            ModerateProbs.LivingPeople(new List<ModerateProbs.Person> 
            { 
                new ModerateProbs.Person {birthDate=12,deathDate=15 },
                new ModerateProbs.Person {birthDate=20,deathDate=90 },
                new ModerateProbs.Person {birthDate=10,deathDate=98 },
                new ModerateProbs.Person {birthDate=01,deathDate=72 },
                new ModerateProbs.Person {birthDate=10,deathDate=98 },
                new ModerateProbs.Person {birthDate=23,deathDate=82 },
                new ModerateProbs.Person {birthDate=13,deathDate=98 },
                new ModerateProbs.Person {birthDate=90,deathDate=98 },
                new ModerateProbs.Person {birthDate=83,deathDate=99 },
                new ModerateProbs.Person {birthDate=75,deathDate=94 },
            });
            ModerateProbs.SmallestDifference(new List<int> { 1, 3, 15, 11, 2 }, new List<int> {23,127,235,19,8 });
            var numberSwapperOutput = ModerateProbs.NumberSwapper(-15, 39);
            Console.WriteLine(numberSwapperOutput.Item1 + " " + numberSwapperOutput.Item2);
            #endregion

            #region Sorting and Searching
            string stringTosearch = "car";
            SortingAndSearching.SparseSearch(new List<string>
            {"at","","","","ball","car","","","","dad","","" }, stringTosearch);

            string testDecodeString = "13522";
            testDecodeString  = new String(testDecodeString.Reverse().ToArray());
            SortingAndSearching.Decode(testDecodeString);
            SortingAndSearching.DecodeMemoization("4321");

            List<int> sortedList = new List<int>
            {
                1,5,7,10,15,16,17,19,45,59,60,85,100,900,1001,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
                ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
                ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
                ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
            };
            SortingAndSearching.SortedSearchNoSize(sortedList, 59);
            Console.WriteLine(SortingAndSearching.SearchInRotatedArrayByBook(new List<int> {6, 8, 10, 11, 9, 9, 9, 9, 9, 9 },0,9, 10));
            SortingAndSearching.SearchInRotatedArray(new List<int> {15,16,19,25,1,3,4,5,9,10 }, 10);

            List<string> words = new List<string> { "dog", "taco", "cato", "god", "gods", "tacos", "ball" };
            //var output = SortingAndSearching.GroupOfAnagrams(words);
            //GOLDMAN SACHS AGAIN ************************************************************
            var output = SortingAndSearching.GroupOfAnagramsByBook(words);
            //GOLDMAN SACHS AGAIN ************************************************************

            int[] largeArray = new int[12] { 1, 15, 28, 100, -1, -1, -1, -1,-1,-1,-1,-1 };
            int[] smallArray = new int[4] { 4, 5, 6, 27 };
            int indexOfLastItem = 4;
            SortingAndSearching.SortedMerge(largeArray,smallArray , indexOfLastItem);

            SortingAndSearching.BinarySearch(new int[7] {3,15,29,50,51,60,100 },0,6,100);
            SortingAndSearching.QuickSort(new int[5] { 3, 1, 5, 7, 4 }, 0, 4);
            #endregion

            #region Recursion
            RecursionAndDynamicProgramming.RecursionAndDynamic.EightQueens();
            RecursionAndDynamicProgramming.RecursionAndDynamic.PaintHelper();
            var parens = RecursionAndDynamicProgramming.RecursionAndDynamic.Parens(3,0);
            var permWithoutDups = RecursionAndDynamicProgramming.RecursionAndDynamic.PermutationsWithoutDupsS("321");
            var returPermWithDups = RecursionAndDynamicProgramming.RecursionAndDynamic.PermutationsWithoutDups(new List<string> { "3", "2", "1" }, 0);
            var returdT = RecursionAndDynamicProgramming.RecursionAndDynamic.PowerSet(new List<int> { 0,1, 2, 3 },0);
            RecursionAndDynamicProgramming.RecursionAndDynamic.MagicIndex(new List<int>());
            int magic = RecursionAndDynamicProgramming.RecursionAndDynamic.MagicIndexDuplicates(new List<int>());
            RecursionAndDynamicProgramming.RecursionAndDynamic.RobotInAGrid();
            RecursionAndDynamicProgramming.RecursionAndDynamic.FibMain();
            Console.WriteLine(RecursionAndDynamicProgramming.RecursionAndDynamic.Fibo(6));
            #endregion

            #region Polymorphism
            InheritancePractices.MainFunc();
            #endregion

            #region Trees
            TreesAndGraphs.FirstCommonNode();
            TreesAndGraphs.BuildOrder();
            var partitionOutput = LinkedLists.Partition(new LinkedListStruct(0), 90);
            LinkedLists.RemoveDups();
            #endregion

            #region Arrays And Strings

            var stringCompressionOutput = ArraysAndStrings.StringCompression("aabccccaaa");
            stringCompressionOutput = ArraysAndStrings.StringCompression("abcaaabccdeefqqkddddddddp");
            stringCompressionOutput = ArraysAndStrings.StringCompression("aabbbbbbbbscccsadpqwww");

            var oneAwayOutput = ArraysAndStrings.OneAway("pale","ple");
            oneAwayOutput = ArraysAndStrings.OneAway("pales", "pale");
            oneAwayOutput = ArraysAndStrings.OneAway("pale", "bale");
            oneAwayOutput = ArraysAndStrings.OneAway("pale", "bake");
            var r = ArraysAndStrings.CheckPermutation("abcd", "cbda");
            var urlifyOutput = ArraysAndStrings.URLify("Mr John Smith ");
            #endregion

            #region Previous Preparation
            permutation("ale","");
            string stop = string.Empty;

            //
            //array1: 50 5 20 30 40
            //array2 : 5 10 20 0 2
            //array3 : 5 10 30 20 5
            int[] arr = new int[5];
            arr[0] = 5; arr[1] = 6; arr[2] = 20; arr[3] = 10; arr[4] = 5;
            var rotatedArray = new SearchRotatedArray().Sort(arr, 0, arr.Length - 1, 10);
            //
            var result = new Parenthesis().isValid();
            new MagicIndex().ComputeWithFast();
            //1 , 1 , 2 , 3 , 5 , 8
            int N = 6;
            var fiboN = Fibonacci.Compute(N);
            var fiboNRecursive = Fibonacci.ComputeRecursively(0,1,N,2);
            var fiboNCache = Fibonacci.CacheFiboOptimization(N);
            var fiboCache = Fibonacci.CacheReturnStack(N);



            var minimalTree = new MinimalTree();
            Node node = minimalTree.CreateBinaryTree(new List<int> { 1,5,7,10,15,20,25,30});

            var linkedlist = new RemoveDups();
            linkedlist.main();
            
            //dummy tree traverse
            var treeLogic = new TreeLogic();
            treeLogic.TreeTraverse();
            //

            //dummy order date for balancer
            var datesOrdered = Order_DateTimes.OrderDate();
            //

            // table manipulation BP Services
            var tableManip = new TableManipulation();
            tableManip.Manipulate();
            //

            var dict = new Dictionary<int, int>{ { 0, 2 } };
            var dum = 5;
            var pos = dict.FirstOrDefault(it => it.Value > dum);
            if (pos.Value == 0) throw new Exception("Empty Dict");
            Console.WriteLine("pos" + pos);

            #endregion
        }

        public static void ParseInt(string input)
        {
            //"1012"
            int res = input[0] - 48;
            for (int i=1; i<input.Length;i++)
            {
                res *= 10;
                int tempCode = input[i] - 48;
                res += tempCode;
            }
        }

        public static void permutation(string str, string prefix)
        {
            if (str.Count() == 0)
            {
                Console.WriteLine(prefix);
            }
            else
            {
                for (int i = 0; i < str.Count(); i++)
                {
                    string rem = str.Substring(0, i) + str.Substring(i + 1);
                    permutation(rem, prefix + str[i]);
                }
            }
        }
    }

}
