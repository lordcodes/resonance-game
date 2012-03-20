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
        public static double GROWTH_RATE = 0.6;

        // Size at which wave 'dies'
        public static double MAX_RADIUS = 10;

        // Colours
        public const int REST   = 0;

        public const int RED    = 1;
        public const int YELLOW = 2;
        public const int BLUE   = 3;
        public const int GREEN  = 4;
        public const int CYMBAL = 5;

        // Fields

        // Current radius of the shockwave
        private double radius;

        // List of Bad Vibes which this shockwave has already hit
        List<BadVibe> bVibes;
        int numHit = 0;
        // Location
        Vector3 position;
        int colour;

        // WorldTransform
        Matrix transform;

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
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();

            foreach (BadVibe bv in bvs) 
            {
                if (bv.Status != BadVibe.State.DEAD && !bVibes.Contains(bv))
                {
                    double dist = Vector3.Distance(Position, bv.Body.Position);

                    //DebugDisplay.update("RADIUS", bv.Size + " : " + Radius + " : " + dist);

                    if (bv.Size + Radius >= dist)
                    {
                        //Collision
                        bVibes.Add(bv);

                        Vector3 blast = bv.Body.Position - position;
                        if (bv.damage(colour, blast))
                        {
                            numHit++;
                        }
                    }
                }
            }
        }

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

        public int NumberHit
        {
            get
            {
                return numHit;
            }
        }

        public int Colour
        {
            get
            {
                return colour;
            }
        }

    }
}
