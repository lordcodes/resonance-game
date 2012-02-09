using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class DebugMenu : MenuScreen
    {
        private static int genNumber = 0;

        public DebugMenu()
            : base("Debug Menu")
        {
            //Resume
            //GV to full health
            //Reset object positions
            //Add a BV
            //Kill all BVs
            //Exit the Game
            //Quit to Main Menu

            //MenuElement play = new MenuElement("Enter the Resonance Chamber", new ItemDelegate(delegate { startGame(); }));
            //MenuElement quit = new MenuElement("Quit Game", new ItemDelegate(MenuActions.exit));

            //MenuItems.Add(play);
            //MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            Font = this.ScreenManager.Content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFirst"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSecond"));
            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuThird"));
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            int x = 1350;
            int y = 300;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Position = new Vector2(ScreenManager.pixelsX(x), ScreenManager.pixelsY(y));
                y += 75;
            }
        }

        /// <summary>
        /// Give the player character full health.
        /// </summary>
        public void fullHealth()
        {
            GameScreen.getGV().fullHealth();
        }

        /// <summary>
        /// Kill all the bad vibes in the world
        /// </summary>
        public void killBadVibes()
        {
            Dictionary<string, Object> objects = ScreenManager.game.World.returnObjects();
            foreach (var entry in objects)
            {
                if (entry.Value is BadVibe)
                {
                    ((BadVibe)entry.Value).kill();
                }
            }
        }

        /// <summary>
        /// Resets the level to objects original position. Does not relod the level XML and re-create the objects.
        /// </summary>
        public void softReset()
        {
            ScreenManager.game.World.reset();
        }

        /// <summary>
        /// Add a BadVibe to the world
        /// </summary>
        public void addBadVibe()
        {
            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + genNumber, new Vector3(0, 1, -2), 0);
            ScreenManager.game.World.addObject(bv);
            genNumber++;
        }

        private void quitGame()
        {
            ScreenManager.Game.Exit();
        }
    }
}
