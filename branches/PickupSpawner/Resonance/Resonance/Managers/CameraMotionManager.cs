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
        private static Vector3 currentTarget;

        public static KeyboardState lastKbd;
        public static GamePadState lastPad;

        private static bool targetSet = false;

        public static void update(GamePadState pad, KeyboardState kbd)
        {
            if (!targetSet)
            {
                currentTarget = Game.getGV().Body.Position;
                targetSet = true;
            }

            
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

            updateCamera(currentTarget);

            lastKbd = kbd;
            lastPad = pad;
        }

        /// <summary>
        /// Updates Camera and HUD
        /// </summary>
        /// <param name="player">The good vibe class</param>
        private static void updateCamera(Vector3 toFace)
        {
            if (targetPosition.X != currentPosition.X || targetPosition.Y != currentPosition.Y || targetPosition.Z != currentPosition.Z)
            {
                Vector3 left = targetPosition - currentPosition;
                currentPosition += (left * ZOOM_RATE);
            }
            Drawing.UpdateCamera(toFace, currentPosition);
        }

        private static void defaultGV()
        {
            currentTarget = Game.getGV().Body.Position;
            targetPosition = defaultPos;
        }

        private static void closeGV()
        {
            currentTarget = Game.getGV().Body.Position;
            targetPosition = closePos;
        }

        private static void topDownGV()
        {
            currentTarget = Game.getGV().Body.Position;
            targetPosition = topDownPos;
        }

        private static void nearestBV()
        {
            /*double closest = Double.PositiveInfinity;
            Vector3 closestPoint = currentTarget;
            Dictionary<string, Object> objects = Program.game.World.returnObjects();
            foreach (KeyValuePair<string, Object> pair in objects)
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe bv = (BadVibe)pair.Value;
                    double dx = Game.getGV().Body.Position.X - bv.Body.Position.X;
                    double dz = Game.getGV().Body.Position.Z - bv.Body.Position.Z;
                    double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                    d = Math.Sqrt(d);

                    if (d < closest)
                    {
                        closest = d;
                        closestPoint = bv.Body.Position;
                    }
                }
            }
            currentTarget = closestPoint;*/
        }
    }
}
