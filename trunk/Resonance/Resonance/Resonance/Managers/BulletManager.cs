using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics;
using BEPUphysics.Paths;
using Microsoft.Xna.Framework.Content;
namespace Resonance
{
   
    static class BulletManager
    {
        public static bool BOSS_EXISTS = false;

        private static Vector3 bulletPosition;
        private static int bulletIndex = 0;
        private static Vector3 bossPosition;
        private static Bullet bullet = new Bullet(17, "activeBullet", bulletPosition);
        private static int iteration;
        private static int DAMAGE = 5;
        private static int ACTIVE = 1;
        private static int INACTIVE = 0;
        private static DateTime beatTimeBefore = DateTime.Now;
        private static DateTime timeNow;
        private static long TIMESPAN = 15000000;
        private static Random Rand = new Random();
        private static int CHUNK = 15;     
        private static string[] colors = new string[] { "red", "blue", "green", "yellow"};
        private static int index;


        public static void shoot(Vector3 bulPos)
        {
                bulletIndex = ACTIVE;
                bulletPosition = bulPos;                
                ScreenManager.game.World.addObject(bullet);                
                index = Rand.Next();
                bullet.setColor(colors[index % 4]);
                bullet.ModelInstance.setTexture(index % 4);
        }


        public static void updateBossPosition(Vector3 position)
        {
            bossPosition = position;
        }


        public static void updateBullet()
        {
            if (BOSS_EXISTS)
            {
                iteration = ScreenManager.game.Iteration;
                if (bulletIndex == ACTIVE)
                {
                    if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 2f)
                    {
                        GameScreen.getGV().AdjustHealth(-DAMAGE);
                        ScreenManager.game.World.removeObject(bullet);
                        Drawing.addWave(bulletPosition);
                        bulletIndex = INACTIVE;
                    }
                    else
                    {
                        timeNow = DateTime.Now;
                        long ticks = timeNow.Ticks - beatTimeBefore.Ticks;
                        DebugDisplay.update("Time difference", ticks.ToString());
                        ScreenManager.game.World.removeObject(bullet);
                        Vector3 dir = GameScreen.getGV().Body.Position - bulletPosition;
                        if (ticks >= TIMESPAN)
                        {
                            beatTimeBefore = DateTime.Now;
                            bulletPosition += dir;
                        }
                        else
                            bulletPosition += dir / CHUNK;
                        bullet = new Bullet(17, "activeBullet", bulletPosition);
                        bullet.setColor(colors[index % 4]);
                        bullet.ModelInstance.setTexture(index % 4);
                        ScreenManager.game.World.addObject(bullet);
                    }

                }
                if (bulletIndex == INACTIVE)
                {
                    shoot(bossPosition);
                }
            }
        }
        
        public static void destroyBullet(string color)
        {
            if (BOSS_EXISTS)
            {
                if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 30f && bullet.getColor().Equals(color))
                {
                    bulletIndex = INACTIVE;
                    ScreenManager.game.World.removeObject(bullet);
                }
            }
        }
    }
}