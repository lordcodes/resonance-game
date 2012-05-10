using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Resonance
{
    abstract class MenuScreen : Screen
    {
        SpriteFont headingFont;
        SpriteFont font;
        List<Texture2D> bg;
        List<MenuElement> menuItems = new List<MenuElement>();
        int selected = 0;
        string title;
        int upTimes = 0;
        int downTimes = 0;

        protected bool musicStart;

        public MenuScreen(string title)
        {
            this.title = title;
            bg = new List<Texture2D>();
        }

        public override void LoadContent()
        {
            headingFont = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/HeadingFont");
            font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MenuFont");
        }

        public override void HandleInput(InputDevices input)
        {
            bool lastUp = (input.LastKeys.IsKeyDown(Keys.Up) && input.Keys.IsKeyDown(Keys.Up)) ||
                          (input.LastPlayerOne.IsButtonDown(Buttons.DPadUp) && input.PlayerOne.IsButtonDown(Buttons.DPadUp)) ||
                          (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickUp) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickUp));
            bool lastDown = (input.LastKeys.IsKeyDown(Keys.Down) && input.Keys.IsKeyDown(Keys.Down)) ||
                          (input.LastPlayerOne.IsButtonDown(Buttons.DPadDown) && input.PlayerOne.IsButtonDown(Buttons.DPadDown)) ||
                          (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickDown) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickDown));

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
            bool back = (!input.LastKeys.IsKeyDown(Keys.Escape) && !input.LastPlayerOne.IsButtonDown(Buttons.B)) &&
                         (input.Keys.IsKeyDown(Keys.Escape) || input.PlayerOne.IsButtonDown(Buttons.B));
            bool backPause = !input.LastPlayerOne.IsButtonDown(Buttons.Start) && input.PlayerOne.IsButtonDown(Buttons.Start);

            if (back)
            {
                if (this is MainMenu)
                {
                    string msg = "Are you sure you want to quit the game?\n";
                    msg += "(Enter - OK, Escape - Cancel)";

                    ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
                }
                else
                {
                    selected = 0;
                    ExitScreen();
                }
            }
            if (backPause && !(this is MainMenu))
            {
                selected = 0;
                ExitScreen();
            }

            if (up) moveUp();
            else if (lastUp)
            {
                upTimes++;
                if (upTimes == 25)
                {
                    moveUp();
                    upTimes = 0;
                }
            }
            if (down) moveDown();
            else if (lastDown)
            {
                downTimes++;
                if (downTimes == 25)
                {
                    moveDown();
                    downTimes = 0;
                }
            }
            if (select) itemSelect();
        }

        public void moveUp()
        {
            selected--;
            if (selected < 0) selected = menuItems.Count - 1;
            MusicHandler.playSound("menu-cursor");
        }

        public void moveDown()
        {
            selected++;
            if (selected >= menuItems.Count) selected = 0;
            MusicHandler.playSound("menu-cursor");
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

            if (musicStart) controlMusic(true);

            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelect = (i == selected);

                menuItems[i].Update(this, gameTime, isSelect);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            updateItemLocations();

            bool debugScreen = (this is DebugMenu);
            bool pauseScreen = (this is PauseMenu);
            bool endGameScreen = (this is GameOverScreen || this is SuccessScreen);
            bool settingsScreen = (this is SettingsMenu || this is InGameSettingsMenu);

            if(pauseScreen || endGameScreen) ScreenManager.darkenBackground(0.9f);
            else if (debugScreen) ScreenManager.darkenBackground(0.75f);
            else if (settingsScreen) ScreenManager.darkenBackground(1f);
            ScreenManager.SpriteBatch.Begin();
            drawMenu(selected);

            // Draw each menu entry in turn.
            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelect = (i == selected);

                menuItems[i].Draw(this, gameTime, isSelect);
            }

            ScreenManager.SpriteBatch.DrawString(headingFont, title, new Vector2(ScreenManager.ScreenWidth / 2 - headingFont.MeasureString(title).X / 2, 50), Color.White);
            ScreenManager.SpriteBatch.End();
        }

        protected virtual void updateItemLocations()
        {
        }

        protected virtual void drawMenu(int index)
        {
        }

        public virtual void controlMusic(bool play)
        {
        }

        public virtual void reset()
        {
        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public SpriteFont HeadingFont
        {
            get { return headingFont; }
            set { headingFont = value; }
        }

        protected List<MenuElement> MenuItems
        {
            get { return menuItems; }
        }

        protected List<Texture2D> Bgs
        {
            get { return bg; }
        }

        public int Selected
        {
            get { return selected; }
            set { selected = value; }
        }
    }
}
