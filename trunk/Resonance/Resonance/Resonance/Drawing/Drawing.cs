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

        private static SpriteBatch spriteBatch;
        private static SpriteFont font;

        private static Vector3 cameraPosition;

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
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("DebugFont");
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
            UpdateCamera(new Vector4(0, 4f, 6f, (float)(Math.PI*0.25)));
        }

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw()
        {

        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(int gameModelNum, Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            DrawModel(GameModelsStore.getModel(gameModelNum).model, Matrix.Multiply(GameModelsStore.getModel(gameModelNum).scale, worldTransform), effect);
            Vector3 projectedPosition = graphics.GraphicsDevice.Viewport.Project(pos, effect.Projection, effect.View, Matrix.Identity);
            Vector2 screenPosition = new Vector2(projectedPosition.X, projectedPosition.Y-100);
            String health = "";

            drawDebugInfo(worldObject.returnIdentifier(), screenPosition);
            
            if (worldObject.returnIdentifier().Equals("Player")) 
            {
                health = "HEALTH: " + ((GoodVibe)((DynamicObject)worldObject)).GetHealth();
            }
            
            drawDebugInfo("Debug Info\n"+health, new Vector2(20,45)); // This is not very efficient atm
        }

        /// <summary>
        /// Gives the Drawing object information about the game world, atm this is only the player but
        /// this will eventually be given information abut the entire world to be drawn
        /// </summary>
        /// <param name="newPos">(TEMP) Provides a vector to containing X,Y,Z coords of good vibe and its rotation in W</param>
        public static void UpdateCamera(Vector4 newPos)
        {
            goodVibePos = newPos;
            Matrix goodVibeRotation = Matrix.CreateRotationY(goodVibePos.W);
            Vector3 goodVibePosition = new Vector3(goodVibePos.X, goodVibePos.Y, goodVibePos.Z);
            cameraPosition = new Vector3(0, 4f, 6f);
            cameraPosition = Vector3.Transform(cameraPosition, goodVibeRotation) + goodVibePosition;
            effect.View = Matrix.CreateLookAt(cameraPosition, goodVibePosition, Vector3.Up);
            //effect.View = Matrix.CreateLookAt(cameraPosition, goodVibePosition, new Vector3(1,0,0)); // Uncomment this line to make awesome stuff happen when you move. :D
            goodVibeTranslation = Matrix.Multiply(goodVibeRotation, Matrix.CreateTranslation(goodVibePosition));
        }

        private static void drawDebugInfo(String text, Vector2 coords)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, coords, Color.White, 0f,Vector2.Zero,1f,SpriteEffects.None,0f);
            spriteBatch.End();
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
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

        public static GameModels gameModelStore
        {
            get
            {
                return GameModelsStore;
            }
        }

    }
}
