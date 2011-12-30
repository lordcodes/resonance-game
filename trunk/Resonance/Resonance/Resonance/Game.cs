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

        public static int DIFFICULTY = EASY;

        GraphicsDeviceManager graphics;
        MusicHandler musicHandler;

        //Previous Input States
        GamePadState oldPadState1;
        GamePadState oldPadState2;
        KeyboardState oldKeyState;

        World world;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawing.Init(Content, graphics);
            musicHandler = new MusicHandler(Content);
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

            /*int startTime = Environment.TickCount;
            PathFind pathFind = new PathFind();
            List<Vector3> path = pathFind.find(new Vector3(-19, 0.4f, -19), new Vector3(19, 0.4f, 19));
            Console.WriteLine("Path took: " + (Environment.TickCount - startTime));
            if (path != null)
            {
                DebugDisplay.update("Path", "" + path[0] + " " + path[path.Count-1]);
            }
            else
            {
                DebugDisplay.update("Path", "Not found");
            }*/
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
                //DebugDisplay.update("GV Position", "" + ((DynamicObject)world.getObject("Player")).Body.Position);
                keyInput();
                if (!UI.Paused)
                {
                    //Update bad vibe position
                    moveBadVibes();

                    //Break rest layers
                    if (musicHandler.getTrack().nextQuarterBeat())
                    {
                        breakRestLayers();
                    }

                    // Update shockwaves
                    ((GoodVibe)world.getObject("Player")).updateWaves();

                    ((GoodVibe)world.getObject("Player")).checkDistance();

                    world.update();
                    base.Update(gameTime);
                    musicHandler.Update();
                    removeDeadBadVibes();
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
                    //if (paused) musicHandler.getTrack().pauseTrack();
                    //if (!paused) musicHandler.getTrack().playTrack();
                }
            }

            if (UI.Paused)
            {
                menuControls(keyboardState, playerOne, playerTwo);
            }
            else
            {
                //Player One
                playerOnePresses(playerOne);
                //Player Two
                playerTwoPresses(playerTwo);

                //PC Testing Controls
                pcTestingControls(keyboardState);


           if ((keyboardState.IsKeyDown(Keys.Q) && !oldKeyState.IsKeyDown(Keys.Q)) || (playerOne.Buttons.LeftShoulder == ButtonState.Pressed && oldPadState1.Buttons.LeftShoulder != ButtonState.Pressed))
                {
                    Drawing.addWave(((DynamicObject)World.getObject("Player")).Body.Position);
                }
            }

            //Cache the previous key state.
            oldPadState1 = playerOne;
            oldPadState2 = playerTwo;
            oldKeyState = keyboardState;
        }

        private void pcTestingControls(KeyboardState keyboardState)
        {
            //PC Testing Controls
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                musicHandler.getTrack().playTrack();
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                musicHandler.getTrack().stopTrack();
            }
            if (keyboardState.IsKeyDown(Keys.P))
            {
                musicHandler.getTrack().pauseTrack();
            }
            if (keyboardState.IsKeyDown(Keys.M))
            {
                MiniMap.large = true;
            } else {
                MiniMap.large = false;
            }
            if (keyboardState.IsKeyDown(Keys.Z) && !oldKeyState.IsKeyDown(Keys.Z))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.GREEN);
            }
            if (keyboardState.IsKeyDown(Keys.X) && !oldKeyState.IsKeyDown(Keys.X))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.YELLOW);
            }
            if (keyboardState.IsKeyDown(Keys.C) && !oldKeyState.IsKeyDown(Keys.C))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.BLUE);
            }
            if (keyboardState.IsKeyDown(Keys.V) && !oldKeyState.IsKeyDown(Keys.V))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.RED);
            }
            if (keyboardState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
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
        /// This handles player one button presses
        /// </summary>
        private void playerOnePresses(GamePadState playerOne)
        {
            if (playerOne.Buttons.Start == ButtonState.Pressed)
            {
                musicHandler.getTrack().playTrack();
            }
            if (playerOne.Buttons.A == ButtonState.Pressed)
            {
                musicHandler.getTrack().stopTrack();
            }
            if (playerOne.Buttons.B == ButtonState.Pressed)
            {
                musicHandler.getTrack().pauseTrack();
            }
            if (playerOne.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                MiniMap.large = true;
            } else {
                MiniMap.large = false;
            }

            //Update graphics
            UpdateGoodVibePosition();
        }

        /// <summary>
        /// This handles player two button presses
        /// </summary>
        private void playerTwoPresses(GamePadState playerTwo)
        {
            if (playerTwo.Buttons.A == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.A))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.GREEN);
            }
            if (playerTwo.Buttons.Y == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.Y))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.YELLOW);
            }
            if (playerTwo.Buttons.X == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.X))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.BLUE);
            }
            if (playerTwo.Buttons.B == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.B))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.RED);
            }
            /*if (playerTwo.Buttons.RightStick == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.RightStick))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            if (playerTwo.Buttons.LeftStick == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.LeftStick))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            */if (playerTwo.Buttons.LeftShoulder == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.LeftShoulder))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
            if (playerTwo.Buttons.RightShoulder == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.RightShoulder))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.CYMBAL);
            }
        }

        public World World
        {
            get
            {
                return world;
            }
        }

        /// <summary>
        /// This handles basic user input to move the good vibe around the world, this is temporary 
        /// and will eventualy feed into the World object rather than directly to the Drawing
        /// </summary>
        private void UpdateGoodVibePosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            float initial = ((DynamicObject)world.getObject("Player")).Body.AngularVelocity.Y;
            bool upPressed   = keyboardState.IsKeyDown(Keys.Up) || (currentState.DPad.Up == ButtonState.Pressed);
            bool downPressed = keyboardState.IsKeyDown(Keys.Down) || (currentState.DPad.Down == ButtonState.Pressed);
            
            if (currentState.IsConnected == false)
            {
                if (keyboardState.IsKeyDown(Keys.Left) || (currentState.DPad.Left == ButtonState.Pressed))
                {
                    if (!downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_ANTI);
                    }
                    else
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_CLOCK);
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Right) || (currentState.DPad.Right == ButtonState.Pressed))
                {
                    if (!downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_CLOCK);
                    }
                    else
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_ANTI);
                    }
                }

                if (upPressed ^ downPressed)
                {
                    if (upPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_FORWARD);
                    }
                    if (downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_BACKWARD);
                    }
                }
            }
            else
            {
                float x = currentState.ThumbSticks.Left.X;
                float y = currentState.ThumbSticks.Left.Y;
                float camerax = currentState.ThumbSticks.Right.X;
                float cameray = currentState.ThumbSticks.Right.Y;
                float height  = currentState.Triggers.Left;
               
                if (height > 0 && ((DynamicObject)world.getObject("Player")).Body.AngularVelocity.Y == initial)
                {
                    
                    ((DynamicObject)(world.getObject("Player"))).jump(0.2f);
                    
                }
                if (x == 0 && y > 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_FORWARD);
                }
                if (x == 0 && y < 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_BACKWARD);
                    
                }
                
                if (x < 0 && y == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_LEFT);
                  
                }
                if (x < 0 && y > 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_LEFT);
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_FORWARD);
                }
                if (x < 0 && y < 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_LEFT);
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_BACKWARD);
                }
                if (x > 0 && y < 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_RIGHT);
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_BACKWARD);
                }
                if (x > 0 && y > 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_RIGHT);
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_FORWARD);
                }
                if (x > 0 && y == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(DynamicObject.MOVE_RIGHT);
                }

                if (camerax == -1 && cameray == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_ANTI);
                }

                if (camerax == 1 && cameray == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).rotate(DynamicObject.ROTATE_CLOCK);
                }
                
            }
            Drawing.UpdateCamera((GoodVibe)world.getObject("Player"));
        }

        /// <summary>
        /// Find all the dead bad vibes and remove them from the game
        /// </summary>
        private void removeDeadBadVibes()
        {
            List<string> deadVibes = new List<string>();

            foreach (KeyValuePair<string, Object> pair in World.returnObjects())
            {
                if(pair.Value is BadVibe) {
                    BadVibe vibe = (BadVibe)pair.Value;
                    if (vibe.Dead == true) deadVibes.Add(pair.Key);
                }
            }
            for (int i = 0; i < deadVibes.Count; i++)
            {
                World.removeObject(World.getObject(deadVibes[i]));
                musicHandler.playSound("beast_dying");
            }
        }

        /// <summary>
        /// Find all the living bad vibes and move them
        /// </summary>
        private void moveBadVibes()
        {
            foreach (KeyValuePair<string, Object> pair in World.returnObjects())
            {
                if (pair.Value is BadVibe)
                {
                    BadVibe vibe = (BadVibe)pair.Value;
                    if (vibe.Dead == false)
                    {
                        vibe.Move();
                    }
                }
            }
        }

        /// <summary>
        /// Find all the living bad vibes and move them
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            Drawing.Draw(gameTime);
        }

    }
}
