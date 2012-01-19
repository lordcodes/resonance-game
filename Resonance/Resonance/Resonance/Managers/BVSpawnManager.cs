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

        public BVSpawnManager() 
        {
            numBVs = 0;
            totalNumBVs = 0;
        }

        public void vibeDied()
        {
            numBVs--;
        }

        public void update()
        {
            if (numBVs < MIN_BVS)
            {
                bool placed = false;

                while (!placed)
                {
                    Random r = new Random((int)DateTime.Now.Ticks);
                    int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
                    int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);

                    Vector3 pos = new Vector3((float)x, 0.5f, (float)z);

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
        }
    }
}
