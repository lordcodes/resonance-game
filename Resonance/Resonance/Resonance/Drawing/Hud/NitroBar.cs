using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class NitroBar : PickupBar
    {
        public NitroBar(GraphicsDeviceManager graphics)
            : base(graphics)
        {
        }

        public override void loadTextures(ContentManager content)
        {
            HUDTextureBare = content.Load<Texture2D>("Drawing/HUD/Textures/pickupemptyy");
            HUDTextureFull = content.Load<Texture2D>("Drawing/HUD/Textures/pickupfully");
            HUDAlpha = content.Load<Texture2D>("Drawing/HUD/Textures/pickupalphay");

            HUDBuffer = new RenderTarget2D(Graphics.GraphicsDevice, HUDTextureBare.Width, HUDTextureBare.Height, true, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public override void saveTexture(SpriteBatch spriteBatch, int value)
        {
            GraphicsDevice gd = Graphics.GraphicsDevice;
            MouseState ms = Mouse.GetState();
            gd.SetRenderTarget(HUDBuffer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(HUDTextureFull, new Vector2(0, ScreenManager.pixelsY(0)), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            BlendState b = new BlendState();
            spriteBatch.GraphicsDevice.BlendState = b;

            int healthLimit = ScreenManager.pixelsX(0) + HUDTextureBare.Width * value / GoodVibe.MAX_NITRO;
            spriteBatch.Draw(HUDAlpha, new Vector2(healthLimit, ScreenManager.pixelsY(0)), Color.White);
            spriteBatch.End();
            gd.SetRenderTarget(null);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle(ScreenManager.pixelsX(30), ScreenManager.pixelsY(10), ScreenManager.pixelsX(HUDTextureBare.Width), ScreenManager.pixelsY(HUDTextureBare.Height));
            Rectangle source = new Rectangle(0, 0, HUDTextureBare.Width, HUDTextureBare.Height);
            spriteBatch.Draw(HUDTextureBare, destination, source, Color.White);
            spriteBatch.Draw((Texture2D)HUDBuffer, destination, source, Color.White);
        }
    }
}
