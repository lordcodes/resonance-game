using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class MainMenu : MenuScreen
    {
        private static int level = 0;

        Texture2D textBG;
        Song music;

        public MainMenu()
            : base("Welcome to the Resonance Chamber")
        {
            MenuElement play = new MenuElement("Enter the Resonance Chamber", new ItemDelegate(delegate { startGame(); }));
            MenuElement settings = new MenuElement("Settings", new ItemDelegate(delegate { openSettings(); }));
            MenuElement highScores = new MenuElement("High Scores", new ItemDelegate(delegate { openHighScores(); }));
            MenuElement quit = new MenuElement("Quit Game", new ItemDelegate(new ItemDelegate(delegate { quitGame(); })));

            MenuItems.Add(play);
            MenuItems.Add(settings);
            MenuItems.Add(highScores);
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

            //Draw lines from menu items to the associated ring

            Vector2 start = new Vector2(screenSize.X / 2, screenSize.Y / 4);
            Vector2 end = new Vector2(screenSize.X + 100, screenSize.Y / 4);
            //Utility.drawLine(ScreenManager.SpriteBatch, Bgs[4], start, end, Color.Orange, 3);
        }

        private void startGame()
        {
            ScreenManager.game = new GameScreen(ScreenManager,level);
            LoadingScreen.LoadAScreen(ScreenManager, true, ScreenManager.game);
        }

        private void openSettings()
        {
            ScreenManager.addScreen(new SettingsMenu());
        }

        private void openHighScores()
        {
        }

        private void quitGame()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}
