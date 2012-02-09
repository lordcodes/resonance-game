using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;

namespace Resonance
{
    /// <summary>
    /// Class that maintains the GameComponents that need to be Updated and or Drawn
    /// </summary>
    class DrawableManager
    {
        private static HashSet<GameComponent> components = new HashSet<GameComponent>();

        /// <summary>
        /// Add a game component
        /// </summary>
        /// <param name="component">The GameComponent you wish to add</param>
        public static void Add(GameComponent component)
        {
            components.Add(component);
        }

        /// <summary>
        /// Draws all the currently stored GameComponents
        /// </summary>
        /// <param name="time">The gameTime</param>
        public static void Draw(GameTime time)
        {
            foreach (GameComponent component in components)
            {
                if (component is DrawableGameComponent) ((DrawableGameComponent)component).Draw(time);
            }
        }

        /// <summary>
        /// Updates all the currently stored GameComponents
        /// </summary>
        /// <param name="time">The gameTime</param>
        public static void Update(GameTime time)
        {
            foreach (GameComponent component in components)
            {
                try
                {
                    component.Update(time);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Remove a GameComponent so it is no longer Updated/Drawn
        /// </summary>
        /// <param name="component"></param>
        public static void Remove(GameComponent component)
        {
            components.Remove(component);
        }

        /// <summary>
        /// Remove all GameComponenets
        /// </summary>
        public static void Clear()
        {
            components.Clear();
        }
    }
}
