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
    class LoadingScreen : Screen
    {
        private Texture2D splash;
        private SpriteFont font;
        private float timeElapsed = 0;
        private float timeToUpdate = 0.5f;
        private int frameNumber = 0;
        Screen screenToLoad;

        bool displaySplash;
        bool startLoading = true;
        bool loadingDone = false;

        public LoadingScreen(bool displayScreen, Screen loadScreen)
        {
            displaySplash = displayScreen;
            screenToLoad = loadScreen;
        }

        public override void LoadContent()
        {
            splash = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/splash1");
            font = ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/DebugFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;
                if (frameNumber < 3) frameNumber++;
                else frameNumber = 0;
            }

            if (startLoading)
            {
                screenToLoad.ScreenManager = ScreenManager;
                screenToLoad.LoadContent();
                screenToLoad.LoadedUsingLoading = true;
                startLoading = false;
            }
            if (loadingDone)
            {
                ScreenManager.removeScreen(this);
                ScreenManager.addScreen(screenToLoad);
                ScreenManager.addScreen(new HintScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (displaySplash)
            {
                ScreenManager.SpriteBatch.Begin();
                string text = "Loading";
                for (int i = 0; i < frameNumber; i++) text += ".";
                ScreenManager.SpriteBatch.Draw(splash, new Rectangle(0, 0, ScreenManager.ScreenWidth + 1, ScreenManager.ScreenHeight + 1), Color.White);
                ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(ScreenManager.pixelsX(100), ScreenManager.pixelsY(950)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.End();
            }

            if (!Loading.isLoading && !startLoading)
            {
                loadingDone = true;
            }
        }

        public static void LoadAScreen(ScreenManager screenManager, bool displayScreen, Screen loadScreen)
        {
            foreach (Screen screen in screenManager.getScreens())
            {
                screen.ExitScreen();
            }

            LoadingScreen loadingScreen = new LoadingScreen(displayScreen, loadScreen);

            screenManager.addScreen(loadingScreen);
        }
    }
}
