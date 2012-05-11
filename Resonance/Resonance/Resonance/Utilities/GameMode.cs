using System.Collections.Generic;

namespace Resonance {

    /// <summary>
    /// Superclass to represent a game mode.
    /// </summary>
    class GameMode {

        public static bool HEALTH_RECHARGE = false;
        public static bool MUSIC_REPEAT = false;

        public int MODE;

        public const int ARCADE      = 0; // == ARCADE
        public const int OBJECTIVES  = 1; // Default 'story' mode.
        public const int ELIMINATION = 2;


        private bool TERMINATION_CRITERION_MET = false;

        public GameMode(int m) {
            MODE = m;
            TERMINATION_CRITERION_MET = false;

            if (MODE == ARCADE) {
                HEALTH_RECHARGE = true;
                MUSIC_REPEAT = false;
            }
            else if (MODE == OBJECTIVES) {
                HEALTH_RECHARGE = false;
                MUSIC_REPEAT = true;
            }
        }

        public bool terminated() {
            switch (MODE) {
                case ARCADE: {
                    if (MusicHandler.getTrack().SONG_ENDED) TERMINATION_CRITERION_MET = true;
                    break;
                }
                case OBJECTIVES:    { // TODO: Survival mode termination criteria (Nothing: Only way to end is for GV to be dead. Handled separately)

                    break;
                }
                case ELIMINATION: { // TODO: Elimination mode termination criteria (no BV left)
                    List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();
                    if (bvs.Count == 0) TERMINATION_CRITERION_MET = true;
                    break;
                }
            }

            return TERMINATION_CRITERION_MET;
        }

        /*public int finaliseScore(bool gVKilled, int score) {
            switch (MODE) {
                case TIME_ATTACK: {
                        if (gVKilled) return 0; else return score;
                }
                case SURVIVAL: {
                    return score;
                }
                case ELIMINATION: {
                    if (gVKilled) return 0; else return score;
                }
            }

            // Should be impossible.
            return 0;
        }*/
    }
}
