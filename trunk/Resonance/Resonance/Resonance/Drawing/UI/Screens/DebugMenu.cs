
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

            MenuElement resume = new MenuElement("Resume Game", new ItemDelegate(delegate { resumeGame(); }));
            MenuElement health = new MenuElement("Restore GV to Full Health", new ItemDelegate(delegate { fullHealth(); }));
            MenuElement resetPos = new MenuElement("Reset Object Positions", new ItemDelegate(delegate { softReset(); }));
            MenuElement addBV = new MenuElement("Add a BV to the World", new ItemDelegate(delegate { addBadVibe(); }));
            MenuElement killBVs = new MenuElement("Kill All the BVs", new ItemDelegate(delegate { killBadVibes(); }));
            MenuElement quit = new MenuElement("Quit the Game", new ItemDelegate(delegate { quitGame(); }));

            MenuItems.Add(resume);
            MenuItems.Add(health);
            MenuItems.Add(resetPos);
            MenuItems.Add(addBV);
            MenuItems.Add(killBVs);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            base.LoadContent();
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

        private void resumeGame()
        {
            ExitScreen();
        }

        public void fullHealth()
        {
            GameScreen.getGV().fullHealth();
        }

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
            ExitScreen();
        }

        public void softReset()
        {
            ScreenManager.game.World.reset();
        }

        public void addBadVibe()
        {
            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVDebug" + genNumber, new Vector3(0, 1, -2), 0);
            ScreenManager.game.World.addObject(bv);
            genNumber++;
        }

        private void quitGame()
        {
            ScreenManager.Game.Exit();
        }
    }
}
