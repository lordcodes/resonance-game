using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class HintScreen : Screen
    {
        Texture2D background;
        SpriteFont font;

        public HintScreen()
        {
        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Drawing/UI/hintscreen");
            font = ScreenManager.Game.Content.Load<SpriteFont>("Drawing/Fonts/MenuFont");
        }

        public override void HandleInput(InputDevices input)
        {
            bool ok = (!input.LastKeys.IsKeyDown(Keys.Enter) && !input.LastPlayerOne.IsButtonDown(Buttons.A)) &&
                      (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.IsButtonDown(Buttons.A)) ||
                      (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                      (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));

            if (ok) ExitScreen();            
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.darkenBackground(0.9f);

            int padding = 100;

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            int availableWidth = (int)(screenSize.X - 2 * padding);
            int availableHeight = (int)(screenSize.Y - 2 * padding);

            float scaleX = (float)availableWidth / background.Width;
            float scaleY = (float)availableHeight / background.Height;            
            float usedScale = Math.Min(scaleX, scaleY);

            float startX = screenSize.X / 2 - usedScale * background.Width / 2;
            float startY = screenSize.Y / 2 - usedScale * background.Height / 2;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(Drawing.scaleTexture(background, usedScale), new Vector2(startX, startY), Color.White);
            ScreenManager.SpriteBatch.DrawString(font, "How to Play-", new Vector2(startX, startY - 30), Color.White);
            ScreenManager.SpriteBatch.DrawString(font, "Press A to continue", new Vector2(startX, startY + usedScale * background.Height + 10), Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }
}