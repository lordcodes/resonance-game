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
                    Random r = new Random((int)DateTime.Now.Ticks);
                    int x = r.Next((int)-World.MAP_X, (int)World.MAP_X);
                    int z = r.Next((int)-World.MAP_Z, (int)World.MAP_Z);

                    Vector3 pos = new Vector3((float)x, 0.5f, (float)z);

                    Vector3 goodVibePosition = Game.getGV().Body.Position;
                    double xDiff = Math.Abs(goodVibePosition.X - pos.X);
                    double yDiff = Math.Abs(goodVibePosition.Y - pos.Y);
                    double zDiff = Math.Abs(goodVibePosition.Z - pos.Z);
                    double distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));

                    if (distance < 100)
                    {
                        if (!Program.game.World.querySpace(pos))
                        {
                            placed = true;

                            Pickup p = new Pickup(GameModels.PICKUP, "Pickup" + totalNumPickups, pos, r.Next(0,16)%4+2, r.Next(MIN_PICKUP_TIME_LIVE, MAX_PICKUP_TIME_LIVE), r.Next(MIN_PICKUP_TIME_EFFECT,MAX_PICKUP_TIME_EFFECT));
                            Program.game.World.addObject(p);

                            totalNumPickups++;
                            numPickups++;
                        }
                    }
                }
            }
        }
    }
}
