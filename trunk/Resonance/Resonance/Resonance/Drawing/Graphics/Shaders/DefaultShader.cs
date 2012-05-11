using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class DefaultShader : Shader
    {
        public Matrix[] currentBones = null;
        public DefaultShader(string file) : base(file, true) { }

        public Matrix[] Bones
        {
            set
            {
                if(value != currentBones)
                {
                    currentBones = value;
                    Effect.Parameters["xBones"].SetValue(value);
                }
            }
        }

    }
}
