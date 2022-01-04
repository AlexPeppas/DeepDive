using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.Services
{
    public class TreeLogic
    {
        public class Tree
        {
            public int data { get; set; }
            public Tree left { get; set; }
            public Tree right { get; set; }

            public Tree(int data)
            {
                this.data = data;
                this.left = null;
                this.right = null;
            }
        }

        public Tree TreeSeed()
        {
            var root = new Tree(1);
            root.left = new Tree(2);
            root.right = new Tree(3);
            root.left.left = new Tree(4);
            root.left.right = new Tree(5);
            root.right.left = null;
            root.right.right = new Tree(6);
            return root;
        }

        public void TreePreOrder(Tree node, int layer) //rootleftright
        {
            if (node == null) return;

            Console.WriteLine(node.data);
            TreePreOrder(node.left, layer++);
            TreePreOrder(node.right, layer++);
        }
        public void TreeInOrder(Tree node) //leftrootright
        {
            if (node == null) return;
            TreeInOrder(node.left);
            Console.WriteLine(node.data);
            TreeInOrder(node.right);
        }
        public void TreePostOrder(Tree node) //leftrightroot
        {
            if (node == null) return;
            TreePostOrder(node.left);
            TreePostOrder(node.right);
            Console.WriteLine(node.data);
        }
        public void TreeTraverse()
        {
            var root = TreeSeed();
            int layer = 1;
            Console.WriteLine("PREORDER");
            TreePreOrder(root, layer);
            Console.WriteLine("INORDER");
            TreeInOrder(root);
            Console.WriteLine("POSTORDER");
            TreePostOrder(root);
            Console.WriteLine("LEFTSIDE");
            TreeLeftSide(root);
        }
        public void TreeLeftSide(Tree node)
        {
            if (node == null) return;


            Console.WriteLine(node.data);
            
            

        }
    }
}
/*
                      1             //layer 1
                 2         3        //layer 2
              4     5         6     //layer 3
     
     //124536 - PREORDER
     //425136 - INORDER
     //452631 - POSTORDER

                       1
                 2         3
                              6
                            7   8          //output 1267



    */