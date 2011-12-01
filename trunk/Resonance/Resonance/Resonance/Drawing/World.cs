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


namespace Resonance
{
    class World
    {
        private Dictionary<string, Object> objects = new Dictionary<string, Object>();
        Space space;
        Game game;

        public World(Game game) 
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            this.game = game;
        }

       

        public void addObject(Object obj)
        {
            objects.Add(obj.returnIdentifier(), obj);
            if (obj is DynamicObject)
            {
                space.Add(((DynamicObject)obj).Body);
                if (obj is BadVibe)
                {
                    space.Add(((BadVibe)obj).Rotator);
                }
                    
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
