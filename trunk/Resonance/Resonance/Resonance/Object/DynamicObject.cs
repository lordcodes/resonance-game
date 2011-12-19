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
            TriangleMesh.GetVerticesAndIndicesFromModel(GameModels.getModel(modelNum).PhysicsModel, out vertices, out indices);
            float scaleFactor = GameModels.getModel(modelNum).PhysicsScale.M11;
            Vector3 scale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
            for (int i = 0; i < vertices.Length; i++ )
            {
                vertices[i] = Vector3.Multiply(vertices[i], scale);
            }
            body = new ConvexHull(pos, vertices, GameModels.getModel(modelNum).mass);
            body.Tag = GameModels.getModel(modelNum);
            body.Material.KineticFriction = 0.8f;
            body.Material.StaticFriction = 1f;
        }

        public void move(float distance)
        {
            Quaternion orientation = Body.Orientation;
            Vector3 rotateVector = QuaternionToEuler(orientation);

            Vector3 velocity = Body.LinearVelocity;
            if(velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(distance * Math.Cos(rotateVector.Y));
            if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(distance * Math.Sin(rotateVector.Y));
            Body.LinearVelocity = velocity;
        }
        public void moveLeft(float distance)
        {
            Quaternion orientation = Body.Orientation;
            Vector3 rotateVector = QuaternionToEuler(orientation);
            Vector3 velocity = Body.LinearVelocity;

             velocity.Z += (float)(distance * Math.Sin(rotateVector.Y));
             velocity.X -= (float)(distance * Math.Cos(rotateVector.Y));
            
            Body.LinearVelocity = velocity;
        }
        public void moveRight(float distance)
        {
            Quaternion orientation = Body.Orientation;
            Vector3 rotateVector = QuaternionToEuler(orientation);
            Vector3 velocity = Body.LinearVelocity;

            velocity.Z += (float)(distance * Math.Sin(rotateVector.Y));
            velocity.X -= (float)(distance * Math.Cos(rotateVector.Y));

            Body.LinearVelocity = velocity;
        }
        public void rotate(float angle)
        {
            Vector3 velocity = Body.AngularVelocity;
            if (velocity.Y < 2 && velocity.Y > -2) velocity.Y += (float)angle;
            Body.AngularVelocity = velocity;
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
    }
}
