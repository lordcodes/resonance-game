using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Shockwave : Object
    {
        // Constants

        public static double INITIAL_RADIUS = 0.05;
        public static double GROWTH_RATE = 0.1;

        // Size at which wave 'dies'
        public static double MAX_RADIUS = 2.5;

        // Colours
        public static int REST   = 0;

        public static int GREEN  = 1;
        public static int YELLOW = 2;
        public static int BLUE   = 3;
        public static int RED    = 4;
        public static int CYMBAL = 5;

        // Fields

        // Current radius of the shockwave
        private double radius;

        // List of Bad Vibes which this shockwave has already hit
        List<BadVibe> bVibes;

        // Location
        Vector3 position;

        // WorldTransform
        Matrix transform;

        // Properties

        public double Radius
        {
            get
            {
                return radius;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public Matrix Transform
        {
            get
            {
                return transform;
            }
        }

        public Shockwave(int modelNum, String name, Game game, Vector3 pos, Matrix t)
            : base(modelNum, name, game, pos)
        {
            position = new Vector3(pos.X, pos.Y, pos.Z);
            radius = INITIAL_RADIUS;
            transform = t;
            Matrix scale = Matrix.CreateScale((float)INITIAL_RADIUS*2, 1.0f, (float)INITIAL_RADIUS*2);
            transform = Matrix.Multiply(transform, scale);
            Matrix translate = Matrix.CreateTranslation((float)-(INITIAL_RADIUS*2-1) * position.X, 0.0f, (float)-(INITIAL_RADIUS*2-1) * position.Z);
            transform = Matrix.Multiply(transform, translate);

            bVibes = new List<BadVibe>();
        }


        // Methods

        public void grow()
        {
            radius *= (1 + GROWTH_RATE);

            Matrix scale = Matrix.CreateScale((float) (1.0f + GROWTH_RATE), 1.0f, (float) (1.0f + GROWTH_RATE));
            transform = Matrix.Multiply(transform, scale);
            Matrix translate = Matrix.CreateTranslation((float) -GROWTH_RATE * position.X, 0.0f, (float) -GROWTH_RATE * position.Z);
            transform = Matrix.Multiply(transform, translate);
        }
    }
}
