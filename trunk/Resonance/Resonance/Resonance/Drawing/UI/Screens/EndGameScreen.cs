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
        Texture2D textBG;
        GameStats stats;

        public EndGameScreen(GameStats stats)
            : base("Game Over")
        {
            this.stats = stats;

            MenuElement playAgain = new MenuElement("Play Again", new ItemDelegate(delegate { playGameAgain(); }));
            MenuElement quit = new MenuElement("Back to Main Menu", new ItemDelegate(delegate { quitGame(); }));
            MenuElement quitCompletely = new MenuElement("Quit Game", new ItemDelegate(delegate { quitGameCompletely(); }));

            MenuItems.Add(playAgain);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MenuFont");
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

            //ScreenManager.SpriteBatch.Draw(textBG, Vector2.Zero, Color.White);

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
            message += "Number of Nitro Uses: " + stats.NitroUses + "\n\n";
            message += "Number of Shield Uses: " + stats.ShieldUses + "\n\n";
            message += "Number of Freeze Uses: " + stats.FreezeUses + "\n\n";
            message += "Number of Pickups: " + stats.Powerups + "\n\n";

            ScreenManager.SpriteBatch.DrawString(Font, message, new Vector2(x, y), Color.White);
        }

        private void playGameAgain()
        {
            ScreenManager.game = new GameScreen(ScreenManager, 0);
            LoadingScreen.LoadAScreen(ScreenManager, true, ScreenManager.game);
        }

        private void quitGame()
        {
            ScreenManager.mainMenu.reset();
            LoadingScreen.LoadAScreen(ScreenManager, false, ScreenManager.mainMenu);
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
