using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class Graphics
    {
        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;
        private static Effect customEffect;
        private static Matrix world;
        private static Matrix view;
        private static Matrix projection;

        Vector3 ambientLightColor;
        Vector3 diffuseColor;

        Vector3 lightDirection;
        Vector3 diffuseLightColor;
        Vector3 lightDirection2;
        Vector3 diffuseLightColor2;

        Vector4 specularColorPower;
        Vector3 specularLightColor;
        Vector3 cameraPosition;

        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        public Matrix View
        {
            get
            {
                return view;
            }
        }

        public Graphics(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public void loadContent()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            view = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
            world = Matrix.Identity;

            ambientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            diffuseColor = new Vector3(0.3f, 0.3f, 0.3f);

            diffuseLightColor = new Vector3(0.9f, 0.9f, 0.9f);
            lightDirection = new Vector3(0.5f, -0.5f, 0.6f);
            lightDirection.Normalize();

            diffuseLightColor2 = new Vector3(0.4f, 0.35f, 0.4f);
            lightDirection2 = new Vector3(0.45f, -0.8f, 0.45f);
            lightDirection2.Normalize();

            specularColorPower = new Vector4(1, 1, 1, 128.0f);

            specularLightColor = new Vector3(0.15f, 0.15f, 0.15f);


            customEffect = Content.Load<Effect>("Drawing/Effect");
            customEffect.Parameters["World"].SetValue(Matrix.Identity);
            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void UpdateCamera(GoodVibe player)
        {
            Quaternion orientation = player.Body.Orientation;
            Vector3 rotation = DynamicObject.QuaternionToEuler(orientation);
            Vector3 position = player.Body.Position;
            Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
            Vector3 defaultCameraPosition = new Vector3(0, 4f, 6f);
            cameraPosition = Vector3.Transform(defaultCameraPosition, goodVibeRotation) + position;
            view = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(int gameModelNum, Matrix worldTransform)
        {
            DrawModel(GameModels.getModel(gameModelNum), Matrix.Multiply(GameModels.getModel(gameModelNum).GraphicsScale, worldTransform));
        }


        private void DrawModel(ImportedGameModel gmodel, Matrix world)
        {
            Model m = gmodel.GraphicsModel;
            Matrix[] modelTransforms = gmodel.ModelTransforms;
            Texture2D colorTexture = ((BasicEffect)m.Meshes[0].Effects[0]).Texture;

            if (colorTexture == null) colorTexture = gmodel.Texture;

            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            customEffect.Parameters["ColorTexture"].SetValue(colorTexture);
            customEffect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            customEffect.Parameters["LightDirection"].SetValue(-lightDirection);
            customEffect.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            customEffect.Parameters["LightDirection2"].SetValue(-lightDirection2);
            customEffect.Parameters["DiffuseLightColor2"].SetValue(diffuseLightColor2);
            customEffect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            customEffect.Parameters["CameraPosition"].SetValue(cameraPosition);
            customEffect.Parameters["SpecularColorPower"].SetValue(specularColorPower);

            graphics.GraphicsDevice.Textures[0] = colorTexture;

            foreach (ModelMesh mesh in m.Meshes)
            {
                customEffect.Parameters["World"].SetValue(modelTransforms[mesh.ParentBone.Index] * world);

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    customEffect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                    customEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }
        }

    }
}
