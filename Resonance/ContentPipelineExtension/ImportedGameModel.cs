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
        private String textureFile;

        private ModelContent graphicsModel;
        private ModelContent physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private TextureContent texture;

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

        public string TextureFile
        {
            get
            {
                return textureFile;
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

        public TextureContent Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
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
                if (physicsModel == null) return graphicsModelFile;
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

        public ImportedGameModel(int newGraphicsModel, float graphicsModelScale, int newPhysicsModel, float physicsModelScale, String newTexture)
        {
            graphicsModelFile = newGraphicsModel;
            physicsModelFile = newPhysicsModel;
            textureFile = newTexture;
            graphicsScaleFloat = graphicsModelScale;
            physicsScaleFloat = physicsModelScale;
        }

        public ImportedGameModel(int newGraphicsModel, float graphicsModelScale, int newPhysicsModel, float physicsModelScale, TextureContent newTexture)
        {
            graphicsModelFile = newGraphicsModel;
            physicsModelFile = newPhysicsModel;
            texture = newTexture;
            graphicsScale = Matrix.CreateScale(graphicsModelScale, graphicsModelScale, graphicsModelScale);
            physicsScale = Matrix.CreateScale(physicsModelScale, physicsModelScale, physicsModelScale);
        }

    }
}
