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
            set
            {
                originalPosition = value;
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
            // Code that was here is now done in DrawableManager 
        }
    }
}
