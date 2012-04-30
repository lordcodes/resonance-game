using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace Resonance
{
    class DrumManager
    {
        private static int healthCount = 0;
        private static int speedCount = 0;
        private static int shieldCount = 0;
        private static int freezeCount = 0;

        public static void input(InputDevices input)
        {
            KeyboardState kbd = input.Keys;
            KeyboardState lastKbd = input.LastKeys;
            GamePadState pad = input.PlayerTwo;
            GamePadState lastPad = input.LastPlayerTwo;

            bool green = (pad.Buttons.A == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.A)) || 
                         (kbd.IsKeyDown(Keys.V) && !lastKbd.IsKeyDown(Keys.V));
            bool yellow = (pad.Buttons.Y == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.Y)) ||
                          (kbd.IsKeyDown(Keys.X) && !lastKbd.IsKeyDown(Keys.X));
            bool blue = (pad.Buttons.X == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.X)) ||
                        (kbd.IsKeyDown(Keys.C) && !lastKbd.IsKeyDown(Keys.C));
            bool red = (pad.Buttons.B == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.B)) ||
                       (kbd.IsKeyDown(Keys.Z) && !lastKbd.IsKeyDown(Keys.Z));
            bool cymbal = (pad.Buttons.LeftShoulder == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.LeftShoulder)) ||
                          (kbd.IsKeyDown(Keys.B) && !lastKbd.IsKeyDown(Keys.B)) ||
                          (pad.Buttons.RightShoulder == ButtonState.Pressed && !lastPad.IsButtonDown(Buttons.RightShoulder));

            if (green)
            {
                MusicHandler.playSound(MusicHandler.GREEN);
                MusicHandler.getTrack().inTime();
                increaseHealth();
                GameScreen.getGV().createShockwave(Shockwave.GREEN);
                BulletManager.destroyBullet("green");
                
            }
            if (yellow)
            {
                MusicHandler.playSound(MusicHandler.YELLOW);
                MusicHandler.getTrack().inTime();
                increaseSpeed();
                GameScreen.getGV().createShockwave(Shockwave.YELLOW);
                BulletManager.destroyBullet("yellow");
            }
            if (blue)
            {
                MusicHandler.playSound(MusicHandler.BLUE);
                MusicHandler.getTrack().inTime();
                increaseShield();
                GameScreen.getGV().createShockwave(Shockwave.BLUE);
                BulletManager.destroyBullet("blue");
            }
            if (red)
            {
                MusicHandler.playSound(MusicHandler.RED);
                MusicHandler.getTrack().inTime();
                increaseFreeze();
                GameScreen.getGV().createShockwave(Shockwave.RED);
                BulletManager.destroyBullet("red");
            }
            if (cymbal)
            {
                MusicHandler.playSound(MusicHandler.CYMBAL);
                MusicHandler.getTrack().inTime();
                GameScreen.getGV().createShockwave(Shockwave.CYMBAL);
                BulletManager.destroyBullet("cymbal");
            }


            if (green | blue | yellow | red | cymbal) DebugDisplay.update("In time", MusicHandler.getTrack().inTime().ToString());

            //if (green | blue | yellow | red | cymbal) Program.game.Music.getTrack().inTime2();
            lastPad = pad;
            lastKbd = kbd;
        }

        public static void increaseHealth()
        {
            if (GameScreen.getGV().InCombat == false)
            {
                healthCount++;
                if (healthCount > 8)
                {
                    GameScreen.getGV().AdjustHealth(3);
                    healthCount = 0;
                }
            }
            else
            {
                healthCount = 0;
            }
        }

        public static void increaseSpeed()
        {
            if (GameScreen.getGV().InCombat == false)
            {
                speedCount++;
                if (speedCount > 4)
                {
                    GameScreen.getGV().adjustNitro(10);
                    speedCount = 0;
                }
            }
            else
            {
                speedCount = 0;
            }
        }
        public static void increaseShield()
        {
            if (GameScreen.getGV().InCombat == false)
            {
                shieldCount++;
                if (shieldCount > 4)
                {
                    GameScreen.getGV().adjustShield(10);
                    shieldCount = 0;
                }
            }
            else
            {
                shieldCount = 0;
            }
        }
        public static void increaseFreeze()
        {
            if (GameScreen.getGV().InCombat == false)
            {
                freezeCount++;
                if (freezeCount > 4)
                {
                    GameScreen.getGV().adjustFreeze(5);
                    freezeCount = 0;
                }
            }
            else
            {
                freezeCount = 0;
            }
        }
    }
}
