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
        protected int gameModelNum;
        private Vector3 position;

        private Game gameRef;

        public Object(int modelNum, string name, Game game, Vector3 pos) 
            : base(game)
        {
            gameModelNum = modelNum;
            identifier = name;
            position = pos;

            gameRef = game;
        }

        public string returnIdentifier()
        {
            return identifier;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this is DynamicObject)
            {
                Drawing.Draw(gameModelNum, ((DynamicObject)this).Body.WorldTransform, ((DynamicObject)this).Body.Position, this);
            }
            else if (this is StaticObject)
            {
                Drawing.Draw(gameModelNum, ((StaticObject)this).Body.WorldTransform.Matrix, position, this);
            }
            else if (this is Shockwave)
            {
                Drawing.Draw(gameModelNum, ((Shockwave)this).Transform, ((Shockwave)this).Position, this);
            }

            base.Draw(gameTime);
        }
    }
}
