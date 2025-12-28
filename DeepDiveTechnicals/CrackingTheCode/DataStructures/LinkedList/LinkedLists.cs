using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DataStructures.LinkedList
{
    public class LinkedListStruct 
    {

        public int Data { get; set; }
        public LinkedListStruct Next = null;

        public LinkedListStruct(int data = 0)
        {
            this.Data = data;
        }

    }
    public static class LinkedLists
    {
        private static LinkedListStruct InitializeLinkedListStruct ()
        {
            Random rand = new Random();

            LinkedListStruct head = new LinkedListStruct(90);
            var node = head;
            for (int i=0;i<10;i++)
            {
                node.Next = new LinkedListStruct(rand.Next(1, 100));
                node = node.Next;
                
            }
            node.Next = new LinkedListStruct(90);
            return head;
        }

        /// <summary>
        /// Problem : 2.1
        /// Description : Write code to remove duplicates from an unsorted linked list.
        ///FOLLOW UP
        ///How would you solve this problem if a temporary buffer is not allowed?
        /// </summary>
        public static void RemoveDups()
        {
            LinkedListStruct head = InitializeLinkedListStruct();
            var set = new HashSet<int>();

            var node = head;
            set.Add(node.Data);
            do {

                if (!set.Contains(node.Next.Data))
                {
                    set.Add(node.Next.Data);
                }
                else
                {
                    node.Next = node.Next.Next;
                }
                node = node.Next;
            } while (node.Next.Next != null);

            do 
            {
                Console.WriteLine(head.Data);
                head = head.Next;
            } while (head.Next != null);
        }

        /// <summary>
        /// Problem : 2.2
        /// Description : Implement an algorithm to find the kth to last element of a singly linked list.
        /// </summary>
        public static int ReturnKthToLast(int k)
        {
            LinkedListStruct head = InitializeLinkedListStruct();
            LinkedListStruct p1 = head;
            LinkedListStruct p2 = head;
            for (int i=0;i<k;i++)
            {
                if (p1 == null) throw new Exception("k>length of linkedList, structure exceeded");
                p1 = p1.Next;
            }
            do {
                p2 = p2.Next;
                p1 = p1.Next;
            } while (p1.Next != null);
            return p2.Data;
        }

        /// <summary>
        /// Problem : 2.3
        /// Description : Implement an algorithm to delete a node in the middle (i.e., any node but
        ///the first and last node, not necessarily the exact middle) of a singly linked list, given only access to
        ///that node.
        ///EXAMPLE
        ///lnput:the node c from the linked list a->b->c->d->e->f
        ///Result: nothing is returned, but the new linked list looks like a->b->d->e- >f
        /// </summary>
        public static bool DeleteMiddleNode(LinkedListStruct node)
        {
            /*
             *  In this problem, you are not given access to the head of the linked list. You only have access to that node.
                The solution is simply to copy the data from the next node over to the current node, and then to delete the
                next node.
             */
            if (node == null || node.Next == null) return false;

            node.Data = node.Next.Data;
            node.Next = node.Next.Next;

            return true;

        }

        /// <summary>
        /// Problem : 2.4
        /// Description : Write code to partition a linked list around a value x, such that all nodes less than x come
        /// before all nodes greater than or equal to x.If x is contained within the list the values of x only need
        /// to be after the elements less than x(see below). The partition element x can appear anywhere in the
        /// "right partition"; it does not need to appear between the left and right partitions.
        /// EXAMPLE
        /// Input: 3 -> 5 -> 8 -> 5 -> 10 -> 2 -> 1 [partition= 5]
        /// Output: 3 -> 1 -> 2 -> 10 -> 5 -> 5 -> 8
        /// </summary>
        public static LinkedListStruct Partition(LinkedListStruct head , int partition) 
        {
            head = InitializeLinkedListStruct();
            //list1 items < partition, list2 items>=partition
            if (head == null || head.Next == null) return null;

            var node = head;
            List<int> lessPositions = new List<int>();
            List<int> greaterPositions = new List<int>();
            int position = 0;
            int partitionFreq = 0;
            while (node != null)
            {
                if (node.Data < partition)
                    lessPositions.Add(node.Data);
                else if (node.Data > partition)
                    greaterPositions.Add(node.Data);
                else
                    partitionFreq++;
                position++;
                node = node.Next;
            }
            var newHead = new LinkedListStruct(lessPositions[0]);
            var newNode = newHead;
            for (int i=1;i<lessPositions.Count;i++)
            {
                newNode.Next = new LinkedListStruct(lessPositions[i]);
                newNode = newNode.Next;
            }
            for (int i=0;i<partitionFreq;i++)
            {
                newNode.Next = new LinkedListStruct(partition);
                newNode = newNode.Next;
            }
            for (int i = 0; i < greaterPositions.Count; i++)
            {
                newNode.Next = new LinkedListStruct(greaterPositions[i]);
                newNode = newNode.Next;
            }
            return newHead;
        }
        /*
         * DIFFERENT APPROACH WITH SPACE OPTIMIZATION (O(1)) INSTEAD OF (O(N)) WITHOUT DATA STRUCTURES JUST HEAD AND TAIL POINTERS FOR NEW LINKEDLISTS.
        In this approach, we start a"new" list (using the existing nodes). Elements bigger than the pivot element are
        put at the tail and elements smaller are put at the head. Each time we insert an element, we update either
        the head or tail.
        1 LinkedlistNode partition(LinkedlistNode node, int x) {
        2 LinkedListNode head node;
        3 LinkedListNode tail= node;
        4
        5 while (node != null) {
        6 LinkedListNode next = node.next;
        7 if (node.data < x) {
        8 // Insert node at head. 
        9 node.next= head;
        10 head= node;
        11 } else {
        12 // Insert node at tail. 
        13 tail.next= node;
        14 tail= node;
        15 }
        16 node= next;
        17 }
        18 tail.next= null;
        19
        20 // The head has changed, so we need to return it to the user.
        21 return head;
        22 }
         */
        public class LinkedListNodeNew
        {
            public int data;
            public LinkedListNodeNew next;
            public LinkedListNodeNew previous;
            public LinkedListNodeNew(int data)
            {
                this.data = data;
                this.next = null;
                this.previous = null;
            }
        }

        public static LinkedListNodeNew head = null;
        public static LinkedListNodeNew tail = null;

        public static void Partition (LinkedListNodeNew root, int partition)
        {
            if (head==null && tail == null)
            {
                head = root;
                tail = root;
            }
            var tempNode = root.next;
            while (tempNode != null)
            {
                if (tempNode.data<partition)
                {
                    tail.next = tempNode;
                    tail = tempNode;
                }
                else if (tempNode.data>partition)
                {
                    head.previous = tempNode;
                    head = tempNode;
                }
                else //==partition
                {
                    tail.next = tempNode;
                    tempNode.next = head;
                }
                tempNode = tempNode.next;
            }


        }
        /// <summary>
        /// Problem : 2.5
        /// Description : You have two numbers represented by a linked list, where each node contains a single
        ///digit.The digits are stored in reverse order, such that the 1 's digit is at the head of the list. Write a
        ///function that adds the two numbers and returns the sum as a linked list.
        ///EXAMPLE
        ///Input: (7-> 1 -> 6) + (5 -> 9 -> 2).That is,617 + 295.
        ///Output: 2 -> 1 -> 9. That is, 912.
        ///FOLLOW UP
        ///Suppose the digits are stored in forward order.Repeat the above problem.
        ///Input: (6 -> 1 -> 7) + (2 -> 9 -> 5).That is,617 + 295.
        ///Output: 9 -> 1 -> 2. That is, 912.
        /// </summary>
        public static int SumLists(LinkedListStruct headDigit1, LinkedListStruct headDigit2)
        {
            List<string> number1 = new List<string>();
            List<string> number2 = new List<string>();

            while (headDigit1.Next!=null)
            {
                number1.Add(headDigit1.Data.ToString());
                headDigit1 = headDigit1.Next;
            }
            while (headDigit2.Next != null)
            {
                number2.Add(headDigit2.Data.ToString());
                headDigit2 = headDigit2.Next;
            }
            string numSt1 = string.Empty;
            for (var i=number1.Count-1;i>=0;i--)
            {
                numSt1 += number1[i];
            }

            string numSt2 = string.Empty;
            for (var i = number2.Count - 1; i >= 0; i--)
            {
                numSt2 += number2[i];
            }

            return (Convert.ToInt32(numSt1) + Convert.ToInt32(numSt2));
        }
        //NOT EFFICIENT
        //DIFFERENT APPROACH
        public static LinkedListStruct SumListsRecursive(LinkedListStruct l1, LinkedListStruct l2,int carry)
        {
            if (l1 == null && l2 == null && carry ==0)
            {
                return null;
            }

            LinkedListStruct result = new LinkedListStruct(-1);
            int value = carry;
            if (l1 != null) value += l1.Data;
            if (l2 != null) value += l2.Data;

            result.Data = value % 10; //second digit of number

            //recurse
            if (l1 != null || l2 != null)
            {
                LinkedListStruct more = SumListsRecursive(l1 == null ? null : l1.Next,
                                                          l2 == null ? null : l2.Next,
                                                          value >= 10 ? 1 : 0);
                //result.setNext(more);
            }
            return result;
        }

        /// <summary>
        /// Problem : 2.6
        /// Description : Implement a function to check if a linked list is a palindrome
        /// </summary>
        /// Handle odd and even situations
        // 0 -> 1 -> 2 -> 3 -> 2 -> 1 -> 0 
        // 0 -> 1 -> 2 -> 3 -> 3 -> 2 -> 1 -> 0
        public static bool Palindrome(LinkedListStruct head)
        {
            LinkedListStruct p1 = head;
            LinkedListStruct p2 = head;

            var stack = new Stack<int>();
            stack.Push(p1.Data);
            while(p2.Next.Next!=null)
            {
                p1 = p1.Next;
                stack.Push(p1.Data);
                p2 = p2.Next.Next;
            }
            if (p2.Next != null)
            {
                p2 = p2.Next;
                if (p1.Data != p1.Next.Data) return false; //no palindrom
                stack.Pop();
                p1 = p1.Next;
            }
            else
                stack.Pop();
            while (p1!=null)
            {
                if (p1.Next.Data != stack.Pop()) return false;
                p1 = p1.Next;
            }
            return true;
        }

        /// <summary>
        /// Problem : 2.7
        /// Description : Given two (singly) linked lists, determine if the two lists intersect. Return the
        ///intersecting node.Note that the intersection is defined based on reference, not value. That is, if the
        ///kth node of the first linked list is the exact same node (by reference) as the jth node of the second
        ///linked list, then they are intersecting.
        ///</summary>
        public static Tuple<bool, int> Intersection(LinkedListStruct h1, LinkedListStruct h2)
        {
            int len1 = 1;
            int len2 = 1;
            if (h1 == null || h2 == null) return new Tuple<bool, int>(false, -1);

            LinkedListStruct tail1 = h1;
            LinkedListStruct tail2 = h2;

            while (tail1 != null)
            {
                tail1 = tail1.Next;
                len1++;
            }
            while (tail2 != null)
            {
                tail2 = tail2.Next;
                len2++;
            }

            if (tail1 != tail2) return new Tuple<bool, int>(false, -1);
            if (len1 > len2)
            {
                for (int i = 0; i < len1 - len2; i++)
                {
                    h1 = h1.Next;
                }
            }
            else if (len2 > len1)
            {
                for (int i = 0; i < len2 - len1; i++)
                {
                    h2 = h2.Next;
                }
            }
            int position = 0;
            while (h1 != h2)
            {
                h1 = h1.Next;
                h2 = h2.Next;
                position++;
            }
            return new Tuple<bool, int>(true, position);
        }
        //Time O(A+B), Space O(1)
        /*
         *  1. Run through each linked list to get the lengths and the tails.
            2. Compare the tails. If they are different (by reference, not by value), return immediately. There is no intersection.
            3. Set two pointers to the start of each linked list.
            4. On the longer linked list, advance its pointer by the difference in lengths.
            5. Now, traverse on each linked list until the pointers are the same.
         */

        /// <summary>
        /// Problem : 2.8
        ///Given a circular linked list, implement an algorithm that returns the node at the beginning of the loop.
        ///DEFINITION
        ///Circular linked list: A(corrupt) linked list in which a node's next pointer points to an earlier node, so
        ///as to make a loop in the linked list.
        ///EXAMPLE
        ///Input: A -> B -> C -> D-> E -> C [Same C as earlier by reference]
        ///Output: C
        ///</summary>
        public static LinkedListStruct LoopDetection(LinkedListStruct h1)
        {
            return null;
        }
    }
}
