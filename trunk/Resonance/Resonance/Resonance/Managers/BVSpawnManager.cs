using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
        private static int spawnerCount;
        private static int bvcount = 0;
        private static List<BVSpawner> spawners;

        //Allocated variables
        private static List<BadVibe> pool;

        public BVSpawnManager() 
        {
            spawnerCount = 1;
            spawners = new List<BVSpawner>();
        }

        public static void allocate()
        {
            pool = new List<BadVibe>(50);
            for (int i = 0; i < 50; i++)
            {
                BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BV" + i, Vector3.Zero, 0);
                pool.Add(bv);
            }
        }

        private static BadVibe getBadVibe()
        {
            BadVibe bv = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return bv;
        }

        private static void addToPool(BadVibe bv)
        {
            pool.Add(bv);
        }

        public static void addNewSpawner(int totalBv, int radius, int totalActive,Vector3 position)
        {
            int i = 0;
            Vector3 pos;
            pos = position;
            BVSpawner spawn = new BVSpawner(GameModels.BV_SPAWNER, "BV_SPAWNER" + spawnerCount, pos, totalBv,radius,totalActive);
            ScreenManager.game.World.addObject(spawn);
            spawners.Add(spawn);
            Random random = new Random();
            while (i < spawn.getTotalAllowedActive())
            {
                /*int rand = random.Next(234);
                if (rand % 2 == 0)
                {*/
                    //BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                BadVibe bv = getBadVibe();
                bv.setup(spawn.getSpawnCords(), spawners.Count - 1);
                spawn.addBadVibe(bv);
                /*}
                else
                {
                    Projectile_BV newProjBV = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                    spawn.addBadVibe( newProjBV);
                }*/
                bvcount++;
                i++;
            }
            spawnerCount++;
        }

        public static void vibeDied(BadVibe bv)
        {
            Random random = new Random();

            int s = bv.SpawnerIndex;
            BVSpawner spawn = spawners[s];
            spawn.removeBadVibe(bv);
            if ((spawn.getTotalCurrentlyActive() < spawn.getTotalAllowedActive())
                    && (spawn.getTotalSpawned() <= spawn.getTotalBadVibes()))
            {
               /*int rand = random.Next(234);
               if (rand % 2 == 0)
                {*/
                BadVibe newBv = getBadVibe();
                newBv.setup(spawn.getSpawnCords(), s);
                spawn.addBadVibe(newBv);
                /*}
                else
               {
                    Projectile_BV newBv = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawners[bv.SpawnerIndex].getSpawnCords(), bv.SpawnerIndex);
                    spawners[bv.SpawnerIndex].addBadVibe(newBv);
               }*/
                bvcount++;
            }
            addToPool(bv);
        }

    }
}
