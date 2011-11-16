using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using ResonanceLibrary;
using System.Xml;


namespace Level_editor
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean ok = true;
            StoredObjects list = new StoredObjects();
            while (ok == true)
            {
                Console.WriteLine("Please specify the identifier");
                string identifier = Console.ReadLine();
                Console.WriteLine("Please specify the type");
                string type = Console.ReadLine();
                Console.WriteLine("Please specify XWorldCoord");
                float xWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify YWorldCoord");
                float yWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify ZWorldCoord");
                float zWorldCoord = float.Parse(Console.ReadLine());
                Console.WriteLine("Please specify game model number");
                int gameModNum = int.Parse(Console.ReadLine());
                StoredObject obj = new StoredObject(identifier,type, xWorldCoord, zWorldCoord, yWorldCoord, gameModNum);
                list.addObject(obj);
                Console.WriteLine("Do you wish to continue? y/n");
                String response = Console.ReadLine();
                if(response.Equals("n"))
                    ok = false;
            }
            Serialize(list, "level1.xml");

        }
        static void Serialize(StoredObjects obj, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize<StoredObjects>(writer, obj, null);
            }
        }
    }
}
