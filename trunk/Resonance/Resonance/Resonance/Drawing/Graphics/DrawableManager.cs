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
        private static HashSet<GameComponent> componentsSecondary = new HashSet<GameComponent>();
        static Profile ThisSection = Profile.Get("Drawing3D");

        /// <summary>
        /// Add a game component
        /// </summary>
        /// <param name="component">The GameComponent you wish to add</param>
        public static void Add(GameComponent component)
        {
            if (component is TextureEffect)
            {
                componentsSecondary.Add(component);
            }
            else
            {
                components.Add(component);
            }
        }

        /// <summary>
        /// Draws all the currently stored GameComponents
        /// </summary>
        /// <param name="time">The gameTime</param>
        public static void Draw(GameTime time)
        {
            using (IDisposable d = ThisSection.Measure())
            {
                DrawSet(components, time);
                DrawSet(componentsSecondary, time);
            }
        }

        private static void DrawSet(HashSet<GameComponent> components, GameTime time)
        {
            foreach (GameComponent component in components)
            {
                if (component is DrawableGameComponent)
                {
                    if (component is DynamicObject)
                    {
                        Drawing.Draw(((DynamicObject)component).Body.WorldTransform, ((DynamicObject)component).Body.Position, (Object)component);
                    }
                    else if (component is StaticObject)
                    {
                        Drawing.Draw(((StaticObject)component).Body.WorldTransform.Matrix, ((StaticObject)component).Body.WorldTransform.Translation, (Object)component);
                    }
                    else if (component is Shockwave)
                    {
                        Drawing.Draw(((Shockwave)component).Transform, ((Shockwave)component).Position, (Object)component);
                    }
                }
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

            foreach (GameComponent component in componentsSecondary)
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
            if (components.Contains(component))
            {
                components.Remove(component);
            }
            else
            {
                componentsSecondary.Remove(component);
            }
        }

        /// <summary>
        /// Remove all GameComponenets
        /// </summary>
        public static void Clear()
        {
            components.Clear();
            componentsSecondary.Clear();
            DebugDisplay.clear();
        }
    }
}
