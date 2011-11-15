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
        private Vector3 position;
        protected int gameModelNum;

        public Object(int modelNum, string name, Game game, Vector3 pos) 
            : base(game)
        {
            position = pos;
            gameModelNum = modelNum;
            identifier = name;
        }

        public string returnIdentifier()
        {
            return identifier;
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (this is DynamicObject)
            {
                Drawing.Draw(gameModelNum, ((DynamicObject)this).Body.WorldTransform);
            }
            else if (this is StaticObject)
            {
                Drawing.Draw(gameModelNum, ((StaticObject)this).Body.WorldTransform.Matrix);
            }
            base.Draw(gameTime);
        }
    }
}
