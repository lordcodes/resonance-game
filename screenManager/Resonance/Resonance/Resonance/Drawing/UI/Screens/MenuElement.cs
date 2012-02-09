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

        public MenuElement(string text, ItemDelegate callBack)
        {
            this.text = text;
            this.callBack = callBack;
        }

        public void Update(MenuScreen screen, GameTime gameTime, bool selected)
        {
            //Do selection effects
        }

        public void Draw(MenuScreen screen, GameTime gameTime, bool selected)
        {
            Color colour;
            if (selected) colour = Color.Orange;
            else colour = Color.White;

            //Pulsate text here

            SpriteFont font = screen.Font;
            Vector2 textOrigin = new Vector2(0, font.LineSpacing / 2);
            screen.ScreenManager.SpriteBatch.DrawString(font, text, position, colour, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
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
    }
}
