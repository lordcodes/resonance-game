using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {
    /// <summary>
    /// Represents an emitter of particles.
    /// </summary>
    class Emitter {
        
        /// Fields
        public Vector3 pos;

        protected int emissionsPerUpdate;
        protected int particlesLeft;
        protected int maxParticleSpd;
        protected int maxParticleLife;

        protected List<Particle> particles;

        protected static Random gen = new Random();

        public Emitter(Vector3 p) {
            particles = new List<Particle>();
            pos = p;
        }

        public List<Particle> getParticles() {
            return particles;
        }

        /// <summary>
        /// The basic Emitter update method. Can be overridden by subclasses.
        /// </summary>
        public void update() {

            // Remove dead particles.
            int killed = 0;
            for (int i = 0; i < particles.Count; ++i) {
                if (particles.ElementAt(i).isDead()) {
                    particles.RemoveAt(i - killed);
                    killed++;
                }
            }

            // Update and draw current particles.
            foreach (Particle p in particles) {
                p.update();
            }

            // Generate new particles.
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    if (particlesLeft <= 0) break;

                    Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    if (gen.Next() % 2 == 0) iDir.X *= -1;
                    if (gen.Next() % 2 == 0) iDir.Y *= -1;
                    if (gen.Next() % 2 == 0) iDir.Z *= -1;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);
                    particles.Add(new Particle(pos, iDir, iSpd, iLife, Color.White));
                    particlesLeft--;
                }
            }
        }
    }
}
