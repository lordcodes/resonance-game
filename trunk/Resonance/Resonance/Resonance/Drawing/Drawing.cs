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
    class Drawing
    {
        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;
        private static Hud hud;
        private static Graphics gameGraphics;
        private static LoadingScreen loadingScreen;
        private static int frameCounter;
        private static int frameTime;
        private static int currentFrameRate;
        private static float screenWidth;
        private static float screenHeight;
        private static double widthRatio;
        private static double heightRatio;
        private static Vector3 playerPos;

        public static int ScreenWidth
        {
            get
            {
                return (int)Math.Round(screenWidth);
            }
        }

        public static int ScreenHeight
        {
            get
            {
                return (int)Math.Round(screenHeight);
            }
        }

        public static double WidthRatio
        {
            get
            {
                return widthRatio;
            }
        }

        public static double HeightRatio
        {
            get
            {
                return heightRatio;
            }
        }

        public static void reset()
        {
            if (gameGraphics != null) gameGraphics.reset();
        }

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            GameModels.Init(Content);
            gameGraphics = new Graphics(Content, graphics);
            hud = new Hud(Content,graphics, gameGraphics);
            loadingScreen = new LoadingScreen(Content, graphics);
            playerPos = new Vector3(0,0,0);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            widthRatio = screenWidth / 1920;
            heightRatio = screenHeight / 1080;
            hud.loadContent();
            GameModels.Load();
            gameGraphics.loadContent(Content, graphics.GraphicsDevice);
            loadingScreen.loadContent();
        }

        /// <summary>
        /// Used to update any frame animation
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            if (Loading.IsLoading)
            {
                loadingScreen.Update(gameTime);
            }
            else if (!UI.Paused)
            {
                gameGraphics.update(new Vector2(0f, 0f));
            }
        }

        public static void addWave(Vector3 position3d)
        {
            Vector2 playerGroundPos = new Vector2();
            float groundWidth = World.MAP_X;
            float groundHeight = World.MAP_Z;
            float xDis = Math.Abs(playerPos.X - World.MAP_MIN_X);
            float yDis = Math.Abs(playerPos.Z - World.MAP_MIN_Z);
            playerGroundPos.X = (float)Math.Round(Graphics.DISP_WIDTH * (xDis / groundWidth));
            playerGroundPos.Y = (float)Math.Round(Graphics.DISP_WIDTH * (yDis / groundHeight));
            gameGraphics.addWave(playerGroundPos);
        }

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            if (Loading.IsLoading)
            {
                loadingScreen.Draw();
            }
            else
            {
                hud.Draw();
                hud.drawDebugInfo(DebugDisplay.getString());
                if (UI.Paused) hud.drawMenu(UI.getString());
                checkFrameRate(gameTime);
            }
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(int gameModelNum, Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            bool blend = false;
            Vector2 playerGroundPos = new Vector2(0f, 0f);
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            if (worldObject is GoodVibe)
            {
                int health = ((GoodVibe)((DynamicObject)worldObject)).Health;
                int score = ((GoodVibe)((DynamicObject)worldObject)).Score;
                DebugDisplay.update("HEALTH", health.ToString());
                DebugDisplay.update("SCORE", score.ToString());
                hud.updateGoodVibe(health, score);
                playerPos = ((GoodVibe)((DynamicObject)worldObject)).Body.Position;
            }
            
            if (worldObject.returnIdentifier().Equals("Ground"))
            {
                //blend = true;
            }
            
            if (worldObject is Shockwave)graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            gameGraphics.Draw(gameModelNum, worldTransform, blend);
            if (worldObject is Shockwave)graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            if (worldObject is BadVibe)
            {
                //Gets list of remaining armour layers
                List<int> seq = ((BadVibe)worldObject).getLayers();
                hud.updateEnemy(worldObject.returnIdentifier(), pos, seq);
            }

        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public static void UpdateCamera(Vector3 point, Vector3 cameraPosition)
        {
            gameGraphics.UpdateCamera(point, cameraPosition);
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

        /// <summary>
        /// Reset graphic options which may become corrupted by SpriteBatch
        /// </summary>
        public static void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Used to update frame rate information
        /// </summary>
        /// <param name="gameTime"></param>
        private static void checkFrameRate(GameTime gameTime)
        {
            frameCounter++;
            frameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (frameTime >= 1000)
            {
                currentFrameRate = frameCounter;
                frameTime = 0;
                frameCounter = 0;
            }
            DebugDisplay.update("FPS", currentFrameRate.ToString());
        }

    }
}
