using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance {
    class ObjectiveManager {

        public const int KILL_ALL_BV = 0;

        private static int cObj = KILL_ALL_BV; // Default objective KILL_ALL_BV

        public static void setObjective(int newObj) {
            cObj = newObj;
        }

        public bool objectiveMet() {
            return false;
        }
    }
}
