using System;
using Microsoft.Xna.Framework;

namespace Resonance
{

    class Bullet : DynamicObject
    {
        public const int RED = 0;
        public const int YELLOW = 1;
        public const int BLUE = 2;
        public const int GREEN = 3;

        private int colour;
        private Vector3 position;

        public Bullet(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            position = pos;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Matrix getTransform()
        {
            Matrix transform = Matrix.CreateTranslation(position);
            return transform;
        }

        public int Colour
        {
            get { return colour; }
            set 
            { 
                colour = value;
                ModelInstance.setTexture(colour);
            }
        }
    }
}
