using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class CreditsScreen : Screen
    {
        List<Texture2D> images;
        List<string> names;
        SpriteFont font;

        Vector2 lPos;
        Vector2 lPosImage;
        Vector2 rPos;
        Vector2 rPosImage;
        Vector2 textOrigin;

        private float timeElapsed;
        private int frame;
        float fadeValue;

        private static float FRAME_DURATION = 3000f;
        private static float FADE_DURATION = 750f;
        private static float FADE_INC = 1f / FADE_DURATION;

        private const int FADEIN = 0;
        private const int DISPLAY = 1;
        private const int FADEOUT = 2;

        private int state;

        public CreditsScreen()
            : base()
        {
            images = new List<Texture2D>(8);
            names = new List<string>(8);
            names.Add("Michael Jones");
            names.Add("Andrew Lord");
            names.Add("Mihai Nemes");
            names.Add("Thomas Pickering");
            names.Add("Alex Sheppard");
            names.Add("Philip Tattersall");
            names.Add("Geoffrey Birch");
            names.Add("Paul Keast");

            timeElapsed = 0;
            fadeValue = 0.0f;
            frame = 0;
            state = 0;

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

            lPos = new Vector2(screenSize.X / 2, screenSize.Y);
            lPosImage = new Vector2(40, screenSize.Y - 250);
            rPos = new Vector2(screenSize.X * 1.5f, screenSize.Y);
            rPosImage = new Vector2((screenSize.X * 2f) - 752, screenSize.Y - 250);
            textOrigin = Vector2.Zero;
        }

        public override void LoadContent() 
        {
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            font = ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MenuFont");
        }

        public override void Update(GameTime gameTime)
        {
            float frameTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            timeElapsed += frameTime;


            fadeValue = Math.Min(1f, fadeValue + FADE_INC);

            switch (state)
            {
                case FADEIN:
                    {
                        fadeValue = Math.Min(1f, fadeValue + (FADE_INC * frameTime));
                        if (timeElapsed > FADE_DURATION)
                        {
                            timeElapsed -= FADE_DURATION;
                            state = DISPLAY;
                        }
                        break;
                    }
                case DISPLAY:
                    {
                        if (timeElapsed > FRAME_DURATION)
                        {
                            timeElapsed -= FRAME_DURATION;
                            state = FADEOUT;
                            fadeValue = 1f;
                        }
                        break;
                    }
                case FADEOUT:
                    {
                        fadeValue = Math.Max(0f, fadeValue - (FADE_INC * frameTime));
                        if (timeElapsed > FADE_DURATION)
                        {
                            timeElapsed -= FADE_DURATION;
                            state = FADEIN;
                            frame++;
                            if (frame > 7) ExitScreen();
                            fadeValue = 0f;
                        }
                        break;
                    }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.darkenBackground(1f);
            ScreenManager.SpriteBatch.Begin();
            Vector2 msgSize = font.MeasureString(names[frame]);
            textOrigin.X = msgSize.X / 2;
            textOrigin.Y = msgSize.Y / 2;
            if (frame % 2 == 0)
            {
                //int prevInd = Math.Max(0, frame - 1);
                //ScreenManager.SpriteBatch.Draw(images[prevInd], new Vector2(rPos.X - 250f, rPos.Y - 250f), Color.White);
                ScreenManager.SpriteBatch.Draw(images[frame], lPosImage, Color.White * fadeValue);
                ScreenManager.SpriteBatch.DrawString(font, names[frame], rPos, Color.White * fadeValue, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                //int prevInd = Math.Max(0, frame - 1);
                //ScreenManager.SpriteBatch.Draw(images[prevInd], new Vector2(lPos.X - 250f, lPos.Y - 250f), Color.White);
                ScreenManager.SpriteBatch.Draw(images[frame], rPosImage, Color.White * fadeValue);
                ScreenManager.SpriteBatch.DrawString(font, names[frame], lPos, Color.White * fadeValue, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            }
            ScreenManager.SpriteBatch.End();
        }

        public override void HandleInput(InputDevices input) 
        { 
        }
    }
}
