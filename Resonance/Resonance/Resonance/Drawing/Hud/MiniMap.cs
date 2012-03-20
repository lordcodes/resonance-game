using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using BEPUphysics;

namespace Resonance
{
    class MiniMap
    {
        /// Constants
        public static int MAP_X = ScreenManager.pixelsX(1920 - 250);
        public static int MAP_Y = ScreenManager.pixelsY(1080 - 250);

        public static int LARGE_MAP_X = ScreenManager.pixelsX(585);
        public static int LARGE_MAP_Y = ScreenManager.pixelsY(165);

        public static int MAP_WIDTH = ScreenManager.pixelsX(220);
        public static int MAP_HEIGHT = ScreenManager.pixelsY(220);

        public static int LARGE_MAP_WIDTH = ScreenManager.pixelsX(750);
        public static int LARGE_MAP_HEIGHT = ScreenManager.pixelsY(750);

        public static int   VIBE_WIDTH           = 16;
        public static int   VIBE_HEIGHT          = 20;

        public static float ZOOM                 = 40f;
        public static float DEFAULT_ZOOM         = 80f;

        public static bool  AUTO_ZOOM            = true;
        public static float MIN_ZOOM_SPEED       = 2f;
        public static float AUTO_ZOOM_STRENGTH   = 2f;

        public static bool  DRAW_SCALE_LINES     = true;
        public static int   SCALE_LINE_INTERVAL  = 12;

        public static bool  SWEEPER_ON           = true;
        public static int   SWEEPER_LENGTH       = 20;

        public static bool  DRAW_WORLD_BOX       = true;
        public static int   WORLD_BOX_THICKNESS  = 12;

        public static bool  DRAW_STATIC_OBJECTS  = true;
        public static int   BOX_THICKNESS        = 1;

        // Distance outside radar at which distant Vibe marker fades. 
        public static float VANISHING_POINT      = 5f;

        public static float BAD_VIBE_ALPHA       = 0.5f;
        public static float PICKUP_ALPHA         = 0.5f;
        public static float SPAWNER_ALPHA        = 0.5f;

        // Colours
        public static Color    OUTLINE_COLOUR = new Color(0.0f, 0.0f, 0.0f, 1.0f); // 0.8 alpha?
        public static Color BACKGROUND_COLOUR = new Color(0.0f, 0.0f, 0.2f, 0.5f);
        public static Color  GOOD_VIBE_COLOUR = new Color(0.0f, 0.7f, 0.0f, 0.5f);
        public static Color   BAD_VIBE_COLOUR = new Color(0.7f, 0.0f, 0.0f, BAD_VIBE_ALPHA);
        public static Color PROJ_BAD_VIBE_COLOUR = new Color(0.8f, 0.0f, 0.0f, BAD_VIBE_ALPHA);
        public static Color     PICKUP_COLOUR = new Color(0.7f, 0.7f, 0.0f, PICKUP_ALPHA);
        public static Color    SPAWNER_COLOUR = new Color(0.7f, 0.7f, 0.0f, SPAWNER_ALPHA);
        public static Color SCALE_LINE_COLOUR = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public static Color    SWEEPER_COLOUR = new Color(0.0f, 0.0f, 0.9f, 0.5f);
        public static Color         BOX_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        public static Color  WORLD_BOX_COLOUR = new Color(0.0f, 0.0f, 0.5f, 0.05f);

        /// Fields

        public  static bool      large;
 
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;
        private static Texture2D dVibe;
        private static Texture2D block;
        private static Texture2D pickup;
        private static Texture2D spawner;
        private static Texture2D texPixel;

        private static int SPEED_SAMPLES = 10;
        private static List<float> speeds;

        private static int sweeperX = MAP_X + MAP_WIDTH;

        // Defines scale for things which grow larger in large map mode.
        private static float scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

