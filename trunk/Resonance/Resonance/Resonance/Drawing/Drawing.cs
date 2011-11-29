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
        private static int frameCounter;
        private static int frameTime;
        private static int currentFrameRate;

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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            hud.loadContent();
            GameModels.Load();
            gameGraphics.loadContent();
        }

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            hud.Draw();
            hud.drawDebugInfo(DebugDisplay.getString());
            checkFrameRate(gameTime);
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(int gameModelNum, Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            if (worldObject is Shockwave) graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            gameGraphics.Draw(gameModelNum, worldTransform);
            if (worldObject is Shockwave) graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            if (worldObject is GoodVibe) 
            {
                int health = ((GoodVibe)((DynamicObject)worldObject)).GetHealth();
                DebugDisplay.update( "HEALTH",health.ToString());
                hud.updateHealth(health);
            }

            if (worldObject is BadVibe)
            {
                //Gets list of remaining armour layers
                List<int> seq = ((BadVibe)worldObject).getLayers();

                //prints the armour sequence of BV above head in text
                /*string toPrint = "";
                foreach (int layer in seq)
                {
                    toPrint += (layer.ToString() + ",");
                }
                hud.updateEnemy(toPrint, pos, 100);*/

                hud.updateEnemy(worldObject.returnIdentifier(), pos, 100);
            }

        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public static void UpdateCamera(GoodVibe player)
        {
            gameGraphics.UpdateCamera(player);
        }

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
