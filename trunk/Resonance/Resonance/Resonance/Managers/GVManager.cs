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
    class GVManager
    {

        public static readonly int NITROUS = 0;
        public static readonly int SHIELD = 1;
        public static readonly int FREEZE = 2;
        

        public static void input(InputDevices input)
        {
            KeyboardState kbd = input.Keys;
            KeyboardState lastKbd = input.LastKeys;
            GamePadState pad = input.PlayerOne;
            GamePadState lastPad = input.LastPlayerOne;

            if ((kbd.IsKeyDown(Keys.Q) && !lastKbd.IsKeyDown(Keys.Q)) ||
                (pad.Buttons.LeftShoulder == ButtonState.Pressed && lastPad.Buttons.LeftShoulder != ButtonState.Pressed))
            {
                Drawing.DoDisp = true;
                Drawing.addWave(GameScreen.getGV().Body.Position);
            }
            if ((kbd.IsKeyDown(Keys.L) && !lastKbd.IsKeyDown(Keys.L)))
            {
                GameScreen.getGV().showBeat();
            }
            if ((pad.Buttons.Back == ButtonState.Pressed) || (kbd.IsKeyDown(Keys.Space)))
            {
                GameScreen.musicHandler.getTrack().playTrack();
            }
            if ((pad.Buttons.RightShoulder == ButtonState.Pressed) || kbd.IsKeyDown(Keys.S))
            {
                GameScreen.musicHandler.getTrack().stopTrack();
            }
            if (kbd.IsKeyDown(Keys.P))
            {
                GameScreen.musicHandler.getTrack().pauseTrack();
            }
            if (pad.Buttons.LeftShoulder == ButtonState.Pressed || kbd.IsKeyDown(Keys.M))
            {
                MiniMap.enlarge();
            }
            else
            {
                MiniMap.ensmall();
            }

            if (pad.Buttons.X == ButtonState.Pressed || kbd.IsKeyDown(Keys.J))
            {
                
                GameScreen.getGV().selectedPower = SHIELD;
            }

            if (pad.Buttons.Y == ButtonState.Pressed || kbd.IsKeyDown(Keys.K))
            {
                GameScreen.getGV().selectedPower = NITROUS;
            }

            if (pad.Buttons.B == ButtonState.Pressed || kbd.IsKeyDown(Keys.L))
            {
                GameScreen.getGV().selectedPower = FREEZE;
            }

            if (pad.Triggers.Right > 0.1 || kbd.IsKeyDown(Keys.T))
            {
               usePower(GameScreen.getGV().selectedPower);
            }
            else //if ((pad.Triggers.Right > 0 && pad.Triggers.Right < 0.1) || kbd.IsKeyUp(Keys.T))
            {
                GameScreen.getGV().shieldDown();
                GVMotionManager.resetBoost();
            }
            GVMotionManager.input(kbd, pad);

            lastPad = pad;
            lastKbd = kbd;
        }

        public static void usePower(int power)
        {
            if (power == SHIELD)
            {

                if (GameScreen.getGV().Shield > 0)
                {
                    GameScreen.getGV().freezeHealth(true);
                    GameScreen.getGV().adjustShield(-1);
                    GameScreen.getGV().shieldUp();
                }
                else
                {
                    GameScreen.getGV().freezeHealth(false);
                    GameScreen.getGV().shieldDown();
                }
            }

            if (power == FREEZE)
            {
                if (GameScreen.getGV().Freeze > 0)
                {
                    GameScreen.getGV().FreezeActive = true;
                    GameScreen.getGV().adjustFreeze(-1);
                }

            }

            if (power == NITROUS)
            {
               
                if (GameScreen.getGV().Nitro > 0.1)
                {
                    GameScreen.getGV().adjustNitro(-1);
                    GVMotionManager.BOOSTING = true;
                    GameScreen.stats.usedNitro();
                }
                
            }
        }
    }
}
