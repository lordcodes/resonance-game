using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Resonance
{
    class ScreenManager : DrawableGameComponent
    {
        List<Screen> screens;
        List<Screen> updateScreens;

        SpriteBatch spriteBatch;
        ContentManager content;
        InputDevices input;

        Texture2D blankTex;

        bool initialised = false;

        private static float screenWidth;
        private static float screenHeight;
        private static double widthRatio;
        private static double heightRatio;

        public static GameScreen game;


        public ScreenManager(ResonanceGame game)
            : base(game)
        {
            screens = new List<Screen>();
            updateScreens = new List<Screen>();
            input = new InputDevices();
        }

        public override void Initialize()
        {
            base.Initialize();
            initialised = true;
        }

        protected override void LoadContent()
        {
            content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            widthRatio = screenWidth / 1920;
            heightRatio = screenHeight / 1080;

            blankTex = content.Load<Texture2D>("Drawing/Textures/texPixel");
            foreach (Screen screen in screens)
            {
                screen.LoadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            updateScreens.Clear();
            foreach (Screen screen in screens)
            {
                updateScreens.Add(screen);
            }

            bool takeInput = true;

            while (updateScreens.Count > 0)
            {
                Screen screen = updateScreens[updateScreens.Count - 1];
                updateScreens.RemoveAt(updateScreens.Count - 1);
                if (takeInput)
                {
                    screen.HandleInput(input);
                    screen.Update(gameTime);
                    takeInput = false;
                }
                else
                {
                    bool doUpdate = (screen is GameScreen);
                    if (!doUpdate)
                    {
                        screen.Update(gameTime);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            bool first = true;
            foreach (Screen screen in screens)
            {
                if (first)
                {
                    screen.Draw(gameTime);
                }
                else
                {
                    bool doDraw = (screen is GameScreen);
                    if (!doDraw)
                    {
                        screen.Draw(gameTime);
                    }
                }
            }
        }

        public void addScreen(Screen screen)
        {
            if (!screen.LoadedUsingLoading)
            {
                screen.ScreenManager = this;

                if (initialised) screen.LoadContent();
            }
            screens.Add(screen);
        }

        public void removeScreen(Screen screen)
        {
            screens.Remove(screen);
            updateScreens.Remove(screen);
        }

        public Screen[] getScreens()
        {
            return screens.ToArray<Screen>();
        }

        public void darkenBackground(float alpha)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(blankTex, new Rectangle(0, 0, (int)screenWidth + 1, (int)screenHeight + 1), Color.Black * alpha);
            spriteBatch.End();
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public ContentManager Content
        {
            get { return content; }
        }

        /// <summary>
        /// Converts X pixel values from HD resolution to the current resolution which may not be HD
        /// </summary>
        /// <param name="input">X HD coordinate</param>
        /// <returns>True coordinate</returns>
        public static int pixelsX(int input)
        {
            return (int)Math.Round(input * widthRatio);
        }

        /// <summary>
        /// Converts Y pixel values from HD resolution to the current resolution which may not be HD
        /// </summary>
        /// <param name="input">Y HD coordinate</param>
        /// <returns>True coordinate</returns>
        public static int pixelsY(int input)
        {
            return (int)Math.Round(input * heightRatio);
        }

        public static int ScreenWidth
        {
            get { return (int)Math.Round(screenWidth); }
        }

        public static int ScreenHeight
        {
            get { return (int)Math.Round(screenHeight); }
        }

        public static double HeightRatio
        {
            get { return heightRatio; }
        }

        public static double WidthRatio
        {
            get { return widthRatio; }
        }
    }
}
