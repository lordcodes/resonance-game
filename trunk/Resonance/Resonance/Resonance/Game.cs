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

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawer = new Drawing(Content, graphics);
            musicHandler = new MusicHandler(Content);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Space) || (currentState.Buttons.Start == ButtonState.Pressed))
            {
                musicHandler.playTrack();
            }
            if (keyboardState.IsKeyDown(Keys.S) || (currentState.Buttons.X == ButtonState.Pressed))
            {
                musicHandler.stopTrack();
            }
            if (keyboardState.IsKeyDown(Keys.P) || (currentState.Buttons.A == ButtonState.Pressed))
            {
                musicHandler.pauseTrack();
            }
            UpdateGoodVibePosition();
            Drawer.Update(goodVibePos);
            base.Update(gameTime);
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
