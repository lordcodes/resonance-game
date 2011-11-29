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
namespace Resonance
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        MusicHandler musicHandler;

        //Input states
        GamePadState oldPadState1;
        GamePadState oldPadState2;
        KeyboardState oldKeyState;

        World world;
        Vector4 goodVibePos;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawing.Init(Content, graphics);
            musicHandler = new MusicHandler(Content);

            //Allows you to set the resolution of the game (not tested on Xbox yet)
            /*IsMouseVisible = false;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Window.AllowUserResizing = true;*/
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
            oldPadState1 = GamePad.GetState(PlayerIndex.One);
            oldPadState2 = GamePad.GetState(PlayerIndex.Two);
            oldKeyState = Keyboard.GetState();

            Drawing.loadContent();
            world = new World(this);

            //to test the level editor uncomment the next two lines
            StaticObject ground = null ;
            StaticObject tree = null;
            StaticObject mush = null;
            GoodVibe player = null;
            BadVibe bv = null;
            StoredObjects obj = Content.Load<StoredObjects>("Levels/Level1");
            for (int i = 0; i < 5; i++)
            {
                if (obj.list[i].type.Equals("Ground") == true)
                {
                    ground = new StaticObject(GameModels.GROUND, "Ground", this, Vector3.Zero);
                    world.addObject(ground);
                }
                if (obj.list[i].type.Equals("Good_vibe") == true)
                {
                    player = new GoodVibe(GameModels.GOOD_VIBE,"Player",this,new Vector3(obj.list[i].xWorldCoord,obj.list[i].yWorldCoord,obj.list[i].zWorldCoord));
                    world.addObject(player);
                }
                if (obj.list[i].type.Equals("Tree") == true)
                {
                    tree = new StaticObject(GameModels.TREE, "Tree1", this, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    world.addObject(tree);
                }
                if (obj.list[i].type.Equals("Mushroom") == true)
                {
                    mush = new StaticObject(GameModels.MUSHROOM, "Mushroom1", this, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    world.addObject(mush);
                }
                if (obj.list[i].type.Equals("Bad_vibe") == true)
                {
                    bv = new BadVibe(GameModels.BAD_VIBE, "BV0", this, new Vector3(obj.list[i].xWorldCoord, obj.list[i].yWorldCoord, obj.list[i].zWorldCoord));
                    world.addObject(bv);
                }

            }

            //Not needed now that we have loading from level file
            //StaticObject ground = new StaticObject(GameModels.GROUND, "Ground", this, Vector3.Zero);
            //GoodVibe player = new GoodVibe(GameModels.GOOD_VIBE, "Player", this, new Vector3(0, 0.65f, 6f));
            //GoodVibe player = new GoodVibe(GameModels.MUSHROOM, "Player", this, new Vector3(0, 5f, 6f));
            //StaticObject tree = new StaticObject(GameModels.TREE, "Tree1", this, new Vector3(0,0,-0.1f));
            //StaticObject mush = new StaticObject(GameModels.MUSHROOM, "Mushroom1", this, new Vector3(3, 3, 3));
            //BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BV0", this, new Vector3(-3, 0.65f, 3));
            //world.addObject(tree);
            //world.addObject(mush);
            //world.addObject(bv);

            goodVibePos = new Vector4(0, 0.65f, 6f, (float)(Math.PI * 0.25));
            
            //StoredObjects obj = Content.Load<StoredObjects>("Level1");
            //world.addObject(bv2);
            //world.addObject(bv3);
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
            GamePadState playerOne = GamePad.GetState(PlayerIndex.One);
            GamePadState playerTwo = GamePad.GetState(PlayerIndex.Two);


            //XBOX Controls
            if (playerOne.Buttons.Back == ButtonState.Pressed || playerTwo.Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
            //Player One
            playerOnePresses(playerOne);
            //Player Two
            playerTwoPresses(playerTwo);

            //PC Testing Controls
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                musicHandler.getTrack().playTrack();
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                musicHandler.getTrack().stopTrack();
                musicHandler.playCollision();
            }
            if (keyboardState.IsKeyDown(Keys.P))
            {
                musicHandler.getTrack().pauseTrack();
            }
            if (keyboardState.IsKeyDown(Keys.H) && !oldKeyState.IsKeyDown(Keys.H))
            {
                musicHandler.getTrack().inTime();
                ((GoodVibe)(world.getObject("Player"))).createShockwave(Shockwave.BLUE);
            }

            //Update graphics
            UpdateGoodVibePosition();

            //Update bad vibe position
            ((BadVibe)world.getObject("BV0")).moveTowardsGoodVibe();

            world.update();
            base.Update(gameTime);
            musicHandler.Update();

            //Cache the previous key state.
            oldPadState1 = playerOne;
            oldPadState2 = playerTwo;
            oldKeyState = keyboardState;
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
            if (playerOne.Buttons.X == ButtonState.Pressed)
            {
                musicHandler.getTrack().stopTrack();
            }
            if (playerOne.Buttons.B == ButtonState.Pressed)
            {
                musicHandler.getTrack().pauseTrack();
            }
        }

        /// <summary>
        /// This handles player two button presses
        /// </summary>
        private void playerTwoPresses(GamePadState playerTwo)
        {
            if (playerTwo.Buttons.A == ButtonState.Pressed && !oldPadState2.IsButtonDown(Buttons.A))
            {
                musicHandler.getTrack().inTime();
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
            
            bool upPressed   = keyboardState.IsKeyDown(Keys.Up) || (currentState.DPad.Up == ButtonState.Pressed);
            bool downPressed = keyboardState.IsKeyDown(Keys.Down) || (currentState.DPad.Down == ButtonState.Pressed);
            
            float forwardSpeed = 0.25f;
            float rotateSpeed = 0.25f;
            if (currentState.IsConnected == false)
            {
                if (keyboardState.IsKeyDown(Keys.Left) || (currentState.DPad.Left == ButtonState.Pressed))
                {
                    if (!downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(rotateSpeed);
                    }
                    else
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(-rotateSpeed);
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Right) || (currentState.DPad.Right == ButtonState.Pressed))
                {
                    if (!downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(-rotateSpeed);
                    }
                    else
                    {
                        ((DynamicObject)(world.getObject("Player"))).rotate(rotateSpeed);
                    }
                }

                if (upPressed ^ downPressed)
                {
                    if (upPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).move(-forwardSpeed);
                    }
                    if (downPressed)
                    {
                        ((DynamicObject)(world.getObject("Player"))).move(forwardSpeed);
                    }
                    goodVibePos.X = -10f;
                    goodVibePos.Y = 2f;
                    goodVibePos.Z = -10f;
                }
            }
            else
            {
                float x = currentState.ThumbSticks.Left.X;
                float y = currentState.ThumbSticks.Left.Y;
                float camerax = currentState.ThumbSticks.Right.X;
                float cameray = currentState.ThumbSticks.Right.Y;

                if (x == 0 && y > 0)
                {
                   ((DynamicObject)(world.getObject("Player"))).move(-forwardSpeed);
                   goodVibePos.X = -10f;
                   goodVibePos.Y = 2f;
                   goodVibePos.Z = -10f;
                }
                if (x == 0 && y < 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).move(forwardSpeed);
                    
                }
                
                if (x < 0 && y == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).moveLeft(forwardSpeed);
                  
                }
                if (x > 0 && y == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).moveRight(-rotateSpeed);
                }

                if (camerax == -1 && cameray == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).rotate(rotateSpeed);
                }

                if (camerax == 1 && cameray == 0)
                {
                    ((DynamicObject)(world.getObject("Player"))).rotate(-rotateSpeed);
                }
                
            }
            Vector3 pos = ((DynamicObject)(world.getObject("Player"))).Body.Position;
            Drawing.UpdateCamera((GoodVibe)world.getObject("Player"));
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
