using System;
using Microsoft.Xna.Framework;

namespace Resonance
{

    class Bullet : DynamicObject
    {
        private GameModelInstance bullet;
        private static string color = "red";
        public Bullet(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
           

        }

        public void setColor(string col)
        {
           color = col;
        }

        public string getColor()
        {
            return color;
        }
    }
}
