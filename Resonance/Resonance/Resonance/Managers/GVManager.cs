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

        public static void inputs(GamePadState playerOne, MusicHandler musicHandler, KeyboardState keyboardState)
        {
            
            if ((playerOne.Buttons.Start == ButtonState.Pressed) ||
                (keyboardState.IsKeyDown(Keys.Space)))
            {
                musicHandler.getTrack().playTrack();
            }
            if ((playerOne.Buttons.A == ButtonState.Pressed) ||
                keyboardState.IsKeyDown(Keys.S))
            {
                musicHandler.getTrack().stopTrack();
            }
            if (playerOne.Buttons.B == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.P))
            {
                musicHandler.getTrack().pauseTrack();
            }
            if (playerOne.Buttons.LeftShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.M))
            {
                MiniMap.enlarge();
            }
            else
            {
                MiniMap.ensmall();
            }

            if (playerOne.Buttons.X == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.J))
            {
                Game.getGV().selectedPower = SHIELD;
            }

            if (playerOne.Buttons.Y == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.K))
            {
                Game.getGV().selectedPower = NITROUS;
            }

            if (playerOne.Buttons.B == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.L))
            {
                Game.getGV().selectedPower = FREEZE;
            }

            if (playerOne.Triggers.Right > 0.1 || keyboardState.IsKeyDown(Keys.T))
            {
               usePower(Game.getGV().selectedPower);
            }
            else
            {
                GVMotionManager.Z_ACCELERATION = GVMotionManager.DEFAULT_Z_ACCELERATION;
                GVMotionManager.MAX_Z_SPEED = GVMotionManager.DEFAULT_MAX_Z_SPEED;
            }
            CameraMotionManager.update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            GVMotionManager.input(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
        }

        public static void usePower(int power)
        {
            if (power == SHIELD)
            {
                Game.getGV().adjustShield(-1);
                if (Game.getGV().Shield > 0)
                {
                    
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
                    GVMotionManager.boost(); 
                }
                
            }
        }
    }
}
