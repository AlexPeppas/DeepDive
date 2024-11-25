using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals
{
    /// <summary>
    /// In a classic problem Rat in a Maze use the technique of Memoization so you can scale this problem to thousand of paths and without reducing the performance dramatically.
    /// Approach and explanation inline (line-by-line)
    /// The rat can only move forward and downwards. -1 are blocking points. every place [r,c] has a distinct random integer.
    /// I have to fix the integers in stringBuilder because a "1" in a lookup of "054964910" will have a hit even if the last digits were initially "10"
    /// </summary>
    public static class MyCustomProblems
    {
        public static List<string> pathsMemo = new List<string>(); //the list of all available paths
        public static Tuple<int, int> src = new Tuple<int, int>(0, 0); //start point
        public static Tuple<int, int> dst = new Tuple<int, int>(3, 2); //end point
        public static void RatInMazeCached()
        {
            int[,] maze = new int[4, 3]
            {
                {0,2,3 },
                {8,-1,4 },
                {9,1,5 },
                {6,-1,7 }
                
            };
            RatInMazeCachedHelper(maze, src.Item1, src.Item2, new StringBuilder());
        }
        public static void RatInMazeCachedHelper(int[,] maze, int row, int col, StringBuilder currentBuilder)
        {
            if (row>=maze.GetLength(0) || row<0 || col>=maze.GetLength(1) || col<0)
            {
                //out of bounds
                return;
            }
            if (maze[row,col]==-1)
            {
                //blocking position
                return;
            }
            currentBuilder.Append(maze[row, col].ToString());
            if (row==dst.Item1 && col==dst.Item2)
            {
                //found destination
                pathsMemo.Add(currentBuilder.ToString());
                currentBuilder.Remove(currentBuilder.Length - 1, 1); //pop
                return;
            }
            
            bool pathPreExisted = false; // search in memo 
            var tempPathList = new List<string>(); // temp array if path pre-existed
            bool forwardNeighbor = false;
            bool downwardsNeighbor = false;

            foreach (var path in pathsMemo)
            {
                if (path.Contains(maze[row,col].ToString())) // if one existing path contains current [row,col] item
                {
                    pathPreExisted = true; //mark it
                    var indexOfPath = path.IndexOf(maze[row, col].ToString())+1; //retrieve the next item in the path 
                    if (row + 1 < maze.GetLength(0)) //in bounds
                    {
                        if (Convert.ToInt32(path[indexOfPath].ToString()) == maze[row + 1, col]) // if they match then the downwards neighbor has already been checked
                            downwardsNeighbor = true;
                    }
                    else
                        downwardsNeighbor = true; // out of bounds
                    if (col + 1 < maze.GetLength(1)) // in bounds
                    {
                        if (Convert.ToInt32(path[indexOfPath].ToString()) == maze[row, col + 1]) // if they match then the forward neighbor has already been checked
                            forwardNeighbor = true;
                    }
                    else
                        forwardNeighbor = true; // out of bounds

                    var cloneBuilder = new StringBuilder(currentBuilder.ToString());
                    while (indexOfPath<path.Length) // build the rest of the path from the memo and add the cloneBuilder to the tempPathList for every possible path in foreach
                    {
                        cloneBuilder.Append(path[indexOfPath].ToString());
                        indexOfPath++;
                    }
                    tempPathList.Add(cloneBuilder.ToString());
                    // add it in a temp and not pathsMemo because you cannot modify the already running target of foreach
                }
            }
            pathsMemo.AddRange(tempPathList);//when you finish add the tempList with new paths into the original pathsMemo

            
            if (!pathPreExisted) // if did not existed recurse as a new normal path is being built.
            {
                RatInMazeCachedHelper(maze, row, col + 1, currentBuilder);
                RatInMazeCachedHelper(maze, row + 1, col, currentBuilder);
                if (currentBuilder.Length > 0)
                    currentBuilder.Remove(currentBuilder.Length - 1, 1); //pop backtrack
            }
            else
            {
                if (forwardNeighbor && !downwardsNeighbor) // recurse only to downWardsNeighbor
                {
                    RatInMazeCachedHelper(maze, row + 1, col, currentBuilder);
                    if (currentBuilder.Length > 0)
                        currentBuilder.Remove(currentBuilder.Length - 1, 1); //pop backtrack
                }
                else if (!forwardNeighbor && downwardsNeighbor) // recurse only to forwardNeighbor
                {
                    RatInMazeCachedHelper(maze, row, col + 1, currentBuilder);
                    if (currentBuilder.Length > 0)
                        currentBuilder.Remove(currentBuilder.Length - 1, 1); //pop backtrack
                }
                else if (forwardNeighbor && downwardsNeighbor) // return because both neighbors has been visited
                {
                    if (currentBuilder.Length > 0)
                        currentBuilder.Remove(currentBuilder.Length - 1, 1); //pop backtrack
                    return;
                }
                else // pre-existed and non of the neighbors have been visited with only forward and downwards movement? Ops you're in error case ! 
                {
                    throw new Exception("You should not dive in here :/");
                }
            }
        }

        /// <summary>
        /// Start from (0,0) in a grid. Your robot starts moving forward. Every time it hits a blocking position (=-1) or
        /// out of bounds rotate 90degrees and keep moving.
        /// When you detect a cycle print the cycle block and return the list of all visited blocks.
        /// </summary>
        public static void RobotInGridCycleDetection()
        {
            var grid = new int[4, 4]
            {
                {0, 2, 3, 4 },
                {8, -1, 4, 5 },
                {9, 1, 5, 6 },
                {6, -1, 7, 8 }
            };

            DetectCycleAndReturnPath(grid, 0, 0, new List<(int row, int col, int angle)>());
        }

        private static int currentAngleDirection = 0; // 0, 90, 180, 270

        private static bool DetectCycleAndReturnPath(int[,] grid, int row, int col, List<(int row, int col, int angle)> path)
        {
            if (row < 0 || row >= grid.GetLength(0) || col < 0 || col >= grid.GetLength(1) || grid[row, col] == -1)
            {
                // out of bounds or blocking position
                if (currentAngleDirection + 90 > 270)
                {
                    currentAngleDirection = 0;
                }
                else
                {
                    currentAngleDirection += 90;
                }

                return false;
            }

            var syntheticKey = $"{row}{col}{currentAngleDirection}";
            if (SyntheticKeys.Contains(syntheticKey))
            {
                Debug.WriteLine($"Cycled detected at row {row}, col {col} with direction {currentAngleDirection}.");
                Debug.WriteLine($"Path is: {string.Join("|>", path.Select(item => $"[{item.row},{item.col},{item.angle}]"))}");
                return true;
            }

            SyntheticKeys.Add(syntheticKey);

            path.Add(new(row, col, currentAngleDirection));

            var firstPossibleDirection = PickMove(row, col, currentAngleDirection);
            var secondPossibleDirection = PickMove(row, col, currentAngleDirection+90);
            var thridPossibleDirection = PickMove(row, col, currentAngleDirection + 180);
            var fourthPossibleDirection = PickMove(row, col, currentAngleDirection + 270);
            
            return DetectCycleAndReturnPath(grid, firstPossibleDirection.row, firstPossibleDirection.col, path)
                || DetectCycleAndReturnPath(grid, secondPossibleDirection.row, secondPossibleDirection.col, path)
                || DetectCycleAndReturnPath(grid, thridPossibleDirection.row, thridPossibleDirection.col, path)
                || DetectCycleAndReturnPath(grid, fourthPossibleDirection.row, fourthPossibleDirection.col, path);
        }

        private static (int row, int col) PickMove(int row, int col, int angleDirection)
        {
            return angleDirection switch
            {
                90 => (row + 1, col), // move downards
                180 => (row, col - 1), // move backwrds left
                270 => (row - 1, col), // move upwards
                _ => (row, col + 1) // 0, 360 move forward right
            };
        }

        private static readonly HashSet<string> SyntheticKeys = new(); // to detect cycles we keep (int,row,angleDirection)
    }
}