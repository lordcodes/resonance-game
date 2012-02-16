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

        private bool TERMINATION_CRITERION_MET;

        public GameMode(int m) {
            MODE = m;
            TERMINATION_CRITERION_MET = false;
        }

        public bool terminated() {
            switch (MODE) {
                case TIME_ATTACK: {
                    if (GameScreen.musicHandler.getTrack().SONG_ENDED) TERMINATION_CRITERION_MET = true;
                    break;
                }
                case SURVIVAL:    { // TODO: Survival mode termination criteria (Nothing: Only way to end is for GV to be dead. Handled separately)
                    break;
                }
                case ELIMINATION: { // TODO: Elimination mode termination criteria (no BV left)
                    if (ScreenManager.game.World.returnBadVibes().Count == 0) TERMINATION_CRITERION_MET = true;
                    break;
                }
            }

            return TERMINATION_CRITERION_MET;
        }

        public int finaliseScore(bool gVKilled, int score) {
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
        }
    }
}
