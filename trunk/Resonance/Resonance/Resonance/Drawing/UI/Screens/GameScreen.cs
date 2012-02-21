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
using BEPUphysics.Paths.PathFollowing;
using System.IO;
using AnimationLibrary;

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
        public static GameMode mode;
        public static GameStats stats;
        public static MusicHandler musicHandler;

        public static bool GV_KILLED = false;
        public static bool USE_BV_SPAWNER = true;
        public static bool USE_PICKUP_SPAWNER = true;

        GraphicsDeviceManager graphics;

        World world;
        BVSpawnManager bvSpawner;
        public PickupSpawnManager pickupSpawner;

        // Testing variable
        //bool beatTested = false;

        bool isLoaded;

        public GameScreen(ScreenManager scrn)
        {
            isLoaded = false;
            this.ScreenManager = scrn;
            mode = new GameMode(GameMode.TIME_ATTACK);
            stats = new GameStats();
            graphics = Program.game.GraphicsManager;
            Drawing.Init(ScreenManager.Content, graphics);
            musicHandler = new MusicHandler(ScreenManager.Content);

            if (USE_BV_SPAWNER) bvSpawner = new BVSpawnManager();
            if (USE_PICKUP_SPAWNER) pickupSpawner = new PickupSpawnManager();
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
                Drawing.loadContent();
                world = new World();

                //When loading a level via MenuActions the load is done in a separate thread and you get a nice loading screen
                //MenuActions.loadLevel(1);
                Loading.load(delegate { loadLevel(1); }, "Level " + 1);
                Drawing.reset();

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
            string level = "Levels/Level" + i;
            world.readXmlFile(level, ScreenManager.Content);

            GVMotionManager.initialised = false;
            ParticleEmitterManager.initialise();
            WeatherManager.initialise();
            CameraMotionManager.initialise();
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

            if ((pad1Ever && !connected) || pause)
            {
                ScreenManager.addScreen(new PauseMenu());
            }
            else if (debug)
            {
                ScreenManager.addScreen(new DebugMenu());
            }

            //Camera
            CameraMotionManager.update(input);
            //Player One
            GVManager.input(input);
            //Player Two
            DrumManager.input(input);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            DrawableManager.Update(gameTime);

            Drawing.Update(gameTime);

            float health = getGV().healthFraction();
            if (health < 0.1) musicHandler.HeartBeat = true;
            else musicHandler.HeartBeat = false;
            //Update bad vibe positions
            List<string> deadVibes = processBadVibes();
            removeDeadBadVibes(deadVibes);

            //Break rest layers
            if (musicHandler.getTrack().nextQuarterBeat()) breakRestLayers();

            // Update shockwaves
            getGV().updateWaves();

            //Check if combat and freeze range
            getGV().detectCombatAndFreeze();

            //Update pickups
            PickupManager.update();
            //List<Pickup> pickups = world.returnPickups();
            //world.updatePickups(pickups);
            //PickupManager.updateTimeRemaining();

            world.update();

            musicHandler.Update();

            WeatherManager.update();

            //Update Spawners
            if (USE_BV_SPAWNER) BVSpawnManager.update();
            if (USE_PICKUP_SPAWNER) pickupSpawner.update();

            if (GV_KILLED || mode.terminated())
            {
                endGame();
            }

            //DebugDisplay.update("In time", musicHandler.getTrack().inTime().ToString());

            // Recoded in Hud.cs
            /*if (musicHandler.getTrack().inTime() > 0.8f) {
                if (!beatTested) {
                    //musicHandler.playSound("chink");
                    DebugDisplay.update("Hit", "Now!");
                    beatTested = true;

                    Game.getGV().showBeat();
                }
            } else {
                if (beatTested) {
                    beatTested = false;
                    //musicHandler.playSound("chink");
                    DebugDisplay.update("Hit", "Not now!");
                }
            }*/
            //}
        }

        // Called when game finished (won or lost).
        private void endGame()
        {
            String r;
            int finalScore;
            if (GV_KILLED) r = "GV Killed."; else r = "Game won!";
            finalScore = mode.finaliseScore(GV_KILLED, stats.Score);
            DebugDisplay.update("Game Over! State", r);
            DebugDisplay.update("Final Score", finalScore.ToString());
        }

        private void updateStats()
        {
        }

        /// <summary>
        /// Process all the bad vibes, either move or kill them
        /// </summary>
        /// <returns>the list of dead bad vibes</returns>
        private List<string> processBadVibes()
        {
            List<string> deadVibes = new List<string>();
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();
            foreach (BadVibe bv in bvs)
            {
                if (bv.Status == BadVibe.State.DEAD)
                {
                    if (bv.getAnimationCounter() <= 0)
                    {
                        deadVibes.Add(bv.returnIdentifier());
                    }
                    bv.decrementAnimationCounter();
                }
                else if (bv.Status != BadVibe.State.DEAD)
                {
                    bv.Move();
                }
            }
            return deadVibes;
        }

        /// <summary>
        /// Remove all dead bad vibes from the game
        /// </summary>
        /// <param name="deadVibes">the list of dead bad vibes</param>
        private void removeDeadBadVibes(List<string> deadVibes)
        {
            for (int i = 0; i < deadVibes.Count; i++)
            {
                if (USE_BV_SPAWNER) BVSpawnManager.vibeDied((BadVibe)World.getObject(deadVibes[i]));
                World.removeObject(World.getObject(deadVibes[i]));
                stats.addBV();
            }
        }

        /// <summary>
        /// Break the rest layer on all bad vibes if present
        /// </summary>
        private void breakRestLayers()
        {
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();
            foreach (BadVibe bv in bvs)
            {
                if (bv.Status != BadVibe.State.DEAD)
                {
                    bv.damage(Shockwave.REST);
                }
            }
        }

        public static GoodVibe getGV()
        {
            return (GoodVibe)ScreenManager.game.World.getObject("Player");
        }

        public World World
        {
            get
            {
                return world;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            //Set the ambient light of the 3D scene, default is new Vector3(0.1f, 0.1f, 0.1f)
            //Drawing.setAmbientLight(new Vector3(2f, 2f, 2f));

            graphics.GraphicsDevice.Clear(Color.Black);
            if (Drawing.requestRender)
            {
                Drawing.drawReflection();
                graphics.GraphicsDevice.Clear(Color.Black);
                DrawableManager.Draw(gameTime);
            }
            Drawing.drawGame();
            graphics.GraphicsDevice.Clear(Color.Black);
            DrawableManager.Draw(gameTime);
            Drawing.Draw(gameTime);
        }

    }
}
