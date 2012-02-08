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
using AnimationLibrary;

namespace Resonance
{
    public class ImportedGameModel : GameComponent
    {
        private Model graphicsModel;
        private Model physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private Matrix[] modelTransforms;
        private Texture2D texture;
        bool animation;
        private float mass;
        private AnimationPlayer animPlayer = null;

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

        public float Mass
        {
            get
            {
                return mass;
            }
        }

        public bool Animation
        {
            get
            {
                return animation;
            }
        }

        public AnimationPlayer AnimPlayer
        {
            get
            {
                return animPlayer;
            }
        }

        public Matrix[] Bones
        {
            get
            {
                return animPlayer.GetSkinTransforms();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(animPlayer != null)animPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        public override void Initialize()
        {
        }

        public ImportedGameModel(Model newGraphicsModel, Matrix graphicsModelScale, Model newPhysicsModel, Matrix physicsModelScale, Texture2D newTexture, bool animation)
            : base(Program.game)
        {
            graphicsModel = newGraphicsModel;
            physicsModel = newPhysicsModel;
            texture = newTexture;
            modelTransforms = new Matrix[GraphicsModel.Bones.Count];
            GraphicsModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
            graphicsScale = graphicsModelScale;
            physicsScale = physicsModelScale;
            this.animation = animation;
            mass = 25f;
            //Set up animation

            if (Animation)
            {
                SkinningData skinningData = graphicsModel.Tag as SkinningData;
                if (skinningData != null)
                {
                    animPlayer = new AnimationPlayer(skinningData);
                    AnimationClip clip = skinningData.AnimationClips["Take 001"];
                    animPlayer.StartClip(clip);
                }
            }
        }

    }
}
