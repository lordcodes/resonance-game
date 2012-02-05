using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class MainMenuScreen
    {
        private GraphicsDeviceManager graphics;
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private List<Texture2D> bg;

        private List<MenuItem> currentMenu;
        private List<MenuItem> mainMenu;
        private List<MenuItem> levelsMenu;

        private int selected = 0;

        //private float timeElapsed = 0;
        //private float timeToUpdate = 0.5f;
        //private int frameNumber = 0;

        /// <summary>
        /// Create class representing the main menu screen
        /// </summary>
        /// <param name="content">ContentManager</param>
        /// <param name="graphics">GraphicsDeviceManager graphics used to create SpriteBatch for image drawing</param>
        public MainMenuScreen(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            this.content = content;
            mainMenu = new List<MenuItem>();
            levelsMenu = new List<MenuItem>();
            bg = new List<Texture2D>();
        }

        /// <summary>
        /// Called once to load data needed to display main menu screen
        /// </summary>
        public void loadContent()
        {
            //bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenu"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFirst"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSecond"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuThird"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFourth"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuFifth"));
            bg.Add(content.Load<Texture2D>("Drawing/UI/MainMenu/Textures/MainMenuSixth"));

            font = content.Load<SpriteFont>("Drawing/Fonts/MainMenuFont");
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            //levelsMenu.Add(new MenuItem("Level 1", new ItemDelegate(delegate { MenuActions.loadLevel(1); })));
            //levelsMenu.Add(new MenuItem("Level 2", new ItemDelegate(delegate { MenuActions.loadLevel(2); })));
            //levelsMenu.Add(new MenuItem("Back", mainMenu));

            //mainMenu.Add(new MenuItem("Enter the Resonance Chamber", levelsMenu));
            mainMenu.Add(new MenuItem("Enter the Resonance Chamber", new ItemDelegate(delegate { MenuActions.loadLevel(1); })));
            //mainMenu.Add(new MenuItem("High Scores", levelsMenu));
            //mainMenu.Add(new MenuItem("Credits", levelsMenu));
            mainMenu.Add(new MenuItem("Quit Game", new ItemDelegate(MenuActions.exit)));

            /*levelsMenu.Add(new MenuItem("Load Level 1", new ItemDelegate(delegate { MenuActions.loadLevel(1); })));
            levelsMenu.Add(new MenuItem("Load Level 2", new ItemDelegate(delegate { MenuActions.loadLevel(2); })));
            levelsMenu.Add(new MenuItem("Back", pauseMenu));*/

            currentMenu = mainMenu;
        }

        /// <summary>
        /// Function to move up the menu, i.e highlight the option above the currently highlighted option 
        /// </summary>
        public void moveUp()
        {
            selected--;
            if (selected < 0) selected = currentMenu.Count - 1;
        }

        /// <summary>
        /// Function to move down the menu, i.e highlight the option below the currently highlighted option 
        /// </summary>
        public void moveDown()
        {
            selected++;
            if (selected >= currentMenu.Count) selected = 0;
        }

        /// <summary>
        /// Select the currently highlighted option of the menu
        /// </summary>
        public void select()
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
        /// Called every frame, to update main menu
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// Called to draw the loading screen
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();
            string text = "Welcome to the Resonance Chamber";
            spriteBatch.Draw(bg[selected], new Rectangle(0, 0, Drawing.ScreenWidth+1, Drawing.ScreenHeight+1), Color.White);
            spriteBatch.DrawString(font, text, new Vector2(Drawing.pixelsX(20), Drawing.pixelsY(30)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            int x = 1350;
            int y = 300;
            int i = 0;
            for(int index = 0; index < currentMenu.Count; index++)
            {
                if(index == selected) spriteBatch.DrawString(font, currentMenu[index].Text, new Vector2(Drawing.pixelsX(x), Drawing.pixelsY(y + i)), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                else spriteBatch.DrawString(font, currentMenu[index].Text, new Vector2(Drawing.pixelsX(x), Drawing.pixelsY(y + i)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                i += 75;
            }

            spriteBatch.End();
        }
    }
}
