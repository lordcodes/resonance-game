using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics;
using BEPUphysics.Paths;

namespace Resonance
{
    class BadVibe : DynamicObject
    {
        public const double RADIUS = 0.5;
        private const double ATTACK_RATE = 0.8;
        private const double REDUCTION_RATE = 15;
        private const double ATTACK_RANGE = 3;

        private int iterationCount = 0;

        int previousDirection;  //Remembers the previous movement direction 

        AI ai;

        ArmourSequence armour;
        bool dead;

        public BadVibe(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            previousDirection = -1;
            dead = false;

            armour = ArmourSequence.random();
            setColour();

            ai = new AI();
        }

        /// <summary>
        /// Returns the sequence of armour layers of the Bad Vibe
        /// </summary>
        /// <returns></returns>
        public List<int> getLayers()
        {
            return armour.Sequence;
        }

        public bool Dead
        {
            get
            {
                return dead;
            }
        }

        /// <summary>
        /// Moves the bad vibe in the world
        /// 
        /// Takes into account previous direction of movement so that the vibe is more likely to carry on in that direction
        /// Bin1: x and z positive
        /// Bin2: x and z negative
        /// Bin3: x negative
        /// Bin4: z negative
        /// 
        /// </summary>
        public void Move()
        {           
            if (getDistance() > 10)
            {
                moveAround();
            }
            else
            {
                if (getDistance() > 3)
                {
                    moveTowardsGoodVibe();
                }
                else
                {
                    Vector3 gvPos = ((GoodVibe)Program.game.World.getObject("Player")).Body.Position;
                    RotateToFaceGoodVibe(gvPos);
                }

                if ( getDistance() < ATTACK_RANGE )
                {
                    if ( (iterationCount % REDUCTION_RATE) == 0 )
                    {
                        Random r = new Random((int)DateTime.Now.Ticks);
                        double attack = r.NextDouble();
                        if (attack < ATTACK_RATE)
                        {
                            attackGoodVibe();
                        }
                    }
                    iterationCount++;
                }
            }

            if (iterationCount == 59) iterationCount = 0;

        }

        private void moveAround()
        {
            double binBoundary1 = 0.25, binBoundary2 = 0.5, binBoundary3 = 0.75;
            int total = 0;
            foreach (byte x in Encoding.Unicode.GetBytes(this.returnIdentifier())) total += x;

            Random r = new Random((int)DateTime.Now.Ticks * total);
            double direction = r.NextDouble();

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
                default: break;
            }

            //Movement
            if (direction < binBoundary1)
            {
                previousDirection = 0;
                move(BV_FORWARD);
            }
            else if (direction < binBoundary2)
            {
                move(BV_BACKWARD);
                previousDirection = 1;
            }
            else if (direction < binBoundary3)
            {
                move(BV_FORWARD);
                rotate(ROTATE_ANTI);
                previousDirection = 2;
            }
            else
            {
                move(BV_FORWARD);
                rotate(ROTATE_CLOCK);
                previousDirection = 3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void attackGoodVibe()
        {
            ((GoodVibe)Program.game.World.getObject("Player")).AdjustHealth(-1);
        }

        public void RotateToFaceGoodVibe(Vector3 gvPos)
        {
            Vector3 bvDir = Body.OrientationMatrix.Backward;
            Vector3 bvPos = Body.Position;
            Vector3 diff = Vector3.Normalize(gvPos - bvPos);
            Quaternion rot;
            Toolbox.GetQuaternionBetweenNormalizedVectors(ref bvDir, ref diff, out rot);
            Vector3 angles = BadVibe.QuaternionToEuler(rot);
            rot.X = 0;
            rot.Z = 0;
            rotator.TargetOrientation = Quaternion.Concatenate(Body.Orientation, rot);
        }

        /// <summary>
        /// Moves Bad Vibe towards Good Vibe
        /// </summary>
        /// 
        public void moveTowardsGoodVibe()
        {
            //Vector3 target = ai.calculateStep(this);
            Vector3 target = ((DynamicObject)Program.game.World.getObject("Player")).Body.Position;
            RotateToFaceGoodVibe(target);
            move(BV_FORWARD);
        }

        /// <summary>
        /// Calculates the difference between good vibe and bad vibe
        /// </summary>
        public double getDistance()
        {
            Vector3 goodVibePosition = ((GoodVibe)Program.game.World.getObject("Player")).Body.Position;
            Vector3 badVibePosition = this.Body.Position;
            double xDiff = Math.Abs(goodVibePosition.X - badVibePosition.X);
            double yDiff = Math.Abs(goodVibePosition.Y - badVibePosition.Y);
            double zDiff = Math.Abs(goodVibePosition.Z - badVibePosition.Z);
            double distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));
            return distance;
        }

