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
        public static int MAX_NITRO = 200;
        public static int MAX_SHIELD = 100;
        public static int MAX_FREEZE = 200;

        List<Shockwave> waves; // Resonance waves which currently exist

        int score; //score value
        int health; //health stored as an int between 0 - MAX_HEALTH.
        int nitro; //speed boost between 0 - MAX_NITRO.
        int shield; //shield between 0 - MAX_SHIELD.
        int freeze; //SOMETHINGELSE between 0 - 100.
        private int currentPower = 0;

        private bool isInCombat;

        public int selectedPower
        {
            get
            {
                return currentPower;
            }
            set
            {
                currentPower = value;
            }
        }

        public bool InCombat
        {
            get
            {
                return isInCombat;
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public int Nitro
        {
            get
            {
                return nitro;
            }
        }

        public int Shield
        {
            get
            {
                return shield;
            }
        }

        public int Freeze
        {
            get
            {
                return freeze;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GoodVibe(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            waves = new List<Shockwave>();
            score = 0;
            health = MAX_HEALTH;
            nitro = 200;
            shield = 100;
            freeze = 200;
        }

        /// <summary>
        /// Reset GV to full health
        /// </summary>
        public void fullHealth()
        {
            health = MAX_HEALTH;
        }

        /// <summary>
        /// Adjust the GV's health
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void AdjustHealth(int change)
        {
            //check for <0 or >MAX_HEALTH
            if (health + change <= 0)
            {
                health = 0;
            }
            else if (health + change >= MAX_HEALTH)
            {
                health = MAX_HEALTH;
            }
            else
            {
                health += change;
            }
        }

        /// <summary>
        /// Adjust nitro
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustNitro(int change)
        {
            if (nitro + change <= 0)
            {
                nitro = 0;
            }
            else if (nitro + change >= MAX_NITRO)
            {
                nitro = MAX_NITRO;
            }
            else
            {
                nitro += change;
            }
        }

        /// <summary>
        /// Adjust shield
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustShield(int change)
        {
            if (shield + change <= 0)
            {
                shield = 0;
            }
            else if (shield + change >= MAX_SHIELD)
            {
                shield = MAX_SHIELD;
            }
            else
            {
                shield += change;
            }
        }

        /// <summary>
        /// Adjust freeze
        /// </summary>
        /// <param name="change">Amount to change by</param>
        public void adjustFreeze(int change)
        {
            if (freeze + change <= 0)
            {
                freeze = 0;
            }
            else if (freeze + change >= MAX_FREEZE)
            {
                freeze = MAX_FREEZE;
            }
            else
            {
                freeze += change;
            }
        }

        /// <summary>
        /// Create a shockwave from the GV
        /// </summary>
        /// <param name="colour">Colour of the shockwave</param>
        public void createShockwave(int colour)
        {
            string waveName = "";
            Shockwave w = new Shockwave(GameModels.SHOCKWAVE_GREEN, waveName, this.Body.Position, this.Body.WorldTransform, colour);
            switch (colour)
            {
                case Shockwave.GREEN:
                    {
                        waveName = "GREEN";
                        w = new Shockwave(GameModels.SHOCKWAVE_GREEN, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.YELLOW:
                    {
                        waveName = "YELLOW";
                        w = new Shockwave(GameModels.SHOCKWAVE_YELLOW, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.BLUE:
                    {
                        waveName = "BLUE";
                        w = new Shockwave(GameModels.SHOCKWAVE_BLUE, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.RED:
                    {
                        waveName = "RED";
                        w = new Shockwave(GameModels.SHOCKWAVE_RED, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.CYMBAL:
                    {
                        waveName = "CYMBAL";
                        w = new Shockwave(GameModels.SHOCKWAVE_CYMBAL, waveName, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
            }
            waves.Add(w);
            Program.game.Components.Add(w);
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
                    Program.game.Components.Remove(waves[i]);
                    waves.RemoveAt(i);
                }

                if (i + 1 == waves.Count) break;
            }
        }

        /// <summary>
        /// Detect if the GV is in combat, and resets BVs to unfrozen when out of range
        /// </summary>
        public void detectCombatAndFreeze()
        {
            isInCombat = false;
            foreach (BadVibe bv in Program.game.World.returnBadVibes())
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
                    bv.Status = BadVibe.State.NORMAL;
                }
            }
        }

        /// <summary>
        /// When freeze is used, check for bad vibes in range and freeze them
        /// </summary>
        public void freezeBadVibes()
        {
            isInCombat = false;
            foreach (BadVibe bv in Program.game.World.returnBadVibes())
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
        }

        /// <summary>
        /// Put the GV shield up
        /// </summary>
        public void shieldUp()
        {
            this.gameModelNum = GameModels.SHIELD_GV;
        }

        /// <summary>
        /// Put GV shield down
        /// </summary>
        public void shieldDown()
        {
            this.gameModelNum = GameModels.GOOD_VIBE;
        }


    }
}
