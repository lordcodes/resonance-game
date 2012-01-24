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
        public static int healthCount = 0;

        public static void Input(GamePadState playerTwo, GamePadState oldPadState2, MusicHandler musicHandler, World world, KeyboardState keyboardState, KeyboardState oldKeyState)
        {
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                musicHandler.getTrack().playTrack();
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                musicHandler.getTrack().stopTrack();
            }
            if (keyboardState.IsKeyDown(Keys.P))
            {
                musicHandler.getTrack().pauseTrack();
            }
            if (keyboardState.IsKeyDown(Keys.M))
            {
                MiniMap.large = true;
            }
            else
            {
                MiniMap.large = false;
            }
            if ((playerTwo.Buttons.A == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.A)) || 
                (keyboardState.IsKeyDown(Keys.V) && !oldKeyState.IsKeyDown(Keys.V)))
            {
                musicHandler.playSound("TomLow");
                musicHandler.getTrack().inTime();
                increaseHealth();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.GREEN);
            }
            if ((playerTwo.Buttons.Y == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.Y)) ||
                (keyboardState.IsKeyDown(Keys.X) && !oldKeyState.IsKeyDown(Keys.X)))
            {
                musicHandler.playSound("TomHigh");
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.YELLOW);
            }
            if ((playerTwo.Buttons.X == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.X)) ||
                (keyboardState.IsKeyDown(Keys.C) && !oldKeyState.IsKeyDown(Keys.C)))
            {
                musicHandler.playSound("TomMiddle");
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.BLUE);
            }
            if ((playerTwo.Buttons.B == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.B)) ||
                (keyboardState.IsKeyDown(Keys.Z) && !oldKeyState.IsKeyDown(Keys.Z)))
            {
                musicHandler.playSound("Snare");
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.RED);
            }
            /*if (playerTwo.Buttons.RightStick == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.RightStick))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            if (playerTwo.Buttons.LeftStick == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.LeftStick))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            */
            if ((playerTwo.Buttons.LeftShoulder == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.LeftShoulder)) ||
                (keyboardState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B)))
            {
                musicHandler.playSound("Crash");
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            if ((playerTwo.Buttons.RightShoulder == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.RightShoulder)) ||
                (keyboardState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B)))
            {
                musicHandler.playSound("Crash");
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
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
    }
}
