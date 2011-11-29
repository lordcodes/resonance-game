using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Shockwave : Object
    {
        public Shockwave(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
        }
    }
}
