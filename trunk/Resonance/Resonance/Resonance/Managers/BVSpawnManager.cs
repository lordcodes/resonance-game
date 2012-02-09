using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
        //private static int MIN_BVS = 15;

        //private static int numBVs = 0;
        //private static int totalNumBVs = 0;
        private static int spawnerCount = 2;
        private static int bvcount = 0;
        private static List<BVSpawner> spawners = new List<BVSpawner>();
        private static bool debugSwtich = false;

        public BVSpawnManager() 
        {
            spawnerCount = 2;
            spawners = new List<BVSpawner>();
        }

        public static void addNewSpawner(int totalBv, int radius, int totalActive,Vector3 position)
        {
            Vector3 pos;
            if (debugSwtich)
            {
                pos = new Vector3(-4f, 0.5f, -2f);
            }
            else
            {
                pos = position;
            }
            BVSpawner spawn = new BVSpawner(GameModels.BV_SPAWNER, "BV_SPAWNER" + spawnerCount, pos, totalBv,radius,totalActive);
            ScreenManager.game.World.addObject(spawn);
            spawners.Add(spawn);
            spawnerCount++;
        }

        public static void vibeDied(BadVibe bv)
        {
            spawners[bv.SpawnerIndex].removeBadVibe(bv);
        }

        public static void update()
        {
            int i = 0;
            while (i < spawners.Count)
            {
                if ((spawners[i].getTotalCurrentlyActive() < spawners[i].getTotalAllowedActive())
                    && (spawners[i].getTotalSpawned() <= spawners[i].getTotalBadVibes()))
                {
                    BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawners[i].getSpawnCords(), i);
                    spawners[i].addBadVibe(bv);
                    bvcount++;
                }
                i++;
            }
        }
    }
}
