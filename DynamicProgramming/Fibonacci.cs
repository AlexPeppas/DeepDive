using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.DynamicProgramming
{
    public static class Fibonacci
    {
        public static int Compute(int n)
        {
            //1 1 2 3 5 8 13
            int f0 = 0;
            int f1 = 1;
            int step = 2;
            int temp;
            while (step<=n)
            {
                temp = f1;
                f1 = f1 + f0;
                f0 = temp;
                step++;
            }
            return f1;
        }
        public static int ComputeRecursively(int f0, int f1, int n, int step)
        {
            int temp;
            temp = f1;
            f1 = f1 + f0;
            f0 = temp;
            step++;
            if (step <= n) 
                f1 = ComputeRecursively(f0, f1, n, step);
            return f1;
        }
        public static int CacheFiboOptimization (int n)
        {
            return CacheFiboOptimization(n, new int[n + 1]);
        }
        public static int CacheFiboOptimization(int i, int[] cache)
        {
            if (i == 0 || i == 1) return i;

            if (cache[i] == 0)
            {
                cache[i] = CacheFiboOptimization(i - 1, cache) + CacheFiboOptimization(i - 2, cache);
            }
            return cache[i];
        }
        public static int[] CacheReturnStack(int n)
        {
            return CacheReturnStack(n, new int[n + 1]).Item2;
        }
        public static Tuple<int,int[]> CacheReturnStack(int i, int[] cache)
        {
            if (i == 0 || i == 1) return new Tuple<int, int[]>(i,cache);

            if (cache[i] == 0)
            {
                cache[i] = CacheReturnStack(i - 1, cache).Item1 + CacheReturnStack(i - 2, cache).Item1;
            }
            return new Tuple<int, int[]>(cache[i], cache);
        }
    }
}
