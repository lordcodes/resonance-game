using System.Collections.Generic;
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

            int textureCount = input.ReadInt32();
            for (int i = 0; i < textureCount; i++)
            {
                string textureRef = input.ReadString();
                double frameDelay = input.ReadDouble();
                bool paused = input.ReadBoolean();
                int frameCount = input.ReadInt32();
                List<Texture2D> frameList = new List<Texture2D>();
                for (int j = 0; j < frameCount; j++)
                {
                    frameList.Add(input.ReadObject<Texture2D>());
                }
                TextureAnimation textureAnimation = new TextureAnimation(frameList, TextureAnimation.START.PLAYING, frameDelay);
                models.addTextureAnimation(textureRef, textureAnimation);
            }


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
                bool animation;
                int gameModelNum;
                string textureRef;
                graphicsModel = input.ReadInt32();
                graphicsScale = input.ReadObject<Matrix>();
                physicsModel = input.ReadInt32();
                physicsScale = input.ReadObject<Matrix>();
                textureRef = input.ReadString();
                animation = input.ReadBoolean();
                gameModelNum = input.ReadInt32();
                GameModel newModel = new GameModel(models.addModelFromRef(graphicsModel), graphicsScale, models.addModelFromRef(physicsModel), physicsScale, models.getTextureAnimationOriginal(textureRef), animation);
                
                models.addModel(newModel, gameModelNum);
            }

            return models;
        }
    }
}
