using System;
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
            ModelInstance.Transparency = 0.5f;
            ModelInstance.Shadow = false;
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

        public Color getColour() {
            switch (colour) {
                case WHITE  : return Color.White;
                case BLUE   : return Color.Blue;
                case RED    : return Color.Red;
                case GREEN  : return Color.Green;
                case YELLOW : return Color.Yellow;
            }

            return Color.White;
        }
    }
}
