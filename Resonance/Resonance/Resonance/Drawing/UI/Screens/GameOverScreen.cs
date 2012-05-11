using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class GameOverScreen : MenuScreen
    {
        Texture2D textBG;
        GameStats stats;

        public GameOverScreen(GameStats stats)
            : base("Game Over")
        {
            this.stats = stats;

            MenuElement playAgain = new MenuElement("Play Again", playGameAgain);
            MenuElement quit = new MenuElement("Back to Main Menu", quitGame);
            MenuElement quitCompletely = new MenuElement("Quit Game", quitGameCompletely);

            MenuItems.Add(playAgain);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuBox"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox"));
            textBG = this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MenuItemBG");
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

        protected override void  drawMenu(int index)
        {
            //410 width by 540 height
            //Ratio 1.32 (H / W)

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            int x = (int)screenSize.X / 2 - 200;
            int y = (int)screenSize.Y / 2 - 240;

            ScreenManager.SpriteBatch.Draw(Bgs[0], new Vector2(x, y), Color.White);

            x = (int)((screenSize.X * 2) * 0.75f - 360);
            y = (int)screenSize.Y / 2 - 250;

            ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);

            x += 60;
            y += 60;

            string message = "Final score: " + stats.Score + "\n\n";
            message += "Total Bad Vibes Killed: " + stats.BVsKilled + "\n\n";
            message += "Highest Multi-Kill with Single Wave: " + stats.Multikill + "\n\n";
            message += "\n            HIGHSCORES             \n";
            for (int ii = 0; ii < HighScoreManager.data.SIZE; ii++)
            {
                message += "    " + HighScoreManager.data.PlayerName[ii] + "       " + HighScoreManager.data.Score[ii] + "           \n";
            }

            ScreenManager.SpriteBatch.DrawString(Font, message, new Vector2(x, y), Color.White);
        }

        private void playGameAgain()
        {
            if (GameScreen.mode.MODE == GameMode.OBJECTIVES)
            {
                ObjectiveManager.setObjective(ObjectiveManager.DEFAULT_OBJECTIVE);
                ObjectiveManager.loadObjectivesGame(ScreenManager);
            }
            else
            {
                ScreenManager.game = new GameScreen(ScreenManager, 0);
                LoadingScreen.LoadAScreen(ScreenManager, 1, ScreenManager.game);
            }
        }

        private void quitGame()
        {
            ScreenManager.mainMenu.reset();
            LoadingScreen.LoadAScreen(ScreenManager, 0, ScreenManager.mainMenu);
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
