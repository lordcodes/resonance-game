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
        public static readonly int TREE = 1;
        public static readonly int HOUSE = 2;
        public static readonly int BAD_VIBE = 3;
        public static readonly int GOOD_VIBE = 4;
        public static readonly int GROUND = 5;
        public static readonly int MUSHROOM = 6;

        private static GameModel Tree;
        private static GameModel Bad_vibe;
        private static GameModel Good_vibe;
        private static GameModel Ground;
        private static GameModel Mushroom;

        private ContentManager Content;

        /// <summary>
        /// Creates a new GameModels object and stores all the GameModel objects in one place
        /// to be grabbed by the Drawing class
        /// </summary>
        /// <param name="Content">Pass it the content manager to load textures</param>
        public GameModels(ContentManager newContent)
        {
            Content = newContent;
        }

        /// <summary>
        /// Load all the models for the game
        /// </summary>
        public void Load()
        {
            Tree = new GameModel(Content, "Drawing/Models/basicTree", 0.5f);
            Ground = new GameModel(Content, "Drawing/Models/terrain", 1f);
            Good_vibe = new GameModel(Content, "Drawing/Models/box", 1f);
            Bad_vibe = new GameModel(Content, "Drawing/Models/box", 1f);
            Mushroom = new GameModel(Content, "Drawing/Models/bender", 1f);
        }

        /// <summary>
        /// Returns a GameModel object which contains the Model and scale information
        /// </summary>
        /// <param name="name">Pass it the name of the model e.g GameModels.TREE</param>
        public GameModel getModel(int name)
        {
            if (name == TREE) return Tree;
            if (name == GOOD_VIBE) return Good_vibe;
            if (name == BAD_VIBE) return Bad_vibe;
            if (name == GROUND) return Ground;
            if (name == MUSHROOM) return Mushroom;
            return null;
        }
    }
}
