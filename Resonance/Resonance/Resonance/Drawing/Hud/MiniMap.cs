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
        public static int   MAP_X                = Drawing.pixelsX(1920 - 250);
        public static int   MAP_Y                = Drawing.pixelsY(1080 - 250);

        public static int   LARGE_MAP_X          = Drawing.pixelsX(585);
        public static int   LARGE_MAP_Y          = Drawing.pixelsY(165);

        public static int   MAP_WIDTH            = Drawing.pixelsX(220);
        public static int   MAP_HEIGHT           = Drawing.pixelsY(220);

        public static int   LARGE_MAP_WIDTH      = Drawing.pixelsX(750);
        public static int   LARGE_MAP_HEIGHT     = Drawing.pixelsY(750);

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

        // Distance outside radar at which distant Vibe marker fades. 
        public static float VANISHING_POINT      = 5f;

        public static float BAD_VIBE_ALPHA       = 0.5f;

        // Colours
        public static Color OUTLINE_COLOUR    = new Color(0f, 0f, 0f, 0.8f);
        public static Color BACKGROUND_COLOUR = new Color(0f, 0f, 0.2f, 0.5f);
        public static Color GOOD_VIBE_COLOUR  = new Color(0f, 0.7f, 0f, 0.5f);
        public static Color BAD_VIBE_COLOUR   = new Color(0.7f, 0f, 0f, BAD_VIBE_ALPHA);
        public static Color SCALE_LINE_COLOUR = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public static Color SWEEPER_COLOUR    = new Color(0.0f, 0.0f, 0.9f, 0.5f);

        /// Fields

        public  static bool      large;
 
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;
        private static Texture2D dVibe;

        private static int SPEED_SAMPLES = 10;
        private static List<float> speeds;

        private static int sweeperX = MAP_X + MAP_WIDTH;

        private static float scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

        /// Constructor

        ///<summary>
        /// Create a new MiniMap
        ///</summary>
        public MiniMap()
        {
            ZOOM = DEFAULT_ZOOM;
            large = false;

            speeds = new List<float>();

            for (int i = 0; i < SPEED_SAMPLES; i++)
            {
                speeds.Add(0f);
            }
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
        /// Keeps track of the last SPEED_SAMPLES readings of GV's speed and averages them.
        /// This is used when drawing scale lines on map, to reduce vibrating.
        ///</summary>
        private float estimateSpeed(GoodVibe gV)
        {
            speeds.RemoveAt(0);
            speeds.Add(gV.Body.MotionState.LinearVelocity.Length());

            float sum   = 0;
            int   count = 0;

            foreach (float x in speeds)
            {
                sum += x;
                count++;
            }

            return sum / count;
        }

        ///<summary>
        /// Draws the minimap on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            int mapX, mapY, mapH, mapW;

            if (!large)
            {
                mapX = MAP_X;
                mapY = MAP_Y;
                mapW = MAP_WIDTH;
                mapH = MAP_HEIGHT;

                scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));
            }
            else
            {
                mapX = LARGE_MAP_X;
                mapY = LARGE_MAP_Y;
                mapW = LARGE_MAP_WIDTH;
                mapH = LARGE_MAP_HEIGHT;

                scaleFactor = (LARGE_MAP_WIDTH / (2 * DEFAULT_ZOOM));
            }

            GoodVibe gVRef = (GoodVibe)Program.game.World.getObject("Player");

            if (AUTO_ZOOM)
            {
                //float speed = gVRef.Body.MotionState.LinearVelocity.Length();
                float speed = estimateSpeed(gVRef);

                if (speed > MIN_ZOOM_SPEED)
                {
                    ZOOM = DEFAULT_ZOOM + ((speed - MIN_ZOOM_SPEED) * 5);
                }
                else
                {
                    ZOOM = DEFAULT_ZOOM;
                }

                scaleFactor = (mapW / (2 * ZOOM));
            }

            // Calculate how far from the good vibe the corner of the map is.
            float corner_dist = (float) Math.Sqrt(Math.Pow((double) ZOOM, 2.0) * 2);

            // Draw fill
            spriteBatch.Draw(background, new Rectangle(mapX, mapY, mapW, mapH), BACKGROUND_COLOUR);

            // Draw outline
            spriteBatch.Draw(outline, new Microsoft.Xna.Framework.Rectangle(mapX, mapY, mapW, mapH), OUTLINE_COLOUR);

            // Draw scale lines, to provide a frame of reference.
            if (DRAW_SCALE_LINES)
            {
                for (float i = mapW / 2; i < mapW; i += scaleFactor * SCALE_LINE_INTERVAL)
                {
                    spriteBatch.Draw(background, new Rectangle(mapX + (int) i, mapY, 1, mapH), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(mapX + mapW - (int) i, mapY, 1, mapH), SCALE_LINE_COLOUR);
                }
                for (float i = mapH / 2; i < mapH; i += scaleFactor * SCALE_LINE_INTERVAL)
                {
                    spriteBatch.Draw(background, new Rectangle(mapX, mapY + (int) i, mapW, 1), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(mapX, mapY + mapH - (int) i, mapW, 1), SCALE_LINE_COLOUR);
                }
            }

            // Draw good vibe
            int gvx = mapX + (int) ((mapW / 2f) - (VIBE_WIDTH / 2f));
            int gvy = mapY + (int) ((mapH / 2f) - (VIBE_HEIGHT / 2f));

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

                float alpha = BAD_VIBE_ALPHA;

                inXRange = false;
                inYRange = false;

                // Check if bad vibe is in range
                if ((bVPos.X > gVPos.X - ZOOM) && (bVPos.X < gVPos.X + ZOOM)) inXRange = true;
                if ((bVPos.Y > gVPos.Y - ZOOM) && (bVPos.Y < gVPos.Y + ZOOM)) inYRange = true;

                if (inXRange && inYRange) {
                    float bVR = - (DynamicObject.QuaternionToEuler(v.Body.Orientation)).Y + (DynamicObject.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                    bVR += (float) Math.PI;
                    
                    bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                    Vector2 centre = new Vector2(vibe.Width / 2f, vibe.Height / 2f);
                    //spriteBatch.Draw(vibe, new Rectangle((int) bVScreenPos.X, (int) bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), BAD_VIBE_COLOUR);
                    spriteBatch.Draw(vibe, new Vector2((int)bVScreenPos.X, (int)bVScreenPos.Y), null, BAD_VIBE_COLOUR, bVR, centre, 1f, SpriteEffects.None, 0f);
                } else if (inXRange ^ inYRange) {
                    float dist = (float)Resonance.Game.getDistance(Resonance.Game.getGV().Body.Position, v.Body.Position);
                    bool visible = true;

                    // Calculate the alpha transparency that the distant vibe should have. Determine whether it is visible or not.
                    alpha = calculateBVAlpha(dist, corner_dist);
                    if (alpha < 0f) visible = false;

                    if (inXRange) {
                        if (bVPos.Y < gVPos.Y) {
                            bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapY - (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapY + mapH - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.X < gVPos.X)
                        {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                    }

                    if (visible)
                    {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color c = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), c);
                    }
                } else {
                    // Draw in corresponding corner, transparency proportional to distance.

                    float dist = (float)Resonance.Game.getDistance(Resonance.Game.getGV().Body.Position, v.Body.Position);
                    bool visible = true;

                    // Calculate the alpha transparency that the distant vibe should have. Determine whether it is visible or not.
                    alpha = calculateBVAlpha(dist, corner_dist);
                    if (alpha < 0f) visible = false;

                    if (bVPos.X < gVPos.X) {
                        if (bVPos.Y < gVPos.Y)
                        {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), mapY - (dVibe.Height / 2));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), mapY + mapH - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.Y < gVPos.Y)
                        {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), mapY - (dVibe.Height / 2));
                        }
                        else
                        {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), mapY + mapH - (dVibe.Height / 2));
                        }
                    }

                    if (visible)
                    {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color c = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), c);
                    }
                }
            }

            // Draw sweeper
            if (SWEEPER_ON) {
                if (sweeperX < mapX) sweeperX += mapW;

                spriteBatch.Draw(background, new Rectangle(sweeperX, mapY, 1, mapH), SWEEPER_COLOUR);

                int x;
                float alpha = BAD_VIBE_ALPHA;
                for (int i = 0; i < SWEEPER_LENGTH; i++)
                {
                    alpha -= (BAD_VIBE_ALPHA / (float) SWEEPER_LENGTH);
                    x = sweeperX + i;
                    if (x > mapX + mapW) x -= mapW;
                    spriteBatch.Draw(background, new Rectangle(x, mapY, 1, mapH), new Color(0f, 0f, 0.9f, alpha));
                }

                sweeperX--;
            }
        }

        // Calculates the alpha transparency of a bad vibe.
        private static float calculateBVAlpha(float dist, float corner_dist) {
            if (dist > corner_dist)
            {
                float diff = dist - corner_dist;
                if (diff > VANISHING_POINT * scaleFactor)
                {
                    return -1f;
                }
                else
                {
                    float ratio = diff / (VANISHING_POINT * scaleFactor);
                    return BAD_VIBE_ALPHA - (ratio * BAD_VIBE_ALPHA);
                }
            }

            return BAD_VIBE_ALPHA;
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
