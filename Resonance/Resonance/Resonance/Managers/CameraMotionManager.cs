using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    /// <summary>
    /// This class handles the motion of the game camera.
    /// </summary>
    class CameraMotionManager
    {
        public static void trackGV(KeyboardState kbd) {
            if (!kbd.IsKeyDown(Keys.RightShift)) {
                Drawing.UpdateCamera((GoodVibe) Program.game.World.getObject("Player"));
            }
        }
    }
}
