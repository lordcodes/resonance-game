using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class EndGameScreen : MenuScreen
    {
        public EndGameScreen()
            : base("Game Over")
        {
            MenuElement playAgain = new MenuElement("Play Again", new ItemDelegate(delegate { playGameAgain(); }));
            MenuElement quit = new MenuElement("Back to Main Menu", new ItemDelegate(delegate { quitGame(); }));
            MenuElement quitCompletely = new MenuElement("Quit Game", new ItemDelegate(delegate { quitGameCompletely(); }));

            MenuItems.Add(playAgain);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuBox"));
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);

            //Centre in x is 400
            Vector2 position = new Vector2(0f, screenSize.Y / 2 - 100 - Font.LineSpacing);
            for (int i = 0; i < MenuItems.Count; i++)
            {
                position.X = (int)screenSize.X / 2;
                MenuItems[i].Position = position;

                position.Y += MenuItems[i].Size(this).Y + 100;
            }
        }

        private void playGameAgain() //TODO: fixme
        {
            //LoadingScreen.LoadAScreen(ScreenManager, false, new MainMenu());
            ScreenManager.game = new GameScreen(ScreenManager);
            LoadingScreen.LoadAScreen(ScreenManager, true, ScreenManager.game);
        }

        private void quitGame()
        {
            LoadingScreen.LoadAScreen(ScreenManager, false, new MainMenu());
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
