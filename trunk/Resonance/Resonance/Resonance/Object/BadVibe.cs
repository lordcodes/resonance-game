using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BadVibe : DynamicObject
    {
        int health;
        int dir;

        public BadVibe(int modelNum, String name, Game game, Vector3 pos)
            : base(modelNum, name, game, pos)
        {
            health = 100;
            dir = -1;
        }

        void AdjustHealth(int change)
        {
        }

        public void Move()
        {
            float offsetx = 0.01f;
            float offsety = 0;
            float offsetz = 0.01f;
            double pos = 0.25;
            double neg = 0.5;
            double posz = 0.75;
            Random r = new Random();
            double direction = r.NextDouble();

            offsetx= (float)r.NextDouble() * (0.1f -0.01f) + 0.01f;
            offsetz = (float)r.NextDouble() * (0.1f - 0.01f) + 0.01f;

            switch (dir)
            {
                case 0:
                    {
                        pos = 0.97;
                        neg = 0.98;
                        posz = 0.99;
                        break;
                    }
                case 1:
                    {
                        pos = 0.01;
                        neg = 0.98;
                        posz = 0.99;
                        break;
                    }
                case 2:
                    {
                        pos = 0.01;
                        neg = 0.02;
                        posz = 0.99;
                        break;
                    }
                case 3:
                    {
                        pos = 0.01;
                        neg = 0.02;
                        posz = 0.03;
                        break;
                    }
                default:
                    {
                        break;
                    }


            } 

            if (direction < pos)
            {
                dir = 0;
            }
            else if (direction < neg)
            {
                offsetx *= -1.0f;
                offsetz *= -1.0f;
                dir = 1;
            }
            else if (direction < posz)
            {
                offsetx *= -1.0f;
                dir = 2;
            }
            else
            {
                offsetz *= -1.0f;
                dir = 3;
            }
   
            this.Body.Position += new Vector3(offsetx, offsety, offsetz);
        }

        

        void SetHealth(int value)
        {
        }

        int GetHealth()
        {
            return health;
        }
    }
}
