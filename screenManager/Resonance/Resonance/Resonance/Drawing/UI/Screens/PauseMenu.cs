using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class PauseMenu : MenuScreen
    {
        public PauseMenu()
            : base("Paused")
        {
            //Resume
            //Quit to Main Menu

            MenuElement quit = new MenuElement("Quit Game", new ItemDelegate(delegate { quitGame(); }));

            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFirst"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSecond"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuThird"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFourth"));
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            int x = 1350;
            int y = 300;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Position = new Vector2(ScreenManager.pixelsX(x), ScreenManager.pixelsY(y));
                y += 75;
            }
        }

        private void quitGame()
        {
            ScreenManager.Game.Exit();
        }
    }
}
