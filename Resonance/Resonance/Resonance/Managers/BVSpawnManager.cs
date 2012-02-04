using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
        private static int MIN_BVS = 15;

        private static int numBVs = 0;
        private static int totalNumBVs = 0;
        private static int spawnerCount = 2;
        private static int bvcount = 0;
        private static List<BVSpawner> spawners = new List<BVSpawner>();
        private static bool debugSwtich = true;

        public BVSpawnManager() 
        {
            spawnerCount = 2;
            spawners = new List<BVSpawner>();
            numBVs = 0;
            totalNumBVs = 0;
        }

        public static void addNewSpawner(int totalBv, int radious, int totalActive )
        {
            //DebugDisplay.update("Spawner Added","true");
            Random r = new Random((int)DateTime.Now.Ticks);
            Vector3 pos;
            int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
            int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);
            if (debugSwtich)
            {
                pos = new Vector3(5f, 0.5f, 5f);
            }
            else
            {
                pos = new Vector3((float)x, 0.5f, (float)z);
            }
            BVSpawner spawn = new BVSpawner(GameModels.BV_SPAWNER, "BV_SPAWNER" + spawnerCount, pos, totalActive,radious,totalActive);
            Program.game.World.addObject(spawn);
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
            DebugDisplay.update("number of spawn objects", spawners.Count.ToString());
            while (i < spawners.Count)
            {
                DebugDisplay.update("Updating", "true");
                if (spawners[i].getTotalCurrentlyActive() < spawners[i].getTotalAllowedActive())
                {
                    BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, new Vector3(5, 2, 6),i);
                    spawners[i].addBadVibe(bv);
                    bvcount++;
                }
                i++;
            }

           /* if (numBVs < MIN_BVS)
            {
                bool placed = false;

                while (!placed)
                {
                    Random r = new Random((int)DateTime.Now.Ticks);
                    int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
                    int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);

                    Vector3 pos = new Vector3((float)x, 0.5f, (float)z);

                    Vector3 goodVibePosition = Game.getGV().Body.Position;
                    double xDiff = Math.Abs(goodVibePosition.X - pos.X);
                    double yDiff = Math.Abs(goodVibePosition.Y - pos.Y);
                    double zDiff = Math.Abs(goodVibePosition.Z - pos.Z);
                    double distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));

                    if (distance < 25)
                    {
                        if (!Program.game.World.querySpace(pos))
                        {
                            placed = true;
                            string id = "BV" + totalNumBVs;
                            //try
                            //{
                            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, id, pos);
                            Program.game.World.addObject(bv);
                            //}
                            //catch (Exception ex)
                            //{
                            //} 
                            totalNumBVs++;
                            numBVs++;
                        }
                    }
                }
            }*/
        }
    }
}
