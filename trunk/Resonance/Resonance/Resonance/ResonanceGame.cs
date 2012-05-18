using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Resonance
{
    public class ResonanceGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        public const float FPS = 30f;
        private const bool HD1080 = true;

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public ResonanceGame()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / FPS);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            if (HD1080)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }
            Window.AllowUserResizing = true;            

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
           
            screenManager.addScreen(new MainMenu());
        }

        protected override void Initialize()
        {
            Components.Add(new GamerServicesComponent(this));
            
            base.Initialize();

            //Must come after base.initialise()
            HighScoreManager.loadFile();
            //HighScoreManager.initializeData();
        }
        
        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        public GraphicsDeviceManager GraphicsManager
        {
            get { return graphics; }
        }
    }
}
