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

        public CustomFont(ContentManager c)
        {
            Content = c;
        }

        public void load()
        {
            ImportedCustomFont font = Content.Load<ImportedCustomFont>("Drawing/Fonts/Custom/Score/ScoreFont");
        }
    }
}
