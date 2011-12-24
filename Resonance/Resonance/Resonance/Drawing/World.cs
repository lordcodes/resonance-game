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


namespace Resonance
{
    class World
    {
        public static float MAP_X;
        public static float MAP_Z;
        private const float ACCURACY = 0.1f;

        private Dictionary<string, Object> objects;
        Space space;
        Game game;

        public World(Game game) 
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            this.game = game;
            objects = new Dictionary<string, Object>();
        }

        public void addObject(Object obj)
        {
            objects.Add(obj.returnIdentifier(), obj);
            if (obj is DynamicObject)
            {
                space.Add(((DynamicObject)obj).Body);
                space.Add(((DynamicObject)obj).Rotator);
                Entity e = ((DynamicObject)obj).Body;
            }
            else if(obj is StaticObject)
            {
                space.Add(((StaticObject)obj).Body);
                if (obj.returnIdentifier() == "Ground")
                {
                    Vector3 max = ((StaticObject)obj).Body.BoundingBox.Max;
                    Vector3 min = ((StaticObject)obj).Body.BoundingBox.Min;

                    MAP_Z = Math.Abs(max.Z - min.Z);
                    MAP_X = Math.Abs(max.X - min.X);
                }
            }
            game.Components.Add(obj);
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
                BroadPhaseEntry e = list[0];
                DebugDisplay.update("Point query", "" + list.Count + " " + point);
                return true;
            }
            else return false;
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
            game.Components.Remove(obj);
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
            StoredObjects obj = Content.Load<StoredObjects>(levelName);
            clear();

            for (int i = 0; i < obj.list.Count; i++)
            {
                if (obj.list[i].type.Equals("Ground") == true)
                {
                    ground = new StaticObject(GameModels.GROUND, "Ground", game, Vector3.Zero);
                    addObject(ground);
                }
                if (obj.list[i].type.Equals("Good_vibe") == true)
                {
                    player = new GoodVibe(GameModels.GOOD_VIBE, "Player", game, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(player);
                }
                if (obj.list[i].type.Equals("Tree") == true)
                {
                    tree = new StaticObject(GameModels.TREE, obj.list[i].identifier, game, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(tree);
                }
                if (obj.list[i].type.Equals("Mushroom") == true)
                {
                    mush = new StaticObject(GameModels.MUSHROOM, obj.list[i].identifier, game, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(mush);
                }
                if (obj.list[i].type.Equals("Bad_vibe") == true)
                {
                    bv = new BadVibe(GameModels.BAD_VIBE, obj.list[i].identifier, game, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    addObject(bv);
                }
            }
           
        }
    }
}
