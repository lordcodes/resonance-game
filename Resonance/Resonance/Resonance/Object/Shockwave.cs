using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class Shockwave : Object
    {
        // Constants
        public static double INITIAL_RADIUS = 0.05;
        public static double GROWTH_RATE = 0.3;

        // Size at which wave 'dies'
        public static double MAX_RADIUS = 5;

        // Colours
        public const int REST   = 0;

        public const int GREEN  = 1;
        public const int YELLOW = 2;
        public const int BLUE   = 3;
        public const int RED    = 4;
        public const int CYMBAL = 5;

        // Fields

        // Current radius of the shockwave
        private double radius;

        // List of Bad Vibes which this shockwave has already hit
        List<BadVibe> bVibes;
        // Location
        Vector3 position;
        int colour;

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

        public Shockwave(int modelNum, String name, Vector3 pos, Matrix t, int colour)
            : base(modelNum, name, pos)
        {
            this.colour = colour;
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
            Matrix translate = Matrix.CreateTranslation((float)-GROWTH_RATE * position.X, 0.0f, (float)-GROWTH_RATE * position.Z);
            transform = Matrix.Multiply(transform, translate);
        }

        public void checkBadVibes()
        {
            Dictionary<string,Object> objects = Program.game.World.returnObjects();
            foreach (KeyValuePair<string,Object> pair in objects)
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;

                    if(!bVibes.Contains(vibe)) {
                        
                        double dx = Position.X - vibe.Body.Position.X;
                        double dz = Position.Z - vibe.Body.Position.Z;
                        double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                        d = Math.Sqrt(d);

                        if (BadVibe.RADIUS + Radius >= d)
                        {
                            //Collision
                            bVibes.Add(vibe);
                            vibe.damage(colour);
                        }
                    }
                }
            }
        }
    }
}
