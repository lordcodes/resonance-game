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

        protected EntityRotator rotator;

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
            body.Tag = GameModels.getModel(modelNum);
            //body.Material.KineticFriction = 0.8f;
            //body.Material.StaticFriction = 1f;
            rotator = new EntityRotator(body);
            rotator.AngularMotor.Settings.Servo.SpringSettings.StiffnessConstant = 300000;
        }

        public void reset()
        {
            body.Position = base.OriginalPosition;
        }

        public void move(int direction)
        {
            Vector3 rotateVector = QuaternionToEuler(Body.Orientation);
            Vector3 velocity = Body.LinearVelocity;

            //Calculate which way to move Up, Down, Left or Right
            float xCoefficient = MOVE_SPEED;
            float zCoefficient = MOVE_SPEED;

            switch(direction)
            {
                case (BV_FORWARD):
                    {
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Cos(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Sin(rotateVector.Y));
                        break;
                    }
                case (BV_BACKWARD):
                    {
                        xCoefficient *= -1;
                        zCoefficient *= -1;
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Cos(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Sin(rotateVector.Y));
                        break;
                    }
                case (MOVE_LEFT):
                    {
                        xCoefficient *= -1;
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Sin(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Cos(rotateVector.Y));
                        break;
                    }
                case (MOVE_RIGHT):
                    {
                        zCoefficient *= -1;
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Sin(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Cos(rotateVector.Y));
                        break;
                    }
                case (MOVE_FORWARD):
                    {

                        xCoefficient *= -1;
                        zCoefficient *= -1;
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Cos(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Sin(rotateVector.Y));
                        break;
                    }
                case (MOVE_BACKWARD):
                    {
                        if (velocity.Z < 4 && velocity.Z > -4) velocity.Z += (float)(zCoefficient * Math.Cos(rotateVector.Y));
                        if (velocity.X < 4 && velocity.X > -4) velocity.X += (float)(xCoefficient * Math.Sin(rotateVector.Y));
                        break;
                    }
            }

            
            Body.LinearVelocity = velocity;
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
      /*  public void rotate(float angle)
        {
            Vector3 velocity = Body.AngularVelocity;
            if (velocity.Y < 2 && velocity.Y > -2) velocity.Y += (float)angle;
            Body.AngularVelocity = velocity;
        }
        */
       public void rotate(int direction)
        {
            float coefficient = ROTATE_SPEED;

            if (direction == ROTATE_CLOCK) coefficient *= -1;

            Quaternion rot;
            Vector3 axis = Vector3.Up;
            Quaternion.CreateFromAxisAngle(ref axis, coefficient, out rot);
            rot.X = 0;
            rot.Z = 0;
            rotator.TargetOrientation = Quaternion.Concatenate(Body.Orientation, rot);
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

        public EntityRotator Rotator
        {
            get
            {
                return rotator;
            }
        }
    }
}
