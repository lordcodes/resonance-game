using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics;
using BEPUphysics.Paths;

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
