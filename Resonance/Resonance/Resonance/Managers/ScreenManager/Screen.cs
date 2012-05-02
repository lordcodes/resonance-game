using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
                ((MenuScreen)this).controlMusic(false);
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
