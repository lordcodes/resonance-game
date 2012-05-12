using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    /// <summary>
    /// Class that represents the in game HUD.
    /// </summary>
    class Hud
    {
        private static SpriteBatch spriteBatch;
        private static SpriteFont font;
        private static SpriteFont scoreFont;
        private static SpriteFont objectivefont;
        private static ImportedCustomFont countDownFont;
        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;
        private static Graphics gameGraphics;
        private static Dictionary<string, BadVibeDetails> badVibes = new Dictionary<string, BadVibeDetails>();
        private static int score = 0;
        private static int health;
        private static int nitro;
        private static int shield;
        private static int freeze;
        private static int powerupValue;
        private static float AlphaValue = 0;
        private static float vibration = 0;
        private static double delay = 0.1;
        private static Texture2D healthBar;
        private static Texture2D healthSlice;
        private static Texture2D drumPad;
        private static Texture2D rest;
        private static Texture2D tempo;
        private static Texture2D block;
        private static Texture2D damage;
        private static MiniMap miniMap;
        private static Texture2D mask;
        private static HealthBar healthBarClass;
        private static ShieldBar shieldBar;
        private static FreezeBar freezeBar;
        private static NitroBar nitroBar;
        private static Texture2D pickupBackground;
        private static Texture2D pickupPercentageBackground;
        private static Texture2D pickupShield;
        private static Texture2D pickupNitro;
        private static Texture2D pickupFreeze;
        private static Texture2D plusFour;
        private static Texture2D plusFive;
        private static Texture2D x2;
        private static Texture2D x3;

        private static float ARMOUR_SCALE = 1.5f;

        /// <summary>
        /// Create a new HUD object.
        /// </summary>
        /// <param name="newContent">ContentManger object used throughout the game</param>
        /// <param name="newGraphics">GraphicsDeviceManager used throughout the game</param>
        /// <param name="newGameGraphics">Graphics object used to the 3D rendering</param>
        public Hud(ContentManager newContent, GraphicsDeviceManager newGraphics, Graphics newGameGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            gameGraphics = newGameGraphics;
        }

        /// <summary>
        /// Called once to load the textures needed for the HUD
        /// </summary>
        public void loadContent()
        {
            if (ScreenManager.ScreenWidth >= 1450)
            {
                font = Content.Load<SpriteFont>("Drawing/Fonts/HealthBarFont");
                objectivefont = Content.Load<SpriteFont>("Drawing/Fonts/ObjectiveFont");
            }
            else
            {
                font = Content.Load<SpriteFont>("Drawing/Fonts/HealthBarSmallFont");
                objectivefont = Content.Load<SpriteFont>("Drawing/Fonts/ObjectiveSmallFont");
            }
            scoreFont   = Content.Load<SpriteFont>         ("Drawing/Fonts/ScoreFont");
            countDownFont = Content.Load<ImportedCustomFont>("Drawing/Fonts/Custom/CountDown/CountDownFont");
            healthBar   = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthSlice");
            drumPad     = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour");
            rest        = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour_rest");
            block       = Content.Load<Texture2D>          ("Drawing/HUD/Textures/block");
            tempo       = Content.Load<Texture2D>          ("Drawing/HUD/Textures/tempo");
            damage      = Content.Load<Texture2D>          ("Drawing/HUD/Textures/damage");
            mask         = Content.Load<Texture2D>         ("Drawing/HUD/Textures/minimapalpha");
            pickupBackground = Content.Load<Texture2D>     ("Drawing/HUD/Textures/pickupbackground");
            pickupPercentageBackground = Content.Load<Texture2D>("Drawing/HUD/Textures/pickuppercentagebackground");
            pickupShield = Content.Load<Texture2D>         ("Drawing/HUD/Textures/pickupshield");
            pickupNitro  = Content.Load<Texture2D>         ("Drawing/HUD/Textures/pickupnitro");
            pickupFreeze = Content.Load<Texture2D>         ("Drawing/HUD/Textures/pickupfreeze");
            plusFour     = Content.Load<Texture2D>         ("Drawing/HUD/Textures/plus4");
            plusFive     = Content.Load<Texture2D>         ("Drawing/HUD/Textures/plus5");
            x2           = Content.Load<Texture2D>         ("Drawing/HUD/Textures/x2");
            x3           = Content.Load<Texture2D>         ("Drawing/HUD/Textures/x3");

            if (GameScreen.USE_MINIMAP)
            {
                miniMap = new MiniMap(graphics);
                miniMap.loadTextures(Content);
            }

            healthBarClass = new HealthBar(graphics);
            healthBarClass.loadTextures(Content);

            shieldBar = new ShieldBar(graphics);
            shieldBar.loadTextures(Content);

            nitroBar = new NitroBar(graphics);
            nitroBar.loadTextures(Content);

            freezeBar = new FreezeBar(graphics);
            freezeBar.loadTextures(Content);
        }

        /// <summary>
        /// Called to draw text in the debug position on screen
        /// </summary>
        /// <param name="text">Text string to display</param>
        public void drawDebugInfo(String text)
        {
            Vector2 coords = new Vector2(ScreenManager.pixelsX(17), ScreenManager.pixelsY(200));
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            drawText(coords2, text, Color.Black);
            drawText(coords, text, Color.White);
        }

        /// <summary>
        /// Draw text on screen at set coords.
        /// </summary>
        /// <param name="coords">Vector3 coords for position of text</param>
        /// <param name="text">Text string to display</param>
        /// <param name="color">Color of text to draw</param>
        private void drawText(Vector2 coords, String text, Color color)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, coords, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            Drawing.resetGraphics();
        }


        /// <summary>
        /// Called every time the HUD needs to be drawn.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            drawBadVibeArmour();
            drawDamage(gameTime);
            drawHealthBar();
            highlightedPower();
            drawShieldBar();
            drawNitroBar();
            drawFreezeBar();
            highlightedPowerPercentage();
            if (GameScreen.USE_MINIMAP) drawMiniMap();
            drawThrobber();
            drawCountDown();
            drawScore();
            drawMultiplier();
            drawLightning();
            spriteBatch.End();
            
            Drawing.resetGraphics();
        }

        private static void drawScore()
        {
            int xOffset = (int)Math.Round(scoreFont.MeasureString(score.ToString()).X);
            Vector2 coords = new Vector2(ScreenManager.pixelsX(1895) - xOffset, ScreenManager.pixelsY(20));
            spriteBatch.DrawString(scoreFont, score.ToString(), coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            coords.X--;
            coords.Y--;
            spriteBatch.DrawString(scoreFont, score.ToString(), coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private static void drawMultiplier()
        {
            Rectangle destination = new Rectangle(ScreenManager.pixelsX(1895 - plusFour.Width), ScreenManager.pixelsY(110), ScreenManager.pixelsX(plusFour.Width), ScreenManager.pixelsY(plusFour.Height));
            Rectangle source = new Rectangle(0, 0, plusFour.Width, plusFour.Height);

            if (PickupManager.pickupType == Pickup.X2)
            {
                spriteBatch.Draw(x2, destination, source, Color.White);
            }
            else if (PickupManager.pickupType == Pickup.X3)
            {
                spriteBatch.Draw(x3, destination, source, Color.White);
            }
            else if (PickupManager.pickupType == Pickup.PLUS4)
            {
                spriteBatch.Draw(plusFour, destination, source, Color.White);
            }
            else if (PickupManager.pickupType == Pickup.PLUS5)
            {
                spriteBatch.Draw(plusFive, destination, source, Color.White);
            }            
        }

        /// <summary>
        /// Draws the 3 second coundtown in the top centre of the screen
        /// </summary>
        private static void drawCountDown()
        {
            string objectiveString = "";
            string temp = "";
            ObjectiveManager.getObjectiveStrings(ref objectiveString, ref temp);
            int xOffset;

            string[] fragments = objectiveString.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fragments.Length; i++)
            {
                xOffset = (int)Math.Round(objectivefont.MeasureString(fragments[i]).X / 2);                

                TimeSpan time = ScreenManager.game.CountDown;
                if (time.Milliseconds > 0)
                {
                    spriteBatch.DrawString(objectivefont, fragments[i], new Vector2(ScreenManager.pixelsX(960) - xOffset, ScreenManager.pixelsY(20 + i * 35)), Color.White);
                    if (i == fragments.Length-1)
                    {
                        countDownFont.drawCentre(ScreenManager.pixelsX(960), ScreenManager.pixelsY(55 + i * 35), ScreenManager.WidthRatio, ScreenManager.HeightRatio, time.Seconds.ToString(), spriteBatch);
                    }
                }
                else if (time.TotalMilliseconds < 0 && time.TotalMilliseconds > -2000)
                {
                    spriteBatch.DrawString(objectivefont, fragments[i], new Vector2(ScreenManager.pixelsX(960) - xOffset, ScreenManager.pixelsY(20 + i * 35)), Color.White);
                    xOffset = (int)Math.Round(objectivefont.MeasureString("GO!").X / 2);
                    if (i == fragments.Length - 1)
                    {
                        spriteBatch.DrawString(objectivefont, "GO!", new Vector2(ScreenManager.pixelsX(960) - xOffset, ScreenManager.pixelsY(55 + i * 35)), Color.White);
                    }
                }
            }
        }

        public static void showDamage()
        {
            AlphaValue = 0.6f;
            vibration = GameScreen.VIBRATION;
            //Console.WriteLine("reset to max");
        }

        private void drawDamage(GameTime gameTime)
        {
            delay -= gameTime.ElapsedGameTime.TotalSeconds;
            Color TempColour = Color.White;
            if (delay <= 0)
            {
                delay = 0.001;
                if (AlphaValue > 0)
                {
                    TempColour *= AlphaValue;
                    spriteBatch.Draw(damage, new Rectangle(ScreenManager.pixelsX(0), ScreenManager.pixelsY(0), ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), TempColour);
                    AlphaValue -= 0.01f;
                    if (vibration > 0.01f)
                    {
                        vibration -= 0.01f;
                    }
                    else
                    {
                        vibration = 0;
                    }
                    GamePad.SetVibration(PlayerIndex.One, vibration, vibration);
                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
          //  Console.WriteLine(vibration.ToString());
        }

        private void drawThrobber() {
            if (MusicHandler.getTrack().inTime() > 0.5f)
            {
                spriteBatch.Draw(tempo, new Rectangle(ScreenManager.pixelsX(50), ScreenManager.pixelsY(1000), tempo.Width, tempo.Height), Color.White);
                GameScreen.getGV().showBeat();
            } else {
                spriteBatch.Draw(tempo, new Rectangle(ScreenManager.pixelsX(50), ScreenManager.pixelsY(1000), tempo.Width, tempo.Height), Color.Black);
            }
        }

        /// <summary>
        /// Update the HUD with information about the enemies.
        /// </summary>
        /// <param name="name">Enemy name</param>
        /// <param name="pos">Vector3 position of the enemy</param>
        /// <param name="armour">List of armour values</param>
        public void updateEnemy(string name, Vector3 pos, List<int> armour)
        {
            int bvDist = (int)Vector3.Distance(GameScreen.getGV().Body.Position, pos);

            if (bvDist <= BadVibe.MAX_ARMOUR_DISPLAY_DIST) {
                if (BadVibe.DRAW_HEALTH_AS_STRING) {
                    string armourString = "";

                    for (int i = 0; i < armour.Count; i++)
                    {
                        if (i != 0) armourString += " ";
                        if (Shockwave.REST == armour[i]) armourString += "_";
                        if (Shockwave.GREEN == armour[i]) armourString += "G";
                        if (Shockwave.YELLOW == armour[i]) armourString += "Y";
                        if (Shockwave.BLUE == armour[i]) armourString += "B";
                        if (Shockwave.RED == armour[i]) armourString += "R";
                        if (Shockwave.CYMBAL == armour[i]) armourString += "C";
                    }

                    int xOffset = (int)Math.Round(font.MeasureString(armourString).X / 2);

                    Vector2 newpos = new Vector2(500 + pos.X, 200 + pos.Z);
                    Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(pos.X, pos.Y + 1.2f, pos.Z), gameGraphics.Projection, CameraMotionManager.Camera.View, Matrix.Identity);
                    Vector2 screenPosition = new Vector2(projectedPosition.X - xOffset, projectedPosition.Y);

                    if (badVibes.ContainsKey(name)) badVibes[name] = new BadVibeDetails(screenPosition, armour, bvDist);
                    else badVibes.Add(name, new BadVibeDetails(screenPosition, armour, bvDist));

                } else {
                    int xOffset;

                    if (!BadVibe.DRAW_HEALTH_VERTICALLY) {
                        xOffset = (int) ((armour.Count * drumPad.Width * ARMOUR_SCALE) + ((armour.Count - 1) * BadVibe.ARMOUR_SPACING)) / 2;
                    } else {
                        xOffset = drumPad.Width / 2;
                    }

                    Vector2 newpos = new Vector2(500 + pos.X, 200 + pos.Z);
                    Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(pos.X, pos.Y + 1.2f, pos.Z), gameGraphics.Projection, CameraMotionManager.Camera.View, Matrix.Identity);
                    Vector2 screenPosition = new Vector2(projectedPosition.X - xOffset, projectedPosition.Y);

                    if (badVibes.ContainsKey(name)) badVibes[name] = new BadVibeDetails(screenPosition, armour,bvDist);
                    else badVibes.Add(name, new BadVibeDetails(screenPosition, armour, bvDist));
                }
            }
        }

        private void drawBadVibeArmour()
        {
            foreach (KeyValuePair<string, BadVibeDetails> badVibe in badVibes)
            {
                string key = badVibe.Key;
                BadVibeDetails details = badVibes[key];
                Vector2 pos = details.ScreenPos;
                int dist = details.Distance;
                List<int> armour = details.Armour; 

                if (BadVibe.DRAW_HEALTH_AS_STRING)
                {
                    string armourString = "";
                    for (int i = 0; i < armour.Count; i++)
                    {
                        if (i != 0) armourString += " ";
                        if (Shockwave.REST == armour[i]) armourString += "_";
                        if (Shockwave.GREEN == armour[i]) armourString += "G";
                        if (Shockwave.YELLOW == armour[i]) armourString += "Y";
                        if (Shockwave.BLUE == armour[i]) armourString += "B";
                        if (Shockwave.RED == armour[i]) armourString += "R";
                        if (Shockwave.CYMBAL == armour[i]) armourString += "C";
                    }
                    drawBadVibeHealthString(key, armourString, pos);
                }
                else
                {
                    drawBadVibeHealth(armour, pos, dist);
                }
            }

            badVibes.Clear();
        }

        public void drawBadVibeHealth(List<int> arm, Vector2 pos, int bvDist) {
            int posX = (int) pos.X;
            int posY = (int) pos.Y;

            Color c = new Color();

            float alphaC;

            // Don't draw if BV dead and no armour left.
            if (arm.Count == 0) return;

            if (bvDist <= BadVibe.MAX_ARMOUR_TRANSPARENCY_DIST) {
                alphaC  = 1f;
            } else {
                int window = BadVibe.MAX_ARMOUR_DISPLAY_DIST - BadVibe.MAX_ARMOUR_TRANSPARENCY_DIST;
                int diff   = bvDist - BadVibe.MAX_ARMOUR_TRANSPARENCY_DIST;

                alphaC  = (float) (1.0 - ((float) diff / (float) window));
            }

            // Draw v bars
            if (BadVibe.DRAW_HEALTH_VERTICALLY) {
                int rectX = (int) (posX - ((drumPad.Width * 1.5f * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING * 2f));
                int rectY = (int) (posY - ((arm.Count - 1) * drumPad.Height * ARMOUR_SCALE) - (arm.Count + 1) * BadVibe.ARMOUR_SPACING);
                int rectW = (int) ((drumPad.Width * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING);
                int rectH = (int) (((arm.Count + 1) * (drumPad.Height * ARMOUR_SCALE)) + (arm.Count - 1) * BadVibe.ARMOUR_SPACING);

                for (int i = 0; i < 4; i++) {
                    switch (i) {
                        case 0: { c = new Color(alphaC * 0.25f, 0.00f         , 0.00f         , alphaC * 0.45f); break; }
                        case 1: { c = new Color(alphaC * 0.25f, alphaC * 0.25f, 0.00f         , alphaC * 0.45f); break; }
                        case 2: { c = new Color(0.00f         , 0.00f         , alphaC * 0.25f, alphaC * 0.45f); break; }
                        case 3: { c = new Color(0.00f         , alphaC * 0.25f, 0.00f         , alphaC * 0.45f); break; }
                    }

                    spriteBatch.Draw(block, new Rectangle(rectX, rectY, rectW, rectH), c);

                    rectX += rectW;

                    if (i != 3) {
                        c = new Color(0f, 0f, (float) (alphaC * (128.0 / 255.0)), alphaC);
                        spriteBatch.Draw(block, new Rectangle(rectX, rectY, 1, rectH), c);
                    }
                }
            }

            for (int i = 0; i < arm.Count; i++) {
                switch (arm[i]) {
                    case 0: { c = new Color(alphaC, alphaC, alphaC, alphaC); break; }
                    case 1: { c = new Color(alphaC, 0f    , 0f    , alphaC); break; }
                    case 2: { c = new Color(alphaC, alphaC, 0f    , alphaC); break; }
                    case 3: { c = new Color(0.5f  , 0.5f  , alphaC, alphaC); break; }
                    case 4: { c = new Color(0f    , alphaC, 0f    , alphaC); break; }
                    case 5: { c = new Color(alphaC, alphaC, alphaC, alphaC); break; }
                }

                if (BadVibe.DRAW_HEALTH_VERTICALLY) {
                    posX = (int) pos.X;

                    switch (arm[i]) {
                        case 1: { posX -= (int) ((drumPad.Width * 1.5f * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING * 1.5f); break; }
                        case 2: { posX -= (int) ((drumPad.Width * 0.5f * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING * 0.5f); break; }
                        case 3: { posX += (int) ((drumPad.Width * 0.5f * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING * 0.5f); break; }
                        case 4: { posX += (int) ((drumPad.Width * 1.5f * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING * 1.5f); break; }
                    }
                }

                if (arm[i] != 0) {
                    spriteBatch.Draw(drumPad, new Rectangle(posX, posY, (int) (drumPad.Width * ARMOUR_SCALE), (int) (drumPad.Height * ARMOUR_SCALE)), c);
                } else {
                    spriteBatch.Draw(rest,    new Rectangle(posX, posY, (int) (drumPad.Width * ARMOUR_SCALE), (int) (drumPad.Height * ARMOUR_SCALE)), c);
                }

                if (!BadVibe.DRAW_HEALTH_VERTICALLY) {
                    posX += (int) (drumPad.Width * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING;
                } else {
                    posY -= (int) (drumPad.Height * ARMOUR_SCALE) + BadVibe.ARMOUR_SPACING;
                }
            }
            Drawing.resetGraphics();
        }

        /// <summary>
        /// Draws bad vibes health above them as a string.
        /// </summary>
        /// <param name="name">Name of bad vibe (not displayed datm)</param>
        /// <param name="armour">Armour string to use only atm</param>
        /// <param name="coords">Coords to display the armour levels</param>
        public void drawBadVibeHealthString(String name, string armour, Vector2 coords)
        {
            spriteBatch.DrawString(font, armour, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Drawing.resetGraphics();
        }

        /// <summary>
        /// Renders the health bar texture in the HUDBuffer render target
        /// </summary>
        public static void saveHealthBar()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            healthBarClass.saveTexture(spriteBatch, health);
        }

        /// <summary>
        /// Draws the health bar for the player on screen
        /// </summary>
        private void drawHealthBar()
        {
            healthBarClass.draw(spriteBatch);

            string objectiveString = "";
            string progressString = "";
            if (GameScreen.mode.MODE == GameMode.OBJECTIVES)
            {
                ObjectiveManager.getObjectiveStrings(ref progressString, ref objectiveString);
                ObjectiveManager.getProgress(ref progressString);
            }
            else
            {
                TimeSpan ts = MusicHandler.getTrack().Song.Duration - MediaPlayer.PlayPosition;
                int remainingM = ts.Minutes;
                int remainingS = ts.Seconds;
                if (remainingS < 0) remainingS = 0;
                string formattedTimeSpan = string.Format("{0:D2}:{1:D2}", remainingM, remainingS);
                objectiveString = "Arcade Mode";
                progressString = "Time remaining: " + formattedTimeSpan;
            }

            int xOffset = (int)Math.Round(font.MeasureString(objectiveString).X / 2);
            Vector2 coords = new Vector2(ScreenManager.pixelsX(350) - xOffset, ScreenManager.pixelsY(24));
            //spriteBatch.DrawString(font, "Objective: " + GameScreen.mode.MODE, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, objectiveString, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            coords.X--;
            coords.Y--;
            spriteBatch.DrawString(font, objectiveString, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            xOffset = (int)Math.Round(font.MeasureString(progressString).X / 2);
            coords.X = ScreenManager.pixelsX(315) - xOffset;
            coords.Y = ScreenManager.pixelsY(95);
            spriteBatch.DrawString(font, progressString, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            coords.X--;
            coords.Y--;
            spriteBatch.DrawString(font, progressString, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //int x = ScreenManager.pixelsX(10);
            //int y = ScreenManager.pixelsY(10);
            //int width = ScreenManager.pixelsX(healthBar.Width);
            //int height = ScreenManager.pixelsY(healthBar.Height);
            //int sliceX = x + ScreenManager.pixelsX(9);
            //int sliceY = y + ScreenManager.pixelsY(9);
            //int sliceWidth = 1;
            //int sliceHeight = ScreenManager.pixelsY(healthSlice.Height);
            //int limit = (int)Math.Round((float)ScreenManager.pixelsX(582) * health / GoodVibe.MAX_HEALTH);

            //float greenValue;
            //Color c;
            //spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);
            //for (int i = 0; i < limit; i++)
            //{
            //    greenValue = (float)i / ScreenManager.pixelsX(582);
            //    float red = (float)(greenValue > 0.5 ? 1 - 2 * (greenValue - 0.5) : 1.0);
            //    float green = (float)(greenValue > 0.5 ? 1.0 : 2 * greenValue);
            //    c = new Color(red, green, 0f);

            //    spriteBatch.Draw(healthSlice, new Rectangle(sliceX+i, sliceY, sliceWidth, sliceHeight), c);
            //}

            //Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(32)); //TODO: tidy this
            //Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            //spriteBatch.DrawString(font, "Health", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(font, "Health", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void saveFreezeBar()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            freezeBar.saveTexture(spriteBatch, freeze);
        }

        private void drawFreezeBar()
        {
            freezeBar.draw(spriteBatch);

            //int x = ScreenManager.pixelsX(14);
            //int y = ScreenManager.pixelsY(healthBar.Height + 5);
            //int width = ScreenManager.pixelsX(healthBar.Width / 2);
            //int height = ScreenManager.pixelsY(healthBar.Height / 2);

            //int sliceX = x + ScreenManager.pixelsX(4);
            //int sliceY = y + ScreenManager.pixelsY(5);
            //int sliceWidth = 1;
            //int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            //int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * freeze / GoodVibe.MAX_FREEZE);

            //spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            //Color c;
            //for (int i = 0; i < limit; i++)
            //{
            //    c = new Color(1f, 0.48f, 0f);

            //    spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            //}

            //Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81)); //TODO: tidy this
            //Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            //spriteBatch.DrawString(font, "Freeze", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(font, "Freeze", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void saveNitroBar()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            nitroBar.saveTexture(spriteBatch, nitro);
        }

        private void drawNitroBar()
        {
            nitroBar.draw(spriteBatch);

            //int x = ScreenManager.pixelsX(14);
            //int y = ScreenManager.pixelsY(healthBar.Height + 5 + healthBar.Height / 2);
            //int width = ScreenManager.pixelsX(healthBar.Width / 2);
            //int height = ScreenManager.pixelsY(healthBar.Height / 2);

            //int sliceX = x + ScreenManager.pixelsX(4);
            //int sliceY = y + ScreenManager.pixelsY(5);
            //int sliceWidth = 1;
            //int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            //int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * nitro / GoodVibe.MAX_NITRO);

            //spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            //Color c;
            //for (int i = 0; i < limit; i++)
            //{
            //    c = new Color(1f, 0.9f, 0f);

            //    spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            //}

            //Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81 + healthBar.Height / 2)); //TODO: tidy this
            //Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            //spriteBatch.DrawString(font, "Nitro", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(font, "Nitro", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void saveShieldBar()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            shieldBar.saveTexture(spriteBatch, shield);
        }

        private void drawShieldBar()
        {
            shieldBar.draw(spriteBatch);

            //int x = ScreenManager.pixelsX(14);
            //int y = ScreenManager.pixelsY(healthBar.Height + 5 + healthBar.Height);
            //int width = ScreenManager.pixelsX(healthBar.Width / 2);
            //int height = ScreenManager.pixelsY(healthBar.Height / 2);

            //int sliceX = x + ScreenManager.pixelsX(4);
            //int sliceY = y + ScreenManager.pixelsY(5);
            //int sliceWidth = 1;
            //int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            //int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * shield / GoodVibe.MAX_SHIELD);

            //spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            //Color c;
            //for (int i = 0; i < limit; i++)
            //{
            //    c = new Color(0.38f, 1f, 0.99f);

            //    spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            //}

            //Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81 + healthBar.Height)); //TODO: tidy this
            //Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            //spriteBatch.DrawString(font, "Shield", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(font, "Shield", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Renders the mini map texture in the miniMapBuffer render target
        /// </summary>
        public static void saveMiniMap()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            miniMap.saveTexture(spriteBatch);
        }

        /// <summary>
        /// Draws the Mini Map on screen
        /// </summary>
        public void drawMiniMap()
        {
            miniMap.draw(spriteBatch);
        }

        public void drawLightning() {
            WeatherManager.drawLightning(spriteBatch, block);
        }

        /// <summary>
        /// Updates the HUD with infomation about the player.
        /// </summary>
        /// <param name="h">Players health</param>
        /// <param name="s">Players score</param>
        public void updateGoodVibe(int h, int s, int n, int sh, int f)
        {
            health = h;
            score = s;
            nitro = n;
            shield = sh;
            freeze = f;
        }

        private void highlightedPowerPercentage()
        {
            string percent = powerupValue.ToString() + "%";
            int xOffset = (int)Math.Round(font.MeasureString(percent).X / 2);
            Vector2 coords = new Vector2(ScreenManager.pixelsX(33 + pickupBackground.Width / 2) - xOffset, ScreenManager.pixelsY(122));
            spriteBatch.DrawString(font, percent, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            coords.X--;
            coords.Y--;
            spriteBatch.DrawString(font, percent, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void highlightedPower()
        {
            Rectangle destination = new Rectangle(ScreenManager.pixelsX(30), ScreenManager.pixelsY(110), ScreenManager.pixelsX(pickupPercentageBackground.Width), ScreenManager.pixelsY(pickupPercentageBackground.Height));
            Rectangle source = new Rectangle(0, 0, pickupPercentageBackground.Width, pickupPercentageBackground.Height);
            spriteBatch.Draw(pickupPercentageBackground, destination, source, Color.White);

            destination = new Rectangle(ScreenManager.pixelsX(30), ScreenManager.pixelsY(10), ScreenManager.pixelsX(pickupBackground.Width), ScreenManager.pixelsY(pickupBackground.Height));
            source = new Rectangle(0, 0, pickupBackground.Width, pickupBackground.Height);
            spriteBatch.Draw(pickupBackground, destination, source, Color.White);

            if (GameScreen.getGV().selectedPower == 0)
            {
                spriteBatch.Draw(pickupNitro, destination, source, Color.White);
                powerupValue = (int)Math.Round((double)nitro / GoodVibe.MAX_NITRO * 100);
                //DebugDisplay.update("SELECTED POWER", "NITROUS");
            }

            if (GameScreen.getGV().selectedPower == 1)
            {
                spriteBatch.Draw(pickupShield, destination, source, Color.White);
                powerupValue = (int)Math.Round((double)shield / GoodVibe.MAX_SHIELD * 100);
                //DebugDisplay.update("SELECTED POWER", "SHIELD");
            }

            if (GameScreen.getGV().selectedPower == 2)
            {
                spriteBatch.Draw(pickupFreeze, destination, source, Color.White);
                powerupValue = (int)Math.Round((double)freeze / GoodVibe.MAX_FREEZE * 100);
                //DebugDisplay.update("SELECTED POWER", "FREEZE");
            }
        }
    }

    class BadVibeDetails
    {
        private Vector2 screenPos;
        private int distance;
        private List<int> armour;
        public Vector2 ScreenPos { get { return screenPos; }}
        public List<int> Armour { get { return armour; }}
        public int Distance { get { return distance; }}

        public BadVibeDetails(Vector2 screenPos, List<int> armour, int distance)
        {
            this.screenPos = screenPos;
            this.armour = armour;
            this.distance = distance;
        }
    }

}
