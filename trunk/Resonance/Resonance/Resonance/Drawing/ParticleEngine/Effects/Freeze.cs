using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class Freeze : Emitter {

        private bool FREEZE_ON;

        public Freeze(Vector3 p) : base(p) {
            emissionsPerUpdate = 100;
            particlesLeft      = -1;     // Continuous
            maxParticleSpd     = 2f;
            maxParticleLife    = 10;
            iColour            = Color.White;
            iPSize             = 0.075f;
            deceleration       = 3f;

            FREEZE_ON = false;
        }

        public override void setPos(Vector3 p) {
            pos = new Vector3(p.X, p.Y + 1.2f, p.Z);
        }

        public void switchOn() {
            FREEZE_ON = true;
        }

        public void switchOff() {
            FREEZE_ON = false;
        }

        protected override void generateParticles() {
            if (FREEZE_ON) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    if (gen.Next() % 2 == 0) iDir.X *= -1;
                    if (gen.Next() % 2 == 0) iDir.Y *= -1;
                    if (gen.Next() % 2 == 0) iDir.Z *= -1;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);

                    Particle p = ParticleEmitterManager.getParticle();
                    p.init(pos, iDir, iSpd, iPSize, iLife, iColour, 1f, Vector3.Zero, deceleration, false);
                    particles.Add(p);
                }
            }
        }
    }
}
