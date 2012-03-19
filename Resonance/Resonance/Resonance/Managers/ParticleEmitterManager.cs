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

        private static bool paused = false;

        public static ContentManager Content;

        // Holds all currently existing emitters.
        private static List<Emitter> emitters;

        // Particle 'pool'. Any particles generated are never deleted; they are instead put here, available for recycling.
        public static List<Particle> particlePool;

        public static List<Emitter> getEmitters() {
            return emitters;
        }

        public static void initialise() {
            paused = false;
            Content = ScreenManager.game.ScreenManager.Content;
            particlePool = new List<Particle>();
            emitters = new List<Emitter>();
        }

        public static void addEmitter(Emitter e) {
            emitters.Add(e);
        }

        public static void addToPool(Particle p) {
            particlePool.Add(p);
        }

        public static Particle getParticle() {
            Particle p;
            if (particlePool.Count > 0) {
                p = particlePool.Last();
                particlePool.Remove(p);
                return p;
            } else return new Particle();
        }

        public static void update() {
            if (!paused) {
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

        public static void pause(bool status) {
            paused = status;
        }

        public static bool isPaused() {
            return paused;
        }
    }
}
