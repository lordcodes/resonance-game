using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PickupManager
    {
        //public static int currentMultipler = 1;
        public static int pickupType = -1;
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
        /// <param name="pickupType">Multiplier amount</param>
        /// <param name="time">How long this multiplier will last</param>
        public static void newMultiplier(int ppickupType, int time)
        {
            //currentMultipler = pickupType;
            pickupType = ppickupType;
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
            //DebugDisplay.update("multiplier time left", timeRemaining.ToString());
            //DebugDisplay.update("multiplierindex", pickupType.ToString());
            if (timeRemaining > 0)
            {
                timeRemaining--;
            }
            else
            {
                pickupType = -1;
                //currentMultipler = 1;
            }
        }
        
        /// <summary>
        /// Returns a list of Pickup objects
        /// </summary>
        /// <returns>The list of Pickup Objects</returns>
        public static List<Pickup> returnPickups()
        {
            List<Pickup> pickups = new List<Pickup>();

            foreach (KeyValuePair<string, Object> kVP in ScreenManager.game.World.returnObjects())
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
                double diff = Vector3.Distance(GameScreen.getGV().Body.Position, pickupPoint);

                if (diff < pickups[i].Size+3) //TODO: fix with correct GV physics model
                {
                    
                    //new Explosion(pickups[i].Body.Position);

                    switch (pickups[i].PowerUpType)
                    {
                        case Pickup.X3:
                            {
                                MusicHandler.playSound(MusicHandler.DING);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().adjustNitro(10);
                                break;
                            }
                        case Pickup.PLUS4:
                            {
                                MusicHandler.playSound(MusicHandler.DING);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().adjustShield(5);
                                break;
                            }
                        case Pickup.X2:
                            {
                                MusicHandler.playSound(MusicHandler.DING);
                                PickupManager.newMultiplier(pickups[i].PowerUpType, pickups[i].PowerupLength);
                                //Game.getGV().AdjustHealth(5);
                                break;
                            }
                        case Pickup.PLUS5:
                            {
                                MusicHandler.playSound(MusicHandler.DING);
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
                    GameScreen.stats.gotPowerup();
                    //Drawing.addWave(pickupPoint);
                    //ScreenManager.game.World.removeObject(pickups[i]);
                    ScreenManager.game.World.fadeObjectAway(pickups[i], 0.6f);
                    ScreenManager.game.pickupSpawner.pickupPickedUp();
                    continue;
                }

                pickups[i].TimeToLive--;
                if (pickups[i].TimeToLive == 0)
                {
                    //ScreenManager.game.World.removeObject(pickups[i]);
                    ScreenManager.game.World.fadeObjectAway(pickups[i], 0.6f);
                    ScreenManager.game.pickupSpawner.pickupPickedUp();
                }
            }
        }
    }
}
