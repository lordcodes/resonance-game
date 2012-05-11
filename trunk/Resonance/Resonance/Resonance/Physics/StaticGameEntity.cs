using BEPUphysics.CollisionShapes;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;

namespace Resonance
{
    class StaticGameEntity : GameEntity
    {
        InstancedMeshShape shape;

        public StaticGameEntity(int modelNum)
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
            shape = new InstancedMeshShape(vertices, indices);
        }

        public InstancedMeshShape Shape
        {
            get { return shape; }
        }
    }
}
