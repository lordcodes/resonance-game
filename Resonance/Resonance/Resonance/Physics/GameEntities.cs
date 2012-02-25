using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace Resonance
{
    class GameEntities
    {
        private static Dictionary<int, GameEntity> entities = new Dictionary<int, GameEntity>();

        public static bool isAdded(int modelNum)
        {
            return entities.ContainsKey(modelNum);
        }

        public static void addEntity(int modelNum, bool dynamic)
        {
            if (dynamic)
            {
                entities.Add(modelNum, new DynamicGameEntity(modelNum));
            }
            else
            {
                entities.Add(modelNum, new StaticGameEntity(modelNum));
            }
        }

        public static GameEntity getEntity(int modelNum)
        {
            return entities[modelNum];
        }
    }
}
