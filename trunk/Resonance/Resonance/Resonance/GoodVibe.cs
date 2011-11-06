using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class GoodVibe : VibeInterface
    {
        int health; //health stored as an int between 0 - 100.

        /// <summary>
        /// Constructor
        /// Set initial health to 100
        /// </summary>
        GoodVibe()
        {
            health = 100;
        }
               
        void VibeInterface.AdjustHealth(int change)
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

        void VibeInterface.SetHealth(int value)
        {
            //check between 0-100
            health = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int VibeInterface.GetHealth()
        {
            return health;
        }
    }
}
