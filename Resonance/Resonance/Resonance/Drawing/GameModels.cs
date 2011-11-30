using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class GameModels
    {
        public static readonly int TREE             = 1;
        public static readonly int HOUSE            = 2;
        public static readonly int BAD_VIBE         = 3;
        public static readonly int GOOD_VIBE        = 4;
        public static readonly int GROUND           = 5;
        public static readonly int MUSHROOM         = 6;
        public static readonly int SHOCKWAVE_GREEN  = 7;
        public static readonly int SHOCKWAVE_YELLOW = 8;
        public static readonly int SHOCKWAVE_BLUE   = 9;
        public static readonly int SHOCKWAVE_RED    = 10;
        public static readonly int SHOCKWAVE_CYMBAL = 11;
        public static readonly int BAD_VIBE_GREEN   = 12;
        public static readonly int BAD_VIBE_YELLOW  = 13;
        public static readonly int BAD_VIBE_BLUE    = 14;
        public static readonly int BAD_VIBE_RED     = 15;
        public static readonly int BAD_VIBE_CYMBAL  = 16;

        private static GameModel Tree;
        private static GameModel Bad_vibe;
        private static GameModel Bad_vibe_green;
        private static GameModel Bad_vibe_yellow;
        private static GameModel Bad_vibe_blue;
        private static GameModel Bad_vibe_red;
        private static GameModel Bad_vibe_cymbal;
        private static GameModel Good_vibe;
        private static GameModel Ground;
        private static GameModel Mushroom;
        private static GameModel Shockwave_green;
        private static GameModel Shockwave_yellow;
        private static GameModel Shockwave_blue;
        private static GameModel Shockwave_red;
        private static GameModel Shockwave_cymbal;

        private static ContentManager Content;

        /// <summary>
        /// Creates a new GameModels object and stores all the GameModel objects in one place
        /// to be grabbed by the Drawing class
        /// </summary>
        /// <param name="Content">Pass it the content manager to load textures</param>
        public static void Init(ContentManager newContent)
        {
            Content = newContent;
        }

        /// <summary>
        /// Load all the models for the game
        /// </summary>
        public static void Load()
        {
            Tree             = new GameModel(Content, "Drawing/Models/tree", 1f, "Drawing/Models/tree", 1f, "");
            Ground           = new GameModel(Content, "Drawing/Models/terrain", 20f, "Drawing/Models/terrain", 20f, "Drawing/Textures/texGrassReal");
            Good_vibe        = new GameModel(Content, "Drawing/Models/truck", 1f, "Drawing/Models/truck", 1f, "");
            Bad_vibe         = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "");
            Bad_vibe_green   = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "Drawing/Textures/texGreen");
            Bad_vibe_yellow  = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "Drawing/Textures/texYellow");
            Bad_vibe_blue    = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "Drawing/Textures/texBlue");
            Bad_vibe_red     = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "Drawing/Textures/texRed");
            Bad_vibe_cymbal  = new GameModel(Content, "Drawing/Models/virus", 0.5f, "Drawing/Models/box", 1f, "Drawing/Textures/texCymbal");
            Mushroom         = new GameModel(Content, "Drawing/Models/house", 1f, "Drawing/Models/house", 1f, "");
            Shockwave_green  = new GameModel(Content, "Drawing/Models/wave", 1f, "Drawing/Models/wave", 1f, "Drawing/Textures/texGreen");
            Shockwave_yellow = new GameModel(Content, "Drawing/Models/wave", 1f, "Drawing/Models/wave", 1f, "Drawing/Textures/texYellow");
            Shockwave_blue   = new GameModel(Content, "Drawing/Models/wave", 1f, "Drawing/Models/wave", 1f, "Drawing/Textures/texBlue");
            Shockwave_red    = new GameModel(Content, "Drawing/Models/wave", 1f, "Drawing/Models/wave", 1f, "Drawing/Textures/texRed");
            Shockwave_cymbal = new GameModel(Content, "Drawing/Models/wave", 1f, "Drawing/Models/wave", 1f, "Drawing/Textures/texCymbal");
        }

        /// <summary>
        /// Returns a GameModel object which contains the Model and scale information
        /// </summary>
        /// <param name="name">Pass it the name of the model e.g GameModels.TREE</param>
        public static GameModel getModel(int name)
        {
            if (name == TREE) return Tree;
            if (name == GOOD_VIBE) return Good_vibe;
            if (name == BAD_VIBE) return Bad_vibe;
            if (name == BAD_VIBE_GREEN) return Bad_vibe_green;
            if (name == BAD_VIBE_YELLOW) return Bad_vibe_yellow;
            if (name == BAD_VIBE_BLUE) return Bad_vibe_blue;
            if (name == BAD_VIBE_RED) return Bad_vibe_red;
            if (name == BAD_VIBE_CYMBAL) return Bad_vibe_cymbal;
            if (name == GROUND) return Ground;
            if (name == MUSHROOM) return Mushroom;
            if (name == SHOCKWAVE_GREEN) return Shockwave_green;
            if (name == SHOCKWAVE_YELLOW) return Shockwave_yellow;
            if (name == SHOCKWAVE_BLUE) return Shockwave_blue;
            if (name == SHOCKWAVE_RED) return Shockwave_red;
            if (name == SHOCKWAVE_CYMBAL) return Shockwave_cymbal;
            return null;
        }
    }
}
