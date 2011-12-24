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

        public static int   MAP_X                = 1100;
        public static int   MAP_Y                = 500;

        public static int   MAP_WIDTH            = 220;
        public static int   MAP_HEIGHT           = 220;

        public static int   VIBE_WIDTH           = 16;
        public static int   VIBE_HEIGHT          = 20;

        public static float ZOOM                 = 10f;
        public static float DEFAULT_ZOOM         = 10f;

        public static bool  AUTO_ZOOM            = true;
        public static float MIN_ZOOM_SPEED       = 2f;

        public static bool  DRAW_SCALE_LINES     = true;
        public static int   SCALE_LINE_INTERVAL  = 3;

        public static bool  SWEEPER_ON           = true;
        public static int   SWEEPER_LENGTH       = 10;

        // Colours
        public static Color OUTLINE_COLOUR    = new Color(0f, 0f, 0f, 0.8f);
        public static Color BACKGROUND_COLOUR = new Color(0f, 0f, 0.2f, 0.5f);
        public static Color GOOD_VIBE_COLOUR  = new Color(0f, 0.7f, 0f, 0.5f);
        public static Color BAD_VIBE_COLOUR   = new Color(0.7f, 0f, 0f, 0.5f);
        public static Color SCALE_LINE_COLOUR = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public static Color SWEEPER_COLOUR    = new Color(0.0f, 0.0f, 0.9f, 0.5f);

        /// Fields

        private static GraphicsDeviceManager graphics;
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;
        private static Texture2D dVibe;

        private static int sweeperX = MAP_X + MAP_WIDTH;

        private static float scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

        /// Constructor

        ///<summary>
        /// Create a new MiniMap
        ///</summary>
        public MiniMap()
        {
            ZOOM = DEFAULT_ZOOM;
        }


        /// Methods
        
        ///<summary>
        ///</summary>
        public void loadTextures(ContentManager content)
        {
            outline    = content.Load<Texture2D>("Drawing/HUD/Textures/miniMap");
            background = content.Load<Texture2D>("Drawing/HUD/Textures/miniMapBG");
            vibe       = content.Load<Texture2D>("Drawing/HUD/Textures/map_vibe");
            dVibe      = content.Load<Texture2D>("Drawing/HUD/Textures/map_distant_vibe");
        }


        ///<summary>
        /// Draws the minimap on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            GoodVibe gVRef = (GoodVibe)Program.game.World.getObject("Player");

            if (AUTO_ZOOM)
            {
                float speed = gVRef.Body.MotionState.LinearVelocity.Length();

                if (speed > MIN_ZOOM_SPEED)
                {
                    ZOOM = DEFAULT_ZOOM + ((speed - MIN_ZOOM_SPEED) * 5);
                }
                else
                {
                    ZOOM = DEFAULT_ZOOM;
                }

                scaleFactor = (MAP_WIDTH / (2 * ZOOM));
            }

            // Draw fill
            spriteBatch.Draw(background, new Rectangle(MAP_X, MAP_Y, MAP_WIDTH, MAP_HEIGHT), BACKGROUND_COLOUR);

            // Draw outline
            spriteBatch.Draw(outline, new Microsoft.Xna.Framework.Rectangle(MAP_X, MAP_Y, MAP_WIDTH, MAP_HEIGHT), OUTLINE_COLOUR);

            // Draw scale lines, to provide a frame of reference.
            if (DRAW_SCALE_LINES)
            {
                for (float i = MAP_WIDTH / 2; i < MAP_WIDTH; i += scaleFactor * SCALE_LINE_INTERVAL)
                {
                    spriteBatch.Draw(background, new Rectangle(MAP_X + (int) i, MAP_Y, 1, MAP_HEIGHT), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(MAP_X + MAP_WIDTH - (int) i, MAP_Y, 1, MAP_HEIGHT), SCALE_LINE_COLOUR);
                }
                for (float i = MAP_HEIGHT / 2; i < MAP_HEIGHT; i += scaleFactor * SCALE_LINE_INTERVAL)
                {
                    spriteBatch.Draw(background, new Rectangle(MAP_X, MAP_Y + (int) i, MAP_WIDTH, 1), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(MAP_X, MAP_Y + MAP_HEIGHT - (int) i, MAP_WIDTH, 1), SCALE_LINE_COLOUR);
                }
            }

            // Draw good vibe
            int gvx = MAP_X + (int) ((MAP_WIDTH / 2f) - (VIBE_WIDTH / 2f));
            int gvy = MAP_Y + (int) ((MAP_HEIGHT / 2f) - (VIBE_HEIGHT / 2f));
            //spriteBatch.Draw(vibe, new Rectangle(gvx, gvy, VIBE_WIDTH, VIBE_HEIGHT), GOOD_VIBE_COLOUR);

            float r = 0f;// gVRef.Body.Orientation.Y;
            spriteBatch.Draw(vibe, new Vector2(gvx, gvy), null, GOOD_VIBE_COLOUR, r, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Loop through and draw bad vibes.
            List<BadVibe> badVibes = Program.game.World.returnBadVibes();

            Vector2 gVPos = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            Vector2 bVPos;
            Vector2 bVScreenPos;
            bool  inXRange, inYRange;
           
            foreach(BadVibe v in badVibes)
            {
                bVPos = new Vector2(v.Body.Position.X, v.Body.Position.Z);
                Vector2 relToGV = gVPos - bVPos;
                float angle = (DynamicObject.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                relToGV = rotateVector2(relToGV, angle);
                bVPos = gVPos - relToGV;

                inXRange = false;
                inYRange = false;

                // Check if bad vibe is in range
                if ((bVPos.X > gVPos.X - ZOOM) && (bVPos.X < gVPos.X + ZOOM)) inXRange = true;
                if ((bVPos.Y > gVPos.Y - ZOOM) && (bVPos.Y < gVPos.Y + ZOOM)) inYRange = true;

                if (inXRange && inYRange) {
                    float bVR = 0f;// (DynamicObject.QuaternionToEuler(v.Body.Orientation)).Y;
                    
                    bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                    //spriteBatch.Draw(vibe, new Rectangle((int) bVScreenPos.X, (int) bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), BAD_VIBE_COLOUR);
                    spriteBatch.Draw(vibe, new Vector2((int)bVScreenPos.X, (int)bVScreenPos.Y), null, BAD_VIBE_COLOUR, bVR, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                } else if (inXRange ^ inYRange) {
                    if (inXRange) {
                        if (bVPos.Y < gVPos.Y) {
                            bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), MAP_Y - (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), MAP_Y + MAP_HEIGHT - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.X < gVPos.X)
                        {
                            bVScreenPos = new Vector2(MAP_X - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(MAP_X + MAP_WIDTH - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                    }

                    spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), BAD_VIBE_COLOUR);
                } else {
                    // Draw in corresponding corner, transparency proportional to distance.
                    if (bVPos.X < gVPos.X) {
                        if (bVPos.Y < gVPos.Y)
                        {
                            bVScreenPos = new Vector2(MAP_X - (dVibe.Width / 2), MAP_Y - (dVibe.Height / 2));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(MAP_X - (dVibe.Width / 2), MAP_Y + MAP_HEIGHT - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.Y < gVPos.Y)
                        {
                            bVScreenPos = new Vector2(MAP_X + MAP_WIDTH - (dVibe.Width / 2), MAP_Y - (dVibe.Height / 2));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(MAP_X + MAP_WIDTH - (dVibe.Width / 2), MAP_Y + MAP_HEIGHT - (dVibe.Height / 2));
                        }
                    }

                    spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), BAD_VIBE_COLOUR);
                }
            }

            // Draw sweeper
            if (SWEEPER_ON) {
                if (sweeperX < MAP_X) sweeperX += MAP_WIDTH;

                spriteBatch.Draw(background, new Rectangle(sweeperX, MAP_Y, 1, MAP_HEIGHT), SWEEPER_COLOUR);

                int x;
                float alpha = 0.5f;
                for (int i = 0; i < SWEEPER_LENGTH; i++)
                {
                    alpha -= (0.5f / (float) SWEEPER_LENGTH);
                    x = sweeperX + i;
                    if (x > MAP_X + MAP_WIDTH) x -= MAP_WIDTH;
                    spriteBatch.Draw(background, new Rectangle(x, MAP_Y, 1, MAP_HEIGHT), new Color(0f, 0f, 0.9f, alpha));
                }

                sweeperX--;
            }
        }

        public static Vector2 rotateVector2(Vector2 vec, float theta)
        {
            Vector2 result = new Vector2();

            result.X = (float) ((vec.X * Math.Cos(theta)) - (vec.Y * Math.Sin(theta)));
            result.Y = (float) ((vec.X * Math.Sin(theta)) + (vec.Y * Math.Cos(theta)));

            return result;
        }
    }
}
