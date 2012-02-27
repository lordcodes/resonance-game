using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.MathExtensions;

namespace Resonance
{
    class GoodVibe : DynamicObject
    {
        public static readonly int NITROUS = 0;
        public static readonly int SHIELD = 1;
        public static readonly int FREEZE = 2;

        public static int MAX_HEALTH = 100;
        public static int MAX_NITRO  = 200;
        public static int MAX_SHIELD = 100;
        public static int MAX_FREEZE = 200;

        private GameModelInstance sheildUpModel;
        private GameModelInstance sheildDownModel;

        List<Shockwave> waves; // Resonance waves which currently exist

        //private int currentScore; //score for this sequence
        private int health; //health stored as an int between 0 - MAX_HEALTH.
        private int nitro; //speed boost between 0 - MAX_NITRO.
        private int shield; //shield between 0 - MAX_SHIELD.
        private int freeze; //SOMETHINGELSE between 0 - 100.
        private int currentPower = 0;
        private Boolean shieldOn = false;

        private bool isInCombat;
        private bool freezeActive;

        /// <summary>
        /// Constructor
        /// </summary>
        public GoodVibe(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            waves = new List<Shockwave>();
            //currentScore = 0;
            health = MAX_HEALTH;
            nitro = MAX_NITRO / 2;
            shield = MAX_SHIELD / 2;
            freeze = MAX_FREEZE / 2;

            sheildUpModel = new GameModelInstance(GameModels.SHIELD_GV);
            sheildDownModel = new GameModelInstance(GameModels.GOOD_VIBE);
        }

        /// <summary>
        /// Adjusts the GVs score according to the multiplier
        /// </summary>
        /// <param name="change">Amount to adjust the score</param>
        public void adjustScore(int change)
        {
            if (PickupManager.pickupType == -1)
            {
                GameScreen.stats.addScore(change);
            }
            else if (PickupManager.pickupType == Pickup.X2)
            {
                GameScreen.stats.addScore(change * 2);
            }
            else if (PickupManager.pickupType == Pickup.X3)
            {
                GameScreen.stats.addScore(change * 3);
            }
            else if (PickupManager.pickupType == Pickup.PLUS4)
            {
                GameScreen.stats.addScore(change + 4);
            }
            else if (PickupManager.pickupType == Pickup.PLUS5)
            {
                GameScreen.stats.addScore(change + 5);
            }
            //GameScreen.stats.addScore(change * PickupManager.currentMultipler);
        }

        /// <summary>
        /// Reset GV to full health
        /// </summary>
        public void fullHealth()
        {
            health = MAX_HEALTH;
        }

        public float healthFraction()
        {
            return (float)health / (float)MAX_HEALTH;
        }

        /// <summary>
        /// Adjust the GV's health  between 0 - MAX_HEALTH
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void AdjustHealth(int change)
        {
            if(shieldOn == false)
            health = (int) MathHelper.Clamp(health += change, 0, MAX_HEALTH);        

            if (health <= 0) GameScreen.GV_KILLED = true;
        }

        /// <summary>
        /// Adjust nitro between 0 - MAX_NITRO
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustNitro(int change)
        {
            nitro = (int)MathHelper.Clamp(nitro += change, 0, MAX_NITRO);
        }

        /// <summary>
        /// Adjust shield between 0 - MAX_SHIELD
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustShield(int change)
        {
            shield = (int)MathHelper.Clamp(shield += change, 0, MAX_SHIELD);
        }

        /// <summary>
        /// Adjust freeze between 0 - MAX_FREEZE
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustFreeze(int change)
        {
            freeze = (int)MathHelper.Clamp(freeze += change, 0, MAX_FREEZE);
        }

