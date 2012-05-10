using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using BEPUphysics.Collidables;
using Microsoft.Xna.Framework;


namespace Resonance
{
    class Checkpoint : Object
    {
        public const int RED = 0;
        public const int YELLOW = 1;
        public const int BLUE = 2;
        public const int GREEN = 3;
        public const int WHITE = 4;

        private static Random randomGen = new Random((int)(DateTime.Now.Ticks));

        int colour;
        Vector3 position;
        Matrix transform;

        bool hit;

        public Checkpoint(int modelNum, String name, Vector3 pos, int colour)
            : base(modelNum, name, pos)
        {
            ModelInstance.setTexture(colour);
            hit = false;
            this.colour = colour;
            position = pos;
            transform = Matrix.CreateTranslation(position);
            float angle = MathHelper.ToRadians(randomGen.Next(0, 360));
            Matrix rot = Matrix.CreateRotationY(angle);
            transform = Matrix.Multiply(rot, transform);
        }

        public bool beenHit() {
            return hit;
        }

        public bool hitPoint(int drum)
        {
            if (drum == this.colour)
            {
                hit = true;
                ModelInstance.setTexture(WHITE);
                colour = WHITE;
                return true;
            }
            return false;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Matrix Transform
        {
            get { return transform; }
        }
    }
}
