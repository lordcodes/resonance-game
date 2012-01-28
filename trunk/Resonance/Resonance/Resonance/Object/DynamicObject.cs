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

        public void calculateSize()
        {
            BoundingBox box = Body.CollisionInformation.BoundingBox;

            double x = Math.Abs(box.Max.X - box.Min.X);
            double z = Math.Abs(box.Max.Z - box.Min.Z);

            if (x > z) radius = x;
            else radius = z;

            DebugDisplay.update("BoxDetails", box.Max.X + "," + box.Max.Z + " : " + box.Min.X + "," + box.Min.Z);
            DebugDisplay.update("BVDetails", x + " : " + z + " : " + radius);
        }

        public void reset()
        {
            body.Position = base.OriginalPosition;
        }        
       
        public void jump(float height)
        {
           
            Quaternion orientation = Body.Orientation;
            Vector3 rotateVector = QuaternionToEuler(orientation);
            Vector3 velocity = Body.LinearVelocity;
            velocity.X = (float)(height * Math.Sin(rotateVector.Y));
            velocity.Y += (float) (height * Math.Cos(rotateVector.X));
            Body.LinearVelocity = velocity;
        }

        public static Vector3 QuaternionToEuler(Quaternion quat)
        {
            float w = quat.W;
            float y = quat.Y;
            float x = quat.X;
            float z = quat.Z;

            Vector3 radAngles = new Vector3();
            radAngles.Y = (float)Math.Atan2(2 * (w * y + x * z), 1 - 2 * (Math.Pow(y, 2) + Math.Pow(x, 2)));
            radAngles.X = (float)Math.Asin(2 * (w * x - z * y));
            radAngles.Z = (float)Math.Atan2(2 * (w * z + y * x), 1 - 2 * (Math.Pow(x, 2) + Math.Pow(z, 2)));
            return radAngles;
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
