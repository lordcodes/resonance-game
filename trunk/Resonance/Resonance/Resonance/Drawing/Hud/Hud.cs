
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;
        private static Graphics gameGraphics;
        private static Dictionary<string, BadVibeDetails> badVibes = new Dictionary<string, BadVibeDetails>();
        private static int score = 0;
        private static int health;
        private static int nitro;
        private static int shield;
        private static int freeze;
        private static Texture2D healthBar;
        private static Texture2D healthSlice;
        private static Texture2D drumPad;
        private static Texture2D rest;
        private static Texture2D tempo;
        private static Texture2D block;
        private static MiniMap miniMap;
        private static ImportedCustomFont scoreFont;
        private static HealthBar healthBarClass;

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
            font        = Content.Load<SpriteFont>         ("Drawing/Fonts/DebugFont");
            healthBar   = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthSlice");
            drumPad     = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour");
            rest        = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour_rest");
            block       = Content.Load<Texture2D>          ("Drawing/HUD/Textures/block");
            tempo       = Content.Load<Texture2D>          ("Drawing/HUD/Textures/tempo");
            scoreFont   = Content.Load<ImportedCustomFont> ("Drawing/Fonts/Custom/Score/ScoreFont");

            miniMap = new MiniMap();
            miniMap.loadTextures(Content);

            healthBarClass = new HealthBar(graphics);
            healthBarClass.loadTextures(Content);
        }

        //private TestEmitter tEmm = new TestEmitter(new Vector3(250f, 250f, 250f));
        //public static Texture2D getBlock() { return block; }

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
        public void Draw()
        {
            if (spriteBatch == null) spriteBatch = ScreenManager.game.ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            drawBadVibeArmour();
            drawHealthBar();
            highlightedPower();
            drawNitroBar();
            drawShieldBar();
            drawFreezeBar();
            if(GameScreen.USE_MINIMAP)drawMiniMap();
            drawThrobber();
            scoreFont.drawLeft(ScreenManager.pixelsX(1890), ScreenManager.pixelsY(15), ScreenManager.WidthRatio, ScreenManager.HeightRatio, score.ToString(), spriteBatch);
            drawLightning();
            spriteBatch.End();
            Drawing.resetGraphics();
        }

        private void drawThrobber() {
            if (GameScreen.musicHandler.getTrack().inTime() > 0.5f) {
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
                    case 3: { c = new Color(0f    , 0f    , alphaC, alphaC); break; }
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
        /// Draws the helath bar for the player on screen.
        /// </summary>
        private void drawHealthBar()
        {
            //healthBarClass.draw(spriteBatch);

            int x = ScreenManager.pixelsX(10);
            int y = ScreenManager.pixelsY(10);
            int width = ScreenManager.pixelsX(healthBar.Width);
            int height = ScreenManager.pixelsY(healthBar.Height);
            int sliceX = x + ScreenManager.pixelsX(9);
            int sliceY = y + ScreenManager.pixelsY(9);
            int sliceWidth = 1;
            int sliceHeight = ScreenManager.pixelsY(healthSlice.Height);
            int limit = (int)Math.Round((float)ScreenManager.pixelsX(582) * health / GoodVibe.MAX_HEALTH);

            float greenValue;
            Color c;
            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);
            for (int i = 0; i < limit; i++)
            {
                greenValue = (float)i / ScreenManager.pixelsX(582);
                float red = (float)(greenValue > 0.5 ? 1 - 2 * (greenValue - 0.5) : 1.0);
                float green = (float)(greenValue > 0.5 ? 1.0 : 2 * greenValue);
                c = new Color(red, green, 0f);

                spriteBatch.Draw(healthSlice, new Rectangle(sliceX+i, sliceY, sliceWidth, sliceHeight), c);
            }

            Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(32)); //TODO: tidy this
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            spriteBatch.DrawString(font, "Health", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Health", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void drawNitroBar()
        {
            int x = ScreenManager.pixelsX(14);
            int y = ScreenManager.pixelsY(healthBar.Height + 5);
            int width = ScreenManager.pixelsX(healthBar.Width / 2);
            int height = ScreenManager.pixelsY(healthBar.Height / 2);

            int sliceX = x + ScreenManager.pixelsX(4);
            int sliceY = y + ScreenManager.pixelsY(5);
            int sliceWidth = 1;
            int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * nitro / GoodVibe.MAX_NITRO);

            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            Color c;
            for (int i = 0; i < limit; i++)
            {
                c = new Color(1f, 0.9f, 0f);

                spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            }

            Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81)); //TODO: tidy this
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            spriteBatch.DrawString(font, "Nitro", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Nitro", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void drawShieldBar()
        {
            int x = ScreenManager.pixelsX(14);
            int y = ScreenManager.pixelsY(healthBar.Height + 5 + healthBar.Height / 2);
            int width = ScreenManager.pixelsX(healthBar.Width / 2);
            int height = ScreenManager.pixelsY(healthBar.Height / 2);

            int sliceX = x + ScreenManager.pixelsX(4);
            int sliceY = y + ScreenManager.pixelsY(5);
            int sliceWidth = 1;
            int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * shield / GoodVibe.MAX_SHIELD);

            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            Color c;
            for (int i = 0; i < limit; i++)
            {
                c = new Color(0.38f, 1f, 0.99f);

                spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            }

            Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81 + healthBar.Height / 2)); //TODO: tidy this
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            spriteBatch.DrawString(font, "Shield", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Shield", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void drawFreezeBar()
        {
            int x = ScreenManager.pixelsX(14);
            int y = ScreenManager.pixelsY(healthBar.Height + 5 + healthBar.Height);
            int width = ScreenManager.pixelsX(healthBar.Width / 2);
            int height = ScreenManager.pixelsY(healthBar.Height / 2);

            int sliceX = x + ScreenManager.pixelsX(4);
            int sliceY = y + ScreenManager.pixelsY(5);
            int sliceWidth = 1;
            int sliceHeight = ScreenManager.pixelsY(healthSlice.Height / 2 - 1);
            int limit = (int)Math.Round((float)ScreenManager.pixelsX(582 / 2) * freeze / GoodVibe.MAX_FREEZE);

            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);

            Color c;
            for (int i = 0; i < limit; i++)
            {
                c = new Color(1f, 0.48f, 0f);

                spriteBatch.Draw(healthSlice, new Rectangle(sliceX + i, sliceY, sliceWidth, sliceHeight), c);
            }

            Vector2 coords = new Vector2(ScreenManager.pixelsX(25), ScreenManager.pixelsY(81 + healthBar.Height)); //TODO: tidy this
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            spriteBatch.DrawString(font, "Freeze", coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Freeze", coords2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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

        public void highlightedPower()
        {
            if (GameScreen.getGV().selectedPower == 0)
            {
                DebugDisplay.update("SELECTED POWER", "NITROUS");
            }

            if (GameScreen.getGV().selectedPower == 1)
            {
                DebugDisplay.update("SELECTED POWER", "SHIELD");
            }

            if (GameScreen.getGV().selectedPower == 2)
            {
                DebugDisplay.update("SELECTED POWER", "FREEZE");
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
