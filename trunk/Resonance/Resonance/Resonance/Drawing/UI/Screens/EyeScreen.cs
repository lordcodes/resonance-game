using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class EyeScreen : Screen
    {
        Texture2D background;
        SpriteFont font;

        public EyeScreen(string msg, int action)
        {
        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/Popup");
            font = ScreenManager.Game.Content.Load<SpriteFont>("Drawing/Fonts/PlainMenuFont");
        }

        public override void HandleInput(InputDevices input)
        {
            bool ok = (!input.LastKeys.IsKeyDown(Keys.Enter) && !input.LastPlayerOne.IsButtonDown(Buttons.A)) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.IsButtonDown(Buttons.A));
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));

        }

        public override void Draw(GameTime gameTime)
        {
            /*ScreenManager.darkenBackground(0.75f);

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            Vector2 msgSize = font.MeasureString(text);
            Vector2 msgPos = (screenSize - msgSize) / 2;

            ScreenManager.SpriteBatch.Begin();
            //ScreenManager.SpriteBatch.Draw(background, new Rectangle((int)msgPos.X - 35, (int)msgPos.Y - 25,
            //(int)msgSize.X + 70, (int)msgSize.Y + 50), Color.White);
            ScreenManager.SpriteBatch.Draw(background, new Vector2(msgPos.X - 35, msgPos.Y - 25), Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, msgPos, Color.White);
            ScreenManager.SpriteBatch.End();*/
        }
    }
}
