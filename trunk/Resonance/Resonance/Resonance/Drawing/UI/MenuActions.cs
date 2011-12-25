using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    /// <summary>
    /// Static class used to hold all the functions used by the menu. i.e the functions that are called whenever a menu option is selected
    /// </summary>
    class MenuActions
    {
        private static int genNumber = 0;

        /// <summary>
        /// Called once to allo this class access to the rest of the game
        /// </summary>
        /// <param name="nGame">Supply the game object so the functions can acces the game</param>
        public static void init()
        {
        }

        /// <summary>
        /// Give the player character full health.
        /// </summary>
        public static void fullHealth()
        {
            ((GoodVibe)Program.game.World.getObject("Player")).fullHealth();
            UI.play();
        }

        /// <summary>
        /// Kill all the bad vibes in the world
        /// </summary>
        public static void killBadVibes()
        {
            Dictionary<string, Object> objects = Program.game.World.returnObjects();
            foreach (var entry in objects)
            {
                if (entry.Value is BadVibe)
                {
                    ((BadVibe)entry.Value).kill();
                }
            }
            UI.play();
        }

        /// <summary>
        /// Add a BadVibe to the world
        /// </summary>
        public static void addBadVibe()
        {
            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + genNumber, new Vector3(0, 1, -2));
            Program.game.World.addObject(bv);
            genNumber++;
            UI.play();
        }

        /// <summary>
        /// Exit the game
        /// </summary>
        public static void exit()
        {
            Program.game.Exit();
        }

        /// <summary>
        /// Exit the menu and resume the game
        /// </summary>
        public static void resume()
        {
            UI.play();
        }

        /// <summary>
        /// Load a level
        /// </summary>
        /// <param name="level">Number representing level, int corresponds the the name of the level e.g Level1.xml</param>
        public static void loadLevel(int level)
        {
            Loading.load(delegate { Program.game.loadLevel(level); }, "Level " + level);
        }

        /// <summary>
        /// Resets the level to objects original position. Does not relod the level XML and re-create the objects.
        /// </summary>
        public static void softReset()
        {
            Program.game.World.reset();
            UI.play();
        }

    }
}
