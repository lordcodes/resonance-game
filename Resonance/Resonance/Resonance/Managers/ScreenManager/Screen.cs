using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    abstract class Screen
    {
        ScreenManager screenManager;
        bool loadedUsingLoading = false;

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void HandleInput(InputDevices input) { }

        public void ExitScreen()
        {
            if (this is MenuScreen)
            {
                MenuScreen m = ((MenuScreen)this);
                m.controlMusic(false);
                m.Selected = 0;
            }
            ScreenManager.removeScreen(this);
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }

        public bool LoadedUsingLoading
        {
            get { return loadedUsingLoading; }
            set { loadedUsingLoading = value; }
        }
    }
}
