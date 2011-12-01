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
            ImportedGameModels models = new ImportedGameModels();
            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Model graphicsModel = input.ReadObject<Model>();
                Matrix graphicsScale = input.ReadObject<Matrix>();
                Model physicsModel = input.ReadObject<Model>();
                Matrix physicsScale = input.ReadObject<Matrix>();
                Texture2D texture = input.ReadObject<Texture2D>();
                int gameModelNum =  input.ReadInt32();
                models.addModel(new ImportedGameModel(graphicsModel, graphicsScale, physicsModel, physicsScale, texture), gameModelNum);
            }

            return models;
        }
    }
}
