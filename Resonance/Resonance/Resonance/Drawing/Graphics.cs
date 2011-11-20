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
        private static Texture2D colorTexture;
        private static Matrix world;
        private static Matrix view;
        private static Matrix projection;

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
            colorTexture = Content.Load<Texture2D>("Drawing/Textures/texMissing");
            customEffect = Content.Load<Effect>("Drawing/Effect");
            customEffect.Parameters["World"].SetValue(Matrix.Identity);
            customEffect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.Up));
            customEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f));
            customEffect.Parameters["ColorTexture"].SetValue(colorTexture);
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphics.GraphicsDevice.Textures[0] = colorTexture;
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
            Vector3 cameraPosition = new Vector3(0, 4f, 6f);
            cameraPosition = Vector3.Transform(cameraPosition, goodVibeRotation) + position;
            view = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(int gameModelNum, Matrix worldTransform)
        {
            DrawModel(GameModels.getModel(gameModelNum), Matrix.Multiply(GameModels.getModel(gameModelNum).graphicsScale, worldTransform));
        }


        private void DrawModel(GameModel gmodel, Matrix world)
        {
            Model m = gmodel.graphicsModel;
            Matrix[] modelTransforms = gmodel.modelTransforms;
            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);

            foreach (ModelMesh mesh in m.Meshes)
            {
                customEffect.Parameters["World"].SetValue(modelTransforms[mesh.ParentBone.Index] * world);

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    customEffect.Parameters["ColorTexture"].SetValue(colorTexture);
                    customEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }
        }

    }
}
