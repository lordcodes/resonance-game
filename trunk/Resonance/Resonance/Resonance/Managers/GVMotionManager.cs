using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;

namespace Resonance {
    /// <summary>
    /// Handles and provides an interface between button presses and Good Vibe motion.
    /// </summary>
    class GVMotionManager {
        public static float MAX_R_SPEED                     =   0.20f;
        public static float MAX_X_SPEED                     =  10.00f;
        public static float MAX_Z_SPEED                     =  12.00f;
        public static readonly float DEFAULT_MAX_Z_SPEED    =  12.00f;
        public static float R_ACCELERATION                  =   0.02f;
        public static float X_ACCELERATION                  =   0.50f;
        public static float Z_ACCELERATION                  =   0.50f;
        public static readonly float DEFAULT_Z_ACCELERATION =   0.50f;
        public static float R_SPEED                         =   0.00f;
        public static float X_SPEED                         =   0.00f;
        public static float Z_SPEED                         =   0.00f;

        private static bool MOVING_F         = false;
        private static bool MOVING_B         = false;


        private static GoodVibe gv;

        private static float JUMP_HEIGHT = 0.5f;

        private static SingleEntityAngularMotor servo;
        private static LinearAxisMotor           lamZ;
        private static LinearAxisMotor           lamX;

        public static bool initialised = false;

        /// Methods

        public static void init() {
            gv = (GoodVibe)Program.game.World.getObject("Player");

            gv.Body.Material.KineticFriction *= 2f;
            gv.Body.Material.StaticFriction  *= 2f;
            gv.Body.Mass *= 2f;

            // ADD SERVO TO GV
            servo = new SingleEntityAngularMotor(gv.Body);

            servo.Settings.Mode = MotorMode.Servomechanism;

            servo.Settings.Servo.SpringSettings.DampingConstant   *= 10f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 100f;

            Program.game.World.addToSpace(servo);


            // ADD LINEARAXISMOTORs TO GV
            lamZ               = new LinearAxisMotor();
            lamZ.Settings.Mode = MotorMode.VelocityMotor;
            lamZ.ConnectionA   = null;
            lamZ.ConnectionB   = gv.Body;
            lamZ.Axis          = new Vector3(0f, 0f, 1f);
            lamZ.IsActive      = true;
            Program.game.World.addToSpace(lamZ);

            lamX               = new LinearAxisMotor();
            lamX.Settings.Mode = MotorMode.VelocityMotor;
            lamX.ConnectionA   = null;
            lamX.ConnectionB   = gv.Body;
            lamX.Axis          = new Vector3(0f, 0f, 1f);
            lamX.IsActive      = true;
            Program.game.World.addToSpace(lamX);

            // INITIALISATION COMPLETE
            initialised = true;
        }

        public static void setGVRef(GoodVibe newGV) {
            gv = newGV;
        }

        private static void rotate(float power) {
            float inc = -power * R_ACCELERATION;

            float posInc = inc;
            float posSpd = R_SPEED;
            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;

            float max = power * MAX_R_SPEED;
            if (max < 0) max *= -1;
            if (max > MAX_R_SPEED) max = MAX_R_SPEED;

            if (posSpd + posInc < max) R_SPEED += inc;

            Quaternion cAng = gv.Body.Orientation;
            Quaternion dAng = Quaternion.CreateFromAxisAngle(Vector3.Up, R_SPEED);
            Quaternion eAng = Quaternion.Concatenate(cAng, dAng);

            servo.Settings.Servo.Goal = eAng;

            //DebugDisplay.update("ROTATION", (DynamicObject.QuaternionToEuler(gv.Body.Orientation)).Y.ToString());
        }

        public static void boost()
        {
            GVMotionManager.Z_ACCELERATION = 1f;
            GVMotionManager.MAX_Z_SPEED = 30f;
            move(1f);
        }

        private static void move(float power) {
            float inc = power * Z_ACCELERATION;

            float posInc = inc;
            float posSpd = Z_SPEED;
            float max = power * MAX_Z_SPEED;

            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;
            if (max < 0) max *= -1;
            if (max > MAX_Z_SPEED) max = MAX_Z_SPEED;
            if (posSpd + posInc < max) Z_SPEED += inc;

            Vector3 oVector = DynamicObject.QuaternionToEuler(gv.Body.Orientation);
            Vector3 vel     = gv.Body.LinearVelocity;

            //////////// USING VELOCITIES /////////////////

            /*float xInc = (float)(-power * Z_ACCELERATION * Math.Sin(oVector.Y));
            float zInc = (float)(-power * Z_ACCELERATION * Math.Cos(oVector.Y));

            if (vel.X < MAX_Z_SPEED && vel.X > -MAX_Z_SPEED) vel.X += xInc;
            if (vel.Z < MAX_Z_SPEED && vel.Z > -MAX_Z_SPEED) vel.Z += zInc;

            gv.Body.LinearVelocity = vel;*/

            ///////////// USING LINEARAXISMOTOR ///////////////

            lamZ.Axis = gv.Body.OrientationMatrix.Forward;
            lamZ.Settings.VelocityMotor.GoalVelocity = Z_SPEED;
        }

