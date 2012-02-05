using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class Explosion : Emitter {

        public Explosion(Vector3 p)
            : base(p) {
            emissionsPerUpdate = 500;
            particlesLeft = 500;
            maxParticleSpd = 0.6f;
            maxParticleLife = 30;
            iColour = Color.White;
        }

        protected override void generateParticles() {
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    if (gen.Next() % 2 == 0) iDir.X *= -1;
                    if (gen.Next() % 2 == 0) iDir.Y *= -1;
                    if (gen.Next() % 2 == 0) iDir.Z *= -1;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);

                    if (gen.Next() % 4 == 0) { 
                        particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, iColour, 1f, Vector3.Zero));
                    } else {
                        particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, Color.Black, 1f, Vector3.Zero));
                    }
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }
    }
}
