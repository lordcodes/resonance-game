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
        private static BasicEffect effect;
        private static Texture2D texture;
        private static ContentManager Content;

        public Matrix Projection
        {
            get
            {
                return effect.Projection;
            }
        }

        public Matrix View
        {
            get
            {
                return effect.View;
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
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            effect.View = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
            effect.LightingEnabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, 0));
            texture = Content.Load<Texture2D>("Drawing/Textures/texMissing");
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.EnableDefaultLighting();
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
            effect.View = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(int gameModelNum, Matrix worldTransform)
        {
            DrawModel(GameModels.getModel(gameModelNum).graphicsModel, Matrix.Multiply(GameModels.getModel(gameModelNum).graphicsScale, worldTransform), effect);
        }

        private Matrix GetParentTransform(Model m, ModelBone mb)
        {
            return (mb == m.Root) ? mb.Transform :
                mb.Transform * GetParentTransform(m, mb.Parent);
        }

        private void DrawModel(Model m, Matrix world, BasicEffect be)
        {
            foreach (ModelMesh mm in m.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    be.World = GetParentTransform(m, mm.ParentBone) * world;
                    graphics.GraphicsDevice.SetVertexBuffer(mmp.VertexBuffer, mmp.VertexOffset);
                    graphics.GraphicsDevice.Indices = mmp.IndexBuffer;
                    be.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mmp.NumVertices, mmp.StartIndex, mmp.PrimitiveCount);
                }
            }
        }

        private void DrawGameModel(int model, Vector3 pos)
        {
            DrawModel(GameModels.getModel(model).graphicsModel, Matrix.Multiply(GameModels.getModel(model).graphicsScale, Matrix.CreateTranslation(pos)), effect);
        }

    }
}
