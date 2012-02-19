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
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance
{
    class Pickup : DynamicObject
    {
        //for reference
        public const int HEALTH = 2;
        public const int NITROUS = 3;
        public const int SHIELD = 4;
        public const int FREEZE = 5;

        private int powerupType;
        private int powerupLength; //length of time the powerup has an effect
        private int initialTime; //initial time
        private int timeToLive; //current time left
        private SingleEntityAngularMotor servo;

        public Pickup(int modelNum, String name, Vector3 pos, int length, int time)
            : base(modelNum, name, pos)
        {
            switch (modelNum)
            {
                case 12: //TODO: SORT THIS CRAP OUT
                    {
                        powerupType = 2;
                        break;
                    }
                case 13:
                    {
                        powerupType = 3;
                        break;
                    }
                case 14:
                    {
                        powerupType = 4;
                        break;
                    }
                case 15:
                    {
                        powerupType = 5;
                        break;
                    }
            }

            //powerupType = power;
            powerupLength = length;
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
                return powerupType;
            }
        }

        public int PowerupLength
        {
            get
            {
                return powerupLength;
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
