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
        public const int MAX_HEALTH = 4;

        private int health;
        private bool dead;

        private SingleEntityAngularMotor servo;

        public Boss(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            servo = new SingleEntityAngularMotor(Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            ScreenManager.game.World.addToSpace(servo);

            health = MAX_HEALTH;
            dead = false;
        }

        /// <summary>
        /// Moves the bad vibe in the world
        /// </summary>
        public void faceGV()
        {
            Vector3 dir = Body.OrientationMatrix.Backward;
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
        public void damage(bool deflect)
        {
            if (deflect)
            {
                if(health > 0) health --;
                BulletImpact e = (BulletImpact) ParticleEmitterManager.getEmitter<BulletImpact>();
                if (e == null) e = new BulletImpact();
                Vector3 bPos = BulletManager.getBullet().Body.Position;
                e.init(bPos, bPos - Body.Position);
            }
            if (health <= 0)
            {
                dead = true;
            }
        }

        public int Health
        {
            get { return health; }
        }

        public bool Dead
        {
            get { return dead; }
        }
    }
}
