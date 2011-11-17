using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.MathExtensions;

namespace Resonance
{
    class GoodVibe : DynamicObject
    {
        int health; //health stored as an int between 0 - 100.

        /// <summary>
        /// Constructor
        /// Set initial health to 100
        /// </summary>
        public GoodVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            health = 100;
        }
               
        void AdjustHealth(int change)
        {
            //check for <0 or >100
            //health <0, dead vibe
            if (health - change <= 0)
            {
                health = 0;
                Console.WriteLine("Dead vibe!");
            }
            //full health
            else if (health + change >= 100)
            {
                health = 100;
                Console.WriteLine("Full health");
            }
            else
            {
                health += change;
            }
        }

        void SetHealth(int value)
        {
            //check between 0-100
            health = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHealth()
        {
            return health;
        }
    }
}
