using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance {
    class ObjectiveManager {

        public const int KILL_ALL_BV            = 0;
        public const int COLLECT_ALL_PICKUPS    = 1;
        public const int KILL_BOSS              = 2;
        public const int TERRITORIES            = 3;
        public const int SURVIVE                = 4;

        private const int DEFAULT_OBJECTIVE = KILL_ALL_BV;

        private static int cObj = DEFAULT_OBJECTIVE;

        public static void setObjective(int newObj) {
            cObj = newObj;
        }

        public static int currentObjective() {
            return cObj;
        }

        public bool objectiveMet() {
            return false;
        }

        public String getObjectiveString() {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    return "Destroy the Bad Vibes.";
                }
                case (COLLECT_ALL_PICKUPS) : {
                    return "Collect the Pickups";
                }
                case (KILL_BOSS) : {
                    return "Destroy the Master Bad Vibe";
                }
                case (SURVIVE) : {
                    return "Stay alive.";
                }
                case (TERRITORIES) : {
                    return "Heal each brain area by destroying the Bad Vibes therein.";
                }
            }

            return "";
        }

        public String getProgressString() {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    int killed = 0;
                    int total = 0;
                    return killed.ToString() + " / " + total.ToString() + " destroyed.";
                }
                case (COLLECT_ALL_PICKUPS) : {
                    int collected = 0;
                    int total = 0;
                    return collected.ToString() + " / " + total.ToString() + " collected.";
                }
                case (KILL_BOSS) : {
                    int bossHealth = 0;
                    int bossMaxHealth = 0;
                    double pct = 100d * (double) bossHealth / (double) bossMaxHealth;
                    return "Master Bad Vibe " + ((int) pct).ToString() + " destroyed.";
                }
                case (SURVIVE) : {
                    int remainingMins = 0;
                    int remainingSecs = 0;

                    String secString = remainingSecs.ToString();
                    if (secString.Length < 2) secString = "0" + secString;
                    String minString = remainingMins.ToString();

                    return "Stay alive for " + minString + ":" + secString;
                }
                case (TERRITORIES) : {
                    int healed = 0;
                    int total = 0;
                    return healed.ToString() + " / " + total.ToString() + " areas healed.";
                }
            }

            return "";
        }
    }
}
