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
            TriangleMesh.GetVerticesAndIndicesFromModel(Drawing.gameModelStore.getModel(modelNum).model, out vertices, out indices);
            body = new ConvexHull(pos, vertices, Drawing.gameModelStore.getModel(modelNum).mass);
            body.Tag = Drawing.gameModelStore.getModel(modelNum);
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

        public void rotate(float angle)
        {
            Vector3 velocity = Body.AngularVelocity;
            if (velocity.Y < 2.5 && velocity.Y > -2.5) velocity.Y += (float)angle;
            Body.AngularVelocity = velocity;
        }

        public Vector3 QuaternionToEuler(Quaternion rotation)
        {
            float q0 = rotation.W;
            float q1 = rotation.Y;
            float q2 = rotation.X;
            float q3 = rotation.Z;

            Vector3 radAngles = new Vector3();
            radAngles.Y = (float)Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
            radAngles.X = (float)Math.Asin(2 * (q0 * q2 - q3 * q1));
            radAngles.Z = (float)Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));

            Vector3 angles = new Vector3();
            angles.X = MathHelper.ToDegrees(radAngles.X);
            angles.Y = MathHelper.ToDegrees(radAngles.Y);
            angles.Z = MathHelper.ToDegrees(radAngles.Z);
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
