using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;

namespace Resonance
{
    class DynamicGameEntity : GameEntity
    {
        ConvexHullShape shape;
        Entity entity;

        public DynamicGameEntity(int modelNum)
            : base()
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(GameModels.getModel(modelNum).PhysicsModel, out vertices, out indices);
            float scaleFactor = GameModels.getModel(modelNum).PhysicsScale.M11;
            Vector3 scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Multiply(vertices[i], scale);
            }
            shape = new ConvexHullShape(vertices);
            entity = new Entity(shape, 1f);
        }

        public ConvexHullShape Shape
        {
            get { return shape; }
        }

        public float Mass
        {
            get { return entity.Mass; }
        }

        public float Volume
        {
            get { return entity.Volume; }
        }

        public Matrix3X3 InertiaTensor
        {
            get { return entity.LocalInertiaTensor; }
        }
    }
}
