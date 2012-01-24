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
    class Game : Microsoft.Xna.Framework.Game
    {

        public const int BEGINNER = 0;
        public const int EASY     = 1;
        public const int MEDIUM   = 2;
        public const int HARD     = 3;
        public const int EXPERT   = 4;
        public const int INSANE   = 5;

        public static int DIFFICULTY = BEGINNER;

        public static bool USE_SPAWNER = false;

        GraphicsDeviceManager graphics;
        MusicHandler musicHandler;

        //Previous Input States
        GamePadState oldPadState1;
        GamePadState oldPadState2;
        KeyboardState oldKeyState;
        DrumManager drumManager;

        World world;
        BVSpawnManager spawner;
        public AnimationPlayer animationPlayer;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawing.Init(Content, graphics);
            musicHandler = new MusicHandler(Content);
            if(USE_SPAWNER) spawner = new BVSpawnManager();
            UI.init();

            //Allows you to set the resolution of the game (not tested on Xbox yet)
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            long start = DateTime.Now.Ticks;
            BadVibe.initialiseBank();

            oldPadState1 = GamePad.GetState(PlayerIndex.One);
            oldPadState2 = GamePad.GetState(PlayerIndex.Two);
            oldKeyState = Keyboard.GetState();

            Drawing.loadContent();
            world = new World();

            //When loading a level via MenuActions the load is done in a separate thread and you get a nice loading screen
            MenuActions.loadLevel(1);

            double loadTime = (double)(DateTime.Now.Ticks - start) / 10000000;
            DebugDisplay.update("LOAD TIME(S)", loadTime.ToString());
        }

        /// <summary>
        /// This method is used to load a level. MenuAction calls this method from a separate thread 
        /// and therefore we can have an animated loading screen while you wait.
        /// </summary>
        /// <param name="i">Int number of the level, taken from the level name, i.e Level1.xml</param>
        public void loadLevel(int i)
        {
            string level = "Levels/Level"+i;
            world.readXmlFile(level, Content);
            GVMotionManager.initialised = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Drawing.Update(gameTime);

            if (!Loading.IsLoading)
            {
                keyInput();
                if (!UI.Paused)
                {
                    //Update bad vibe position
                    List<string> deadVibes = processBadVibes();

                    //Break rest layers
                    if (musicHandler.getTrack().nextQuarterBeat())
                    {
                        breakRestLayers();
                    }

                    // Update shockwaves
                    getGV().updateWaves();
                    getGV().detectCombatAndFreeze();
                    

                    world.update();
                    
                    musicHandler.Update();
                    removeDeadBadVibes(deadVibes);
                    if(USE_SPAWNER) spawner.update();

                    animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
                    //DrumManager.increaseHealth();
                    base.Update(gameTime);
                    DebugDisplay.update("COMBAT STATUS:", getGV().InCombat.ToString());
                }
            }
        }

        private void keyInput()
        {
            GamePadState playerOne = GamePad.GetState(PlayerIndex.One);
            GamePadState playerTwo = GamePad.GetState(PlayerIndex.Two);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) || playerOne.Buttons.Start == ButtonState.Pressed || playerTwo.Buttons.Start == ButtonState.Pressed)
            {
                if (!oldKeyState.IsKeyDown(Keys.Escape) && oldPadState1.Buttons.Start != ButtonState.Pressed && oldPadState2.Buttons.Start != ButtonState.Pressed)
                {
                    if (UI.Paused) UI.play();
                    else UI.pause();
                }
            }

            if (UI.Paused)
            {
                
                
                menuControls(keyboardState, playerOne, playerTwo);
            }
            else
            {
                CameraMotionManager.update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
                //Player One
                //playerOnePresses(playerOne);
                GVManager.inputs(playerOne,musicHandler,keyboardState);
                //Player Two
                DrumManager.Input(playerTwo, oldPadState2, musicHandler, world,keyboardState,oldKeyState);
                //PC Testing Controls
                 // pcTestingControls(keyboardState);
              //  DrumManager.pcInput(keyboardState,musicHandler,oldKeyState,world);


           if ((keyboardState.IsKeyDown(Keys.Q) && !oldKeyState.IsKeyDown(Keys.Q)) || (playerOne.Buttons.LeftShoulder == ButtonState.Pressed && oldPadState1.Buttons.LeftShoulder != ButtonState.Pressed))
                {
                    Drawing.DoDisp = true;
                    Drawing.addWave(((DynamicObject)World.getObject("Player")).Body.Position);
                }
            }

            //Cache the previous key state.
            oldPadState1 = playerOne;
            oldPadState2 = playerTwo;
            oldKeyState = keyboardState;
        }

        private void menuControls(KeyboardState keyboardState, GamePadState playerOne, GamePadState playerTwo)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || playerOne.DPad.Up == ButtonState.Pressed || playerTwo.DPad.Up == ButtonState.Pressed)
            {
                if (!oldKeyState.IsKeyDown(Keys.Up) && oldPadState1.DPad.Up != ButtonState.Pressed && oldPadState2.DPad.Up != ButtonState.Pressed)
                {
                    UI.moveUp();
                }
            }

            if (keyboardState.IsKeyDown(Keys.Down) || playerOne.DPad.Down == ButtonState.Pressed || playerTwo.DPad.Down == ButtonState.Pressed)
            {
                if (!oldKeyState.IsKeyDown(Keys.Down) && oldPadState1.DPad.Down != ButtonState.Pressed && oldPadState2.DPad.Down != ButtonState.Pressed)
                {
                    UI.moveDown();
                }
            }

            if (keyboardState.IsKeyDown(Keys.Enter) || playerOne.Buttons.A == ButtonState.Pressed || playerTwo.Buttons.A == ButtonState.Pressed)
            {
                if (!oldKeyState.IsKeyDown(Keys.Enter) && oldPadState1.Buttons.A != ButtonState.Pressed && oldPadState2.Buttons.A != ButtonState.Pressed)
                {
                    UI.select();
                }
            }
        }

        /// <summary>
        /// Process all the bad vibes, either move or kill them
        /// </summary>
        /// <returns>the list of dead bad vibes</returns>
        private List<string> processBadVibes()
        {
            List<string> deadVibes = new List<string>();

            foreach (KeyValuePair<string, Object> pair in World.returnObjects())
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;
                    if (vibe.Dead)
                    {
                        deadVibes.Add(pair.Key);
                    }
                    else if (!vibe.Dead)
                    {
                        vibe.Move();
                    }
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
                World.removeObject(World.getObject(deadVibes[i]));
                musicHandler.playSound("beast_dying");
                if(USE_SPAWNER) spawner.vibeDied();
            }
        }

        /// <summary>
        /// Break the rest layer on all bad vibes if present
        /// </summary>
        private void breakRestLayers()
        {
            foreach (KeyValuePair<string, Object> pair in World.returnObjects())
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;
                    if (vibe.Dead == false)
                    {
                        vibe.damage(Shockwave.REST);
                    }
                }
            }
        }

        public static double getDistance(Vector3 from, Vector3 to)
        {
            double xDiff = Math.Abs(from.X - to.X);
            double yDiff = Math.Abs(from.Y - to.Y);
            double zDiff = Math.Abs(from.Z - to.Z);
            double distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));
            return distance;
        }

        public static GoodVibe getGV()
        {
            return (GoodVibe)Program.game.World.getObject("Player");
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
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            Drawing.Draw(gameTime);
        }

    }
}
