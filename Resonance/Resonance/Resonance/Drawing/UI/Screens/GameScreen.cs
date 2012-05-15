using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScreen : Screen
    {
        public const int BEGINNER = 0;
        public const int EASY     = 1;
        public const int MEDIUM   = 2;
        public const int HARD     = 3;
        public const int EXPERT   = 4;
        public const int INSANE   = 5;

        public static int DIFFICULTY = BEGINNER;
        public static GameMode mode = new GameMode(GameMode.OBJECTIVES);
        public static float VIBRATION = 0.4f;

        public static bool GV_KILLED = false;
        public static bool GV_KILLED_AT_GAME_END = false;
        public static bool GAME_CAN_END = true;
        public const bool USE_BV_SPAWNER = true;
        public const bool USE_PICKUP_SPAWNER = true;
        public const bool USE_MINIMAP = true;
        public const bool USE_BADVIBE_AI = true;
        public const bool USE_WHEATHER = true;
        public const bool USE_PROFILER = true;
        public const bool USE_DEBUG = true;

        // Time in milliseconds between each update of the hud and the minimap
        private const float HUD_UPDATE_DELAY = 500;
        private const float MAP_UPDATE_DELAY = 100;

        private Stopwatch preEndGameTimer;
        private int preEndGameDuration = 4000;
        GraphicsDeviceManager graphics;
        World world;

        bool isLoaded;
        int iteration = 0;

        TimeSpan countDown;
        bool intro;
        bool endgame;

        private float hudTimeElapsed = 0;
        private float mapTimeElapsed = 0;

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
            endgame = false;
            if (mode.MODE == GameMode.ARCADE ||((mode.MODE == GameMode.OBJECTIVES) && (ObjectiveManager.currentObjective() == ObjectiveManager.DEFAULT_OBJECTIVE)))
            {
                MusicHandler.reset();
            }
            graphics = Program.game.GraphicsManager;
            Drawing.Init(ScreenManager.Content, graphics);
            preEndGameTimer = new Stopwatch();
            
            if(mode.MODE == GameMode.ARCADE || 
                (mode.MODE == GameMode.OBJECTIVES && ObjectiveManager.currentObjective() == ObjectiveManager.DEFAULT_OBJECTIVE)) 
            {
                GameStats.init();
            }

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

        public GameMode getMode() {
            return mode;
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
                LoadingScreen.CurrentlyLoading = "Initialising Bad Vibes";
                BadVibe.initialiseBank();
                LoadingScreen.CurrentlyLoading = "Initialising Camera";
                CameraMotionManager.initCamera();
                LoadingScreen.CurrentlyLoading = "Drawing";
                Drawing.loadContent();
                LoadingScreen.CurrentlyLoading = "Level";
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
            world.readXmlFile(level, ScreenManager.Content, i);

            GVMotionManager.initialised = false;
            ParticleEmitterManager.initialise();
            WeatherManager.initialise();
            CameraMotionManager.initialise();
            PickupSpawnManager.init();
            PickupManager.init();
            MiniMap.clear();
            if(BulletManager.BOSS_EXISTS) BulletManager.init();
            ObjectiveManager.defaultSetup();
            if (mode.MODE == GameMode.OBJECTIVES && ObjectiveManager.currentObjective() == ObjectiveManager.SURVIVE) ObjectiveManager.surviveSetup();
        }

        private void allocate()
        {
            world.allocate();
            BVSpawnManager.allocate();
            Shockwave.fillPool();
            PickupSpawnManager.allocate();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
        }

        public override void HandleInput(InputDevices input)
        {
            bool pause = (!input.LastKeys.IsKeyDown(Keys.Escape) && input.LastPlayerOne.Buttons.Start != ButtonState.Pressed) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.Buttons.Start == ButtonState.Pressed);
            bool debug = (!input.LastKeys.IsKeyDown(Keys.Tab)) && (input.Keys.IsKeyDown(Keys.Tab));
            bool pad1Ever = input.PlayerOneHasBeenConnected;
            bool pad2Ever = input.PlayerTwoHasBeenConnected;
            bool connected = input.PlayerOne.IsConnected;

            if (!endgame)
            {
                if (pad1Ever && !connected || pause)
                {
                    ScreenManager.addScreen(ScreenManager.pauseMenu);
                }
                if (debug)
                {
                    ScreenManager.addScreen(ScreenManager.debugMenu);
                }

                //Player One
                GVManager.input(input);
                //Player Two
                if (intro) DrumManager.input(input);
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

                if (intro && !endgame)
                {
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
                    if (BulletManager.BOSS_EXISTS) getBoss().faceGV();
                    BulletManager.updateBullet(gameTime);

                    //Update pickups
                    if (USE_PICKUP_SPAWNER && ObjectiveManager.currentObjective() != ObjectiveManager.COLLECT_ALL_PICKUPS)
                    {
                        PickupManager.update();
                        PickupSpawnManager.update();
                    }

                    //Update Objective Pickups
                    if(GameScreen.mode.MODE == GameMode.OBJECTIVES && ObjectiveManager.currentObjective() == ObjectiveManager.COLLECT_ALL_PICKUPS) updateObjectivePickups();

                    if (ParticleEmitterManager.isPaused()) ParticleEmitterManager.pause(false);
                    if (WeatherManager.isPaused()) WeatherManager.pause(false);
                }

                if (USE_WHEATHER) WeatherManager.update();
                MusicHandler.Update();
                world.update();

                if ((mode.MODE == GameMode.OBJECTIVES) && (ObjectiveManager.currentObjective() == ObjectiveManager.SURVIVE)) {
                    ObjectiveManager.updateSpawners();
                }

                //DebugDisplay.update("numpickups", PickupSpawnManager.numPickups.ToString());

                if(intro)
                {

                    if (GAME_CAN_END)
                    {
                        string x = "";
                        if (!endgame && GV_KILLED || mode.terminated() || (mode.MODE == GameMode.OBJECTIVES && (ObjectiveManager.getProgress(ref x))))
                        {
                            if (!preEndGameTimer.IsRunning)
                            {
                                preEndGameTimer.Start();
                                if (!GV_KILLED) MusicHandler.playSound("winwhoosh");
                                GV_KILLED_AT_GAME_END = GV_KILLED;
                                endgame = true;
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
                    HighScoreManager.updateTable(GameStats.Score);
                    WeatherManager.playLightning();
                    ScreenManager.addScreen(new GameOverScreen());
                }
                else
                    ScreenManager.addScreen(new SuccessScreen());
            } else if (mode.MODE == GameMode.OBJECTIVES) {
                if (!GV_KILLED) {
                    ObjectiveManager.calcuateScoreBonus(true);
                    if (ObjectiveManager.currentObjective() != ObjectiveManager.FINAL_OBJECTIVE) {
                        ObjectiveManager.nextObjective();
                        ObjectiveManager.loadObjectivesGame(ScreenManager);
                    } else {
                        HighScoreManager.updateTable(GameStats.Score);
                        ScreenManager.addScreen(new SuccessScreen());
                    }
                } else {
                    WeatherManager.playLightning();
                    HighScoreManager.updateTable(GameStats.Score);
                    ScreenManager.addScreen(new GameOverScreen());
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

        float isintime;
        bool restbrokenthisbeat = false;
        /// <summary>
        /// Updates the Objective Pickups (collision and spin)
        /// </summary>
        private void updateObjectivePickups()
        {
            List<Object> objectivePickups = ScreenManager.game.World.returnObjectSubset<ObjectivePickup>();            

            for (int i = 0; i < objectivePickups.Count; i++)
            {
                ((ObjectivePickup)objectivePickups[i]).collision();
                ((ObjectivePickup)objectivePickups[i]).spinMe();
            }
        }

        /// <summary>
        /// Process all the bad vibes, either move or kill them
        /// </summary>
        /// <returns>the list of dead bad vibes</returns>
        private int processBadVibes()
        {
            int numberKilled = 0;
            int i;

            //bool breakRest = (MusicHandler.getTrack().nextQuarterBeat());
            bool breakRest = false;
            isintime = MusicHandler.getTrack().inTime2(MusicTrack.NoteMode.QUARTER);
            if (isintime > 0.9) {
                if (!restbrokenthisbeat) {
                    breakRest = true;
                    restbrokenthisbeat = true;
                }
            } else {
                restbrokenthisbeat = false;
            }

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
                GameStats.addBV();
            }
        }

        public static GoodVibe getGV()
        {
            return (GoodVibe)ScreenManager.game.World.getObject("Player");
        }

        public static Boss getBoss()
        {
            return (Boss)ScreenManager.game.World.getObject("Boss");
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
                hudTimeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (hudTimeElapsed > HUD_UPDATE_DELAY)
                {
                    hudTimeElapsed -= HUD_UPDATE_DELAY;
                    Hud.saveHud();
                }
                if (USE_MINIMAP && intro)
                {
                    mapTimeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (mapTimeElapsed > MAP_UPDATE_DELAY)
                    {
                        mapTimeElapsed -= MAP_UPDATE_DELAY;
                        Hud.saveMiniMap();
                    }
                }
                Drawing.Draw(gameTime);
            }
        }
    }
}
