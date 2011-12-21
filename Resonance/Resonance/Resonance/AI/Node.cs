using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Node
    {
        public Vector3 Point { get; set; }
        public int Cost { get; set; }
        public int PathCost { get; set; }
        public Node After { get; set; }
        public Node NextElement { get; set; }

        public Node(Vector3 position, int cost, int pathcost, Node next)
        {
            Point = position;
            Cost = cost;
            PathCost = pathcost;
            After = next;
        }
    }
}
