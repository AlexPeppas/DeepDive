using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.OpenAIPrep
{
    /// <summary>
    /// If no fresh orange initially return 0
    /// If impossible return -1
    /// For each fresh (1) orange of a 4-d neighbor rotten (2) orange, it takes 1min to rot.
    /// </summary>
    public sealed class RottingOranges
    {
        private readonly int[,] _seed;

        public RottingOranges(int[,] seed)
        {
            if (seed is null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            _seed = seed;
        }

        public int[,] PeekMaze => _seed;

        public int CalculateTimeUntilEveryOrangeRottens()
        {
            var layerRottenQueue = new Queue<(int x,int y)> ();
            var numberOfFreshOranges = 0;
            var totalMinutes = 0;

            for (var i=0; i< _seed.GetLength(0); i++)
            {
                for (var j=0; j< _seed.GetLength(1); j++)
                {
                    if (_seed[i,j] == 2)
                    {
                        layerRottenQueue.Enqueue((i, j));
                        continue;
                    }
                    if (_seed[i,j] == 1)
                    {
                        numberOfFreshOranges++;
                    }
                }
            }

            if (numberOfFreshOranges == 0)
            {
                return 0;
            }

            while (layerRottenQueue.Count > 0 && numberOfFreshOranges >0) // one layer at a time
            {
                var foundFreshPerLayer = false;
                var layerCount = layerRottenQueue.Count;
                for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
                {
                    var (x, y) = layerRottenQueue.Dequeue();

                    /// Top Neighbor
                    foundFreshPerLayer |= Move(x - 1, y, ref numberOfFreshOranges, layerRottenQueue);

                    /// Left Neighbot
                    foundFreshPerLayer |= Move(x, y - 1, ref numberOfFreshOranges, layerRottenQueue);

                    /// Right Neighbot
                    foundFreshPerLayer |= Move(x, y + 1, ref numberOfFreshOranges, layerRottenQueue);

                    /// Bottom Neighbor
                    foundFreshPerLayer |= Move(x + 1, y, ref numberOfFreshOranges, layerRottenQueue);
                }

                if (foundFreshPerLayer)
                {
                    totalMinutes++;
                }
            }

            if (numberOfFreshOranges > 0)
            {
                return -1; // impossible
            }

            return totalMinutes;
        }

        private bool Move(int x, int y, ref int numberOfFreshOranges, Queue<(int, int)> rottenQueue)
        {
            if (x >= 0 && x < _seed.GetLength(0) && y >= 0 && y < _seed.GetLength(1))
            {
                if (_seed[x , y] == 1)
                {
                    numberOfFreshOranges--;
                    _seed[x, y] = 2;
                    rottenQueue.Enqueue((x, y));
                    return true;
                }
            }

            return false;
        }
    }
}
