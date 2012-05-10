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
using ResonanceLibrary;
using System.IO;
using AnimationLibrary;
using System.Diagnostics;
using BEPUphysics.MathExtensions;

namespace Resonance
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScreen : Screen
    {
        public const int BEGINNER = 0;
        public const int EASY = 1;
        public const int MEDIUM = 2;
        public const int HARD = 3;
        public const int EXPERT = 4;
        public const int INSANE = 5;

        public static int DIFFICULTY = BEGINNER;
        public static GameMode mode = new GameMode(GameMode.OBJECTIVES);
        public static GameStats stats = new GameStats();
        public static float VIBRATION = 0.4f;

        public static bool GV_KILLED = false;
        public static bool GV_KILLED_AT_GAME_END = false;
        public static bool GAME_CAN_END = true;
        public const bool USE_BV_SPAWNER = true;
        public const bool USE_PICKUP_SPAWNER = true;
        public const bool USE_MINIMAP = true;
        public const bool USE_BADVIBE_AI = true;
        public const bool USE_WHEATHER = true;
        public const bool USE_PROFILER = false;

        private Stopwatch preEndGameTimer;
        private int preEndGameDuration = 4000;
        GraphicsDeviceManager graphics;
        World world;
        BVSpawnManager bvSpawner;
        public PickupSpawnManager pickupSpawner;

        bool isLoaded;
        int iteration = 0;

        TimeSpan countDown;
        bool intro;

        private int zone;

        static Profile DrawSection = Profile.Get("DrawingTotal");
        static Profile UpdateSection = Profile.Get("UpdateTotal");

        //Allocated variables
        string[] deadVibes;

        public GameScreen(ScreenManager scrn, int level)
        {
            zone = level;
            isLoaded = false;
            GV_KILLED = false;
            this.ScreenManager = scrn;
            countDown = TimeSpan.FromSeconds(5);
            intro = false;
            MusicHandler.reset();
            graphics = Program.game.GraphicsManager;
            Drawing.Init(ScreenManager.Content, graphics);
            preEndGameTimer = new Stopwatch();

            if (USE_BV_SPAWNER) bvSpawner = new BVSpawnManager();
            if (USE_PICKUP_SPAWNER) pickupSpawner = new PickupSpawnManager();
            deadVibes = new string[50];

            
           

        }

        public int Iteration
        {
            get { return iteration; }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected void Initialize()
        {
            // TODO: Add your initialization logic here
            //base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            //Clear any pre-existing GameComponents from the manager
            DrawableManager.Clear();

            if (!isLoaded)
            {
                BadVibe.initialiseBank();
                CameraMotionManager.initCamera();
                Drawing.loadContent();
                world = new World();

                //When loading a level via MenuActions the load is done in a separate thread and you get a nice loading screen
                loadLevel(zone);
                Drawing.reset();
                System.GC.Collect();
                isLoaded = true;
            }
        }

        /// <summary>
        /// This method is used to load a level. MenuAction calls this method from a separate thread 
        /// and therefore we can have an animated loading screen while you wait.
        /// </summary>
        /// <param name="i">Int number of the level, taken from the level name, i.e Level1.xml</param>
        public void loadLevel(int i)
        {
            allocate();
            string level = "Levels/Level" + i;
            world.readXmlFile(level, ScreenManager.Content);

            GVMotionManager.initialised = false;
            ParticleEmitterManager.initialise();
            WeatherManager.initialise();
            CameraMotionManager.initialise();
        }

        private void allocate()
        {
            world.allocate();
            BVSpawnManager.allocate();
            Shockwave.fillPool();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public override void HandleInput(InputDevices input)
        {
            bool pause = (!input.LastKeys.IsKeyDown(Keys.Escape) && input.LastPlayerOne.Buttons.Start != ButtonState.Pressed) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.Buttons.Start == ButtonState.Pressed);
            bool debug = (!input.LastKeys.IsKeyDown(Keys.Tab)) && (input.Keys.IsKeyDown(Keys.Tab));
            bool pad1Ever = input.PlayerOneHasBeenConnected;
            bool pad2Ever = input.PlayerTwoHasBeenConnected;
            bool connected = input.PlayerOne.IsConnected;

            if (pad1Ever && !connected || pause)
            {
                ScreenManager.addScreen(ScreenManager.pauseMenu);
            }
            if (debug)
            {
                ScreenManager.addScreen(ScreenManager.debugMenu);
            }
            if (intro)
            {
                //Player One
                GVManager.input(input);
                //Player Two
                DrumManager.input(input);
            }
            //Camera
            CameraMotionManager.update(input);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            using (IDisposable d = UpdateSection.Measure())
            {
                if(countDown.TotalMilliseconds > -2000) countDown -= gameTime.ElapsedGameTime;
                if(countDown < TimeSpan.Zero && !intro)
                {
                    intro = true;
                }

                DrawableManager.Update(gameTime);
                Drawing.Update(gameTime);
                MusicHandler.getTrack().playTrack();

                if (intro)
                {
                    if (USE_MINIMAP) Hud.saveMiniMap();

                    float health = getGV().healthFraction();
                    if (health < 0.1) MusicHandler.HeartBeat = true;
                    else MusicHandler.HeartBeat = false;

                    //Update bad vibe positions, frozen status, detect if GV
                    //in combat and break rest layers
                    if (!preEndGameTimer.IsRunning && USE_BADVIBE_AI)
                    {
                        int numberKilled = processBadVibes();
                        removeDeadBadVibes(numberKilled);
                    }

                    // Update shockwaves
                    getGV().updateWaves();

                    //Update pickups
                    if (USE_PICKUP_SPAWNER)
                    {
                        PickupManager.update();
                        pickupSpawner.update();
                    }

                    if (ParticleEmitterManager.isPaused()) ParticleEmitterManager.pause(false);
                    if (WeatherManager.isPaused()) WeatherManager.pause(false);
                }

                if (USE_WHEATHER) WeatherManager.update();
                MusicHandler.Update();
                world.update();

                if(intro)
                {

                    if (GAME_CAN_END)
                    {
                        string x = "";
                        if (GV_KILLED || mode.terminated() || (mode.MODE == GameMode.OBJECTIVES && (ObjectiveManager.getProgress(ref x))))
                        {
                            if (!preEndGameTimer.IsRunning)
                            {
                                preEndGameTimer.Start();
                                if (!GV_KILLED) MusicHandler.playSound("winwhoosh");
                                GV_KILLED_AT_GAME_END = GV_KILLED;
                            }
                        }

                        if (preEndGameTimer.ElapsedMilliseconds >= preEndGameDuration)
                        {
                            endGame();
                        }
                        else if (preEndGameTimer.IsRunning)
                        {
                            Vector3 lt = WeatherManager.getCurrentAmbientLight();
                            Vector3 newLt;
                            if (!GV_KILLED) {
                                newLt = new Vector3(lt.X + 0.05f, lt.Y + 0.05f, lt.Z + 0.05f);
                                getGV().Body.Position += new Vector3(0f, 0.075f, 0f);
                                getGV().Body.LinearVelocity = Vector3.Zero;
                            }
                            else newLt = new Vector3(lt.X - 0.01f, lt.Y - 0.05f, lt.Z - 0.05f);
                            WeatherManager.forceAmbientLight(newLt);
                            Drawing.setAmbientLight(newLt);
                        }
                    }
                }

                iteration++;
                if (iteration == 60) iteration = 0;
            }
        }

        /// <summary>
        /// Ensure everything is paused
        /// </summary>
        public void pause()
        {
            MusicHandler.getTrack().pauseTrack(); //TODO: check beat detection still works after resuming from pause
            WeatherManager.pause(true);
            ParticleEmitterManager.pause(true);
        }

        // Called when game finished (won or lost).
        private void endGame()
        {
            GV_KILLED = GV_KILLED_AT_GAME_END;
            if (mode.MODE == GameMode.ARCADE) {
                if (GV_KILLED)
                {
                    WeatherManager.playLightning();
                    ScreenManager.addScreen(new GameOverScreen(stats));
                }
                ScreenManager.addScreen(new SuccessScreen(stats));
            } else if (mode.MODE == GameMode.OBJECTIVES) {
                if (!GV_KILLED) {
                    if (ObjectiveManager.currentObjective() != ObjectiveManager.FINAL_OBJECTIVE) {
                        ObjectiveManager.nextObjective();
                        ObjectiveManager.loadObjectivesGame(ScreenManager);
                    } else {
                        ScreenManager.addScreen(new SuccessScreen(stats));
                    }
                } else {
                    WeatherManager.playLightning();
                    HighScoreManager.updateTable(stats.Score);
                    HighScoreManager.saveFile();
                    ScreenManager.addScreen(new GameOverScreen(stats));
                }
            }

        }

        /// <summary>
        /// Takes a screenshot of the game screen and saves a png in bin\x86\Debug
        /// </summary>
        public void takeScreenshot()
        {
            //http://www.brianclifton.com/blogs/programming/screenshot-xna-csharp
#if WINDOWS
    int w = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
    int h = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

    //force a frame to be drawn (otherwise back buffer is empty)
    Draw(new GameTime());

    //pull the picture from the buffer
    int[] backBuffer = new int[w * h];
    graphics.GraphicsDevice.GetBackBufferData(backBuffer);

    //copy into a texture
    Texture2D texture = new Texture2D(graphics.GraphicsDevice, w, h, false, graphics.GraphicsDevice.PresentationParameters.BackBufferFormat);
    texture.SetData(backBuffer);

    //save to disk
    Stream stream = File.OpenWrite("screenshot" + Guid.NewGuid().ToString() + ".png");
    texture.SaveAsPng(stream, w, h);
    stream.Close();

#endif 
        }

        /// <summary>
        /// Process all the bad vibes, either move or kill them
        /// </summary>
        /// <returns>the list of dead bad vibes</returns>
        private int processBadVibes()
        {
            int numberKilled = 0;
            int i;

            bool breakRest = (MusicHandler.getTrack().nextQuarterBeat());
            GoodVibe gv = getGV();
            gv.InCombat = false;
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();
            for(i = 0; i < bvs.Count; i++)
            {
                BadVibe bv = (BadVibe)bvs[i];
                if (bv.Status == BadVibe.State.DEAD)
                {
                    if (bv.getAnimationCounter() <= 0)
                    {
                        deadVibes[numberKilled] = bv.returnIdentifier();
                        numberKilled++;
                    }
                    bv.decrementAnimationCounter();
                }
                else
                {
                    if(bv.Status == BadVibe.State.NORMAL) bv.Move();
                    if (breakRest) bv.damage(Shockwave.REST);

                    double dx = gv.Body.Position.X - bv.Body.Position.X;
                    double dz = gv.Body.Position.Z - bv.Body.Position.Z;
                    double d = Math.Pow(dx, 2) + Math.Pow(dz, 2);
                    d = Math.Sqrt(d);

                    if (d <= Shockwave.MAX_RADIUS * 2) //Doubled radius of influence of freeze
                    {
                        if (gv.FreezeActive)
                        {
                            bv.Status = BadVibe.State.FROZEN;
                        }
                        else
                        {
                            bv.Status = BadVibe.State.NORMAL;
                        }
                        if (d <= Shockwave.MAX_RADIUS)
                        {
                            gv.InCombat = true;
                        }
                    }
                    else
                    {
                        if (bv.Status == BadVibe.State.FROZEN) bv.Status = BadVibe.State.NORMAL;
                    }
                }
            }

            BulletManager.updateBullet();
            
            gv.FreezeActive = false;
            return numberKilled;
        }

        /// <summary>
        /// Remove all dead bad vibes from the game
        /// </summary>
        /// <param name="deadVibes">the list of dead bad vibes</param>
        private void removeDeadBadVibes(int numberKilled)
        {
            for (int i = 0; i < numberKilled; i++)
            {
                if (USE_BV_SPAWNER) BVSpawnManager.vibeDied((BadVibe)World.getObject(deadVibes[i]));
                stats.addBV();
            }
        }

        public static GoodVibe getGV()
        {
            return (GoodVibe)ScreenManager.game.World.getObject("Player");
        }

        public World World
        {
            get { return world; }
        }

        public TimeSpan CountDown
        {
            get { return countDown; }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            using (IDisposable d = DrawSection.Measure())
            {
                Hud.saveHealthBar();
                Hud.saveShieldBar();
                Hud.saveNitroBar();
                Hud.saveFreezeBar();
                Drawing.Draw(gameTime);
            }
        }
    }
}
