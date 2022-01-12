using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals
{
    public static class HardProblems
    {
        /// <summary>
        /// Problem : 17.2
        /// Description : Write a method to shuffle a deck of cards. It must be a perfect shuffle-in other words, each
        /// of the 52! permutations of the deck has to be equally likely.Assume that you are given a random
        /// number generator which is perfect.
        /// <Approach>
        /// We can follow the approach build Shuffle(n) by Shuffle(n-1). So I build my base case by recursing until
        /// I reach the start of the list with my cards. Then as my index is increasing I am swaping randomly with 
        /// my random generator my current incoming card (pointed by my index) with a random other card in the range
        /// [0,index]. If I want i can keep only the last produced shuffle of the whole deck. Otherwise I can add to a List<int[]>
        /// all my different shuffles during recursion.
        /// <Time>O(n) foreach card</Time>
        /// <Space>If I remove my allShuffles List<int[]> and just return the final record then my space is O(logn) for
        /// the memory stack used by recursion</Space>
        /// </Approach>
        /// []
        /// [1]
        /// [1,2] -> [3,1,2] -> [3,4,2,1] -> [3,4,5,1,2]
        /// </summary>
        public static int[] Shuffle()
        {
            int[] cards = new int[10] //52 cards
            {1,2,3,4,5,6,7,8,9,10};
            //  {1,2,3,4,..,52} where 1=1heart 13=1diamond etc...
            return ShuffleHelper(cards,9);
        }
        public static List<int[]> allShuffles = new List<int[]>();
        public static int[] ShuffleHelper(int[] cards, int index)
        {
            if (index == 0) return cards; //baseCase

            cards = ShuffleHelper(cards, index - 1);

            int indexToSwap = RandomGeneratorBetweenArray(0, index);

            int temp = cards[indexToSwap];
            cards[indexToSwap] = cards[index];
            cards[index] = temp;
            var tempCards = new int[index + 1];
            for (int i=0;i<=index;i++)
            {
                tempCards[i] = cards[i];
            }
            allShuffles.Add(tempCards);
            return cards;
        }
        public static int RandomGeneratorBetweenArray(int lowerIndex, int higherIndex)
        {
            Random number = new Random();
            return number.Next(lowerIndex, higherIndex - 1);
        }

        /// <summary>
        /// Problem : 17.3
        /// Description : Write a method to randomly generate a set of m integers from an array of size n. Each
        /// element must have equal probability of being chosen.
        /*
         * (1,3,5) array (1,2,3,5,6,7,8)
         * my initial array has length of 7. Each of my records has a 1/7 probability of being chosen
         */
        public static void RandomSet()
        {
            List<int> initialList = new List<int> { 1, 3, 5, 2, 6, 7, 8 };
            decimal probability = (decimal)(1.0m / initialList.Count) * 100;

            List<int> probabiListic = new List<int>();
            foreach (var item in initialList)
            {
                Random num = new Random();
                int currentProb = num.Next(0, 100);

                if (currentProb <= Convert.ToInt32(probability))
                    probabiListic.Add(item);
            }
            Console.WriteLine("Stop");
        }
    }
}
