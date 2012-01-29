using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;
using BEPUphysics;

namespace Resonance
{
    class Object : DrawableGameComponent
    {
        private string identifier;
        private GameModelInstance modelInstance;
        private Vector3 originalPosition;

        public Vector3 OriginalPosition
        {
            get
            {
                return originalPosition;
            }
        }

        public GameModelInstance ModelInstance
        {
            get
            {
                return modelInstance;
            }
            set
            {
                modelInstance = value;
            }
        }

        public Object(int modelNum, string name, Vector3 pos) 
            : base(Program.game)
        {
            identifier = name;
            originalPosition = pos;
            modelInstance = new GameModelInstance(modelNum);
        }

        public string returnIdentifier()
        {
            return identifier;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this is DynamicObject)
            {
                Drawing.Draw(((DynamicObject)this).Body.WorldTransform, ((DynamicObject)this).Body.Position, this);
            }
            else if (this is StaticObject)
            {
                //Drawing.Draw(gameModelNum, ((StaticObject)this).Body.WorldTransform.Matrix, ((StaticObject)this).Body.WorldTransform.Translation, this);
                Drawing.Draw(((StaticObject)this).Body.WorldTransform.Matrix, ((StaticObject)this).Body.WorldTransform.Translation, this);
            }
            else if (this is Shockwave)
            {
                Drawing.Draw(((Shockwave)this).Transform, ((Shockwave)this).Position, this);
            }

            base.Draw(gameTime);
        }
    }
}
