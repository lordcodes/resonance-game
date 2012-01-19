using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics;

namespace Resonance
{
    class AIManager
    {
        public static float MAX_ROT_SPEED = 0.25f;
        public static float MAX_MOVE_SPEED = 2.00f;
        public static float ROT_ACCEL = 0.02f;
        public static float MOVE_ACCEL = 0.25f;
        public static float ROT_SPEED = 0.00f;

        private static float TARGET_RANGE = 20f;
        private static float ATTACK_RANGE = 3f;
        private static int SPOT_RANGE = 3; //Distance to spot obstacles in front
        private static int ATTACK_RATE = 4; //No. of times per second
        private static int CHANCE_MISS = 20; //Between 0 and 100

        private static int ROTATE_CHANCE = 4; //Between 0 and 100

        private BadVibe bv;
        private SingleEntityAngularMotor servo;
        private int uniqueId = 0;
        private int iteration = 0;

        int previousDirection = -1;

        public AIManager(BadVibe bvRef)
        {
            bv = bvRef;
            init();
        }

        private void init()
        {
            foreach (byte x in Encoding.Unicode.GetBytes(bv.returnIdentifier())) uniqueId += x;
            Random r = new Random((int)(DateTime.Now.Ticks * uniqueId));
            double angle = r.Next(0, 360);
            angle /= 360;
            angle *= (2* Math.PI);
            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)angle);

            servo = new SingleEntityAngularMotor(bv.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.SpringSettings.DampingConstant *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;
            servo.Settings.Servo.Goal = orientation;
            Program.game.World.addToSpace(servo);
        }

        public void moveManager()
        {
            if (!inTargetRange())
            {
                //Move randomly
                randomMove();
                //moveAround(); old method to randomly move
            }
            else if (!inAttackRange())
            {
                //Move towards GV
                moveToGV();
            }
            else
            {
                //Rotate to face GV and attack it
                rotateToGV();
                attack();
            }
            iteration++;
            if (iteration == 60) iteration = 0;
        }

        private void rotateToGV()
        {
            Vector3 bvDir = bv.Body.OrientationMatrix.Backward;
            Vector3 bvPos = bv.Body.Position;
            Vector3 gvPos = ((GoodVibe)Program.game.World.getObject("Player")).Body.Position;
            Vector3 diff = Vector3.Normalize(gvPos - bvPos);
            Quaternion rot;
            Toolbox.GetQuaternionBetweenNormalizedVectors(ref bvDir, ref diff, out rot);
            Vector3 angles = BadVibe.QuaternionToEuler(rot);
            rot.X = 0;
            rot.Z = 0;
            servo.Settings.Servo.Goal = Quaternion.Concatenate(bv.Body.Orientation, rot);
        }

        private void moveToGV()
        {
            rotateToGV();
            move(-1f);
        }

