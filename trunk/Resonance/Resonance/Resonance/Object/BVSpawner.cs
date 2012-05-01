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
        private int maxBVs;
        private int spawnRadius;
        private int maxActive;

        private int totalSpawned;
        private List<BadVibe> badVibes;

        public BVSpawner(int modelNum, String name, Vector3 pos, int maxbvs, int radius, int maxactive)
            : base(modelNum, name, pos)
        {
            badVibes = new List<BadVibe>(maxBVs);
            maxBVs = maxbvs;
            spawnRadius = radius;
            maxActive = maxactive;
            totalSpawned = 0;
        }

        public void addBV(BadVibe bv)
        {
            badVibes.Add(bv);
            ScreenManager.game.World.addObject(bv);
            totalSpawned++;
        }

        public void replaceBV(BadVibe bv, BadVibe newBv, int spawnNumber)
        {
            badVibes.Remove(bv);
            ScreenManager.game.World.removeObject(bv);
            if(badVibes.Count < maxActive && totalSpawned <= maxBVs)
            {
                newBv.setup(getSpawnCords(), spawnNumber);
                badVibes.Add(newBv);
                ScreenManager.game.World.addObject(newBv);
                totalSpawned++;
            }
        }

        public int MaxBVs
        {
            get { return maxBVs; }
        }

        public int TotalSpawned
        {
            get { return totalSpawned; }
        }

        public int MaxActive
        {
            get { return maxActive; }
        }

        public int CurrentActive
        {
            get { return badVibes.Count; }
        }

        public Vector3 getSpawnCords()
        {
            Vector3 pos;
            Random r = new Random((int)DateTime.Now.Ticks);
            do
            {
                int x = r.Next((int)(this.OriginalPosition.X - spawnRadius), (int)(this.OriginalPosition.X + spawnRadius));
                int z = r.Next((int)(this.OriginalPosition.Z - spawnRadius), (int)(this.OriginalPosition.Z + spawnRadius));
                pos = new Vector3((float)x, 25f, (float)z);
            } while (ScreenManager.game.World.querySpace(pos));
            return pos;
        } 
    }
}
