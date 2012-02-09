using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class PopupScreen : Screen
    {
        public static int TO_MAIN_MENU = 0;
        public static int QUIT_GAME = 1;

        string text;
        int action;
        Texture2D background;
        SpriteFont font;

        public PopupScreen(string msg, int action)
        {
            this.text = msg;
            this.action = action;
        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Drawing/Textures/texPopup");
            font = ScreenManager.Game.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
        }

        public override void HandleInput(InputDevices input)
        {
            bool ok = (!input.LastKeys.IsKeyDown(Keys.Enter) && input.LastPlayerOne.Buttons.A != ButtonState.Pressed) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.Buttons.A == ButtonState.Pressed);
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && input.LastPlayerOne.Buttons.B != ButtonState.Pressed) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.Buttons.B == ButtonState.Pressed);

            if (ok) success();
            else if (back) cancel();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.darkenBackground(0.75f);

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            Vector2 msgSize = font.MeasureString(text);
            Vector2 msgPos = (screenSize - msgSize) / 2;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, new Rectangle((int)msgPos.X - 25, (int)msgPos.Y - 15,
                (int)msgSize.X + 50, (int)msgSize.Y + 30), Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, msgPos, Color.White);
            ScreenManager.SpriteBatch.End();
        }

        private void success()
        {
            if (action == 0)
            {
                LoadingScreen.LoadAScreen(ScreenManager, false, new MainMenu());
            }
            else if (action == 1)
            {
                ScreenManager.Game.Exit();
            }
            ExitScreen();
        }

        private void cancel()
        {
            ExitScreen();
        }
    }
}
