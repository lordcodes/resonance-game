using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResonanceLibrary
{
    public class StoredObject
    {
        public string identifier;
        public string type;
        public float xWorldCoord;
        public float zWorldCoord;
        public float yWorldCoord;
        public int gameModelNum;


        public StoredObject(string ident,string ty, float x, float z, float y, int num)
        {
            identifier = ident;
            type = ty;
            xWorldCoord = x;
            zWorldCoord = z;
            yWorldCoord = y;
            gameModelNum = num;
        }
    }
}
