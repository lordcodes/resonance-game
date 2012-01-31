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

        public static float ZOOM                 = 40f;
        public static float DEFAULT_ZOOM         = 40f;

        public static bool  AUTO_ZOOM            = true;
        public static float MIN_ZOOM_SPEED       = 2f;

        public static bool  DRAW_SCALE_LINES     = true;
        public static int   SCALE_LINE_INTERVAL  = 12;

        public static bool  SWEEPER_ON           = true;
        public static int   SWEEPER_LENGTH       = 20;

        public static bool  DRAW_WORLD_BOX       = false;//true;


        // Distance outside radar at which distant Vibe marker fades. 
        public static float VANISHING_POINT      = 5f;

        public static float BAD_VIBE_ALPHA       = 0.5f;
        public static float PICKUP_ALPHA         = 0.5f;

        // Colours
        public static Color    OUTLINE_COLOUR = new Color(0.0f, 0.0f, 0.0f, 1.0f); // 0.8 alpha?
        public static Color BACKGROUND_COLOUR = new Color(0.0f, 0.0f, 0.2f, 0.5f);
        public static Color  GOOD_VIBE_COLOUR = new Color(0.0f, 0.7f, 0.0f, 0.5f);
        public static Color   BAD_VIBE_COLOUR = new Color(0.7f, 0.0f, 0.0f, BAD_VIBE_ALPHA);
        public static Color     PICKUP_COLOUR = new Color(0.7f, 0.7f, 0.0f, PICKUP_ALPHA);
        public static Color SCALE_LINE_COLOUR = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public static Color    SWEEPER_COLOUR = new Color(0.0f, 0.0f, 0.9f, 0.5f);

        /// Fields

        public  static bool      large;
 
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;
        private static Texture2D dVibe;
        private static Texture2D block;
        private static Texture2D pickup;

        private static int SPEED_SAMPLES = 10;
        private static List<float> speeds;

        private static int sweeperX = MAP_X + MAP_WIDTH;

        private static float scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

        private GoodVibe gVRef;

        private static int mapX, mapY, mapH, mapW;

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
            block      = content.Load<Texture2D>("Drawing/HUD/Textures/block");
            pickup     = content.Load<Texture2D>("Drawing/HUD/Textures/pickup");
        }


        public static void enlarge() {
            large = true;

            mapX = LARGE_MAP_X;
            mapY = LARGE_MAP_Y;
            mapW = LARGE_MAP_WIDTH;
            mapH = LARGE_MAP_HEIGHT;

            scaleFactor = (LARGE_MAP_WIDTH / (2 * DEFAULT_ZOOM));
        }

        public static void ensmall() {
            large = false;

            mapX = MAP_X;
            mapY = MAP_Y;
            mapW = MAP_WIDTH;
            mapH = MAP_HEIGHT;

            scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));
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

        /// <summary>
        /// Draws a bounding box. EPICALLY BROKEN.
        /// </summary>
        private void drawWorldBox(SpriteBatch spriteBatch, BoundingBox bBox) {
            Vector3 min = bBox.Min;
            Vector3 max = bBox.Max;

            Vector2 gVPos2D = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            Vector2[] corners = new Vector2[4];
            Vector2[] relCnrs = new Vector2[4];

            corners[0].X = min.X; corners[0].Y = min.Z;
            corners[1].X = min.X; corners[1].Y = max.Z;
            corners[2].X = max.X; corners[2].Y = max.Z;
            corners[3].X = max.X; corners[3].Y = min.Z;

            relCnrs[0].X = (corners[0].X - gVPos2D.X) * scaleFactor; relCnrs[0].Y = (corners[0].Y - gVPos2D.Y) * scaleFactor;
            relCnrs[1].X = (corners[1].X - gVPos2D.X) * scaleFactor; relCnrs[1].Y = (corners[1].Y - gVPos2D.Y) * scaleFactor;
            relCnrs[2].X = (corners[2].X - gVPos2D.X) * scaleFactor; relCnrs[2].Y = (corners[2].Y - gVPos2D.Y) * scaleFactor;
            relCnrs[3].X = (corners[3].X - gVPos2D.X) * scaleFactor; relCnrs[3].Y = (corners[3].Y - gVPos2D.Y) * scaleFactor;

            for (int i = 0; i < 3; i++) {
                int h = (int) Math.Sqrt((Math.Pow((relCnrs[i].X - relCnrs[i + 1].X), 2)) + (Math.Pow((relCnrs[i].Y - relCnrs[i + 1].Y), 2)));
                int x = (int) relCnrs[i + 1].X;
                int y = (int) relCnrs[i + 1].Y;
                float ang = (float) Math.Atan(((relCnrs[i].X - relCnrs[i + 1].X) / ((relCnrs[i].Y - relCnrs[i + 1].Y))));
                spriteBatch.Draw(background, new Rectangle(x, y, 1, h), null, BAD_VIBE_COLOUR, ang, relCnrs[i + 1], SpriteEffects.None, 0f);
            }
        }

        ///<summary>
        /// Draws the minimap on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            /*if (!large) {
                mapX = MAP_X;
                mapY = MAP_Y;
                mapW = MAP_WIDTH;
                mapH = MAP_HEIGHT;

                scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));
            } else  {
                mapX = LARGE_MAP_X;
                mapY = LARGE_MAP_Y;
                mapW = LARGE_MAP_WIDTH;
                mapH = LARGE_MAP_HEIGHT;

                scaleFactor = (LARGE_MAP_WIDTH / (2 * DEFAULT_ZOOM));
            }*/

            gVRef = (GoodVibe) Program.game.World.getObject("Player");

            if (AUTO_ZOOM) {
                //float speed = gVRef.Body.MotionState.LinearVelocity.Length();
                float speed = estimateSpeed(gVRef);

                if (speed > MIN_ZOOM_SPEED)  {
                    ZOOM = DEFAULT_ZOOM + ((speed - MIN_ZOOM_SPEED) * 5);
                } else {
                    ZOOM = DEFAULT_ZOOM;
                }

                scaleFactor = (mapW / (2 * ZOOM));
            }

            // Calculate how far from the good vibe the corner of the map is.
            float corner_dist = (float) Math.Sqrt(Math.Pow((double) ZOOM, 2.0) * 2);

            // Draw fill
            spriteBatch.Draw(background, new Rectangle(mapX, mapY, mapW, mapH), BACKGROUND_COLOUR);

            // Draw scale lines, to provide a frame of reference.
            if (DRAW_SCALE_LINES)  {
                for (float i = mapW / 2; i < mapW; i += scaleFactor * SCALE_LINE_INTERVAL)  {
                    spriteBatch.Draw(background, new Rectangle(mapX + (int) i, mapY, 1, mapH), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(mapX + mapW - (int) i, mapY, 1, mapH), SCALE_LINE_COLOUR);
                }
                for (float i = mapH / 2; i < mapH; i += scaleFactor * SCALE_LINE_INTERVAL) {
                    spriteBatch.Draw(background, new Rectangle(mapX, mapY + (int) i, mapW, 1), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(mapX, mapY + mapH - (int) i, mapW, 1), SCALE_LINE_COLOUR);
                }
            }

            // Draw world
            if (DRAW_WORLD_BOX) drawWorldBox(spriteBatch, ((StaticObject)Program.game.World.getObject("Ground")).Body.BoundingBox);

            // Draw good vibe
            int gvx = mapX + (int) ((mapW / 2f) - (VIBE_WIDTH / 2f));
            int gvy = mapY + (int) ((mapH / 2f) - (VIBE_HEIGHT / 2f));

            float r = 0f;// gVRef.Body.Orientation.Y;
            spriteBatch.Draw(vibe, new Vector2(gvx, gvy), null, GOOD_VIBE_COLOUR, r, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Loop through and draw bad vibes.
            List<BadVibe> badVibes = Program.game.World.returnBadVibes();
            List<Pickup> pickups = PickupManager.returnPickups();
            List<Object>  toDraw   = new List<Object>();

            foreach (BadVibe b in badVibes) {
                toDraw.Add((Object) b);
            }
            foreach (Pickup p in pickups) {
                toDraw.Add((Object) p);
            }

            Vector2 gVPos = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            Vector2 objPos       = Vector2.Zero;
            Vector2 objScreenPos = Vector2.Zero;
            bool  inXRange, inYRange;
           
            foreach(Object o in toDraw) {
                BadVibe v = null;
                Pickup  p = null;
                if (o is BadVibe) v = (BadVibe) o;
                if (o is Pickup)  p = (Pickup)  o;

                     if (o is BadVibe) objPos = new Vector2(v.Body.Position.X,    v.Body.Position.Z);
                else if (o is Pickup)  objPos = new Vector2(p.OriginalPosition.X, p.OriginalPosition.Z);

                Vector2 relToGV = gVPos - objPos;
                float angle = (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                relToGV = Utility.rotateVector2(relToGV, angle);
                objPos = gVPos - relToGV;

                float alpha = BAD_VIBE_ALPHA;
                if (o is Pickup) alpha = PICKUP_ALPHA;

                inXRange = false;
                inYRange = false;

                // Check if bad vibe is in range
                if ((objPos.X > gVPos.X - ZOOM) && (objPos.X < gVPos.X + ZOOM)) inXRange = true;
                if ((objPos.Y > gVPos.Y - ZOOM) && (objPos.Y < gVPos.Y + ZOOM)) inYRange = true;

                if (inXRange && inYRange) {
                    float objR = 0f;

                    if (o is BadVibe) {
                        objR = -(Utility.QuaternionToEuler(v.Body.Orientation)).Y + (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                        objR += (float)Math.PI;
                    }
                    
                    objScreenPos = new Vector2(gvx + ((objPos.X - gVPos.X) * scaleFactor), gvy + ((objPos.Y - gVPos.Y) * scaleFactor));
                    Vector2 centre = new Vector2(vibe.Width / 2f, vibe.Height / 2f);

                    if (o is BadVibe) {
                        spriteBatch.Draw(vibe,   new Vector2((int)objScreenPos.X, (int)objScreenPos.Y), null, BAD_VIBE_COLOUR, objR, centre, 1f, SpriteEffects.None, 0f);
                    } else if (o is Pickup) {
                        spriteBatch.Draw(pickup, new Vector2((int)objScreenPos.X, (int)objScreenPos.Y), null, PICKUP_COLOUR,   objR, centre, 1f, SpriteEffects.None, 0f);
                    }
                } else if ((o is BadVibe) && (inXRange ^ inYRange)) {
                    float dist = Vector3.Distance(Resonance.Game.getGV().Body.Position, v.Body.Position);
                    Vector2 bVPos = objPos;
                    Vector2 bVScreenPos = objScreenPos;
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
                        if (bVPos.X < gVPos.X)  {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        } else {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                    }

                    if (visible) {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color c = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), c);
                    }
                } else if (o is BadVibe) {
                    Vector2 bVPos = objPos;
                    Vector2 bVScreenPos = objScreenPos;

                    // Draw in corresponding corner, transparency proportional to distance.

                    float dist = Vector3.Distance(Resonance.Game.getGV().Body.Position, v.Body.Position);
                    bool visible = true;

                    // Calculate the alpha transparency that the distant vibe should have. Determine whether it is visible or not.
                    alpha = calculateBVAlpha(dist, corner_dist);
                    if (alpha < 0f) visible = false;

                    if (bVPos.X < gVPos.X) {
                        if (bVPos.Y < gVPos.Y) {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), mapY - (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2(mapX - (dVibe.Width / 2), mapY + mapH - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.Y < gVPos.Y) {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), mapY - (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2(mapX + mapW - (dVibe.Width / 2), mapY + mapH - (dVibe.Height / 2));
                        }
                    }

                    if (visible) {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color c = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), c);
                    }
                }
            }

            // Draw sweeper
            if (SWEEPER_ON) {
                if (sweeperX < mapX) sweeperX += mapW;

                //spriteBatch.Draw(background, new Rectangle(sweeperX, mapY, 1, mapH), SWEEPER_COLOUR);

                int x;
                float alpha = BAD_VIBE_ALPHA;

                for (int i = 0; i < SWEEPER_LENGTH; i++) {
                    alpha -= (BAD_VIBE_ALPHA / (float) SWEEPER_LENGTH);

                    x = sweeperX + i;
                    if (x > mapX + mapW) x -= mapW;
                    spriteBatch.Draw(block, new Rectangle(x, mapY, 1, mapH), new Color(0f, 0f, 0.9f + alpha - 1f, alpha));
                }

                sweeperX--;
            }

            // Draw outline
            spriteBatch.Draw(outline, new Microsoft.Xna.Framework.Rectangle(mapX, mapY, mapW, mapH), OUTLINE_COLOUR);
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
    }
}
