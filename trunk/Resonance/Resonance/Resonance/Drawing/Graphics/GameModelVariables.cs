using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AnimationLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class GameModelVariables : GameComponent
    {
        GameModel gameModel;
        private AnimationPlayer animPlayer = null;
        private int currentFrame = 0;
        private float timeElapsed;

        public Texture2D Texture
        {
            get
            {
                return gameModel.getTexture(currentFrame);
            }
        }

        public Matrix[] Bones
        {
            get
            {
                return animPlayer.GetSkinTransforms();
            }
        }

        public void SetTexture(int index)
        {
            if (index < gameModel.Textures.Count)
            {
                currentFrame = index;
            }
        }

        public GameModelVariables(int gameModelNumber)
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
        }

        public override void Update(GameTime gameTime)
        {
            if (animPlayer != null) animPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            if (gameModel.FrameDelay > 0 && gameModel.Textures.Count > 1)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeElapsed > gameModel.FrameDelay)
                {
                    timeElapsed -= gameModel.FrameDelay;
                    currentFrame++;
                    if (currentFrame >= gameModel.Textures.Count)
                    {
                        currentFrame = 0;
                    }
                }


            }
        }

        public override void Initialize()
        {
        }
    }
}
