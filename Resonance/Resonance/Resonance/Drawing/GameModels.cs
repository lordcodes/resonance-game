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
using AnimationLibrary;


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

        private static ContentManager Content;

        private static ImportedGameModels importedGameModels;

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
            // Edit "Content/Drawing/modelDetails.md" to add new models to the game
            importedGameModels = Content.Load<ImportedGameModels>("Drawing/modelDetails");
            SkinningData skinningData = importedGameModels.getModel(BAD_VIBE_BLUE).GraphicsModel.Tag as SkinningData;
            if (skinningData == null)
               Console.WriteLine("This model does not contain a SkinningData tag.");

            Program.game.animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            Program.game.animationPlayer.StartClip(clip);

        }

        /// <summary>
        /// Returns a GameModel object which contains the Model and scale information
        /// </summary>
        /// <param name="name">Pass it the name of the model e.g GameModels.TREE</param>
        public static ImportedGameModel getModel(int name)
        {
            return importedGameModels.getModel(name);
        }
    }
}
