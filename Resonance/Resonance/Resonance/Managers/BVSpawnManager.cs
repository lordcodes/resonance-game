using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
        private const int SPAWN_RADIUS = 10;
        private const int MAX_ACTIVE = 1;
        private const int MAX_BV = 12;

        private static int spawnerCount;
        private static int bvcount = 0;
        private static List<BVSpawner> spawners;

        //Allocated variables
        private static List<BadVibe> bvPool;

        public BVSpawnManager() 
        {
            spawnerCount = 1;
            spawners = new List<BVSpawner>();
        }

        public static void allocate()
        {
            bvPool = new List<BadVibe>(50);
            for (int i = 0; i < 50; i++)
            {
                BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BV" + i, Vector3.Zero, 0);
                bvPool.Add(bv);
            }
        }

        private static BadVibe getBadVibe()
        {
            BadVibe bv = bvPool[bvPool.Count - 1];
            bvPool.RemoveAt(bvPool.Count - 1);
            return bv;
        }

        private static void addToPool(BadVibe bv)
        {
            bvPool.Add(bv);
        }

        public static void addNewSpawner(Vector3 position)
        {
            int i = 0;

            BVSpawner spawn = new BVSpawner(GameModels.BV_SPAWNER, "BVSpawner" + spawnerCount, position, MAX_BV, SPAWN_RADIUS, MAX_ACTIVE);
            ScreenManager.game.World.addObject(spawn);
            spawners.Add(spawn);

            Random random = new Random();
            while (i < spawn.MaxActive)
            {
                /*int rand = random.Next(234);
                if (rand % 2 == 0)
                {*/
                    //BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                BadVibe bv = getBadVibe();
                bv.setup(spawn.getSpawnCords(), spawners.Count - 1);
                spawn.addBV(bv);
                /*}
                else
                {
                    Projectile_BV newProjBV = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                    spawn.addBadVibe( newProjBV);
                }*/
                i++;
            }
            spawnerCount++;
        }

        public static void vibeDied(BadVibe bv)
        {
            Random random = new Random();

            int s = bv.SpawnerIndex;
            BVSpawner spawn = spawners[s];

            spawn.replaceBV(bv, getBadVibe(), s);
            addToPool(bv);
        }

    }
}
