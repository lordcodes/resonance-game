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
        private Model GraphicsModel;
        private Model PhysicsModel;
        private Matrix GraphicsScale;
        private Matrix PhysicsScale;
        private float Mass;

        public Model graphicsModel
        {
            get
            {
                return GraphicsModel;
            }
        }

        public Model physicsModel
        {
            get
            {
                if(PhysicsModel == null)return GraphicsModel;
                return PhysicsModel;
            }
        }

        public Matrix physicsScale
        {
            get
            {
                return PhysicsScale;
            }
        }

        public Matrix graphicsScale
        {
            get
            {
                return GraphicsScale;
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
        /// <param name="graphicsModelFile">The name of the graphics model file to load</param>
        /// <param name="graphicsModelScale">The scale as a float for the grphics model</param>
        /// <param name="physicsModelFile">The name of the physics model file to load</param>
        /// <param name="physicsModelScale">The scale as a float for the physics model</param>
        public GameModel(ContentManager Content, String graphicsModelFile, float graphicsModelScale, String physicsModelFile, float physicsModelScale)
        {
            GraphicsModel = Content.Load<Model>(graphicsModelFile);
            GraphicsScale = Matrix.CreateScale(graphicsModelScale, graphicsModelScale, graphicsModelScale);

            if(!graphicsModelFile.Equals(physicsModelFile))PhysicsModel = Content.Load<Model>(physicsModelFile);
            PhysicsScale = Matrix.CreateScale(physicsModelScale, physicsModelScale, physicsModelScale);

            Mass = 10f;
        }

        /// <summary>
        /// Creates a new GameModel object and stores everything needed to draw the object atm includes
        /// model data and scale. Will eventualy include texture data
        /// Only use this for very simple meshes as comples shapes will slow down the physics engine. 
        /// </summary>
        /// <param name="Content">Pass it the content manager to load textures</param>
        /// <param name="graphicsModelFile">The name of the graphics model file to load</param>
        /// <param name="graphicsModelScale">The scale as a float for the grphics model</param>
        public GameModel(ContentManager Content, String graphicsModelFile, float graphicsModelScale) : this(Content, graphicsModelFile, graphicsModelScale, graphicsModelFile, graphicsModelScale){}
        

    }
}
