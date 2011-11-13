using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class BadVibe : Object
    {
        int health;

        BadVibe(int modelNum, Game game) : base(modelNum, game)
        {
            health = 100;
        }

        void AdjustHealth(int change)
        {
        }


        void SetHealth(int value)
        {
        }

        int GetHealth()
        {
            return health;
        }
    }
}
