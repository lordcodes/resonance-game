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


    class Projectile_BV : BadVibe
    {
       public Projectile_BV(int modelNum, String name, Vector3 position,int spawnNumber)
            : base(modelNum, name, position,spawnNumber)
        {
            
        }
        
    }
}
