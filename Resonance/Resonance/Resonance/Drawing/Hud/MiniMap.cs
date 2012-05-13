using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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
        public static int   SCALE_LINE_INTERVAL  = 24;

        public static bool  SWEEPER_ON           = true;
        public static int   SWEEPER_LENGTH       = 20;

        public static bool  DRAW_WORLD_BOX       = true;
        public static int   WORLD_BOX_THICKNESS  = 12;

        public static bool  DRAW_STATIC_OBJECTS  = true;
        public static int   BOX_THICKNESS        = 1;

        // Distance outside radar at which distant Vibe marker fades. 
        public static float VANISHING_POINT      = 80f;

        public static float ENTIRE_MAP_ALPHA     = 0.7f;
        public static float BAD_VIBE_ALPHA       = 0.8f;
        public static float PICKUP_ALPHA         = 0.8f;
        public static float SPAWNER_ALPHA        = 0.8f;

        // Colours
        //public static Color       OUTLINE_COLOUR = new Color(0.0f, 0.0f, 0.0f, 1.0f          ); // 0.8 alpha?
        public static Color    BACKGROUND_COLOUR = new Color(0.0f, 0.0f, 0.2f, 0.5f          );
        public static Color     GOOD_VIBE_COLOUR = new Color(0.0f, 0.7f, 0.0f, 0.5f          );
        public static Color      BAD_VIBE_COLOUR = new Color(0.7f, 0.0f, 0.0f, BAD_VIBE_ALPHA);
        public static Color PROJ_BAD_VIBE_COLOUR = new Color(0.8f, 0.0f, 0.0f, BAD_VIBE_ALPHA);
        public static Color        PICKUP_COLOUR = new Color(0.7f, 0.7f, 0.0f, PICKUP_ALPHA  );
        public static Color       SPAWNER_COLOUR = new Color(0.7f, 0.7f, 0.0f, SPAWNER_ALPHA );
        public static Color    SCALE_LINE_COLOUR = new Color(0.1f, 0.1f, 0.1f, 0.7f          );
        public static Color       SWEEPER_COLOUR = new Color(0.0f, 0.0f, 0.9f, 0.5f          );
        public static Color            BOX_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.5f          );
        public static Color     WORLD_BOX_COLOUR = new Color(0.0f, 0.0f, 0.5f, 0.05f         );

        /// Fields

        public  static bool      large;

        private static GraphicsDeviceManager graphics;
        private static RenderTarget2D miniMapBuffer;
        private static Texture2D outline;
        private static Texture2D background;
        private static Texture2D vibe;
        private static Texture2D dVibe;
        private static Texture2D block;
        private static Texture2D pickup;
        private static Texture2D spawner;
        private static Texture2D texPixel;
        private static Texture2D checkpoint;

        private static int SPEED_SAMPLES = 10;
        private static List<float> speeds;

        private static int sweeperX = MAP_WIDTH;

        // Defines scale for things which grow larger in large map mode.
        private static float scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));

        // Defines scale for things which stay the same size in small and large map mode.
        private static float sF = (MAP_WIDTH / (3 * DEFAULT_ZOOM));

        private GoodVibe gVRef;


        private static int SCALE_GRANULARITY         = 10;
        private static Texture2D[] scaledVibes       = new Texture2D[SCALE_GRANULARITY];
        private static Texture2D[] scaledPickups     = new Texture2D[SCALE_GRANULARITY];
        private static Texture2D[] scaledSpawners    = new Texture2D[SCALE_GRANULARITY];
        private static Texture2D[] scaledCheckpoints = new Texture2D[SCALE_GRANULARITY];

        private static int mapX, mapY, mapH, mapW;

        private static Texture2D savedMinimap;

        /// Constructor

        ///<summary>
        /// Create a new MiniMap
        ///</summary>
        public MiniMap(GraphicsDeviceManager graphics)
        {
            MiniMap.graphics = graphics;

            ZOOM = DEFAULT_ZOOM;
            large = false;

            speeds = new List<float>();

            for (int i = 0; i < SPEED_SAMPLES; i++) {
                speeds.Add(0f);
            }
        }


        /// Methods
        
        public static void createScaledTextures() {
                  scaledVibes[0] = texPixel;
                scaledPickups[0] = texPixel;
               scaledSpawners[0] = texPixel;
            scaledCheckpoints[0] = texPixel;

            float scale = 1f / (float) SCALE_GRANULARITY;
            float step  = scale;

            for (int i = 1; i < SCALE_GRANULARITY; i++) {
                      scaledVibes[i] = Drawing.scaleTexture(vibe      , scale);
                    scaledPickups[i] = Drawing.scaleTexture(pickup    , scale);
                   scaledSpawners[i] = Drawing.scaleTexture(spawner   , scale);
                scaledCheckpoints[i] = Drawing.scaleTexture(checkpoint, scale);
                scale += step;
            }
        }


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
            checkpoint = content.Load<Texture2D>("Drawing/HUD/Textures/checkpoint");
            miniMapBuffer = new RenderTarget2D(graphics.GraphicsDevice, MAP_WIDTH, MAP_HEIGHT, true, SurfaceFormat.Color, DepthFormat.Depth24);
            
            createScaledTextures();
        }


        public static void enlarge() {
            large = true;

            mapX = LARGE_MAP_X;
            mapY = LARGE_MAP_Y;
            mapW = LARGE_MAP_WIDTH;
            mapH = LARGE_MAP_HEIGHT;

            scaleFactor = (LARGE_MAP_WIDTH / (2 * DEFAULT_ZOOM));
            miniMapBuffer = new RenderTarget2D(graphics.GraphicsDevice, LARGE_MAP_WIDTH, LARGE_MAP_HEIGHT, true, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public static void ensmall()
        {
            large = false;

            mapX = MAP_X;
            mapY = MAP_Y;
            mapW = MAP_WIDTH;
            mapH = MAP_HEIGHT;

            scaleFactor = (MAP_WIDTH / (2 * DEFAULT_ZOOM));
            miniMapBuffer = new RenderTarget2D(graphics.GraphicsDevice, MAP_WIDTH, MAP_HEIGHT, true, SurfaceFormat.Color, DepthFormat.Depth24);
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

            for (int i = 0; i < speeds.Count; i++) {
                sum += speeds[i];
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
            if(savedMinimap != null)spriteBatch.Draw(savedMinimap, new Vector2(mapX, mapY), Color.White * ENTIRE_MAP_ALPHA);
            spriteBatch.Draw(outline, new Rectangle(mapX, mapY, mapW, mapH), Color.White);
        }

        /// <summary>
        /// Renders the mini map texture in the miniMapBuffer render target
        /// </summary>
        public void saveTexture(SpriteBatch spriteBatch)
        {
            GraphicsDevice gd = graphics.GraphicsDevice;
            gd.SetRenderTarget(miniMapBuffer);
            gd.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); //TODO: change to alpha blend and fix corner masking

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
                sF = (MAP_WIDTH / (3 * DEFAULT_ZOOM));     // MAP_WIDTH applies only to small map, as sF is mode independent.
            }

            // Calculate how far from the good vibe the corner of the map is.
            float corner_dist = (float) Math.Sqrt(Math.Pow((double) ZOOM, 2.0) * 2);

            // Draw fill
            spriteBatch.Draw(background, new Rectangle(0, 0, mapW, mapH), BACKGROUND_COLOUR);

            // Draw scale lines, to provide a frame of reference.
            if (DRAW_SCALE_LINES)  {
                for (float i = mapW / 2; i < mapW; i += scaleFactor * SCALE_LINE_INTERVAL)  {
                    spriteBatch.Draw(background, new Rectangle((int) i, 0, 1, mapH), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(mapW - (int) i, 0, 1, mapH), SCALE_LINE_COLOUR);
                }
                for (float i = mapH / 2; i < mapH; i += scaleFactor * SCALE_LINE_INTERVAL) {
                    spriteBatch.Draw(background, new Rectangle(0, (int) i, mapW, 1), SCALE_LINE_COLOUR);
                    spriteBatch.Draw(background, new Rectangle(0, mapH - (int) i, mapW, 1), SCALE_LINE_COLOUR);
                }
            }

            // GV's position on screen.
            /*int gvx = mapX + (int) ((mapW / 2f) - (VIBE_WIDTH / 2f));
            int gvy = mapY + (int) ((mapH / 2f) - (VIBE_HEIGHT / 2f));*/

            int gvx = (int) (mapW / 2f);
            int gvy =(int) (mapH / 2f);

            // Draw world
            if (DRAW_WORLD_BOX) drawWorldBox(spriteBatch, ((StaticObject)ScreenManager.game.World.getObject("Walls")).Body.BoundingBox, gvx, gvy);

            List<Object> objs = ScreenManager.game.World.returnObjectSubset<StaticObject>();

            StaticObject sObj;
            for (int i = 0; i < objs.Count; i++ ) {
                sObj = (StaticObject) objs[i];
                if (!(sObj is BVSpawner) && !(sObj.Equals(ScreenManager.game.World.getObject("Ground")))) {
                    try {
                        drawBox(spriteBatch, sObj.Body.BoundingBox, gvx, gvy);
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
            List<Type> drawnTypes = new List<Type>() {typeof(BadVibe), typeof(BVSpawner), typeof(Pickup), typeof(Checkpoint), typeof(ObjectivePickup)};
            List<Object> toDraw = ScreenManager.game.World.returnObjectSubset(drawnTypes);

            Vector2 gVPos = new Vector2(gVRef.Body.Position.X, gVRef.Body.Position.Z);

            Vector2 objPos       = Vector2.Zero;
            Vector2 objScreenPos = Vector2.Zero;
            Vector2 relToGV;
            Vector2 centre;
            Color drawColour = Color.Black;
            bool  inXRange, inYRange;

            // Scale each texture
            /*Texture2D scaledVibe    = Drawing.scaleTexture(vibe   , sF);
            Texture2D scaledPickup  = Drawing.scaleTexture(pickup , sF);
            Texture2D scaledSpawner = Drawing.scaleTexture(spawner, sF);*/

            int idx = (int) (((float) SCALE_GRANULARITY) * sF);
            Texture2D scaledVibe       =       scaledVibes[idx];
            Texture2D scaledPickup     =     scaledPickups[idx];
            Texture2D scaledSpawner    =    scaledSpawners[idx];
            Texture2D scaledCheckpoint = scaledCheckpoints[idx];

            Object          o;
            BadVibe         v;
            Pickup          p;
            BVSpawner       s;
            Checkpoint      c;
            ObjectivePickup op;
            for (int i = 0; i < toDraw.Count; i++ )
            {
                o  = toDraw[i];
                v  = null;
                p  = null;
                s  = null;
                c  = null;
                op = null;
                if (o is BadVibe)         v  = (BadVibe)         o;
                if (o is Pickup)          p  = (Pickup)          o;
                if (o is BVSpawner)       s  = (BVSpawner)       o;
                if (o is Checkpoint)      c  = (Checkpoint)      o;
                if (o is ObjectivePickup) op = (ObjectivePickup) o;

                if (o is BadVibe) objPos = new Vector2(v.Body.Position.X, v.Body.Position.Z);
                else if (o is Pickup) objPos = new Vector2(p.OriginalPosition.X, p.OriginalPosition.Z);
                else if (o is BVSpawner) objPos = new Vector2(s.OriginalPosition.X, s.OriginalPosition.Z);
                else if (o is Checkpoint) objPos = new Vector2(c.OriginalPosition.X, c.OriginalPosition.Z);
                else if (o is ObjectivePickup) objPos = new Vector2(op.Body.Position.X, op.Body.Position.Z);

                relToGV = gVPos - objPos;
                float angle = (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                relToGV = Utility.rotateVector2(relToGV, angle);
                objPos = gVPos - relToGV; // Object position relative to origin.

                float alpha = BAD_VIBE_ALPHA;
                if (o is Pickup) alpha = PICKUP_ALPHA;
                else if (o is BVSpawner) alpha = SPAWNER_ALPHA;

                inXRange = false;
                inYRange = false;

                // Check if bad vibe is in range
                if ((objPos.X > gVPos.X - ZOOM) && (objPos.X < gVPos.X + ZOOM)) inXRange = true;
                if ((objPos.Y > gVPos.Y - ZOOM) && (objPos.Y < gVPos.Y + ZOOM)) inYRange = true;

                if (inXRange && inYRange) {
                    float objR = 0f;

                    if (o is BadVibe) {
                        objR = -(Utility.QuaternionToEuler(v.Body.Orientation)).Y + (Utility.QuaternionToEuler(gVRef.Body.Orientation)).Y;
                    }

                    objScreenPos = new Vector2(gvx + ((objPos.X - gVPos.X) * scaleFactor), gvy + ((objPos.Y - gVPos.Y) * scaleFactor));

                    if (o is BadVibe) {
                        scaledTex = scaledVibe;
                        if (o is BadVibe) drawColour = BAD_VIBE_COLOUR;
                        else drawColour = PROJ_BAD_VIBE_COLOUR;
                    } else if (o is Pickup || o is ObjectivePickup) {
                        scaledTex = scaledPickup;
                        drawColour = PICKUP_COLOUR;
                    } else if (o is BVSpawner)  {
                        scaledTex = scaledSpawner;
                        drawColour = SPAWNER_COLOUR;
                    } else if (o is Checkpoint)  {
                        scaledTex = scaledCheckpoint;
                        drawColour = c.getColour();
                    }

                    centre = new Vector2(scaledTex.Width / 2f, scaledTex.Height / 2f);
                    spriteBatch.Draw(scaledTex, objScreenPos, null, drawColour, objR, centre, 1f, SpriteEffects.None, 0f);
                }
                else if ((o is BadVibe) && (inXRange ^ inYRange))
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
                            //bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapY - (dVibe.Height / 2));
                            if (!large) bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), 3);
                            else bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), 13);
                        } else {
                            //bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapY + mapH - (dVibe.Height / 2));
                            if (!large) bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapH - 20);
                            else bVScreenPos = new Vector2(gvx + ((bVPos.X - gVPos.X) * scaleFactor), mapH - 30);
                        }
                    } else {
                        if (bVPos.X < gVPos.X) {
                            if (!large) bVScreenPos = new Vector2(3, gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                            else bVScreenPos = new Vector2(13, gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        } else {
                            if (!large) bVScreenPos = new Vector2(mapW - 20, gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                            else bVScreenPos = new Vector2(mapW - 30, gvy + ((bVPos.Y - gVPos.Y) * scaleFactor));
                        }
                    }

                    if (visible) {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color col = new Color(cVec.X, cVec.Y, cVec.Z, alpha + 0.2f);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), col);
                    }
                }
                else if (o is BadVibe)
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
                            bVScreenPos = new Vector2((dVibe.Width / 2), (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2((dVibe.Width / 2), mapH - (dVibe.Height / 2));
                        }
                    } else {
                        if (bVPos.Y < gVPos.Y) {
                            bVScreenPos = new Vector2(mapW - (dVibe.Width / 2), (dVibe.Height / 2));
                        } else {
                            bVScreenPos = new Vector2(mapW - (dVibe.Width / 2), mapH - (dVibe.Height / 2));
                        }
                    }

                    if (visible) {
                        Vector3 cVec = BAD_VIBE_COLOUR.ToVector3();
                        Color col;
                        col = new Color(cVec.X, cVec.Y, cVec.Z, alpha);
                        spriteBatch.Draw(dVibe, new Rectangle((int)bVScreenPos.X, (int)bVScreenPos.Y, VIBE_WIDTH, VIBE_HEIGHT), col);
                    }
                }
            }

            // Draw sweeper
            if (SWEEPER_ON) {
                if (sweeperX < 0) sweeperX += mapW;

                //spriteBatch.Draw(background, new Rectangle(sweeperX, mapY, 1, mapH), SWEEPER_COLOUR);

                int x;
                float alpha = BAD_VIBE_ALPHA;

                for (int i = 0; i < SWEEPER_LENGTH; i++) {
                    alpha -= (BAD_VIBE_ALPHA / (float) SWEEPER_LENGTH);

                    x = sweeperX + i;
                    if (x > mapW) x -= mapW;
                    spriteBatch.Draw(block, new Rectangle(x, 0, 1, mapH), new Color(0f, 0f, 0.9f + alpha - 1f, alpha));
                }

                sweeperX --;
            }

            // Draw outline
            //spriteBatch.Draw(outline, new Rectangle(0, 0, mapW, mapH), OUTLINE_COLOUR);

            spriteBatch.End();
            gd.SetRenderTarget(null);

            savedMinimap = (Texture2D)miniMapBuffer;
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
