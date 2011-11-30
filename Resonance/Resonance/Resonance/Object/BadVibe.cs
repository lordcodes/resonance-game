using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;

namespace Resonance
{
    class BadVibe : DynamicObject
    {
        public const double RADIUS = 0.5;

        Game game;
        int previousDirection;  //Remembers the previous movement direction 
        //TODO: change to enum 

        ArmourSequence armour;
        Boolean dead;

        public BadVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            this.game = game;
            previousDirection = -1;
            dead = false;

            armour = ArmourSequence.random();
        }

        /// <summary>
        /// Returns the sequence of armour layers of the Bad Vibe
        /// </summary>
        /// <returns></returns>
        public List<int> getLayers()
        {
            return armour.Sequence;
        }

        public Boolean Dead
        {
            get
            {
                return dead;
            }
        }

        /// <summary>
        /// Moves the bad vibe in the world randomly
        /// 
        /// Takes into account previous direction of movement so that the vibe is more likely to carry on in that direction
        /// Bin1: x and z positive
        /// Bin2: x and z negative
        /// Bin3: x negative
        /// Bin4: z negative
        /// 
        /// @offsetx: the amount of movement in the x direction
        /// @offsetz: the amount of movement in the z direction
        /// </summary>
        public void Move()
        {
            float offsetx = 0.01f;
            //float offsety = 0;
            float offsetz = 0.01f;

            double binBoundary1 = 0.25;
            double binBoundary2 = 0.5;
            double binBoundary3 = 0.75;
            int total = 0;
            byte[] Unicode = Encoding.Unicode.GetBytes(this.returnIdentifier());
            foreach (byte x in Unicode ){
                total+= x;
            }
            Random r = new Random((int)DateTime.Now.Ticks*total);
            double direction = r.NextDouble();

            offsetx = (float)r.NextDouble() * (0.05f - 0.01f) + 0.01f;
            offsetz = (float)r.NextDouble() * (0.05f- 0.01f) + 0.01f;

            //Probability of direction change
            switch (previousDirection)
            {
                case 0:
                    {
                        binBoundary1 = 0.97;
                        binBoundary2 = 0.98;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 1:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.98;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 2:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.02;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 3:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.02;
                        binBoundary3 = 0.03;
                        break;
                    }
                default:
                    {
                        break;
                    }
            } 

            //Movement
            if (direction < binBoundary1)
            {
                previousDirection = 0;
                move(0.1f);
            }
            else if (direction < binBoundary2)
            {
                move(-0.2f);
                previousDirection = 1;
            }
            else if (direction < binBoundary3)
            {
                move(0.1f);
                rotate(0.2f);
                previousDirection = 2;
            }
            else
            {
                move(0.1f);
                rotate(-0.2f);
                previousDirection = 3;
            }
        }

        /// <summary>
        /// Moves Bad Vibe towards Good Vibe
        /// </summary>
        public void moveTowardsGoodVibe()
        {
            //getGoodVibePos();
            Vector3 goodVibePosition = ((GoodVibe)game.World.getObject("Player")).Body.Position;
            Vector3 badVibePosition = this.Body.Position;
            //Console.WriteLine("BV " + this.Body.Position + "\nGV " + ((GoodVibe)game.World.getObject("Player")).Body.Position);
            //DebugDisplay.update("BV", this.Body.Position.ToString());
            //DebugDisplay.update("GV", ((GoodVibe)game.World.getObject("Player")).Body.Position.ToString());

            Vector3 difference = badVibePosition + goodVibePosition;
            Vector3 differenceangle = new Vector3();
            differenceangle.Y = (float)Math.Tan(difference.Z / difference.X);

            Quaternion orientation = this.Body.Orientation;
            Vector3 rotateVector = QuaternionToEuler(orientation);

         
            Vector3 gothisway = differenceangle - rotateVector;

            rotate((float)gothisway.Y);
            move(0.1f);
        }

        /// <summary>
        /// Gets the position of the Good Vibe
        /// </summary>
        void getGoodVibePos()
        { 
            Console.WriteLine(((GoodVibe)game.World.getObject("Player")).Body.Position);
        }

        /// <summary>
        /// Damage the bad vibe
        /// </summary>
        /// <param name="colour">The colour pf wave that has been attacked with</param>
        public void damage(int colour)
        {
            armour.breakLayer(colour, this);
        }

        private void kill()
        {
            dead = true;
        }



        /// <summary>
        /// Initialises the bank of armour sequences
        /// </summary>
        public static void initialiseBank()
        {
            if (!ArmourSequence.initialised)
            {
                ArmourSequence.initialiseBank();
            }
        }




        /// <summary>
        /// Armour Sequence nested class
        /// </summary>
        private class ArmourSequence
        {
            // Constants

            static List<ArmourSequence> bank = new List<ArmourSequence>(); //the bank of armour sequences
            static int COUNT; // Number of sequences

            // Fields

            public static Boolean initialised = false; //whether bank has been initialised
            List<int> sequence; //the armour sequence

            /// <summary>
            /// Creates an armour sequence. Constructor.
            /// </summary>
            /// <param name="seq">int array of the armour sequence</param>
            private ArmourSequence(int[] seq)
            {
                sequence = new List<int>();
                for (int i = 0; i < seq.Length; i++)
                {
                    sequence.Add(seq[i]);
                }
            }

            /// <summary>
            /// Returns the armour sequence
            /// </summary>
            public List<int> Sequence {
                get
                {
                    return sequence;
                }
            }

            /// <summary>
            /// Select a random armour sequence
            /// </summary>
            /// <returns></returns>
            public static ArmourSequence random()
            {
                Random generator = new Random();
                int x = generator.Next();
                x %= COUNT;

                return new ArmourSequence(bank[x].Sequence.ToArray());
            }

            /// <summary>
            /// Break the amour layer
            /// </summary>
            /// <param name="colour">The colour of wave</param>
            public void breakLayer(int colour, BadVibe vibe)
            {
                if (sequence[0] == colour) {
                    sequence.RemoveAt(0);
                }
                if (sequence.Count == 0) vibe.kill();
            }






            /// <summary>
            /// Initialise the bank of armour sequences
            /// </summary>
            public static void initialiseBank() {
                initialised = true;
                COUNT = 0;
                //Random x = new Random();
                //x %=

                // Easy
                /*{
                    int[] seq = {1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 5};
                    bank.Add(new ArmourSequence(seq));
                }

                {
                    int[] seq = {1, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 5};
                    bank.Add(new ArmourSequence(seq));
                }*/
                {
                    int[] seq = { 1, 1, 2, 2, 3, 3, 4, 4, 1, 2, 3, 4, 5 };
                    bank.Add(new ArmourSequence(seq));
                    COUNT++;
                }
            }

        }
    }
}
