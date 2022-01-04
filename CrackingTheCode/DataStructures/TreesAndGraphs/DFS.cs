using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DataStructures.TreesAndGraphs
{
    public class Node
    {
        public int data { get; set; }
        public Node left { get; set; }
        public Node right { get; set; }
        public bool visited { get; set; }
        public Node(int data)
        {
            this.data = data;
            this.left = null;
            this.right = null;
            this.visited = false;
        }
    }
    public class DFS
    {
        public void Search(Node root)
        {
            if (root == null) return;
            Console.WriteLine(root.data);
            if (!root.left.visited)
                Search(root.left);
            if (!root.right.visited)
                Search(root.right);

        }
    }
}

/*
      1                     1                          1             
        / \                   / \                        / \            
       /   \                 /   \                      /   \           
      2     3    [s]        2     3                    2     3          
     /      /                \     \                    \     \         
    /      /                  \     \                    \     \        
   4      5          ->        4     5          ->        4     5       
  /      / \                  /     / \                  /     / \      
 /      /   \                /     /   \                /     /   \     
6      7     8   [s]        6     7     8   [s]        6     7     8
 \          / \            /           / \              \         / \   
  \        /   \          /           /   \              \       /   \  
   9      10   11        9           11   10              9 
     */
