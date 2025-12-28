using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DataStructures.StacksAndQueues
{
    //CustomStack and CustomQueue
    public class CustomStack<T>
    {
        public class StackNode<T>
        {
            public T Data { get; set; }
            public StackNode<T> Previous { get; set; }

            public StackNode(T data)
            {
                Data = data;
            }
        }

        public StackNode<T> Top;

        public T Pop()
        {
            if (Top.Data == null) throw new Exception("Stack is Empty");
            T item = Top.Data;
            Top = Top.Previous;
            return item;
        }

        public void Push(T item)
        {
            StackNode<T> toPush = new StackNode<T>(item);
            toPush.Previous = Top;
            Top = toPush;
        }

        public T Peek()
        {
            if (Top.Data == null) throw new Exception("Stack is Empty");
            return Top.Data;
        }

        public bool IsEmpty()
        {
            return (Top == null);
        }
    }

    public class CustomQueue<T>
    {
        public class QueueNode<T>
        {
            public T Data;
            public QueueNode<T> Previous;

            public QueueNode(T Data)
            {
                this.Data = Data;
            }
        }

        private QueueNode<T> head;
        private QueueNode<T> tail;

        public void Add(T item)
        {
            
            var toAdd = new QueueNode<T>(item);
            if (tail != null)
            {
                tail.Previous = toAdd;
            }
            tail = toAdd;
            if (head == null)
                head = tail;   
            
        }

        public T Remove()
        {
            if (head == null) throw new Exception("Queue Is Empty");
            T data = head.Data;
            head = head.Previous; //new head is the previous one
            if (head == null)
                tail = null;
            return data;
        }

        public T Peek()
        {
            if (head.Data == null) throw new Exception("Queue Is Empty");
            return head.Data;
        }

        public bool IsEmpty()
        {
            return (head == null);
        }
    }

    public class StackWithMin<T> : CustomStack<T>
    {
        
    }

    /// <summary>
    /// Problem : 3.3
    /// Description : Imagine a (literal) stack of plates. If the stack gets too high, it might topple.
    ///Therefore, in real life, we would likely start a new stack when the previous stack exceeds some
    ///threshold.Implement a data structure SetOfStacks that mimics this. SetOfStacks should be
    ///composed of several stacks and should create a new stack once the previous one exceeds capacity.
    ///SetOfStacks.push() and SetOfStacks.pop() should behave identically to a single stack
    ///(that is, pop () should return the same values as it would if there were just a single stack).
    ///FOLLOW UP
    ///Implement a function popAt(int index) which performs a pop operation on a specific substack.
    /// </summary>
    public class StackOfPlates<T>
    {
        public List<CustomStack<T>> stacksList;
        CustomStack<T> currentStack;
        private int threshold;
        private int counter;

        public StackOfPlates(int threshold)
        {
            this.threshold = threshold;
            this.counter = 0;
            this.currentStack = new CustomStack<T>();
            stacksList = new List<CustomStack<T>>();
        }

        public void Push(T item)
        {
            counter++;
            if (counter<threshold)
            {
                currentStack.Push(item);
            }
            else
            {
                counter = 0;
                currentStack = new CustomStack<T>();
                currentStack.Push(item);
                stacksList.Add(currentStack);
            }
        }

        public T Pop()
        {
            if (currentStack.Top==null)
            {
                if (stacksList.Count >= 1)
                {
                    stacksList.Remove(currentStack); //remove the empty current stack
                    currentStack = stacksList[stacksList.Count - 1];
                }
                else
                    throw new Exception("List of Stacks is empty");
            }
            T item = currentStack.Pop();
            return item;
        }

        public T Peek()
        {
            if (stacksList.Count < 1) throw new Exception("List of Stacks is empty");
            return stacksList[stacksList.Count - 1].Top.Data;
        }

        public bool IsEmpty()
        {
            return (stacksList[stacksList.Count - 1].Top.Data == null);
        }

        public T PopAt(int index)
        {
            return stacksList[index].Pop();
        }
    }

    /// <summary>
    /// Problem : 3.4
    /// Description : Implement a MyQueue class which implements a queue using two stacks
    /// </summary>
    public class QueuesViaStacks<T>
    {
        public CustomStack<T> newStack, oldStack;

        public QueuesViaStacks()
        {
            newStack = new CustomStack<T>();
            oldStack = new CustomStack<T>();
        }

        public void Add(T item)
        {
            newStack.Push(item);
        }

        public T Remove()
        {
            ShiftStacks();
            return oldStack.Pop();
        }

        public T Peek()
        {
            ShiftStacks();
            return oldStack.Peek();
        }

        public void ShiftStacks()
        {
            if (oldStack.IsEmpty())
            {
                while (!newStack.IsEmpty())
                {
                    oldStack.Push(newStack.Pop());
                }
            }
        }
    }
}
