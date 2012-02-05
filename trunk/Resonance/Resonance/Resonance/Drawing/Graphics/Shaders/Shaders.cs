using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class Shaders
    {
        Shader defaultShader;
        Shader particleShader;

        public Shader Default
        {
            get
            {
                return defaultShader;
            }
        }

        public Shaders()
        {
            defaultShader = new Shader("Drawing/Shaders/Default");
        }
    }
}
