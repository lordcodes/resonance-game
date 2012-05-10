using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;

namespace Resonance
{
    public class HighScoreManager
    {
        
        public struct HighScoreData
        {
            public string[] PlayerName;
            public int[] Score;
            public int SIZE;
            public HighScoreData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];
                SIZE = count;
            }
        }
        public static HighScoreData data;

        public static void initializeData()
        {
            data = new HighScoreData(6);
            data.PlayerName[0] = "Alex Sheppard";
            data.Score[0] = 0;

            data.PlayerName[1] = "Andrew Lord";
            data.Score[1] = 0;

            data.PlayerName[2] = "Mihai Nemes";
            data.Score[2] = 0;

            data.PlayerName[3] = "Michael Jones";
            data.Score[3] = 0;

            data.PlayerName[4] = "Phillip Tattersal";
            data.Score[4] = 0;

            data.PlayerName[5] = "Tom Pickering";
            data.Score[5] = 0;
        }
        public static void updateTable(int score)
        {
            int index = data.SIZE;
            while (data.Score[index-1] < score && index-1 >= 0)
            {
                index--;
            }
            if (index != data.SIZE)
            {
                int index2 = data.SIZE - 1;
                while (index2 > index)
                {
                    data.Score[index2] = data.Score[index2 - 1];
                    index2--;
                }
                data.Score[index] = score;
            }
            Console.WriteLine("THIS IS THE UPDATED VERSION OF THE TABLE THAT WILL BE SAVED");
            for (index = 0; index < data.SIZE; index++)
            {
                Console.WriteLine(data.PlayerName[index] + " " + data.Score[index]);
            }
        }
       
        public static void saveFile()
        {
            if (!Guide.IsVisible)
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                if(result.IsCompleted)
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                       createFile(device);
                    }
                 }
            }
        }
        public static void loadFile()
        {
            if (!Guide.IsVisible)
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                if (result.IsCompleted)
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        readFile(device);
                    }
                }
            }
        }
        private static void readFile(StorageDevice device)
        {
            IAsyncResult result2 = device.BeginOpenContainer("StorageScore", null, null);
            // Wait for the WaitHandle to become signaled.
            result2.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(result2);
            // Close the wait handle.
            result2.AsyncWaitHandle.Close();
            string filename = "Highscore.sav";
            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return;
            }
            Console.WriteLine("STARTING TO READ FILE");
            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            data = (HighScoreData)serializer.Deserialize(stream);
            Console.WriteLine("THIS IS THE TABLE WHEN READ FROM TABLE");
            for (int index = 0; index < data.SIZE; index++)
            {
                Console.WriteLine(data.PlayerName[index] + " " + data.Score[index]);
            }
            // Close the file.
            stream.Close();
            // Dispose the container.
            container.Dispose();

        }
        private static void createFile(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result2 =
                device.BeginOpenContainer("StorageScore", null, null);

            // Wait for the WaitHandle to become signaled.
            result2.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result2);

            // Close the wait handle.
            result2.AsyncWaitHandle.Close();
            string filename = "Highscore.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
            {
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);
                Console.WriteLine("FILE EXISTS");
            }
            Stream stream = container.CreateFile(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            serializer.Serialize(stream, data);
            stream.Close();
            container.Dispose();
        }
    }
}
