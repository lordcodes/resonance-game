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

                            Pickup p = new Pickup(GameModels.PICKUP, "Pickup" + totalNumPickups, pos, r.Next(0,16)%4, 1200);
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
