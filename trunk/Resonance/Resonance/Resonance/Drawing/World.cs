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
using BEPUphysics.Entities;
using BEPUphysics.BroadPhaseSystems;


namespace Resonance
{
    class World
    {
        private const float accuracy = 0.5f;

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
            }
            game.Components.Add(obj);
        }

        public void addToSpace(ISpaceObject obj)
        {
            space.Add(obj);
        }

        public bool querySpace(Vector3 point)
        {
            IList<BroadPhaseEntry> list = new List<BroadPhaseEntry>();
            BoundingSphere sphere = new BoundingSphere(point, accuracy);
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
            if (obj is BadVibe)
            {
                space.Remove(((BadVibe)obj).Body);
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

        public void readXmlFile(string levelName, ContentManager Content)
        {
            
           
        }
    }
}
