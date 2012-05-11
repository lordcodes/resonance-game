using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class SettingsMenu : MenuScreen
    {
        List<string> difficulties;
        List<string> gameModes;
        Vector2 begSize;
        Vector2 lrOrigin;

        int currentMusicVolume;
        int currentVibrationLevel;
        bool changedVibration = false;
        double vibrateTimer;

        public SettingsMenu()
            : base("Settings")
        {
            MenuElement gameMode = new MenuElement("Game Mode", changeGameMode);
            MenuElement difficulty = new MenuElement("Difficulty", changeDifficulty);
            MenuElement musicVolume = new MenuElement("Music Volume", changeMusicVolume);
            MenuElement vibration = new MenuElement("Vibration", changeVibration);
            MenuElement back = new MenuElement("Back to Main Menu", toMainMenu);

            MenuItems.Add(gameMode);
            MenuItems.Add(difficulty);
            MenuItems.Add(musicVolume);
            MenuItems.Add(vibration);
            MenuItems.Add(back);

            this.musicStart = false;

            difficulties = new List<string>(6);
            difficulties.Add("Beginner");
            difficulties.Add("Easy");
            difficulties.Add("Medium");
            difficulties.Add("Hard");
            difficulties.Add("Expert");
            difficulties.Add("Insane");

            gameModes = new List<string>(2);
            gameModes.Add("Arcade");
            gameModes.Add("Objectives");

            currentMusicVolume = (int)Math.Round(MediaPlayer.Volume * 10f, 0);
            currentVibrationLevel = (int)Math.Round(GameScreen.VIBRATION * 10, 0);
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
                    GameScreen.mode.MODE--;
                    if (GameScreen.mode.MODE < 0) GameScreen.mode.MODE = 1;
                }
                else if (Selected == 1)
                {
                    GameScreen.DIFFICULTY--;
                    if (GameScreen.DIFFICULTY < 0) GameScreen.DIFFICULTY = 5;
                }
                else if (Selected == 2)
                {
                    currentMusicVolume--;
                    if (currentMusicVolume < 0) currentMusicVolume = 10;
                    MediaPlayer.Volume = ((float)currentMusicVolume) / 10f;
                }
                else if (Selected == 3)
                {
                    currentVibrationLevel--;
                    if (currentVibrationLevel < 0) currentVibrationLevel = 10;
                    setVibration();
                }
            }
            else if (right)
            {
                if (Selected == 0)
                {
                    GameScreen.mode.MODE++;
                    if (GameScreen.mode.MODE > 1) GameScreen.mode.MODE = 0;
                }
                else if (Selected == 1)
                {
                    GameScreen.DIFFICULTY++;
                    if (GameScreen.DIFFICULTY > 5) GameScreen.DIFFICULTY = 0;
                }
                else if (Selected == 2)
                {
                    currentMusicVolume++;
                    if (currentMusicVolume > 10) currentMusicVolume = 0;
                    MediaPlayer.Volume = ((float)currentMusicVolume) / 10f;
                }
                else if (Selected == 3)
                {
                    currentVibrationLevel++;
                    if (currentVibrationLevel > 10) currentVibrationLevel = 0;
                    setVibration();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (changedVibration)
            {
                vibrateTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (vibrateTimer <= 0)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    changedVibration = false;
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
                position.Y += MenuItems[i].Size(this).Y + 40;
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
            Vector2 msgSize = Font.MeasureString(gameModes[GameScreen.mode.MODE]);
            Vector2 textOrigin = new Vector2(msgSize.X / 2, msgSize.Y / 2);
            Vector2 pos = difficultyPos;
            pos.X -= begSize.X / 2;
            ScreenManager.SpriteBatch.DrawString(Font, "<", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, gameModes[GameScreen.mode.MODE], difficultyPos, Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            pos.X += begSize.X;
            ScreenManager.SpriteBatch.DrawString(Font, ">", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);

            difficultyPos = MenuItems[1].Position;
            difficultyPos.X = screenSize.X + 120;
            msgSize = Font.MeasureString(difficulties[GameScreen.DIFFICULTY]);
            textOrigin = new Vector2(msgSize.X / 2, msgSize.Y / 2);
            pos = difficultyPos;
            pos.X -= begSize.X / 2;
            ScreenManager.SpriteBatch.DrawString(Font, "<", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, difficulties[GameScreen.DIFFICULTY], difficultyPos, Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
            pos.X += begSize.X;
            ScreenManager.SpriteBatch.DrawString(Font, ">", pos, Color.White, 0f, lrOrigin, 1f, SpriteEffects.None, 0f);

            x = (pos.X - begSize.X);
            y = MenuItems[2].Position.Y - 5;

            for (int i = 0; i < 10; i++)
            {
                if (i < currentMusicVolume) ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);
                else ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
                x += 25;
            }

            x = (pos.X - begSize.X);
            y = MenuItems[3].Position.Y - 5;

            for (int i = 0; i < 10; i++)
            {
                if (i < currentVibrationLevel) ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);
                else ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
                x += 25;
            }
            //ScreenManager.SpriteBatch.DrawString(Font, currentMusicVolume.ToString(), new Vector2(500,500), Color.White, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
        }

        private void toMainMenu()
        {
            ExitScreen();
        }

        private void changeGameMode()
        {
            GameScreen.mode.MODE++;
            if (GameScreen.mode.MODE > 1) GameScreen.mode.MODE = 0;
        }

        private void changeDifficulty()
        {
            GameScreen.DIFFICULTY++;
            if (GameScreen.DIFFICULTY > 5) GameScreen.DIFFICULTY = 0;
        }

        private void changeMusicVolume()
        {
            currentMusicVolume++;
            if (currentMusicVolume > 10) currentMusicVolume = 0;
            MediaPlayer.Volume = ((float)currentMusicVolume) / 10f;
        }

        private void changeVibration()
        {
            currentVibrationLevel++;
            if (currentVibrationLevel > 10) currentVibrationLevel = 0;
            setVibration();
        }

        private void setVibration()
        {
            GameScreen.VIBRATION = ((float)currentVibrationLevel) / 10f;
            GamePad.SetVibration(PlayerIndex.One, GameScreen.VIBRATION, GameScreen.VIBRATION);
            changedVibration = true;
            vibrateTimer = 0.5;
        }
    }
}
