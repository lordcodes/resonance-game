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

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            GameModels.Init(Content);
            hud = new Hud(Content,graphics);
            gameGraphics = new Graphics(Content, graphics);
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
        public static void Draw()
        {
            hud.drawDebugInfo(DebugDisplay.getString(), new Vector2(20,45));

        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(int gameModelNum, Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            gameGraphics.Draw(gameModelNum, worldTransform);
            Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(pos, gameGraphics.Projection, gameGraphics.View, Matrix.Identity);
            Vector2 screenPosition = new Vector2(projectedPosition.X, projectedPosition.Y-100);

            hud.drawDebugInfo(worldObject.returnIdentifier(), screenPosition);
            
            if (worldObject.returnIdentifier().Equals("Player")) 
            {
                DebugDisplay.update( "HEALTH",((GoodVibe)((DynamicObject)worldObject)).GetHealth().ToString());
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

    }
}
