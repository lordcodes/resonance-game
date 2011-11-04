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
    class Drawing
    {
        GraphicsDeviceManager graphics;
        Model goodVibe;
        BasicEffect effect;
        Texture2D texture;
        ContentManager Content;

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public Drawing(ContentManager newContent, GraphicsDeviceManager newGraphics)
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
            goodVibe = Content.Load<Model>("Drawing/Models/box");
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            effect.View = Matrix.CreateLookAt(new Vector3(0, 8, 22), Vector3.Zero, Vector3.Up);
            effect.LightingEnabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, 0));
            texture = Content.Load<Texture2D>("Drawing/Textures/texBadVibe");
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.EnableDefaultLighting();
        }

        private Matrix GetParentTransform(Model m, ModelBone mb)
        {
            return (mb == m.Root) ? mb.Transform :
                mb.Transform * GetParentTransform(m, mb.Parent);
        }

        private void DrawModel(Model m, Matrix world, BasicEffect be)
        {
            foreach (ModelMesh mm in goodVibe.Meshes)
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

        /// <summary>
        /// This is called when the world should be drawn.
        /// </summary>
        public void Draw()
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            const int NumberSquares = 10;
            for (int i = 0; i < NumberSquares; i++)
            {
                Matrix world = Matrix.CreateTranslation(((i%5)-2)*4,0,(i/5-3)*-4);
                DrawModel(goodVibe, world, effect);
            }
        }

    }
}
