using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics;

namespace Resonance
{
    class Boss : DynamicObject
    {
        //Number of deflected bullets to kill
        public const int MAX_HEALTH = 20;

        private int health;

        private SingleEntityAngularMotor servo;

        public Boss(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            servo = new SingleEntityAngularMotor(Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            ScreenManager.game.World.addToSpace(servo);

            health = MAX_HEALTH;
        }

        /// <summary>
        /// Moves the bad vibe in the world
        /// </summary>
        public void faceGV()
        {
            Vector3 dir = Body.OrientationMatrix.Forward;
            Vector3 pos = Body.Position;
            Vector3 gvPos = GameScreen.getGV().Body.Position;
            Vector3 diff = Vector3.Normalize(gvPos - pos);
            Quaternion rot;
            Toolbox.GetQuaternionBetweenNormalizedVectors(ref dir, ref diff, out rot);
            rot.X = 0;
            rot.Z = 0;
            servo.Settings.Servo.Goal = Quaternion.Concatenate(Body.Orientation, rot);
        }

        /// <summary>
        /// Damage the bad vibe
        /// </summary>
        /// <param name="colour">The colour wave that has been attacked with</param>
        public bool damage(bool deflect)
        {
            if (deflect)
            {
                if(health > 0) health--;
            }
            if (health <= 0)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        public int getHealth() {
            return health;
        }
    }
}
