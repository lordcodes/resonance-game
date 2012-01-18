
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
        private static Dictionary<string, Vector2> dictionary = new Dictionary<string, Vector2>();
        private static int score = 0;
        private static float health = 1;
        private static Texture2D healthBar;
        private static Texture2D healthSlice;
        private static Texture2D drumPad;
        private static Texture2D rest;
        private static Texture2D block;
        private static MiniMap miniMap;
        private static ImportedCustomFont scoreFont;

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

            //miniMap = new MiniMap();
        }

        /// <summary>
        /// Called once to load the textures needed for the HUD
        /// </summary>
        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            font        = Content.Load<SpriteFont>         ("Drawing/Fonts/DebugFont");
            healthBar   = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>          ("Drawing/HUD/Textures/healthSlice");
            drumPad     = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour");
            rest        = Content.Load<Texture2D>          ("Drawing/HUD/Textures/armour_rest");
            block       = Content.Load<Texture2D>          ("Drawing/HUD/Textures/block");
            scoreFont   = Content.Load<ImportedCustomFont> ("Drawing/Fonts/Custom/Score/ScoreFont");

            miniMap = new MiniMap();
            miniMap.loadTextures(Content);
        }

        /// <summary>
        /// Called to draw text in the debug position on screen
        /// </summary>
        /// <param name="text">Text string to display</param>
        public void drawDebugInfo(String text)
        {
            Vector2 coords = new Vector2(Drawing.pixelsX(17), Drawing.pixelsY(80));
            Vector2 coords2 = new Vector2(coords.X - 1, coords.Y - 1);
            drawText(coords2, text, Color.Black);
            drawText(coords, text, Color.White);
        }

        /// <summary>
        /// Draw text in the central, pause menu, position on screen
        /// </summary>
        /// <param name="text">Text to display</param>
        public void drawMenu(String text)
        {
            Vector2 coords = new Vector2(Drawing.pixelsX(800), Drawing.pixelsY(400));
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
            spriteBatch.Begin();
            /*foreach (KeyValuePair<string, Vector2> pair in dictionary)
            {
                spriteBatch.DrawString(font, "BV", pair.Value, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }*/
            drawHealthBar();
            drawMiniMap();
            scoreFont.drawLeft(Drawing.pixelsX(1890), Drawing.pixelsY(15), Drawing.WidthRatio, Drawing.HeightRatio, score.ToString(), spriteBatch);
            spriteBatch.End();
            Drawing.resetGraphics();
        }

        /// <summary>
        /// Update the HUD with information about the enemies.
        /// </summary>
        /// <param name="name">Enemy name</param>
        /// <param name="pos">Vector3 position of the enemy</param>
        /// <param name="armour">List of armour values</param>
        public void updateEnemy(string name, Vector3 pos, List<int> armour)
        {
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
                Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(pos.X, pos.Y + 1.2f, pos.Z), gameGraphics.Projection, gameGraphics.View, Matrix.Identity);
                Vector2 screenPosition = new Vector2(projectedPosition.X - xOffset, projectedPosition.Y);

                if (dictionary.ContainsKey(name)) dictionary[name] = newpos;
                else dictionary.Add(name, newpos);

                drawBadVibeHealthString(name, armourString, screenPosition);
            } else {
                int xOffset;

                if (!BadVibe.DRAW_HEALTH_VERTICALLY) {
                    xOffset = ((armour.Count * drumPad.Width) + ((armour.Count - 1) * BadVibe.ARMOUR_SPACING)) / 2;
                } else {
                    xOffset = drumPad.Width / 2;
                }

                Vector2 newpos = new Vector2(500 + pos.X, 200 + pos.Z);
                Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(pos.X, pos.Y + 1.2f, pos.Z), gameGraphics.Projection, gameGraphics.View, Matrix.Identity);
                Vector2 screenPosition = new Vector2(projectedPosition.X - xOffset, projectedPosition.Y);

                if (dictionary.ContainsKey(name)) dictionary[name] = newpos;
                else dictionary.Add(name, newpos);

                drawBadVibeHealth(armour, screenPosition);
            }
        }

        public void drawBadVibeHealth(List<int> arm, Vector2 pos) {
            spriteBatch.Begin();

            int posX = (int) pos.X;
            int posY = (int) pos.Y;

            Color c = new Color();

            // Draw v bars
            if (BadVibe.DRAW_HEALTH_VERTICALLY) {
                int rectX = (int) (posX - ((drumPad.Width * 1.5f) + BadVibe.ARMOUR_SPACING * 2f));
                int rectY = posY - ((arm.Count - 1) * drumPad.Height) - (arm.Count + 1) * BadVibe.ARMOUR_SPACING;
                int rectW = drumPad.Width + BadVibe.ARMOUR_SPACING;
                int rectH = ((arm.Count + 1) * drumPad.Height) + (arm.Count - 1) * BadVibe.ARMOUR_SPACING;

                for (int i = 0; i < 4; i++) {
                    switch (i) {
                        case 0: { c = new Color(0f, 0.25f, 0f, 0.05f); break; }
                        case 1: { c = new Color(0.25f, 0.25f, 0f, 0.05f); break; }
                        case 2: { c = new Color(0f, 0f, 0.25f, 0.05f); break; }
                        case 3: { c = new Color(0.25f, 0f, 0f, 0.05f); break; }
                    }

                    spriteBatch.Draw(block, new Rectangle(rectX, rectY, rectW, rectH), c);

                    rectX += rectW;

                    if (i != 3) {
                        spriteBatch.Draw(block, new Rectangle(rectX, rectY, 1, rectH), Color.Navy);
                    }
                }
            }

            for (int i = 0; i < arm.Count; i++) {
                switch (arm[i]) {
                    case 0 : { c = new Color(1f, 1f, 1f, 1f); break; }
                    case 1 : { c = new Color(0f, 1f, 0f, 1f); break; }
                    case 2 : { c = new Color(1f, 1f, 0f, 1f); break; }
                    case 3 : { c = new Color(0f, 0f, 1f, 1f); break; }
                    case 4 : { c = new Color(1f, 0f, 0f, 1f); break; }
                    case 5 : { c = new Color(1f, 1f, 1f, 1f); break; }
                }

                if (BadVibe.DRAW_HEALTH_VERTICALLY) {
                    posX = (int) pos.X;

                    switch (arm[i]) {
                        case 1: { posX -= (int) ((drumPad.Width * 1.5f) + BadVibe.ARMOUR_SPACING * 1.5f); break; }
                        case 2: { posX -= (int) ((drumPad.Width * 0.5f) + BadVibe.ARMOUR_SPACING * 0.5f); break; }
                        case 3: { posX += (int) ((drumPad.Width * 0.5f) + BadVibe.ARMOUR_SPACING * 0.5f); break; }
                        case 4: { posX += (int) ((drumPad.Width * 1.5f) + BadVibe.ARMOUR_SPACING * 1.5f); break; }
                    }
                }

                if (arm[i] != 0) {
                    spriteBatch.Draw(drumPad, new Rectangle(posX, posY, drumPad.Width, drumPad.Height), c);
                } else {
                    spriteBatch.Draw(rest,    new Rectangle(posX, posY, drumPad.Width, drumPad.Height), c);
                }

                if (!BadVibe.DRAW_HEALTH_VERTICALLY) {
                    posX += drumPad.Width + BadVibe.ARMOUR_SPACING;
                } else {
                    posY -= drumPad.Height + BadVibe.ARMOUR_SPACING;
                }
            }

            spriteBatch.End();
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
            spriteBatch.Begin();
            spriteBatch.DrawString(font, armour, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            Drawing.resetGraphics();
        }

        /// <summary>
        /// Draws the helath bar for the player on screen.
        /// </summary>
        private void drawHealthBar()
        {
            int x = Drawing.pixelsX(10);
            int y = Drawing.pixelsY(10);
            int width = Drawing.pixelsX(healthBar.Width);
            int height = Drawing.pixelsY(healthBar.Height);
            int sliceX = x + Drawing.pixelsX(9);
            int sliceY = y + Drawing.pixelsY(9);
            int sliceWidth = 1;
            int sliceHeight = Drawing.pixelsY(healthSlice.Height);
            int limit = (int)Math.Round((float)Drawing.pixelsX(582) * health);

            float greenValue;
            Color c;

            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);
            for (int i = 0; i < limit; i++)
            {
                greenValue = (float)i / 582;
                float red = (float)(greenValue > 0.5 ? 1 - 2 * (greenValue - 0.5) : 1.0);
                float green = (float)(greenValue > 0.5 ? 1.0 : 2 * greenValue);
                c = new Color(red, green, 0f);

                spriteBatch.Draw(healthSlice, new Rectangle(sliceX+i+1, sliceY+1, sliceWidth, sliceHeight), c);
            }
        }

        /// <summary>
        /// Draws the Mini Map on screen
        /// </summary>
        public void drawMiniMap()
        {
            miniMap.draw(spriteBatch);
        }

        /// <summary>
        /// Updates the HUD with infomation about the player.
        /// </summary>
        /// <param name="h">Players helath</param>
        /// <param name="s">Players score</param>
        public void updateGoodVibe(int h, int s)
        {
            health = (float)h / 100;
            score = s;
        }
    }
}
