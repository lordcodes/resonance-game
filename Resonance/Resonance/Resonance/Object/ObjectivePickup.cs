using System;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance
{
    class ObjectivePickup : DynamicObject
    {
        private SingleEntityAngularMotor servo;
        private float rotateSpeed = -1f;
        Random r;

        public ObjectivePickup(int modelNum, String name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            servo = new SingleEntityAngularMotor(this.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.SpringSettings.DampingConstant *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;

            r = new Random((int)(DateTime.Now.Ticks));
            float angle = MathHelper.ToRadians(r.Next(0, 360));

            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            servo.Settings.Servo.Goal = orientation;
            ScreenManager.game.World.addToSpace(servo);
        }

        public void init(int modelNum, Vector3 pos, int length, int time)
        {
            Body.Position = pos;

            float angle = MathHelper.ToRadians(r.Next(0, 360));

            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            servo.Settings.Servo.Goal = orientation;
        }

        /// <summary>
        /// Rotates the pickup
        /// </summary>
        public void spinMe()
        {
            Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, rotateSpeed);
            Quaternion orientation = Quaternion.Concatenate(this.Body.Orientation, change);
            servo.Settings.Servo.Goal = orientation;
        }

        public void collision()
        {
            Vector3 pos = this.Body.Position;
            double diff = Vector3.Distance(GameScreen.getGV().Body.Position, pos);

            if (diff < this.Size + 3) //TODO: fix with correct GV physics model
            {
                MusicHandler.playSound(MusicHandler.CHINK);
                ScreenManager.game.World.fadeObjectAway(this, 0.6f);
            }           
        }
    }
}
