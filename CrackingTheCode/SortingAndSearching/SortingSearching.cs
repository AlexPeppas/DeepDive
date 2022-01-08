using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiveTechnicals.CrackingTheCode
{
    public static class SortingAndSearching
    {
        /// <summary>
        /// WarmUp : BinarySearch
        /// </summary>
        public static void BinarySearch(int[] arr, int left, int right, int target)
        {
            if (arr.Length == 0)
            {
                Console.WriteLine("No Target");
                return;
            }

            if (left > right)
            {
                Console.WriteLine("No Target");
                return;
            }
            if (left == right)
            {
                if (arr[left] == target)
                {
                    Console.WriteLine("Target's index: " + left);
                    return;
                }

                Console.WriteLine("No Target");
                return;

            }
            int mid = (left + right) / 2;

            if (target == arr[mid])
            {
                Console.WriteLine("Target's index : " + mid);
                return;
            }

            if (target < arr[mid])
                BinarySearch(arr, left, mid - 1, target);
            else
                BinarySearch(arr, mid + 1, right, target);


        }

        /// <summary>
        /// WarmUp : QuickSort
        /// </summary>
        public static void QuickSort(int[] arr, int left, int right)
        {
            int index = Partition(arr, left, right);
            if (left < index - 1)
                QuickSort(arr, left, index - 1);
            if (index < right)
                QuickSort(arr, index, right);
        }

        public static int Partition(int[] arr, int left, int right)
        {
            int pivot = arr[(left + right) / 2];
            while (left <= right)
            {
                while (arr[left] < pivot)
                    left++;
                while (arr[right] > pivot)
                    right--;

                if (left <= right)
                {
                    int temp = arr[right];
                    arr[right] = arr[left];
                    arr[left] = temp;
                    left++;
                    right--;
                }
            }
            return left;
        }

        /// <summary>
        /// Problem : 10.1
        /// Description : You are given two sorted arrays, A and B, where A has a large enough buffer at the
        /// end to hold B.Write a method to merge B into A in sorted order.
        /// </summary>
        public static void SortedMerge(int[] arrLarge, int[] arrSmall, int arrLastItemIndex)
        {
            int leftPointer = 0;
            int rightPointer = 0;
            int currentPointer = arrLastItemIndex;

            while (leftPointer < arrLastItemIndex && rightPointer < arrSmall.Length)
            {
                if (arrLarge[leftPointer] < arrSmall[rightPointer])
                {
                    arrLarge[currentPointer] = arrLarge[leftPointer];
                    currentPointer++;
                    leftPointer++;
                }
                else if (arrLarge[leftPointer] >= arrSmall[rightPointer])
                {
                    arrLarge[currentPointer] = arrSmall[rightPointer];
                    rightPointer++;
                    currentPointer++;
                }
            }
            while (leftPointer < arrLastItemIndex)
            {
                arrLarge[currentPointer] = arrLarge[leftPointer];
                leftPointer++;
                currentPointer++;
            }
            while (rightPointer < arrSmall.Length)
            {
                arrLarge[currentPointer] = arrSmall[rightPointer];
                rightPointer++;
                currentPointer++;
            }
        }

        /// <summary>
        /// Problem : 10.2
        /// Description : Write a method to sort an array of strings so that all tne anagrnms are next to each other.
        /// </summary>
        /// 
        //Custom NEEDS TO BE FIXED
        public static List<string> GroupOfAnagrams(List<string> words)
        {
            //dog , cat , god , tac , ball

            Dictionary<Dictionary<char, int>, List<int>> complexHash = new Dictionary<Dictionary<char, int>, List<int>>();
            int wordIndex = 0;
            foreach (var word in words)
            {
                var tempDict = word.GroupBy(it => it).ToDictionary(item => item.Key, item => item.Count());
                tempDict = tempDict.OrderBy(it => it.Key).ToDictionary(item => item.Key, item => item.Value);
                if (complexHash.ContainsKey(tempDict))
                {
                    complexHash[tempDict].Add(wordIndex);
                }
                else
                {
                    complexHash.Add(tempDict, new List<int> { wordIndex });
                }
                wordIndex++;
            }
            var outputstring = new List<string>();
            foreach (var key in complexHash)
            {
                foreach (var value in key.Value)
                {
                    outputstring.Add(words[value]);
                }
            }
            return outputstring;
        }

        public static List<string> GroupOfAnagramsByBook(List<string> words)
        {
            Dictionary<string, List<string>> complexHash = new Dictionary<string, List<string>>();

            foreach (var word in words)
            {
                string key = SortChars(word);
                if (complexHash.ContainsKey(key))
                    complexHash[key].Add(word);
                else
                    complexHash.Add(key, new List<string> { word });
            }

            var helperList = new List<string>();

            foreach (var key in complexHash)
            {
                foreach (var word in key.Value)
                {
                    helperList.Add(word);
                }
            }
            return helperList;
        }

        public static string SortChars(string word)
        {
            var sch1 = word.ToCharArray();
            Array.Sort(sch1);

            return new string(sch1);
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
        public static int SearchInRotatedArray(List<int> list, int target)
        {
            int index = SearchInRotatedArray(list, 0, list.Count - 1, target);
            if (index == -1) throw new Exception("The target does not exist in the list");
            Console.WriteLine("Target's index : " + index);
            return index;
        }
        private static int SearchInRotatedArray(List<int> list, int start, int end, int target)
        {
            //{ 15, 16, 19, 20, 25, 1, 3, 4, 5, 7, 10, 14}
            if (start > end) return -1;

            int mid = (start + end) / 2;

            if (list[mid] == target) return mid;

            if (list[start] == target) return start;

            if (list[start] < target)
            {
                return SearchInRotatedArray(list, start + 1, mid - 1, target);
            }
            else
            {
                if (list[end] == target) return end;
                return SearchInRotatedArray(list, mid + 1, end - 1, target);
            }
        }

        public static int SearchInRotatedArrayByBook(List<int> list, int start, int end, int target)
        {
            if (start > end) return -1;

            int mid = (start + end) / 2;

            if (list[mid] == target) return mid;

            if (list[start] < list[mid]) //ordered normaly left
            {
                if (list[start] <= target && target < list[mid]) //between left set
                    return SearchInRotatedArrayByBook(list, start, mid - 1, target); //search left
                else
                    return SearchInRotatedArrayByBook(list, mid + 1, end, target); //search right
            }
            else if (list[end] > list[mid]) //ordered normaly right
            {
                if (list[end] >= target && target > list[mid]) //between right set
                    return SearchInRotatedArrayByBook(list, mid + 1, end, target); //search right
                else
                    return SearchInRotatedArrayByBook(list, start, mid - 1, target); //search left
            }
            else if (list[start] == list[mid]) //left or right are all repeats
            {
                if (list[mid] != list[end]) //not duplicates left half 
                    return SearchInRotatedArrayByBook(list, mid + 1, end, target); //search right
                else
                {
                    //search both halves
                    int result = SearchInRotatedArrayByBook(list, start, mid - 1, target);
                    if (result == -1)
                        return SearchInRotatedArrayByBook(list, mid + 1, end, target);
                    else
                        return result;
                }
            }
            return -1;
        }

        /// <summary>
        /// Problem : 10.4
        /// Description : You are given an array-like data structure Listy which lacks a size
        /// method.It does, however, have an elementAt(i) method that returns the element at index i in
        /// 0( 1) time.If i is beyond the bounds of the data structure, it returns -1. (For this reason, the data
        /// structure only supports positive integers.) Given a Listy which contains sorted, positive integers,
        /// find the index at which an element x occurs. If x occurs multiple times, you may return any index
        /// </summary>
        /// NOT WORKING GOOD FIX IT
        public static void SortedSearchNoSize(List<int> list, int target)
        {
            int nextTargetIndex;
            if (list[target] == target) Console.WriteLine(target);
            else if (list[target] > target) // search left
                Console.WriteLine(SortedSearchNoSize(list, 0, target - 1, target));
            else if (list[target] < target && list[target] != -1)
            {
                do
                {
                    nextTargetIndex = 2 * target;
                    if (list[nextTargetIndex] >= target) //search right
                    {
                        Console.WriteLine(SortedSearchNoSize(list, target + 1, nextTargetIndex, target));
                        break;
                    }
                } while (list[nextTargetIndex] != -1);
                if (list[nextTargetIndex] == -1)
                {
                    do
                    {
                        nextTargetIndex--;
                    } while (list[nextTargetIndex] == -1);
                    Console.WriteLine(SortedSearchNoSize(list, target + 1, nextTargetIndex, target)); // search right
                }
            }
            else if (list[target] == -1)
            {
                nextTargetIndex = target;
                do
                {
                    nextTargetIndex--;
                } while (list[nextTargetIndex] == -1);
                Console.WriteLine(SortedSearchNoSize(list, 0, nextTargetIndex, target)); //search right
            }
        }

        private static int SortedSearchNoSize(List<int> list, int start, int end, int target)
        {
            if (start > end) return -1;
            int mid = (start + end) / 2;

            if (list[mid] == target) return mid;

            if (list[mid] > target) //search left
                return SortedSearchNoSize(list, start, mid - 1, target);
            else
                return SortedSearchNoSize(list, mid + 1, end, target);

        }

        public static int SortedSearchNoSizeByBook(List<int> list, int target)
        {
            int index = 1;
            while (list.ElementAt(index) != -1 && list.ElementAt(index) < target)
            {
                index *= 2;
            }
            return SortedSearchNoSizeByBook(list, index / 2, index, target); //low and high. some values between low and high until its very end are going to equal to -1
            //this is the out of bounds case where we should handle it for mid=-1 circumstances
        }

        //WE FIND THE LENGTH IN O(LogN) RUNTIME AND THEN FIND THE TARGET IN O(LogN) RUNTIME. OVERALL RUNTIME EQUALS O(LogN)
        private static int SortedSearchNoSizeByBook(List<int> list, int start, int end, int target)
        {
            int mid;
            while (start <= end)
            {
                mid = (start + end) / 2;
                int midValue = list.ElementAt(mid);
                if (midValue == -1 || midValue > target)
                    end = mid - 1;
                else if (midValue < target)
                    start = mid + 1;
                else
                    return mid;
            }

            return -1;
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
        public static int Decode(string input)
        {
            //3 -> c
            //12345 -> [a + (2345)] + [l+ (345)]
            if (input.Length == 0 || input[0] == '0')
                return 0;
            if (input.Length == 1)
                return 1;
            if (input.Length == 2 && Convert.ToInt16(input) <= 26)
                return 2;
            int ways = HelperDecodeSec(input, input.Length-1);
            return ways;
        }
        
        private static int HelperDecodeSec(string input, int index)
        {
            if (index <= 0)
                return 1;
            if (input[index].ToString() == "0")
                return 0;
            int result;

            int temp = Convert.ToInt32(input[index].ToString() + input[index - 1].ToString());
            result = HelperDecodeSec(input, index - 1);
            
            
            if (temp <= 26)
                result += HelperDecodeSec(input, index - 2);
            
            return result;
        }

        /// <summary>
        /// Problem : Google Interview Optimize with Memoization
        /// Description : Given a string ex "1234" which identifies a word (encoded a = "1" , b = "2" etc..)
        /// find all the possible ways to translate it, ex,
        /// "1" + "2" + "3" + "4" --> a b c d
        /// "12" + "3" + "4" --> l c d
        /// "1" + "23" + "4" --> a w d
        /// output : 3 ways
        /// </summary>
        /// 
        /* Memo optimization visualized (C == Cached and calculated before)
         *      4321
         *       ""
         *     1   12
              2 23   3(C)
            3  4(C)
          4
        */
        public static int DecodeMemoization(string input)
        {
            //3 -> c
            //12345 -> [a + (2345)] + [l+ (345)]
            if (input.Length == 0 || input[0] == '0')
                return 0;
            if (input.Length == 1)
                return 1;
            if (input.Length == 2 && Convert.ToInt16(input) <= 26)
                return 2;
            int ways = HelperDecodeMemoization(input, input.Length - 1, new Dictionary<string, int>());
            return ways;
        }

        private static int HelperDecodeMemoization(string input, int index, Dictionary<string,int> map)
        {
            if (index <= 0)
                return 1;
            if (input[index].ToString() == "0")
                return 0;
            int result;

            int temp = Convert.ToInt32(input[index].ToString() + input[index - 1].ToString());

            if (map.ContainsKey(input[index - 1].ToString()))
            {
                result = map[input[index - 1].ToString()];
            }
            else
            { 
                result = HelperDecodeMemoization(input, index - 1, map);
                map.Add(input[index - 1].ToString(), result);
            }

            if (temp <= 26)
            {
                if (index - 2 >= 0)
                {
                    if (map.ContainsKey(input[index - 2].ToString()))
                    {
                        result += map[input[index - 2].ToString()];
                    }
                    else
                    {
                        result += HelperDecodeMemoization(input, index - 2, map);
                        map.Add(input[index - 2].ToString(), result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Problem : 10.5
        /// Description : Given a sorted array of strings that is interspersed with empty strings, write a
        /// method to find the location of a given string.
        /// EXAMPLE
        /// Input: ball, {"at",
        /// ""}
        /// Output: 4
        /// </summary>
        public static int SparseSearch(List<string> list,string target)
        {
            if (list.Count == 0) return -1;
            int returned =SparseSearchHelper(list, 0, list.Count - 1, target);
            return returned;
        }

        public static int SparseSearchHelper(List<string> list, int start, int end, string target)
        {
            if (start > end) return -1;
            int mid = (start + end ) / 2;

            if (list[mid]=="")
            {
                int counter = mid+1;
                while(list[counter]=="" && counter<=list.Count-1)
                {
                    if (counter == list.Count - 1) break; 
                    counter++; 
                }
           

                if (list[counter] != "")
                {
                    if (list[counter].CompareTo(target) == 0)
                        return counter;
                    else if (list[counter].CompareTo(target) > 0)
                    {
                        counter = mid - 1;
                        while (list[counter] == "" && counter >= 0)
                        {
                            counter--;
                        }
                        if (list[counter].CompareTo(target) == 0)
                            return counter;
                        else
                            return SparseSearchHelper(list, start, counter - 1, target);
                    }
                    else
                        return SparseSearchHelper(list, counter + 1, end, target);
                }
                else
                {
                    counter = mid - 1;
                    while (list[counter] == "" && counter >= 0)
                    {
                        counter--;
                    }
                    if (list[counter].CompareTo(target) == 0)
                        return counter;
                    else
                        return SparseSearchHelper(list, start, counter-1, target);
                }
            }
            else
            {
                if (list[mid].CompareTo(target) == 0)
                    return mid;
                else if (list[mid].CompareTo(target) > 0)
                    return SparseSearchHelper(list, start, mid - 1, target);
                else
                    return SparseSearchHelper(list, mid+1, end, target);
            }
        }

        /// <summary>
        /// Problem : 10.9
        /// Description : Given an M x N matrix in which each row and each column is sorted in
        /// ascending order, write a method to find an element.
        /// </summary>
        /// Input : +target = 8
        /// 1 2 4 7
        /// 3 4 6 9
        /// 5 6 8 15
        /// Output : [2,2]
        /// TODO
        public static void SortedMatrixSearch(int[,] arr, int target)
        {
            int rows = arr.GetLength(0);
            int cols = arr.GetLength(1);
            
        }

        public static void SortedMatrixSearch(int[] list, int target, int start, int end)
        {

        }
    }
}

