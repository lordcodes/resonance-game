using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class MenuControlManager
    {
        public static GamePadState lastPad;
        public static KeyboardState lastKbd;

        public static void input(GamePadState pad, KeyboardState kbd)
        {
            bool up = (!lastKbd.IsKeyDown(Keys.Up) && lastPad.DPad.Up != ButtonState.Pressed) &&
                      (kbd.IsKeyDown(Keys.Up) || pad.DPad.Up == ButtonState.Pressed);
            bool down = (!lastKbd.IsKeyDown(Keys.Down) && lastPad.DPad.Down != ButtonState.Pressed) &&
                        (kbd.IsKeyDown(Keys.Down) || pad.DPad.Down == ButtonState.Pressed);
            bool select = (!lastKbd.IsKeyDown(Keys.Enter) && lastPad.Buttons.A != ButtonState.Pressed) &&
                          (kbd.IsKeyDown(Keys.Enter) || pad.Buttons.A == ButtonState.Pressed);
            bool pause = (!lastKbd.IsKeyDown(Keys.Escape) && lastPad.Buttons.Start != ButtonState.Pressed) &&
                         (kbd.IsKeyDown(Keys.Escape) || pad.Buttons.Start == ButtonState.Pressed);
            if (pause)
            {
                    if (UI.Paused) UI.play();
                    else UI.pause();
            }

            if (UI.Paused)
            {
                if (up) UI.moveUp();
                if (down) UI.moveDown();
                if (select) UI.select();
            }

            lastPad = pad;
            lastKbd = kbd;
        }
    }
}
