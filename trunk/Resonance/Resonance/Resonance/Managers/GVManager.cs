using Microsoft.Xna.Framework.Input;

namespace Resonance
{
    class GVManager
    {
        public static readonly int NITROUS = 0;
        public static readonly int SHIELD = 1;
        public static readonly int FREEZE = 2;

        private static bool usedBoost = false;
        private static bool usedFreeze = false;
        private static bool usedShield = false;

        public static void input(InputDevices input)
        {
            KeyboardState kbd = input.Keys;
            KeyboardState lastKbd = input.LastKeys;
            GamePadState pad = input.PlayerOne;
            GamePadState lastPad = input.LastPlayerOne;

            bool shoulders = pad.Buttons.LeftShoulder == ButtonState.Pressed || pad.Buttons.RightShoulder == ButtonState.Pressed;

            if ((kbd.IsKeyDown(Keys.Q) && !lastKbd.IsKeyDown(Keys.Q)) || (pad.Buttons.LeftStick == ButtonState.Pressed && lastPad.Buttons.LeftStick != ButtonState.Pressed))
            {
                Drawing.addWave(GameScreen.getGV().Body.Position);
            }
            if ((kbd.IsKeyDown(Keys.L) && !lastKbd.IsKeyDown(Keys.L)))
            {
                GameScreen.getGV().showBeat();
            }
            if (kbd.IsKeyDown(Keys.Space))
            {
                MusicHandler.getTrack().playTrack();
            }
            if (kbd.IsKeyDown(Keys.S))
            {
                MusicHandler.getTrack().stopTrack();
            }
            if (kbd.IsKeyDown(Keys.P))
            {
                MusicHandler.getTrack().pauseTrack();
            }
            if (shoulders || kbd.IsKeyDown(Keys.M))
            {
                if (GameScreen.USE_MINIMAP) MiniMap.enlarge();
            }
            else
            {
                if (GameScreen.USE_MINIMAP) MiniMap.ensmall();
            }

            if (kbd.IsKeyDown(Keys.PrintScreen) && !lastKbd.IsKeyDown(Keys.PrintScreen))
            {
                ScreenManager.game.takeScreenshot();
            }

            if (pad.Buttons.X == ButtonState.Pressed || kbd.IsKeyDown(Keys.J))
            {
                
                GameScreen.getGV().selectedPower = SHIELD;
                GVMotionManager.resetBoost();
            }

            if (pad.Buttons.Y == ButtonState.Pressed || kbd.IsKeyDown(Keys.K))
            {
                GameScreen.getGV().selectedPower = NITROUS;

                GameScreen.getGV().shieldDown();
            }

            if (pad.Buttons.B == ButtonState.Pressed || kbd.IsKeyDown(Keys.L))
            {
                GameScreen.getGV().selectedPower = FREEZE;

                GameScreen.getGV().shieldDown();
                GVMotionManager.resetBoost();
            } else {
                if (GameScreen.getGV().getFreezer() != null) GameScreen.getGV().getFreezer().switchOff();
            }

            if (pad.Triggers.Right > 0.1 || pad.Triggers.Left > 0.1 || kbd.IsKeyDown(Keys.T))
            {
               usePower(GameScreen.getGV().selectedPower);
            }
            else
            {
                GameScreen.getGV().shieldDown();
                GVMotionManager.resetBoost();
                usedBoost = false;
                usedShield = false;
                usedFreeze = false;
            }
            GVMotionManager.input(kbd, pad);

            lastPad = pad;
            lastKbd = kbd;
        }

        public static void usePower(int power)
        {
            if (power == SHIELD)
            {
                if (GameScreen.getGV().Shield > 0)
                {
                    GameScreen.getGV().adjustShield(-1);
                    GameScreen.getGV().shieldUp();
                    if (!usedShield)
                    {
                        usedShield = true;
                    }
                }
                else
                {
                    GameScreen.getGV().shieldDown();
                    usedShield = false;
                }
            }

            if (power == FREEZE)
            {
                if (GameScreen.getGV().getFreezer() == null) {
                    GameScreen.getGV().createFreezer();
                }

                GameScreen.getGV().getFreezer().setPos(GameScreen.getGV().Body.Position);

                if (GameScreen.getGV().Freeze > 0)
                {
                    GameScreen.getGV().FreezeActive = true;
                    GameScreen.getGV().adjustFreeze(-1);
                    GameScreen.getGV().getFreezer().switchOn();
                    if (!usedFreeze)
                    {
                        usedFreeze = true;
                    }
                }

            }

            if (power == NITROUS)
            {
                if (GameScreen.getGV().Nitro > 0.1)
                {
                    GameScreen.getGV().adjustNitro(-1);
                    GVMotionManager.BOOSTING = true;
                    if (!usedBoost)
                    {
                        usedBoost = true;
                    }
                }
                else
                {
                    GVMotionManager.resetBoost();
                    usedBoost = false;
                }
            }
        }
    }
}
