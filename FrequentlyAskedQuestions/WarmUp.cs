using System;
using System.Collections.Generic;
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

        public static void QuickSortHelper(int low,int high, int[] arr)
        {
            if (low >= high)
                return;

            int pivot = arr[(low + high)/ 2];
            var index = Partition(arr,pivot,low,high); // build items around partition (middle element)

            QuickSortHelper(low, index - 1, arr); //continue for the left and right sub-arrays
            QuickSortHelper(index, high, arr);
        }

        public static int Partition(int[] arr, int partitionKey,int leftPntr, int rightPntr)
        {
            while (leftPntr<=rightPntr)
            {
                while (arr[leftPntr] < partitionKey)
                { 
                    leftPntr++; 
                }
                
                while (arr[rightPntr]> partitionKey)
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
    }
}
