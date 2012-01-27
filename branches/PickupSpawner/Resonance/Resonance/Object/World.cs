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

        public World() 
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            objects = new Dictionary<string, Object>();
        }

        public void addObject(Object obj)
        {
            objects.Add(obj.returnIdentifier(), obj);
            if (obj is DynamicObject)
            {
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
            }
            Program.game.Components.Add(obj);
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
                //BroadPhaseEntry e = list[0];
                //DebugDisplay.update("Point query", "" + list.Count + " " + point);
                return true;
            }
            else return false;
        }

        public List<Object> rayCast(Vector3 position, Vector3 direction, float distance)
        {
            List<Object> objects = new List<Object>();

            List<RayCastResult> rayCastResults = new List<RayCastResult>();
            if (space.RayCast(new Ray(position, direction), distance, RayCastFilter, rayCastResults))
            {
                DebugDisplay.update("SEESOMETHING", "I CAN SEE SOMETHING");
                foreach (RayCastResult result in rayCastResults)
                {
                    var entityCollision = rayCastResults[0].HitObject as EntityCollidable;
                    if (entityCollision != null)
                    {
                        //DebugDisplay.update("0", entityCollision.Entity.Tag.ToString());
                        objects.Add(getObject(entityCollision.Entity.Tag.ToString()));
                    }
                    else
                    {
                        //DebugDisplay.update("0", rayCastResults[0].HitObject.Tag.ToString());
                        objects.Add(getObject(rayCastResults[0].HitObject.Tag.ToString()));
                    }
                }
            }
            else
            {
                DebugDisplay.update("SEESOMETHING", "I CANNOT SEE ANYTHING");
            }
            return objects;
        }

        bool RayCastFilter(BroadPhaseEntry entry)
        {
            return entry != Game.getGV().Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;
        }

        //removes the object from the dictionary
        public void removeObject(Object obj)
        {
            objects.Remove(obj.returnIdentifier());
            if (obj is DynamicObject)
            {
                space.Remove(((DynamicObject)obj).Body);
            }
            if (obj is StaticObject)
            {
                space.Remove(((StaticObject)obj).Body);
            }
            Program.game.Components.Remove(obj);
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

        /// <summary>
        /// Returns all bad vibes as a list.
        /// </summary>
        public List<BadVibe> returnBadVibes()
        {
            List<BadVibe> badVibes = new List<BadVibe>();
 
            foreach(KeyValuePair<string, Object> kVP in objects) {
                Object obj = kVP.Value;

                if (obj is BadVibe)
                {
                    badVibes.Add((BadVibe) obj);
                }
            }

            return badVibes;
        }

        public void readXmlFile(string levelName, ContentManager Content)
        {
            //to test the level editor uncomment the next two lines
            StaticObject ground = null;
            StaticObject tree = null;
            StaticObject mush = null;
            GoodVibe player = null;
            BadVibe bv = null;
            Pickup p = null;
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
                }
                if (obj.list[i].type.Equals("Tree") == true)
                {
                    tree = new StaticObject(GameModels.TREE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(tree);
                }
                if (obj.list[i].type.Equals("Mushroom") == true)
                {
                    mush = new StaticObject(GameModels.MUSHROOM, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(mush);
                }
                if (obj.list[i].type.Equals("Bad_vibe") == true)
                {
                    bv = new BadVibe(GameModels.BAD_VIBE, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(bv);
                }
                if (obj.list[i].type.Equals("Pickup") == true)
                {
                    //p = new Pickup(GameModels.PICKUP, obj.list[i].identifier, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord), obj.list[i].pickuptype, 60); //TODO: fix xml
                    //addObject(p);
                }
            }
            
            //TODO: Temp crate add
            //p = new Pickup(GameModels.PICKUP, "Pickup2", new Vector3(5f, 0f, 5f), 1, 120);
            //addObject(p);
            //p = new Pickup(GameModels.PICKUP, "Pickup3", new Vector3(15f, 0f, 5f), 1, 180);
            //addObject(p);
        }

        /// <summary>
        /// Returns a list of Pickup objects
        /// </summary>
        /// <returns>The list of Pickup Objects</returns>
        public List<Pickup> returnPickups()
        {
            List<Pickup> pickups = new List<Pickup>();

            foreach (KeyValuePair<string, Object> kVP in objects)
            {
                Object obj = kVP.Value;

                if (obj is Pickup)
                {
                    pickups.Add((Pickup)obj);
                }
            }

            return pickups;
        }

        /// <summary>
        /// Checks if Pickups intersect with GoodVibe and remove pickups with TimeToLive = 0
        /// </summary>
        /// <param name="pickups">List of Pickup objects</param>
        public void updatePickups(List<Pickup> pickups)
        {
            //pickUpCollision(Game.getGV().Body.Position, Game.getGV().Body.OrientationMatrix.Forward, 3f);

            for (int i = 0; i < pickups.Count; i++)
            {
                Vector3 pickupPoint = pickups[i].OriginalPosition;
                double diff = Game.getDistance(Game.getGV().Body.Position, pickupPoint);

                if(diff < pickups[i].Size)
                {                    
                    Program.game.Music.playSound(MusicHandler.PICKUP);
                    Drawing.addWave(pickupPoint);
                    removeObject(pickups[i]);
                    Program.game.pickupspawner.pickupPickedUp();
                    continue;
                }

                pickups[i].TimeToLive--;
                if (pickups[i].TimeToLive == 0)
                {
                    //removeObject(pickups[i]);
                    //Program.game.pickupspawner.pickupPickedUp();
                }
            }
        }
        
        /// <summary>
        /// Deprecated method.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        public void pickUpCollision(Vector3 position, Vector3 direction, float distance)
        {
            List<RayCastResult> rayCastResults = new List<RayCastResult>();
            if (space.RayCast(new Ray(position, direction), distance, RayCastFilter, rayCastResults))
            {
                //DebugDisplay.update("pickup collision", "collision");
                foreach (RayCastResult result in rayCastResults)
                {
                    var entityCollision = rayCastResults[0].HitObject as EntityCollidable;
                    if (entityCollision == null)
                    {
                        //DebugDisplay.update("1", rayCastResults[0].HitObject.Tag.ToString());

                        Object obj = getObject(rayCastResults[0].HitObject.Tag.ToString());
                        if (obj is Pickup)
                        {
                            Program.game.Music.playSound(MusicHandler.PICKUP);
                            Drawing.addWave(obj.OriginalPosition);
                            //((StaticObject)obj).Body.
                            removeObject(obj);
                        }
                    }
                }
            }
        }
    }
}
