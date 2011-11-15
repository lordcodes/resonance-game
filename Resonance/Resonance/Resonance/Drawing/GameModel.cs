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
    class GameModel
    {
        private Model Model;
        private Matrix Scale;
        private float Mass;

        public Model model
        {
            get
            {
                return Model;
            }
        }

        public Matrix scale
        {
            get
            {
                return Scale;
            }
        }

        public float mass
        {
            get
            {
                return Mass;
            }
        }

        /// <summary>
        /// Creates a new GameModel object and stores everything needed to draw the object atm includes
        /// model data and scale. Will eventualy include texture data 
        /// </summary>
        /// <param name="Content">Pass it the content manager to load textures</param>
        /// <param name="modelFile">The name of the model files to load</param>
        /// <param name="modelScale">The scale as a float for the model</param>
        public GameModel(ContentManager Content, String modelFile, float modelScale)
        {
            Model = Content.Load<Model>(modelFile);
            Scale = Matrix.CreateScale(modelScale, modelScale, modelScale);
            Mass = 1.0f;
        }
        
    }
}
