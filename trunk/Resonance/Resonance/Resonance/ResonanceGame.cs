using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    public class ResonanceGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public ResonanceGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            //Allows you to set the resolution of the game (not tested on Xbox yet)
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Window.AllowUserResizing = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            screenManager.addScreen(new MainMenu());
        }

        protected override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            //ScreenManager.game = new GameScreen(screenManager);
            //screenManager.addScreen(ScreenManager.game);
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
