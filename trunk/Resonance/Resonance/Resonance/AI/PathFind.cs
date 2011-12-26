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
        List<Node> closedList;

        bool[,] map;

        public PathFind()
        {
            openList = new Minheap();
            closedList = new List<Node>();

            MAP_HEIGHT = (int)Math.Round(World.MAP_Z,0);
            MAP_WIDTH = (int)Math.Round(World.MAP_X,0);

            //DebugDisplay.update("World", "" + World.MAP_X + " " + World.MAP_Z + " " + MAP_WIDTH + " " + MAP_HEIGHT);

            map = new bool[MAP_WIDTH, MAP_HEIGHT];
            buildMap();
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

        public void find(Vector3 pathStart, Vector3 pathEnd)
        {
            int startX = (int)Math.Round(pathStart.X);
            int startZ = (int)Math.Round(pathStart.Z);
            int endX = (int)Math.Round(pathEnd.X);
            int endZ = (int)Math.Round(pathEnd.Z);

            //DebugDisplay.update("Start", "" + startX + " " + startZ);

            //Start and end validity checks
            if (startX == endX && startZ == endZ) { return; } //already there
            else if (map[endX, endZ]) { return; } //target square is blocked

            //Add starting square to open list to be checked
            openList.add(new Node(startX, startZ));

            //Repeat until path is found
            do
            {
                //if open list is not empty
                if (openList.Count > 0)
                {
                    //Get root of the open list and add to closed list
                    Node current = openList.extractRoot();
                    closedList.Add(current);
                    //DebugDisplay.update("Root", "" + current.X + " " + current.Z);
                }
                break;
            } while (true);

        }

    }
}
