using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public static class WarmUp
    {
        public sealed class IslandsCount
        {
            /// 0 1 1 1
            /// 0 0 0 0
            /// 1 1 0 0 
            /// 0 0 1 1
            public int Count(int[,] islandsGrid)
            {
                var immutableGrid = new int[islandsGrid.GetLength(0),islandsGrid.GetLength(1)];
                Array.Copy(islandsGrid, immutableGrid, islandsGrid.Length);

                var bfs = new Queue<(int i, int j)>();
                var islandsCount = 0;

                for (var i = 0; i < islandsGrid.GetLength(0); i++)
                {
                    for (var j = 0; j < islandsGrid.GetLength(1); j++)
                    {
                        if (immutableGrid[i,j] == 0)
                        {
                            continue;
                        }
                        else if (immutableGrid[i,j] == 1)
                        {
                            islandsCount++;
                            immutableGrid[i,j] = 2; // mark as visited
                            bfs.Enqueue((i, j));
                            while (bfs.Count > 0)
                            {
                                var current = bfs.Dequeue();
                                
                                if (current.i + 1 >= 0 && current.i + 1 < immutableGrid.GetLength(0) && immutableGrid[current.i + 1, current.j] == 1)
                                {
                                    immutableGrid[current.i + 1, current.j] = 2;
                                    bfs.Enqueue((current.i + 1, current.j));
                                }
                                if (current.i - 1 >= 0 && current.i - 1 < immutableGrid.GetLength(0) && immutableGrid[current.i - 1, current.j] == 1)
                                {
                                    immutableGrid[current.i - 1, current.j] = 2;
                                    bfs.Enqueue((current.i - 1, current.j));
                                }
                                if (current.j + 1 >= 0 && current.j + 1 < immutableGrid.GetLength(1) && immutableGrid[current.i, current.j + 1] == 1)
                                {
                                    immutableGrid[current.i, current.j +1] = 2;
                                    bfs.Enqueue((current.i, current.j + 1));
                                }
                                if (current.j - 1 >= 0 && current.j - 1 < immutableGrid.GetLength(1) && immutableGrid[current.i, current.j - 1] == 1)
                                {
                                    immutableGrid[current.i, current.j - 1] = 2;
                                    bfs.Enqueue((current.i, current.j - 1));
                                }
                            }
                        }
                        else
                        {
                            // it must be 2, thus already visited
                            continue;
                        }
                    }
                }

                return islandsCount;
            }
        }
    }
}
