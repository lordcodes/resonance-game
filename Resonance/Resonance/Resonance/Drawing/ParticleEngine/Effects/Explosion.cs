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
        }
    }
}
