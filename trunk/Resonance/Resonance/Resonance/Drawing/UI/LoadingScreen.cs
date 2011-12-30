using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    /// <summary>
    /// Displays and animates the loading screen.
    /// </summary>
    class LoadingScreen
    {
        private GraphicsDeviceManager graphics;
        private ContentManager content;
        private Texture2D splash;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private float timeElapsed = 0;
        private float timeToUpdate = 0.5f;
        private int frameNumber = 0;

        /// <summary>
        /// Create class representing the loading screen
        /// </summary>
        /// <param name="content">ContentManager content objects used to load all the data</param>
        /// <param name="graphics">GraphicsDeviceManager graphics used to create SpriteBatch for image drawing</param>
        public LoadingScreen(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            this.content = content;
        }

        /// <summary>
        /// Called once to load data needed to dislpay loading screen
        /// </summary>
        public void loadContent()
        {
            splash = content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/splash1");
            font = content.Load<SpriteFont>("Drawing/Fonts/DebugFont");
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        /// <summary>
        /// Called every frame, to update work out what frame the loading screen animation should be on
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;
                if (frameNumber < 3)frameNumber++;
                else frameNumber = 0;
            }
        }

        /// <summary>
        /// Called to draw th eloading screen
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();
            string text = "Loading";
            for (int i = 0; i < frameNumber; i++) text += ".";
            spriteBatch.Draw(splash, new Rectangle(0, 0, Drawing.ScreenWidth+1, Drawing.ScreenHeight+1), Color.White);
            spriteBatch.DrawString(font, text, new Vector2(Drawing.pixelsX(100), Drawing.pixelsY(950)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
