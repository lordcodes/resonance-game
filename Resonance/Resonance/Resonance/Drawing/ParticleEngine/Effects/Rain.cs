using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class Rain : Emitter {

        Random gen;

        public Rain(Vector3 p) : base(p) {
            emissionsPerUpdate = 0;
            particlesLeft      = 5000; // This will always be reset anyway as there is (potentially) infinite rain.
            maxParticleSpd     = 2f;
            maxParticleLife    = 1000; // Particles are removed after they hit the ground.
            iPSize             = 0f;
            iColour            = Color.Blue;
            deceleration       = 0f;

            gen = new Random();
        }

        public void setEmissionsPerUpdate(int ePU) {
            emissionsPerUpdate = ePU;
        }

        public void setRaindropSize(float s) {
            iPSize = s;
        }

        protected override void generateParticles() {
            for (int i = 0; i < emissionsPerUpdate; ++i) {
                Vector3 iDir = Vector3.Down;
                //float mapW = World.MAP_X;
                //float mapD = World.MAP_Z;

                // These only have to represent the area of screen you can see at one time, as Rain follows the GV
                float mapW = 50f;
                float mapD = 50f;

                float iSpd  = maxParticleSpd;
                int   iLife = maxParticleLife;

                Vector3 posOffset = new Vector3((float) gen.Next((int) (mapW / 2)), 0f, (float) gen.Next((int) (mapD / 2)));
                if (gen.Next() % 2 == 0) posOffset.X *= -1;
                if (gen.Next() % 2 == 0) posOffset.Z *= -1;

                Vector3 gravity = new Vector3(0f, -0.05f, 0f);

                Particle p = ParticleEmitterManager.getParticle();
                p.init(pos + posOffset, gravity, iSpd, iPSize, iLife, iColour, 1f, gravity, deceleration, false);
                p.setDieOnFloor(true);
                particles.Add(p);
            }
        }
    }
}