        // Defines scale for things which stay the same size in small and large map mode.
        private static float sF = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

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
            spawner    = content.Load<Texture2D>("Drawing/HUD/Textures/spawner");
            texPixel   = content.Load<Texture2D>("Drawing/Textures/texPixel");
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
        /// Draws the world's bounding box.
        /// </summary>
        private void drawWorldBox(SpriteBatch spriteBatch, BoundingBox bBox, int gvx, int gvy) {
            Vector3 min = bBox.Min;
            Vector3 max = new Vector3(-min.X, -min.Y, -min.Z);

            Vector2[] cnrScrPos = new Vector2[4];
            Vector2[] corners   = new Vector2[4];

            Vector2 gVPos2D = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            corners[0].X = min.X; corners[0].Y = min.Z;
            corners[1].X = min.X; corners[1].Y = max.Z;
            corners[2].X = max.X; corners[2].Y = max.Z;
            corners[3].X = max.X; corners[3].Y = min.Z;

            Vector2 relToGV, objPos;
            float angle = (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;;


            // Calculate positions of box corners on screen.
            for (int i = 0; i < 4; i++) {
                relToGV      = gVPos2D - corners[i];
                relToGV      = Utility.rotateVector2(relToGV, angle);
                objPos       = gVPos2D - relToGV;
                cnrScrPos[i] = new Vector2(gvx + ((objPos.X - gVPos2D.X) * scaleFactor), gvy + ((objPos.Y - gVPos2D.Y) * scaleFactor));
            }

            for (int i = 0; i < WORLD_BOX_THICKNESS; i++) {
                Color c = new Color(0f, 0f, WORLD_BOX_COLOUR.B / (255f * i), WORLD_BOX_COLOUR.A / 255f);
                Utility.drawBox(spriteBatch, texPixel, cnrScrPos, c, i);
            }

            // Draws world corners
            /*for (int i = 0; i < 4; i++) {
                switch (i) {
                    case(0) : {
                        spriteBatch.Draw(pickup, new Rectangle((int) cnrScrPos[i].X, (int) cnrScrPos[i].Y, 25, 25), null, Color.Red, 0, new Vector2(12.5f, 12.5f), SpriteEffects.None, 0f);
                        break;
                    }
                    case(1) : {
                        spriteBatch.Draw(pickup, new Rectangle((int) cnrScrPos[i].X, (int) cnrScrPos[i].Y, 25, 25), null, Color.Green, 0, new Vector2(12.5f, 12.5f), SpriteEffects.None, 0f);
                        break;
                    }
                    case(2) : {
                        spriteBatch.Draw(pickup, new Rectangle((int) cnrScrPos[i].X, (int) cnrScrPos[i].Y, 25, 25), null, Color.Blue, 0, new Vector2(12.5f, 12.5f), SpriteEffects.None, 0f);
                        break;
                    }
                    case(3) : {
                        spriteBatch.Draw(pickup, new Rectangle((int) cnrScrPos[i].X, (int) cnrScrPos[i].Y, 25, 25), null, Color.Yellow, 0, new Vector2(12.5f, 12.5f), SpriteEffects.None, 0f);
                        break;
                    }
                }
            }*/
        }

                /// <summary>
        /// Draws the world's bounding box.
        /// </summary>
        private void drawBox(SpriteBatch spriteBatch, BoundingBox bBox, int gvx, int gvy) {
            Vector3 min = bBox.Min;
            Vector3 max = bBox.Max;

            Vector2[] cnrScrPos = new Vector2[4];
            Vector2[] corners   = new Vector2[4];

            Vector2 gVPos2D = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            corners[0].X = min.X; corners[0].Y = min.Z;
            corners[1].X = min.X; corners[1].Y = max.Z;
            corners[2].X = max.X; corners[2].Y = max.Z;
            corners[3].X = max.X; corners[3].Y = min.Z;

            Vector2 relToGV, objPos;
            float angle = (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;;


            // Calculate positions of box corners on screen.
            for (int i = 0; i < 4; i++) {
                relToGV      = gVPos2D - corners[i];
                relToGV      = Utility.rotateVector2(relToGV, angle);
                objPos       = gVPos2D - relToGV;
                cnrScrPos[i] = new Vector2(gvx + ((objPos.X - gVPos2D.X) * scaleFactor), gvy + ((objPos.Y - gVPos2D.Y) * scaleFactor));
            }

            Utility.drawBox(spriteBatch, texPixel, cnrScrPos, BOX_COLOR, BOX_THICKNESS);
        }

        ///<summary>
        /// Draws the minimap on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            // Good Vibe reference
            gVRef = (GoodVibe) ScreenManager.game.World.getObject("Player");

            // Drawn texture. Reassigned later, but set to texPixel to stop compiler complaining.
            Texture2D scaledTex = texPixel;

            if (AUTO_ZOOM) {
                float speed = estimateSpeed(gVRef);

                if (speed > MIN_ZOOM_SPEED)  {
                    ZOOM = DEFAULT_ZOOM + ((speed - MIN_ZOOM_SPEED) * AUTO_ZOOM_STRENGTH);
                } else {
                    ZOOM = DEFAULT_ZOOM;
                }

                scaleFactor = (mapW / (2 * ZOOM)); // MAP_W applies to both map sizes, mode dependent.
                sF = (MAP_WIDTH / (2 * ZOOM));     // MAP_WIDTH applies only to small map, as sF is mode independent.
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

            // GV's position on screen.
            /*int gvx = mapX + (int) ((mapW / 2f) - (VIBE_WIDTH / 2f));
            int gvy = mapY + (int) ((mapH / 2f) - (VIBE_HEIGHT / 2f));*/

            int gvx = mapX + (int) (mapW / 2f);
            int gvy = mapY + (int) (mapH / 2f);

            // Draw world
            if (DRAW_WORLD_BOX) drawWorldBox(spriteBatch, ((StaticObject)ScreenManager.game.World.getObject("Ground")).Body.BoundingBox, gvx, gvy);

            List<Object> objs = ScreenManager.game.World.returnObjectSubset<StaticObject>();

            foreach (StaticObject s in objs) {
                if (!(s is BVSpawner) && !(s.Equals(ScreenManager.game.World.getObject("Ground")))) {
                    try {
                        drawBox(spriteBatch, s.Body.BoundingBox, gvx, gvy);
                    } catch (Exception e) {}
                }
            }

            // Draw good vibe
            float r = 0f;// gVRef.Body.Orientation.Y;new Vector2((vibe.Width / 2f) * scaleFactor, (vibe.Height / 2f) * scaleFactor)
            Vector2 drawPos = new Vector2(gvx - (vibe.Width / 2f) * sF, gvy - (vibe.Height / 2f) * sF);
            Vector2 origin = new Vector2(vibe.Width * scaleFactor / 2f, vibe.Height * scaleFactor / 2f);
            origin = new Vector2(0f, 0f);
            //Texture2D scaled = Drawing.scaleTexture(vibe, 2f);
            spriteBatch.Draw(vibe, drawPos, null, GOOD_VIBE_COLOUR, r, origin, sF, SpriteEffects.None, 0f);

            // Loop through and draw stuff.
            List<Type> drawnTypes = new List<Type>() {typeof(BadVibe), typeof(BVSpawner), typeof(Pickup),typeof(Projectile_BV)};
            List<Object> toDraw = ScreenManager.game.World.returnObjectSubset(drawnTypes);

            Vector2 gVPos = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            Vector2 objPos       = Vector2.Zero;
            Vector2 objScreenPos = Vector2.Zero;
            Vector2 relToGV;
            Vector2 centre;
            Color drawColour = Color.Black;
            bool  inXRange, inYRange;
           
            foreach(Object o in toDraw) {
                BadVibe   v = null;
                Pickup    p = null;
                BVSpawner s = null;
                if (o is BadVibe || o is Projectile_BV)   v = (BadVibe)   o;
                if (o is Pickup)    p = (Pickup)    o;
                if (o is BVSpawner) s = (BVSpawner) o;

                if (o is BadVibe || o is Projectile_BV) objPos = new Vector2(v.Body.Position.X, v.Body.Position.Z);
                else if (o is Pickup)    objPos = new Vector2(p.OriginalPosition.X, p.OriginalPosition.Z);
                else if (o is BVSpawner) objPos = new Vector2(s.OriginalPosition.X, s.OriginalPosition.Z);

                relToGV = gVPos - objPos;
                float angle = (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                relToGV = Utility.rotateVector2(relToGV, angle);
                objPos = gVPos - relToGV; // Object position relative to origin.

                float alpha = BAD_VIBE_ALPHA;
                     if (o is Pickup)    alpha = PICKUP_ALPHA;
                else if (o is BVSpawner) alpha = SPAWNER_ALPHA;

                inXRange = false;
                inYRange = false;

                // Check if bad vibe is in range
                if ((objPos.X > gVPos.X - ZOOM) && (objPos.X < gVPos.X + ZOOM)) inXRange = true;
                if ((objPos.Y > gVPos.Y - ZOOM) && (objPos.Y < gVPos.Y + ZOOM)) inYRange = true;

                if (inXRange && inYRange) {
                    float objR = 0f;

                    if (o is BadVibe || o is Projectile_BV)
                    {
                        objR = -(Utility.QuaternionToEuler(v.Body.Orientation)).Y + (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                    }
                    
                    objScreenPos = new Vector2(gvx + ((objPos.X - gVPos.X) * scaleFactor), gvy + ((objPos.Y - gVPos.Y) * scaleFactor));

                    if (o is BadVibe || o is Projectile_BV) {            
                        scaledTex = Drawing.scaleTexture(vibe, sF);
                        if (o is BadVibe)
                            drawColour = BAD_VIBE_COLOUR;
                        else
                            drawColour = PROJ_BAD_VIBE_COLOUR;
                    } else if (o is Pickup) {
                        scaledTex = Drawing.scaleTexture(pickup, sF);
                        drawColour = PICKUP_COLOUR;
                    } else if (o is BVSpawner) {
                        scaledTex = Drawing.scaleTexture(spawner, sF);
                        drawColour = SPAWNER_COLOUR;
                    }

                    centre = new Vector2(scaledTex.Width / 2f, scaledTex.Height / 2f);
                    spriteBatch.Draw(scaledTex, objScreenPos, null, drawColour, objR, centre, 1f, SpriteEffects.None, 0f);
                }
                else if ((o is BadVibe || o is Projectile_BV) && (inXRange ^ inYRange))
                {
                    float dist = Vector3.Distance(GameScreen.getGV().Body.Position, v.Body.Position);
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
                }
                else if (o is BadVibe || o is Projectile_BV)
                {
                    Vector2 bVPos = objPos;
                    Vector2 bVScreenPos = objScreenPos;

                    // Draw in corresponding corner, transparency proportional to distance.

                    float dist = Vector3.Distance(GameScreen.getGV().Body.Position, v.Body.Position);
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
                        Color c;
                        c = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
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

                sweeperX --;
            }

            // Draw outline
            spriteBatch.Draw(outline, new Microsoft.Xna.Framework.Rectangle(mapX, mapY, mapW, mapH), OUTLINE_COLOUR);
        }

        // Calculates the alpha transparency of a bad vibe.
        private static float calculateBVAlpha(float dist, float corner_dist) {
            if (dist > corner_dist) {
                float diff = dist - corner_dist;
                if (diff > VANISHING_POINT * scaleFactor) {
                    return -1f;
                } else {
                    float ratio = diff / (VANISHING_POINT * scaleFactor);
                    return BAD_VIBE_ALPHA - (ratio * BAD_VIBE_ALPHA);
                }
            }

            return BAD_VIBE_ALPHA;
        }
    }
}
