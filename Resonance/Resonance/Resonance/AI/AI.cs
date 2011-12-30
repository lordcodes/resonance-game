using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class AI
    {
        public AI(BadVibe enemy, Vector3 target)
        {
            PathFind pathFind = new PathFind();
            ThreadRun.run(delegate { pathFind.find(enemy.Body.Position, target); }, enemy.returnIdentifier() + " path-finding.");
        }


    }
}
