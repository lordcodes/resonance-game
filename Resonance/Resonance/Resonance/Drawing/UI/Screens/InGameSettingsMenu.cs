using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class InGameSettingsMenu : MenuScreen
    {
        int currentMusicVolume;
        int currentVibrationLevel;
        bool changedVibration = false;
        double vibrateTimer;

        public InGameSettingsMenu()
            : base("Settings")
        {
            MenuElement musicVolume = new MenuElement("Music Volume", changeMusicVolume);
            MenuElement vibration = new MenuElement("Vibration", changeVibration);
            MenuElement back = new MenuElement("Back to Main Menu", toMainMenu);

            MenuItems.Add(musicVolume);
            MenuItems.Add(vibration);
            MenuItems.Add(back);

            this.musicStart = false;

            currentMusicVolume = (int)Math.Round(MediaPlayer.Volume * 10f, 0);
            currentVibrationLevel = (int)Math.Round(GameScreen.VIBRATION * 10, 0);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/PauseMenuStatsBox"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/VolumeFull"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/VolumeBlank"));
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
                    currentMusicVolume--;
                    if (currentMusicVolume < 0) currentMusicVolume = 10;
                    MediaPlayer.Volume = ((float)currentMusicVolume) / 10f;
                }
                else if (Selected == 1)
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
                    currentMusicVolume++;
                    if (currentMusicVolume > 10) currentMusicVolume = 0;
                    MediaPlayer.Volume = ((float)currentMusicVolume) / 10f;
                }
                else if (Selected == 1)
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
                if (i == MenuItems.Count - 2) diff = 0f;
            }
        }

        protected override void drawMenu(int index)
        {
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight / 2);

            float x = screenSize.X - 360;
            float y = screenSize.Y - 250;
            ScreenManager.SpriteBatch.Draw(Bgs[0], new Vector2(x, y), Color.White);

            Vector2 pos = MenuItems[0].Position;
            pos.X = screenSize.X + 30;

            x = pos.X;
            y = MenuItems[0].Position.Y - 5;

            for (int i = 0; i < 10; i++)
            {
                if (i < currentMusicVolume) ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);
                else ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
                x += 25;
            }

            x = pos.X;
            y = MenuItems[1].Position.Y - 5;

            for (int i = 0; i < 10; i++)
            {
                if (i < currentVibrationLevel) ScreenManager.SpriteBatch.Draw(Bgs[1], new Vector2(x, y), Color.White);
                else ScreenManager.SpriteBatch.Draw(Bgs[2], new Vector2(x, y), Color.White);
                x += 25;
            }
        }

        private void toMainMenu()
        {
            ExitScreen();
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
