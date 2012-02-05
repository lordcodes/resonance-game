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

        public EffectTechniqueCollection Techniques
        {
            get
            {
                return customEffect.Techniques;
            }
        }

        public EffectParameterCollection Parameters
        {
            get
            {
                return customEffect.Parameters;
            }
        }

        public EffectPassCollection Passes
        {
            get
            {
                return customEffect.CurrentTechnique.Passes;
            }
        }

        public void applyTechnique(EffectTechnique technique)
        {
            customEffect.CurrentTechnique = technique;
        }

        public void applyPass(int pass)
        {
            customEffect.CurrentTechnique.Passes[0].Apply();
        }

        public void sceneSetup(Matrix view, Matrix projection, Vector3 camera)
        {
            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            customEffect.Parameters["CameraPosition"].SetValue(camera);
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera)
        {
            customEffect.Parameters["World"].SetValue(world);
            sceneSetup(view, projection, camera);
        }

        public void sceneSetup(Matrix world, Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            customEffect.Parameters["ColorTexture"].SetValue(texture);
            sceneSetup(world, view, projection, camera);
        }

        public void sceneSetup(Matrix view, Matrix projection, Vector3 camera, Texture2D texture)
        {
            customEffect.Parameters["ColorTexture"].SetValue(texture);
            sceneSetup(view, projection, camera);
        }

        public Shader(string file)
        {
            customEffect = Drawing.Content.Load<Effect>(file);
        }
    }
}
