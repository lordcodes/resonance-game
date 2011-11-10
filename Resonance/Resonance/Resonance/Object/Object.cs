using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class Object
    {
        private string identifier;
        private float xWorldCord;
        private float yWorldCord;
        private float zWorldCord;

        public Object()
        {
            xWorldCord = 0;
            yWorldCord = 0;
            zWorldCord = 0;
        }

        public void addIdentifier(string name)
        {
            identifier = name;
        }

        public string returnIdentifier()
        {
            return identifier;
        }
        public Object(float x, float y, float z)
        {
            xWorldCord = x;
            yWorldCord = y;
            zWorldCord = z;
        }

        public void setXWorldCord(float x)
        {
            xWorldCord = x;
        }

        public void setYWorldCord(float y)
        {
            yWorldCord = y;
        }

        public void setZWorldCord(float z)
        {
            zWorldCord = z;
        }

        public float getXWorldCord()
        {
            return xWorldCord;
        }

        public float getZWorldCord()
        {
            return zWorldCord;
        }

        public float getYWorldCord()
        {
            return yWorldCord;
        }
    }
}
