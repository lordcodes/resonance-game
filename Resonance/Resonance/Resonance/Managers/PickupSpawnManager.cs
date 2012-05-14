using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Resonance
{
    class PickupSpawnManager
    {
        private static int MIN_PICKUPS = 10; //minimum number of pickups in the world
        private static int MIN_PICKUP_TIME_LIVE = (int)(5 * ResonanceGame.FPS); //length of time the pickup is on the world
        private static int MAX_PICKUP_TIME_LIVE = (int)(15 * ResonanceGame.FPS);
        private static int MIN_PICKUP_TIME_EFFECT = (int)(5 * ResonanceGame.FPS); //length of time the pickup has an effect
        private static int MAX_PICKUP_TIME_EFFECT = (int)(10 * ResonanceGame.FPS);
        private static int DISTANCE_FROM_PLAYER = 100;

        private static int numPickups;
        private static int totalNumPickups;

        private static Random r;
        private static int[] types;
        private static List<Pickup> pickupPool;

        /// <summary>
        /// Creates a new PickupSpawnManager
        /// </summary>
        public static void init() 
        {
            numPickups = 0;
            totalNumPickups = 0;
            types = new int[4];
            types[0] = GameModels.X2;
            types[1] = GameModels.X3;
            types[2] = GameModels.PLUS4;
            types[3] = GameModels.PLUS5;
            r = new Random((int)DateTime.Now.Ticks);
        }

        public static void allocate()
        {
            pickupPool = new List<Pickup>(40);
            for (int i = 0; i < 40; i++)
            {
                //pickupPool.Add(new Pickup(GameModels.PICKUP, "Pickup" + i, Vector3.Zero, 0, 0));
            }
        }

        private static Pickup getPickup(int modelNum, Vector3 pos, int length, int time)
        {
            Pickup p = pickupPool[pickupPool.Count - 1];
            pickupPool.RemoveAt(pickupPool.Count - 1);
            p.init(modelNum, pos, length, time);
            return p;
        }

        public static void addToPool(Pickup p)
        {
            pickupPool.Add(p);
        }

        /// <summary>
        /// Decreases the number of pickups in the world
        /// </summary>
        public static void pickupPickedUp()
        {
            numPickups--;
        }

        /// <summary>
        /// Spawns new pickups if there are less than MIN_PICKUPS in the world
        /// </summary>
        public static void update()
        {
            //DebugDisplay.update("num pickups", numPickups.ToString());
            //DebugDisplay.update("true num pickups", ScreenManager.game.World.returnObjectSubset<Pickup>().Count.ToString());
            //DebugDisplay.update("total pickups", totalNumPickups.ToString());
            if (numPickups < MIN_PICKUPS)
            {
                Vector3 gvPos = GameScreen.getGV().Body.Position;

                int minX = (int)gvPos.X - DISTANCE_FROM_PLAYER;
                if (minX < (int)-World.MAP_X) minX = (int)-World.PLAYABLE_MAP_X - 15;

                int maxX = (int)gvPos.X + DISTANCE_FROM_PLAYER;
                if (maxX > (int)World.MAP_X) maxX = (int)World.PLAYABLE_MAP_X - 15;

                int minZ = (int)gvPos.Z - DISTANCE_FROM_PLAYER;
                if (minZ < (int)-World.MAP_Z) minZ = (int)-World.PLAYABLE_MAP_Z - 15;

                int maxZ = (int)gvPos.Z + DISTANCE_FROM_PLAYER;
                if (maxZ > (int)World.MAP_Z) maxZ = (int)World.PLAYABLE_MAP_Z - 15;

                int x = r.Next(minX, maxX);
                int z = r.Next(minZ, maxZ);

                Vector3 pos = new Vector3((float)x, 5f, (float)z);

                if (!ScreenManager.game.World.querySpace(pos))
                {
                    int model;                    
                    if (ObjectiveManager.currentObjective() == ObjectiveManager.KILL_BOSS)
                    {
                        model = Pickup.DEFLECT;
                    }
                    else
                    {
                        int rand = r.Next(0, 16) % 4;
                        model = types[rand];
                    }

                    //Pickup p = getPickup(model, pos, r.Next(MIN_PICKUP_TIME_LIVE, MAX_PICKUP_TIME_LIVE), r.Next(MIN_PICKUP_TIME_EFFECT, MAX_PICKUP_TIME_EFFECT));
                    Pickup p = new Pickup(model, "Pickup" + totalNumPickups, pos, r.Next(MIN_PICKUP_TIME_LIVE, MAX_PICKUP_TIME_LIVE), r.Next(MIN_PICKUP_TIME_EFFECT, MAX_PICKUP_TIME_EFFECT));
                    ScreenManager.game.World.addObject(p);
                    p.calculateSize();

                    totalNumPickups++;
                    numPickups++;
                }
            }
        }
    }
}
