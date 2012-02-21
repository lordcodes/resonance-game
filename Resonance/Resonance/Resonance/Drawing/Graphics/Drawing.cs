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
using BEPUphysics;

namespace Resonance
{
    class Drawing
    {
        private static bool FLOOR_REFLECTIONS = true;

        private static GraphicsDeviceManager graphics;
        private static ContentManager content;
        private static Hud hud;
        private static Graphics gameGraphics;
        private static int frameCounter;
        private static int frameTime;
        private static int currentFrameRate;
        private static Vector3 playerPos;
        private static bool drawingReflection = false;
        private static RenderTarget2D mirrorRenderTarget;
        private static Texture2D shinyFloorTexture;
        private static int drawCount = 0;
        static Texture2D sampleTexture;

        /// <summary>
        /// Change the ambient light level of the scene, use positive values to make it brighter, negative values to make it darker.  
        /// </summary>
        /// <param name="light">Vector3 of the additions that should be made to the RGB of the ambient light</param>
        public static void setAmbientLight(Vector3 light)
        {
            gameGraphics.CustomShaders.setAmbientLight(light);
        }

        public static ContentManager Content
        {
            get { return content; }
        }

        public static bool requestRender
        {
            get
            {
                if (FLOOR_REFLECTIONS && drawCount > 0)
                {
                    drawCount = 0;
                    return true;
                }
                return false;
            }
        }

        public static bool DoDisp
        {
            get;set;
        }

        public static bool DrawingReflection
        {
            get
            {
                return drawingReflection;
            }
        }

        public static Vector3 CameraPosition
        {
            get
            {
                return gameGraphics.CameraPosition;
            }
        }

        public static Vector3 CameraCoords
        {
            get
            {
                return gameGraphics.CameraCoords;
            }
        }


        public static void reset()
        {
            if (gameGraphics != null) gameGraphics.reset();
        }

        public static void drawReflection()
        {
            graphics.GraphicsDevice.SetRenderTarget(mirrorRenderTarget);
            drawingReflection = true;
        }

        public static void drawGame()
        {
            if (FLOOR_REFLECTIONS)
            {
                graphics.GraphicsDevice.SetRenderTarget(null);
                shinyFloorTexture = (Texture2D)mirrorRenderTarget;
                try
                {
                    ((GroundShader)gameGraphics.CustomShaders.Ground).setReflectionTexture(shinyFloorTexture);
                }catch(Exception){
                }
                drawingReflection = false;
            }
        }

        public static Texture2D flipTexture(Texture2D source, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            Color[] data = new Color[source.Width * source.Height];
            Color[] flippedData = new Color[data.Length];

            source.GetData<Color>(data);

            for (int x = 0; x < source.Width; x++)
                for (int y = 0; y < source.Height; y++)
                {
                    int idx = (horizontal ? source.Width - 1 - x : x) + ((vertical ? source.Height - 1 - y : y) * source.Width);
                    flippedData[x + y * source.Width] = data[idx];
                }

            flipped.SetData<Color>(flippedData);

            return flipped;
        }

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            content = newContent;
            graphics = newGraphics;
            GameModels.Init(content);
            gameGraphics = new Graphics(content, graphics);
            hud = new Hud(content,graphics, gameGraphics);
            playerPos = new Vector3(0,0,0);
            DoDisp = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            sampleTexture = content.Load<Texture2D>("Drawing/Textures/texMissing");
            hud.loadContent();
            GameModels.Load();
            gameGraphics.loadContent(content, graphics.GraphicsDevice);
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            mirrorRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, GraphicsSettings.REFLECTION_TEXTURE_SIZE, GraphicsSettings.REFLECTION_TEXTURE_SIZE, false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        /// <summary>
        /// Used to update any frame animation
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            // Fire trail experiment
            if (GVMotionManager.BOOSTING)
            {
                Random randomGen = new Random();
                int num = randomGen.Next(3);
                if (num == 1)
                {
                    new FireTextureEffect(1, 1, new Vector3(GameScreen.getGV().Body.Position.X + 1/(randomGen.Next(-3,2)+1f), 0.3f, GameScreen.getGV().Body.Position.Z));
                }
            }

            gameGraphics.Shaders.setPointLightPos(new Vector3(GameScreen.getGV().Body.Position.X, GameScreen.getGV().Body.Position.Y+3, GameScreen.getGV().Body.Position.Z));


            gameGraphics.update(new Vector2(0f, 0f));
        }

        public static Vector2 groundPos(Vector3 position3d, bool one)
        {
            Vector2 playerGroundPos = new Vector2();
            float pad = -2f;
            float groundWidth = World.MAP_X+pad;
            float groundHeight = World.MAP_Z+pad;
            float xDis = Math.Abs(position3d.X - World.MAP_MIN_X + pad/2);
            float yDis = Math.Abs(position3d.Z - World.MAP_MIN_Z + pad/2);
            if (!one)
            {
                playerGroundPos.X = (float)Math.Round(Graphics.DISP_WIDTH * (xDis / groundWidth));
                playerGroundPos.Y = (float)Math.Round(Graphics.DISP_WIDTH * (yDis / groundHeight));
            }
            else
            {
                playerGroundPos.X = 1-(float)(xDis / groundWidth);
                playerGroundPos.Y = (float)(yDis / groundHeight);

            }
            return playerGroundPos;
        }

