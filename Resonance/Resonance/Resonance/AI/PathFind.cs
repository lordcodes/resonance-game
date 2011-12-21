using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PathFind
    {
        public static Node pathFind(Vector3 pathStart, Vector3 pathEnd)
        {
            Vector3 start = pathEnd;
            Vector3 end = pathStart;

            Node beginNode = new Node(start, 0, 0, null);

            Minheap heap = new Minheap();
            heap.add(beginNode);


            return beginNode;
        }
    }
}
