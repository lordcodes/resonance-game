using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class Object
    {
        private int xWorldCord;
        private int yWorldCord;
        private int zWorldCord;

        public Object()
        {
            xWorldCord = 0;
            yWorldCord = 0;
            zWorldCord = 0;
        }

        public Object(int x, int y, int z)
        {
            xWorldCord = x;
            yWorldCord = y;
            zWorldCord = z;
        }

        public void setXWoldCord(int x)
        {
            xWorldCord = x;
        }

        public void setYWorldCord(int y)
        {
            yWorldCord = y;
        }

        public void setZWorldCord(int z)
        {
            zWorldCord = z;
        }

        public int getXWorldCord()
        {
            return xWorldCord;
        }

        public int getZWorldCord()
        {
            return zWorldCord;
        }

        public int getYWorldCord()
        {
            return yWorldCord;
        }
    }
}
