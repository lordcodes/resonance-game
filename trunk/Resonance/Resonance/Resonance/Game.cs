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

namespace Resonance
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Drawing Drawer;
        float goodVibeRotation = 0;
        Vector4 goodVibePos;
        MusicHandler musicHandler;

        KeyboardState oldKeyState;
        GamePadState oldPadState;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawer = new Drawing(Content, graphics);
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
            Drawer.loadContent();
            goodVibePos = new Vector4(0, 0.65f, 6f, (float)(Math.PI * 0.25));
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
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState playerOne = GamePad.GetState(PlayerIndex.One);
            GamePadState playerTwo = GamePad.GetState(PlayerIndex.Two);

            // Allows the game to exit
            if (playerOne.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            //Player One
            playerOnePresses(keyboardState, playerOne);


            //Player Two
            playerTwoPresses(keyboardState, playerTwo);

            //Play sound effects
            if (keyboardState.IsKeyDown(Keys.D1)) musicHandler.getSound().playSound(0);
            if (keyboardState.IsKeyDown(Keys.D2)) musicHandler.getSound().playSound(1);
            if (keyboardState.IsKeyDown(Keys.D3)) musicHandler.getSound().playSound(2);
            if (keyboardState.IsKeyDown(Keys.D4)) musicHandler.getSound().playSound(3);
            if (keyboardState.IsKeyDown(Keys.D5)) musicHandler.getSound().playSound(4);
            if (keyboardState.IsKeyDown(Keys.D6)) musicHandler.getSound().playSound(5);
            if (keyboardState.IsKeyDown(Keys.D7)) musicHandler.getSound().playSound(6);

            //Update graphics
            UpdateGoodVibePosition();
            Drawer.Update(goodVibePos);
            base.Update(gameTime);

            //Cache the previous key state. As found this method is run so quickly that pressing key once
            //caused it to do the beat method inTime() about 4 times and that messed up the detecting
            //if you were in time to the music.
            oldKeyState = keyboardState;
            oldPadState = playerTwo;
        }

        /// <summary>
        /// This handles player one button presses
        /// </summary>
        private void playerOnePresses(KeyboardState keyboardState, GamePadState playerOne)
        {
            if (keyboardState.IsKeyDown(Keys.Space) || (playerOne.Buttons.Start == ButtonState.Pressed))
            {
                musicHandler.getTrack().playTrack();
            }
            if (keyboardState.IsKeyDown(Keys.S) || (playerOne.Buttons.X == ButtonState.Pressed))
            {
                musicHandler.getTrack().stopTrack();
            }
            if (keyboardState.IsKeyDown(Keys.P) || (playerOne.Buttons.A == ButtonState.Pressed))
            {
                musicHandler.getTrack().pauseTrack();
            }
        }

        /// <summary>
        /// This handles player two button presses
        /// </summary>
        private void playerTwoPresses(KeyboardState keyboardState, GamePadState playerTwo)
        {
            if (keyboardState.IsKeyDown(Keys.H) && !oldKeyState.IsKeyDown(Keys.H) || playerTwo.Buttons.A == ButtonState.Pressed && !oldPadState.IsButtonDown(Buttons.A))
            {
                musicHandler.getTrack().inTime();
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
            Drawer.Draw();
            base.Draw(gameTime);
        }
    }
}
