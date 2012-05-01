using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;

namespace Resonance
{
    /// <summary>
    /// This class handles the motion of the game camera.
    /// </summary>
    class CameraMotionManager
    {
        private static ChaseCamera camera;

        public const int DEFAULT = 0;
        public const int CLOSE = 1;
        public const int TOPDOWN = 2;

        private static Vector3 defaultPos  = new Vector3(0,   10f,  20f);
        private static Vector3 closePos    = new Vector3(0,    3f,   4f);
        private static Vector3 topDownPos  = new Vector3(0,   40f,   1f);
        private static Vector3 startCamPos = new Vector3(20f, 20f, -20f);
        private static Vector3 boostingPos = new Vector3(0, 9f, 25f);

        private const float ZOOM_RATE_NORMAL = 0.01f;
        private const float ZOOM_RATE_START = 0.05f;
        private static float ZOOM_RATE = ZOOM_RATE_START;

        private static List<int> views = new List<int>{ DEFAULT, CLOSE, TOPDOWN };

        private static int view = 0;
        private static Vector3 currentPosition;
        private static Vector3 targetPosition;

        private static bool starting;

        public static void initCamera()
        {
            camera = new ChaseCamera(startCamPos);
            starting = true;
        }

        public static void initialise()
        {
            currentPosition = startCamPos;
            camera.ChaseObject = GameScreen.getGV().Body;
            updateCamera();
        }

        public static void update(InputDevices input)
        {
            KeyboardState lastKbd = input.LastKeys;
            KeyboardState kbd = input.Keys;
            GamePadState lastPad = input.LastPlayerOne;
            GamePadState pad = input.PlayerOne;
            
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

            updateCamera();
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
            else if (starting && targetPosition == currentPosition)
            {
                starting = false;
                ZOOM_RATE = ZOOM_RATE_NORMAL;
            }

            //Check if camera is going through wall here


            camera.update(currentPosition);
        }

        private static void defaultGV()
        {
            if (GVMotionManager.BOOSTING) targetPosition = boostingPos;
            else targetPosition = defaultPos;
        }

        private static void closeGV()
        {
            targetPosition = closePos;
        }

        private static void topDownGV()
        {
            targetPosition = topDownPos;
        }

        public static ChaseCamera Camera
        {
            get
            {
                return camera;
            }
        }
    }
}
