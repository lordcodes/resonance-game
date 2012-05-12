using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace Resonance
{
    class GameOverScreen : MenuScreen
    {
        GameStats stats;

        Vector2 lPos;
        Vector2 lPosText;
        Vector2 rPos;
        Vector2 rPosText;

        int leftTimes;
        int rightTimes;

        private static System.IAsyncResult result = null;
        private int ii = 0;

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

            
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            lPos = new Vector2(screenSize.X - 580f, screenSize.Y - 550);
            rPos = new Vector2(screenSize.X + 30f, screenSize.Y - 550);

            lPosText = new Vector2(lPos.X + 60f, lPos.Y + 60f);
            rPosText = new Vector2(rPos.X + 60f, rPos.Y + 60f);

            leftTimes = 0;
            rightTimes = 0;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/GameOverScreenBox"));
        }

        public override void HandleInput(InputDevices input)
        {
            base.HandleInput(input);

            bool lastLeft = (input.LastKeys.IsKeyDown(Keys.Left) && input.Keys.IsKeyDown(Keys.Left)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.DPadLeft) && input.PlayerOne.IsButtonDown(Buttons.DPadLeft)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft));
            bool lastRight = (input.LastKeys.IsKeyDown(Keys.Right) && input.Keys.IsKeyDown(Keys.Right)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.DPadRight) && input.PlayerOne.IsButtonDown(Buttons.DPadRight)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickRight) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickRight));

            bool left = (!input.LastKeys.IsKeyDown(Keys.Left) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadLeft) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft)) &&
                      (input.Keys.IsKeyDown(Keys.Left) || input.PlayerOne.IsButtonDown(Buttons.DPadLeft) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft));
            bool right = (!input.LastKeys.IsKeyDown(Keys.Right) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadRight) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickRight)) &&
                      (input.Keys.IsKeyDown(Keys.Right) || input.PlayerOne.IsButtonDown(Buttons.DPadRight) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickRight));

            if (left) moveUp();
            else if (lastLeft)
            {
                leftTimes++;
                if (leftTimes == 25)
                {
                    moveUp();
                    leftTimes = 0;
                }
            }
            if (right) moveDown();
            else if (lastRight)
            {
                rightTimes++;
                if (rightTimes == 25)
                {
                    moveDown();
                    rightTimes = 0;
                }
            }
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);

            //Centre in x is 400
            Vector2 position = new Vector2(screenSize.X - 500, ScreenManager.pixelsY(200));
            for (int i = 0; i < MenuItems.Count; i++)
            {
                position.X = position.X + 250;
                MenuItems[i].Position = position;
            }
        }

        protected override void  drawMenu(int index)
        {
            ScreenManager.SpriteBatch.Draw(Bgs[0], lPos, Color.White);
            ScreenManager.SpriteBatch.Draw(Bgs[0], rPos, Color.White);

            if (!Guide.IsVisible)
            {
                if (ii == 0) ii = 1;

                switch (ii)
                {
                    case 1:
                        {
                            if (HighScoreManager.position != -1)
                            {
                                result = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Player Name", "Enter your name:", "", null, null);
                            }
                            ii = 2;
                            break;
                        }

                    case 2:
                        {
                           
                            if (result.IsCompleted)
                            {
                                HighScoreManager.data.PlayerName[HighScoreManager.position] = Guide.EndShowKeyboardInput(result);
                                ii = 3;
                            }
                            break;
                        }
                }
                GamerServicesDispatcher.Update();
            }
            HighScoreManager.saveFile();
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            int x = (int)screenSize.X / 2 - 200;
            int y = (int)screenSize.Y / 2 - 240;

            string message = "Final score: " + stats.Score + "\n\n";
            message += "Total Bad Vibes Killed: " + stats.BVsKilled + "\n\n";
            message += "Highest Multi-Kill with Single Wave: " + stats.Multikill + "\n\n";

            ScreenManager.SpriteBatch.DrawString(Font, message, lPosText, Color.White);

            message = "            HIGHSCORES             \n";
            for (int ii = 0; ii < HighScoreManager.data.SIZE; ii++)
            {
                message += "    " + HighScoreManager.data.PlayerName[ii] + "       " + HighScoreManager.data.Score[ii] + "           \n";
            }

            ScreenManager.SpriteBatch.DrawString(Font, message, rPosText, Color.White);

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
