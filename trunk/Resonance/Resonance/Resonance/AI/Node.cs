using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Node
    {
        private Vector3 point;
        private int fCost;
        private int gCost;
        private int hCost;

        public Node(Vector3 position)
        {
            point = position;
        }

        public Vector3 Point
        {
            get
            {
                return point;
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
