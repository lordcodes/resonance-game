using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentPipelineExtension
{
    [ContentImporter(".md", DisplayName = "Model Importer", DefaultProcessor="GameModelsProcessor")]
    class ModelImporter : ContentImporter<ImportedGameModels>
    {   
        Dictionary<string, int> gameModels = new Dictionary<string, int>();

        public override ImportedGameModels Import(string filename, ContentImporterContext context)
        {
            
            ImportedGameModels models = new ImportedGameModels();
            using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
            {
                int count = 0;
                int modelCount = 0;
                while (!reader.EndOfStream)
                {
                    string current = reader.ReadLine();
                    if (!current.Equals("") && current.ToCharArray()[0] != '-')
                    {
                        string[] values = current.Split(',');
                        for (int i = 0; i < values.Length; i++) values[i] = values[i].Trim();
                        if (values.Length == 2)
                        {
                            string modelFile = getPath(filename, values[1]);
                            gameModels.Add(values[0], modelCount);
                            models.addModelRef(modelCount, modelFile);
                            modelCount++;
                        }
                        else if (values.Length > 4)
                        {
                            int modelNum = int.Parse(values[0]);
                            int graphicsModel = getModelRef(values[1]);
                            float graphicsScale = float.Parse(values[2]);
                            int physicsModel = getModelRef(values[3]);
                            float physicsScale = float.Parse(values[4]);
                            string texture = "";
                            if (values.Length > 5) texture = getPath(filename, values[5]);
                            ImportedGameModel model = new ImportedGameModel(graphicsModel, graphicsScale, physicsModel, physicsScale, texture);
                            models.addModel(model, modelNum);
                            count++;
                        }
                    }
                }
            }
            return models;
        }

        private string getPath(string input, string file)
        {
            return Path.Combine(Path.GetDirectoryName(input), file);
        }

        private int getModelRef(string name)
        {
            return gameModels[name];
        }
    }

    [ContentProcessor(DisplayName = "GameModelsProcessor")]
    public class ModelsProcessor : ContentProcessor<ImportedGameModels, ImportedGameModels>
    {
        public override ImportedGameModels Process(ImportedGameModels input, ContentProcessorContext context)
        {
            ImportedGameModels models = new ImportedGameModels();
            Dictionary<int, ImportedGameModel> gameModels = input.getModels();
            Dictionary<int, string> modelsRef = input.getModelStringDic();
            foreach (KeyValuePair<int, string> pair in modelsRef)
            {
                ModelContent model = loadModel(pair.Value, context);
                models.addModelRef(pair.Key, model);
            }

            foreach (KeyValuePair<int, ImportedGameModel> pair in gameModels)
            {
                ImportedGameModel model = pair.Value;
                ExternalReference<TextureContent> textureRef;
                TextureContent texture = null;
                if(!model.TextureFile.Equals(""))
                {
                    textureRef = new ExternalReference<TextureContent>(model.TextureFile);
                    texture = context.BuildAndLoadAsset<TextureContent, TextureContent>(textureRef, "TextureProcessor");
                }
                //int graphicsModel = loadModel(input.getModelString(model.GraphicsModelFile), context);
                int graphicsModel = model.GraphicsModelFile;
                int physicsModel = model.PhysicsModelFile;

                ImportedGameModel newModel = new ImportedGameModel(graphicsModel, model.GraphicsScaleFloat, physicsModel, model.PhysicsScaleFloat, texture);
                models.addModel(newModel, pair.Key);
            }
            return models;
        }

        private ModelContent loadModel(string file, ContentProcessorContext context)
        {
            return context.BuildAndLoadAsset<NodeContent, ModelContent>(new ExternalReference<NodeContent>(file), "ModelProcessor");
        }
    }

    [ContentTypeWriter]
    public class GameModelsContentWriter : ContentTypeWriter<ImportedGameModels>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Resonance.GameModelsContentReader, Resonance";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Resonance.ImportedGameModels, Resonance";
        }

        protected override void Write(ContentWriter output, ImportedGameModels value)
        {
            Dictionary<int, ImportedGameModel> gameModels = value.getModels();
            Dictionary<int, ModelContent> models = value.getModelDic();
            output.Write(models.Count);
            foreach (KeyValuePair<int, ModelContent> pair in models)
            {
                output.WriteObject<ModelContent>(pair.Value);
                output.Write(pair.Key);
            }
            output.Write(gameModels.Count);
            foreach (KeyValuePair<int, ImportedGameModel> pair in gameModels)
            {
                ImportedGameModel model = pair.Value;
                output.Write(model.GraphicsModel);
                output.WriteObject<Matrix>(model.GraphicsScale);
                output.Write(model.PhysicsModel);
                output.WriteObject<Matrix>(model.PhysicsScale);
                output.WriteObject<TextureContent>(model.Texture);
                output.Write(pair.Key);
            }
        }

    }

}
