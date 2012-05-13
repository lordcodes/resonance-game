using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class Graphics
    {
        private static GraphicsDeviceManager graphics;
        private static ContentManager content;
        private static Shaders shaders;
        private static Matrix world;
        private static Matrix projection;
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
        public void loadContent(ImportedGameModels importedModels, ContentManager content, GraphicsDevice graphicsDevice)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, GraphicsSettings.DRAW_DISTANCE);
            world = Matrix.Identity;
            shaders = new Shaders();
            shaders.Default.sceneSetup(world, CameraMotionManager.Camera.View, projection, Vector3.Zero);
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            dispMap = LoadedContent.getDisp(importedModels, graphics.GraphicsDevice);
            init2dTextures();

            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
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
            Matrix theView = CameraMotionManager.Camera.View;
            Vector3 cameraPosition = CameraMotionManager.Camera.Position;
            Matrix projection2 = projection;
            if (drawingReflection)
            {
                float heightDisp = 11.5f;
                float dimension = 16.5f;
                float scale = (float)(10.4 * Math.Pow((cameraPosition.Y), -0.9));
                Vector3 reflecCameraCoords = new Vector3(cameraPosition.X, -heightDisp, cameraPosition.Z + 0.1f);
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

            shader.Transparency = 1;
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;

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

            EffectPassCollection passes = shader.Passes;
            for(int i = 0; i < passes.Count; i++)
            {
                passes[i].Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,particleVertices,0,4,particleIndices,0,2);
            }

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }


        public void Draw(Object worldObject, Matrix worldTransform, bool disp, bool drawingReflection, bool drawingShadows)
        {
            GameModel gmodel = worldObject.ModelInstance.Model;
            GameModelInstance modelVariables = worldObject.ModelInstance;
            Matrix world = Matrix.Multiply(gmodel.GraphicsScale, worldTransform);
            Model m = gmodel.GraphicsModel;
            Matrix[] modelTransforms = gmodel.ModelTransforms;
            Matrix theView = CameraMotionManager.Camera.View;
            Vector3 cameraPosition = CameraMotionManager.Camera.Position;
            Vector3 cameraPosition2 = CameraMotionManager.Camera.Position;
            Matrix projection2 = projection;
            Shader currentShader;
            Matrix lightsViewProjectionMatrix;

            Vector3 pos = GameScreen.getGV().Body.Position;
            Vector3 lightPos = new Vector3(cameraPosition.X, 50f, cameraPosition.Z);
            Matrix lightsView = Matrix.CreateLookAt(lightPos, new Vector3(pos.X, 0, pos.Z), new Vector3(0, 1, 0));
            Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 25f,1000f);

            lightsViewProjectionMatrix = lightsView * lightsProjection;


            currentShader = shaders.Default;
            currentShader.Transparency = modelVariables.Transparency;
            if (drawingShadows)
            {
                Shaders.Default.Parameters["xLightsWorldViewProjection"].SetValue(world * lightsViewProjectionMatrix);
                Shaders.Ground.Parameters["xLightsWorldViewProjection"].SetValue(world * lightsViewProjectionMatrix);
                Shaders.Ground.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * lightsView * projection);
            }
            else if (drawingReflection)
            {
                float height = 11.5f;
                float dimension = 16.5f * height;
                float scale = (float)(10.463 * Math.Pow((cameraPosition.Y), -0.9));
                Vector3 reflecCameraCoords = new Vector3(cameraPosition.X, -height, cameraPosition.Z + 0.1f);
                cameraPosition2 = reflecCameraCoords;
                theView = Matrix.CreateLookAt(reflecCameraCoords, cameraPosition, Vector3.Up);
                projection2 = Matrix.CreatePerspective(dimension, dimension, height, 100.0f);
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


                currentShader.Parameters["xWorldViewProjection"].SetValue(world * theView * projection);
                currentShader.Parameters["xLightsWorldViewProjection"].SetValue(world * lightsViewProjectionMatrix);
                currentShader.Parameters["xLightPos"].SetValue(lightPos);
            }
            else
            {
                currentShader = shaders.Default;
                currentShader.Transparency = modelVariables.Transparency;
            }

            Texture2D colorTexture = null;
            currentShader.sceneSetup(theView, projection2, cameraPosition2, colorTexture);

            ModelMeshCollection meshes = m.Meshes;
            for(int i = 0; i < meshes.Count; i++)
            {
                ModelMesh mesh = meshes[i];
                try
                {
                    colorTexture = ((BasicEffect)m.Meshes[i].Effects[0]).Texture;
                }
                catch (Exception){}
                if (colorTexture == null) colorTexture = modelVariables.Texture;
                currentShader.ColourTexture = colorTexture;

                currentShader.World = modelTransforms[mesh.ParentBone.Index] * world;
                
                if (gmodel.ModelAnimation)
                {
                    currentShader.Technique = "Animation";
                    ((DefaultShader)currentShader).Bones = modelVariables.Bones;
                    if (drawingShadows) currentShader.Technique = "AnimationShadow";
                }
                else
                {
                    currentShader.Technique = "StaticObject";
                    if(drawingShadows)currentShader.Technique = "ShadowMap";
                }

                ModelMeshPartCollection meshParts = mesh.MeshParts;
                for(int j = 0; j < meshParts.Count; j++)
                {
                    ModelMeshPart meshPart = meshParts[j];
                    graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    EffectPassCollection passes = currentShader.Passes;
                    for(int k = 0; k < passes.Count; k++)
                    {
                        passes[k].Apply();
                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }
        }
    }
}
