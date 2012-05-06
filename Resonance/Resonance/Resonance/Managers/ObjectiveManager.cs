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

        private const int DEFAULT_OBJECTIVE     = KILL_ALL_BV;

        private const int FINAL_OBJECTIVE       = KILL_BOSS;

        private static int cObj                 = DEFAULT_OBJECTIVE;

        private static int bvKilledAtStart      = 0;

        public static void setObjective(int newObj) {
            cObj = newObj;

            if (newObj == KILL_ALL_BV) bvKilledAtStart = GameScreen.stats.BVsKilled;
        }

        public static int currentObjective() {
            return cObj;
        }

        public String getObjectiveString() {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    return "Destroy the Bad Vibes";
                }
                case (COLLECT_ALL_PICKUPS) : {
                    return "Collect the Pickups";
                }
                case (KILL_BOSS) : {
                    return "Destroy the Master Bad Vibe";
                }
                case (SURVIVE) : {
                    return "Stay alive";
                }
                case (TERRITORIES) : {
                    return "Heal each brain area by destroying the Bad Vibes therein";
                }
            }

            return "";
        }

        public bool getProgress(ref string oStr) {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    int killed = GameScreen.stats.BVsKilled - bvKilledAtStart;
                    int total = BVSpawnManager.MAX_BV * BVSpawnManager.getSpawnerCount();
                    oStr = "" + killed + " / " + total + " destroyed";

                    if (killed == total) return true; else return false;
                }
                case (COLLECT_ALL_PICKUPS) : {
                    int collected = 0;
                    int total = 0;
                    oStr = "" + collected + " / " + total + " collected";

                    if (collected == total) return true; else return false;
                }
                case (KILL_BOSS) : {
                    int bossHealth = 0;
                    int bossMaxHealth = 0;
                    double pct = 100d * (double) bossHealth / (double) bossMaxHealth;
                    oStr = "Master Bad Vibe " + ((int) pct) + " destroyed";

                    if (bossHealth == 0) return true; else return false;
                }
                case (SURVIVE) : {
                    TimeSpan ts = MusicHandler.getTrack().Song.Duration; //TODO: duration-elapsed
                    string formattedTimeSpan = string.Format("{1:D2}:{2:D2}", ts.Minutes, ts.Seconds);

                    //int remainingMins = 0;
                    //int remainingSecs = 0;

                    //string secString = remainingSecs.ToString();
                    //if (secString.Length < 2) secString = "0" + secString;

                    oStr = "Stay alive for " + formattedTimeSpan;

                    if ((ts.Minutes == 0) && (ts.Seconds == 0)) return true; else return false;
                }
                case (TERRITORIES) : {
                    int healed = 0;
                    int total = 0;
                    oStr = "" + healed + " / " + total + " areas healed";

                    if (healed == total) return true; else return false;
                }
            }

            oStr = "";
            return false;
        }
    }
}
