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
    interface DrawingInterface
    {

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        void loadContent();

        /// <summary>
        /// This is called when the world should be drawn.
        /// </summary>
        void Draw();

        /// <summary>
        /// Gives the Drawing object information about the game world, atm this is only the player but
        /// this will eventually be given information abut the entire world to be drawn
        /// </summary>
        /// <param name="newPos">(TEMP) Provides a vector to containing X,Y,Z coords of good vibe and its rotation in W</param>
        void Update(Vector4 newPos);
    }
}
