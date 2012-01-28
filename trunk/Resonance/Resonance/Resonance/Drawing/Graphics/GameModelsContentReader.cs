using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class GameModelsContentReader : ContentTypeReader<ImportedGameModels>
    {
        protected override ImportedGameModels Read(ContentReader input, ImportedGameModels existingInstance)
        {
            Dictionary<string, Model> gameModels = new Dictionary<string, Model>();
            ImportedGameModels models = new ImportedGameModels();
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Model model = input.ReadObject<Model>();
                int modelRef = input.ReadInt32();
                models.addModelRef(modelRef, model);
            }
            count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int graphicsModel;
                Matrix graphicsScale;
                int physicsModel;
                Matrix physicsScale;
                List<Texture2D> textures = new List<Texture2D>();
                bool animation;
                int gameModelNum;
                int numberTextures;
                float frameDelay = 0;
                graphicsModel = input.ReadInt32();
                graphicsScale = input.ReadObject<Matrix>();
                physicsModel = input.ReadInt32();
                physicsScale = input.ReadObject<Matrix>();
                frameDelay = (float)input.ReadDouble();
                numberTextures = input.ReadInt32();
                for (int j = 0; j < numberTextures; j++)
                {
                    textures.Add(input.ReadObject<Texture2D>());
                }
                animation = input.ReadBoolean();
                gameModelNum = input.ReadInt32();
                GameModel newModel = new GameModel(models.addModelFromRef(graphicsModel), graphicsScale, models.addModelFromRef(physicsModel), physicsScale, textures, animation, frameDelay);
                Program.game.Components.Add(newModel);
                models.addModel(newModel, gameModelNum);
            }

            return models;
        }
    }
}
