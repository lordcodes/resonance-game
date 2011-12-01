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
        private String graphicsModelFile;
        private String physicsModelFile;
        private float graphicsScaleFloat;
        private float physicsScaleFloat;
        private String textureFile;

        private ModelContent graphicsModel;
        private ModelContent physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private TextureContent texture;
        private float Mass;

        public string GraphicsModelFile
        {
            get
            {
                return graphicsModelFile;
            }
        }

        public string PhysicsModelFile
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

        public ModelContent GraphicsModel
        {
            get
            {
                return graphicsModel;
            }
        }

        public ModelContent PhysicsModel
        {
            get
            {
                if (physicsModel == null) return graphicsModel;
                return physicsModel;
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

        public float mass
        {
            get
            {
                return Mass;
            }
        }

        public ImportedGameModel(String newGraphicsModel, float graphicsModelScale, String newPhysicsModel, float physicsModelScale, String newTexture)
        {
            graphicsModelFile = newGraphicsModel;
            physicsModelFile = newPhysicsModel;
            textureFile = newTexture;
            graphicsScaleFloat = graphicsModelScale;
            physicsScaleFloat = physicsModelScale;
            Mass = 10f;
        }

        public ImportedGameModel(ModelContent newGraphicsModel, float graphicsModelScale, ModelContent newPhysicsModel, float physicsModelScale, TextureContent newTexture)
        {
            graphicsModel = newGraphicsModel;
            physicsModel = newPhysicsModel;
            texture = newTexture;
            graphicsScale = Matrix.CreateScale(graphicsModelScale, graphicsModelScale, graphicsModelScale);
            physicsScale = Matrix.CreateScale(physicsModelScale, physicsModelScale, physicsModelScale);
            Mass = 10f;
        }

    }
}
