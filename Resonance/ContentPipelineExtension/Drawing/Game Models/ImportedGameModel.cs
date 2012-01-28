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
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    public class ImportedGameModel
    {
        private int graphicsModelFile;
        private int physicsModelFile;
        private float graphicsScaleFloat;
        private float physicsScaleFloat;
        private List<String> textureFiles = new List<string>();
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private List<TextureContent> textures = new List<TextureContent>();
        private bool animation;
        private double frameDelay;

        public double FrameDelay
        {
            get
            {
                return frameDelay;
            }
        }

        public int GraphicsModelFile
        {
            get
            {
                return graphicsModelFile;
            }
        }

        public int PhysicsModelFile
        {
            get
            {
                return physicsModelFile;
            }
        }

        public List<string> TextureFiles
        {
            get
            {
                return textureFiles;
            }
        }

        public float GraphicsScaleFloat
        {
            get
            {
                return graphicsScaleFloat;
            }
        }

        public float PhysicsScaleFloat
        {
            get
            {
                return physicsScaleFloat;
            }
        }

        public List<TextureContent> Textures
        {
            get
            {
                return textures;
            }
        }

        public int GraphicsModel
        {
            get
            {
                return graphicsModelFile;
            }
        }

        public int PhysicsModel
        {
            get
            {
                if (physicsModelFile < 0) return graphicsModelFile;
                return physicsModelFile;
            }
        }

        public Matrix PhysicsScale
        {
            get
            {
                return physicsScale;
            }
        }

        public Matrix GraphicsScale
        {
            get
            {
                return graphicsScale;
            }
        }

        public bool Animation
        {
            get
            {
                return animation;
            }
        }

        public ImportedGameModel(int newGraphicsModel, float graphicsModelScale, int newPhysicsModel, float physicsModelScale, List<String> newTextures, bool animation, double frameDelay)
        {
            graphicsModelFile = newGraphicsModel;
            physicsModelFile = newPhysicsModel;
            textureFiles = newTextures;
            graphicsScaleFloat = graphicsModelScale;
            physicsScaleFloat = physicsModelScale;
            this.animation = animation;
            this.frameDelay = frameDelay;
        }

        public ImportedGameModel(int newGraphicsModel, float graphicsModelScale, int newPhysicsModel, float physicsModelScale, List<TextureContent> newTextures, bool animation, double frameDelay)
        {
            graphicsModelFile = newGraphicsModel;
            physicsModelFile = newPhysicsModel;
            textures = newTextures;
            graphicsScale = Matrix.CreateScale(graphicsModelScale, graphicsModelScale, graphicsModelScale);
            physicsScale = Matrix.CreateScale(physicsModelScale, physicsModelScale, physicsModelScale);
            this.animation = animation;
            this.frameDelay = frameDelay;
        }

    }
}
