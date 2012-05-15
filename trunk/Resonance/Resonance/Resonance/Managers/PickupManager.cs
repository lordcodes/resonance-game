using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Resonance
{
    class PickupManager
    {
        //public static int currentMultipler = 1;
        private static int pickupType;
        private static int timeRemaining;

        public static void init()
        {
             pickupType = -1;
            timeRemaining = 0;
        }

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
            List<Object> pickups = ScreenManager.game.World.returnObjectSubset<Pickup>();
            pickupCollision(pickups);
            youSpinMeRightRoundBabyRightRoundLikeAPickupBabyRightRoundRoundRound(pickups);
            updateTimeRemaining();
        }

        /// <summary>
        /// Update the pickup multiplier
        /// </summary>
        private static void updateTimeRemaining()
        {
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

        private static void youSpinMeRightRoundBabyRightRoundLikeAPickupBabyRightRoundRoundRound(List<Object> pickups)
        {
            for (int i = 0; i < pickups.Count; i++)
            {
                ((Pickup)pickups[i]).spinMe();
            }
        }

        /// <summary>
        /// Checks if Pickups intersect with GoodVibe and remove pickups with TimeToLive = 0
        /// </summary>
        /// <param name="pickups">List of Pickup objects</param>
        private static void pickupCollision(List<Object> pickups)
        {
            for (int i = 0; i < pickups.Count; i++)
            {
                Pickup p = (Pickup)pickups[i];
                Vector3 pickupPoint = p.Body.Position;
                double diff = Vector3.Distance(GameScreen.getGV().Body.Position, pickupPoint);

                if (diff < p.Size+3) //TODO: fix with correct GV physics model
                {
                    MusicHandler.playSound(MusicHandler.DING);
                    if (ObjectiveManager.currentObjective() == ObjectiveManager.KILL_BOSS)
                    {
                        GameScreen.getGV().adjustDeflectShield(10);
                    }
                    else
                    {
                        newMultiplier(p.PowerUpType, p.PowerupLength);
                    }

                    //PickupSpawnManager.addToPool(p);
                    ScreenManager.game.World.fadeObjectAway(p, 0.6f);
                    //ScreenManager.game.World.removeObject(p);
                    PickupSpawnManager.pickupPickedUp();
                    continue;
                }

                p.TimeToLive--;
                if (p.TimeToLive == 0)
                {
                    //PickupSpawnManager.addToPool(p);
                    ScreenManager.game.World.fadeObjectAway(p, 0.6f);
                    //ScreenManager.game.World.removeObject(p);
                    PickupSpawnManager.pickupPickedUp();
                }
            }
        }

        public static int PickupType
        {
            get
            {
                return pickupType;
            }
        }

        public static int TimeRemaining
        {
            get
            {
                return timeRemaining;
            }
        }
    }
}
