using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class TestEmitter : Emitter {
        public TestEmitter(Vector3 p)
            : base(p) {
            emissionsPerUpdate = 50;
            particlesLeft = 50000;
            maxParticleSpd = 1;
            maxParticleLife = 100;
        }
    }
}
