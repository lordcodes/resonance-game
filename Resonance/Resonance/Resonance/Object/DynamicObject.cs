using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.EntityStateManagement;

namespace Resonance
{
    class DynamicObject : Object
    {
        ConvexHull body;

        public DynamicObject(int modelNum, string name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(Drawing.gameModelStore.getModel(modelNum).model, out vertices, out indices);
            body = new ConvexHull(pos, vertices, Drawing.gameModelStore.getModel(modelNum).mass);
            //MotionState motion = new MotionState();
            //motion.Position = pos;
            //motion.LinearVelocity = Vector3.Zero;
            //motion.AngularVelocity = Vector3.Zero;
            //body = new ConvexHull(motion, vertices, Drawing.gameModelStore.getModel(modelNum).mass);
            body.Tag = Drawing.gameModelStore.getModel(modelNum);
        }

        public ConvexHull Body
        {
            get
            {
                return body;
            }
        }
    }
}
