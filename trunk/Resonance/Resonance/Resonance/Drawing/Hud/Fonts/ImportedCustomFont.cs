using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class ImportedCustomFont
    {
        private Dictionary<char, Texture2D> dictionary = new Dictionary<char, Texture2D>();


        public void add(char character, Texture2D image)
        {
            dictionary.Add(character, image);
        }

        public void drawLeft(int x, int y, double widthRatio, double heightRatio, string text, SpriteBatch spriteBatch)
        {
            int width;
            int height;
            Texture2D image;
            x = x - (int)Math.Round(getWidth(text)*widthRatio);
            for (int i = 0; i < text.Length; i++)
            {
                image = getImage(text[i]);
                if(image != null)
                {
                    width = (int)Math.Round(image.Width * widthRatio);
                    height = (int)Math.Round(image.Height * widthRatio);
                    spriteBatch.Draw(image, new Rectangle(x, y, width, height), Color.White);
                    x += width;
                }
            }
        }

        public void drawCentre(int x, int y, double widthRatio, double heightRatio, string text, SpriteBatch spriteBatch)
        {
            int width;
            int height;
            Texture2D image;
            x = x - (int)Math.Round(getWidth(text) * widthRatio) / 2;
            for (int i = 0; i < text.Length; i++)
            {
                image = getImage(text[i]);
                if (image != null)
                {
                    width = (int)Math.Round(image.Width * widthRatio);
                    height = (int)Math.Round(image.Height * widthRatio);
                    spriteBatch.Draw(image, new Rectangle(x, y, width, height), Color.White);
                    x += width;
                }
            }
        }

        private int getWidth(string text)
        {
            int width = 0;
            Texture2D image;
            for (int i = 0; i < text.Length; i++)
            {
                image = getImage(text[i]);
                if (image != null)
                {
                    width += image.Width;
                }
            }
            return width;
        }

        private Texture2D getImage(char character)
        {
            if(dictionary.ContainsKey(character)) return dictionary[character];

            return null;
        }
    }
}
