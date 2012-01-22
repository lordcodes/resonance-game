using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResonanceLibrary
{
    public class StoredObjects
    {
        public List<StoredObject> list = new List<StoredObject>();
        public void addObject(StoredObject obj)
        {
            list.Add(obj);
        }
    }
}