        private void randomMove()
        {
            Random r = new Random((int)(DateTime.Now.Ticks * uniqueId));
            int choice = r.Next(0, 100);
            //Either carry on moving or generate new orientation or chance to stop moving
            if (choice < ROTATE_CHANCE)
            {
                //Generate new orientation
                double angle = r.Next(0, 90);
                angle /= 360;
                angle *= (2 * Math.PI);
                Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)angle);
                Quaternion orientation = Quaternion.Concatenate(bv.Body.Orientation, change);
                servo.Settings.Servo.Goal = orientation;
            }
            else
            {
                move(-1f);
            }                
        }

        public void move(float power)
        {
            //Query world space for objects every position in front up to and including SPOT_RANGE
            bool found = obstacleFound();
            DebugDisplay.update("Obstacle", found.ToString());
            //Get object that is obstacle
            //Apply steering force to avoid it
            // compute avoidance steering force: take offset from obstacle to me,
            // take the component of that which is lateral (perpendicular to my
            // forward direction), set length to maxForce, add a bit of forward
            // component (in capture the flag, we never want to slow down)
            //Vector3 offset = Position - nearest.obstacle.center;
            //avoidance = offset.perpendicularComponent (forward());
            //avoidance = OpenSteerUtility.perpendicularComponent(offset, forward());

            //avoidance.Normalise();//.normalize ();
            //avoidance *= maxForce();
            //avoidance += forward() * maxForce() * 0.75f;

            Vector3 orientation = DynamicObject.QuaternionToEuler(bv.Body.Orientation);
            Vector3 velocity = bv.Body.LinearVelocity;

            float xInc = (float)(-power * MOVE_ACCEL * Math.Sin(orientation.Y));
            float zInc = (float)(-power * MOVE_ACCEL * Math.Cos(orientation.Y));

            if (velocity.X < MAX_MOVE_SPEED && velocity.X > -MAX_MOVE_SPEED) velocity.X += xInc;
            if (velocity.Z < MAX_MOVE_SPEED && velocity.Z > -MAX_MOVE_SPEED) velocity.Z += zInc;

            bv.Body.LinearVelocity = velocity;
        }

        public void rotate(float power)
        {
            float inc = -power * ROT_ACCEL;

            float posInc = inc;
            float posSpd = ROT_SPEED;
            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;

            if (posSpd + posInc < MAX_ROT_SPEED) ROT_SPEED += inc;

            Quaternion cAng = bv.Body.Orientation;
            Quaternion dAng = Quaternion.CreateFromAxisAngle(Vector3.Up, ROT_SPEED);
            Quaternion eAng = Quaternion.Concatenate(cAng, dAng);

            servo.Settings.Servo.Goal = eAng;
        }

        public void attack()
        {
            if (iteration % (60 / ATTACK_RATE) == 0)
            {
                Random r = new Random((int)DateTime.Now.Ticks * uniqueId);
                int chance = r.Next(0, 100);
                if (chance > CHANCE_MISS)
                {
                    ((GoodVibe)Program.game.World.getObject("Player")).AdjustHealth(-1);
                }
            }
        }

        private bool inTargetRange()
        {
            if (bv.getDistance() < TARGET_RANGE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool inAttackRange()
        {
            if (bv.getDistance() < ATTACK_RANGE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool obstacleFound()
        {
            Vector3 orientation = DynamicObject.QuaternionToEuler(bv.Body.Orientation);
            Vector3 position = bv.Body.Position;
            //double limitX = Math.Cos(orientation.Y) * SPOT_RANGE;
            //double limitZ = Math.Sin(orientation.Y) * SPOT_RANGE;

            bool found = false;
            for (int i = 1; i <= SPOT_RANGE; i++)
            {
                if (!found)
                {
                    double x = position.X + (Math.Cos(orientation.Y) * i);
                    double z = position.Z + (Math.Sin(orientation.Y) * i);
                    found = Program.game.World.querySpace(new Vector3((float)x, position.Y, (float)z));
                }
            }
            return found;
        }

        private void moveAround()
        {
            double binBoundary1 = 0.25, binBoundary2 = 0.5, binBoundary3 = 0.75;
            int total = 0;
            foreach (byte x in Encoding.Unicode.GetBytes(bv.returnIdentifier())) total += x;

            Random r = new Random((int)DateTime.Now.Ticks * total);
            double direction = r.NextDouble();

            //Probability of direction change
            switch (previousDirection)
            {
                case 0:
                    {
                        binBoundary1 = 0.97;
                        binBoundary2 = 0.98;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 1:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.98;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 2:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.02;
                        binBoundary3 = 0.99;
                        break;
                    }
                case 3:
                    {
                        binBoundary1 = 0.01;
                        binBoundary2 = 0.02;
                        binBoundary3 = 0.03;
                        break;
                    }
                default: break;
            }

            //Movement
            if (direction < binBoundary1)
            {
                previousDirection = 0;
                move(-1f);
            }
            else if (direction < binBoundary2)
            {
                move(-1f);
                previousDirection = 1;
            }
            else if (direction < binBoundary3)
            {
                move(1f);
                rotate(-1f);
                previousDirection = 2;
            }
            else
            {
                move(1f);
                rotate(1f);
                previousDirection = 3;
            }
        }

    }
}
