using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BEPUphysics.Entities.Prefabs;
using System.Xml;
using System.IO;
using ResonanceLibrary;
using BEPUphysics.Entities;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Constraints;


namespace Resonance
{
    class World
    {
        public static float MAP_X;
        public static float MAP_Z;
        public static float MAP_MIN_X;
        public static float MAP_MIN_Z;
        private const float ACCURACY = 0.1f;
        private const int BVSpawnRadious = 10;
        private const int BVAllowedActive = 1;
        private const int MaxBV = 12;

        private Dictionary<string, Object> objects;
        Space space;

        public World() 
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            //Speed improvements
            SolverSettings.DefaultMinimumIterations = 0;
#if WINDOWS
            for(int i = 0; i < System.Environment.ProcessorCount; i++)
            {
                space.ThreadManager.AddThread();
            }
#else
            space.ThreadManager.AddThread(o => System.Threading.Thread.CurrentThread.SetProcessorAffinity(1), null);
            space.ThreadManager.AddThread(o => System.Threading.Thread.CurrentThread.SetProcessorAffinity(3), null);
            space.ThreadManager.AddThread(o => System.Threading.Thread.CurrentThread.SetProcessorAffinity(5), null);
#endif

            objects = new Dictionary<string, Object>(5000);
        }

        public void addObject(Object obj)
        {
            
            objects.Add(obj.returnIdentifier(), obj);
           
            
            if (obj is DynamicObject)
            {
                if (obj.returnIdentifier().Contains("Bullet") == false)
                space.Add(((DynamicObject)obj).Body);
            }
            else if(obj is StaticObject)
            {
                
                    space.Add(((StaticObject)obj).Body);
                if (obj.returnIdentifier() == "Ground")
                {
                    Vector3 max = ((StaticObject)obj).Body.BoundingBox.Max;
                    Vector3 min = ((StaticObject)obj).Body.BoundingBox.Min;

                    MAP_MIN_X = min.X;
                    MAP_MIN_Z = min.Z;

                    MAP_Z = Math.Abs(max.Z - min.Z);
                    MAP_X = Math.Abs(max.X - min.X);
                }
                //Add walls
                float wall_length = 20f;
                float wall_height = 150f;
                Vector3 pos1 = new Vector3(0, 75f, (MAP_Z / 2) + (wall_length / 2));
                Vector3 pos2 = new Vector3((MAP_X / 2) + (wall_length / 2), 75f, 0);
                Vector3 pos3 = new Vector3(0, 75f, (-MAP_Z / 2) - (wall_length / 2));
                Vector3 pos4 = new Vector3((-MAP_X / 2) - (wall_length / 2), 75f, 0);
                Box b1 = new Box(pos1, MAP_X, wall_height, wall_length);
                Box b2 = new Box(pos2, wall_length, wall_height, MAP_Z);
                Box b3 = new Box(pos3, MAP_X, wall_height, wall_length);
                Box b4 = new Box(pos4, wall_length, wall_height, MAP_Z);
                space.Add(b1);
                space.Add(b2);
                space.Add(b3);
                space.Add(b4);
            }
            //Program.game.Components.Add(obj);
            DrawableManager.Add(obj);
        }

        public void addToSpace(ISpaceObject obj)
        {
            space.Add(obj);
        }

        public void clear()
        {
            List<Object> objectsToRemove = new List<Object>();
            foreach (var entry in objects)
            {
                objectsToRemove.Add(entry.Value);
            }
            foreach (var entry in objectsToRemove)
            {
                removeObject(entry);
            }

        }

        public void reset()
        {
            foreach (var entry in objects)
            {
                if (entry.Value is DynamicObject)
                {
                    ((DynamicObject)entry.Value).reset();
                }
            }
        }

        public bool querySpace(Vector3 point)
        {
            IList<BroadPhaseEntry> list = new List<BroadPhaseEntry>();
            BoundingSphere sphere = new BoundingSphere(point, ACCURACY);
            space.BroadPhase.QueryAccelerator.GetEntries(sphere, list);

            if (list.Count > 0)
            {
                return true;
            }
            else return false;
        }

