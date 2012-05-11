using System;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance
{
    class Pickup : DynamicObject
    {
        public const int X2 = 0;
        public const int X3 = 1;
        public const int PLUS4 = 2;
        public const int PLUS5 = 3;

        private int pickupType;
        private int pickupLength; //length of time the pickup has an effect
        private int initialTime; //initial time
        private int timeToLive; //current time left
        private SingleEntityAngularMotor servo;

        public Pickup(int modelNum, String name, Vector3 pos, int length, int time)
            : base(modelNum, name, pos)
        {
            if (modelNum == GameModels.X2)
            {
                pickupType = X2;
            }
            else if (modelNum == GameModels.X3)
            {
                pickupType = X3;
            }
            else if (modelNum == GameModels.PLUS4)
            {
                pickupType = PLUS4;
            }
            else if (modelNum == GameModels.PLUS5)
            {
                pickupType = PLUS5;
            }

            pickupLength = length;
            initialTime = time;
            timeToLive = time;

            servo = new SingleEntityAngularMotor(this.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.SpringSettings.DampingConstant *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;

            //int uniqueId = 0;
            //foreach (byte x in Encoding.Unicode.GetBytes(name)) uniqueId += x;
            //Random r = new Random((int)(DateTime.Now.Ticks * uniqueId));
            Random r = new Random((int)(DateTime.Now.Ticks));
            float angle = MathHelper.ToRadians(r.Next(0, 360));

            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            servo.Settings.Servo.Goal = orientation;
            ScreenManager.game.World.addToSpace(servo);
        }

        /// <summary>
        /// Resets the timer on the Pickup to its initial value
        /// </summary>
        public void resetTimeToLive()
        {
            timeToLive = initialTime;
        }

        /// <summary>
        /// Rotates the pickup
        /// </summary>
        public void spinMe()
        {
            float angle = -0.1f;
            Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            Quaternion orientation = Quaternion.Concatenate(this.Body.Orientation, change);
            servo.Settings.Servo.Goal = orientation;
        }

        public int PowerUpType
        {
            get
            {
                return pickupType;
            }
        }

        public int PowerupLength
        {
            get
            {
                return pickupLength;
            }
        }

        public int TimeToLive
        {
            get
            {
                return timeToLive;
            }

            set
            {
                timeToLive = value;
            }
        }
    }
}
