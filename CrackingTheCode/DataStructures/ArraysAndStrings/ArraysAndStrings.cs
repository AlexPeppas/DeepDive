using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DeepDiveTechnicals.DataStructures.ArraysAndStrings
{
    public static class ArraysAndStrings
    {
        private static Dictionary<char, int> alphabetDict = new Dictionary<char, int>()
        {
            {'a',0 },{'b',0},{'c',0 },{'d',0},{'e',0 },{'f',0},{'g',0 },{'h',0},{'i',0 },{'j',0},{'k',0 },{'l',0},{'m',0 },{'n',0}
            ,{'o',0 },{'p',0},{'q',0 },{'r',0},{'s',0 },{'t',0},{'u',0 },{'v',0},{'w',0 },{'x',0},{'y',0 },{'z',0}
        };

        /// <summary>
        /// Problem : 1.1
        /// Description : Implement an algorithm to determine if a string has all unique characters. What if you
        /// cannot use additional data structures?
        /// </summary>
        public static bool IsUnique(string input)
        {
            if (input.Length > 26) return false;
            foreach (var c in input)
            {
                if (alphabetDict[c] == 0)
                    alphabetDict[c]++;
                else return false;
            }
            return true;
        }

        /// <summary>
        /// Problem : 1.2
        /// Description : Given two strings, write a method to decide if one is a permutation of the other.
        /// Approach : Sorting char[] and compare
        /// </summary>
        public static bool CheckPermutation(string st1, string st2)
        {
            //dbcda
            //acbdd

            if (st1.Length != st2.Length) return false;
            var stc1 = st1.ToCharArray();
            Array.Sort(stc1);

            var stc2 = st2.ToCharArray();
            Array.Sort(stc2);
            return new String(stc1).Equals(new String(stc2));
        }

        /// <summary>
        /// Problem : 1.2
        /// Description : Given two strings, write a method to decide if one is a permutation of the other.
        /// Approach : Using Histogram
        /// </summary>
        public static bool CheckPermutationHistogram(string st1, string st2)
        {
            if (st1.Length != st2.Length) return false;

            var letters = new int[128];

            foreach (var letter in st1)
            {
                letters[letter]++;
            }

            foreach (var letter in st2)
            {
                letters[letter]--;
                if (letters[letter] < 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Problem : 1.3
        /// Description : Write a method to replace all spaces in a string with '%20'. You may assume that the string
        ///has sufficient space at the end to hold the additional characters, and that you are given the "true"
        ///length of the string. (Note: If implementing in Java, please use a character array so that you can
        ///perform this operation in place.)
        ///EXAMPLE
        ///Input: "Mr John Smith ", 13
        ///Output: "Mr%20John%20Smith"
        /// </summary>
        public static string URLify(string input)
        {
            StringBuilder output = new StringBuilder();
            input.Trim();

            foreach (var ch in input)
            {
                if (ch != ' ')
                    output.Append(ch);
                else
                {
                    output.Append('%');
                    output.Append('2');
                    output.Append('0');
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Problem : 1.4
        /// Description : Given a string, write a function to check if it is a permutation of a palindrome.
        /// A palindrome is a word or phrase that is the same forwards and backwards.A permutation
        ///is a rearrangement of letters.The palindrome does not need to be limited to just dictionary words.
        ///EXAMPLE
        ///Input: Tact Coa
        ///Output: True (permutations: "taco cat", "atco eta", etc.)
        /// </summary>
        public static bool PalindromPermutation(string input)
        {
            if (!CheckIfPalindromEfficient(input)) return false;
            //if it's a palindrom of a permutation i cannot check with a stack. I have to validate based on 
            //the strategy that a palindrom should be consisted of n even chars and only a single odd char. (even || odd frequency)

            return true;
        }
        private static bool CheckIfPalindromEfficient(string inputToCheck)
        {
            bool singleOddChar = true;

            foreach (var c in inputToCheck)
            {
                alphabetDict[c]++;
            }
            //if not dict use BuildCharFrequencyTable and approach with int[]
            foreach (var item in alphabetDict)
            {
                if (item.Value % 2 != 0)
                    if (singleOddChar)
                        singleOddChar = false;
                    else return false;
            }
            return true;
        }
        //optional alternative solution
        private static int[] BuildCharFrequencyTable(string inputToCheck)
        {
            int[] table = new int[Convert.ToInt32(Char.GetNumericValue('z') - Char.GetNumericValue('a') + 1)];
            foreach (char c in inputToCheck)
            {
                int x = GetCharNumber(c);
                if (x != -1)
                    table[x]++;
            }
            return table;
        }
        private static int GetCharNumber(char c)
        {
            int a = Convert.ToInt32(Char.GetNumericValue('a'));
            int z = Convert.ToInt32(Char.GetNumericValue('z'));
            int val = Convert.ToInt32(Char.GetNumericValue(c));
            if (a <= val && val <= z)
                return (val - a);
            return -1;
        }

        /// <summary>
        /// Problem : 1.5
        /// Description : There are three types of edits that can be performed on strings: insert a character,
        ///remove a character, or replace a character.Given two strings, write a function to check if they are one edit (or zero edits) away.
        ///EXAMPLE
        ///pale, ple -> true
        ///pales, pale -> true
        ///pale, bale -> true
        ///pale, bake -> false
        /// </summary>
        public static bool OneAway(string master, string slave)
        {
            int accum = 0;
            if ((master.Length == slave.Length) || (master.Length == slave.Length + 1) || (master.Length == slave.Length - 1))
            {
                int i = 0;
                int j = 0;
                do
                {

                    if (slave[i] == master[j])
                    {
                        i++;
                        j++;
                        continue;
                    }
                    else
                    {
                        if (accum > 0) return false;
                        if (slave[i] == master[j + 1])
                        {
                            accum++;
                            j += 2;
                        }
                        else
                        {
                            j++;
                            accum++;
                        }
                        i++;

                    }

                } while (i < slave.Length);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Problem : 1.6
        /// Description : Implement a method to perform basic string compression using the counts
        ///of repeated characters.For example, the string aabcccccaaa would become a2blc5a3.If the
        ///"compressed" string would not become smaller than the original string, your method should return
        ///the original string. You can assume the string has only uppercase and lowercase letters(a - z).
        /// </summary>
        public static string StringCompression(string phrase)
        {
            StringBuilder output = new StringBuilder();
            int sum = 1;
            for (var i=0;i<phrase.Length-1;i++)
            {
                if (phrase[i] != phrase[i + 1])
                {
                    output.Append(phrase[i]);
                    output.Append(sum);
                    sum = 1;
                }
                else
                    sum++;
            }
            if (phrase[phrase.Length-2]!=phrase[phrase.Length-1])
            {
                output.Append(phrase[phrase.Length-1]);
                output.Append(1);
            }
            else
            {
                output.Append(phrase[phrase.Length - 1]);
                output.Append(sum);
            }
            return output.ToString();
        }

        /// <summary>
        /// Problem : 1.9
        /// Description : Assume you have a method isSubstring which checks if one word is a substring
        /// of another.Given two strings, 51 and 52, write code to check if 52 is a rotation of 51 using only one
        /// call to i5Sub5tring(e.g., "waterbottle" is a rotation of" erbottlewat").
        /// </summary>
        public static bool StringRotation (string s1, string s2)
        {
            if (s1.Length==s2.Length && s1.Length>0)
            {
                string s1s1 = s1 + s1;
                return isSubstring(s1s1, s2);
            }
            return false;
        }
        public static bool isSubstring(string s1, string s2)
        {
            return true; //mock
        }
    }
}
