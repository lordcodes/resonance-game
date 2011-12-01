using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Resonance
{
    class CustomFont
    {
        ContentManager Content;
        String file;
        //Texture2D sheet;

        public CustomFont(ContentManager c, String f)
        {
            Content = c;
            file = f;
        }

        public void load()
        {
            //Texture2D sheet = Content.Load<Texture2D>(file);
        }
    }
}
