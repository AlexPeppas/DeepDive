using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DynamicProgramming
{
    /*
    Parens: Implement an algorithm to print all valid (e.g., properly opened and closed) combinations
    of n pairs of parentheses.
    EXAMPLE
    Input: 3
    Output: ( ( () ) ) , ( () () ) , ( () ) () , () ( () ) , () () ()
    */
    
    public class Parenthesis
    {
        public bool isValid()
        {
            char[] arr = new char[10];
            arr[0] = '('; arr[1] = '('; arr[2] = ')'; arr[3] = ')'; arr[4] = ')'; arr[5] = ')'; arr[6] = '('; arr[7] = ')';
            arr[8] = '('; arr[9] = ')';

            Stack<string> stack = new Stack<string>();
            foreach (var chrct in arr)
            {
                if (chrct == '(')
                    stack.Push(chrct.ToString());
                else
                {
                    if (stack.Count == 0) 
                        return false;
                    stack.Pop();
                } 
            }
            if (stack.Count == 0)
                return true;
            else return false;
        }
    }
}
