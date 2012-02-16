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
        ScreenState screenState = ScreenState.TO_ON;
        bool loadedUsingLoading = false;
        bool exiting = false;
        float transition = 1f;

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
            ScreenManager.removeScreen(this);
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }

        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }

        public bool LoadedUsingLoading
        {
            get { return loadedUsingLoading; }
            set { loadedUsingLoading = value; }
        }

        public bool Exiting
        {
            get { return exiting; }
            set { exiting = value; }
        }

        public float Transition
        {
            get { return transition; }
            set { transition = value; }
        }
    }

    public enum ScreenState { TO_ON, ACTIVE, TO_OFF, HIDDEN };
}
