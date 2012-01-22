using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    class CustomFont
    {
        Dictionary<char, TextureContent> dictionary = new Dictionary<char, TextureContent>();

        public Dictionary<char, TextureContent> Dictionary
        {
            get
            {
                return dictionary;
            }
        }

        public void add(char name, TextureContent image)
        {
            dictionary.Add(name, image);
        }
    }
}
