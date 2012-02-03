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

        private int numBVs;
        private int totalNumBVs;
        private int spawnerCount;
        List<BVSpawner> spawners;

        public BVSpawnManager() 
        {
            spawnerCount = 2;
            spawners = new List<BVSpawner>();
            numBVs = 0;
            totalNumBVs = 0;
            //addNewSpawner(12,12,3);
        }

        public void addNewSpawner(int totalBv, int radious, int totalActive )
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
            int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);
            Vector3 pos = new Vector3((float)x, 0.5f, (float)z);
            BVSpawner spawn = new BVSpawner(GameModels.BV_SPAWNER, "BV_SPAWNER" + spawnerCount, pos, totalActive,radious,totalActive);
            Program.game.World.addObject(spawn);
            //spawners.Add(spawn);
            spawnerCount++;
        }

        public void vibeDied()
        {
            numBVs--;
        }

        public void update()
        {


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
