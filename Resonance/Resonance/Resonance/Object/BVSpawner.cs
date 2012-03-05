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
    class BVSpawner : StaticObject
    {
        private int totalBadVibes;
        private int spawnRadius;
        private int totalActive;
        private int totalSpawned;
        private List<BadVibe> badVibes;

        public BVSpawner(int modelNum, String name, Vector3 pos, int TotalBadVibes, int SpawnRadius, int TotalActive)
            : base(modelNum, name, pos)
        {
            badVibes = new List<BadVibe>();
            totalBadVibes = TotalBadVibes;
            spawnRadius = SpawnRadius;
            totalActive = TotalActive;
            totalSpawned = 0;
        }

        public void addBadVibe(BadVibe bv)
        {
            badVibes.Add(bv);
            ScreenManager.game.World.addObject(bv);
            totalSpawned++;
        }

       

        public int getTotalSpawned()
        {
            return totalSpawned;
        }

        public void removeBadVibe(BadVibe bv)
        {
            badVibes.Remove(bv);
        }

        public int getTotalBadVibes()
        {
            return totalBadVibes;
        }

        public int getTotalAllowedActive()
        {
            return totalActive;
        }

        public int getTotalCurrentlyActive()
        {
            return badVibes.Count;
        }

        public Vector3 getSpawnCords()
        {
            Vector3 pos;
            do
            {
                Random r = new Random((int)DateTime.Now.Ticks);
                int x = r.Next((int)(this.OriginalPosition.X - spawnRadius), (int)(this.OriginalPosition.X + spawnRadius));
                int z = r.Next((int)(this.OriginalPosition.Z - spawnRadius), (int)(this.OriginalPosition.Z + spawnRadius));
                pos = new Vector3((float)x, 0.5f, (float)z);
            } while (ScreenManager.game.World.querySpace(pos));
            DebugDisplay.update("X = ",pos.X.ToString());
            DebugDisplay.update("Y = ", pos.Z.ToString());
            return pos;
        } 
    }
}
