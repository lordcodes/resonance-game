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
    class HighScoreManager
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
        public static string HighScoresFilename = "HighScore.lst";

        public static void SaveHighScores(HighScoreData data, string filename)
        {
            // Get the path of the save game
            string fullpath = filename;

            // Open the file, creating it if necessary
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        public static HighScoreData LoadHighScores(string filename)
        {
            HighScoreData data;

            // Get the path of the save game
            string fullpath =  filename;

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate,
            FileAccess.Read);
            try
            {

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (data);
        }
        public static void Initialize()
        {
            // Get the path of the save game
            string fullpath = HighScoresFilename;

            // Check to see if the save exists
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                // Create the data to save
                HighScoreData data = new HighScoreData(5);
                data.PlayerName[0] = "Neil";
                data.Score[0] = 50;

                data.PlayerName[1] = "Shawn";
                data.Score[1] = 40;

                data.PlayerName[2] = "Mark";
                data.Score[2] = 30;

                data.PlayerName[3] = "Cindy";
                data.Score[3] = 20;

                data.PlayerName[4] = "Sam";
                data.Score[4] = 10;

                SaveHighScores(data, HighScoresFilename);
            }

        }

    }
}
