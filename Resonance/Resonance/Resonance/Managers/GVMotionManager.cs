using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance
{
    /// <summary>
    /// Handles and provides an interface between button presses and Good Vibe motion.
    /// </summary>
    class GVMotionManager
    {
        public static float MAX_R_SPEED      =   0.25f;
        public static float MAX_X_SPEED      =   4.00f;
        public static float MAX_Z_SPEED      =  12.00f;
        public static float R_ACCELERATION   =   0.02f;
        public static float X_ACCELERATION   =   0.25f;
        public static float Z_ACCELERATION   =   0.50f;
        public static float R_SPEED          =   0.00f;

        private static GoodVibe gv = (GoodVibe)Program.game.World.getObject("Player");

        private static float JUMP_HEIGHT = 0.5f;

        private static SingleEntityAngularMotor motor;
        private static SingleEntityAngularMotor servo;
        private static bool initialised = false;

        public GVMotionManager() {
        }

        /// Methods

        public static void init() {
            //motor = new SingleEntityAngularMotor(gv.Body);
            servo = new SingleEntityAngularMotor(gv.Body);

            servo.Settings.Mode = MotorMode.Servomechanism;
            //motor.Settings.Mode = MotorMode.VelocityMotor;

            DebugDisplay.update("Damping:  ", servo.Settings.Servo.SpringSettings.DampingConstant.ToString());
            DebugDisplay.update("Stiffness:", servo.Settings.Servo.SpringSettings.StiffnessConstant.ToString());

            servo.Settings.Servo.SpringSettings.DampingConstant   *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;

            //motor.Settings.VelocityMotor.Softness = 0.0005f;

            //Program.game.World.addToSpace(motor);
            Program.game.World.addToSpace(servo);

            initialised = true;
        }

        public static void setGVRef(GoodVibe newGV) {
            gv = newGV;
        }

        private static void rotate(float power)
        {
            //float inc = power * MAX_ROTATE_SPEED;
            float inc = -power * R_ACCELERATION;

            float posInc = inc;
            float posSpd = R_SPEED;
            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;

            if (posSpd + posInc < MAX_R_SPEED) R_SPEED += inc;

            DebugDisplay.update("Speed", R_SPEED.ToString());

            /*Vector3 angV = gv.Body.AngularVelocity;

            if (inc >= 0) {
                if (angV.Y + inc <  MAX_ROTATE_SPEED) angV.Y += inc; else angV.Y =  MAX_ROTATE_SPEED;
            } else {
                if (angV.Y + inc > -MAX_ROTATE_SPEED) angV.Y += inc; else angV.Y = -MAX_ROTATE_SPEED;
            }
           
            motor.Settings.VelocityMotor.GoalVelocity = angV;*/

            Quaternion cAng = gv.Body.Orientation;
            //Quaternion dAng = Quaternion.CreateFromAxisAngle(Vector3.Up, -power * 0.3f);
            Quaternion dAng = Quaternion.CreateFromAxisAngle(Vector3.Up, R_SPEED);
            Quaternion eAng = Quaternion.Concatenate(cAng, dAng);

            servo.Settings.Servo.Goal = eAng;

            DebugDisplay.update("O", eAng.ToString());

            //gv.Body.AngularVelocity = angV;
        }

        private static void move(float power) {
            Vector3 oVector = DynamicObject.QuaternionToEuler(gv.Body.Orientation);
            Vector3 vel     = gv.Body.LinearVelocity;

            float xInc = (float)(-power * Z_ACCELERATION * Math.Sin(oVector.Y));
            float zInc = (float)(-power * Z_ACCELERATION * Math.Cos(oVector.Y));

            /*float xMax = MAX_Z_SPEED  * (float) Math.Sin(oVector.Y);
            float zMax = MAX_Z_SPEED  * (float) Math.Cos(oVector.Y);*/
            /*float xAcc = ACCELERATION * (float) Math.Sin(oVector.Y);
            float zAcc = ACCELERATION * (float) Math.Cos(oVector.Y);

            if (vel.X < xMax && vel.X > -xMax) vel.X += power * (float) (xAcc);
            if (vel.Z < zMax && vel.Z > -zMax) vel.Z += power * (float) (zAcc);*/

            if (vel.X < MAX_Z_SPEED && vel.X > -MAX_Z_SPEED) vel.X += xInc;
            if (vel.Z < MAX_Z_SPEED && vel.Z > -MAX_Z_SPEED) vel.Z += zInc;

            //if (vel.Length() < MAX_Z_SPEED) vel.Z += (float)(-power * ACCELERATION * Math.Cos(oVector.Y));
            //if (vel.Length() < MAX_Z_SPEED) vel.X += (float)(-power * ACCELERATION * Math.Sin(oVector.Y));

            gv.Body.LinearVelocity = vel;
        }

        private static void strafe(float power) {
            Vector3 oVector = DynamicObject.QuaternionToEuler(gv.Body.Orientation);
            Vector3 vel     = gv.Body.LinearVelocity;

            float xInc = (float)(-power * X_ACCELERATION * Math.Cos(oVector.Y));
            float zInc = (float)(-power * X_ACCELERATION * Math.Sin(oVector.Y));

            if (vel.X < MAX_X_SPEED && vel.X > -MAX_X_SPEED) vel.X += xInc;
            if (vel.Z < MAX_X_SPEED && vel.Z > -MAX_X_SPEED) vel.Z += zInc;

            gv.Body.LinearVelocity = vel;
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

            // These values record whether or not a motion has been performed with the dPad so that the same motion isn't
            // applied twice if the dPad is used in conjunction with the analogue sticks.
            bool rotated  = false;
            bool movedZ   = false;
            bool strafed  = false;

            bool forward  = kbd.IsKeyDown(Keys.Up)    || (pad.DPad.Up    == ButtonState.Pressed);
            bool backward = kbd.IsKeyDown(Keys.Down)  || (pad.DPad.Down  == ButtonState.Pressed);
            bool rotateL  = kbd.IsKeyDown(Keys.Left)  || (pad.DPad.Left  == ButtonState.Pressed);
            bool rotateR  = kbd.IsKeyDown(Keys.Right) || (pad.DPad.Right == ButtonState.Pressed);

            bool strafeL  = kbd.IsKeyDown(Keys.OemComma);
            bool strafeR  = kbd.IsKeyDown(Keys.OemPeriod);

            // Analogue stick positions
            float leftX   = pad.ThumbSticks.Left.X;
            float leftY   = pad.ThumbSticks.Left.Y;
            float rightX  = pad.ThumbSticks.Right.X;
            float rightY  = pad.ThumbSticks.Right.Y;
            float leftL   = (float) Math.Sqrt(Math.Pow(leftX,  2) + Math.Pow(leftX,  2));
            float rightL  = (float) Math.Sqrt(Math.Pow(rightX, 2) + Math.Pow(rightX, 2));

            // Trigger positions
            float rTrig = pad.Triggers.Left;

            // Rotate GV based on keyboard / dPad
            if (rotateL ^ rotateR) {
                if (backward)
                {
                    //if (rotateL) gv.rotate(DynamicObject.ROTATE_CLOCK); else gv.rotate(DynamicObject.ROTATE_ANTI);
                    if (rotateL) rotate(1f); else rotate(-1f);
                }
                else
                {
                    //if (rotateL) gv.rotate(DynamicObject.ROTATE_ANTI); else gv.rotate(DynamicObject.ROTATE_CLOCK);
                    if (rotateL) rotate(-1f); else rotate(1f);
                }

                rotated = true;
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

                movedZ = true;
            }

            // Strafe based on keyboard.
            if (strafeL ^ strafeR) {
                if (strafeL) {
                    strafe(1f);
                } else {
                    strafe(-1f);
                }

                strafed = true;
            }

            // Move / strafe based on analogue sticks (if no movement performed above).
            // UNTESTED
            /*if (!movedZ && !strafed) {
                move(leftY);
                strafe(leftX);
            }*/

            //if (!rotated) rotate();

            float x = leftX;
            float y = leftY;
            float camerax = rightX;
            float cameray = rightY;

            if (x == 0 && y > 0) {
                gv.move(DynamicObject.MOVE_FORWARD);
            }
            if (x == 0 && y < 0) {
                gv.move(DynamicObject.MOVE_BACKWARD);
            }
            if (x < 0 && y == 0) {
                gv.move(DynamicObject.MOVE_LEFT);
            }
            if (x < 0 && y > 0) {
                gv.move(DynamicObject.MOVE_LEFT);
                gv.move(DynamicObject.MOVE_FORWARD);
            }
            if (x < 0 && y < 0) {
                gv.move(DynamicObject.MOVE_LEFT);
                gv.move(DynamicObject.MOVE_BACKWARD);
            }
            if (x > 0 && y < 0) {
                gv.move(DynamicObject.MOVE_RIGHT);
                gv.move(DynamicObject.MOVE_BACKWARD);
            }
            if (x > 0 && y > 0) {
                gv.move(DynamicObject.MOVE_RIGHT);
                gv.move(DynamicObject.MOVE_FORWARD);
            }
            if (x > 0 && y == 0) {
                gv.move(DynamicObject.MOVE_RIGHT);
            }

            if (camerax == -1 && cameray == 0) {
                gv.rotate(DynamicObject.ROTATE_ANTI);
            }
            if (camerax == 1 && cameray == 0) {
                gv.rotate(DynamicObject.ROTATE_CLOCK);
            }

            // Jump?
            if ((gv.Body.Position.Y == 0) && (rTrig > 0)) {
                gv.jump(JUMP_HEIGHT);
            }

            // Reset
        }
    }
}
