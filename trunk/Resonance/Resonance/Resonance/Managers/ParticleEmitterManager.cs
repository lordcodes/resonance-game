using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance {

    /// <summary>
    /// Manages all particle system data.
    /// </summary>
    class ParticleEmitterManager {

        // Holds all currently existing emitters.
        private static List<Emitter> emitters;

        public static List<Emitter> getEmitters() {
            return emitters;
        }

        public static void initialise() {
            emitters = new List<Emitter>();
        }

        public static void addEmitter(Emitter e) {
            emitters.Add(e);
        }

        public static void update() {
            foreach(Emitter e in emitters) {
                e.update();
            }
        }
    }
}
