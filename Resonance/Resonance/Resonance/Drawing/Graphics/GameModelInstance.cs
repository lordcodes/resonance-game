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
        private int currentFrame = 0;
        private float timeElapsed;
        private bool textureAnimPaused = false;
        private bool textureAnimPlayOnce = false;

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
                return gameModel.getTexture(currentFrame);
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
            if (index < gameModel.TextureCount)
            {
                currentFrame = index;
            }
        }

        /// <summary>
        /// Play the texture animation
        /// </summary>
        public void playTextureAnim()
        {
            textureAnimPaused = false;
        }

        /// <summary>
        /// Play the texture animation once
        /// </summary>
        public void playTextureAnimOnce()
        {
            textureAnimPaused = false;
            textureAnimPlayOnce = true;
        }

        /// <summary>
        /// Pause the texture animation
        /// </summary>
        public void pauseTextureAnim()
        {
            textureAnimPaused = true;
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
                    AnimationClip clip = skinningData.AnimationClips["Take 001"];
                    animPlayer.StartClip(clip);
                }
            }

            textureAnimPaused = !gameModel.TextureAnimationStart;

            Program.game.Components.Add(this);
        }

        /// <summary>
        /// Updates any anymations that is associated with this GameModelInstance
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (animPlayer != null) animPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            if (!textureAnimPaused && gameModel.FrameDelay > 0 && gameModel.TextureCount > 1)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeElapsed > gameModel.FrameDelay)
                {
                    timeElapsed -= gameModel.FrameDelay;
                    currentFrame++;
                    if (currentFrame >= gameModel.TextureCount)
                    {
                        currentFrame = 0;
                        if (textureAnimPlayOnce)
                        {
                            textureAnimPlayOnce = false;
                            textureAnimPaused = true;
                        }
                    }
                }
            }
        }
    }
}
