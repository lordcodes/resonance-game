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
        private GraphicsDeviceManager graphics;
        private Model goodVibe;
        private Model ground;
        private Model tree;
        private BasicEffect effect;
        private Texture2D texture;
        private ContentManager Content;
        private Vector4 goodVibePos;
        private Matrix goodVibeTranslation;

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
            ground = Content.Load<Model>("Drawing/Models/ground");
            tree = Content.Load<Model>("Drawing/Models/basicTree");
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            effect.View = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
            effect.LightingEnabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, 0));
            texture = Content.Load<Texture2D>("Drawing/Textures/texMissing");
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.EnableDefaultLighting();
            Update(new Vector4(0, 4f, 6f, (float)(Math.PI*0.25)));
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

        /// <summary>
        /// This is called when the world should be drawn.
        /// </summary>
        public void Draw()
        {
            graphics.GraphicsDevice.Clear(Color.White);
            DrawModel(ground, Matrix.CreateTranslation(0, 0, 0), effect);
            DrawModel(tree, Matrix.CreateTranslation(0, 0, 0), effect);
            DrawModel(goodVibe, goodVibeTranslation, effect);
        }

        /// <summary>
        /// Gives the Drawing object information about the game world, atm this is only the player but
        /// this will eventually be given information abut the entire world to be drawn
        /// </summary>
        /// <param name="newPos">(TEMP) Provides a vector to containing X,Y,Z coords of good vibe and its rotation in W</param>
        public void Update(Vector4 newPos)
        {
            goodVibePos = newPos;
            Matrix goodVibeRotation = Matrix.CreateRotationY(goodVibePos.W);
            Vector3 goodVibePosition = new Vector3(goodVibePos.X, goodVibePos.Y, goodVibePos.Z);
            Vector3 cameraPosition = new Vector3(0, 4f, 6f);
            cameraPosition = Vector3.Transform(cameraPosition, goodVibeRotation)+goodVibePosition;
            effect.View = Matrix.CreateLookAt(cameraPosition, goodVibePosition, Vector3.Up);
            goodVibeTranslation = Matrix.Multiply(goodVibeRotation, Matrix.CreateTranslation(goodVibePosition));
        }

    }
}
