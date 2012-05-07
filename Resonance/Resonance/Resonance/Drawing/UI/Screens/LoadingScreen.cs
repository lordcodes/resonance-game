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
        public const int NONE              = 0;
        public const int ARCADE_SPLASH     = 1;
        public const int OBJECTIVE_SPLASH1 = 2;
        public const int OBJECTIVE_SPLASH2 = 3;
        public const int OBJECTIVE_SPLASH3 = 4;
        public const int OBJECTIVE_SPLASH4 = 5;
        public const int OBJECTIVE_SPLASH5 = 6;

        private Texture2D[] bgs;
        private SpriteFont font;
        private float timeElapsed = 0;
        private float timeToUpdate = 0.5f;
        private int frameNumber = 0;
        Screen screenToLoad;

        int displaySplash;
        bool startLoading = true;
        bool loadingDone = false;

        public LoadingScreen(int displayScreen, Screen loadScreen)
        {
            displaySplash = displayScreen;
            screenToLoad = loadScreen;
            bgs = new Texture2D[14];
        }

        public override void LoadContent()
        {
            bgs[13] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain5");
            bgs[12] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain4");
            bgs[11] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain3");
            bgs[10] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain2");
            bgs[9] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain1");
            bgs[8] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain0");
            bgs[7] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/brain");
            bgs[6] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen1");
            bgs[5] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen2");
            bgs[4] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen3");
            bgs[3] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen4");
            bgs[2] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen5");
            bgs[1] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen6");
            bgs[0] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/LoadingScreen7");
            font = ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MenuFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;
                if (frameNumber < 6) frameNumber++;
                else frameNumber = 0;
            }

            if (startLoading)
            {
                screenToLoad.ScreenManager = ScreenManager;
                Loading.load(delegate { screenToLoad.LoadContent(); }, "LoadingContent");
                screenToLoad.LoadedUsingLoading = true;
                startLoading = false;
            }
            if (loadingDone)
            {
                ScreenManager.removeScreen(this);
                ScreenManager.addScreen(screenToLoad);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (displaySplash > 0)
            {
                ScreenManager.SpriteBatch.Begin();
                string text = "Loading";
                for (int i = 0; i < frameNumber; i++) text += ".";

                Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

                if(displaySplash <= 7) ScreenManager.SpriteBatch.Draw(bgs[6 + displaySplash], new Vector2((int)screenSize.X - 250, (int)screenSize.Y - 275), Color.White);

                ScreenManager.SpriteBatch.Draw(bgs[frameNumber], new Vector2(ScreenManager.pixelsX(1650), ScreenManager.pixelsY(825)), Color.White);
                ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(ScreenManager.pixelsX(100), ScreenManager.pixelsY(950)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.End();
            }

            if (!Loading.isLoading && !startLoading)
            {
                loadingDone = true;
            }
        }

        public static void LoadAScreen(ScreenManager screenManager, int displayScreen, Screen loadScreen)
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
