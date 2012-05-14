using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public static string CurrentlyLoading = "";

        private Texture2D[] bgs;
        private SpriteFont font;
        private float timeElapsed = 0;
        private float timeToUpdate = 0.5f;
        private int frameNumber = 0;
        Screen screenToLoad;

        private static float OBJ_FONT_SIZE = 0.6f;

        int displaySplash;
        float fadeValue;
        private static float FADE_INC = 1f / 100f;
        bool startLoading = true;
        bool loadingDone = false;

        public LoadingScreen(int displayScreen, Screen loadScreen)
        {
            displaySplash = displayScreen;
            screenToLoad = loadScreen;
            bgs = new Texture2D[15];
            fadeValue = 0.0f;
        }

        public override void LoadContent()
        {
            bgs[14] = ScreenManager.Content.Load<Texture2D>("Drawing/UI/LoadingScreen/Textures/button_a");
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

        public override void HandleInput(InputDevices input)
        {
            bool select = (!input.LastKeys.IsKeyDown(Keys.Enter) && input.LastPlayerOne.Buttons.A != ButtonState.Pressed) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.Buttons.A == ButtonState.Pressed);
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                            (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));
            bool backPause = !input.LastPlayerOne.IsButtonDown(Buttons.Start) && input.PlayerOne.IsButtonDown(Buttons.Start);

            if (loadingDone && (select || back || backPause))
            {
                ScreenManager.removeScreen(this);
                ScreenManager.addScreen(screenToLoad);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!loadingDone)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (timeElapsed > timeToUpdate)
                {
                    timeElapsed -= timeToUpdate;
                    if (frameNumber < 6) frameNumber++;
                    else frameNumber = 0;
                }
            }
            else frameNumber = 0;

            if (startLoading)
            {
                screenToLoad.ScreenManager = ScreenManager;
                Loading.load(delegate { screenToLoad.LoadContent(); }, "LoadingContent");
                screenToLoad.LoadedUsingLoading = true;
                startLoading = false;
            }
            if(!loadingDone) fadeValue = Math.Min(1f, fadeValue + FADE_INC);
        }

        public override void Draw(GameTime gameTime)
        {
            if (displaySplash > 0)
            {
                ScreenManager.SpriteBatch.Begin();
                string text = "Loading (" + CurrentlyLoading+ ")";
                for (int i = 0; i < frameNumber; i++) text += ".";

                if (loadingDone) text = "Press A to continue";

                Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

                if (displaySplash <= 7) 
                {
                    bool arcade = displaySplash == ARCADE_SPLASH;
                    bool firstObj = displaySplash == OBJECTIVE_SPLASH1;

                    if (!arcade)
                    {
                        string obj = "";
                        string con = "";
                        string drum = "";
                        ObjectiveManager.getLoadingScreenString(ref obj, ref con, ref drum);

                        ScreenManager.SpriteBatch.DrawString(font, "Objective", new Vector2(ScreenManager.pixelsX(80), screenSize.Y - 140), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);
                        ScreenManager.SpriteBatch.DrawString(font, obj, new Vector2(ScreenManager.pixelsX(80), screenSize.Y - 115), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);

                        ScreenManager.SpriteBatch.DrawString(font, "Controller player", new Vector2(ScreenManager.pixelsX(80), screenSize.Y - 55), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);
                        ScreenManager.SpriteBatch.DrawString(font, con, new Vector2(ScreenManager.pixelsX(80), screenSize.Y - 30), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);

                        ScreenManager.SpriteBatch.DrawString(font, "Drum kit player", new Vector2(ScreenManager.pixelsX(80), screenSize.Y + 30), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);
                        ScreenManager.SpriteBatch.DrawString(font, drum, new Vector2(ScreenManager.pixelsX(80), screenSize.Y + 55), Color.White, 0f,
                            Vector2.Zero, OBJ_FONT_SIZE, SpriteEffects.None, 0f);
                    }

                    float x = (screenSize.X * 1.5f) - 287;
                    float y = screenSize.Y - 249;
                    if(!arcade && !firstObj) ScreenManager.SpriteBatch.Draw(bgs[5 + displaySplash], new Vector2(x,y), Color.White);
                    ScreenManager.SpriteBatch.Draw(bgs[6 + displaySplash], new Vector2(x,y), Color.White * fadeValue);
                }

                if (displaySplash == OBJECTIVE_SPLASH1)
                {
                    //Display "story" text
                }

                Vector2 textSize = font.MeasureString(text);
                int ax = ScreenManager.pixelsX(150) + (int)textSize.X;
                int width = (int)textSize.Y + 10;
                int ay = ScreenManager.pixelsY(950) - (width / 2);

                ScreenManager.SpriteBatch.Draw(bgs[frameNumber], new Vector2(ScreenManager.pixelsX(1650), ScreenManager.pixelsY(825)), Color.White);
                ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(ScreenManager.pixelsX(100), ScreenManager.pixelsY(950)), Color.White, 0f, new Vector2(0, textSize.Y / 2), 1f, SpriteEffects.None, 0f);
                if(loadingDone) ScreenManager.SpriteBatch.Draw(bgs[14], new Rectangle(ax,ay,width,width), Color.White);
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
