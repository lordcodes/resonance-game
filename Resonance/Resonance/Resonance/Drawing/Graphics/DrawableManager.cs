using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class DrawableManager
    {
        private static Dictionary<GameComponent, bool> components = new Dictionary<GameComponent, bool>();

        public static void Add(GameComponent component)
        {
            components.Add(component, true);
        }

        public static void Draw(GameTime time)
        {
            foreach (KeyValuePair<GameComponent, bool> pair in components)
            {
                if (pair.Key is DrawableGameComponent) ((DrawableGameComponent)pair.Key).Draw(time);
            }
        }

        public static void Update(GameTime time)
        {
            foreach (KeyValuePair<GameComponent, bool> pair in components)
            {
                try
                {
                    pair.Key.Update(time);
                }
                catch (Exception) { }
            }
        }

        public static void Remove(GameComponent component)
        {
            components.Remove(component);
        }
    }
}
