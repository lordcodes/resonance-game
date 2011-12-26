using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Node
    {
        private int pointX;
        private int pointZ;
        private int fCost;
        private int gCost;
        private int hCost;

        public Node(int x, int z)
        {
            pointX = x;
            pointZ = z;
            hCost = 0;
            fCost = 0;
            gCost = 0;
        }

        public int X
        {
            get
            {
                return pointX;
            }
        }

        public int Z
        {
            get
            {
                return pointZ;
            }
        }

        public int F
        {
            get
            {
                return fCost;
            }
            set
            {
                fCost = value;
            }
        }

        public int G
        {
            get
            {
                return gCost;
            }
            set
            {
                gCost = value;
            }
        }

        public int H
        {
            get
            {
                return hCost;
            }
            set
            {
                hCost = value;
            }
        }
    }
}
