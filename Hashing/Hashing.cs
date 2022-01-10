using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals
{
    public class Hashing
    {
        public long UniqueHash(string input)
        {
            /*
             * this -> t h i s
             * 104 + 116g + 120g^2 + 130g^3
             */
            int g = 31;
            int g_pow = 1;
            long hash = 0;
            int m = Convert.ToInt32(Math.Pow(10, 9)) + 9;
            if (input == null) return -1;

            input = input.Replace(" ", "").Trim();

            for (var i=0;i<input.Length;i++)
            {
                //unicode
                //hash = (hash + Convert.ToInt64(Math.Pow(g, i) * input[i]));
                hash = (hash + g_pow * input[i])% m;
                g_pow = (g_pow * g) % m;
            }
            return hash;
        }

        public string Base62(int deci)
        {
            string input = "10201401jdsadjajdaisjdOEasmdoafjaoksFOAPSFJAPRWJAPR";
            string hash_str = "";
            while (deci>0)
            {
                hash_str += input[deci % 62];
                deci /= 62;
            }
            return hash_str; 
            //md5 hash technique
            //62^7 char --> 3.5 trillion unique Ids
            //26 A-Z + 26 a-z + 10 (0,9) -> base 62. 7 char length URLShortener
        }

        public string Base62Shortener(int number)
        {
            int remainder = 0;
            List<int> hashDigits = new List<int>();
            while (number>0)
            {
                remainder = number % 62;
                number = number / 62;
                hashDigits.Add(remainder);
            }

            string hashString = "";
            int i = 0;
            Dictionary<int, string> base62Alphabet = new Dictionary<int, string>();
            //{ };  [a,b,c,...,A,B,C,...,0,1,2,...]

            while (hashDigits.Count>i)
            {
                hashString += base62Alphabet[hashDigits[i]];
                i++;
            }
            return hashString;
        }
    }
}
