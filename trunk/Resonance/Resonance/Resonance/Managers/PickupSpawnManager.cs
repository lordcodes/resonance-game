using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PickupSpawnManager
    {
        private static int MIN_PICKUPS = 10;
        private static int MIN_PICKUP_TIME_LIVE = 300; //length of time the pickup is on the world
        private static int MAX_PICKUP_TIME_LIVE = 600;
        private static int MIN_PICKUP_TIME_EFFECT = 600; //length of time the pickup has an effect
        private static int MAX_PICKUP_TIME_EFFECT = 1200;
        private static int DISTANCE_FROM_PLAYER = 100;

        private int numPickups;
        private int totalNumPickups;

        public PickupSpawnManager() 
        {
            numPickups = 0;
            totalNumPickups = 0;
        }

        public void pickupPickedUp()
        {
            numPickups--;
        }

        public void update()
        {
            //DebugDisplay.update("num pickups", numPickups.ToString());
            //DebugDisplay.update("total pickups", totalNumPickups.ToString());
            if (numPickups < MIN_PICKUPS)
            {
                bool placed = false;

                while (!placed)
                {
                    Vector3 gvPos = Game.getGV().Body.Position;

                    int minX = (int)gvPos.X - DISTANCE_FROM_PLAYER;
                    if (minX < (int)-World.MAP_X) minX = (int)-World.MAP_X;

                    int maxX = (int)gvPos.X + DISTANCE_FROM_PLAYER;
                    if (maxX > (int)World.MAP_X) maxX = (int)World.MAP_X;

                    int minZ = (int)gvPos.Z - DISTANCE_FROM_PLAYER;
                    if (minZ < (int)-World.MAP_Z) minZ = (int)-World.MAP_Z;

                    int maxZ = (int)gvPos.Z + DISTANCE_FROM_PLAYER;
                    if (maxZ > (int)World.MAP_Z) maxZ = (int)World.MAP_Z;

                    Random r = new Random((int)DateTime.Now.Ticks);
                    //int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
                    //int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);

                    int x = r.Next(minX, maxX);
                    int z = r.Next(minZ, maxZ);

                    Vector3 pos = new Vector3((float)x, 0.5f, (float)z);
                    double distance = Vector3.Distance(gvPos, pos);

                    //if (distance < 100)
                    //{
                        if (!Program.game.World.querySpace(pos))
                        {
                            placed = true;

                            Pickup p = new Pickup(GameModels.PICKUP, "Pickup" + totalNumPickups, pos, r.Next(0,16)%4+2, r.Next(MIN_PICKUP_TIME_LIVE, MAX_PICKUP_TIME_LIVE), r.Next(MIN_PICKUP_TIME_EFFECT,MAX_PICKUP_TIME_EFFECT));
                            Program.game.World.addObject(p);
                            p.calculateSize();
                            //DebugDisplay.update("size", p.Size.ToString());

                            totalNumPickups++;
                            numPickups++;
                        }
                    //}
                }
            }
        }
    }
}