        /// <summary>
        /// Damage the bad vibe
        /// </summary>
        /// <param name="colour">The colour pf wave that has been attacked with</param>
        public void damage(int colour)
        {
            armour.breakLayer(colour, this);
            if (!dead) setColour();

        }

        public void kill()
        {
            dead = true;
        }

        private void setColour()
        {
            switch (armour.Sequence[0])
            {
                case Shockwave.GREEN:
                    {
                        gameModelNum = GameModels.BAD_VIBE_GREEN;
                        break;
                    }
                case Shockwave.YELLOW:
                    {
                        gameModelNum = GameModels.BAD_VIBE_YELLOW;
                        break;
                    }
                case Shockwave.BLUE:
                    {
                        gameModelNum = GameModels.BAD_VIBE_BLUE;
                        break;
                    }
                case Shockwave.RED:
                    {
                        gameModelNum = GameModels.BAD_VIBE_RED;
                        break;
                    }
                case Shockwave.CYMBAL:
                    {
                        gameModelNum = GameModels.BAD_VIBE_CYMBAL;
                        break;
                    }
            }
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

            static List<ArmourSequence> bank          = new List<ArmourSequence>(); //the bank of armour sequences
            static List<ArmourSequence> beginnerBank  = new List<ArmourSequence>(); //the bank of armour sequences
            static List<ArmourSequence> easyBank      = new List<ArmourSequence>(); //the bank of armour sequences
            static List<ArmourSequence> mediumBank    = new List<ArmourSequence>(); //the bank of armour sequences
            static List<ArmourSequence> hardBank      = new List<ArmourSequence>(); //the bank of armour sequences
            static List<ArmourSequence> expertBank    = new List<ArmourSequence>(); //the bank of armour sequences

            static int COUNT;          // Number of sequences
            static int BEGINNER_COUNT; // Number of sequences
            static int EASY_COUNT;     // Number of sequences
            static int MEDIUM_COUNT;   // Number of sequences
            static int HARD_COUNT;     // Number of sequences
            static int EXPERT_COUNT;   // Number of sequences

            // Fields

            public static bool initialised = false; //whether bank has been initialised
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

                if (Resonance.Game.DIFFICULTY == Resonance.Game.BEGINNER)
                {
                    x %= BEGINNER_COUNT;
                    return new ArmourSequence(beginnerBank[x].Sequence.ToArray());
                }
                else if (Resonance.Game.DIFFICULTY == Resonance.Game.EASY)
                {
                    x %= EASY_COUNT;
                    return new ArmourSequence(easyBank[x].Sequence.ToArray());
                }
                else if (Resonance.Game.DIFFICULTY == Resonance.Game.MEDIUM)
                {
                    x %= MEDIUM_COUNT;
                    return new ArmourSequence(mediumBank[x].Sequence.ToArray());
                }
                else if (Resonance.Game.DIFFICULTY == Resonance.Game.HARD)
                {
                    x %= HARD_COUNT;
                    return new ArmourSequence(hardBank[x].Sequence.ToArray());
                }
                else if (Resonance.Game.DIFFICULTY == Resonance.Game.EXPERT)
                {
                    x %= EXPERT_COUNT;
                    return new ArmourSequence(expertBank[x].Sequence.ToArray());
                }
                else if (Resonance.Game.DIFFICULTY == Resonance.Game.INSANE) {
                    return generateRandom();
                }

                return new ArmourSequence(beginnerBank[x].Sequence.ToArray());
            }

            /// <summary>
            /// Break the amour layer
            /// </summary>
            /// <param name="colour">The colour of wave</param>
            public void breakLayer(int colour, BadVibe vibe)
            {
                if (sequence[0] == colour)
                {
                    switch (sequence[0])
                    {
                        case Shockwave.GREEN:
                            {
                                ((GoodVibe)Program.game.World.getObject("Player")).Score++;
                                break;
                            }
                        case Shockwave.YELLOW:
                            {
                                ((GoodVibe)Program.game.World.getObject("Player")).Score++;
                                break;
                            }
                        case Shockwave.BLUE:
                            {
                                ((GoodVibe)Program.game.World.getObject("Player")).Score++;
                                break;
                            }
                        case Shockwave.RED:
                            {
                                ((GoodVibe)Program.game.World.getObject("Player")).Score++;
                                break;
                            }
                        case Shockwave.CYMBAL:
                            {
                                ((GoodVibe)Program.game.World.getObject("Player")).Score += 5;
                                break;
                            }
                        case Shockwave.REST:
                            {
                                break;
                            }
                    }
                    sequence.RemoveAt(0);
                }
                if (sequence.Count == 0)
                {
                    ((GoodVibe)Program.game.World.getObject("Player")).Score += 10;
                    vibe.kill();
                }
            }


