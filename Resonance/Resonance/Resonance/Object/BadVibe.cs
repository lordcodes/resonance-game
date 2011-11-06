using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class BadVibe : VibeInterface
    {
        int health;

        BadVibe()
        {
            health = 100;
        }

        void VibeInterface.AdjustHealth(int change)
        {
        }


        void VibeInterface.SetHealth(int value)
        {
        }

        int VibeInterface.GetHealth()
        {
            return health;
        }
    }
}
