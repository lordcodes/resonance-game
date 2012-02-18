using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    public class ImportedTextureAnimation
    {
        private List<string> textureStrings;
        private List<TextureContent> textureContents;
        private double frameDelay;
        private bool paused = false;

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
                return paused;
            }
        }

        public List<string> TextureStrings
        {
            get
            {
                return textureStrings;
            }
        }

        public List<TextureContent> TextureContents
        {
            get
            {
                return textureContents;
            }
        }

        public ImportedTextureAnimation(List<string> textureStrings, double frameDelay, bool textureAnimPaused)
        {
            this.textureStrings = textureStrings;
            this.frameDelay = frameDelay;
            this.paused = textureAnimPaused;
        }

        public ImportedTextureAnimation(List<TextureContent> textureContents, double frameDelay, bool textureAnimPaused)
        {
            this.textureContents = textureContents;
            this.frameDelay = frameDelay;
            this.paused = textureAnimPaused;
        }
    }
}
