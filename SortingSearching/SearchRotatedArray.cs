using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.SortingSearching
{
    /*
    Search in Rotated Array: Given a sorted array of n integers that has been rotated an unknown
    number of times, write code to find an element in the array. You may assume that the array was
    originally sorted in increasing order.
    EXAMPLE
    lnput:find5in{15, 16, 19, 20, 25, 1, 3, 4, 5, 7, 10, 14}
    Output: 8 (the index of 5 in the array)
    */
    //array1 : 50 5 20 30 40
    //array2 : 5 10 20 0 2
    //array3 : 5 10 30 20 5

        //O(logn) Runtime
    public class SearchRotatedArray
    {
        public int Sort(int[] array,int left, int right,int target)
        {

            int mid = (left + right) / 2;
            if (left > right) return -1;

            if (array[mid] == target) return mid;

            if (array[mid] > array[left]) //left part ordered
            {
                if (target >= array[left] && target < array[mid])
                {
                    return Sort(array, left, mid - 1, target); //search left
                }
                else
                {
                    return Sort(array, mid + 1, right, target);//search right
                }
            }
            else if (array[mid] < array[right]) //right part ordered
            {
                if (target <= array[right] && target > array[mid])
                {
                    return Sort(array, mid + 1, right, target); //search right
                }
                else
                {
                    return Sort(array, left, mid - 1, target); //search left
                }
            }
            else if (array[mid] == array[left]) //all rights or all lefts are repeats (cause of sorted)
            {
                if (array[mid] != array[right])
                {
                    return Sort(array, mid + 1, right, target); //search right
                }
                else
                {
                    int result = Sort(array, left, mid - 1, target); //search left
                    if (result == -1)
                    {
                        return Sort(array, mid + 1, right, target); //search right
                    }
                    else
                        return result;
                }
            }
            return -1;
        }
    }
}
