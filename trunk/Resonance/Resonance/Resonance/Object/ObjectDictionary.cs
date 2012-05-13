using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class ObjectDictionary
    {
        private Dictionary<string, Object> objects = new Dictionary<string, Object>(5000);
        private Dictionary<string, Object> badVibeObjects = new Dictionary<string, Object>(5000);
        List<Object> returnObjs;

        public void Clear()
        {
            objects.Clear();
            badVibeObjects.Clear();
        }

        public void Remove(String key)
        {
            objects.Remove(key);
            if (badVibeObjects.ContainsKey(key))
            {
                badVibeObjects.Remove(key);
            }
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public Object this[String key]
        {
            get
            {
                return objects[key];
            }
        }

        public void Add(String key, Object obj)
        {
            objects.Add(key, obj);

            if (obj is BadVibe)
            {
                badVibeObjects.Add(key, obj);
            }
        }

        public List<Object> returnObjectSubset<T>()
        {
            if (typeof(BadVibe) is T)
            {
                return badVibeObjects.Select(fruit => fruit.Value).ToList();
            }
            returnObjs.Clear();
            foreach (KeyValuePair<string, Object> kVP in objects)
            {
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

        public void allocate()
        {
            returnObjs = new List<Object>(objects.Count);
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
    }
}
