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
        private static List<GameComponent> components = new List<GameComponent>(1000);
        private static List<GameComponent> componentsSecondary = new List<GameComponent>(1000);
        private static List<GameComponent> componentsTertiary = new List<GameComponent>(1000);
        static Profile ThisSection = Profile.Get("Drawing3D");

        /// <summary>
        /// Add a game component
        /// </summary>
        /// <param name="component">The GameComponent you wish to add</param>
        public static void Add(GameComponent component)
        {
            if (component is GoodVibe)
            {
                componentsTertiary.Add(component);
            }
            else if (component is TextureEffect)
            {
                componentsSecondary.Add(component);
            }
            else
            {
                components.Add(component);
            }
        }

        public static void DrawObjects(GameTime gameTime)
        {
            Drawing.Clear();
            DrawSet(components, gameTime);
            DrawSet(componentsSecondary, gameTime);
            DrawSet(componentsTertiary, gameTime);
        }

        /// <summary>
        /// Draws all the currently stored GameComponents
        /// </summary>
        /// <param name="gameTime">The gameTime</param>
        public static void Draw(GameTime gameTime)
        {
            using (IDisposable d = ThisSection.Measure())
            {
                // Draw reflections
                if (GraphicsSettings.FLOOR_REFLECTIONS)
                {
                    Drawing.renderReflections(gameTime);
                }
                if (GraphicsSettings.FLOOR_SHADOWS)
                {
                    Drawing.renderShadows(gameTime);
                }

                DrawObjects(gameTime);
            }
        }

        private static void DrawSet(List<GameComponent> components, GameTime time)
        {
            for (int i = 0; i < components.Count; i++)
            {
                GameComponent component = components[i];
                if (component is DrawableGameComponent)
                {
                    if (component is DynamicObject)
                    {
                        if (component is GoodVibe) {
                            Matrix r = Matrix.CreateRotationZ(GVMotionManager.BANK_ANGLE);
                            Matrix t = Matrix.Multiply(r, ((DynamicObject)component).Body.WorldTransform);
                            Drawing.Draw(t, ((DynamicObject)component).Body.Position, (Object)component);
                        } else {
                            Drawing.Draw(((DynamicObject)component).Body.WorldTransform, ((DynamicObject)component).Body.Position, (Object)component);
                        }
                        // Draw the shield if it is up
                        if (component is GoodVibe && ((GoodVibe)component).ShieldOn)
                        {
                            Drawing.Draw(((DynamicObject)component).Body.WorldTransform, ((DynamicObject)component).Body.Position, ((GoodVibe)component).ShieldObject);
                        }
                    }
                    else if (component is StaticObject)
                    {
                        Drawing.Draw(((StaticObject)component).Body.WorldTransform.Matrix, ((StaticObject)component).Body.WorldTransform.Translation, (Object)component);
                    }
                    else if (component is Shockwave)
                    {
                        Drawing.Draw(((Shockwave)component).Transform, ((Shockwave)component).Position, (Object)component);
                    }
                    else if (component is Checkpoint)
                    {
                        Checkpoint c = (Checkpoint)component;
                        Drawing.Draw(c.Transform, c.Position, (Object)component);
                    }
                    else if (component is TextureEffect)
                    {
                        Drawing.DrawTexture(((TextureEffect)component).Texture, ((TextureEffect)component).Position, ((TextureEffect)component).Width, ((TextureEffect)component).Height);
                    }
                }
            }
        }

        private static void UpdateSet(List<GameComponent> components, GameTime time)
        {
            for (int i = 0; i < components.Count; i++)
            {
                try
                {
                    components[i].Update(time);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Updates all the currently stored GameComponents
        /// </summary>
        /// <param name="time">The gameTime</param>
        public static void Update(GameTime time)
        {
            UpdateSet(components, time);
            UpdateSet(componentsSecondary, time);
            UpdateSet(componentsTertiary, time);
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
            componentsTertiary.Clear();
            DebugDisplay.clear();
        }
    }
}
