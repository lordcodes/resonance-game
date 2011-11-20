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
        private static Dictionary<string, Vector2> dictionary = new Dictionary<string, Vector2>();

        public Hud(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
        }

        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Drawing/DebugFont");
        }

        public void drawDebugInfo(String text, Vector2 coords)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            resetGraphics();
        }

        public void Draw()
        {
            /*spriteBatch.Begin();
            foreach (KeyValuePair<string, Vector2> pair in dictionary)
            {
                spriteBatch.DrawString(font, "BV", pair.Value, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            resetGraphics();*/
        }

        public void updateEnemyRadar(string name, Vector3 pos)
        {
            Vector2 newpos = new Vector2(500 + pos.X , 200+ pos.Z);
            if (dictionary.ContainsKey(name)) dictionary[name] = newpos;
            else dictionary.Add(name, newpos);
        }

        private void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

    }
}
