using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance
{
    /// <summary>
    /// Class that maintains the GameComponents that need to be Updated and or Drawn
    /// </summary>
    class DrawableManager
    {
        private static List<GameComponent> components = new List<GameComponent>(1000);
        private static List<GameComponent> pickupComponents = new List<GameComponent>(1000);
        private static List<GameComponent> goodVibeComponents = new List<GameComponent>(1000);
        private static List<GameComponent> groundComponents = new List<GameComponent>(1000);
        private static List<GameComponent> wallComponents = new List<GameComponent>(1000);

        // Lists below store duplicate refrences so i dont have to re check at draw time
        private static List<GameComponent> shadowedComponents = new List<GameComponent>(1000);
        private static List<GameComponent> reflectedComponents = new List<GameComponent>(1000);

        static Profile ThisSection = Profile.Get("Drawing3D");

        /// <summary>
        /// Add a game component
        /// </summary>
        /// <param name="component">The GameComponent you wish to add</param>
        public static void Add(GameComponent component)
        {
            if (component is Pickup || component is TextureEffect)
            {
                pickupComponents.Add(component);
                reflectedComponents.Add(component);
            }
            else if (component is GoodVibe)
            {
                goodVibeComponents.Add(component);
                reflectedComponents.Add(component);
            }
            else if (component is Object && (
                (((Object)component).returnIdentifier().Equals("Ground")) || 
                ((Object)component).returnIdentifier().Equals("Walls")))
            {
                if(((Object)component).returnIdentifier().Equals("Walls"))
                {
                    ((Object)component).ModelInstance.Shadow = false;
                    wallComponents.Add(component);
                }
                else
                {
                    groundComponents.Add(component);
                    reflectedComponents.Add(component);
                }
            }
            else
            {
                components.Add(component);
                reflectedComponents.Add(component);
            }

            if (component is Object && ((Object)component).ModelInstance.Shadow)
            {
                shadowedComponents.Add(component);
            }
        }

        public static void DrawObjects(GameTime gameTime)
        {
            Drawing.Clear();
            Drawing.blendOn();
            DrawSet(groundComponents, gameTime);
            Drawing.blendOff();
            DrawSet(wallComponents, gameTime);
            DrawSet(components, gameTime);
            DrawSet(goodVibeComponents, gameTime);
            DrawSet(pickupComponents, gameTime);
            DebugDisplay.update("DrawableObjects", components.Count + pickupComponents.Count + goodVibeComponents.Count + "");
        }

        public static void DrawShadowedObjects(GameTime gameTime)
        {
            Drawing.Clear();
            Drawing.setShadows();
            DrawSet(shadowedComponents, gameTime);
            Drawing.unsetShadows();
        }

        public static void DrawReflectedObjects(GameTime gameTime)
        {
            Drawing.Clear();
            DrawSet(reflectedComponents, gameTime);
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
                            Matrix r1 = Matrix.CreateRotationZ(GVMotionManager.BANK_ANGLE);
                            Matrix r2 = Matrix.CreateRotationX(GVMotionManager.PITCH_ANGLE);
                            Matrix r = Matrix.Multiply(r1, r2);
                            Matrix t = Matrix.Multiply(r, ((DynamicObject)component).Body.WorldTransform);
                            Drawing.Draw(t, ((DynamicObject)component).Body.Position, (Object)component);
                            // Draw the shield if it is up
                            if (((GoodVibe)component).ShieldOn)
                            {
                                Drawing.Draw(((DynamicObject)component).Body.WorldTransform, ((DynamicObject)component).Body.Position, ((GoodVibe)component).ShieldObject);
                            }
                        } else if (component is Bullet) {
                            Drawing.Draw(((Bullet)component).getTransform(), ((Bullet)component).Position, (Object)component);
                        }
                        else
                        {
                            Drawing.Draw(((DynamicObject)component).Body.WorldTransform, ((DynamicObject)component).Body.Position, (Object)component);
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
            UpdateSet(groundComponents, time);
            UpdateSet(components, time);
            UpdateSet(pickupComponents, time);
            UpdateSet(goodVibeComponents, time);
            UpdateSet(wallComponents, time);
        }

        /// <summary>
        /// Remove a GameComponent so it is no longer Updated/Drawn
        /// </summary>
        /// <param name="component"></param>
        public static void Remove(GameComponent component)
        {
            if (pickupComponents.Contains(component))
            {
                pickupComponents.Remove(component);
            }
            else if (components.Contains(component))
            {
                components.Remove(component);
            }
            else if (goodVibeComponents.Contains(component))
            {
                goodVibeComponents.Remove(component);
            }
            else if (groundComponents.Contains(component))
            {
                groundComponents.Remove(component);
            }
            else if (wallComponents.Contains(component))
            {
                wallComponents.Remove(component);
            }
            
            if (shadowedComponents.Contains(component))
            {
                shadowedComponents.Remove(component);
            }

            if (reflectedComponents.Contains(component))
            {
                reflectedComponents.Remove(component);
            }
        }

        /// <summary>
        /// Remove all GameComponenets
        /// </summary>
        public static void Clear()
        {
            components.Clear();
            pickupComponents.Clear();
            goodVibeComponents.Clear();
            groundComponents.Clear();
            shadowedComponents.Clear();
            reflectedComponents.Clear();
            DebugDisplay.clear();
        }
    }
}
