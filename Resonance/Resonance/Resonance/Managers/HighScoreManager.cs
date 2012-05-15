using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.GamerServices;

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
        public static int position = -1;
        public static StorageDevice device = null;
        public static void initializeData()
        {
            data = new HighScoreData(10);
            data.PlayerName[0] = "Alex Sheppard";
            data.Score[0] = 0;
            data.PlayerName[1] = "Andrew Lord";
            data.Score[1] = 0;
            data.PlayerName[2] = "Mihai Nemes";
            data.Score[2] = 0;
            data.PlayerName[3] = "Michael Jones";
            data.Score[3] = 0;
            data.PlayerName[4] = "Philip Tattersall";
            data.Score[4] = 0;
            data.PlayerName[5] = "Tom Pickering";
            data.Score[5] = 0;
            data.PlayerName[6] = "Chuck Norris";
            data.Score[6] = 0;
            data.PlayerName[7] = "Bruce Lee";
            data.Score[7] = 0;
            data.PlayerName[8] = "Rocky Balboa";
            data.Score[8] = 0;
            data.PlayerName[9] = "Muhammad Ali";
            data.Score[9] = 0;
        }

        public static void updateTable(int score)
        {
            
            position = -1;
            int scoreIndex = -1;
            for (int i = 0; i < data.SIZE; i++)
            {
                if (score >= data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }
            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.SIZE - 1; i > scoreIndex; i--)
                {                
                    data.Score[i] = data.Score[i - 1];
                    data.PlayerName[i] = data.PlayerName[i - 1];
                }                
                data.Score[scoreIndex] = score;
                data.PlayerName[scoreIndex] = "Player";
            }
            position = scoreIndex;
            //Console.WriteLine("THIS IS THE UPDATED VERSION OF THE TABLE THAT WILL BE SAVED");
            //for (int index = 0; index < data.SIZE; index++)
            //{
            //    Console.WriteLine(index+1 + " " + data.PlayerName[index] + " " + data.Score[index]);
            //}
        }       
        public static void saveFile()
        {
                    if (device != null && device.IsConnected)
                    {
                        createFile();
                    }            
        }
        public static void loadFile()
        {            
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                while (!result.IsCompleted) { };
                if (result.IsCompleted)
                {
                    device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        readFile();
                    }
                }                       
        }
        private static void readFile()
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
                initializeData();
                container.Dispose();
                //Console.WriteLine("The file does not exist");
                return;
            }
            //Console.WriteLine("STARTING TO READ FILE");
            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            data = (HighScoreData)serializer.Deserialize(stream);
            //Console.WriteLine("THIS IS THE TABLE WHEN READ FROM TABLE");
            //for (int index = 0; index < data.SIZE; index++)
            //{
            //    Console.WriteLine(index+1 + " " + data.PlayerName[index] + " " + data.Score[index]);
            //}
            // Close the file.
            stream.Close();
            // Dispose the container.
            container.Dispose();
        }
        private static void createFile()
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
                //Console.WriteLine("FILE EXISTS");
            }
            Stream stream = container.CreateFile(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            serializer.Serialize(stream, data);
            stream.Close();
            container.Dispose();
        }
    }
}
