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
        private static int Score = 0;
        private static float health = 1;
        private static float worldWidth = 10;
        private static float screenWidth;
        private static float screenHeight;
        private static float widthRatio;
        private static float heightRatio;
        private static Texture2D healthBar;
        private static Texture2D healthSlice;

        public int score
        {
            get
            {
                return Score;
            }

            set
            {
                Score = value;
            }
        }

        public Hud(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
        }

        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Drawing/Fonts/DebugFont");
            healthBar = Content.Load<Texture2D>("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>("Drawing/HUD/Textures/healthSlice");
            screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            widthRatio =  screenWidth/1920;
            heightRatio =  screenHeight/1080;
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
            spriteBatch.Begin();
            foreach (KeyValuePair<string, Vector2> pair in dictionary)
            {
                spriteBatch.DrawString(font, "BV", pair.Value, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            drawHealthBar();
            spriteBatch.End();
            resetGraphics();
        }

        public void updateEnemyRadar(string name, Vector3 pos)
        {
            Vector2 newpos = new Vector2(500 + pos.X , 200+ pos.Z);
            if (dictionary.ContainsKey(name)) dictionary[name] = newpos;
            else dictionary.Add(name, newpos);
        }

        private void drawHealthBar()
        {
            int x = (int)(10 * heightRatio);
            int y = (int)(10 * heightRatio);
            int width = (int)(healthBar.Width * widthRatio);
            int height = (int)(healthBar.Height*heightRatio);
            int sliceX = x + (int)(10 * heightRatio);
            int sliceY = y + (int)(10 * widthRatio);
            int sliceWidth = 1;
            int sliceHeight = (int)(healthSlice.Height * heightRatio);

            int limit = (int)(582 * widthRatio * health);


            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);
            for (int i = 0; i < limit; i++)
            {
                spriteBatch.Draw(healthSlice, new Rectangle(sliceX+i, sliceY, sliceWidth, sliceHeight), Color.White);
            }
        }

        private void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        public void updateHealth(int h)
        {
            health = (float)h / 100;
        }
    }
}
