using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    public class TextureAnimation
    {
        public enum START { PLAYING, PAUSED, PLAYONCE };

        private List<Texture2D> textures;
        private int currentFrame = 0;
        private double frameDelay;
        private double timeElapsed;
        private bool textureAnimPaused = false;
        private bool textureAnimPlayOnce = false;

        public TextureAnimation Copy
        {
            get
            {
                if(Paused) return new TextureAnimation(textures, TextureAnimation.START.PAUSED, frameDelay);
                return new TextureAnimation(textures, TextureAnimation.START.PLAYING, frameDelay);
            }
        }

        public double FrameDelay
        {
            get
            {
                return frameDelay;
            }
        }

        public bool Paused
        {
            get
            {
                return textureAnimPaused;
            }
        }

        public List<Texture2D> Textures
        {
            set
            {
                textures = value;
            }
        }

        /// <summary>
        /// Returns the texture that should be currently displayed.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                if(currentFrame<textures.Count)return textures[currentFrame];
                return null;
            }
        }

        /// <summary>
        /// Set the current texture of this TextureAnimation
        /// </summary>
        /// <param name="index">Index of the texture.</param>
        public void setTexture(int index)
        {
            currentFrame = index;
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

        public void setTexture(int index, Texture2D texture)
        {
            textures[index] = texture;
        }

        public TextureAnimation(List<Texture2D> textures, START state, double frameDelay)
        {
            this.frameDelay = frameDelay;
            this.textures = textures;

            if (state == START.PAUSED)
            {
                textureAnimPaused = true;
            }
            else if (state == START.PLAYONCE)
            {
                textureAnimPaused = false;
                textureAnimPlayOnce = true;
            }
        }

        public TextureAnimation(Texture2D texture)
        {
            this.frameDelay = 40f;
            this.textures = new List<Texture2D>();
            this.textures.Add(texture);
            textureAnimPaused = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!textureAnimPaused && frameDelay > 0 && textures.Count > 1)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeElapsed > frameDelay)
                {
                    timeElapsed -= frameDelay;
                    currentFrame++;
                    if (currentFrame >= textures.Count)
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
