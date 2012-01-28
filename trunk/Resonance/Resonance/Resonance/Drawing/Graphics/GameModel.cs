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
    public class GameModel : GameComponent
    {
        private Model graphicsModel;
        private Model physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private Matrix[] modelTransforms;
        private List<Texture2D> textures = new List<Texture2D>();
        bool modelAnimation;
        private float mass;
        private AnimationPlayer animPlayer = null;
        private int currentFrame = 0;
        private float frameDelay;
        private float timeElapsed;

        public Texture2D Texture
        {
            get
            {
                if (textures.Count == 0) return null;
                return textures[currentFrame];
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

        public bool ModelAnimation
        {
            get
            {
                return modelAnimation;
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

        public void setTexture(int index)
        {
            if (index < textures.Count)
            {
                currentFrame = index;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(animPlayer != null)animPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            if (frameDelay > 0 && textures.Count > 1)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeElapsed > frameDelay)
                {
                    timeElapsed -= frameDelay;
                    currentFrame++;
                    if (currentFrame >= textures.Count)
                    {
                        currentFrame = 0;
                    }
                }
                    
                
            }
        }

        public override void Initialize()
        {
        }

        public GameModel(Model newGraphicsModel, Matrix graphicsModelScale, Model newPhysicsModel, Matrix physicsModelScale, List<Texture2D> newTextures, bool modelAnimation, float frameDelay)
            : base(Program.game)
        {
            graphicsModel = newGraphicsModel;
            physicsModel = newPhysicsModel;
            textures = newTextures;
            modelTransforms = new Matrix[GraphicsModel.Bones.Count];
            GraphicsModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
            graphicsScale = graphicsModelScale;
            physicsScale = physicsModelScale;
            this.modelAnimation = modelAnimation;
            this.frameDelay = frameDelay;
            mass = 25f;

            //Set up model animation
            if (ModelAnimation)
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
