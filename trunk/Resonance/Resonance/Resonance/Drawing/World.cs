using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Resonance.Drawing
{
    class World
    {
        private Dictionary<String, Object> objects = new Dictionary<string, Object>();

        //I was thinking maybe every object could have like a string identifier so that I can store them into a 
        // hash map (dictionary) making updating, and deleting much more faster than just a simple link list
        // - I didn`t update the object class because I didn`t want to change someone else`s code without talking to them
        //first. 
        public void addObject(Object obj)
        {

        }
        public void removeObject(Object obj)
        {

        }
        public bool updatePosition(float posX, float posY, float posZ, Object obj)
        {

            return false;
        }


    }

  
}
