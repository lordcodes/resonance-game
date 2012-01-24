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
        List<Shockwave> waves; // Resonance waves which currently exist
        private bool isInCombat;

        int score;
        int health; //health stored as an int between 0 - MAX_HEALTH.
        public static int MAX_HEALTH = 100;
        int nitro; //speed boost between 0 - MAX_NITRO.
        public static int MAX_NITRO = 200;
        int shield; //SOMETHING between 0 - MAX_SHIELD.
        public static int MAX_SHIELD = 100;
        int freeze; //SOMETHINGELSE between 0 - 100.
        public static int MAX_FREEZE = 100;

        public static int curentPower = 0;

        public static readonly int NITROUS = 0;
        public static readonly int SHIELD = 1;
        public static readonly int FREEZE = 2;

        public bool InCombat
        {
            get
            {
                return isInCombat;
            }
        }
        public int WaveCount
        {
            get
            {
                return waves.Count;
            }
        }

        public int Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
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

            set
            {
                nitro = value;
            }
        }

        public int Shield
        {
            get
            {
                return shield;
            }

            set
            {
                shield = value;
            }
        }

        public int Freeze
        {
            get
            {
                return freeze;
            }

            set
            {
                freeze = value;
            }
        }

        /// <summary>
        /// Constructor
        /// Set initial health to MAX_HEALTH
        /// </summary>
        public GoodVibe(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            waves = new List<Shockwave>();
            score = 0;
            health = MAX_HEALTH;
            nitro = 0;
            shield = 0;
            freeze = 0;
        }

        public void fullHealth()
        {
            health = MAX_HEALTH;
        }

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

        public void adjustNitro(int change)
        {
            //check for <0 or >MAX_NITRO
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

        public void adjustShield(int change)
        {
            //check for <0 or >MAX_SHIELD
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

        public void adjustFreeze(int change)
        {
            //check for <0 or >MAX_FREEZE
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

        public void createShockwave(int colour)
        {
            string waveName = "";
            Shockwave w = new Shockwave(GameModels.SHOCKWAVE_GREEN, waveName, this.Body.Position, this.Body.WorldTransform, colour);
            switch(colour) 
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

            //Drawing.addWave(this.Body.Position);
        }

        public void updateWaves()
        {
            foreach (Shockwave w in waves)
            {
                w.grow();
                w.checkBadVibes();
            }

            removeWaves();
        }

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

        public void detectCombatAndFreeze()
        {
            isInCombat = false;
            Dictionary<string, Object> objects = Program.game.World.returnObjects();
            foreach (KeyValuePair<string, Object> pair in objects)
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;
                    double dx = this.Body.Position.X - vibe.Body.Position.X;
                    double dz = this.Body.Position.Z - vibe.Body.Position.Z;
                    double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                    d = Math.Sqrt(d);

                    if (d <= Shockwave.MAX_RADIUS)
                    {
                        isInCombat = true;
                    }
                    else
                    {
                        vibe.Freeze = false;
                    }
                }
            }
        }

        /// <summary>
        /// When freeze is used, check for bad vibes in range and freeze them
        /// </summary>
        public void freezeBadVibes()
        {
            isInCombat = false;
            Dictionary<string, Object> objects = Program.game.World.returnObjects();
            foreach (KeyValuePair<string, Object> pair in objects)
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;
                    double dx = this.Body.Position.X - vibe.Body.Position.X;
                    double dz = this.Body.Position.Z - vibe.Body.Position.Z;
                    double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                    d = Math.Sqrt(d);

                    if (d <= Shockwave.MAX_RADIUS)
                    {
                        vibe.Freeze = true;
                    }
                }
            }
        }
    }
}
