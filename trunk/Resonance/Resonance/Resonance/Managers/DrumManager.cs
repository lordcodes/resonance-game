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

        public static GamePadState lastPad;
        public static KeyboardState lastKbd;

        public static void input(GamePadState pad, KeyboardState kbd)
        {
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
                Program.game.Music.playSound(MusicHandler.GREEN);
                Program.game.Music.getTrack().inTime();
                increaseHealth();
                Game.getGV().createShockwave(Shockwave.GREEN);
            }
            if (yellow)
            {
                Program.game.Music.playSound(MusicHandler.YELLOW);
                Program.game.Music.getTrack().inTime();
                increaseSpeed();
                Game.getGV().createShockwave(Shockwave.YELLOW);
            }
            if (blue)
            {
                Program.game.Music.playSound(MusicHandler.BLUE);
                Program.game.Music.getTrack().inTime();
                increaseShield();
                Game.getGV().createShockwave(Shockwave.BLUE);
            }
            if (red)
            {
                Program.game.Music.playSound(MusicHandler.RED);
                Program.game.Music.getTrack().inTime();
                increaseFreeze();
                Game.getGV().createShockwave(Shockwave.RED);
            }
            if (cymbal)
            {
                Program.game.Music.playSound(MusicHandler.CYMBAL);
                Program.game.Music.getTrack().inTime();
                Game.getGV().createShockwave(Shockwave.CYMBAL);
            }

            if (green | blue | yellow | red | cymbal) DebugDisplay.update("In time", Program.game.Music.getTrack().inTime().ToString());

            lastPad = pad;
            lastKbd = kbd;
        }

        public static void increaseHealth()
        {
            if (Game.getGV().InCombat == false)
            {
                healthCount++;
                if (healthCount > 4)
                {
                    Game.getGV().AdjustHealth(5);
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
            if (Game.getGV().InCombat == false)
            {
                speedCount++;
                if (speedCount > 4)
                {
                    Game.getGV().adjustNitro(10);
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
            if (Game.getGV().InCombat == false)
            {
                shieldCount++;
                if (shieldCount > 4)
                {
                    Game.getGV().adjustShield(5);
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
            if (Game.getGV().InCombat == false)
            {
                freezeCount++;
                if (freezeCount > 4)
                {
                    Game.getGV().adjustFreeze(5);
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
