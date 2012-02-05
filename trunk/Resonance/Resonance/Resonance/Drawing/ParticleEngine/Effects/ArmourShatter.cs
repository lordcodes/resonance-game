using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class ArmourShatter : Emitter {

        // The direction in which the armour layer is being blasted.
        Vector3 blastVec;

        public ArmourShatter(Vector3 p, Vector3 blast) : base(p) {
            pTex = ParticleEmitterManager.Content.Load<Texture2D>("Drawing/Textures/texTriangle");
            blastVec = blast;
            emissionsPerUpdate = 800;
            particlesLeft = 800;
            maxParticleSpd = 0.6f;
            maxParticleLife = 30;
        }

        /// <summary>
        /// Override generateParticles as the blast vector will have to come into play.
        /// </summary>
        private void generateParticles() {
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    if (gen.Next() % 2 == 0) iDir.X *= -1;
                    if (gen.Next() % 2 == 0) iDir.Y *= -1;
                    if (gen.Next() % 2 == 0) iDir.Z *= -1;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);
                    particles.Add(new Particle(pos, iDir, iSpd, iLife, Color.White));
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }
    }
}
