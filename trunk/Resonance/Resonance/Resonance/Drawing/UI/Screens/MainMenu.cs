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
        Texture2D textBG;

        public MainMenu()
            : base("Welcome to the Resonance Chamber")
        {
            MenuElement play = new MenuElement("Enter the Resonance Chamber", new ItemDelegate(delegate { startGame(); }));
            MenuElement quit = new MenuElement("Quit Game", new ItemDelegate(new ItemDelegate(delegate { quitGame(); })));

            MenuItems.Add(play);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuFirst"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuSecond"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuThird"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuFourth"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuFifth"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenuSixth"));
            textBG = this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MenuItemBG");
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            int x = 1400;
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

        private void quitGame()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
