using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Resonance
{
    class PickupManager
    {
        //public static int currentMultipler = 1;
        private static int pickupType = -1;
        private static int timeRemaining = 0;

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

        private static void newDeflectShield()
        {
            GameScreen.getGV().adjustDeflectShield(1);
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
                        newDeflectShield();
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
