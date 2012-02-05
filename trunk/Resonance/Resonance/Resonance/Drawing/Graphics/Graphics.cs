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

        // Reduce this variable if the shockwave is causing frame rate to suffer
        public static int DISP_WIDTH = 32;

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


        public Graphics(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            content = newContent;
            graphics = newGraphics;
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
        }

        public void addWave(Vector2 position)
        {
            if (dispMap != null) dispMap.addWave(position);
        }

        public void reset()
        {
            if (dispMap != null) dispMap.reset();
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void updateCamera(Vector3 newCameraPosition)
        {
            cameraCoords = newCameraPosition;
            Quaternion orientation = Game.getGV().Body.Orientation;
            Vector3 rotation = Utility.QuaternionToEuler(orientation);
            Vector3 position = Game.getGV().Body.Position;
            Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
            cameraPosition = Vector3.Transform(cameraCoords, goodVibeRotation) + position;
            view = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(Object worldObject, Matrix worldTransform, bool disp, bool drawingReflection)
        {
            drawModel(worldObject, worldTransform, disp,  drawingReflection);
        }

        public void update(Vector2 playerPos)
        {
            if (dispMap != null) dispMap.update(playerPos);
        }

        public void drawParticle(Texture2D texture, Matrix world, float width, float height, Color colour)
        {
            draw2dTexture(shaders.Particle, texture, world, width, height, colour);
        }

        public void drawTexture(Texture2D texture, Matrix world, float width, float height)
        {
            draw2dTexture(shaders.Default, texture, world, width, height, Color.White);
        }

        private void draw2dTexture(Shader shader, Texture2D texture, Matrix world, float width, float height, Color colour)
        {
            shader.sceneSetup(world, view, projection, cameraPosition, texture);
            shader.applyTechnique(shader.Techniques["StaticObject"]);

            int number_of_vertices = 4;
            int number_of_indices = 6;
            VertexDeclaration vertexDeclaration;
            VertexPositionNormalTexture[] vertices;
            VertexBuffer vertexBuffer;
            short[] indices;
            IndexBuffer indexBuffer;

            if(shader is ParticleShader)
            {
                ((ParticleShader)shader).setColour(colour);
            }

            vertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            }
            );


            float halfWidth = width / 2;
            float halfHeight = height / 2;
            Vector3 topLeft = new Vector3(-halfWidth, halfHeight, 0);
            Vector3 bottomLeft = new Vector3(-halfWidth, -halfHeight, 0);
            Vector3 topRight = new Vector3(halfWidth,halfHeight, 0);
            Vector3 bottomRight = new Vector3(halfWidth, -halfHeight, 0);

            vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(topLeft, Vector3.Zero, new Vector2(0, 1));
            vertices[1] = new VertexPositionNormalTexture(topRight, Vector3.Zero, new Vector2(1,1));
            vertices[2] = new VertexPositionNormalTexture(bottomLeft, Vector3.Zero, new Vector2(0, 0));
            vertices[3] = new VertexPositionNormalTexture(bottomRight, Vector3.Zero, new Vector2(1, 0));

            indices = new short[] {  0,  1,  2,
                                     1,  3,  2};

            vertexBuffer = new VertexBuffer(
                graphics.GraphicsDevice,
                vertexDeclaration,
                number_of_vertices,
                BufferUsage.None
            );

            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            indexBuffer = new IndexBuffer(graphics.GraphicsDevice,
                IndexElementSize.SixteenBits,
                number_of_indices,
                BufferUsage.None
                );

            indexBuffer.SetData<short>(indices);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //graphics.GraphicsDevice.BlendState = BlendState.Additive;

            foreach (EffectPass pass in shader.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,vertices,0,4,indices,0,2);
            }

            //graphics.GraphicsDevice.BlendState = BlendState.Opaque;
        }


        private void drawModel(Object worldObject, Matrix worldTransform, bool disp, bool drawingReflection)
        {
            //graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GameModel gmodel = worldObject.ModelInstance.Model;
            GameModelInstance modelVariables = worldObject.ModelInstance;
            Matrix world = Matrix.Multiply(gmodel.GraphicsScale, worldTransform);
            Model m = gmodel.GraphicsModel;
            Matrix[] modelTransforms = gmodel.ModelTransforms;
            Matrix theView = view;
            Vector3 cameraPosition2 = cameraPosition;
            Matrix projection2 = projection;
            Shader currentShader;

            if (drawingReflection)
            {
                Vector3 cameraCoords = new Vector3(0, -10f, 0.1f);
                Quaternion orientation = Game.getGV().Body.Orientation;
                Vector3 rotation = Vector3.Zero;
                Vector3 position = Game.getGV().Body.Position;
                Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
                theView = Matrix.CreateLookAt(cameraCoords, Vector3.Zero, Vector3.Down);
                float plus = -2.0f;
                projection2 = Matrix.CreateOrthographic(World.MAP_X+plus, World.MAP_Z+plus, 1.0f, 1000.0f);
            }


            if (disp)
            {
                currentShader = shaders.Ground;
                ((GroundShader)currentShader).setDispMap(dispMap.getMap());

                try
                {
                    Vector2 pos = Drawing.groundPos(Game.getGV().Body.Position, true);
                    ((GroundShader)currentShader).GoodVibePos = pos;
                }
                catch (Exception)
                {
                    ((GroundShader)currentShader).GoodVibePos = Vector2.Zero;
                }
            }
            else
            {
                currentShader = shaders.Default;
            }

            Texture2D colorTexture = ((BasicEffect)m.Meshes[0].Effects[0]).Texture;
            if (colorTexture == null) colorTexture = modelVariables.Texture;
            currentShader.sceneSetup(theView, projection2, cameraPosition2, colorTexture);

            foreach (ModelMesh mesh in m.Meshes)
            {
                currentShader.World = modelTransforms[mesh.ParentBone.Index] * world;

                if (gmodel.ModelAnimation)
                {
                    currentShader.applyTechnique(currentShader.Techniques["Animation"]);
                    Matrix[] bones = modelVariables.Bones;
                    ((DefaultShader)currentShader).Bones = bones;
                }
                else
                {
                    currentShader.applyTechnique(currentShader.Techniques["StaticObject"]);
                }

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