        private static void strafe(float power) {
            float inc = power * X_ACCELERATION;

            float posInc = inc;
            float posSpd = X_SPEED;
            float max = power * MAX_X_SPEED;

            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;
            if (max < 0) max *= -1;
            if (max > MAX_X_SPEED) max = MAX_X_SPEED;
            if (posSpd + posInc < max) X_SPEED += inc;

            Vector3 oVector = DynamicObject.QuaternionToEuler(gv.Body.Orientation);
            Vector3 vel     = gv.Body.LinearVelocity;

            /*float xInc = (float)(-power * X_ACCELERATION * Math.Cos(oVector.Y));
            float zInc = (float)(-power * X_ACCELERATION * Math.Sin(oVector.Y));

            if (vel.X < MAX_X_SPEED && vel.X > -MAX_X_SPEED) vel.X += xInc;
            if (vel.Z < MAX_X_SPEED && vel.Z > -MAX_X_SPEED) vel.Z += zInc;

            gv.Body.LinearVelocity = vel;*/

            lamX.Axis = gv.Body.OrientationMatrix.Left;
            lamX.Settings.VelocityMotor.GoalVelocity = X_SPEED;
        }



        /// <summary>
        /// Takes the state of GV motion input devices and moves the GV based on these.
        /// </summary>
        /// <param name="kbd"> Current keyboard state. </param>
        /// <param name="pad"> Current game pad state. </param>
        public static void input(KeyboardState kbd, GamePadState pad)
        {
            if (!initialised) {
                init();
            }

            // Analogue stick positions
            float leftX = pad.ThumbSticks.Left.X;
            float leftY = pad.ThumbSticks.Left.Y;
            float rightX = pad.ThumbSticks.Right.X;
            float rightY = pad.ThumbSticks.Right.Y;
            float leftL = (float)Math.Sqrt(Math.Pow(leftX, 2) + Math.Pow(leftX, 2));
            float rightL = (float)Math.Sqrt(Math.Pow(rightX, 2) + Math.Pow(rightX, 2));

            bool  forward  = kbd.IsKeyDown(Keys.Up)    || (pad.DPad.Up    == ButtonState.Pressed) || leftY  > 0;
            bool  backward = kbd.IsKeyDown(Keys.Down)  || (pad.DPad.Down  == ButtonState.Pressed) || leftY  < 0;
            bool  rotateL  = kbd.IsKeyDown(Keys.Left)  || (pad.DPad.Left  == ButtonState.Pressed) || rightX < 0;
            bool  rotateR  = kbd.IsKeyDown(Keys.Right) || (pad.DPad.Right == ButtonState.Pressed) || rightX > 0;

            bool  strafeL  = kbd.IsKeyDown(Keys.OemComma);
            bool  strafeR  = kbd.IsKeyDown(Keys.OemPeriod);

            bool chargeNitro = kbd.IsKeyDown(Keys.D1); //TODO: change to non combat drum pattern
            bool nitro = kbd.IsKeyDown(Keys.D2);
            bool chargeShield = kbd.IsKeyDown(Keys.D3); //TODO: change to non combat drum pattern
            bool shield = kbd.IsKeyDown(Keys.D4);
            bool chargeFreeze = kbd.IsKeyDown(Keys.D5); //TODO: change to non combat drum pattern
            bool freeze = kbd.IsKeyDown(Keys.D6);

            // Trigger positions
            float lTrig = pad.Triggers.Left;
            float rTrig = pad.Triggers.Right; //TODO: use boost with right trigger

            // Rotate GV based on keyboard / dPad
            if (rotateL ^ rotateR) {
                float power = 1f;
                //if (rightX != 0f) power = -rightX;

                if (backward) {
                    if (rotateL) rotate(power); else rotate(-power);
                }
                else {
                    if (rotateL) rotate(-power); else rotate(power);
                }
            } else {
                R_SPEED = 0f;
            }

            // Move forward / backward based on keyboard / dPad
            if (forward ^ backward) {
                if (forward) {
                    move(1f);
                } else {
                    move(-1f);
                }
            } else {
                if (Z_SPEED > 0) if (Z_ACCELERATION > Z_SPEED)  Z_SPEED = 0f; else Z_SPEED -= Z_ACCELERATION;
                if (Z_SPEED < 0) if (Z_ACCELERATION > -Z_SPEED) Z_SPEED = 0f; else Z_SPEED += Z_ACCELERATION;
                move(0f);
                //lamZ.Settings.VelocityMotor.GoalVelocity = 0f;
            }

            // Strafe based on keyboard.
            if (strafeL ^ strafeR) {
                if (strafeL) {
                    strafe(1f);
                } else {
                    strafe(-1f);
                }
            } else {
                if (X_SPEED > 0) if (X_ACCELERATION > X_SPEED)  X_SPEED = 0f; else X_SPEED -= X_ACCELERATION;
                if (X_SPEED < 0) if (X_ACCELERATION > -X_SPEED) X_SPEED = 0f; else X_SPEED += X_ACCELERATION;
                strafe(0f);
            }

            // Move / strafe based on analogue sticks (if no movement performed above).
            // UNTESTED
            /*if (!movedZ && !strafed) {
                move(leftY);
                strafe(leftX);
            }*/

            // Jump?
            if ((gv.Body.Position.Y == 0) && (lTrig > 0)) {
                gv.jump(JUMP_HEIGHT);
            }

            //Charge speed boost when not in combat
            if (!gv.InCombat && chargeNitro)
            {
                gv.adjustNitro(1);
            }


            //Charge shield when not in combat
            if (!gv.InCombat && chargeShield)
            {
                gv.adjustShield(1);
            }

            //Use shield
            if (shield && (gv.Shield > 0))
            {
                gv.adjustShield(-1);
            }

            //Charge freeze when not in combat
            if (!gv.InCombat && chargeFreeze)
            {
                gv.adjustFreeze(1);
            }
        }
    }
}
