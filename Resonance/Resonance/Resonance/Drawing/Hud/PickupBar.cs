using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
     abstract class PickupBar
    {
        private GraphicsDeviceManager graphics;
        private Texture2D HudTextureBare;//empty bar with border
        private Texture2D HudTextureFull;//full health gradient
        private Texture2D HudAlpha;//sliding overlay to mask health gradient
        private RenderTarget2D HudBuffer;

        public PickupBar(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public abstract void loadTextures(ContentManager content);
        public abstract void saveTexture(SpriteBatch spriteBatch, int value);
        public abstract void draw(SpriteBatch spriteBatch);

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public Texture2D HUDTextureBare
        {
            get { return HudTextureBare; }
            set { HudTextureBare = value; }
        }

        public Texture2D HUDTextureFull
        {
            get { return HudTextureFull; }
            set { HudTextureFull = value; }
        }

        public Texture2D HUDAlpha
        {
            get { return HudAlpha; }
            set { HudAlpha = value; }
        }

        public RenderTarget2D HUDBuffer
        {
            get { return HudBuffer; }
            set { HudBuffer = value; }
        }
    }
}
