using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class CreditsScreen : Screen
    {
        List<Texture2D> images;
        List<string> names;

        public CreditsScreen()
            : base()
        {
            images = new List<Texture2D>(8);
            names = new List<string>(8);
            names.Add("Alex Sheppard");
            names.Add("Andrew Lord");
            names.Add("Michael Jones");
            names.Add("Mihai Nemes");
            names.Add("Philip Tattersall");
            names.Add("Thomas Pickering");
            names.Add("Geoffrey Birch");
            names.Add("Paul Keast");
        }

        public override void LoadContent() 
        {
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
            images.Add(ScreenManager.Content.Load<Texture2D>("Drawing/Textures/CreditsPlaceholder"));
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void HandleInput(InputDevices input) 
        { 
        }
    }
}
