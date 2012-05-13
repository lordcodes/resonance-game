using System;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
namespace Resonance
{
   
    static class BulletManager
    {
        public static bool BOSS_EXISTS = false;

        public  const int  INACTIVE = 0;
        public  const int  ACTIVE   = 1;
        private const int  DAMAGE   = 4;
        private const long TIMESPAN = 150000000;
        private const int  CHUNK    = 15;     

        private static int bulletIndex;
        private static Bullet bullet;
        private static DateTime beatTimeBefore;
        private static DateTime timeNow;
        private static Random rand;
        private static Entity target;
        private static Entity start;

        public static void init()
        {
            bulletIndex = INACTIVE;
            beatTimeBefore = DateTime.Now;
            rand = new Random();
            target = GameScreen.getGV().Body;
            start = GameScreen.getBoss().Body;
            bullet = new Bullet(17, "activeBullet", start.Position);
        }

        public static void shoot()
        {
                bulletIndex = ACTIVE;
                bullet.Position = start.Position;  
                ScreenManager.game.World.addObject(bullet);                
                int r = rand.Next();
                bullet.Colour = r % 4;
        }

        public static void updateBullet()
        {
            if (BOSS_EXISTS)
            {
                if (bulletIndex == ACTIVE)
                {
                    if (Vector3.Distance(bullet.Position, target.Position) < 0.5f)
                    {
                        GameScreen.getGV().AdjustHealth(-DAMAGE);
                        Drawing.addWave(bullet.Position);
                        bulletIndex = INACTIVE;
                    }
                    else
                    {
                        timeNow = DateTime.Now;
                        long ticks = timeNow.Ticks - beatTimeBefore.Ticks;
                        Vector3 dir = target.Position - bullet.Position;
                        if (ticks >= TIMESPAN)
                        {
                            beatTimeBefore = DateTime.Now;
                            bullet.Position += dir;
                        }
                        else
                        {
                            bullet.Position += dir / CHUNK;
                        }
                    }

                }
                else if (bulletIndex == INACTIVE)
                {
                    shoot();
                }
            }
        }
        
        public static void destroyBullet(int colour)
        {
            if (BOSS_EXISTS)
            {
                if (Vector3.Distance(bullet.Position, target.Position) <= 30f && bullet.Colour == colour)
                {
                    bulletIndex = INACTIVE;
                    ScreenManager.game.World.removeObject(bullet);
                }
            }
        }

        public static int getBulletColour() {
            return bullet.Colour;
        }
    }
}