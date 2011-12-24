using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class MiniMap
    {
        /// Constants

        public static int  MAP_X       = 1100;
        public static int  MAP_Y       = 500;

        public static int  MAP_WIDTH   = 220;
        public static int  MAP_HEIGHT  = 220;

        public static int VIBE_WIDTH   = 16;
        public static int VIBE_HEIGHT  = 20;

        public static int DEFAULT_ZOOM = 4;

        public static bool AUTO_ZOOM   = false;

        // Colours
        public static Color OUTLINE_COLOUR    = new Color(0f, 0f, 0f, 0.8f);
        public static Color BACKGROUND_COLOUR = new Color(0f, 0f, 0.2f, 0.5f);
        public static Color GOOD_VIBE_COLOUR  = new Color(0f, 0.7f, 0f, 0.5f);
        public static Color BAD_VIBE_COLOUR   = new Color(0.7f, 0f, 0f, 0.5f);

        /// Fields

        private static GraphicsDeviceManager graphics;
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;

        /// Constructor

        ///<summary>
        /// Create a new MiniMap
        ///</summary>
        public MiniMap()
        {
        }


        /// Methods
        
        ///<summary>
        ///</summary>
        public void loadTextures(ContentManager content)
        {
            outline    = content.Load<Texture2D>("Drawing/HUD/Textures/miniMap");
            background = content.Load<Texture2D>("Drawing/HUD/Textures/miniMapBG");
            vibe       = content.Load<Texture2D>("Drawing/HUD/Textures/map_vibe");
        }


        ///<summary>
        /// Draws the minimap on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            // Draw fill
            spriteBatch.Draw(background, new Rectangle(MAP_X, MAP_Y, MAP_WIDTH, MAP_HEIGHT), BACKGROUND_COLOUR);

            // Draw outline
            spriteBatch.Draw(outline, new Microsoft.Xna.Framework.Rectangle(MAP_X, MAP_Y, MAP_WIDTH, MAP_HEIGHT), OUTLINE_COLOUR);

            // Draw good vibe
            int gvx = MAP_X + (int) ((MAP_WIDTH / 2f) - (VIBE_WIDTH / 2f));
            int gvy = MAP_Y + (int) ((MAP_HEIGHT / 2f) - (VIBE_HEIGHT / 2f));
            spriteBatch.Draw(vibe, new Rectangle(gvx, gvy, VIBE_WIDTH, VIBE_HEIGHT), GOOD_VIBE_COLOUR);

            //spriteBatch.Draw(vibe, new Vector2(gvx - 30, gvy - 30), null, BAD_VIBE_COLOUR, 0.3f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Loop through and draw bad vibes.

        }
    }
}
