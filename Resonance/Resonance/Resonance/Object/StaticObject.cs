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

namespace Resonance
{
    class StaticObject : Object
    {
        StaticMesh body;

        public StaticObject(int modelNum, string name, Game game, Vector3 pos) 
            : base(modelNum, name, game, pos)
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(Drawing.gameModelStore.getModel(modelNum).model, out vertices, out indices);
            body = new StaticMesh(vertices, indices, new AffineTransform(pos));
            body.Tag = Drawing.gameModelStore.getModel(modelNum);
        }

        public StaticMesh Body
        {
            get
            {
                return body;
            }
        }
    }
}
