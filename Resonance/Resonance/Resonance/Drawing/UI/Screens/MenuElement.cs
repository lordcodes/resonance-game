using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
delegate void ItemDelegate();

namespace Resonance
{
    class MenuElement
    {
        string text;
        Vector2 position;
        private ItemDelegate callBack;
        Texture2D background;
        float fade;
        bool mainMenu = false;

        public MenuElement(string text, ItemDelegate callBack)
        {
            this.text = text;
            this.callBack = callBack;
        }

        public void Update(MenuScreen screen, GameTime gameTime, bool selected)
        {
        }

        public void Draw(MenuScreen screen, GameTime gameTime, bool selected)
        {
            Color colour;
            if (selected) colour = Color.Orange;
            else colour = Color.White;

            float speed = (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
            if (selected)
            {
                float temp = fade + speed;
                if (temp < 1) fade = temp;
                else fade = 1;
            }
            else
            {
                fade = Math.Max(fade - speed, 0);
                float temp = fade - speed;
                if (temp > 0) fade = temp;
                else fade = 0;
            }

            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulse = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulse * 0.075f * fade;

            SpriteFont font = screen.Font;
            Vector2 msgSize = font.MeasureString(text);

            if (mainMenu)
            {
                if (selected)
                {
                    screen.ScreenManager.SpriteBatch.Draw(background, new Rectangle((int)position.X - 40, (int)position.Y - 25,
                        (int)msgSize.X + 250, (int)msgSize.Y + 25), Color.White);
                }
            }

            Vector2 textOrigin = new Vector2(msgSize.X / 2, font.LineSpacing / 2);
            screen.ScreenManager.SpriteBatch.DrawString(font, text, position, colour, 0f, textOrigin, scale, SpriteEffects.None, 0f);
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public ItemDelegate CallBack
        {
            get { return callBack; }
        }

        public Texture2D Background
        {
            set { background = value; }
        }

        public bool MainMenu
        {
            set { mainMenu = value; }
        }
    }
}
