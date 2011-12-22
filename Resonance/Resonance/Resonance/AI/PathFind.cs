using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PathFind
    {
        private const float TILE_ACCURACY = 1;

        private static int MAP_HEIGHT;
        private static int MAP_WIDTH;

        Minheap openList;
        List<Node> closedList;

        public PathFind(Game game)
        {
            openList = new Minheap();
            closedList = new List<Node>();

            StaticObject o = (StaticObject)game.World.getObject("ground");
            
        }

        public void pathFind(Vector3 start, Vector3 end)
        {
            if (start == end) { } //already there
            else
            {
                openList.add(new Node(start));
            }



        }
    }
}
