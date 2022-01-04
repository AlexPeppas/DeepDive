using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DataStructures.LinkedList
{
    public class LinkedListNode 
    {
        public int data { get; set; }
        public LinkedListNode next { get; set; }

        public LinkedListNode(int data)
        {
            this.data = data;
            this.next = null;
        }
    }
    public class RemoveDups
    {
        public void SeedLinkedList(LinkedListNode instance, int data)
        {

            if (data >= 5) return;
                instance.next = new LinkedListNode(data);
            SeedLinkedList(instance.next,data++);
        }
        public void main() 
        {
            LinkedListNode head = new LinkedListNode(1);
            head.next = new LinkedListNode(7);
            head.next.next = new LinkedListNode(2);
            head.next.next.next = new LinkedListNode(3);
            head.next.next.next.next = new LinkedListNode(4);
            head.next.next.next.next.next = new LinkedListNode(5);
            head.next.next.next.next.next.next = new LinkedListNode(2);
            head.next.next.next.next.next.next.next = new LinkedListNode(6);
            head.next.next.next.next.next.next.next.next = new LinkedListNode(3);
            head.next.next.next.next.next.next.next.next.next = new LinkedListNode(8);
            //17234568

            LinkedListNode node = null;
            var set = new HashSet<int>();
            node = head;
            set.Add(node.data);
            do {
                if (node.next != null)
                {
                    if (set.Contains(node.next.data))
                    {
                        node.next = node.next.next;
                    }
                    else
                    {
                        set.Add(node.next.data);
                    }
                }
                node = node.next;
            } while (node != null);

            //var k = 3;
            //var result = RemoveKthElement.main(head, k);
            //Console.WriteLine($"This is {k}'d element from LinkedList's finish line : " + result.data);

        }
    }
}
