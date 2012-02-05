using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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

            Vector3 ambientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 diffuseColor = new Vector3(0.3f, 0.3f, 0.3f);
            Vector3 lightDirection = new Vector3(0.5f, -0.5f, 0.6f);
            Vector3 diffuseLightColor = new Vector3(0.9f, 0.9f, 0.9f);
            Vector3 lightDirection2 = new Vector3(0.45f, -0.8f, 0.45f);
            Vector3 diffuseLightColor2 = new Vector3(0.4f, 0.35f, 0.4f);
            Vector4 specularColorPower = new Vector4(1, 1, 1, 128.0f);
            Vector3 specularLightColor = new Vector3(0.15f, 0.15f, 0.15f);

            Default.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            Default.Parameters["LightDirection"].SetValue(-lightDirection);
            Default.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            Default.Parameters["LightDirection2"].SetValue(-lightDirection2);
            Default.Parameters["DiffuseLightColor2"].SetValue(diffuseLightColor2);
            Default.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            Default.Parameters["SpecularColorPower"].SetValue(specularColorPower);
            Default.Parameters["DiffuseColor"].SetValue(diffuseColor);
        }
    }
}
