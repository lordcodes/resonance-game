using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.MathExtensions;
using BEPUphysics;

namespace Resonance
{
    class ChaseCamera
    {
        Vector3 position;
        Matrix view;
        Entity chaseObject;

        public ChaseCamera(Vector3 startPos)
        {
            position = startPos;
            view = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
            rayCastFilter = RayCastFilter;
        }

        private Vector3 calcCamera(Vector3 newPosition)
        {
            /*Vector3 rotation = Utility.QuaternionToEuler(chaseObject.Orientation);
            Vector3 position = chaseObject.Position;
            Matrix rot = Matrix.CreateRotationY(rotation.Y);
            return Vector3.Transform(newPosition, rot) + position;*/

            Vector3 cameraPos = Matrix3X3.Transform(newPosition, chaseObject.BufferedStates.InterpolatedStates.OrientationMatrix);
            cameraPos += chaseObject.BufferedStates.InterpolatedStates.Position;

            Vector3 facing = Vector3.Normalize(cameraPos - chaseObject.Position);
            float distance = Vector3.Distance(cameraPos, chaseObject.Position);

            Vector3 position;
            RayCastResult result;
            if (chaseObject.Space.RayCast(new Ray(chaseObject.Position, facing), rayCastFilter, out result))
            {
                position = result.HitData.Location;
            }
            else
            {
                position = cameraPos;
            }
            return position;
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void update(Vector3 newPosition)
        {
            Vector3 oldPos = position;
            position = Vector3.SmoothStep(oldPos, calcCamera(newPosition), 0.2f);
            view = Matrix.CreateLookAt(position, chaseObject.Position, Vector3.Up);
        }

        Func<BroadPhaseEntry, bool> rayCastFilter;
        bool RayCastFilter(BroadPhaseEntry entry)
        {
            return entry != chaseObject.CollisionInformation && (entry.CollisionRules.Personal <= CollisionRule.Normal);
        }

        public Matrix View
        {
            get { return view; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Entity ChaseObject
        {
            set { chaseObject = value; }
        }

    }
}
