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
            MenuElement resume = new MenuElement("Resume", new ItemDelegate(delegate { resumeGame(); }));
            MenuElement quit = new MenuElement("Quit to Main Menu", new ItemDelegate(delegate { quitGame(); }));
            MenuElement quitCompletely = new MenuElement("Quit Game (For now until I add debug menu)", new ItemDelegate(delegate { quitGameCompletely(); }));

            MenuItems.Add(resume);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);
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

        private void resumeGame()
        {
            ExitScreen();
        }

        private void quitGame()
        {
            string msg = "Are you sure you want to quit to the main menu?\n";
            msg += "A button or Enter - OK\n";
            msg += "B button or Escape - Cancel";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.TO_MAIN_MENU));
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit to the main menu?\n";
            msg += "A button or Enter - OK\n";
            msg += "B button or Escape - Cancel";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
