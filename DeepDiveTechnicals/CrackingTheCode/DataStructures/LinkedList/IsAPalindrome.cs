using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DataStructures.LinkedList
{
    public class IsAPalindrome
    {
        public bool Validation(LinkedListNode head)
        {
            //sample : 1122334 4332211
            Stack<int> stack = new Stack<int>();
            var fast = head; //fast 2xslow
            var slow = head; //slow
           
            //when fast ends then the slow pointer will be exactly at the mid of the linked list
            while (fast !=null && fast.next != null)
            {
                stack.Push(slow.data);
                slow = slow.next;
                fast = fast.next.next;
            }
            //if the linkedlist has odd length then just skip the middle
            if (fast != null)
            {
                slow = slow.next;
            }
            //if current node is not equal to the peek of the stack (keep poping) then 
            //its not a palindrome
            while (slow != null)
            {
                if (slow.data != stack.Pop()) return false;
                slow = slow.next;
            }
            
            return true;
        }
    }
}
