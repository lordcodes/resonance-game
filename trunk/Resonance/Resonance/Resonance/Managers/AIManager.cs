using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionRuleManagement;

namespace Resonance
{
    class AIManager
    {
        public static float MAX_MOVE_SPEED = 3f;
        public static float MOVE_ACCEL = 0.35f;

        private static float TARGET_RANGE = 20f;
        private static float ATTACK_RANGE = 5f;
        private static int SPOT_RANGE = 15; //Distance to spot obstacles in front
        private static int ATTACK_RATE = 4; //No. of times per second
        private static float ATTACK_ANGLE_THRESHOLD = 0.7f; // Measure of angle at which BV has to be (rel to GV) to attack. 1 = 60 deg, sqrt(2) = 90 deg, >=2 = 180 deg.
        private static int CHANCE_MISS = 20; //Between 0 and 100
        private static int ROTATE_CHANCE = 4; //Between 0 and 100

        private static float LEFT_RAY_ANGLE = MathHelper.ToRadians(15f);
        private static float RIGHT_RAY_ANGLE = MathHelper.ToRadians(15f);

        private BadVibe bv;
        private SingleEntityAngularMotor servo;
        private int uniqueId = 0;
        private int iteration = 0;

        private Vector3 target;
        //private bool avoiding;

        public AIManager(BadVibe bvRef)
        {
            bv = bvRef;
            init();
        }

        private void init()
        {
            foreach (byte x in Encoding.Unicode.GetBytes(bv.returnIdentifier())) uniqueId += x;
            //Random r = new Random((int)(DateTime.Now.Ticks * uniqueId));
            //double angle = r.Next(0, 360);
            //angle /= 360;
            //angle *= (2 * Math.PI);
            //Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)angle);

            servo = new SingleEntityAngularMotor(bv.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.SpringSettings.DampingConstant *= 1f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 5f;
            //servo.Settings.Servo.Goal = orientation;
            Program.game.World.addToSpace(servo);
            target = bv.Body.Position;
            //avoiding = false;
        }

        public void moveManager()
        {
            if (bv.Status == BadVibe.State.NORMAL)
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
            Vector3 angles = Utility.QuaternionToEuler(rot);
            rot.X = 0;
            rot.Z = 0;
            //servo.Settings.Servo.Goal = Quaternion.Concatenate(bv.Body.Orientation, rot);
        }

        private void moveToGV()
        {
            Vector3 point = Game.getGV().Body.Position;
            rotateToFacePoint(point);
            target = Game.getGV().Body.Position;
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
                float angle = MathHelper.ToRadians(r.Next(0, 90));
                Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
                Quaternion orientation = Quaternion.Concatenate(bv.Body.Orientation, change);
                //servo.Settings.Servo.Goal = orientation;
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
            avoidObstacles();
            /*Vector3 orientation = DynamicObject.QuaternionToEuler(bv.Body.Orientation);
            Vector3 velocity = bv.Body.LinearVelocity;

            float xInc = (float)(-power * MOVE_ACCEL * Math.Sin(orientation.Y));
            float zInc = (float)(-power * MOVE_ACCEL * Math.Cos(orientation.Y));

            if (velocity.X < MAX_MOVE_SPEED && velocity.X > -MAX_MOVE_SPEED) velocity.X += xInc;
            if (velocity.Z < MAX_MOVE_SPEED && velocity.Z > -MAX_MOVE_SPEED) velocity.Z += zInc;

            bv.Body.LinearVelocity = velocity;*/
        }

        private void avoidObstacles()
        {
            //Cast three rays
            //List<Object> forwardRay = rayCast(bv.Body.Position, bv.Body.OrientationMatrix.Forward);
            //List<Object> leftRay = rayCast(bv.Body.Position, bv rotated by -LEFT_RAY_ANGLE);
            //List<Object> rightRay = rayCast(bv.Body.Position, bv rotated by LEFT_RAY_ANGLE);

            //Object forwardClosest = closestObstacle(forwardRay);
            //Object leftClosest = closestObstacle(leftRay);
            //Object rightClosest = closestObstacle(rightRay);

            //bool forwardFound = forwardClosest != null;
            //bool leftFound = leftClosest != null;
            //bool rightFound = rightClosest != null;

            List<RayHit> forwardRay = rayCastHits(bv.Body.Position, bv.Body.OrientationMatrix.Forward);

            if (forwardRay.Count > 0)
            {
                Vector3 normal = closestObstacleNormal(forwardRay);
                normal.Normalize();
                Vector3 direction = bv.Body.OrientationMatrix.Forward;
                direction.Normalize();
                Vector3 force = Utility.perpendicularComponent(normal, direction);

            }
        }

        public void attack()
        {
            // Make sure BV is facing GV
            Vector3 posDiff = bv.Body.Position - Game.getGV().Body.Position;
            Vector3 bvf = bv.Body.OrientationMatrix.Forward;
            posDiff.Normalize();
            bvf.Normalize();
            float facing = (bvf + posDiff).Length();
            if (facing > ATTACK_ANGLE_THRESHOLD) return;

            // If so, then attack.
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
            if (Vector3.Distance(Game.getGV().Body.Position, bv.Body.Position) < TARGET_RANGE)
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
            if (Vector3.Distance(Game.getGV().Body.Position, bv.Body.Position) < ATTACK_RANGE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private Object closestObstacle(List<Object> obstacles)
        {
            double closestDist = Double.MaxValue;
            Object closestObj = null;

            foreach (Object ob in obstacles)
            {
                double newDistance = Double.MaxValue;
                if (ob is StaticObject)
                {
                    newDistance = Vector3.Distance(bv.Body.Position, ((StaticObject)ob).Body.WorldTransform.Translation);
                }
                else if (ob is DynamicObject)
                {
                    newDistance = Vector3.Distance(bv.Body.Position, ((DynamicObject)ob).Body.Position);
                }
                if (newDistance < closestDist)
                {
                    closestDist = newDistance;
                    closestObj = ob;
                }
            }
            return closestObj;
        }

        private Vector3 closestObstacleNormal(List<RayHit> obstacles)
        {
            double closestDist = Double.MaxValue;
            Vector3 normal = new Vector3();

            foreach (RayHit ob in obstacles)
            {
                double newDistance = ob.T;
                
                if (newDistance < closestDist)
                {
                    closestDist = newDistance;
                    normal = ob.Normal;
                }
            }
            return normal;
        }
        
        private List<Object> rayCast(Vector3 position, Vector3 direction)
        {
            return Program.game.World.rayCastObjects(position, direction, SPOT_RANGE, RayCastFilter);
        }

        private List<RayHit> rayCastHits(Vector3 position, Vector3 direction)
        {
            return Program.game.World.rayCastHitData(position, direction, SPOT_RANGE, RayCastFilter);
        }

        bool RayCastFilter(BroadPhaseEntry entry)
        {
            return entry != bv.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;
        }

        private Quaternion rotateQuaternion(Quaternion quaternion, float angle)
        {
            Quaternion quat = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
            return Quaternion.Concatenate(quaternion, quat);
        }
    }
}
