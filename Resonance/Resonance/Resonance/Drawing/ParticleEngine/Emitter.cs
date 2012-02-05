using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Resonance {
    /// <summary>
    /// Represents an emitter of particles.
    /// </summary>
    class Emitter {
        
        /// Fields

        public Vector3 pos;

        protected int   emissionsPerUpdate;
        protected int   particlesLeft;
        protected int   maxParticleLife;
        protected float maxParticleSpd;
        protected float iPSize;

        protected List<Particle> particles;
        protected Texture2D pTex;

        protected static Random gen = new Random();

        public bool isEmpty() {
            DebugDisplay.update("PC", particles.Count.ToString());
            return ((particles.Count == 0) && (particlesLeft == 0));
        }

        public Texture2D getPTex() {
            return pTex;
        }

        public Emitter(Vector3 p) {
            particles = new List<Particle>();
            pos = p;

            // Default texture
            pTex = ParticleEmitterManager.Content.Load<Texture2D>("Drawing/Textures/texPixel");

            // Default p size
            iPSize = 0.02f;

            ParticleEmitterManager.addEmitter(this);
        }

        public List<Particle> getParticles() {
            return particles;
        }

        /// <summary>
        /// Generates the new particles for this update.
        /// </summary>
        protected virtual void generateParticles() {
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    if (gen.Next() % 2 == 0) iDir.X *= -1;
                    if (gen.Next() % 2 == 0) iDir.Y *= -1;
                    if (gen.Next() % 2 == 0) iDir.Z *= -1;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);
                    particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, Color.White));
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }

        /// <summary>
        /// The basic Emitter update method. Can be overridden by subclasses.
        /// </summary>
        public virtual void update() {
            // Remove dead particles and update alive ones.
            int killed = 0;
            for (int i = 0; i < particles.Count; ++i) {
                if (particles.ElementAt(i - killed).isDead()) {
                    particles.RemoveAt(i - killed);
                    killed++;
                } else {
                    particles.ElementAt(i - killed).update();
                }
            }

            // Generate new particles.
            generateParticles();
        }
    }
}
