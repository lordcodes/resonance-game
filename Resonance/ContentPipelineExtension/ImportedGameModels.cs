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
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace ContentPipelineExtension
{
    public class ImportedGameModels
    {
        private Dictionary<int, ImportedGameModel> gameModels = new Dictionary<int, ImportedGameModel>();
        private Dictionary<int, string> modelsString = new Dictionary<int, string>();
        private Dictionary<string, int> modelsRef = new Dictionary<string, int>();
        private Dictionary<int, ModelContent> models = new Dictionary<int, ModelContent>();

        public void addModel(ImportedGameModel gameModel, int gameModelNumber)
        {
            gameModels.Add(gameModelNumber, gameModel);
        }

        public Dictionary<int, ImportedGameModel> getModels()
        {
            return gameModels;
        }

        public Dictionary<int, string> getModelStringDic()
        {
            return modelsString;
        }

        public void addModelRef(int modelCount, ModelContent modelFile)
        {
            models.Add(modelCount, modelFile);
        }

        public Dictionary<int, ModelContent> getModelDic()
        {
            return models;
        }

        public void addModelRef(int modelCount, string modelFile)
        {
            modelsString.Add(modelCount, modelFile);
            modelsRef.Add(modelFile,modelCount);
        }

        public int getModelRef(string modelFile)
        {
            return modelsRef[modelFile];
        }

        public string getModelString(int modelFile)
        {
            return modelsString[modelFile];
        }
    }
}

