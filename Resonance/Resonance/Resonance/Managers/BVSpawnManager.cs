using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
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
            int i = 0;
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
            Random random = new Random();
            while (i < spawn.getTotalAllowedActive())
            {
               /* int rand = random.Next(234);
                if (rand % 2 == 0)
                {*/
                    BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                    spawn.addBadVibe(bv);
               /* }
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
           
            spawners[bv.SpawnerIndex].removeBadVibe(bv);
            /*if ((spawners[bv.SpawnerIndex].getTotalCurrentlyActive() < spawners[bv.SpawnerIndex].getTotalAllowedActive())
                    && (spawners[bv.SpawnerIndex].getTotalSpawned() <= spawners[bv.SpawnerIndex].getTotalBadVibes()))
            {
               int rand = random.Next(234);
               if (rand % 2 == 0)
                {*/
                    BadVibe newBv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawners[bv.SpawnerIndex].getSpawnCords(), bv.SpawnerIndex);
                    spawners[bv.SpawnerIndex].addBadVibe(newBv);
                /*}
                else
               {
                    Projectile_BV newBv = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawners[bv.SpawnerIndex].getSpawnCords(), bv.SpawnerIndex);
                    spawners[bv.SpawnerIndex].addBadVibe(newBv);
               }*/
                
               
                bvcount++;
            //}
        }

        public static void update()
        {
            int i = 0;
            Random random = new Random();
            while (i < spawners.Count)
            {
                if ((spawners[i].getTotalCurrentlyActive() < spawners[i].getTotalAllowedActive())
                    && (spawners[i].getTotalSpawned() <= spawners[i].getTotalBadVibes()))
                {

                    int rand = random.Next(234);
                    if (rand % 2 == 0)
                    {
                        BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawners[i].getSpawnCords(), i);
                        spawners[i].addBadVibe(bv);
                    }
                    else
                    {
                        Projectile_BV newBv = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawners[i].getSpawnCords(), i);
                        spawners[i].addBadVibe(newBv);
                    }                    
                    bvcount++;
                }
                i++;
            }
        }
    }
}
