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
    class Hud
    {
        private static SpriteBatch spriteBatch;
        private static SpriteFont font;
        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;

        public Hud(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
        }

        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("DebugFont");
        }

        public void drawDebugInfo(String text, Vector2 coords)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
