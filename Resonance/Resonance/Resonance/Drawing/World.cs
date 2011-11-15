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
            Box ground = new Box(Vector3.Zero, 30, 1, 30);
            space.Add(ground);
            this.game = game;
        }


        //I was thinking maybe every object could have like a string identifier so that I can store them into a 
        // hash map (dictionary) making updating, and deleting much more faster than just a simple link list

        public void addObject(Object obj)
        {
            if (checkPosition(obj.Position, obj.returnIdentifier()) == true)
            {
                objects.Add(obj.returnIdentifier(), obj);
                if (obj is DynamicObject)
                {
                    space.Add(((DynamicObject)obj).Body);
                    
                }
                else if(obj is StaticObject)
                {
                    space.Add(((StaticObject)obj).Body);
                }
                game.Components.Add(obj);
            }
        }

       
        //checks if there is another object that has the same position on the map
        private bool checkPosition(Vector3 pos, string ID)
        {
            foreach(string key in objects.Keys)
                if(objects[key].Position == pos && key.Equals(ID) == false)
                     return false;
            return true;
        }

        //removes the object from the dictionary
        public void removeObject(Object obj)
        {
            objects.Remove(obj.returnIdentifier());
        }


        //basic edge detection - checks if the new position assigned to the object does not collide with 
        //the position of other objects.

        public bool updatePosition(Vector3 pos, Object obj)
        {
            if (objects[obj.returnIdentifier()] == null)
                return false;
            else
                if (checkPosition(pos, obj.returnIdentifier()) == true)
                {
                    obj.Position = pos;
                    removeObject(obj);
                    objects.Add(obj.returnIdentifier(), obj);
                    space.Update();
                    return true;
                }
            return false;
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
