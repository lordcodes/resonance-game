using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class MainMenu : MenuScreen
    {
        public MainMenu()
            : base("Welcome to the Resonance Chamber")
        {
            MenuElement play = new MenuElement("Enter the Resonance Chamber", new ItemDelegate(delegate { startGame(); }));
            MenuElement quit = new MenuElement("Quit Game", new ItemDelegate(MenuActions.exit));

            MenuItems.Add(play);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFirst"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSecond"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuThird"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFourth"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFifth"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSixth"));
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

        private void startGame()
        {
            ScreenManager.game = new GameScreen(ScreenManager);
            LoadingScreen.LoadAScreen(ScreenManager, true, ScreenManager.game);
        }
    }
}
