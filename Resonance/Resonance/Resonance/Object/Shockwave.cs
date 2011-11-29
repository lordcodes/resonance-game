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

        public static double INITIAL_RADIUS = 0.1;

        // Size at which wave 'dies'
        public static double MAX_RADIUS = 2.0;

        // Colours
        public static int GREEN  = 0;
        public static int YELLOW = 0;
        public static int BLUE   = 0;
        public static int RED    = 0;
        public static int CYMBAL = 0;

        // Fields

        // Current radius of the shockwave
        private double radius;

        // List of Bad Vibes which this shockwave has already hit
        List<BadVibe> bVibes;

        // Properties

        public double Radius
        {
            get
            {
                return radius;
            }
        }

        public Shockwave(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            radius = INITIAL_RADIUS;

            bVibes = new List<BadVibe>();
        }


        // Methods


    }
}
