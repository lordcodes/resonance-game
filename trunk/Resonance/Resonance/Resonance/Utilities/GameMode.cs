using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance {

    /// <summary>
    /// Superclass to represent a game mode.
    /// </summary>
    class GameMode {

        public readonly int MODE;

        public const int TIME_ATTACK = 0;
        public const int SURVIVAL    = 1;
        public const int ELIMINATION = 2;

        int[] zones;
        int currentZone = 0;

        private bool TERMINATION_CRITERION_MET = false;

        public GameMode(int m) {
            MODE = m;
            TERMINATION_CRITERION_MET = false;
            zones = new int[3];
            zones[0] = TIME_ATTACK;
            zones[1] = SURVIVAL;
            zones[2] = ELIMINATION;
        }

        public void changeZone()
        {
            currentZone++;
            //Something to change the level
        }

        public bool terminated() {
            switch (MODE) {
                case TIME_ATTACK: {
                    if (MusicHandler.getTrack().SONG_ENDED) TERMINATION_CRITERION_MET = true;
                    break;
                }
                case SURVIVAL:    { // TODO: Survival mode termination criteria (Nothing: Only way to end is for GV to be dead. Handled separately)
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
