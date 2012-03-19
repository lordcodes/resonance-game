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
            font = ScreenManager.Game.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
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

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            int msgPosX = (int)(screenSize.X - background.Width) / 2;
            int msgPosY = (int)(screenSize.Y - background.Height) / 2;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, new Vector2(msgPosX, msgPosY), Color.White);
            ScreenManager.SpriteBatch.DrawString(font, "How to Play- press A to continue", new Vector2(msgPosX, msgPosY + background.Height+10), Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }
}
