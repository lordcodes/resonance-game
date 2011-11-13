using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BEPUphysics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class World
    {
        private Dictionary<string, Object> objects = new Dictionary<string, Object>();
        Space space;

        public World() 
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
        }


        //I was thinking maybe every object could have like a string identifier so that I can store them into a 
        // hash map (dictionary) making updating, and deleting much more faster than just a simple link list

        public void addObject(Object obj)
        {
            if (checkPosition(obj.getXWorldCord(), obj.getYWorldCord(), obj.getZWorldCord(), obj.returnIdentifier()) == true)
            {
                objects.Add(obj.returnIdentifier(), obj);
                //space.Add(obj);
            }
        }
       
        //checks if there is another object that has the same position on the map
        private bool checkPosition(float x, float y, float z, string ID)
        {
            foreach(string key in objects.Keys)
                if(objects[key].getXWorldCord() == x && objects[key].getYWorldCord() == y && objects[key].getZWorldCord() == z && key.Equals(ID) == false)
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

        public bool updatePosition(float posX, float posY, float posZ, Object obj)
        {
            if (objects[obj.returnIdentifier()] == null)
                return false;
            else
                if (checkPosition(posX, posY, posZ, obj.returnIdentifier()) == true)
                {
                    obj.setCords(posX, posY, posZ);
                    removeObject(obj);
                    objects.Add(obj.returnIdentifier(), obj);
                    space.Update();
                    return true;
                }
            return false;
        }


    }

  
}
