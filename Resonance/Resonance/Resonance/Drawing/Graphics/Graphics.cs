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
        private static ContentManager content;
        private static Shaders shaders;
        private static Matrix world;
        private static Matrix view;
        private static Matrix projection;
        private static Vector3 cameraPosition;
        private static Vector3 cameraCoords;
        private static DisplacementMap dispMap;
        private static VertexDeclaration particleVertexDeclaration;
        private static VertexBuffer particleVertexBuffer;
        private static IndexBuffer particleIndexBuffer;
        private static short[] particleIndices = new short[] { 0, 1, 2, 1, 3, 2 };
        private static VertexPositionNormalTexture[] particleVertices = new VertexPositionNormalTexture[4];
        private static float particleWidth = 0;
        private static float particleHeight = 0;
        private static RasterizerState particleRasterizerState;
        private static float particleHalfHeight = 0;
        private static float particleHalfWidth = 0;
        public static int DISP_WIDTH = 128;

        public DisplacementMap DispMap
        {
            get
            {
                return dispMap;
            }
        }

        public Shaders Shaders
        {
            get
            {
                return shaders;
            }
        }

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

        public Vector3 CameraPosition
        {
            get
            {
                return cameraPosition;
            }
        }

        public Vector3 CameraCoords
        {
            get
            {
                return cameraCoords;
            }
        }

        public Shaders CustomShaders
        {
            get
            {
                return shaders;
            }
        }


        public Graphics(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            content = newContent;
            graphics = newGraphics;

        }

        public void init2dTextures()
        {
            particleHeight = -1;
            particleWidth = -1;

            particleVertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            }
            );

            particleVertexBuffer = new VertexBuffer(
                graphics.GraphicsDevice,
                particleVertexDeclaration,
                4,
                BufferUsage.None
            );

            particleIndexBuffer = new IndexBuffer(graphics.GraphicsDevice,
                IndexElementSize.SixteenBits,
                6,
                BufferUsage.None
                );

            particleIndexBuffer.SetData<short>(particleIndices);
            particleRasterizerState = new RasterizerState();
            particleRasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = particleRasterizerState;

            particleVertices[0] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(0, 1));
            particleVertices[1] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(1, 1));
            particleVertices[2] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(0, 0));
            particleVertices[3] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(1, 0));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public void loadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            view = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
            world = Matrix.Identity;
            shaders = new Shaders();
            shaders.Default.sceneSetup(world, view, projection, Vector3.Zero);
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            dispMap = new DisplacementMap(graphicsDevice, DISP_WIDTH, DISP_WIDTH);
            init2dTextures();

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        public void addWave(Vector2 position)
        {
            if (dispMap != null)
            {
                dispMap.addWave(position);
                dispMap.addHole(position.X, position.Y);
            }
        }

        public void reset()
        {
            if (dispMap != null) dispMap.reset();
            init2dTextures();
        }

        private Vector3 getCamPos(Vector3 newCameraPosition)
        {
            Quaternion orientation = GameScreen.getGV().Body.Orientation;
            Vector3 rotation = Utility.QuaternionToEuler(orientation);
            Vector3 position = GameScreen.getGV().Body.Position;
            Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
            return Vector3.Transform(newCameraPosition, goodVibeRotation) + position;
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void updateCamera(Vector3 newCameraPosition)
        {
            cameraCoords = newCameraPosition;
            cameraPosition = getCamPos(cameraCoords);
            Vector3 position = GameScreen.getGV().Body.Position;
            view = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(Object worldObject, Matrix worldTransform, bool disp, bool drawingReflection, bool drawingShadows)
        {
            drawModel(worldObject, worldTransform, disp,  drawingReflection, drawingShadows);
        }

        public void update(Vector2 playerPos)
        {
            if (dispMap != null) dispMap.update(playerPos);
        }

        public void drawParticle(Texture2D texture, Matrix world, float width, float height, Color colour)
        {
            draw2dTexture(shaders.Particle, texture, world, width, height, colour, false);
        }

        public void drawTexture(Texture2D texture, Matrix world, float width, float height, bool drawingReflection)
        {
            draw2dTexture(shaders.Default, texture, world, width, height, Color.White, drawingReflection);
        }

        private void draw2dTexture(Shader shader, Texture2D texture, Matrix world, float width, float height, Color colour, bool drawingReflection)
        {
            Matrix theView = view;
            Vector3 cameraPosition2 = cameraPosition;
            Matrix projection2 = projection;
            if (drawingReflection)
            {
                float heightDisp = 11.5f;
                float dimension = 16.5f;
                float scale = (float)(10.4 * Math.Pow((cameraPosition.Y), -0.9));
                Vector3 reflecCameraCoords = new Vector3(cameraPosition.X, -heightDisp, cameraPosition.Z + 0.1f);
                cameraPosition2 = reflecCameraCoords;
                theView = Matrix.CreateLookAt(reflecCameraCoords, cameraPosition, Vector3.Up);
                projection2 = Matrix.CreatePerspective(dimension, dimension, 1.0f, 100.0f);
                Matrix heightScale = Matrix.CreateScale(1f, scale, 1f);
                world = Matrix.Multiply(heightScale, world);
            }

            shader.sceneSetup(world, theView, projection2, cameraPosition, texture);
            shader.Technique = "StaticObject";

            if(shader is ParticleShader)
            {
                ((ParticleShader)shader).setColour(colour);
            }

            if (particleHeight != height || particleWidth != width)
            {
                particleHeight = height;
                particleWidth = width;

                particleHalfWidth = width / 2;
                particleHalfHeight = height / 2;

                particleVertices[0].Position.X = -particleHalfWidth;
                particleVertices[0].Position.Y = particleHalfHeight;
                particleVertices[1].Position.X = particleHalfWidth;
                particleVertices[1].Position.Y = particleHalfHeight;
                particleVertices[2].Position.X = -particleHalfWidth;
                particleVertices[2].Position.Y = -particleHalfHeight;
                particleVertices[3].Position.X = particleHalfWidth;
                particleVertices[3].Position.Y = -particleHalfHeight;
                particleVertexBuffer.SetData<VertexPositionNormalTexture>(particleVertices);
                graphics.GraphicsDevice.SetVertexBuffer(particleVertexBuffer);
            }

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass pass in shader.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,particleVertices,0,4,particleIndices,0,2);
            }

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }


        private void drawModel(Object worldObject, Matrix worldTransform, bool disp, bool drawingReflection, bool drawingShadows)
        {
            GameModel gmodel = worldObject.ModelInstance.Model;
            GameModelInstance modelVariables = worldObject.ModelInstance;
            Matrix world = Matrix.Multiply(gmodel.GraphicsScale, worldTransform);
            Model m = gmodel.GraphicsModel;
            Matrix[] modelTransforms = gmodel.ModelTransforms;
            Matrix theView = view;
            Vector3 cameraPosition2 = cameraPosition;
            Matrix projection2 = projection;
            Shader currentShader;

            currentShader = shaders.Default;
            if (Drawing.SHADOWS)
            {
                Matrix lightsViewProjectionMatrix;

                Matrix lightsView = Matrix.CreateLookAt(new Vector3(0,20, 0), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 1, 0));
                //Matrix lightsView = view;
                Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 5f, 1000f);

                lightsViewProjectionMatrix = lightsView * lightsProjection;
                Shaders.Default.Parameters["xLightsWorldViewProjection"].SetValue(world * lightsViewProjectionMatrix);
                Shaders.Ground.Parameters["xLightsWorldViewProjection"].SetValue(world * lightsViewProjectionMatrix);
                Shaders.Ground.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * view * projection);
            }
            if (drawingShadows)
            {
                //Shaders.Default.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
                //Shaders.Ground.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
                DebugDisplay.update("sh", "done");
            }
            else if (drawingReflection)
            {
                float height = 11.5f;
                //float dimension = 12.7f for 0.1 square;
                float dimension = 16.5f;
                float scale = (float)(10.4 * Math.Pow((cameraPosition.Y), -0.9));
                Vector3 reflecCameraCoords = new Vector3(cameraPosition.X, -height, cameraPosition.Z + 0.1f);
                cameraPosition2 = reflecCameraCoords;
                theView = Matrix.CreateLookAt(reflecCameraCoords, cameraPosition, Vector3.Up);
                projection2 = Matrix.CreatePerspective(dimension, dimension, 1.0f, 100.0f);
                Matrix heightScale = Matrix.CreateScale(1f, scale, 1f);
                world = Matrix.Multiply(heightScale, world);
            }
            else if (disp)
            {
                currentShader = shaders.Ground;
                ((GroundShader)currentShader).setDispMap(dispMap.getMap());

                try
                {
                    ((GroundShader)currentShader).GoodVibePos = Drawing.groundPos(GameScreen.getGV().Body.Position, true);
                    ((GroundShader)currentShader).CameraPos = Drawing.groundPos(cameraPosition, true);
                }
                catch (Exception)
                {
                    ((GroundShader)currentShader).GoodVibePos = Vector2.Zero;
                    ((GroundShader)currentShader).CameraPos = Vector2.Zero;
                }
                
            }
            else
            {
                currentShader = shaders.Default;
            }

            int meshCount = 0;
            Texture2D colorTexture = null;
            try
            {
                colorTexture = ((BasicEffect)m.Meshes[0].Effects[0]).Texture;
            }
            catch (Exception)
            {
            }
            if (colorTexture == null) colorTexture = modelVariables.Texture;
            currentShader.sceneSetup(theView, projection2, cameraPosition2, colorTexture);

            foreach (ModelMesh mesh in m.Meshes)
            {
                try
                {
                    colorTexture = ((BasicEffect)m.Meshes[meshCount].Effects[0]).Texture;
                }
                catch (Exception)
                {
                }
                if (colorTexture == null) colorTexture = modelVariables.Texture;
                currentShader.ColourTexture = colorTexture;
                meshCount++;

                currentShader.World = modelTransforms[mesh.ParentBone.Index] * world;
                
                if (gmodel.ModelAnimation)
                {
                    currentShader.Technique = "Animation";
                    ((DefaultShader)currentShader).Bones = modelVariables.Bones;
                }
                else
                {
                    currentShader.Technique = "StaticObject";
                }
                if(drawingShadows)currentShader.Technique = "ShadowMap";
                //else currentShader.Technique = "ShadowedScene";
                
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    foreach (EffectPass pass in currentShader.Passes)
                    {
                        pass.Apply();
                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }
        }
    }
}
