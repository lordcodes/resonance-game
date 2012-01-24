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

        public StaticObject(int modelNum, string name, Vector3 pos) 
            : base(modelNum, name, pos)
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(GameModels.getModel(modelNum).PhysicsModel, out vertices, out indices);
            float scaleFactor = GameModels.getModel(modelNum).PhysicsScale.M11;
            Vector3 scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Multiply(vertices[i], scale);
            }
            body = new StaticMesh(vertices, indices, new AffineTransform(pos));
            body.Tag = name;
            body.Material.KineticFriction = 0.8f;
            body.Material.StaticFriction = 1f;
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
