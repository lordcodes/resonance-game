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
        public Bullet(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
           

        }
    }
}
