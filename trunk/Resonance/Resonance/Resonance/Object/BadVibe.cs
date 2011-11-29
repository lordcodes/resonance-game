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
        Game game;
        int health;
        int previousDirection;  //Remembers the previous movement direction 
        //TODO: change to enum 

        public BadVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            this.game = game;
            health = 100;
            previousDirection = -1;
        }

        void AdjustHealth(int change)
        {
          
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
   
            //this.Body.Position += new Vector3(offsetx, offsety, offsetz);
            
        }

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

        void getGoodVibePos()
        { 
            Console.WriteLine(((GoodVibe)game.World.getObject("Player")).Body.Position);
        }

        void SetHealth(int value)
        {
        }

        public int GetHealth()
        {
            return health;
        }
    }
}
