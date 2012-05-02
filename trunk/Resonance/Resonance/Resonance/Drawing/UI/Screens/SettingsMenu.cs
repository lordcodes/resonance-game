using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class SettingsMenu : MenuScreen
    {
        List<string> difficulties;
        int currentDifficulty = GameScreen.DIFFICULTY;
        Vector2 begSize;
        Vector2 lrOrigin;

        public SettingsMenu()
            : base("Settings")
        {
            MenuElement difficulty = new MenuElement("Change Difficulty", new ItemDelegate(delegate { changeDifficulty(); }));
            MenuElement back = new MenuElement("Back to Main Menu", new ItemDelegate(delegate { toMainMenu(); }));

            MenuItems.Add(difficulty);
            MenuItems.Add(back);

            this.musicStart = false;

            difficulties = new List<string>(6);
            difficulties.Add("Beginner");
            difficulties.Add("Easy");
            difficulties.Add("Medium");
            difficulties.Add("Hard");
            difficulties.Add("Expert");
            difficulties.Add("Insane");
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox"));

            begSize = Font.MeasureString("   Beginner   ");

            Vector2 size = Font.MeasureString(">");
            lrOrigin = new Vector2(size.X / 2, size.Y / 2);
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            Vector2 position = new Vector2(0f, screenSize.Y / 2 - 100 - Font.LineSpacing);

            float diff = -120f;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                position.X = (int)screenSize.X + diff;
                MenuItems[i].Position = position;
                position.Y += MenuItems[i].Size(this).Y + 100;
                diff = 0f;
            }
        }

        protected override void  drawMenu(int index)
        {
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

            float x = screenSize.X - 360;
            float y = screenSize.Y - 250;
            ScreenManager.SpriteBatch.Draw(Bgs[0], new Vector2(x, y), Color.White);

            Vector2 difficultyPos = MenuItems[0].Position;
            difficultyPos.X += (MenuItems[0].Size(this).X + 50);

            Vector2 msgSize = Font.MeasureString(difficulties[currentDifficulty]);
            Vector2 textOrigin = new Vector2(msgSize.X / 2, msgSize.Y / 2);

            Vector2 pos = difficultyPos;
            pos.X -= begSize.X / 2;
            ScreenManager.SpriteBatch.DrawString(Font, "<", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.DrawString(Font, difficulties[currentDifficulty], difficultyPos, Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);

            pos.X += begSize.X;
            ScreenManager.SpriteBatch.DrawString(Font, ">", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);
        }

        private void toMainMenu()
        {
            ExitScreen();
        }

        private void changeDifficulty()
        {
            currentDifficulty++;
            if (currentDifficulty > 5) currentDifficulty = 0;
        }

        public int CurrentDifficulty
        {
            get { return currentDifficulty; }
        }
    }
}
