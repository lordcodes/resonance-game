using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {

    /// <summary>
    /// Class which represents a single particle of a particle system.
    /// </summary>
    class Particle {

        /// Fields
        Vector3 pos;
        float speed;
        Vector3 direction; // Normalised direction of movement vector.
        int lifespan; // Number of frames for which this particle is alive.
        Color colour;


        /// Properties

        public bool isDead() {
            return (lifespan <= 0);
        }

        public Vector3 getPos() {
            return pos;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Particle(Vector3 p, Vector3 iDirection, float iSpeed, int iLife, Color c) {
            speed = iSpeed;
            pos = p;
            direction = iDirection;
            lifespan = iLife;
            colour = c;
        }


        // s only used for testing
        public void update() {
            pos += (direction * speed);
            lifespan--;

            //s.Draw(Hud.getBlock(), new Rectangle((int) pos.X, (int) pos.Y, 1, 1), null, colour, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
