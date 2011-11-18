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
            float scaleFactor = Drawing.gameModelStore.getModel(modelNum).scale.M11;
            Vector3 scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Multiply(vertices[i], scale);
            }
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
