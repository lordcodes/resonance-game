using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Resonance {

    /// <summary>
    /// Manages all particle system data.
    /// </summary>
    class ParticleEmitterManager {

        public static ContentManager Content;

        // Holds all currently existing emitters.
        private static List<Emitter> emitters;

        public static List<Emitter> getEmitters() {
            return emitters;
        }

        public static void initialise(ContentManager c) {
            Content = c;
            emitters = new List<Emitter>();
        }

        public static void addEmitter(Emitter e) {
            emitters.Add(e);
        }

        public static void update() {
            int noRemoved = 0;
            for (int i = 0; i < emitters.Count; i++) {
                if (emitters.ElementAt(i - noRemoved).isEmpty()) {
                    emitters.RemoveAt(i - noRemoved);
                    noRemoved++;
                } else {
                    emitters.ElementAt(i - noRemoved).update();
                }
            }
        }
    }
}