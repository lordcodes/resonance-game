using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PickupManager
    {
        public static int currentMultipler = 1;
        private static int timeRemaining = 0;
        //List<int> multiplierValues;

        //public PickupManager()
        //{
        //    currentMultipler = 1;
        //    timeRemaining = 0;
        //    //multiplierValues = new List<int>();
        //}

        /// <summary>
        /// Adds the new multiplier values when it is hit
        /// </summary>
        /// <param name="multipler">Multiplier amount</param>
        /// <param name="time">How long this multiplier will last</param>
        public static void newMultiplier(int multipler, int time)
        {
            currentMultipler = multipler;
            timeRemaining = time;
        }

        /// <summary>
        /// Updates pickups each game loop
        /// </summary>
        public static void update()
        {
            youSpinMeRightRoundBabyRightRoundLikeAPickupBabyRightRoundRoundRound(returnPickups());
            pickupCollision(returnPickups());
            updateTimeRemaining();
        }

        /// <summary>
        /// Update the pickup multiplier
        /// </summary>
        private static void updateTimeRemaining()
        {
            DebugDisplay.update("multiplier time left", timeRemaining.ToString());
            DebugDisplay.update("multiplier", currentMultipler.ToString());
            if (timeRemaining > 0)
            {
                timeRemaining--;
            }
            else
            {
                currentMultipler = 1;
            }
        }
        
        /// <summary>
        /// Returns a list of Pickup objects
        /// </summary>
        /// <returns>The list of Pickup Objects</returns>
        public static List<Pickup> returnPickups()
        {
            List<Pickup> pickups = new List<Pickup>();

            foreach (KeyValuePair<string, Object> kVP in Program.game.World.returnObjects())
            {
                Object obj = kVP.Value;

                if (obj is Pickup)
                {
                    pickups.Add((Pickup)obj);
                }
            }

            return pickups;
        }

        private static void youSpinMeRightRoundBabyRightRoundLikeAPickupBabyRightRoundRoundRound(List<Pickup> pickups)
        {
            for (int i = 0; i < pickups.Count; i++)
            {
                pickups[i].spinMe();
            }
        }

        /// <summary>
        /// Checks if Pickups intersect with GoodVibe and remove pickups with TimeToLive = 0
        /// </summary>
        /// <param name="pickups">List of Pickup objects</param>
        private static void pickupCollision(List<Pickup> pickups)
        {
            //pickUpCollision(Game.getGV().Body.Position, Game.getGV().Body.OrientationMatrix.Forward, 3f);

            for (int i = 0; i < pickups.Count; i++)
            {
                Vector3 pickupPoint = pickups[i].Body.Position;
                double diff = Vector3.Distance(Game.getGV().Body.Position, pickupPoint);

                if (diff < pickups[i].Size+3) //TODO: fix with correct GV physics model
                {
                    switch (pickups[i].PowerUpType)
                    {
                        case Pickup.NITROUS:
                            {
                                Program.game.Music.playSound(MusicHandler.CHINK);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().adjustNitro(10);
                                break;
                            }
                        case Pickup.SHIELD:
                            {
                                Program.game.Music.playSound(MusicHandler.DING);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().adjustShield(5);
                                break;
                            }
                        case Pickup.HEALTH:
                            {
                                Program.game.Music.playSound(MusicHandler.SHIMMER);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().AdjustHealth(5);
                                break;
                            }
                        case Pickup.FREEZE:
                            {
                                Program.game.Music.playSound(MusicHandler.RED);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().adjustFreeze(5);
                                break;
                            }
                        default:
                            {
                                //Program.game.Music.playSound("lobster");
                                break;
                            }
                    }

                    //Drawing.addWave(pickupPoint);
                    Program.game.World.removeObject(pickups[i]);
                    Program.game.pickupSpawner.pickupPickedUp();
                    continue;
                }

                pickups[i].TimeToLive--;
                if (pickups[i].TimeToLive == 0)
                {
                    Program.game.World.removeObject(pickups[i]);
                    Program.game.pickupSpawner.pickupPickedUp();
                }
            }
        }
    }
}
