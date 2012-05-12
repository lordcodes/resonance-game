using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class CreditsScreen : Screen
    {
        List<Texture2D> images;
        List<string> names;
        List<List<string>> titles;
        SpriteFont font;

        Vector2 lPos;
        Vector2 lPosImage;
        Vector2 rPos;
        Vector2 rPosImage;
        Vector2 nameOrigin;

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
            names.Add("Geoffrey Burch");
            names.Add("Paul Keast");
            titles = new List<List<string>>(8);
            titles.Add(new List<string>(3) { "Graphics", "Content Pipeline", "Advanced Textures" });
            titles.Add(new List<string>(3) { "Physics", "Artificial Intelligence", "Games Screens and Menus" });
            titles.Add(new List<string>(3) { "Level Editor", "Projectiles", "High Scores" });
            titles.Add(new List<string>(3) { "Mechanics", "Particle Engine", "Mini-Map" });
            titles.Add(new List<string>(3) { "Pickups", "HUD", "Render Targets" });
            titles.Add(new List<string>(3) { "3D Modelling", "Animation", "Bad Vibe Spawners" });
            titles.Add(new List<string>(3) { "Game Music Composer" });
            titles.Add(new List<string>(3) { "Team Mentor", "Technical Consultant"});

            timeElapsed = 0;
            fadeValue = 0.0f;
            frame = 0;
            state = 0;

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

            lPos = new Vector2(screenSize.X / 2, screenSize.Y - 50f);
            lPosImage = new Vector2(40, screenSize.Y - 250);
            rPos = new Vector2(screenSize.X * 1.5f, screenSize.Y - 50f);
            rPosImage = new Vector2((screenSize.X * 2f) - 752, screenSize.Y - 250);
            nameOrigin = Vector2.Zero;
        }

        public override void LoadContent() 
        {
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bacteria_render"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/bad_vibe_render_green"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/UI/Credits/neuron_render"));
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
            Vector2 nameSize = font.MeasureString(names[frame]);
            nameOrigin.X = nameSize.X / 2;
            nameOrigin.Y = nameSize.Y / 2;

            if (frame % 2 == 0)
            {
                ScreenManager.SpriteBatch.Draw(images[frame], lPosImage, Color.White * fadeValue);
                ScreenManager.SpriteBatch.DrawString(font, names[frame], rPos, Color.White * fadeValue, 0f, nameOrigin, 1f, SpriteEffects.None, 0f);
                drawTitles(rPos);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(images[frame], rPosImage, Color.White * fadeValue);
                ScreenManager.SpriteBatch.DrawString(font, names[frame], lPos, Color.White * fadeValue, 0f, nameOrigin, 1f, SpriteEffects.None, 0f);
                drawTitles(lPos);
            }
            ScreenManager.SpriteBatch.End();
        }

        private void drawTitles(Vector2 pos)
        {
            Vector2 titlePos = new Vector2(pos.X, pos.Y);
            int titleNum = titles[frame].Count;
            Vector2 titleSize = Vector2.Zero;
            Vector2 titleOrigin = Vector2.Zero;

            titlePos.Y += 50f;

            for (int i = 0; i < titleNum; i++)
            {
                string title = titles[frame][i];
                titleSize = font.MeasureString(title);
                titleOrigin.X = titleSize.X / 2;
                titleOrigin.Y = titleSize.Y / 2;

                ScreenManager.SpriteBatch.DrawString(font, title, titlePos, Color.White * fadeValue, 0f, titleOrigin, 0.7f, SpriteEffects.None, 0f);
                titlePos.Y += 30f;
            }
        }

        public override void HandleInput(InputDevices input) 
        {
            bool select = (!input.LastKeys.IsKeyDown(Keys.Enter) && input.LastPlayerOne.Buttons.A != ButtonState.Pressed) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.Buttons.A == ButtonState.Pressed);
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));
            bool backPause = !input.LastPlayerOne.IsButtonDown(Buttons.Start) && input.PlayerOne.IsButtonDown(Buttons.Start);

            if (select || back || backPause) ExitScreen();
        }
    }
}
