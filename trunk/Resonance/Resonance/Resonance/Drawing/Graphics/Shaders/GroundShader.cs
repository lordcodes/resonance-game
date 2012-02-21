using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class GroundShader : Shader
    {

        public GroundShader(string file) : base(file, true) { }

        public Vector2 GoodVibePos
        {
            set
            {
                Effect.Parameters["gvPos"].SetValue(value);
            }
        }

        public Vector2 CameraPos
        {
            set
            {
                Effect.Parameters["camPos"].SetValue(value);
            }
        }

        public void setDispMap(Texture2D map)
        {
            Effect.Parameters["DispMap"].SetValue(map);
        }

        public void setReflectionTexture(Texture2D map)
        {
            Effect.Parameters["ReflectionTexture"].SetValue(map);
        }

    }
}
