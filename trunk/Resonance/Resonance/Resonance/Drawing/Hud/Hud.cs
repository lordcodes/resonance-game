
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

            miniMap = new MiniMap();
        }

        /// <summary>
        /// Called once to load the textures needed for the HUD
        /// </summary>
        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Drawing/Fonts/DebugFont");
            healthBar = Content.Load<Texture2D>("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>("Drawing/HUD/Textures/healthSlice");
            scoreFont = Content.Load<ImportedCustomFont>("Drawing/Fonts/Custom/Score/ScoreFont");
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
        /// Draws bad vibes health above them
        /// </summary>
        /// <param name="name">Name of bad vibe (not displayed datm)</param>
        /// <param name="armour">Armour string to use only atm</param>
        /// <param name="coords">Coords to display the armour levels</param>
        public void drawBadVibeHealth(String name, string armour, Vector2 coords)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, armour, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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

            int xOffset = (int)Math.Round(font.MeasureString(armourString).X/2);

            Vector2 newpos = new Vector2(500 + pos.X , 200+ pos.Z);
            Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(pos.X, pos.Y + 1.8f, pos.Z-0.3f), gameGraphics.Projection, gameGraphics.View, Matrix.Identity);
            Vector2 screenPosition = new Vector2(projectedPosition.X-xOffset, projectedPosition.Y);

            if (dictionary.ContainsKey(name)) dictionary[name] = newpos;
            else dictionary.Add(name, newpos);

            drawBadVibeHealth(name, armourString, screenPosition);
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
                greenValue = (float)i /582;
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
