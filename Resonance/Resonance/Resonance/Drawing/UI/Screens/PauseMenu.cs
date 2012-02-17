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
            MenuElement quit = new MenuElement("Back to Main Menu", new ItemDelegate(delegate { quitGame(); }));
            MenuElement quitCompletely = new MenuElement("Quit Game", new ItemDelegate(delegate { quitGameCompletely(); }));

            MenuItems.Add(resume);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/PauseMenuBG"));
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            //Centre in x is 400
            Vector2 position = new Vector2(0f, ScreenManager.pixelsY(400));
            for (int i = 0; i < MenuItems.Count; i++)
            {
                position.X = ScreenManager.pixelsX(400 - (int)MenuItems[i].Size(this).X / 2);
                MenuItems[i].Position = position;

                position.Y += MenuItems[i].Size(this).Y + 25;
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
