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
        //public static readonly int NITROUS = 0;
        //public static readonly int SHIELD = 1;
        //public static readonly int FREEZE = 2;

        private int powerupType;
        private int initialTime;
        private int timeToLive;
        private float size;

        public Pickup(int modelNum, String name, Vector3 pos, int power, int time)
            : base(modelNum, name, pos)
        {
            powerupType = power;
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
