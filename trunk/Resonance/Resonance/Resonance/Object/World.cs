using System;
using System.Collections.Generic;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BEPUphysics.Entities.Prefabs;
using ResonanceLibrary;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Constraints;


namespace Resonance
{
    class World
    {
        public static float MAP_X;
        public static float MAP_Z;
        public static float PLAYABLE_MAP_X;
        public static float PLAYABLE_MAP_Z;
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
            try {
                objects.Add(obj.returnIdentifier(), obj); 
            } catch (Exception e) {}
            
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
                else if(obj.returnIdentifier() == "Walls")
                {
                    float wall_length = 20f;
                    float wall_height = 150f;
                    Vector3 pos1 = new Vector3(0, 75f, (PLAYABLE_MAP_Z / 2 - 7) + (wall_length / 2));
                    Vector3 pos2 = new Vector3((PLAYABLE_MAP_X / 2 - 7) + (wall_length / 2), 75f, 0);
                    Vector3 pos3 = new Vector3(0, 75f, (-PLAYABLE_MAP_Z / 2 + 7) - (wall_length / 2));
                    Vector3 pos4 = new Vector3((-PLAYABLE_MAP_X / 2 + 7) - (wall_length / 2), 75f, 0);
                    Box b1 = new Box(pos1, PLAYABLE_MAP_X, wall_height, wall_length);
                    Box b2 = new Box(pos2, wall_length, wall_height, PLAYABLE_MAP_Z);
                    Box b3 = new Box(pos3, PLAYABLE_MAP_X, wall_height, wall_length);
                    Box b4 = new Box(pos4, wall_length, wall_height, PLAYABLE_MAP_Z);
                    space.Add(b1);
                    space.Add(b2);
                    space.Add(b3);
                    space.Add(b4);
                }
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

            if (queryList.Count > 1)
            {
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

        private void removeObjectFromPhysics(Object obj)
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
        }
        
        //removes the object from the dictionary
        public void removeObject(Object obj)
        {
            removeObjectFromPhysics(obj);
            DrawableManager.Remove(obj);
        }

        //removes the object from the dictionary
        public void fadeObjectAway(Object obj, float seconds)
        {
            removeObjectFromPhysics(obj);
            obj.ModelInstance.fadeAway(seconds, delegate()
            {
                DrawableManager.Remove(obj);
            });
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
            BulletManager.BOSS_EXISTS = false;

            StoredObjects objs = Content.Load<StoredObjects>(levelName);
            clear();

            for (int i = 0; i < objs.list.Count; i++)
            {
                StoredObject obj = objs.list[i];
                Vector3 pos = new Vector3(obj.xWorldCoord, obj.yWorldCoord, obj.zWorldCoord);
                if (obj.type.Equals("Ground"))
                {
                    addObject(new StaticObject(GameModels.GROUND, "Ground", Vector3.Zero));
                    int cObj = 0;
                    if (GameScreen.mode.MODE == GameMode.OBJECTIVES) cObj = ObjectiveManager.currentObjective();
                    else cObj = -1;
                    switch (cObj)
                    {
                        case -1:
                            {
                                PLAYABLE_MAP_X = 400;
                                PLAYABLE_MAP_Z = 400;
                                addObject(new StaticObject(GameModels.TRAININGWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                        case ObjectiveManager.KILL_ALL_BV:
                            {
                                PLAYABLE_MAP_X = 400;
                                PLAYABLE_MAP_Z = 400;
                                addObject(new StaticObject(GameModels.TRAININGWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                        case ObjectiveManager.TERRITORIES:
                            {
                                PLAYABLE_MAP_X = 400;
                                PLAYABLE_MAP_Z = 400;
                                addObject(new StaticObject(GameModels.CHECKPOINTWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                        case ObjectiveManager.SURVIVE:
                            {
                                PLAYABLE_MAP_X = 200;
                                PLAYABLE_MAP_Z = 200;
                                addObject(new StaticObject(GameModels.SURVIVALWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                        case ObjectiveManager.COLLECT_ALL_PICKUPS:
                            {
                                PLAYABLE_MAP_X = 400;
                                PLAYABLE_MAP_Z = 400;
                                addObject(new StaticObject(GameModels.PICKUPSWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                        case ObjectiveManager.KILL_BOSS:
                            {
                                PLAYABLE_MAP_X = 300;
                                PLAYABLE_MAP_Z = 300;
                                addObject(new StaticObject(GameModels.BOSSWALLS, "Walls", new Vector3(0, -1, 0)));
                                break;
                            }
                    }
                }
                if (obj.type.Equals("Boss"))
                {
                    BulletManager.BOSS_EXISTS = false;
                    addObject(new StaticObject(GameModels.BOSS, "Boss", pos));
                    BulletManager.updateBossPosition(pos);
                }
                if (obj.type.Equals("Good_vibe"))
                {
                    GoodVibe player = new GoodVibe(GameModels.GOOD_VIBE, "Player", pos);
                    addObject(player);
                    player.calculateSize();
                }
                if (obj.type.Equals("Neuron"))
                {
                    addObject(new StaticObject(GameModels.NEURON, obj.identifier, pos));
                }
                if (obj.type.Equals("Bacteria"))
                {
                    addObject(new StaticObject(GameModels.BACTERIA, obj.identifier, pos));
                }
                if (obj.type.Equals("BVSpawner"))
                {
                    if (GameScreen.USE_BV_SPAWNER) BVSpawnManager.addNewSpawner(pos);
                }
                if (obj.type.Equals("CheckPoint"))
                {
                    addObject(new Checkpoint(GameModels.CHECKPOINT, obj.identifier, pos, Checkpoint.RED));
                }
                if (obj.type.Equals("ObjectivePickup"))
                {
                    //ObjectivePickup p = new ObjectivePickup(GameModels.PICKUPORB, "ObjectivePickup" + 0, new Vector3(0f, 5f, 25f));
                    ObjectivePickup p = new ObjectivePickup(GameModels.PICKUPORB, obj.identifier, new Vector3(obj.xWorldCoord, obj.yWorldCoord, obj.zWorldCoord)); //TODO: change to pool
                    addObject(p);
                }
            }
        }       
    }
}
