using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.MathExtensions;
using BEPUphysics;
using BEPUphysics.Paths.PathFollowing;

namespace Resonance
{
    class DynamicObject : Object
    {
        Entity body;
        double radius;

        public DynamicObject(int modelNum, string name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            bool isAdded = GameEntities.isAdded(modelNum);

            if (!isAdded)
            {
                GameEntities.addEntity(modelNum, true);
            }
            DynamicGameEntity entity = (DynamicGameEntity)GameEntities.getEntity(modelNum);
            body = new Entity(entity.Shape, entity.Mass, entity.InertiaTensor, entity.Volume);
            body.Position = pos;
            body.Tag = name;
        }

        /// <summary>
        /// Calculates the length of the longest horizontal edge of the bounding box
        /// </summary>
        public void calculateSize()
        {
            BoundingBox box = Body.CollisionInformation.BoundingBox;

            double x = Math.Abs(box.Max.X - box.Min.X);
            double z = Math.Abs(box.Max.Z - box.Min.Z);

            if (x > z) radius = x;
            else radius = z;
        }

        /// <summary>
        /// Returns the length of the shortest horizontal edge of the bounding box.
        /// </summary>
        /// <returns></returns>
        public float calculateMinBBEdge()
        {
            BoundingBox box = Body.CollisionInformation.BoundingBox;

            double x = Math.Abs(box.Max.X - box.Min.X);
            double z = Math.Abs(box.Max.Z - box.Min.Z);

            if (x < z) return (float) x;
            else return (float) z;
        }

        public void reset()
        {
            body.Position = base.OriginalPosition;
        }        

        public Entity Body
        {
            get
            {
                return body;
            }
        }

        public double Size
        {
            get
            {
                return radius;
            }
        }
    }
}
