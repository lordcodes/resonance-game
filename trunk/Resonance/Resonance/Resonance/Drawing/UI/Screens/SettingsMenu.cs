using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class SettingsMenu : MenuScreen
    {
        List<string> difficulties;
        int currentDifficulty = GameScreen.DIFFICULTY;
        Vector2 begSize;
        Vector2 lrOrigin;

        float currentMusicVolume;

        public SettingsMenu()
            : base("Settings")
        {
            MenuElement difficulty = new MenuElement("Difficulty", changeDifficulty);
            MenuElement musicVolume = new MenuElement("Music Volume", changeMusicVolume);
            MenuElement back = new MenuElement("Back to Main Menu", toMainMenu);

            MenuItems.Add(difficulty);
            MenuItems.Add(musicVolume);
            MenuItems.Add(back);

            this.musicStart = false;

            difficulties = new List<string>(6);
            difficulties.Add("Beginner");
            difficulties.Add("Easy");
            difficulties.Add("Medium");
            difficulties.Add("Hard");
            difficulties.Add("Expert");
            difficulties.Add("Insane");

            currentMusicVolume = MediaPlayer.Volume;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/VolumeFull"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/VolumeBlank"));

            begSize = Font.MeasureString("   Beginner   ");

            Vector2 size = Font.MeasureString(">");
            lrOrigin = new Vector2(size.X / 2, size.Y / 2);
        }

        public override void HandleInput(InputDevices input)
        {
            base.HandleInput(input);

            bool left = (!input.LastKeys.IsKeyDown(Keys.Left) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadLeft) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft)) &&
                      (input.Keys.IsKeyDown(Keys.Left) || input.PlayerOne.IsButtonDown(Buttons.DPadLeft) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft));
            bool right = (!input.LastKeys.IsKeyDown(Keys.Right) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadRight) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickRight)) &&
                      (input.Keys.IsKeyDown(Keys.Right) || input.PlayerOne.IsButtonDown(Buttons.DPadRight) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickRight));

            if (left)
            {
                if (Selected == 0)
                {
                    currentDifficulty--;
                    if (currentDifficulty < 0) currentDifficulty = 5;
                }
                else if (Selected == 1)
                {
                    currentMusicVolume -= 0.1f;
                    if (currentMusicVolume < 0) currentMusicVolume = 1f;
                }
            }
            else if (right)
            {
                if (Selected == 0)
                {
                    currentDifficulty++;
                    if (currentDifficulty > 5) currentDifficulty = 0;
                }
                else if (Selected == 1)
                {
                    currentMusicVolume += 0.1f;
                    if (currentMusicVolume > 10f) currentMusicVolume = 0;
                }
            }
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
                if(i == MenuItems.Count - 2) diff = 0f;
            }
        }

        protected override void  drawMenu(int index)
        {
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

            float x = screenSize.X - 360;
            float y = screenSize.Y - 250;
            ScreenManager.SpriteBatch.Draw(Bgs[0], new Vector2(x, y), Color.White);

            Vector2 difficultyPos = MenuItems[0].Position;
            difficultyPos.X = screenSize.X + 120;
            Vector2 msgSize = Font.MeasureString(difficulties[currentDifficulty]);
            Vector2 textOrigin = new Vector2(msgSize.X / 2, msgSize.Y / 2);
            Vector2 pos = difficultyPos;
            pos.X -= begSize.X / 2;
            ScreenManager.SpriteBatch.DrawString(Font, "<", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, difficulties[currentDifficulty], difficultyPos, Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            pos.X += begSize.X;
            ScreenManager.SpriteBatch.DrawString(Font, ">", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);

            x = pos.X -= begSize.X;
            y = MenuItems[1].Position.Y - 5;

            float i;
            for (i = 0; i < currentMusicVolume; i += 0.1f)
            {
                ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);
                x += 25;
            }
            for (; i < 1f; i += 0.1f)
            {
                ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
                x += 25;
            }

            //ScreenManager.SpriteBatch.DrawString(Font, currentMusicVolume.ToString(), difficultyPos, Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
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

        private void changeMusicVolume()
        {
            currentMusicVolume += 0.1f;
            if (currentMusicVolume > 10f) currentMusicVolume = 0;
        }

        public int CurrentDifficulty
        {
            get { return currentDifficulty; }
        }

        public float CurrentMusicVolume
        {
            get { return currentMusicVolume; }
        }
    }
}
