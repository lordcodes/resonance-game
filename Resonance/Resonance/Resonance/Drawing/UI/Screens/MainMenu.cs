using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class MainMenu : MenuScreen
    {
        Texture2D textBG;
        Song music;

        public MainMenu()
            : base("")
        {
            MenuElement play = new MenuElement("Enter the Resonance Chamber", startGame);
            MenuElement settings = new MenuElement("Settings", openSettings);
            MenuElement highScores = new MenuElement("High Scores", openHighScores);
            MenuElement credits = new MenuElement("View Credits", viewCredits);
            MenuElement quit = new MenuElement("Quit Game", quitGame);

            MenuItems.Add(play);
            MenuItems.Add(settings);
            MenuItems.Add(highScores);
            MenuItems.Add(credits);
            MenuItems.Add(quit);

            this.musicStart = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenu1"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenu2"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenu3"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenu4"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MainMenu5"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/Textures/texPixel"));
            textBG = this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/MenuItemBG");
            music = this.ScreenManager.Content.Load<Song>("Music/menu");
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            int x = 1400;
            int y = 400;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Position = new Vector2(ScreenManager.pixelsX(x), ScreenManager.pixelsY(y));
                y += 75;
            }
        }

        public override void controlMusic(bool play)
        {
            base.controlMusic(play);

            if (play)
            {
                //play
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(music);
                this.musicStart = false;
            }
            else
            {
                //stop
                MediaPlayer.Stop();
            }
        }

        public override void reset()
        {
            this.musicStart = true;
        }

        protected override void drawMenu(int index)
        {
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);

            ScreenManager.SpriteBatch.Draw(textBG, Vector2.Zero, Color.White);

            int x = (int)screenSize.X / 2 - 200;
            int y = (int)screenSize.Y / 2 - 287;

            ScreenManager.SpriteBatch.Draw(Bgs[index], new Vector2(x, y), Color.White);
        }

        public void startGame()
        {
            GameStats.init();
            if (GameScreen.mode.MODE == GameMode.OBJECTIVES)
            {
                ObjectiveManager.setObjective(ObjectiveManager.DEFAULT_OBJECTIVE);
                ObjectiveManager.loadObjectivesGame(ScreenManager);
            } else {
                ScreenManager.game = new GameScreen(ScreenManager, 5);
                LoadingScreen.LoadAScreen(ScreenManager, 1, ScreenManager.game);
            }
        }

        private void openSettings()
        {
            ScreenManager.addScreen(ScreenManager.settingsMenu);
        }

        private void openHighScores()
        {
            ScreenManager.addScreen(new HighscoreScreen(Font));
        }

        private void viewCredits()
        {
            ScreenManager.addScreen(new CreditsScreen());
        }

        private void quitGame()
        {
            string msg = "Are you sure you want to quit the game?";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
