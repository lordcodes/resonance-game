using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    /// <summary>
    /// Class that reprents the a single menu item i.e its string value and what heppens when its selected
    /// </summary>
    class MenuItem
    {
        private string text;
        private ItemDelegate callBack;
        private List<MenuItem> nextMenu = new List<MenuItem>();
        private bool isMenu = false;

        /// <summary>
        /// The menu to be displayed if this MenuItem is selected
        /// </summary>
        public List<MenuItem> NextMenu
        {
            get
            {
                return nextMenu;
            }
        }

        /// <summary>
        /// Return true if when selected, this item leads to another menu
        /// </summary>
        public bool IsMenu
        {
            get
            {
                return isMenu;
            }
        }

        /// <summary>
        /// Retursn the string value of the menu item
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
        }

        /// <summary>
        /// Returns the callback function to be fired when the menu item is selected. 
        /// </summary>
        public ItemDelegate CallBack
        {
            get
            {
                return callBack;
            }
        }

        /// <summary>
        /// Constructor used if, when selected, a function should be called. 
        /// </summary>
        /// <param name="text">String value for the menu item</param>
        /// <param name="callBack">Function to be called when the item is selected</param>
        public MenuItem(string text, ItemDelegate callBack)
        {
            this.text = text;
            this.callBack = callBack;
        }

        /// <summary>
        /// COnstructor to be used when the button will lead to another menu
        /// </summary>
        /// <param name="text">String value for the menu item</param>
        /// <param name="nextMenu">List of MenuItems representing the menu to be diplyed once this button is selected</param>
        public MenuItem(string text, List<MenuItem> nextMenu)
        {
            this.text = text;
            this.nextMenu = nextMenu;
            this.isMenu = true;
        }
    }
}
