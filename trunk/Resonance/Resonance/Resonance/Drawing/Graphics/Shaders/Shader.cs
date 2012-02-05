using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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

        public Shader(string file)
        {
            customEffect = Drawing.Content.Load<Effect>(file);
        }
    }
}
