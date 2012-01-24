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

            CameraMotionManager.update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            GVMotionManager.input(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
        }
    }
}
