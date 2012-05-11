using System.Collections.Generic;

namespace Resonance {
    class TestEmitter : Emitter {
        public TestEmitter()
            : base() {
            emissionsPerUpdate = 800;
            particlesLeft = 800;
            maxParticleSpd = 0.6f;
            maxParticleLife = 30;
            particles = new List<Particle>(particlesLeft);
        }
    }
}
