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

        public Matrix World
        {
            set
            {
                Effect.Parameters["World"].SetValue(value);
            }
        }

        public Vector3 AmbientLightColour
        {
            set
            {
                Effect.Parameters["AmbientLightColor"].SetValue(value);
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
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["CameraPosition"].SetValue(camera);
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera)
        {
            Effect.Parameters["World"].SetValue(world);
            sceneSetup(view, projection, camera);
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            Effect.Parameters["ColorTexture"].SetValue(texture);
            sceneSetup(world, view, projection, camera);
        }

        public void sceneSetup(Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            Effect.Parameters["ColorTexture"].SetValue(texture);
            sceneSetup(view, projection, camera);
        }

        public Shader(string file)
        {
            customEffect = Drawing.Content.Load<Effect>(file);
        }
    }
}
