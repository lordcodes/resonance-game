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
using System.ComponentModel;
using AnimationLibrary;
using System.Diagnostics;

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
                int section = 0;
                while (!reader.EndOfStream)
                {
                    string current = reader.ReadLine();
                    if (!current.Equals("") && current.ToCharArray()[0] != '-')
                    {
                        string[] values = current.Split(',');
                        for (int i = 0; i < values.Length; i++) values[i] = values[i].Trim();
                        if (values.Length == 1)
                        {
                            string title = values[0];
                            if (title.Equals("MODELS") || title.Equals("TEXTURES")) section++;
                        }
                        else if (values.Length <= 4 && values.Length >= 2)
                        {
                            if (section == 1)
                            {
                                // MODELS
                                string modelFile = getPath(filename, values[1]);
                                gameModels.Add(values[0], modelCount);
                                models.addModelRef(modelCount, modelFile);
                                modelCount++;
                            }
                            else if (section == 2)
                            {
                                // TEXTURES
                                List<string> textureFiles = new List<string>();
                                string[] files = values[1].Split(';');
                                float delay = 0;
                                bool start = false;
                                for (int i = 0; i < files.Length; i++)
                                {
                                    textureFiles.Add(getPath(filename,files[i]));
                                }

                                if (values.Length > 2)
                                {
                                    delay = float.Parse(values[2]);
                                }
                                if (values.Length > 3)
                                {
                                    start = values[3].Equals("1");
                                }

                                ImportedTextureAnimation textureAnimation = new ImportedTextureAnimation(textureFiles, delay, !start);
                                models.addTextureAnimation(values[0], textureAnimation);
                            }
                        }
                        else if (values.Length > 4)
                        {
                            int modelNum = int.Parse(values[0]);
                            int graphicsModel = getModelRef(values[1]);
                            float graphicsScale = float.Parse(values[2]);
                            int physicsModel = getModelRef(values[3]);
                            float physicsScale = float.Parse(values[4]);
                            List<string> textures = new List<string>();
                            bool animation = false;
                            string textureRef = "";
                            if (values.Length > 5)
                            {
                                textureRef = values[5];
                            }
                            if (values.Length > 6)
                            {
                                animation = values[6].Equals("1");
                            }
                            
                            ImportedGameModel model = new ImportedGameModel(graphicsModel, graphicsScale, physicsModel, physicsScale, textureRef, animation);
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
            Dictionary<string, ImportedTextureAnimation> gameTextures = input.getTextures();
            Dictionary<int, string> modelsRef = input.getModelStringDic();
            foreach (KeyValuePair<int, string> pair in modelsRef)
            {
                ExternalReference<NodeContent> source = (new ExternalReference<NodeContent>(pair.Value));
                NodeContent animation_data = (NodeContent)context.BuildAndLoadAsset<NodeContent, Object>(source, "PassThroughProcessor");
                if (context == null)
                    throw new InvalidContentException("ARGGGGGGGGGGG");
                //ValidateMesh(animation_data, context, null);

                // Find the skeleton.
                BoneContent skeleton = MeshHelper.FindSkeleton(animation_data);
                ModelContent model;
                if (skeleton != null)
                {
                    //throw new InvalidContentException("Input skeleton not found.");

                    // We don't want to have to worry about different parts of the model being
                    // in different local coordinate systems, so let's just bake everything.
                    FlattenTransforms(animation_data, skeleton);

                    // Read the bind pose and skeleton hierarchy data.
                    IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

                    if (bones.Count > SkinnedEffect.MaxBones)
                    {
                        throw new InvalidContentException(string.Format(
                            "Skeleton has {0} bones, but the maximum supported is {1}.",
                            bones.Count, SkinnedEffect.MaxBones));
                    }

                    List<Matrix> bindPose = new List<Matrix>();
                    List<Matrix> inverseBindPose = new List<Matrix>();
                    List<int> skeletonHierarchy = new List<int>();

                    foreach (BoneContent bone in bones)
                    {
                        bindPose.Add(bone.Transform);
                        inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                        skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                    }

                    // Convert animation data to our runtime format.
                    Dictionary<string, AnimationClip> animationClips;
                    animationClips = ProcessAnimations(skeleton.Animations, bones);

                    // Chain to the base ModelProcessor class so it can convert the model data.
                    model = context.BuildAndLoadAsset<NodeContent, ModelContent>(new ExternalReference<NodeContent>(pair.Value), "ModelProcessor"); //base.Process(animation_data, context);

                    // Store our custom animation data in the Tag property of the model.
                    model.Tag = new SkinningData(animationClips, bindPose,
                                                 inverseBindPose, skeletonHierarchy);
                }
                else
                {
                    model = loadModel(pair.Value, context);
                }
                models.addModelRef(pair.Key, model);
                
            }

            foreach (KeyValuePair<string, ImportedTextureAnimation> pair in gameTextures)
            {
                ImportedTextureAnimation textureAnim = pair.Value;
                List<TextureContent> textures = new List<TextureContent>();
                List<string> textureFiles = new List<string>();
                ExternalReference<TextureContent> textureContRef;
                string textureRef = pair.Key;
                double frameDelay = 0;
                bool animationStart = false;
                textureFiles = textureAnim.TextureStrings;
                frameDelay = textureAnim.FrameDelay;
                animationStart = !textureAnim.Paused;
                foreach (string textureFile in textureFiles)
                {
                    textureContRef = new ExternalReference<TextureContent>(textureFile);
                    textures.Add(context.BuildAndLoadAsset<TextureContent, TextureContent>(textureContRef, "TextureProcessor"));
                }
                ImportedTextureAnimation textureAnimation = new ImportedTextureAnimation(textures, frameDelay, animationStart);
                models.addTextureAnimation(textureRef, textureAnimation);
            }

            foreach (KeyValuePair<int, ImportedGameModel> pair in gameModels)
            {
                ImportedGameModel model = pair.Value;
                int graphicsModel = model.GraphicsModelFile;
                int physicsModel = model.PhysicsModelFile;
                bool animation = model.Animation;

                ImportedGameModel newModel = new ImportedGameModel(graphicsModel, model.GraphicsScaleFloat, physicsModel, model.PhysicsScaleFloat, model.TextureRef, animation);
                models.addModel(newModel, pair.Key);
            }
            return models;
        }

        private ModelContent loadModel(string file, ContentProcessorContext context)
        {
            return context.BuildAndLoadAsset<NodeContent, ModelContent>(new ExternalReference<NodeContent>(file), "ModelProcessor");
        }

        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContentDictionary
        /// object to our runtime AnimationClip format.
        /// </summary>
        static Dictionary<string, AnimationClip> ProcessAnimations(
            AnimationContentDictionary animations, IList<BoneContent> bones)
        {
            // Build up a table mapping bone names to indices.
            Dictionary<string, int> boneMap = new Dictionary<string, int>();

            for (int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;

                if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // Convert each animation in turn.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = new Dictionary<string, AnimationClip>();

            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);

                animationClips.Add(animation.Key, processed);
            }

            if (animationClips.Count == 0)
            {
                throw new InvalidContentException(
                            "Input file does not contain any animations.");
            }

            return animationClips;
        }


        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContent
        /// object to our runtime AnimationClip format.
        /// </summary>
        static AnimationClip ProcessAnimation(AnimationContent animation,
                                              Dictionary<string, int> boneMap)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            // For each input animation channel.
            foreach (KeyValuePair<string, AnimationChannel> channel in
                animation.Channels)
            {
                // Look up what bone this channel is controlling.
                int boneIndex;

                if (!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    throw new InvalidContentException(string.Format(
                        "Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));
                }

                // Convert the keyframe data.
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex, keyframe.Time,
                                               keyframe.Transform));
                }
            }

            // Sort the merged keyframes by time.
            keyframes.Sort(CompareKeyframeTimes);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new AnimationClip(animation.Duration, keyframes);
        }


        /// <summary>
        /// Comparison function for sorting keyframes into ascending time order.
        /// </summary>
        static int CompareKeyframeTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }


        /// <summary>
        /// Makes sure this mesh contains the kind of data we know how to animate.
        /// </summary>
        static void ValidateMesh(NodeContent node, ContentProcessorContext context,
                                 string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
            {
                // If this is a bone, remember that we are now looking inside it.
                parentBoneName = node.Name;
            }

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (NodeContent child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }


        /// <summary>
        /// Checks whether a mesh contains skininng information.
        /// </summary>
        static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Bakes unwanted transforms into the model geometry,
        /// so everything ends up in the same coordinate system.
        /// </summary>
        static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child == skeleton)
                    continue;

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }


        /// <summary>
        /// Force all the materials to use our skinned model effect.
        /// </summary>
        [DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
        public MaterialProcessorDefaultEffect DefaultEffect
        {
            get { return MaterialProcessorDefaultEffect.SkinnedEffect; }
            set { }
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
            Dictionary<string, ImportedTextureAnimation> textureAnimations = value.TextureAnimations;

            output.Write(textureAnimations.Count);
            foreach (KeyValuePair<string, ImportedTextureAnimation> pair in textureAnimations)
            {
                ImportedTextureAnimation textureAnimation = pair.Value;
                string textureRef = pair.Key;
                output.Write(textureRef);
                output.Write(textureAnimation.FrameDelay);
                output.Write(textureAnimation.Paused);
                output.Write(textureAnimation.TextureContents.Count);
                foreach (TextureContent texture in textureAnimation.TextureContents)
                {
                    output.WriteObject<TextureContent>(texture);
                }
            }

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
                output.Write(model.TextureRef);
                output.Write(model.Animation);
                output.Write(pair.Key);
            }
        }

    }

}
