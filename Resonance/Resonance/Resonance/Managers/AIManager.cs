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
        public static float MAX_MOVE_SPEED = 6f;
        public static float MOVE_ACCEL = 0.8f;
        public static float ROT_SPEED = 0.1f;

        private static float TARGET_RANGE = 25f;
        private static float ATTACK_RANGE = 5f;
        private static int SPOT_RANGE = 15; //Distance to spot obstacles in front
        private static int ATTACK_RATE = 4; //No. of times per second
        private static float ATTACK_ANGLE_THRESHOLD = 0.7f; // Measure of angle at which BV has to be (rel to GV) to attack. 1 = 60 deg, sqrt(2) = 90 deg, >=2 = 180 deg.
        private static int CHANCE_MISS = 20; //Between 0 and 100
        private static int ROTATE_CHANCE = 4; //Between 0 and 100

        private static int DAMAGE = 5;

        private static float LEFT_RAY_ANGLE = MathHelper.ToRadians(15f);
        private static float RIGHT_RAY_ANGLE = MathHelper.ToRadians(15f);

        private BadVibe bv;
        private SingleEntityAngularMotor servo;
        private int uniqueId = 0;
        private int iteration;


        private int ok = 0;
        Bullet bullet;

        public AIManager(BadVibe bvRef)
        {
            bv = bvRef;
            init();
        }

        private void init()
        {
            foreach (byte x in Encoding.Unicode.GetBytes(bv.returnIdentifier())) uniqueId += x;
            Random r = new Random((int)(DateTime.Now.Ticks * uniqueId));
            float angle = MathHelper.ToRadians(r.Next(0, 360));
            Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);

            servo = new SingleEntityAngularMotor(bv.Body);
            servo.Settings.Mode = MotorMode.Servomechanism;
            servo.Settings.Servo.Goal = orientation;
            ScreenManager.game.World.addToSpace(servo);

            switch (GameScreen.DIFFICULTY)
            {
                case GameScreen.BEGINNER:
                    {
                        DAMAGE = 2;
                        break;
                    }
                case GameScreen.EASY:
                    {
                        DAMAGE = 3;
                        break;
                    }
                case GameScreen.MEDIUM:
                    {
                        DAMAGE = 5; //this will need changing back to 4 after testing today.
                        break;
                    }
                case GameScreen.HARD:
                    {
                        DAMAGE = 5;
                        break;
                    }
                case GameScreen.INSANE:
                    {
                        DAMAGE = 6;
                        break;
                    }
            }
        }

        public void moveManager()
        {
            iteration = ScreenManager.game.Iteration;
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
                Vector3 point = GameScreen.getGV().Body.Position;
                rotateToFacePoint(point);
                attack();
                
            }
                      
        }
   
        private void rotateToFacePoint(Vector3 point)
        {
            Vector3 bvDir = bv.Body.OrientationMatrix.Forward;
            Vector3 bvPos = bv.Body.Position;
            Vector3 diff = Vector3.Normalize(point - bvPos);
            Quaternion rot;
            Toolbox.GetQuaternionBetweenNormalizedVectors(ref bvDir, ref diff, out rot);
            rot.X = 0;
            rot.Z = 0;
            servo.Settings.Servo.Goal = Quaternion.Concatenate(bv.Body.Orientation, rot);
        }

        private void moveToGV()
        {
            Vector3 point = GameScreen.getGV().Body.Position;
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
                float angle = MathHelper.ToRadians(r.Next(0, 90));
                Quaternion change = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
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
            if(iteration % 10 == 0) avoidObstacles();
            Vector3 orientation = Utility.QuaternionToEuler(bv.Body.Orientation);
            Vector3 velocity = bv.Body.LinearVelocity;

            float xInc = (float)(-power * MOVE_ACCEL * Math.Sin(orientation.Y));
            float zInc = (float)(-power * MOVE_ACCEL * Math.Cos(orientation.Y));

            if (velocity.X < MAX_MOVE_SPEED && velocity.X > -MAX_MOVE_SPEED) velocity.X += xInc;
            if (velocity.Z < MAX_MOVE_SPEED && velocity.Z > -MAX_MOVE_SPEED) velocity.Z += zInc;

            bv.Body.LinearVelocity = velocity;
        }

        private void avoidObstacles()
        {
            List<RayHit> forwardRay = rayCastHits(bv.Body.Position, bv.Body.OrientationMatrix.Forward);

            Vector3 original = bv.Body.OrientationMatrix.Forward;
            Vector2 temp = Utility.rotateVector2(new Vector2(original.X, original.Z), LEFT_RAY_ANGLE);
            Vector3 left = new Vector3(temp.X, original.Y, temp.Y);
            temp = Utility.rotateVector2(new Vector2(original.X, original.Z), RIGHT_RAY_ANGLE);
            Vector3 right = new Vector3(temp.X, original.Y, temp.Y);

            List<RayHit> leftRay = rayCastHits(bv.Body.Position, left);
            List<RayHit> rightRay = rayCastHits(bv.Body.Position, right);

            bool forward = forwardRay.Count > 0;
            bool turnLeft = rightRay.Count > 0;
            bool turnRight = leftRay.Count > 0;

            if (forward)
            {
                Vector3 normal = closestObstacleNormal(forwardRay);
                normal.Normalize();
                Vector3 direction = bv.Body.OrientationMatrix.Forward;
                direction.Normalize();
                Vector3 avoid = Utility.perpendicularComponent(normal, direction);
                avoid.Normalize();
                Quaternion rot;
                Toolbox.GetQuaternionBetweenNormalizedVectors(ref direction, ref avoid, out rot);
                rot.X = 0;
                rot.Z = 0;
                Vector3 angles = Utility.QuaternionToEuler(rot);

                float power = 1f;
                if (angles.Y < 0)
                {
                    power *= -1f;
                }
                Quaternion inc = Quaternion.CreateFromAxisAngle(Vector3.Up, (power * ROT_SPEED));
                Quaternion goal = Quaternion.Concatenate(bv.Body.Orientation, inc);
                servo.Settings.Servo.Goal = goal;
            }
            else if (turnLeft ^ turnRight)
            {
                float power = 1f;
                if (turnLeft) power *= -1f;
                Quaternion inc = Quaternion.CreateFromAxisAngle(Vector3.Up, (power * ROT_SPEED));
                Quaternion goal = Quaternion.Concatenate(bv.Body.Orientation, inc);
                servo.Settings.Servo.Goal = goal;
            }
            else if (turnLeft && turnRight)
            {
                Quaternion inc = Quaternion.CreateFromAxisAngle(Vector3.Up, ROT_SPEED);
                Quaternion goal = Quaternion.Concatenate(bv.Body.Orientation, inc);
                servo.Settings.Servo.Goal = goal;
            }
        }

        public void attack()
        {
            // Make sure BV is facing GV
            Vector3 posDiff = bv.Body.Position - GameScreen.getGV().Body.Position;
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
                    GameScreen.getGV().AdjustHealth(-DAMAGE);
                }
            }
        }

        private bool closeToEdge()
        {
            BoundingBox box = ((StaticObject)ScreenManager.game.World.getObject("Ground")).Body.BoundingBox;
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
            if (Vector3.Distance(GameScreen.getGV().Body.Position, bv.Body.Position) < TARGET_RANGE)
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
            if (Vector3.Distance(GameScreen.getGV().Body.Position, bv.Body.Position) < ATTACK_RANGE)
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

            for(int i = 0; i < obstacles.Count; i++)
            {
                Object ob = obstacles[i];
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

            for(int i = 0; i < obstacles.Count; i++)
            {
                RayHit ob = obstacles[i];
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
            return ScreenManager.game.World.rayCastObjects(position, direction, SPOT_RANGE, RayCastFilter);
        }

        private List<RayHit> rayCastHits(Vector3 position, Vector3 direction)
        {
            return ScreenManager.game.World.rayCastHitData(position, direction, SPOT_RANGE, RayCastFilter);
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
