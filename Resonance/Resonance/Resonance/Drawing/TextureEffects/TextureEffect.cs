using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    public class TextureEffect : DrawableGameComponent
    {
        private Vector3 position;
        private bool faceCamera;
        private TextureAnimation textureAnimation;
        private float width;
        private float height;

        public List<Texture2D> Textures
        {
            set
            {
                textureAnimation.Textures = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return textureAnimation.Texture;
            }

            set
            {
                textureAnimation.setTexture(0, value);
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
        }

        public Matrix Position
        {
            get
            {
                return getPosition(CameraMotionManager.Camera.Position);
            }
        }

        public Matrix getPosition(Vector3 camera)
        {
            camera.Y = position.Y;
            return Matrix.Invert(Matrix.CreateLookAt(position,camera,Vector3.Down));
        }

        public TextureEffect(float width, float height, Vector3 position, bool faceCamera, TextureAnimation textureAnimation)
            : base(Program.game)
        {
            this.position = position;
            this.faceCamera = faceCamera;
            this.textureAnimation = textureAnimation;
            this.width = width;
            this.height = height;
            DrawableManager.Add(this);
        }

        public override void Draw(GameTime gameTime)
        {
            //Drawing code moved to DrawableManager
        }

        public override void Update(GameTime gameTime)
        {
            textureAnimation.Update(gameTime);
        }
    }
}
