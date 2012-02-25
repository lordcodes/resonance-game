using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using BEPUphysics.Collidables;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.Entities;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace Resonance
{
    class StaticObject : Object
    {
        InstancedMesh body;

        public StaticObject(int modelNum, string name, Vector3 pos) 
            : base(modelNum, name, pos)
        {
            bool isAdded = GameEntities.isAdded(modelNum);

            if (!isAdded)
            {
                GameEntities.addEntity(modelNum, false);
            }
            StaticGameEntity entity = (StaticGameEntity)GameEntities.getEntity(modelNum);
            body = new InstancedMesh(entity.Shape, new AffineTransform(pos));
            body.Sidedness = TriangleSidedness.Counterclockwise;
            body.Tag = name;
            //body.Material.KineticFriction = 0.8f;
            //body.Material.StaticFriction = 1f;
        }

        public InstancedMesh Body
        {
            get
            {
                return body;
            }
        }
    }
}