        /// <summary>
        /// Create a shockwave from the GV
        /// </summary>
        /// <param name="colour">Colour of the shockwave</param>
        public void createShockwave(int colour)
        {
            string waveName = "";
            Shockwave w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
            switch (colour)
            {
                case Shockwave.GREEN:
                    {
                        waveName = "GREEN";
                        w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        w.ModelInstance.setTexture(0);
                        break;
                    }
                case Shockwave.YELLOW:
                    {
                        waveName = "YELLOW";
                        w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        w.ModelInstance.setTexture(1);
                        break;
                    }
                case Shockwave.BLUE:
                    {
                        waveName = "BLUE";
                        w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        w.ModelInstance.setTexture(2);
                        break;
                    }
                case Shockwave.RED:
                    {
                        waveName = "RED";
                        w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        w.ModelInstance.setTexture(3);
                        break;
                    }
                case Shockwave.CYMBAL:
                    {
                        waveName = "CYMBAL";
                        w = new Shockwave(GameModels.SHOCKWAVE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        w.ModelInstance.setTexture(4);
                        break;
                    }
            }
            waves.Add(w);
            //Program.game.Components.Add(w);
            DrawableManager.Add(w);
        }

        /// <summary>
        /// Update the shockwaves
        /// </summary>
        public void updateWaves()
        {
            foreach (Shockwave w in waves)
            {
                w.grow();
                w.checkBadVibes();
            }
            removeWaves();
        }

        /// <summary>
        /// Remove a shockwave
        /// </summary>
        private void removeWaves()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].Radius >= Shockwave.MAX_RADIUS)
                {
                    //Program.game.Components.Remove(waves[i]);
                    DrawableManager.Remove(waves[i]);
                    waves.RemoveAt(i);
                }

                if (i + 1 == waves.Count) break;
            }
        }
       
        public void freezeHealth(Boolean flag)
        {
            shieldOn = flag;
        }
        /// <summary>
        /// Detect if the GV is in combat, and resets BVs to unfrozen when out of range
        /// </summary>
        /*public void detectCombatAndFreeze()
        {
            isInCombat = false;
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();

            foreach (BadVibe bv in bvs)
            {
                double dx = this.Body.Position.X - bv.Body.Position.X;
                double dz = this.Body.Position.Z - bv.Body.Position.Z;
                double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                d = Math.Sqrt(d);

                if (d <= Shockwave.MAX_RADIUS)
                {
                    isInCombat = true;
                }
                else
                {
                    if(bv.Status == BadVibe.State.FROZEN) bv.Status = BadVibe.State.NORMAL;
                }
            }
        }*/

        /// <summary>
        /// When freeze is used, check for bad vibes in range and freeze them
        /// </summary>
        /*public void freezeBadVibes()
        {
            isInCombat = false;
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();

            foreach (BadVibe bv in bvs)
            {
                double dx = this.Body.Position.X - bv.Body.Position.X;
                double dz = this.Body.Position.Z - bv.Body.Position.Z;
                double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                d = Math.Sqrt(d);

                if (d <= Shockwave.MAX_RADIUS)
                {
                    bv.Status = BadVibe.State.FROZEN;
                }
            }
            GameScreen.stats.usedFreeze();
        }*/

        /// <summary>
        /// Put the GV shield up
        /// </summary>
        public void shieldUp()
        {
            this.ModelInstance = sheildUpModel;
            GameScreen.stats.usedShield();
        }

        /// <summary>
        /// Put GV shield down
        /// </summary>
        public void shieldDown()
        {
            this.ModelInstance = sheildDownModel;
        }

        /// <summary>
        /// Makes changes to the graphics of the good vibe to show beat time.
        /// </summary>
        public void showBeat()
        {
            //Un comment these lines when beat animation of the Good Vibe texture is ready
            //this.ModelInstance.setTexture(1);
            //this.ModelInstance.playTextureAnimOnce();
        }

        public int selectedPower
        {
            get { return currentPower; }
            set { currentPower = value; }
        }

        public bool InCombat
        {
            get { return isInCombat; }
            set { isInCombat = value; }
        }

        public int Health
        {
            get { return health; }
        }

        public int Nitro
        {
            get { return nitro; }
        }

        public int Shield
        {
            get { return shield; }
        }

        public int Freeze
        {
            get { return freeze; }
        }

        public bool FreezeActive
        {
            get { return freezeActive; }
            set { freezeActive = value; }
        }
    }
}
