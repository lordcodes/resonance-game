using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class ChaseCamera
    {
        Vector3 position;
        Matrix view;

        public ChaseCamera(Vector3 startPos)
        {
            position = startPos;
            view = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
        }

        private Vector3 getCamPos(Vector3 newPosition)
        {
            Quaternion orientation = GameScreen.getGV().Body.Orientation;
            Vector3 rotation = Utility.QuaternionToEuler(orientation);
            Vector3 position = GameScreen.getGV().Body.Position;
            Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
            return Vector3.Transform(newPosition, goodVibeRotation) + position;
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void update(Vector3 newPosition)
        {
            Vector3 oldPos = position;
            position = Vector3.SmoothStep(oldPos, getCamPos(newPosition), 0.2f);
            Vector3 gvPos = GameScreen.getGV().Body.Position;
            view = Matrix.CreateLookAt(position, gvPos, Vector3.Up);
        }

        public Matrix View
        {
            get
            {
                return view;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

    }
}
