using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AnimationLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    /// <summary>
    /// Class that stores/updates details that are unique to each instance of a GameModel,
    /// this allows animations to run independently.
    /// </summary>
    class GameModelInstance : GameComponent
    {
        private GameModel gameModel;
        private AnimationPlayer animPlayer = null;
        private AnimationClip clip;
        private TextureAnimation textureAnimation;
        private bool modelAnimPaused = false;
        private bool modelAnimPlayOnce = false;

        /// <summary>
        /// Returns the game model of this GameModelInstance.
        /// </summary>
        public GameModel Model
        {
            get
            {
                return gameModel;
            }
        }

        /// <summary>
        /// Returns the texture that should be currently applied to the graphics model.
        /// This could depend on any animated textures that are applied.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return textureAnimation.Texture;
            }
        }

        /// <summary>
        /// Returns the details of the current Bones of the model, this depends on 
        /// the stage of animation.
        /// </summary>
        public Matrix[] Bones
        {
            get
            {
                return animPlayer.GetSkinTransforms();
            }
        }

        /// <summary>
        /// Set the current texture of this GameModelInstance
        /// </summary>
        /// <param name="index">Index of the texture.</param>
        public void setTexture(int index)
        {
            textureAnimation.setTexture(index);
        }

        /// <summary>
        /// Play the model animation
        /// </summary>
        public void playModelAnim()
        {
            modelAnimPaused = false;
        }

        /// <summary>
        /// Pause the model animation
        /// </summary>
        public void pauseModelAnim()
        {
            modelAnimPaused = true;
        }

        /// <summary>
        /// Play the texture animation
        /// </summary>
        public void playTextureAnim()
        {
            textureAnimation.playTextureAnim();
        }

        /// <summary>
        /// Play the texture animation once
        /// </summary>
        public void playTextureAnimOnce()
        {
            textureAnimation.playTextureAnimOnce();
        }

        /// <summary>
        /// Pause the texture animation
        /// </summary>
        public void pauseTextureAnim()
        {
            textureAnimation.pauseTextureAnim();
        }

        /// <summary>
        /// Stores information that relates to the GameModel but is usnique to each object. 
        /// For example what stange of any animation each object is in  
        /// </summary>
        /// <param name="gameModelNumber">The GameModelNumber that this is an instance of</param>
        public GameModelInstance(int gameModelNumber)
            : base(Program.game)
        {
            gameModel = GameModels.getModel(gameModelNumber);

            //Set up model animation
            if (gameModel.ModelAnimation)
            {
                SkinningData skinningData = gameModel.GraphicsModel.Tag as SkinningData;
                if (skinningData != null)
                {
                    animPlayer = new AnimationPlayer(skinningData);
                    clip = skinningData.AnimationClips["Take 001"];
                    animPlayer.StartClip(clip);
                }
            }

            textureAnimation = gameModel.TextureAnimation.Copy;
            
            //Program.game.Components.Add(this);
            DrawableManager.Add(this);
        }

        /// <summary>
        /// Updates any anymations that are associated with this GameModelInstance
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (animPlayer != null && !modelAnimPaused)
            {
                animPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
                if (modelAnimPlayOnce)
                {

                }
            }

            textureAnimation.Update(gameTime);
        }

        public void resetAnimation()
        {
            animPlayer.StartClip(clip);
        }
    }
}
