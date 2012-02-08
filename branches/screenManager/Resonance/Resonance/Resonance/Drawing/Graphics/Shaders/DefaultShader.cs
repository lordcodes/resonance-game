using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class DefaultShader : Shader
    {

        public DefaultShader(string file) : base(file) { }

        public Matrix[] Bones
        {
            set
            {
                Effect.Parameters["xBones"].SetValue(value);
            }
        }

    }
}
