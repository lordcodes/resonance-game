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

        private Dictionary<string, Object> objects;
        Space space;

        //Allocated variables
        IList<BroadPhaseEntry> queryList;
        BoundingSphere bSphere;
        List<Object> rayCastObs;
        List<RayCastResult> rayCastResults;
        List<RayHit> rayCastHits;
        List<Object> returnObjs;
        Ray ray;

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

        public void allocate()
        {
            queryList = new List<BroadPhaseEntry>(objects.Count);
            bSphere = new BoundingSphere(Vector3.Zero, ACCURACY);
            rayCastObs = new List<Object>(objects.Count);
            rayCastResults = new List<RayCastResult>(objects.Count);
            rayCastHits = new List<RayHit>(objects.Count);
            returnObjs = new List<Object>(objects.Count);
            ray = new Ray();
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
            objects.Clear();
        }

        public void reset()
        {
            foreach (KeyValuePair<string,Object> pair in objects)
            {
                if (pair.Value is DynamicObject)
                {
                    ((DynamicObject)pair.Value).reset();
                }
            }
        }

        public bool querySpace(Vector3 point)
        {
            bSphere.Center = point;
            bSphere.Radius = ACCURACY;
            queryList.Clear();
            space.BroadPhase.QueryAccelerator.GetEntries(bSphere, queryList);

            if (queryList.Count > 0)
            {
                Console.WriteLine(queryList.Count + " " + queryList[0].Tag);
                return true;
            }
            else return false;
        }

        public List<Object> rayCastObjects(Vector3 position, Vector3 direction, float distance, Func<BroadPhaseEntry, bool> filter)
        {
            rayCastObs.Clear();
            rayCastResults.Clear();
            ray.Direction = direction;
            ray.Position = position;
            if (space.RayCast(ray, distance, filter, rayCastResults))
            {
                for (int i = 0; i < rayCastResults.Count; i++)
                {
                    RayCastResult result = rayCastResults[i];
                    var entityCollision = result.HitObject as EntityCollidable;
                    if (entityCollision != null)
                    {
                        //DebugDisplay.update("RAYCAST", "I CAN SEE SOMETHING DYNAMIC");
                        rayCastObs.Add(getObject(entityCollision.Entity.Tag.ToString()));
                    }
                    else
                    {
                        //DebugDisplay.update("RAYCAST", "I CAN SEE SOMETHING STATIC");
                        rayCastObs.Add(getObject(result.HitObject.Tag.ToString()));
                    }
                }
            }
            //else DebugDisplay.update("RAYCAST", "I CANT SEE ANYTHING");
            return rayCastObs;
        }

        public List<RayHit> rayCastHitData(Vector3 position, Vector3 direction, float distance, Func<BroadPhaseEntry, bool> filter)
        {
            rayCastHits.Clear();
            rayCastResults.Clear();
            ray.Direction = direction;
            ray.Position = position;
            if (space.RayCast(ray, distance, filter, rayCastResults))
            {
                for(int i = 0; i < rayCastResults.Count; i++)
                {
                    rayCastHits.Add(rayCastResults[i].HitData);
                }
            }
            return rayCastHits;
        }

        public void removeObject(string id)
        {
            removeObject(objects[id]);
        }

        //removes the object from the dictionary
        public void removeObject(Object obj)
        {
            objects.Remove(obj.returnIdentifier());
            if (obj is DynamicObject)
            {
                bool isBullet = (obj is Bullet);
                if (!isBullet)
                {
                    space.Remove(((DynamicObject)obj).Body);
                }
            }
            if (obj is StaticObject)
            {
                space.Remove(((StaticObject)obj).Body);
            }
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

        public List<Object> returnObjectSubset<T>() {
            returnObjs.Clear();
            foreach(KeyValuePair<string, Object> kVP in objects) {
                Object o = kVP.Value;
                if (o is T) returnObjs.Add(o);
            }

            return returnObjs;
        }

        public List<Object> returnObjectSubset(List<Type> types) {
            returnObjs.Clear();
            foreach(KeyValuePair<string, Object> kVP in objects) {
                Object o = kVP.Value;
                for (int i = 0; i < types.Count; i++) {
                    if (types[i].Equals(o.GetType())) returnObjs.Add(o);
                }
            }
            return returnObjs;
        }

        public void readXmlFile(string levelName, ContentManager Content)
        {
            //to test the level editor uncomment the next two lines
            StaticObject ground = null;
            StaticObject walls = null;
            StaticObject tree = null;
            StaticObject mush = null;
            StaticObject boss = null;
            //Projectile_BV projBV = null;
            GoodVibe player = null;
            StoredObjects obj = Content.Load<StoredObjects>(levelName);
            clear();

            for (int i = 0; i < obj.list.Count; i++)
            {
                if (obj.list[i].type.Equals("Ground") == true)
                {
                    ground = new StaticObject(GameModels.GROUND, "Ground", Vector3.Zero);
                    walls = new StaticObject(GameModels.WALLS, "Walls", new Vector3(0,-1,0));
                    addObject(ground);
                    //addObject(walls);
                }
                if (obj.list[i].type.Equals("Boss") == true)
                {
                    boss = new StaticObject(GameModels.BOSS, "Boss", new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(boss);
                    BulletManager.updateBossPosition(new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
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
                if (obj.list[i].type.Equals("House") == true)
                {
                    mush = new StaticObject(GameModels.HOUSE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(mush);
                }
                
                if (obj.list[i].type.Equals("BVSpawner") == true)
                {
                    BVSpawnManager.addNewSpawner(new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                }
            }
        }

       
    }
}
