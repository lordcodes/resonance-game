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
        private const float ROTATE_SPEED = 0.3f;
        private const float MOVE_SPEED = 0.25f;

        public const int BV_FORWARD = 0;
        public const int BV_BACKWARD = 1;
        public const int MOVE_LEFT = 2;
        public const int MOVE_RIGHT = 3;
        public const int MOVE_FORWARD = 4;
        public const int MOVE_BACKWARD = 5;

        public const int ROTATE_CLOCK = 0;
        public const int ROTATE_ANTI = 1;

        ConvexHull body;
        double radius;

        public DynamicObject(int modelNum, string name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(GameModels.getModel(modelNum).PhysicsModel, out vertices, out indices);
            float scaleFactor = GameModels.getModel(modelNum).PhysicsScale.M11;
            Vector3 scale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
            for (int i = 0; i < vertices.Length; i++ )
            {
                vertices[i] = Vector3.Multiply(vertices[i], scale);
            }
            //body = new ConvexHull(pos, vertices, GameModels.getModel(modelNum).mass);
            body = new ConvexHull(pos, vertices, 50f);
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
       
        public void jump(float height)
        {
           
            Quaternion orientation = Body.Orientation;
            Vector3 rotateVector = Utility.QuaternionToEuler(orientation);
            Vector3 velocity = Body.LinearVelocity;
            velocity.X = (float)(height * Math.Sin(rotateVector.Y));
            velocity.Y += (float) (height * Math.Cos(rotateVector.X));
            Body.LinearVelocity = velocity;
        }

        public ConvexHull Body
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