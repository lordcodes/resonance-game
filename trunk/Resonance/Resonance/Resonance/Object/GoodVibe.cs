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
        Game gameRef;

        int health; //health stored as an int between 0 - 100.
        int score;

        // Resonance waves which currently exist
        List<Shockwave> waves;

        public int WaveCount {
            get
            {
                return waves.Count;
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

        /// <summary>
        /// Constructor
        /// Set initial health to 100
        /// </summary>
        public GoodVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            gameRef = game;
            waves = new List<Shockwave>();
            score = 0;
            health = 100;
        }
               
        void AdjustHealth(int change)
        {
            //check for <0 or >100
            //health <0, dead vibe
            if (health - change <= 0)
            {
                health = 0;
                Console.WriteLine("Dead vibe!");
            }
            //full health
            else if (health + change >= 100)
            {
                health = 100;
                Console.WriteLine("Full health");
            }
            else
            {
                health += change;
            }
        }

        void SetHealth(int value)
        {
            //check between 0-100
            health = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHealth()
        {
            return health;
        }

        public void createShockwave(int colour)
        {
            string waveName = "";
            Shockwave w = new Shockwave(GameModels.SHOCKWAVE_GREEN, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
            switch(colour) 
            {
                case Shockwave.GREEN:
                    {
                        waveName = "GREEN";
                        w = new Shockwave(GameModels.SHOCKWAVE_GREEN, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.YELLOW:
                    {
                        waveName = "YELLOW";
                        w = new Shockwave(GameModels.SHOCKWAVE_YELLOW, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.BLUE:
                    {
                        waveName = "BLUE";
                        w = new Shockwave(GameModels.SHOCKWAVE_BLUE, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.RED:
                    {
                        waveName = "RED";
                        w = new Shockwave(GameModels.SHOCKWAVE_RED, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
                case Shockwave.CYMBAL:
                    {
                        waveName = "CYMBAL";
                        w = new Shockwave(GameModels.SHOCKWAVE_CYMBAL, waveName, gameRef, this.Body.Position, this.Body.WorldTransform, colour);
                        break;
                    }
            }
            waves.Add(w);
            gameRef.Components.Add(w);
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
                    gameRef.Components.Remove(waves[i]);
                    waves.RemoveAt(i);
                }

                if (i + 1 == waves.Count) break;
            }
        }
    }
}
