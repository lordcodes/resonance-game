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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        float goodVibeRotation = 0;
        Vector4 goodVibePos;
        MusicHandler musicHandler;

#if XBOX
        GamePadState oldPadState1;
        GamePadState oldPadState2;
#else
        KeyboardState oldKeyState;
#endif

        //Hello world
        World world;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawing.Init(Content, graphics);
            musicHandler = new MusicHandler(Content);

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
#if XBOX360
            oldPadState1 = GamePad.GetState(PlayerIndex.One);
            oldPadState2 = GamePad.GetState(PlayerIndex.Two);
#else
            oldKeyState = Keyboard.GetState();
#endif
            Drawing.loadContent();
            goodVibePos = new Vector4(0, 0.65f, 6f, (float)(Math.PI * 0.25));
            world = new World(this);
            StaticObject ground = new StaticObject(GameModels.GROUND, "Ground", this, Vector3.Zero);
            GoodVibe player = new GoodVibe(GameModels.GOOD_VIBE, "Player", this, new Vector3(0, 0.5f, 6));
            StaticObject tree = new StaticObject(GameModels.TREE, "Tree1", this, new Vector3(0,0,-0.1f));
            StaticObject mush = new StaticObject(GameModels.MUSHROOM, "Mushroom1", this, new Vector3(3, 3, 3));
            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BV0", this, new Vector3(-3, 0.5f, 3));
            world.addObject(ground);
            world.addObject(player);
            world.addObject(tree);
            world.addObject(mush);
            world.addObject(bv);
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
#if XBOX360
            GamePadState playerOne = GamePad.GetState(PlayerIndex.One);
            GamePadState playerTwo = GamePad.GetState(PlayerIndex.Two);
            // Allows the game to exit
            if (playerOne.Buttons.Back == ButtonState.Pressed || playerTwo.Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //Player One
            playerOnePresses(playerOne);
            //Player Two
            playerTwoPresses(playerTwo);
#else
            KeyboardState keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            //Play sound effects
            if (keyboardState.IsKeyDown(Keys.D1)) musicHandler.getSound().playSound(0);
            if (keyboardState.IsKeyDown(Keys.D2)) musicHandler.getSound().playSound(1);
            if (keyboardState.IsKeyDown(Keys.D3)) musicHandler.getSound().playSound(2);
            if (keyboardState.IsKeyDown(Keys.D4)) musicHandler.getSound().playSound(3);
            if (keyboardState.IsKeyDown(Keys.D5)) musicHandler.getSound().playSound(4);
            if (keyboardState.IsKeyDown(Keys.D6)) musicHandler.getSound().playSound(5);
            if (keyboardState.IsKeyDown(Keys.D7)) musicHandler.getSound().playSound(6);
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
            if (keyboardState.IsKeyDown(Keys.H) && !oldKeyState.IsKeyDown(Keys.H))
            {
                musicHandler.getTrack().inTime();
            }
#endif


            //Update graphics
            UpdateGoodVibePosition();

            // Update bad vibe position
          ((BadVibe)world.getObject("BV0")).Move();

            Drawing.Update(goodVibePos);
            world.update();
            base.Update(gameTime);

            //Cache the previous key state. As found this method is run so quickly that pressing key once
            //caused it to do the beat method inTime() about 4 times and that messed up the detecting
            //if you were in time to the music.
#if XBOX360
            oldPadState1 = playerOne;
            oldPadState2 = playerTwo;
#else
            oldKeyState = keyboardState;
#endif

        }

#if XBOX360
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
#endif

      /*  public World World
        {
            get
            {
                return world;
            }
        }*/

        

        /// <summary>
        /// This handles basic user input to move the good vibe around the world, this is temporary 
        /// and will eventualy feed into the World object rather than directly to the Drawing
        /// </summary>
        private void UpdateGoodVibePosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            float forwardSpeed = 0.05f;
            float rotateSpeed = 0.05f;

            if (keyboardState.IsKeyDown(Keys.Left) || (currentState.DPad.Left == ButtonState.Pressed))
            {
                goodVibeRotation += rotateSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Right) || (currentState.DPad.Right == ButtonState.Pressed))
            {
                goodVibeRotation -= rotateSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Up) || (currentState.DPad.Up == ButtonState.Pressed))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(goodVibeRotation);
                Vector3 v = new Vector3(0, 0, -forwardSpeed);
                v = Vector3.Transform(v, forwardMovement);
                goodVibePos.Z += v.Z;
                goodVibePos.X += v.X;
            }

            if (keyboardState.IsKeyDown(Keys.Down) || (currentState.DPad.Down == ButtonState.Pressed))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(goodVibeRotation);
                Vector3 v = new Vector3(0, 0, forwardSpeed);
                v = Vector3.Transform(v, forwardMovement);
                goodVibePos.Z += v.Z;
                goodVibePos.X += v.X;
            }

            goodVibePos.W = goodVibeRotation;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Drawing.Draw();
            base.Draw(gameTime);
        }
    }
}