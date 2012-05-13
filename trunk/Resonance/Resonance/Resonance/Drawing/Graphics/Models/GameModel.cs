using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    /// <summary>
    /// Class that stores information about the graphics and physics models that are 
    /// the same for each instance of the model that appears in the game world. 
    /// GameModelInstance stores information that is unique to each use of the GameModel
    /// in the world, such as the animation details.
    /// </summary>
    public class GameModel
    {
        private Model graphicsModel;
        private Model physicsModel;
        private Matrix graphicsScale;
        private Matrix physicsScale;
        private Matrix[] modelTransforms;
        bool modelAnimation;
        private float mass;
        private TextureAnimation textureAnimation;

        public TextureAnimation TextureAnimation
        {
            get
            {
                return textureAnimation;
            }
        }

        public Matrix[] ModelTransforms
        {
            get
            {
                return modelTransforms;
            }
        }

        public Model GraphicsModel
        {
            get
            {
                return graphicsModel;
            }
        }

        public Model PhysicsModel
        {
            get
            {
                if (physicsModel == null) return graphicsModel;
                return physicsModel;
            }
        }

        public Matrix PhysicsScale
        {
            get
            {
                return physicsScale;
            }
        }

        public Matrix GraphicsScale
        {
            get
            {
                return graphicsScale;
            }
        }

        public float Mass
        {
            get
            {
                return mass;
            }
        }

        public bool ModelAnimation
        {
            get
            {
                return modelAnimation;
            }
        }

        public GameModel(Model newGraphicsModel, Matrix graphicsModelScale, Model newPhysicsModel, Matrix physicsModelScale, TextureAnimation textureAnimation, bool modelAnimation)
        {
            graphicsModel = newGraphicsModel;
            physicsModel = newPhysicsModel;
            modelTransforms = new Matrix[GraphicsModel.Bones.Count];
            GraphicsModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
            graphicsScale = graphicsModelScale;
            physicsScale = physicsModelScale;
            this.modelAnimation = modelAnimation;
            this.textureAnimation = textureAnimation;
            mass = 1f;
        }

    }
}
