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
    public class ImportedGameModels
    {
        private Dictionary<int, ImportedGameModel> gameModels = new Dictionary<int, ImportedGameModel>();
        private Dictionary<int, Model> models = new Dictionary<int, Model>();

        public void addModelRef(int id, Model model)
        {
            models.Add(id, model);
        }

        public Model addModelFromRef(int id)
        {
            return models[id];
        }

        public void addModel(ImportedGameModel gameModel, int gameModelNumber)
        {
            gameModels.Add(gameModelNumber, gameModel);
        }

        public ImportedGameModel getModel(int modelNum)
        {
            return gameModels[modelNum];
        }
    }
}

