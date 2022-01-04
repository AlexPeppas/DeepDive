using DeepDiveTechnicals.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace DeepDiveTechnicals.DataStructures.TreesAndGraphs
{
    /*
     * Given a sorted (increasing order) array with unique integer elements, write an algorithm
       to create a binary search tree with minimal height
     */
    public class MinimalTree
    {
        public void TreePreOrder(Node node) //rootleftright
        {
            if (node == null) return;

            Console.WriteLine(node.data);
            TreePreOrder(node.left);
            TreePreOrder(node.right);
        }
        public Node CreateBinaryTree(List<int> list)
        {
            var tree = CreateBinaryTree(list, 0, list.Count - 1);
            return tree;
        }

        public Node CreateBinaryTree(List<int> list,int start, int end)
        {
            if (end < start) return null;

            int mid = (start + end) / 2;
            Node node = new Node(list[mid]);
            node.left = CreateBinaryTree(list, 0, mid - 1);
            node.right = CreateBinaryTree(list, mid + 1, end);

            return node;
        }

        //Given a binary tree, design an algorithm which creates a linked list of all the nodes
        //at each depth(e.g., if you have a tree with depth D, you'll have D linked lists).
        public void ListOfDepths(Node root, List<LinkedList<Node>> lists, int level)
        {

        }
        /*
            Check Balanced: Implement a function to check if a binary tree is balanced. For the purposes of
            this question, a balanced tree is defined to be a tree such that the heights of the two subtrees of any
            node never differ by more than one.
         */
        public void CheckBalanced(Node root)
        {

        }
    }
}
/*
 *        1
 *     2     3
     4  5  6  7
    8 9     10 11
*/
