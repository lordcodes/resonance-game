using BEPUphysics.Collidables;
using Microsoft.Xna.Framework;
using BEPUphysics.MathExtensions;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace Resonance
{
    class StaticObject : Object
    {
        InstancedMesh body;

        public StaticObject(int modelNum, string name, Vector3 pos, Quaternion o) 
            : base(modelNum, name, pos)
        {
            bool isAdded = GameEntities.isAdded(modelNum);

            if (!isAdded)
            {
                GameEntities.addEntity(modelNum, false);
            }
            StaticGameEntity entity = (StaticGameEntity)GameEntities.getEntity(modelNum);
            body = new InstancedMesh(entity.Shape, new AffineTransform(o, pos));
            body.Sidedness = TriangleSidedness.Counterclockwise;
            body.Tag = name;
        }

        public StaticObject(int modelNum, string name, Vector3 pos)
            : base(modelNum, name, pos)
        {
            bool isAdded = GameEntities.isAdded(modelNum);

            if (!isAdded)
            {
                GameEntities.addEntity(modelNum, false);
            }
            StaticGameEntity entity = (StaticGameEntity)GameEntities.getEntity(modelNum);
            body = new InstancedMesh(entity.Shape, new AffineTransform(pos));
            body.Sidedness = TriangleSidedness.Counterclockwise;
            body.Tag = name;
        }

        public InstancedMesh Body
        {
            get
            {
                return body;
            }
        }
    }
}