        public List<Object> rayCastObjects(Vector3 position, Vector3 direction, float distance, Func<BroadPhaseEntry, bool> filter)
        {
            List<Object> objects = new List<Object>();

            List<RayCastResult> rayCastResults = new List<RayCastResult>();
            if (space.RayCast(new Ray(position, direction), distance, filter, rayCastResults))
            {
                foreach (RayCastResult result in rayCastResults)
                {
                    var entityCollision = result.HitObject as EntityCollidable;
                    if (entityCollision != null)
                    {
                        //DebugDisplay.update("RAYCAST", "I CAN SEE SOMETHING DYNAMIC");
                        objects.Add(getObject(entityCollision.Entity.Tag.ToString()));
                    }
                    else
                    {
                        //DebugDisplay.update("RAYCAST", "I CAN SEE SOMETHING STATIC");
                        objects.Add(getObject(result.HitObject.Tag.ToString()));
                    }
                }
            }
            //else DebugDisplay.update("RAYCAST", "I CANT SEE ANYTHING");
            return objects;
        }

        public List<RayHit> rayCastHitData(Vector3 position, Vector3 direction, float distance, Func<BroadPhaseEntry, bool> filter)
        {
            List<RayHit> objects = new List<RayHit>();

            List<RayCastResult> rayCastResults = new List<RayCastResult>();
            if (space.RayCast(new Ray(position, direction), distance, filter, rayCastResults))
            {
                foreach (RayCastResult result in rayCastResults)
                {
                    objects.Add(result.HitData);
                }
            }
            return objects;
        }

        //removes the object from the dictionary
        public void removeObject(Object obj)
        {
            objects.Remove(obj.returnIdentifier());
            if (obj is DynamicObject)
            {
                if(!obj.returnIdentifier().Contains("Bullet"))
                    space.Remove(((DynamicObject)obj).Body);
            }
            if (obj is StaticObject)
            {
                space.Remove(((StaticObject)obj).Body);
            }
            //Program.game.Components.Remove(obj);
            DrawableManager.Remove(obj);
        }

        public Object getObject(String name)
        {
            return objects[name];
        }
        

        public void update()
        {
            space.Update();
        }

        public Dictionary<string, Object> returnObjects()
        {
            return objects;
        }

        public void removeObject(String obj)
        {
            objects.Remove(obj);
        }

        public List<Object> returnObjectSubset<T>() {
            List<Object> objs = new List<Object>();

            foreach(KeyValuePair<string, Object> kVP in objects) {
                Object o = kVP.Value;
                if (o is T) objs.Add(o);
            }

            return objs;
        }

        public List<Object> returnObjectSubset(List<Type> types) {
            List<Object> objs = new List<Object>();

            foreach(KeyValuePair<string, Object> kVP in objects) {
                Object o = kVP.Value;
                foreach (Type t in types) {
                    if (t.Equals(o.GetType())) objs.Add(o);
                }
            }

            return objs;
        }

        public void readXmlFile(string levelName, ContentManager Content)
        {
            //to test the level editor uncomment the next two lines
            StaticObject ground = null;
            StaticObject tree = null;
            StaticObject mush = null;
            //Projectile_BV projBV = null;
            GoodVibe player = null;
            //BadVibe bv = null;
            //Pickup p = null;
            StoredObjects obj = Content.Load<StoredObjects>(levelName);
            clear();

            for (int i = 0; i < obj.list.Count; i++)
            {
                if (obj.list[i].type.Equals("Ground") == true)
                {
                    ground = new StaticObject(GameModels.GROUND, "Ground", Vector3.Zero);
                    addObject(ground);
                }
                if (obj.list[i].type.Equals("Good_vibe") == true)
                {
                    player = new GoodVibe(GameModels.GOOD_VIBE, "Player", new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(player);
                    player.calculateSize();
                }
                if (obj.list[i].type.Equals("Tree") == true)
                {
                    tree = new StaticObject(GameModels.TREE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(tree);
                }
                /*if (obj.list[i].type.Equals("Bad_vibe") == true)
                {
                    bv = new BadVibe(GameModels.BAD_VIBE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord),0);
                    addObject(bv);
                    bv.calculateSize();
                }*/
                if (obj.list[i].type.Equals("House") == true)
                {
                    mush = new StaticObject(GameModels.HOUSE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(mush);
                }
                
                if (obj.list[i].type.Equals("BVSpawner") == true)
                {
                    BVSpawnManager.addNewSpawner(MaxBV, BVSpawnRadious, BVAllowedActive, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                }
            }
            
            //TODO: Temp crate add
            //p = new Pickup(GameModels.PICKUP, "Pickup2", new Vector3(5f, 0f, 5f), 1, 120);
            //addObject(p);
            //p = new Pickup(GameModels.PICKUP, "Pickup3", new Vector3(15f, 0f, 5f), 1, 180);
            //addObject(p);
        }

       
    }
}
