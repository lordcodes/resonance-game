using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    abstract class MenuScreen : Screen
    {
        SpriteFont font;
        List<Texture2D> bg;
        List<MenuElement> menuItems = new List<MenuElement>();
        int selected = 0;
        string title;

        public MenuScreen(string title)
        {
            this.title = title;
            bg = new List<Texture2D>();
        }

        public override void HandleInput(InputDevices input)
        {
            bool up = (!input.LastKeys.IsKeyDown(Keys.Up) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadUp) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickUp)) &&
                      (input.Keys.IsKeyDown(Keys.Up) || input.PlayerOne.IsButtonDown(Buttons.DPadUp) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickUp));
            bool down = (!input.LastKeys.IsKeyDown(Keys.Down) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadDown) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickDown)) &&
                      (input.Keys.IsKeyDown(Keys.Down) || input.PlayerOne.IsButtonDown(Buttons.DPadDown) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickDown));
            bool select = (!input.LastKeys.IsKeyDown(Keys.Enter) && input.LastPlayerOne.Buttons.A != ButtonState.Pressed) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.Buttons.A == ButtonState.Pressed);
            bool pause = (!input.LastKeys.IsKeyDown(Keys.Escape) && input.LastPlayerOne.Buttons.Start != ButtonState.Pressed) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.Buttons.Start == ButtonState.Pressed);
            
            /*bool up = (!input.LastKeys.IsKeyDown(Keys.Up) && input.LastPlayerOne.DPad.Up != ButtonState.Pressed
                      (input.Keys.IsKeyDown(Keys.Up) || input.PlayerOne.DPad.Up == ButtonState.Pressed);
            bool down = (!input.LastKeys.IsKeyDown(Keys.Down) && input.LastPlayerOne.DPad.Down != ButtonState.Pressed) &&
                        (input.Keys.IsKeyDown(Keys.Down) || input.PlayerOne.DPad.Down == ButtonState.Pressed);
            bool select = (!input.LastKeys.IsKeyDown(Keys.Enter) && input.LastPlayerOne.Buttons.A != ButtonState.Pressed) &&
                          (input.Keys.IsKeyDown(Keys.Enter) || input.PlayerOne.Buttons.A == ButtonState.Pressed);
            bool pause = (!input.LastKeys.IsKeyDown(Keys.Escape) && input.LastPlayerOne.Buttons.Start != ButtonState.Pressed) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.Buttons.Start == ButtonState.Pressed);*/
            if (pause)
            {
                //Bring up pop-up to ask if you want to close the screen
            }

            if (up) moveUp();
            if (down) moveDown();
            if (select) itemSelect();
        }

        public void moveUp()
        {
            selected--;
            if (selected < 0) selected = menuItems.Count - 1;
        }

        public void moveDown()
        {
            selected++;
            if (selected >= menuItems.Count) selected = 0;
        }

        public void itemSelect()
        {
            menuItems[selected].CallBack();
        }

        protected virtual void cancel()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelect = (i == selected);

                menuItems[i].Update(this, gameTime, isSelect);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            bool debugScreen = (this is DebugMenu);

            updateItemLocations();

            if (!debugScreen)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(bg[selected], new Rectangle(0, 0, ScreenManager.ScreenWidth + 1,
                    ScreenManager.ScreenHeight + 1), Color.White);
            }
            else
            {
                ScreenManager.darkenBackground(0.75f);
                ScreenManager.SpriteBatch.Begin();
            }
            // Draw each menu entry in turn.
            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelect = (i == selected);

                menuItems[i].Draw(this, gameTime, isSelect);
            }
            ScreenManager.SpriteBatch.End();
        }

        protected virtual void updateItemLocations()
        {

        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        protected List<MenuElement> MenuItems
        {
            get { return menuItems; }
        }

        protected List<Texture2D> Bgs
        {
            get { return bg; }
        }
    }
}
