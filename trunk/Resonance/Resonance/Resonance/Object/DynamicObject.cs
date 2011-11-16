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
            //body.IsAffectedByGravity = false;
            body.Tag = Drawing.gameModelStore.getModel(modelNum);
        }

        public void updatePosition(Vector3 position)
        {
            //Matrix rotation = Matrix.CreateRotationY(position.W);
            Vector3 newPosition = new Vector3(position.X, position.Y, position.Z);
            Body.Position = newPosition;
            //Body.WorldTransform = Matrix.Multiply(Drawing.gameModelStore.getModel(GameModels.GOOD_VIBE).scale, Matrix.Multiply(rotation, Matrix.CreateTranslation(newPosition)));  
        }

        public void updatePosition(float x, float z)
        {
            //Matrix rotation = Matrix.CreateRotationY(position.W);
            //Vector3 newPosition = new Vector3(position.X, position.Y, position.Z);
            Body.Position += new Vector3(0.05f, 0f, 0f);
            //Body.WorldTransform = Matrix.Multiply(Drawing.gameModelStore.getModel(GameModels.GOOD_VIBE).scale, Matrix.Multiply(rotation, Matrix.CreateTranslation(newPosition)));  
        }

        public void move(float distance)
        {
            Vector3 p = Body.Position;
            p.Z += (float)(distance * Math.Cos(rotation));
            p.X += (float)(distance * Math.Sin(rotation));
            Body.Position = p;
        }

        public void rotate(float angle)
        {
            rotation += angle;
            if (rotation > 2 * Math.PI)
            {
                rotation -= (float)(2 * Math.PI);
            }
            else if(rotation < 0)
            {
                rotation += (float)(2 * Math.PI);
            }
            float cosRotate = (float)Math.Cos(rotation);
            float sinRotate = (float)Math.Sin(rotation);
            Matrix3X3 orientation = new Matrix3X3(cosRotate, 0f, sinRotate, 0f, 1f, 0f, -sinRotate, 0, cosRotate);
            Body.OrientationMatrix = orientation;
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
