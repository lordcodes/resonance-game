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
        public static float MAX_MOVE_SPEED = 3f;
        public static float ROT_ACCEL = 0.02f;
        public static float MOVE_ACCEL = 0.35f;
        public static float ROT_SPEED = 0.00f;

        private static float TARGET_RANGE = 20f;
        private static float ATTACK_RANGE = 3f;
        private static int SPOT_RANGE = 6; //Distance to spot obstacles in front
        private static int ATTACK_RATE = 4; //No. of times per second
        private static int CHANCE_MISS = 20; //Between 0 and 100

        private static int ROTATE_CHANCE = 4; //Between 0 and 100

        private BadVibe bv;
        private SingleEntityAngularMotor servo;
        private int uniqueId = 0;
        private int iteration = 0;

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
            if (closeToEdge())
            {
                //Move away from the edge
                moveAwayFromEdge();
            }
            else if (!inTargetRange())
            {
                //Move randomly
                randomMove();
            }
            else if (!inAttackRange())
            {
                //Move towards GV
                moveToGV();
            }
            else
            {
                //Rotate to face GV and attack it
                Vector3 point = Game.getGV().Body.Position;
                rotateToFacePoint(point);
                attack();
            }
            iteration++;
            if (iteration == 60) iteration = 0;
        }

        private void rotateToFacePoint(Vector3 point)
        {
            Vector3 bvDir = bv.Body.OrientationMatrix.Forward;
            Vector3 bvPos = bv.Body.Position;
            Vector3 diff = Vector3.Normalize(point - bvPos);
            Quaternion rot;
            Toolbox.GetQuaternionBetweenNormalizedVectors(ref bvDir, ref diff, out rot);
            Vector3 angles = BadVibe.QuaternionToEuler(rot);
            rot.X = 0;
            rot.Z = 0;
            servo.Settings.Servo.Goal = Quaternion.Concatenate(bv.Body.Orientation, rot);
        }

        private void moveToGV()
        {
            Vector3 point = Game.getGV().Body.Position;
            rotateToFacePoint(point);
            move(1f);
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
                move(1f);
            }                
        }

        private void moveAwayFromEdge()
        {
            rotateToFacePoint(new Vector3(0f, bv.Body.Position.Y, 0f));
            move(1f);
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
                    Game.getGV().AdjustHealth(-1);
                }
            }
        }

        private bool closeToEdge()
        {
            BoundingBox box = ((StaticObject)Program.game.World.getObject("Ground")).Body.BoundingBox;
            Vector3 max = box.Max;
            Vector3 min = box.Min;

            float dX1 = Math.Abs(max.X - bv.Body.Position.X);
            float dX2 = Math.Abs(min.X - bv.Body.Position.X);
            float dZ1 = Math.Abs(max.Z - bv.Body.Position.Z);
            float dZ2 = Math.Abs(min.Z - bv.Body.Position.Z);

            if (dX1 <= 1.25f || dX2 <= 1.25f || dZ1 <= 1.25f || dZ2 <= 1.25f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool inTargetRange()
        {
            if (Game.getDistance(Game.getGV().Body.Position, bv.Body.Position) < TARGET_RANGE)
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
            if (Game.getDistance(Game.getGV().Body.Position, bv.Body.Position) < ATTACK_RANGE)
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
            Vector3 velocity = bv.Body.LinearVelocity;
            velocity.Normalize();
            //double limitX = Math.Cos(orientation.Y) * SPOT_RANGE;
            //double limitZ = Math.Sin(orientation.Y) * SPOT_RANGE;

            bool found = false;
            for (int i = 1; i <= SPOT_RANGE; i++)
            {
                if (!found)
                {
                    Vector3 point = velocity * i;
                    found = Program.game.World.querySpace(point);
                }
            }
            return found;
        }
    }
}
