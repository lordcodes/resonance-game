using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Shaders
    {
        DefaultShader defaultShader;
        GroundShader groundShader;
        ParticleShader particleShader;

        public Shader Particle
        {
            get
            {
                return particleShader;
            }
        }

        public Shader Default
        {
            get
            {
                return defaultShader;
            }
        }

        public Shader Ground
        {
            get
            {
                return groundShader;
            }
        }

        public void setAmbientLight(Vector3 light)
        {
            Default.AmbientLightColour = light;
            Ground.AmbientLightColour = light;
        }

        public Shaders()
        {
            defaultShader = new DefaultShader("Drawing/Shaders/Default");
            groundShader = new GroundShader("Drawing/Shaders/Ground");
            particleShader = new ParticleShader("Drawing/Shaders/Particle");

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

            Ground.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            Ground.Parameters["LightDirection"].SetValue(-lightDirection);
            Ground.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            Ground.Parameters["LightDirection2"].SetValue(-lightDirection2);
            Ground.Parameters["DiffuseLightColor2"].SetValue(diffuseLightColor2);
            Ground.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            Ground.Parameters["SpecularColorPower"].SetValue(specularColorPower);
            Ground.Parameters["DiffuseColor"].SetValue(diffuseColor);
            Ground.Parameters["groundSize"].SetValue(World.MAP_X);

            Particle.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            Particle.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            Particle.Parameters["DiffuseColor"].SetValue(diffuseColor);
        }
    }
}
