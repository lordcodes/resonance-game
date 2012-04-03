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
        
        public static void shoot(BadVibe bv,Vector3 bulPos)
        {
            if (bulletIndex == 0)
            {
                bulletIndex = 1;
                shootingBadVibe = bv;
                bulletPosition = bulPos;
                bullet = new Bullet(17, "activeBullet", bulletPosition);
                ScreenManager.game.World.addObject(bullet);            
            }
        }
        public static void updateBullet()
        {
            iteration = ScreenManager.game.Iteration;
            if (bulletIndex == 1)
            {
                //if (iteration % 2 == 0)
               // {
                    ScreenManager.game.World.removeObject(bullet);
                    Vector3 dir = GameScreen.getGV().Body.Position - bulletPosition;
                    bulletPosition += dir/20;
                    
                    bullet = new Bullet(17, "activeBullet", bulletPosition);
                    //calculate new position then time it
                    ScreenManager.game.World.addObject(bullet);  
                //}
            }
        }
    }
}