using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Resonance
{
    /// <summary>
    /// This class handles the motion of the game camera.
    /// </summary>
    class CameraMotionManager
    {
        public const int DEFAULT = 0;
        public const int CLOSE = 1;
        public const int TOPDOWN = 2;

        private static Vector3 defaultPos = new Vector3(0, 8f, 14f);
        private static Vector3 closePos = new Vector3(0, 4f, 8f);
        private static Vector3 topDownPos = new Vector3(0, 25f, 1f);

        private static float ZOOM_RATE = 0.01f;

        private static List<int> views = new List<int>{ DEFAULT, CLOSE, TOPDOWN };

        private static int view = 0;
        private static Vector3 currentPosition = defaultPos;
        private static Vector3 targetPosition = defaultPos;

        private static KeyboardState lastKbd;
        private static GamePadState lastPad;

        public static void update(KeyboardState kbd, GamePadState pad)
        {
            if (!lastKbd.IsKeyDown(Keys.Back) && !(lastPad.Buttons.Back == ButtonState.Pressed))
            {
                if (kbd.IsKeyDown(Keys.Back) || (pad.Buttons.Back == ButtonState.Pressed))
                {
                    view++;
                    if (view >= views.Count) view = 0;
                }
            }
            if (views[view] == DEFAULT) defaultGV();
            else if (views[view] == CLOSE) closeGV();
            else if (views[view] == TOPDOWN) topDownGV();

            lastKbd = kbd;
            lastPad = pad;
        }

        

        /// <summary>
        /// Updates Camera and HUD
        /// </summary>
        /// <param name="player">The good vibe class</param>
        private static void updateCamera()
        {
            if (targetPosition.X != currentPosition.X || targetPosition.Y != currentPosition.Y || targetPosition.Z != currentPosition.Z)
            {
                Vector3 left = targetPosition - currentPosition;
                currentPosition += (left * ZOOM_RATE);
            }
            Drawing.UpdateCamera(((GoodVibe)Program.game.World.getObject("Player")).Body.Position, currentPosition);
        }

        private static void topDownGV()
        {
            targetPosition = topDownPos;
            updateCamera();
        }

        private static void defaultGV()
        {
            targetPosition = defaultPos;
            updateCamera();
        }

        private static void closeGV()
        {
            targetPosition = closePos;
            updateCamera();
        }
    }
}
