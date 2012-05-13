using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    public class ImportedGameModels
    {
        private Dictionary<int, GameModel> gameModels = new Dictionary<int, GameModel>();
        private Dictionary<int, Model> models = new Dictionary<int, Model>();
        private Dictionary<string, TextureAnimation> textureAnimations = new Dictionary<string, TextureAnimation>();
        public List<float[]> masterBuffer;

        public ImportedGameModels()
        {
            //dispMap = new DisplacementMap(Graphics.DISP_WIDTH, Graphics.DISP_WIDTH);
        }

        public TextureAnimation getTextureAnimationOriginal(string id)
        {
            if (textureAnimations.ContainsKey(id))
            {
                return textureAnimations[id];
            }
            return new TextureAnimation(new List<Texture2D>(), TextureAnimation.START.PAUSED, 0);
        }

        public TextureAnimation getTextureAnimationInstance(string id)
        {
            return textureAnimations[id].Copy;
        }

        public void addTextureAnimation(string id, TextureAnimation textureAnimation)
        {
            textureAnimations.Add(id, textureAnimation);
        }

        public void addModelRef(int id, Model model)
        {
            models.Add(id, model);
        }

        public Model addModelFromRef(int id)
        {
            return models[id];
        }

        public void addModel(GameModel gameModel, int gameModelNumber)
        {
            gameModels.Add(gameModelNumber, gameModel);
        }

        public GameModel getModel(int modelNum)
        {
            return gameModels[modelNum];
        }
    }
}

