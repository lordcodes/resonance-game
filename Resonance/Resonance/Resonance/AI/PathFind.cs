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

        public PathFind(Game game)
        {
            openList = new Minheap();
            closedList = new List<Node>();

            MAP_HEIGHT = (int)Math.Round(World.MAP_Z,0);
            MAP_WIDTH = (int)Math.Round(World.MAP_X,0);

            map = new bool[MAP_WIDTH, MAP_HEIGHT];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = game.World.querySpace(new Vector3(i, 0.5f, j));
                }
            }
        }

        public void pathFind(Vector3 pathStart, Vector3 pathEnd)
        {
            int startX = (int)Math.Round(pathStart.X);
            int startZ = (int)Math.Round(pathStart.Z);
            int endX = (int)Math.Round(pathEnd.X);
            int endZ = (int)Math.Round(pathEnd.Z);

            if (startX == endX && startZ == endZ) { } //already there
            else
            {
                //openList.add(new Node(start));
            }

        }
    }
}
