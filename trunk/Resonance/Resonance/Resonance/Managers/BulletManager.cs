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
        private static long COUNTER = 90000000;
        private static DateTime startTime = DateTime.Now;
        private static Random Rand = new Random();
        private static int CHUNK = 15;
        private static TimeSpan seconds;
        private static Texture2D zero = null;
        private static Texture2D one = null;
        private static Texture2D two = null;
        private static Texture2D three = null;
        private static Texture2D four = null;
        private static Texture2D five = null;
        private static Texture2D six = null;
        private static Texture2D seven = null;
        private static Texture2D eight = null;
        private static Texture2D nine = null;
        public static ContentManager content = null;      
        private static string[] colors = new string[] { "red", "blue", "green", "yellow", "cymbal"};
        private static int index;


        public static void init(ContentManager newCont)
        {
            content = newCont;
            zero = content.Load<Texture2D>("Drawing/Textures/zero");
            one = content.Load<Texture2D>("Drawing/Textures/one");
            two = content.Load<Texture2D>("Drawing/Textures/two");
            three = content.Load<Texture2D>("Drawing/Textures/three");
            four = content.Load<Texture2D>("Drawing/Textures/four");
            five = content.Load<Texture2D>("Drawing/Textures/five");
            six = content.Load<Texture2D>("Drawing/Textures/six");
            seven = content.Load<Texture2D>("Drawing/Textures/seven");
            eight = content.Load<Texture2D>("Drawing/Textures/eight");
            nine = content.Load<Texture2D>("Drawing/Textures/nine");
        }
        public static void shoot(Vector3 bulPos)
         {
             if (bulletIndex == INACTIVE && DateTime.Now.Ticks - startTime.Ticks > COUNTER)
             {
                 bulletIndex = ACTIVE;
                 bulletPosition = bulPos;                
                 ScreenManager.game.World.addObject(bullet);                
                 index = Rand.Next();
                 DebugDisplay.update("Random ", index.ToString());
                 DebugDisplay.update("Random 2", (index % 5).ToString());
                 bullet.setColor(colors[index % 5]);
                 bullet.ModelInstance.setTexture(index % 5);
             }
       }


        public static void draw(SpriteBatch spriteBatch)
        {
            if (bulletIndex == INACTIVE)
            {
                seconds = DateTime.Now - startTime;
                if (content != null)
                {

                    DebugDisplay.update("Seconds left until attack", ((int)(9 - seconds.TotalSeconds)).ToString());
                    if(((int)(9 - seconds.TotalSeconds)) == 0)
                        spriteBatch.Draw(zero, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 1)
                        spriteBatch.Draw(one, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 2)
                        spriteBatch.Draw(two, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 3)
                        spriteBatch.Draw(three, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 4)
                        spriteBatch.Draw(four, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 5)
                        spriteBatch.Draw(five, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 6)
                        spriteBatch.Draw(six, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 7)
                        spriteBatch.Draw(seven, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 8)
                        spriteBatch.Draw(eight, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(5)), Color.White);
                    if (((int)(9 - seconds.TotalSeconds)) == 9)
                        spriteBatch.Draw(nine, new Vector2(ScreenManager.pixelsX(890), ScreenManager.pixelsY(200)), Color.White);

                }
            }
        }


         public static void updateBossPosition(Vector3 position)
         {
             bossPosition = position;
         }


       public static void updateBullet()
         {
             iteration = ScreenManager.game.Iteration;
             if (bulletIndex == ACTIVE)
             {
                if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 5f )
                 {
                     //GameScreen.getGV().AdjustHealth(-DAMAGE);
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
                     bullet.setColor(colors[index % 5]);
                     bullet.ModelInstance.setTexture(index % 5);
                     ScreenManager.game.World.addObject(bullet);
                 }   
                
             }
             if (bulletIndex == INACTIVE)
             {
                 shoot(bossPosition);
             }
         }
        
        public static void destroyBullet(string color)
        {
           
            if (Vector3.Distance(bulletPosition, GameScreen.getGV().Body.Position) < 25f && bullet.getColor().Equals(color))
            {
                bulletIndex = INACTIVE;
                ScreenManager.game.World.removeObject(bullet);               
            }
        }
    }
}