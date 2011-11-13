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
        private static GraphicsDeviceManager graphics;
        private static GameModels GameModelsStore;
        private static BasicEffect effect;
        private static Texture2D texture;
        private static ContentManager Content;
        private static Vector4 goodVibePos;
        private static Matrix goodVibeTranslation;

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            GameModelsStore = new GameModels(Content);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            GameModelsStore.Load();
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

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw()
        {
            //graphics.GraphicsDevice.Clear(Color.White);
            // This will eventually be looping through the World object grabbing objects
            DrawGameModel(GameModels.GROUND, Vector3.Zero);
            //DrawGameModel(GameModels.TREE, Vector3.Zero);
            //DrawGameModel(GameModels.MUSHROOM, new Vector3(4,2,2));
            // Special case atm for drawing the good vibe:
            DrawModel(GameModelsStore.getModel(GameModels.GOOD_VIBE).model, Matrix.Multiply(GameModelsStore.getModel(GameModels.GOOD_VIBE).scale, goodVibeTranslation), effect);
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(int gameModelNum, Matrix worldTransform)
        {
            DrawModel(GameModelsStore.getModel(gameModelNum).model, Matrix.Multiply(GameModelsStore.getModel(gameModelNum).scale, worldTransform), effect);
        }

        /// <summary>
        /// Gives the Drawing object information about the game world, atm this is only the player but
        /// this will eventually be given information abut the entire world to be drawn
        /// </summary>
        /// <param name="newPos">(TEMP) Provides a vector to containing X,Y,Z coords of good vibe and its rotation in W</param>
        public static void Update(Vector4 newPos)
        {
            goodVibePos = newPos;
            Matrix goodVibeRotation = Matrix.CreateRotationY(goodVibePos.W);
            Vector3 goodVibePosition = new Vector3(goodVibePos.X, goodVibePos.Y, goodVibePos.Z);
            Vector3 cameraPosition = new Vector3(0, 4f, 6f);
            cameraPosition = Vector3.Transform(cameraPosition, goodVibeRotation) + goodVibePosition;
            effect.View = Matrix.CreateLookAt(cameraPosition, goodVibePosition, Vector3.Up);
            goodVibeTranslation = Matrix.Multiply(goodVibeRotation, Matrix.CreateTranslation(goodVibePosition));
        }

        private static Matrix GetParentTransform(Model m, ModelBone mb)
        {
            return (mb == m.Root) ? mb.Transform :
                mb.Transform * GetParentTransform(m, mb.Parent);
        }

        private static void DrawModel(Model m, Matrix world, BasicEffect be)
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

        private static void DrawGameModel(int model, Vector3 pos)
        {
            DrawModel(GameModelsStore.getModel(model).model, Matrix.Multiply(GameModelsStore.getModel(model).scale, Matrix.CreateTranslation(pos)), effect);
        }

    }
}
