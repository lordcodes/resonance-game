using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace Resonance {
    class ObjectiveManager {

        public const int KILL_ALL_BV            = 0;
        public const int COLLECT_ALL_PICKUPS    = 1;
        public const int SURVIVE                = 2;
        public const int TERRITORIES            = 3;
        public const int KILL_BOSS              = 4;

        private const int DEFAULT_OBJECTIVE     = KILL_ALL_BV;

        public  const int FINAL_OBJECTIVE       = KILL_BOSS;

        private static int cObj                 = DEFAULT_OBJECTIVE;

        private static int bvKilledAtStart      = 0;
        private static TimeSpan initialDistThroughSong;
        private static TimeSpan survivalTime = new TimeSpan(0, 2, 0); // 2 mins

        public static void loadObjectivesGame(ScreenManager sm) {
            ScreenManager.game = new GameScreen(sm, 0);
            LoadingScreen.LoadAScreen(sm, true, ScreenManager.game);
        }

        public static void setObjective(int newObj) {
            cObj = newObj;

            if (newObj == KILL_ALL_BV) bvKilledAtStart = GameScreen.stats.BVsKilled;
            if (newObj == SURVIVE) initialDistThroughSong = MediaPlayer.PlayPosition;
        }

        public static void nextObjective() {
            cObj++;
        }

        public static int currentObjective() {
            return cObj;
        }

        public static String getObjectiveString() {
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

        public static bool getProgress(ref string oStr) {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    int killed = GameScreen.stats.BVsKilled - bvKilledAtStart;
                    int total = BVSpawnManager.MAX_BV * BVSpawnManager.getSpawnerCount();
                    oStr = "" + killed + " / " + total + " destroyed";

                    if (killed == total) return true; else return false;
                    //if (killed >= 1) return true; else return false;
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
                    TimeSpan ts = survivalTime - MediaPlayer.PlayPosition - initialDistThroughSong;

                    int remainingM = ts.Minutes;
                    int remainingS = ts.Seconds;
                    if (remainingS < 0) remainingS = 0;

                    string formattedTimeSpan = string.Format("{0:D2}:{1:D2}", remainingM, remainingS);

                    oStr = "Stay alive for " + formattedTimeSpan;

                    if ((ts.Minutes == 0) && (ts.Seconds <= 0)) return true; else return false;
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
