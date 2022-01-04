using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.Services
{
    public class StringsPermutations
    {
        public void NumberOfPerms()
        {
            string s = "abbc";
            string b = "cbabadcbbabbcbabaabccbabc";
          
            var dict = new Dictionary<char, int>();
            for (int i =0;i<=b.Length-4;i++)
            {
                var list = new List<char>();
                list.Add(b[i]);
                list.Add(b[i+1]);
                list.Add(b[i+2]);
                list.Add(b[i+3]);

            }
        }

    }
}
