using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DynamicProgramming
{
    /*
    Magic Index: A magic index in an array A [ 0 ••• n -1] is defined to be an index such that A[ i] =
    i. Given a sorted array of distinct integers, write a method to find a magic index, if one exists, in
    array A.
    FOLLOW UP
    What if the values are not distinct?
    */
    public class MagicIndex
    {
        public int ComputeSlow(int[] arr)
        {
            
            for (var i=0;i<arr.Length;i++)
            {
                if (arr[i] == i) return i;
            }
            return -1;
        }
        //0  1  2  3  4  5  6  7  8  9   |i's
        //1  2  5  6  7  9  10 11 12 21  |values
        public int ComputeWithFast()
        {
            int[] arr = new int[10];
            arr[0] = -7; arr[1] = -6; arr[2] = -5; arr[3] = 3; arr[4] = 5; arr[5] = 6; arr[6] =7; arr[7] = 8;
            arr[8] = 9; arr[9] = 10;
            var magicIndex = ComputeFast(arr, 0, arr.Length-1);
            Console.WriteLine("Magic index : "+ magicIndex);
            return magicIndex;
        }
        private int ComputeFast(int[] arr,int start,int end)
        {
            int mid = (end + start) / 2;
            if (end < start) throw new Exception("No Magic Number in Array");
            if (arr[mid] == mid) return mid;
            else if (arr[mid] < mid)
            {
                return ComputeFast(arr, mid + 1, end);
            }
            else
                return ComputeFast(arr, start, mid - 1);
        }

    }
}
