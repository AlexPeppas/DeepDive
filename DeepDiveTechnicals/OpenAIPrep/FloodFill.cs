using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepDiveTechnicals.OpenAIPrep
{
    public sealed class FloodFill
    {   
        private readonly int[,] _seed;

        public FloodFill(int[,] seed)
        {
            if (seed is null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            _seed = seed;
        }

        public int[,] PeekCurrentImage => _seed;

        public int[,] Fill(int x, int y, int newColor, bool useRecursion) // available colors are 0,1
        {
            if (x >= _seed.GetLength(0) || x < 0 || y >= _seed.GetLength(1) || y < 0)
            {
                throw new InvalidDataException("Cannot use x,y out of boundaries");
            }

            var parentColor = _seed[x,y];
            if (parentColor == newColor)
            {
                return _seed;
            }

            if (useRecursion)
            {
                FillRecursiveInternal(x, y, newColor, parentColor);
            }
            else
            {
                FillIterativeInternal(x, y, newColor, parentColor);
            }

            return _seed;
        }

        internal void FillRecursiveInternal(int x, int y, int newColor, int parentColor) // available colors are 0,1
        {
            if (x >= _seed.GetLength(0) || x < 0 || y >= _seed.GetLength(1) || y < 0)
            {
                Console.WriteLine("Out of bounds");
                return;
            }

            if (_seed[x,y] != parentColor)
            {
                return;
            }

            _seed[x, y] = newColor;
            FillRecursiveInternal(x - 1, y, newColor,parentColor); // go up
            FillRecursiveInternal(x, y - 1, newColor,parentColor); // go left
            FillRecursiveInternal(x, y + 1, newColor,parentColor); // go right
            FillRecursiveInternal(x + 1, y, newColor,parentColor); // go down
        }

        internal void FillIterativeInternal(int x, int y, int newColor, int parentColor) // available colors are 0,1
        {
            var bfsQueue = new Queue<(int x, int y)>();
            bfsQueue.Enqueue(new(x,y));
            while (bfsQueue.Count > 0) 
            {
                var currentCoordinates = bfsQueue.Dequeue();

                if (currentCoordinates.x >= _seed.GetLength(0) || currentCoordinates.x < 0 || currentCoordinates.y >= _seed.GetLength(1) || currentCoordinates.y < 0)
                {
                    Console.WriteLine("Out of bounds");
                    continue;
                }

                if (_seed[currentCoordinates.x, currentCoordinates.y] != parentColor)
                {
                    continue;
                }


                /// Don't pay the cost of enqueueing redundant cells, check before enqueue not after dequeue to avoid adding the same cell from multiple parents.
                _seed[currentCoordinates.x, currentCoordinates.y] = newColor;
                bfsQueue.Enqueue(new (currentCoordinates.x - 1, currentCoordinates.y)); // enqueue top neighbor
                bfsQueue.Enqueue(new(currentCoordinates.x, currentCoordinates.y - 1)); // enqueue left neighbor
                bfsQueue.Enqueue(new(currentCoordinates.x, currentCoordinates.y + 1)); // enqueue right neighbor
                bfsQueue.Enqueue(new(currentCoordinates.x + 1, currentCoordinates.y)); // enqueue bottom neighbor
            }
        }

        public static int[,] GenerateRandomImage(int xLength, int yLength)
        {
            var ran = new Random();
            var image = new int[xLength, yLength];
            for (var i =0; i < xLength; i++)
            {
                for (var j =0; j < yLength; j++)
                {
                    image[i, j] = ran.Next(0, 2);
                }
            }

            return image;
        }
    }
}
