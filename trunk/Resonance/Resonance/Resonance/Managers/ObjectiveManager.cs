using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Media;

namespace Resonance {
    class ObjectiveManager {
        public const bool DEBUG_MODE            = true;

        public const int KILL_ALL_BV            = 0;
        public const int COLLECT_ALL_PICKUPS    = 1;
        public const int SURVIVE                = 2;
        public const int TERRITORIES            = 3;
        public const int KILL_BOSS              = 4;

        public const int DEFAULT_OBJECTIVE      = KILL_ALL_BV;

        public  const int FINAL_OBJECTIVE       = KILL_BOSS;

        private static int cObj                 = DEFAULT_OBJECTIVE;

        private static int bvKilledAtStart      = 0;
        private static TimeSpan initialDistThroughSong;
        private static TimeSpan survivalTime = new TimeSpan(0, 2, 0); // 2 mins
        //private static TimeSpan survivalTime = new TimeSpan(0, 0, 10); // 1 sec

        public static void loadObjectivesGame(ScreenManager sm) {
            ScreenManager.game = new GameScreen(sm, cObj);
            LoadingScreen.LoadAScreen(sm, cObj + 2, ScreenManager.game);
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

        /// <summary>
        /// Deprecated. Use getObjectiveStrings().
        /// </summary>
        /// <returns></returns>
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

        public static void getObjectiveStrings(ref string longStr, ref string shortStr) {
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    longStr = "Destroy all the Bad Vibes\nto heal the Cerebellum.";
                    shortStr = "Destroy the Bad Vibes";
                    break;
                }
                case (COLLECT_ALL_PICKUPS) : {
                    longStr = "Collect all the pickups\nto heal the Occipital Lobe.";
                    shortStr = "Collect the Pickups";
                    break;
                }
                case (KILL_BOSS) : {
                    longStr  = "Destroy the Master Bad Vibe to heal the Frontal Lobe\nand wake up from the coma.";
                    shortStr = "Destroy the Boss";
                    break;
                }
                case (SURVIVE) : {
                    longStr = "The Bad Vibes are fighting back.\nSurvive the onslaught to heal the Temporal Lobe.";
                    shortStr = "Stay alive";
                    break;
                }
                case (TERRITORIES) : {
                    longStr  = "Hit the gateways with the same-coloured drum\nto heal the Parietal Lobe.";
                    shortStr = "Neutralise the gateways";
                    break;
                }
            }

            return;
        }

        public static void getLoadingScreenString(ref string obj, ref string con, ref string drum)
        {
            switch (cObj)
            {
                case (KILL_ALL_BV):
                    {
                        obj = "Clear the area of Bad Vibes to heal the Cerebullum.";
                        con = "Move out of the way to avoid damage.";
                        drum = "The Bad Vibes have a set armour sequence, hit the drum\ncorresponding with the bottom of the sequence to destroy the layer.";
                        break;
                    }
                case (COLLECT_ALL_PICKUPS):
                    {
                        obj = "Collect all the orbs to heal the Occipital Lobe.";
                        con = "Manoeuvre the landscape to obtain the orbs.";
                        drum = "Hit the drums to destroy the corresponding coloured\nincoming projectiles to defend from damage.";
                        break;
                    }
                case (KILL_BOSS):
                    {
                        obj = "Defeat the boss to heal the Frontal Lobe and awake from the coma.";
                        con = "Collect pickups to fill the deflection bar.";
                        drum = "Hit the drums to deflect the corresponding coloured incoming projectiles\nto damage the boss. If the bar is depleted the drums will simply absorb to prevent damage.";
                        break;
                    }
                case (SURVIVE):
                    {
                        obj = "Survive for 2 minutes to heal the Temporal Lobe.";
                        con = "Move out of the way to avoid damage.";
                        drum = "No where is safe, help survive by destroying Bad Vibes.";
                        break;
                    }
                case (TERRITORIES):
                    {
                        obj = "Neutralise all the gateways to heal the Parietal Lobe.";
                        con = "You are always moving forwards, steer towards the gates to allow them to be hit.";
                        drum = "Hit the gateways with the same-coloured drum to neutralise them.";
                        break;
                    }
            }

            return;
        }

        static int maxPickups = 0;
        public static bool getProgress(ref string oStr) {
            if (DEBUG_MODE) return true;
            switch (cObj) {
                case (KILL_ALL_BV) : {
                    int killed = GameScreen.stats.BVsKilled - bvKilledAtStart;
                    int total = 0;
                    if (GameScreen.USE_BV_SPAWNER) total = BVSpawnManager.MAX_BV * BVSpawnManager.getSpawnerCount();
                    oStr = "" + killed + " / " + total + " destroyed";

                    //if (killed == total) return true; else return false;
                    if (killed >= 1) return true; else return false;
                }
                case (COLLECT_ALL_PICKUPS) : {
                    List<Object> ps = ScreenManager.game.World.returnObjectSubset<ObjectivePickup>();
                    if (ps.Count > maxPickups) maxPickups = ps.Count;
                    int collected = maxPickups - ps.Count;
                    oStr = "" + collected + " / " + maxPickups + " collected";

                    if (collected == maxPickups) {
                        return true;
                    }
                    else return false;
                }
                case (KILL_BOSS) : {
                    Boss b = GameScreen.getBoss();
                    int bossHealth = b.getHealth();

                    double pct = 100d * (double) bossHealth / (double) Boss.MAX_HEALTH;
                    oStr = "Master Bad Vibe " + ((int) pct) + "% destroyed";

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
                    List<Object> cps =  ScreenManager.game.World.returnObjectSubset<Checkpoint>();

                    int healed = 0;
                    int total = cps.Count;

                    for (int i = 0; i < total; i++) {
                        Checkpoint cp = (Checkpoint) cps.ElementAt(i);
                        if (cp.beenHit()) healed++;
                    }

                    oStr = "" + healed + " / " + total + " areas healed";

                    if (healed >= total) return true; else return false;
                }
            }

            oStr = "";
            return false;
        }
    }
}
