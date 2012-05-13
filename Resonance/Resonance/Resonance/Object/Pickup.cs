using System;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance
{
    class Pickup : DynamicObject
    {
        public const int X2 = 0;
        public const int X3 = 1;
        public const int PLUS4 = 2;
        public const int PLUS5 = 3;

        private int pickupType;
        private int pickupLength; //length of time the pickup has an effect
        private int initialTime; //initial time
        private int timeToLive; //current time left
        private SingleEntityAngularMotor servo;
        Random r;
        float ROTATE_SPEED = -0.1f;

        GameModelInstance X2Instance;
        GameModelInstance X3Instance;
        GameModelInstance Plus4Instance;
        GameModelInstance Plus5Instance;

        public Pickup(int modelNum, String name, Vector3 pos, int length, int time)
            : base(modelNum, name, pos)
        {
            if (modelNum == GameModels.X2)
            {
                pickupType = X2;
            }
            else if (modelNum == GameModels.X3)
            {
                pickupType = X3;
            }
            else if (modelNum == GameModels.PLUS4)
            {
                pickupType = PLUS4;
            }
            else if (modelNum == GameModels.PLUS5)
            {
                pickupType = PLUS5;
            }

            pickupLength = length;
            initialTime = time;
            timeToLive = time;

            servo = new SingleEntityAngularMotor(this.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.SpringSettings.DampingConstant *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;

            r = new Random((int)(DateTime.Now.Ticks));
            float angle = MathHelper.ToRadians(r.Next(0, 360));

            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            servo.Settings.Servo.Goal = orientation;
            ScreenManager.game.World.addToSpace(servo);

            // Set it so the pickup does not cast a shadow
            this.ModelInstance.Shadow = false;

            X2Instance = new GameModelInstance(GameModels.X2);
            X3Instance = new GameModelInstance(GameModels.X3);
            Plus4Instance = new GameModelInstance(GameModels.PLUS4);
            Plus5Instance = new GameModelInstance(GameModels.PLUS5);
            X2Instance.Shadow = false;
            X3Instance.Shadow = false;
            Plus4Instance.Shadow = false;
            Plus5Instance.Shadow = false;
        }

        public void init(int modelNum, Vector3 pos, int length, int time)
        {
            if (modelNum == GameModels.X2)
            {
                pickupType = X2;
                //this.ModelInstance = X2Instance;
            }
            else if (modelNum == GameModels.X3)
            {
                pickupType = X3;
                //this.ModelInstance = X3Instance;
            }
            else if (modelNum == GameModels.PLUS4)
            {
                pickupType = PLUS4;
                //this.ModelInstance = Plus4Instance;
            }
            else if (modelNum == GameModels.PLUS5)
            {
                pickupType = PLUS5;
                //this.ModelInstance = Plus5Instance;
            }

            Body.Position = pos;

            pickupLength = length;
            initialTime = time;
            timeToLive = time;

            float angle = MathHelper.ToRadians(r.Next(0, 360));

            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            //servo.Settings.Servo.Goal = orientation;
        }

        /// <summary>
        /// Resets the timer on the Pickup to its initial value
        /// </summary>
        public void resetTimeToLive()
        {
            timeToLive = initialTime;
        }

        /// <summary>
        /// Rotates the pickup
        /// </summary>
        public void spinMe()
        {
            Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, ROTATE_SPEED);
            Quaternion orientation = Quaternion.Concatenate(this.Body.Orientation, change);
            servo.Settings.Servo.Goal = orientation;
        }

        public int PowerUpType
        {
            get
            {
                return pickupType;
            }
        }

        public int PowerupLength
        {
            get
            {
                return pickupLength;
            }
        }

        public int TimeToLive
        {
            get
            {
                return timeToLive;
            }

            set
            {
                timeToLive = value;
            }
        }
    }
}
