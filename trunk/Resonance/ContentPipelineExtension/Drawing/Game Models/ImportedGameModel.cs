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
        private string textureRef;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private List<TextureContent> textures = new List<TextureContent>();
        private bool animation;
        private List<float[]> masterBuffer;

        public string TextureRef
        {
            get
            {
                return textureRef;
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

        public ImportedGameModel(int newGraphicsModel, float graphicsModelScale, int newPhysicsModel, float physicsModelScale, string textureRef, bool animation)
        {
            this.graphicsModelFile = newGraphicsModel;
            this.physicsModelFile = newPhysicsModel;
            this.graphicsScale = Matrix.CreateScale(graphicsModelScale, graphicsModelScale, graphicsModelScale);
            this.physicsScale = Matrix.CreateScale(physicsModelScale, physicsModelScale, physicsModelScale);
            this.animation = animation;
            this.textureRef = textureRef;
            this.graphicsScaleFloat = graphicsModelScale;
            this.physicsScaleFloat = physicsModelScale;

            //DisplacementMap dispMap = new DisplacementMap(128, 128);
            //this.masterBuffer = dispMap.masterBuffer;
        }

    }
}