        /// <summary>
        /// Add a displacement wave to the ground texture.
        /// </summary>
        /// <param name="position3d"></param>
        public static void addWave(Vector3 position3d)
        {
            gameGraphics.addWave(groundPos(position3d, false));
        }

        /// <summary>
        /// Responsible for drawing particles in 3D space.
        /// </summary>
        private static void drawParticles() {
            ParticleEmitterManager.update();
            Vector3 rotation;
            Vector3 pos;
            float size;
            Matrix texturePos, rX, rY, rZ, rR;
            Vector3 gVFwd, gVRdp, angV;
            Quaternion ang;

            foreach (Emitter e in ParticleEmitterManager.getEmitters()) {
                DebugDisplay.update("P Count", e.getParticles().Count.ToString());
                foreach (Particle p in e.getParticles()) {
                    try {
                        pos = p.getPos();
                        size = p.getSize();
                        texturePos = Matrix.CreateTranslation(pos);

                        if (!(e is Rain)) {
                            rotation = p.getRotation();

                           if (rotation != Vector3.Zero) {
                                rX = Matrix.CreateRotationX(rotation.X);
                                rY = Matrix.CreateRotationY(rotation.Y);
                                rZ = Matrix.CreateRotationZ(rotation.Z);
                                texturePos = Matrix.Multiply(rX, Matrix.Multiply(rY, Matrix.Multiply(rZ, texturePos)));
                            }
                        } else {
                            gVFwd = GameScreen.getGV().Body.OrientationMatrix.Forward;
                            gVRdp = p.getPos() - GameScreen.getGV().Body.Position;
                            gVFwd.Normalize();
                            gVRdp.Normalize();

                            rotation = Utility.QuaternionToEuler(GameScreen.getGV().Body.Orientation);

                            if (rotation != Vector3.Zero) {
                                rX = Matrix.CreateRotationX(rotation.X);
                                rY = Matrix.CreateRotationY(rotation.Y + 1.5f);
                                rZ = Matrix.CreateRotationZ(rotation.Z);
                                Toolbox.GetQuaternionBetweenNormalizedVectors(ref gVFwd, ref gVRdp, out ang);
                                angV = Utility.QuaternionToEuler(ang);
                                rR = Matrix.CreateRotationY(ang.Y);
                                texturePos = Matrix.Multiply(rX, Matrix.Multiply(rY, Matrix.Multiply(rZ, Matrix.Multiply(rR, texturePos))));
                            }
                        }

                        gameGraphics.drawParticle(e.getPTex(), texturePos, size, size, p.getColour());
                    } catch (Exception) {}
                }
            }
        }

        public static void DrawTexture(Texture2D texture, Matrix position, float width, float height)
        {
            gameGraphics.drawTexture(texture, position, width, height, drawingReflection);
        }

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            drawCount++;
            drawParticles();
            hud.Draw();
            hud.drawDebugInfo(DebugDisplay.getString());
            checkFrameRate(gameTime);
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            if (!worldObject.returnIdentifier().Equals("Ground") || (worldObject.returnIdentifier().Equals("Ground") && !DrawingReflection))
            {
                bool blend = false;
                Vector2 playerGroundPos = new Vector2(0f, 0f);
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                if (worldObject is GoodVibe)
                {
                    int health = ((GoodVibe)((DynamicObject)worldObject)).Health;
                    int score = GameScreen.stats.Score;
                    int nitro = ((GoodVibe)((DynamicObject)worldObject)).Nitro;
                    int shield = ((GoodVibe)((DynamicObject)worldObject)).Shield;
                    int freeze = ((GoodVibe)((DynamicObject)worldObject)).Freeze;


                    //DebugDisplay.update("HEALTH", health.ToString());
                    //DebugDisplay.update("SCORE", score.ToString());
                    //DebugDisplay.update("nitro", nitro.ToString());
                    //DebugDisplay.update("shield", shield.ToString());
                    //DebugDisplay.update("freeze", freeze.ToString());

                    hud.updateGoodVibe(health, score, nitro, shield, freeze);
                    playerPos = ((GoodVibe)((DynamicObject)worldObject)).Body.Position;
                }

                if (DoDisp && worldObject.returnIdentifier().Equals("Ground"))
                {
                    blend = true;
                }

                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                gameGraphics.Draw(worldObject, worldTransform, blend, drawingReflection);

                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                if (worldObject is BadVibe)
                {
                    //Gets list of remaining armour layers
                    List<int> seq = ((BadVibe)worldObject).getLayers();
                    hud.updateEnemy(worldObject.returnIdentifier(), pos, seq);
                }
            }
        }

        /// <summary>
        /// Updates Camera
        /// </summary>
        public static void UpdateCamera(Vector3 point, Vector3 cameraPosition)
        {
            gameGraphics.updateCamera(cameraPosition);
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
