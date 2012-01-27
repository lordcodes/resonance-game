using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
delegate void ItemDelegate();

namespace Resonance
{
    class UI
    {
        public static bool paused = false;
        private static int selected = 0;
        private static List<MenuItem> currentMenu;
        private static List<MenuItem> pauseMenu = new List<MenuItem>();
        private static List<MenuItem> levelsMenu = new List<MenuItem>();

        /// <summary>
        /// Returns true of the UI has paused the game
        /// </summary>
        public static bool Paused
        {
            get
            {
                return paused;
            }
        }

        /// <summary>
        /// Called once to initialise the static class
        /// </summary>
        /// <param name="game">Supply the Game object so the UI can effect it</param>
        public static void init()
        {
            MenuActions.init();

            // Populate the menus, that are represented as lists of MenuItems
            // Refer to the comments in the MenuItem class for how to create them
            levelsMenu.Add(new MenuItem("Load Level 1", new ItemDelegate(delegate { MenuActions.loadLevel(1); })));
            levelsMenu.Add(new MenuItem("Load Level 2", new ItemDelegate(delegate { MenuActions.loadLevel(2); })));
            levelsMenu.Add(new MenuItem("Back", pauseMenu));

            pauseMenu.Add(new MenuItem("Resume", new ItemDelegate(MenuActions.resume)));
            pauseMenu.Add(new MenuItem("Exit Game", new ItemDelegate(MenuActions.exit)));
            pauseMenu.Add(new MenuItem("Add Bad Vibe (by the tree)", new ItemDelegate(MenuActions.addBadVibe)));
            pauseMenu.Add(new MenuItem("Reset object positions", new ItemDelegate(MenuActions.softReset)));
            pauseMenu.Add(new MenuItem("Kill all the Bad Vibes", new ItemDelegate(MenuActions.killBadVibes)));
            pauseMenu.Add(new MenuItem("Give Good Vibe full health", new ItemDelegate(MenuActions.fullHealth)));
            pauseMenu.Add(new MenuItem("Load a Level", levelsMenu));

            currentMenu = pauseMenu;
        }

        /// <summary>
        /// Function to pause the game
        /// </summary>
        public static void pause()
        {
            paused = true;
        }

        /// <summary>
        /// Function to play the game
        /// </summary>
        public static void play()
        {
            paused = false;
            currentMenu = pauseMenu;
        }

        /// <summary>
        /// Function to move up the menu, i.e highlight the option above the currently highlighted option 
        /// </summary>
        public static void moveUp()
        {
            selected--;
            if (selected < 0) selected = currentMenu.Count-1;
        }

        /// <summary>
        /// Function to move down the menu, i.e highlight the option below the currently highlighted option 
        /// </summary>
        public static void moveDown()
        {
            selected++;
            if (selected >= currentMenu.Count) selected = 0;
        }

        /// <summary>
        /// Select the currently highlighted option of the menu
        /// </summary>
        public static void select()
        {
            MenuItem selection = currentMenu[selected];
            if (selection.IsMenu)
            {
                currentMenu = selection.NextMenu;
                selected = 0;
            }
            else
            {
                selection.CallBack();
            }
        }

        /// <summary>
        /// The the current string representation of the menu
        /// </summary>
        /// <returns></returns>
        public static string getString()
        {
            string menu = "   DEBUG PAUSE MENU\n   ----------------\n";
            for(int i = 0;i<currentMenu.Count;i++)
            {
                if(i == selected) menu += "=>["+currentMenu[i].Text+"]<=";
                else menu += "   " + currentMenu[i].Text;
                menu += "\n";
            }
            return menu;
        }

    }
}
