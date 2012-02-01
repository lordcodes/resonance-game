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
    class Pickup : StaticObject
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
        private float size;

        public Pickup(int modelNum, String name, Vector3 pos, int power, int length, int time)
            : base(modelNum, name, pos)
        {
            powerupType = power;
            powerupLength = length;
            initialTime = time;
            timeToLive = time;
            size = 5f; //TODO: model size
        }

        /// <summary>
        /// Resets the timer on the Pickup to its initial value
        /// </summary>
        public void resetTimeToLive()
        {
            timeToLive = initialTime;
        }

        public float Size
        {
            get
            {
                return size;
            }
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