            private static ArmourSequence generateRandom()
            {
                Random generator = new Random();

                int[] seq = new int[13];

                for (int i = 0; i < 12; i++)
                {
                    seq[i] = generator.Next() % 5;
                }

                seq[12] = Shockwave.CYMBAL;

                return new ArmourSequence(seq);
            }






            /// <summary>
            /// Initialise the bank of armour sequences
            /// </summary>
            public static void initialiseBank() {
                initialised = true;

                initialiseBeginner();
                initialiseEasy();
                initialiseMedium();
                initialiseHard();
                initialiseExpert();

                COUNT = 0;
                {
                    int[] seq = { 1, 1, 2, 2, 3, 3, 4, 4, 1, 2, 3, 4, 5 };
                    bank.Add(new ArmourSequence(seq));
                    COUNT++;
                }
            }

                private static void initialiseBeginner()
                {
                    BEGINNER_COUNT = 0;

                    {
                        int[] seq = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 5 };
                        beginnerBank.Add(new ArmourSequence(seq));
                        BEGINNER_COUNT++;
                    }

                    {
                        int[] seq = { 1, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 5 };
                        beginnerBank.Add(new ArmourSequence(seq));
                        BEGINNER_COUNT++;
                    }
                }

                private static void initialiseEasy()
                {
                    EASY_COUNT = 0;

                    {
                        int[] seq = { 1, 0, 1, 0, 1, 0, 1, 0, 2, 0, 3, 0, 5 };
                        easyBank.Add(new ArmourSequence(seq));
                        EASY_COUNT++;
                    }

                    {
                        int[] seq = { 1, 0, 2, 0, 3, 0, 2, 0, 1, 0, 4, 0, 5 };
                        easyBank.Add(new ArmourSequence(seq));
                        EASY_COUNT++;
                    }

                    {
                        int[] seq = { 2, 0, 2, 0, 3, 0, 0, 0, 4, 0, 4, 0, 5 };
                        easyBank.Add(new ArmourSequence(seq));
                        EASY_COUNT++;
                    }

                    {
                        int[] seq = { 4, 0, 4, 0, 3, 0, 3, 0, 2, 0, 1, 0, 5 };
                        easyBank.Add(new ArmourSequence(seq));
                        EASY_COUNT++;
                    }
                }

                private static void initialiseMedium()
                {
                    MEDIUM_COUNT = 0;

                    {
                        int[] seq = { 1, 1, 1, 1, 3, 3, 3, 3, 4, 4, 4, 4, 5 };
                        mediumBank.Add(new ArmourSequence(seq));
                        MEDIUM_COUNT++;
                    }

                    {
                        int[] seq = { 2, 0, 1, 0, 2, 0, 1, 0, 3, 3, 4, 0, 5 };
                        mediumBank.Add(new ArmourSequence(seq));
                        MEDIUM_COUNT++;
                    }
                }

                private static void initialiseHard()
                {
                    HARD_COUNT = 0;

                    {
                        int[] seq = { 2, 2, 3, 3, 2, 2, 3, 0, 1, 1, 2, 1, 5 };
                        hardBank.Add(new ArmourSequence(seq));
                        HARD_COUNT++;
                    }

                    {
                        int[] seq = { 1, 1, 1, 0, 2, 1, 1, 0, 3, 3, 3, 4, 5 };
                        hardBank.Add(new ArmourSequence(seq));
                        HARD_COUNT++;
                    }
                }

                private static void initialiseExpert()
                {
                    EXPERT_COUNT = 0;

                    {
                        int[] seq = { 3, 2, 4, 2, 4, 2, 3, 0, 2, 1, 0, 2, 5 };
                        expertBank.Add(new ArmourSequence(seq));
                        EXPERT_COUNT++;
                    }

                    {
                        int[] seq = { 4, 1, 2, 1, 3, 2, 3, 2, 0, 1, 0, 1, 5 };
                        expertBank.Add(new ArmourSequence(seq));
                        EXPERT_COUNT++;
                    }
                }

        }
    }
}
