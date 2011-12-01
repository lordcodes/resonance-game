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
    public class ImportedGameModel
    {
        private Model graphicsModel;
        private Model physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private Matrix[] modelTransforms;
        private Texture2D texture;
        private float Mass;

        public Texture2D Texture
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

        public Matrix[] ModelTransforms
        {
            get
            {
                return modelTransforms;
            }
        }

        public Model GraphicsModel
        {
            get
            {
                return graphicsModel;
            }
        }

        public Model PhysicsModel
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

        public ImportedGameModel(Model newGraphicsModel, Matrix graphicsModelScale, Model newPhysicsModel, Matrix physicsModelScale, Texture2D newTexture)
        {
            graphicsModel = newGraphicsModel;
            physicsModel = newPhysicsModel;
            texture = newTexture;
            modelTransforms = new Matrix[GraphicsModel.Bones.Count];
            GraphicsModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
            graphicsScale = graphicsModelScale;
            physicsScale = physicsModelScale;
            Mass = 10f;
        }

    }
}
