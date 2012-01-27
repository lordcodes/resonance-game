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

        private static Vector3 ambientLightColor;
        private static Vector3 diffuseColor;

        private static Vector3 lightDirection;
        private static Vector3 diffuseLightColor;
        private static Vector3 lightDirection2;
        private static Vector3 diffuseLightColor2;

        private static Vector4 specularColorPower;
        private static Vector3 specularLightColor;
        private static Vector3 cameraPosition;

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

        public Graphics(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
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

            dispMap = new DisplacementMap(graphicsDevice, DISP_WIDTH, DISP_WIDTH);

            //bumpMap.SetData
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
        public void UpdateCamera(Vector3 point, Vector3 newCameraPosition)
        {
            cameraPosition = newCameraPosition;
            Quaternion orientation = Game.getGV().Body.Orientation;
            Vector3 rotation = DynamicObject.QuaternionToEuler(orientation);
            Vector3 position = Game.getGV().Body.Position;
            Matrix goodVibeRotation = Matrix.CreateRotationY(rotation.Y);
            cameraPosition = Vector3.Transform(cameraPosition, goodVibeRotation) + position;
            view = Matrix.CreateLookAt(cameraPosition, position, Vector3.Up);
        }

        public void Draw(int gameModelNum, Matrix worldTransform, bool disp)
        {
            DrawModel(GameModels.getModel(gameModelNum), Matrix.Multiply(GameModels.getModel(gameModelNum).GraphicsScale, worldTransform), disp,gameModelNum);
        }

        public void update(Vector2 playerPos)
        {
            if (dispMap != null) dispMap.update(playerPos);
        }

        public void Draw2dTextures(Texture2D texture, Matrix world, float width, float height)
        {
            customEffect.Parameters["World"].SetValue(world);
            customEffect.Parameters["doDisp"].SetValue(false);
            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            customEffect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            customEffect.Parameters["LightDirection"].SetValue(-lightDirection);
            customEffect.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            customEffect.Parameters["LightDirection2"].SetValue(-lightDirection2);
            customEffect.Parameters["DiffuseLightColor2"].SetValue(diffuseLightColor2);
            customEffect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            customEffect.Parameters["CameraPosition"].SetValue(cameraPosition);
            customEffect.Parameters["SpecularColorPower"].SetValue(specularColorPower);
            customEffect.Parameters["ColorTexture"].SetValue(texture);
            graphics.GraphicsDevice.Textures[0] = texture;
            customEffect.CurrentTechnique.Passes[0].Apply();
            customEffect.CurrentTechnique = customEffect.Techniques["Technique1"];

            int number_of_vertices = 4;
            int number_of_indices = 6;
            VertexDeclaration vertexDeclaration;
            VertexPositionNormalTexture[] vertices;
            VertexBuffer vertexBuffer;
            short[] indices;
            IndexBuffer indexBuffer;

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

            graphics.GraphicsDevice.BlendState = BlendState.Additive;

            foreach (EffectPass pass in customEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                        PrimitiveType.TriangleList,
                        vertices,
                        0,   // vertex buffer offset to add to each element of the index buffer
                        4,  // number of vertices to draw
                        indices,
                        0,   // first index element to read
                        2   // number of primitives to draw
                    );
            }


            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
        }


        private void DrawModel(ImportedGameModel gmodel, Matrix world, bool disp, int gameModelNum)
        {
            Model m = gmodel.GraphicsModel;
            Matrix[] modelTransforms = gmodel.ModelTransforms;
            Matrix[] bones = Program.game.animationPlayer.GetSkinTransforms();

            customEffect.Parameters["doDisp"].SetValue(disp);
            if (disp) customEffect.Parameters["DispMap"].SetValue(dispMap.getMap());
            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            customEffect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            customEffect.Parameters["LightDirection"].SetValue(-lightDirection);
            customEffect.Parameters["DiffuseLightColor"].SetValue(diffuseLightColor);
            customEffect.Parameters["LightDirection2"].SetValue(-lightDirection2);
            customEffect.Parameters["DiffuseLightColor2"].SetValue(diffuseLightColor2);
            customEffect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            customEffect.Parameters["CameraPosition"].SetValue(cameraPosition);
            customEffect.Parameters["SpecularColorPower"].SetValue(specularColorPower);

            Texture2D colorTexture = ((BasicEffect)m.Meshes[0].Effects[0]).Texture;
            if (colorTexture == null) colorTexture = gmodel.Texture;
            customEffect.Parameters["ColorTexture"].SetValue(colorTexture);
            graphics.GraphicsDevice.Textures[0] = colorTexture;

            foreach (ModelMesh mesh in m.Meshes)
            {
                customEffect.Parameters["World"].SetValue(modelTransforms[mesh.ParentBone.Index] * world);

                if (GameModels.getModel(gameModelNum).Animation)
                {
                    customEffect.CurrentTechnique = customEffect.Techniques["Animation"];
                    customEffect.Parameters["xBones"].SetValue(bones);
                }
                else
                {
                    customEffect.CurrentTechnique = customEffect.Techniques["StaticObject"];
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    customEffect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                    foreach (EffectPass pass in customEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }

            }
        }

    }
}
