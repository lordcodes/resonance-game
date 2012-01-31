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

        public static GamePadState lastPad;
        public static KeyboardState lastKbd;

        public static void input(GamePadState pad, KeyboardState kbd)
        {
            if ((kbd.IsKeyDown(Keys.Q) && !lastKbd.IsKeyDown(Keys.Q)) ||
                (pad.Buttons.LeftShoulder == ButtonState.Pressed && lastPad.Buttons.LeftShoulder != ButtonState.Pressed))
            {
                Drawing.DoDisp = true;
                Drawing.addWave(Game.getGV().Body.Position);
            }
            if ((kbd.IsKeyDown(Keys.L) && !lastKbd.IsKeyDown(Keys.L)))
            {
                Game.getGV().showBeat();
            }
            if ((pad.Buttons.Start == ButtonState.Pressed) || (kbd.IsKeyDown(Keys.Space)))
            {
                Program.game.Music.getTrack().playTrack();
            }
            if ((pad.Buttons.A == ButtonState.Pressed) || kbd.IsKeyDown(Keys.S))
            {
                Program.game.Music.getTrack().stopTrack();
            }
            if (pad.Buttons.B == ButtonState.Pressed || kbd.IsKeyDown(Keys.P))
            {
                Program.game.Music.getTrack().pauseTrack();
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
                Game.getGV().selectedPower = SHIELD;
            }

            if (pad.Buttons.Y == ButtonState.Pressed || kbd.IsKeyDown(Keys.K))
            {
                Game.getGV().selectedPower = NITROUS;
            }

            if (pad.Buttons.B == ButtonState.Pressed || kbd.IsKeyDown(Keys.L))
            {
                Game.getGV().selectedPower = FREEZE;
            }

            if (pad.Triggers.Right > 0.1 || kbd.IsKeyDown(Keys.T))
            {
               usePower(Game.getGV().selectedPower);
            }
            else //if ((pad.Triggers.Right > 0 && pad.Triggers.Right < 0.1) || kbd.IsKeyUp(Keys.T))
            {
                Game.getGV().shieldDown();
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

                if (Game.getGV().Shield > 0)
                {
                    Game.getGV().adjustShield(-1);
                    Game.getGV().shieldUp();
                }
                else
                {
                    Game.getGV().shieldDown();
                }
            }

            if (power == FREEZE)
            {
                if (Game.getGV().Freeze > 0)
                {
                    Game.getGV().freezeBadVibes();
                    Game.getGV().adjustFreeze(-1);
                }

            }

            if (power == NITROUS)
            {
               
                if (Game.getGV().Nitro > 0.1)
                {
                    Game.getGV().adjustNitro(-1);
                    //GVMotionManager.boost();
                    GVMotionManager.BOOSTING = true;
                }
                
            }
        }
    }
}
