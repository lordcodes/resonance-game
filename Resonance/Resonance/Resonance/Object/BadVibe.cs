using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BadVibe : DynamicObject
    {
        int health;

        public BadVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
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
