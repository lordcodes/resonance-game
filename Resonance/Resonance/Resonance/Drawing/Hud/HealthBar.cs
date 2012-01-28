using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

///http://social.msdn.microsoft.com/Forums/en/xnagamestudioexpress/thread/3352bb09-c9e7-4a83-9699-41b67bb2d1bd

namespace Resonance
{
    class HealthBar
    {
        //private bool SaveTexture = false;
        GraphicsDeviceManager graphics;
        //static ContentManager content;
        //private static KeyboardState keyState;
        //SpriteBatch batch;
        Texture2D HUDTexture;
        RenderTarget2D HUDBuffer;
        private int HUDTextureHeight;

        public HealthBar(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            //graphics = new GraphicsDeviceManager(this);
            //content = new ContentManager(Services);
        }

        //public static ContentManager Content
        //{
        //    get { return content; }
        //}

        //protected override void Initialize()
        //{
        //    IsMouseVisible = true;
        //    base.Initialize();
        //}

        public void loadTextures(ContentManager content)
        {
            //if (loadAllContent)
            {
                //batch = new SpriteBatch(graphics.GraphicsDevice);
                HUDTexture = content.Load<Texture2D>("Drawing/HUD/Textures/bar");
                HUDTextureHeight = HUDTexture.Height / 3;
                HUDBuffer = new RenderTarget2D(graphics.GraphicsDevice, HUDTexture.Width, HUDTextureHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);
                //HUDBuffer = new RenderTarget2D(graphics.GraphicsDevice, HUDTexture.Width, HUDTextureHeight, 1, SurfaceFormat.Color);
            }
        }

        //protected override void UnloadGraphicsContent(bool unloadAllContent)
        //{
        //    if (unloadAllContent == true)
        //    {
        //        content.Unload();
        //    }
        //}

        //protected override void Update(GameTime gameTime)
        //{
        //    keyState = Keyboard.GetState();

        //    if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (keyState.IsKeyDown(Keys.Escape)))
        //        this.Exit();
        //    if (keyState.IsKeyDown(Keys.F1))
        //    {
        //        SaveTexture = true;
        //    }

        //    base.Update(gameTime);
        //}

        public void draw(SpriteBatch spriteBatch)
        {
           // GraphicsDevice gd = graphics.GraphicsDevice;
            MouseState ms = Mouse.GetState();
           // gd.Clear(Color.CornflowerBlue);

            // Set the render target to our hud buffer
       //     gd.SetRenderTarget(HUDBuffer);
            //gd.SetRenderTarget(0, HUDBuffer);
        //    gd.Clear(Color.Black);

            // Draw the full gauge to our buffer, no blending
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.Draw(HUDTexture, new Vector2(Drawing.pixelsX(10), Drawing.pixelsY(10)), new Rectangle(0, HUDTextureHeight, HUDTexture.Width, HUDTextureHeight), Color.White);
            spriteBatch.End();

            // Then draw the gauge mask in the correct place
            // Note: We change the render states from before so we need to draw them in a separate spriteBatch.begin/end

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            //gd.BlendState.ColorSourceBlend = Blend.InverseSourceColor;
//gd.BlendState.ColorDestinationBlend = Blend.InverseSourceColor;
            //gd.RenderState.SourceBlend = Blend.InverseSourceColor;
            //gd.RenderState.DestinationBlend = Blend.InverseSourceColor;
            spriteBatch.Draw(HUDTexture, new Vector2(Math.Max(ms.X, Drawing.pixelsX(0)), Drawing.pixelsY(0)), new Rectangle(0, HUDTextureHeight * 2, HUDTexture.Width, HUDTextureHeight), Color.White);
            spriteBatch.End();

            // Have to resolve the render target to use it    
            
            //gd.ResolveRenderTarget(0);

            // Set the render target back to the original display
       //     gd.SetRenderTarget(null);
            //gd.SetRenderTarget(0, null);

            //if (SaveTexture)
            //{
            //    // For debugging, press F1 to dump the texture to disk in TGA format
            //    SaveTexture = false;
            //    HUDBuffer.GetTexture().Save(@"c:\health.tga", ImageFileFormat.Tga);
            //}

            // Finally draw the hud and the gauge on top of it using alphablending (on by default)
            spriteBatch.Begin();
            Vector2 pos = new Vector2(Drawing.pixelsX(10), Drawing.pixelsY(10));
            spriteBatch.Draw(HUDTexture, pos, new Rectangle(0, 0, HUDTexture.Width, HUDTexture.Height), Color.White);
            //spriteBatch.Draw(HUDTexture, pos, new Rectangle(0, 0, HUDTexture.Width, HUDTextureHeight), Color.White);
            // Use GetTexture to return the texture of our RenderTarget2D
            spriteBatch.Draw(HUDBuffer, pos, Color.White);
            spriteBatch.End();
        }
    }
}