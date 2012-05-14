using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {

    /// <summary>
    /// Manages all particle system data.
    /// </summary>
    class ParticleEmitterManager {
        public static Texture2D TEX_PIXEL;
        public static Texture2D TEX_DIST_BV;
        public static Texture2D TEX_TRIANGLE;

        private const int INITIAL_PARTICLE_POOL_SIZE = 3500;
        private const int INITIAL_EMITTER_POOL_SIZE  = 50;

        private static bool paused = false;

        public static ContentManager Content;

        // Holds all currently existing emitters.
        private static List<Emitter> emitters;

        // Particle 'pool'. Any particles generated are never deleted; they are instead put here, available for recycling.
        public static List<Particle> particlePool;
        public static List<Emitter>   emitterPool;

        public static List<Emitter> getEmitters() {
            return emitters;
        }

        public static void initialise() {
            paused = false;
            Content = ScreenManager.game.ScreenManager.Content;

            TEX_PIXEL    = Content.Load<Texture2D>("Drawing/Textures/texPixel");
            TEX_DIST_BV  = Content.Load<Texture2D>("Drawing/HUD/Textures/map_distant_vibe");
            TEX_TRIANGLE = Content.Load<Texture2D>("Drawing/Textures/texTriangle");

            particlePool = new List<Particle>(INITIAL_PARTICLE_POOL_SIZE);
            emitterPool  = new List<Emitter>(INITIAL_EMITTER_POOL_SIZE);
            emitters = new List<Emitter>(INITIAL_EMITTER_POOL_SIZE);
            fillParticlePool();
        }

        public static void addEmitter(Emitter e) {
            emitters.Add(e);
        }

        public static void addToPool(Particle p) {
            particlePool.Add(p);
        }

        public static void addToPool(Emitter e) {
            emitterPool.Add((Emitter) e);
        }

        public static Particle getParticle() {
            Particle p;
            if (particlePool.Count > 0) {
                p = particlePool.Last();
                particlePool.Remove(p);
                return p;
            } else return new Particle();
        }
        
        public static Emitter getEmitter<T>() {
            for (int i = 0; i < emitterPool.Count; i++) {
                if (emitterPool.ElementAt(i) is T) {
                    Emitter e = emitterPool.ElementAt(i);
                    emitterPool.Remove(e);
                    return e;
                }
            }

            return null;
            /*if (emitterPool.Count > 0) {
                Emitter e = emitterPool.Last();
                emitterPool.Remove(e);
                return e;
            } else return new Emitter();*/
        }

        public static void fillParticlePool() {
            for (int i = 0; i < INITIAL_PARTICLE_POOL_SIZE; i++) {
                particlePool.Add(new Particle());
            }
        }

        public static void fillEmitterPool() {
            emitterPool.Add(new Freeze());
            emitterPool.Add(new Rain());
            emitterPool.Add(new BulletImpact());
            emitterPool.Add(new BulletImpact());
            for (int i = 2; i < INITIAL_EMITTER_POOL_SIZE; i++) {
                if (i % 4 == 0) emitterPool.Add(new Explosion());
                else emitterPool.Add(new ArmourShatter());
            }
        }

        public static void update() {
            Emitter e;

            if (!paused) {
                int noRemoved = 0;
                for (int i = 0; i < emitters.Count; i++) {
                    e = emitters.ElementAt(i - noRemoved);
                    if (e.isEmpty()) {
                        emitters.Remove(e);
                        addToPool(e);
                        noRemoved++;
                    } else {
                        emitters.ElementAt(i - noRemoved).update();
                    }
                }
            }
        }

        public static void pause(bool status) {
            paused = status;

            DebugDisplay.update("PE: ", paused.ToString());
        }

        public static bool isPaused() {
            return paused;
        }
    }
}
