using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class PauseMenu : MenuScreen
    {
        public PauseMenu()
            : base("Paused")
        {
            MenuElement resume = new MenuElement("Resume", resumeGame);
            MenuElement settings = new MenuElement("Settings", openSettings);
            MenuElement quit = new MenuElement("Back to Main Menu", quitGame);
            MenuElement quitCompletely = new MenuElement("Quit Game", quitGameCompletely);

            MenuItems.Add(resume);
            MenuItems.Add(settings);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);

            this.musicStart = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuBox"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/Controller-Small"));
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);

            //Centre in x is 400
            Vector2 position = new Vector2(0f, screenSize.Y / 2 - 100 - Font.LineSpacing);
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuElement elem = MenuItems[i];
                position.X = (int)screenSize.X / 2;
                elem.Position = position;

                position.Y += elem.Size(this).Y + 50;
            }
        }

        protected override void  drawMenu(int index)
        {
            //410 width by 540 height
            //Ratio 1.32 (H / W)

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            int x = (int)screenSize.X / 2 - 200;
            int y = (int)screenSize.Y / 2 - 240;

            ScreenManager.SpriteBatch.Draw(Bgs[0], new Vector2(x, y), Color.White);            
          
            if (screenSize.X * 2 > 1450)           
                x = (int)((screenSize.X * 2) * 0.75f - 360);            
            else x = (int)(screenSize.X * 2) - 720;
            y = (int)screenSize.Y / 2 - 250;

            ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);

            x += 30;
            y += 100;

            ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
        }

        private void resumeGame()
        {
            ExitScreen();
        }

        private void quitGame()
        {
            string msg = "Are you sure you want to quit to the\n";
            msg += "main menu?";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.TO_MAIN_MENU));
        }

        private void openSettings()
        {
            ScreenManager.addScreen(ScreenManager.inGameSettings);
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit the game?";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
