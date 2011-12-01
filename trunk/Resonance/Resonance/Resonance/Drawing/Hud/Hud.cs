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
        //private static float worldWidth = 10;
        private static float screenWidth;
        private static float screenHeight;
        private static double widthRatio;
        private static double heightRatio;
        private static Texture2D healthBar;
        private static Texture2D healthSlice;

        public Hud(ContentManager newContent, GraphicsDeviceManager newGraphics, Graphics newGameGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            gameGraphics = newGameGraphics;
        }

        public void loadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Drawing/Fonts/DebugFont");
            healthBar = Content.Load<Texture2D>("Drawing/HUD/Textures/healthBar");
            healthSlice = Content.Load<Texture2D>("Drawing/HUD/Textures/healthSlice");
            screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            widthRatio =  screenWidth/1920;
            heightRatio =  screenHeight/1080;
        }

        public void drawDebugInfo(String text)
        {
            Vector2 coords = new Vector2(pixelsX(17), pixelsY(80));
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, coords, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            resetGraphics();
        }

        public void drawBadVibeHealth(String name, string armour, Vector2 coords)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, armour, coords, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            resetGraphics();
        }

        public void Draw()
        {
            spriteBatch.Begin();
            /*foreach (KeyValuePair<string, Vector2> pair in dictionary)
            {
                spriteBatch.DrawString(font, "BV", pair.Value, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }*/
            drawHealthBar();
            spriteBatch.End();
            resetGraphics();
        }

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

        private void drawHealthBar()
        {
            int x = pixelsX(10);
            int y = pixelsY(10);
            int width = pixelsX(healthBar.Width);
            int height = pixelsY(healthBar.Height);
            int sliceX = x + pixelsX(9);
            int sliceY = y + pixelsY(9);
            int sliceWidth = 1;
            int sliceHeight = pixelsY(healthSlice.Height);

            int limit = (int)Math.Round((float)pixelsX(582) * health);


            spriteBatch.Draw(healthBar, new Rectangle(x, y, width, height), Color.White);
            for (int i = 0; i < limit; i++)
            {
                spriteBatch.Draw(healthSlice, new Rectangle(sliceX+i, sliceY, sliceWidth, sliceHeight), Color.White);
            }
        }

        private void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private int pixelsX(int input)
        {
            return (int)Math.Round(input * widthRatio);
        }

        private int pixelsY(int input)
        {
            return (int)Math.Round(input * heightRatio);
        }


        public void updateGoodVibe(int h, int s)
        {
            health = (float)h / 100;
            score = s;
        }
    }
}
