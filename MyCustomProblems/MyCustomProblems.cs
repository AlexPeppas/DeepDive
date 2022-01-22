using System;
using System.Collections.Generic;
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
    }
}
