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
        private const int WRONG_DAMAGE = 2;
        //private const long TIMESPAN = 15000000;
        private const long TIMESPAN = 72500000;
        private const double DEFAULT_CHUNK = 0.0000002;

        private static double CHUNK = DEFAULT_CHUNK;    

        private static int bulletIndex;
        private static Bullet bullet;
        private static Random rand;
        private static Entity target;
        private static Entity start;
        private static TimeSpan timeAlive;

        public static void init()
        {
            bulletIndex = INACTIVE;
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
            timeAlive = TimeSpan.Zero;
            CHUNK = DEFAULT_CHUNK;
        }

        public static void updateBullet(GameTime gameTime)
        {
            if (BOSS_EXISTS)
            {
                if (bulletIndex == ACTIVE)
                {
                    if (Vector3.Distance(bullet.Position, target.Position) < 3f)
                    {
                        GameScreen.getGV().AdjustHealth(-DAMAGE);
                        bulletIndex = INACTIVE;
                    }
                    else
                    {
                        timeAlive += gameTime.ElapsedGameTime;
                        Vector3 dir = target.Position - bullet.Position;
                        float dist = Vector3.Distance(bullet.Position, target.Position);
                        dir.Normalize();

                        long ticks = gameTime.ElapsedGameTime.Ticks;

                        bullet.Position += dir * (float)(dist * CHUNK * ticks);

                        if (timeAlive.TotalSeconds > 1.6) CHUNK *= 1.05;
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
                if (Vector3.Distance(bullet.Position, target.Position) <= 30f && bullet.Colour != colour)
                {
                    GameScreen.getGV().AdjustHealth(-WRONG_DAMAGE);
                }
            }
        }

        public static int getBulletColour() {
            return bullet.Colour;
        }
    }
}