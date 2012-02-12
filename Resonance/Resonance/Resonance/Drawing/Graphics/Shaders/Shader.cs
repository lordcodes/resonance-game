using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Shader
    {
        private Effect customEffect;
        private Texture currentTexture;
        private Matrix currentWorld;
        private Matrix currentView;
        private Matrix currentProjection;
        private Vector3 currentAmbientLight;
        private Vector3 currentCameraPosition;

        public Texture ColourTexture
        {
            set
            {
                if (currentTexture != value)
                {
                    currentTexture = value;
                    Effect.Parameters["ColorTexture"].SetValue(value);
                }
            }
        }

        public Matrix World
        {
            set
            {
                if (value != currentWorld)
                {
                    currentWorld = value;
                    Effect.Parameters["World"].SetValue(value);
                }
            }
        }

        public Matrix View
        {
            set
            {
                if (value != currentView)
                {
                    currentView = value;
                    Effect.Parameters["View"].SetValue(value);
                }
            }
        }

        public Matrix Projection
        {
            set
            {
                if (value != currentProjection)
                {
                    currentProjection= value;
                    Effect.Parameters["Projection"].SetValue(value);
                }
            }
        }

        public Vector3 AmbientLightColour
        {
            set
            {
                if (value != currentAmbientLight)
                {
                    currentAmbientLight = value;
                    Effect.Parameters["AmbientLightColor"].SetValue(value);
                }
            }
        }

        public Vector3 CameraPosition
        {
            set
            {
                if (value != currentCameraPosition)
                {
                    currentCameraPosition = value;
                    Effect.Parameters["CameraPosition"].SetValue(value);
                }
            }
        }

        public Effect Effect
        {
            get
            {
                return customEffect;
            }
        }

        public EffectTechniqueCollection Techniques
        {
            get
            {
                return Effect.Techniques;
            }
        }

        public EffectParameterCollection Parameters
        {
            get
            {
                return Effect.Parameters;
            }
        }

        public EffectPassCollection Passes
        {
            get
            {
                return Effect.CurrentTechnique.Passes;
            }
        }

        public void applyTechnique(EffectTechnique technique)
        {
            Effect.CurrentTechnique = technique;
        }

        public void applyPass(int pass)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public void sceneSetup(Matrix view, Matrix projection, Vector3 camera)
        {
            View = view;
            Projection = projection;
            CameraPosition = camera;
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera)
        {
            World = world;
            sceneSetup(view, projection, camera);
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            ColourTexture = texture;
            sceneSetup(world, view, projection, camera);
        }

        public void sceneSetup(Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            ColourTexture = texture;
            sceneSetup(view, projection, camera);
        }

        public Shader(string file)
        {
            customEffect = Drawing.Content.Load<Effect>(file);
        }
    }
}
