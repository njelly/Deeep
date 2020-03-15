using Tofunaut.UnityUtils;
using Tofunaut.Core;
using System.Collections.Generic;

namespace Tofunaut.Deeep.Game
{
    public static class PathFinder
    {
        public static IntVector2[] FindPath(IntVector2 start, IntVector2 goal, Actor actor)
        {
            IntVector2[] bestPath = new IntVector2[0];

            HashSet<IntVector2AsAStarNode> open = new HashSet<IntVector2AsAStarNode>();
            HashSet<IntVector2AsAStarNode> closed = new HashSet<IntVector2AsAStarNode>();
            open.Add(new IntVector2AsAStarNode
            {
                coord = start,
                previous = null,
                f = 0,
                g = 0,
                h = 0,
            });

            while (open.Count > 0)
            {
                List<IntVector2AsAStarNode> openAsList = new List<IntVector2AsAStarNode>(open);
                // sort the open list ascending by f value
                openAsList.Sort((IntVector2AsAStarNode a, IntVector2AsAStarNode b) =>
                {
                    return a.f.CompareTo(b.f);
                });

                // set the current node to the node with the least f
                IntVector2AsAStarNode currentNode = openAsList[0];

                closed.Add(openAsList[0]);
                open.Remove(currentNode);

                if (currentNode.coord.Equals(goal)) // remember to use .Equals() instead of == becuase these are not the same object
                {
                    bestPath = currentNode.ToPath();
                    break;
                }

                List<IntVector2> potentialNextCoords = new List<IntVector2>();
                if (actor.CanOccupyPosition((currentNode.coord + IntVector2.Up).ToUnityVector3_XY()))
                {
                    potentialNextCoords.Add(currentNode.coord + IntVector2.Up);
                }
                if (actor.CanOccupyPosition((currentNode.coord + IntVector2.Down).ToUnityVector3_XY()))
                {
                    potentialNextCoords.Add(currentNode.coord + IntVector2.Down);
                }
                if (actor.CanOccupyPosition((currentNode.coord + IntVector2.Left).ToUnityVector3_XY()))
                {
                    potentialNextCoords.Add(currentNode.coord + IntVector2.Left);
                }
                if (actor.CanOccupyPosition((currentNode.coord + IntVector2.Right).ToUnityVector3_XY()))
                {
                    potentialNextCoords.Add(currentNode.coord + IntVector2.Right);
                }

                foreach (IntVector2 coord in potentialNextCoords)
                {
                    IntVector2AsAStarNode childNode = new IntVector2AsAStarNode();
                    childNode.coord = coord;
                    childNode.previous = currentNode;
                    childNode.g = currentNode.g + 1;
                    childNode.h = (coord - goal).ManhattanDistance;
                    childNode.f = childNode.g + childNode.h;

                    // check if we've visited this coord but now we have a better path to it
                    bool foundBetterPath = false;
                    bool haveVisited = false;
                    foreach (IntVector2AsAStarNode openNode in open)
                    {
                        if (!openNode.coord.Equals(coord))
                        {
                            continue;
                        }

                        haveVisited = true;

                        if (openNode.f < childNode.f)
                        {
                            continue;
                        }

                        // we've found a better node!
                        openNode.g = childNode.g;
                        openNode.h = childNode.h;
                        openNode.f = childNode.f;
                        openNode.previous = childNode.previous;
                        foundBetterPath = true;
                    }

                    if (!haveVisited || !foundBetterPath)
                    {
                        open.Add(childNode);
                    }
                }
            }

            if (bestPath.Length == 0)
            {
                // we couldn't find a path, so return the path that gets us closest
                List<IntVector2AsAStarNode> closedAsList = new List<IntVector2AsAStarNode>(closed);
                closedAsList.Sort((IntVector2AsAStarNode a, IntVector2AsAStarNode b) =>
                {
                    return a.f.CompareTo(b.f);
                });

                bestPath = closedAsList[0].ToPath();
            }

            return bestPath;
        }
    }
}