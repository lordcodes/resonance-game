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
   
    static class BulletManager
    {
        private static Vector3 bulletPosition;
        private static int bulletIndex = 0;
        private static BadVibe shootingBadVibe;
        private static Bullet bullet = null;
        private static int iteration;
        private static int DAMAGE = 5;
        
        //testing purposes this will be changed to use tom`s code
        private static string[] colors = new string[] { "red", "blue", "green", "yellow", "cymbal"};
        private static int index = -1;
        
        public static void shoot(BadVibe bv,Vector3 bulPos)
        {
            if (bulletIndex == 0)
            {
                bulletIndex = 1;
                shootingBadVibe = bv;
                bulletPosition = bulPos;
                bullet = new Bullet(17, "activeBullet", bulletPosition);
                ScreenManager.game.World.addObject(bullet);
                index++;
                if (index > 4)
                    index = 0;
                bullet.setColor(colors[index]);
                
                DebugDisplay.update("Bullet Color", bullet.getColor());
            }
        }
        public static void updateBullet()
        {
            iteration = ScreenManager.game.Iteration;
            if (bulletIndex == 1)
            {
               if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 5f && iteration % 10 == 0)
                {
                    GameScreen.getGV().AdjustHealth(-DAMAGE);
                    ScreenManager.game.World.removeObject(bullet);
                    bulletIndex = 0;
                }
                else
                {
                    ScreenManager.game.World.removeObject(bullet);
                    Vector3 dir = GameScreen.getGV().Body.Position - bulletPosition;
                    bulletPosition += dir / 20;

                    bullet = new Bullet(17, "activeBullet", bulletPosition);
                    bullet.setColor(colors[index]);
                    ScreenManager.game.World.addObject(bullet);
                }   
            }
        }

        public static void destroyBullet(string color)
        {
           
            if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 25f && bullet.getColor().Equals(color))
            {
                bulletIndex = 0;
                ScreenManager.game.World.removeObject(bullet);
               
            }
        }
    }
}