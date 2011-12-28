using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PathFind
    {
        //private const float TILE_SIZE = 1;

        private static int MAP_HEIGHT;
        private static int MAP_WIDTH;

        Minheap openList;
        //List<Vector2> closedList;
        List<int[]> closedList;

        Vector3[,] parents;

        bool[,] map;

        public PathFind()
        {
            openList = new Minheap();
            //closedList = new List<Vector2>();
            closedList = new List<int[]>();

            MAP_HEIGHT = (int)Math.Round(World.MAP_Z,0);
            MAP_WIDTH = (int)Math.Round(World.MAP_X,0);

            parents = new Vector3[MAP_WIDTH, MAP_HEIGHT];

            //DebugDisplay.update("World", "" + World.MAP_X + " " + World.MAP_Z + " " + MAP_WIDTH + " " + MAP_HEIGHT);

            //map = new bool[MAP_WIDTH, MAP_HEIGHT];
            //buildMap();
        }

        private void buildMap()
        {
            int xDir = MAP_WIDTH / 2;
            int zDir = MAP_HEIGHT / 2;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = Program.game.World.querySpace(new Vector3((i - xDir), 0.5f, (j - zDir)));
                }
            }
        }

        private bool checkPosition(int x, int z)
        {
            return Program.game.World.querySpace(new Vector3(x, 0.5f, z));
        }

        public int find(Vector3 pathStart, Vector3 pathEnd)
        {
            bool pathFound = false;

            int startX = (int)Math.Round(pathStart.X);
            int startZ = (int)Math.Round(pathStart.Z);
            int endX = (int)Math.Round(pathEnd.X);
            int endZ = (int)Math.Round(pathEnd.Z);

            //DebugDisplay.update("Start", "" + startX + " " + startZ);

            //Start and end validity checks
            if (startX == endX && startZ == endZ) { return -1; } //already there
            else if (checkPosition(endX, endZ)) { return -2; } //target square is blocked

            //Add starting square to open list to be checked
            openList.add(new Node(startX, startZ));

            //Repeat until path is found
            while(true)
            {
                //if open list is not empty
                if (openList.Count > 0)
                {
                    //Get root of the open list and add to closed list
                    Node current = openList.extractRoot();
                    //closedList.Add(new Vector2(current.X, current.Z));
                    int[] currentNode = { current.X, current.Z };
                    closedList.Add(currentNode);
                    //DebugDisplay.update("Root", "" + current.X + " " + current.Z);
                    Console.WriteLine(current.X + " " + current.Z);

                    //Check adjacent nodes
                    for (int j = current.Z - 1; j <= current.Z + 1; j++)
                    {
                        for (int i = current.X - 1; i <= current.X + 1; i++)
                        {
                            if (adjacentNodeChecks(i, j, current))
                            {
                                //Console.WriteLine("IJ: "+i + " " + j);
                                //If not already on open list add it
                                if (!openList.contains(i, j))
                                {
                                    Node newNode = new Node(i, j);
                                    //If diagonal move, G cost
                                    if ((i == current.X -1 || i == current.X + 1) && (j == current.Z -1 || j == current.Z + 1))
                                    {
                                        newNode.G = current.G + 7;
                                    }
                                    //If horizontal or vertical move, G cost
                                    else
                                    {
                                        newNode.G = current.G + 5;
                                    }
                                    //H cost
                                    int xDistance = Math.Abs(current.X - endX);
                                    int zDistance = Math.Abs(current.Z - endZ);
                                    if (xDistance > zDistance)
                                    {
                                        newNode.H = (7 * zDistance) + (5 * (xDistance - zDistance));
                                    }
                                    else
                                    {
                                        newNode.H = (7 * xDistance) + (5 * (zDistance - xDistance));
                                    }
                                    //F cost
                                    newNode.F = newNode.G + newNode.H;
                                    //Parent
                                    int xDir = MAP_WIDTH / 2;
                                    int zDir = MAP_HEIGHT / 2;
                                    parents[(i+xDir), (j+zDir)] = new Vector3(current.X, 0.4f, current.Z);
                                    //Add to open list
                                    openList.add(newNode);
                                }
                                else
                                {
                                    int gCost = 0;
                                    //If diagonal move, G cost
                                    if ((i == current.X - 1 || i == current.X + 1) && (j == current.Z - 1 || j == current.Z + 1))
                                    {
                                        gCost = current.G + 7;
                                    }
                                    //If horizontal or vertical move, G cost
                                    else
                                    {
                                        gCost = current.G + 5;
                                    }
                                    //Check if new G is less than current G for that square
                                    if (gCost < openList.accessElementGCost(i, j))
                                    {
                                        openList.setElementCosts(i, j, gCost);
                                        int xDir = MAP_WIDTH / 2;
                                        int zDir = MAP_HEIGHT / 2;
                                        parents[(i+xDir), (j+zDir)] = new Vector3(current.X, 0.4f, current.Z);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //No path exists
                    break;
                }
                if(openList.contains(endX,endZ))
                {
                    //Path found
                    pathFound = true;
                    break;
                }
            }
            if (pathFound)
            {
                int x = endX, z = endZ;
                List<Vector3> path = new List<Vector3>();
                do
                {
                    Console.Write("("+x + "," + z + ") ");
                    path.Add(new Vector3(x, 0.5f, z));
                    int xDir = MAP_WIDTH / 2;
                    int zDir = MAP_HEIGHT / 2;
                    x = (int)parents[(x+xDir), (z+zDir)].X;
                    z = (int)parents[(x + xDir), (z + zDir)].Z;
                    if (path.Count > 100) break; //For debugging until bug is fixed
                } while ((x != startX) && (z != startZ));
                //return path;
                return 1;
            }
            else
            {
                //return null;
                return 2;
            }
        }

        private bool adjacentNodeChecks(int x, int z, Node current)
        {
            //Console.WriteLine("Current1: " + current.X + " " + current.Z + " " + x + " " + z);
            int xDir = MAP_WIDTH / 2;
            int zDir = MAP_HEIGHT / 2;
            //Make sure not checking outside map
            if (z >= -zDir && x >= -xDir)
            {
                if (z <= zDir && x <= xDir)
                {
                    //Console.WriteLine("Current2: " + current.X + " " + current.Z + " " + x + " " + z);
                    //Check it isn't already in closed list
                    int[] test = {x,z};
                    if (!closedList.Contains(test))
                    {
                        //Console.WriteLine("Current3: " + current.X + " " + current.Z + " " + x + " " + z);
                        //Check it isn't blocked
                        if (!checkPosition(x, z))
                        {
                            //Console.WriteLine("Current4: " + current.X + " " + current.Z + " " + x + " " + z);
                            //If moving diagonal, check diagonal path is clear
                            bool walkable = true;
                            if (x == current.X + 1 && z == current.Z + 1)
                            {
                                if (checkPosition(current.X, current.Z + 1) || checkPosition(current.X + 1, current.Z))
                                {
                                    walkable = false;
                                }
                            }
                            else if (x == current.X + 1 && z == current.Z - 1)
                            {
                                if (checkPosition(current.X, current.Z - 1) || checkPosition(current.X + 1, current.Z))
                                {
                                    walkable = false;
                                }
                            }
                            else if (x == current.X - 1 && z == current.Z + 1)
                            {
                                if (checkPosition(current.X, current.Z + 1) || checkPosition(current.X - 1, current.Z))
                                {
                                    walkable = false;
                                }
                            }
                            else if (x == current.X - 1 && z == current.Z - 1)
                            {
                                if (checkPosition(current.X, current.Z - 1) || checkPosition(current.X - 1, current.Z))
                                {
                                    walkable = false;
                                }
                            }
                            if (walkable)
                            {
                                //Console.WriteLine("Current: " + current.X + " " + current.Z + " " + x + " " + z);
                                if (x == current.X && z == current.Z) return false;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
