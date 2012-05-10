using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class HighscoreScreen : Screen
    {
        Texture2D textBG;
        SpriteFont headingFont;
        Texture2D box;

        public HighscoreScreen(SpriteFont newFont)
            : base()
        {
            headingFont = newFont;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            box = this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox");
        }

        public override void HandleInput(InputDevices input)
        {
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));
            bool backPause = !input.LastPlayerOne.IsButtonDown(Buttons.Start) && input.PlayerOne.IsButtonDown(Buttons.Start);

            if (back || backPause) ExitScreen();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            string title = "HIGHSCORES\n";
            string message = "\n\n";
            Vector2 msgSize = headingFont.MeasureString(title);
            Vector2 textOrigin = new Vector2(msgSize.X / 2, msgSize.Y / 2);
            ScreenManager.darkenBackground(1f);
            ScreenManager.SpriteBatch.Begin();
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            float x = screenSize.X / 2 - 360;
            float y = screenSize.Y / 2 - 250;
            ScreenManager.SpriteBatch.Draw(box, new Vector2(x, y), Color.White);

            ScreenManager.SpriteBatch.DrawString(headingFont, title, new Vector2(ScreenManager.ScreenWidth / 2, 100),
                Color.White, 0f, textOrigin, 1.2f, SpriteEffects.None, 0f);
            for (int i = 0; i < HighScoreManager.data.SIZE; i++)
            {
                message = HighScoreManager.data.PlayerName[i];
                ScreenManager.SpriteBatch.DrawString(headingFont, message, new Vector2(ScreenManager.ScreenWidth / 2 - 100, 200 + i * 30),
               Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
                message =  HighScoreManager.data.Score[i].ToString();
                ScreenManager.SpriteBatch.DrawString(headingFont, message, new Vector2(ScreenManager.ScreenWidth / 2 + 250, 200 + i * 30),
               Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();
        }

    }
}
