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
                Model graphicsModel;
                Matrix graphicsScale;
                Model physicsModel;
                Matrix physicsScale;
                Texture2D texture;
                graphicsModel = input.ReadObject<Model>();
                graphicsScale = input.ReadObject<Matrix>();
                physicsModel = input.ReadObject<Model>();
                physicsScale = input.ReadObject<Matrix>();
                texture = input.ReadObject<Texture2D>();
                int gameModelNum = input.ReadInt32();
                models.addModel(new ImportedGameModel(graphicsModel, graphicsScale, physicsModel, physicsScale, texture), gameModelNum);
            }

            return models;
        }
    }
}
