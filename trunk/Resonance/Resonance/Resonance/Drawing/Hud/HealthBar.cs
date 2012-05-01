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
        GraphicsDeviceManager graphics;
        Texture2D HUDTextureBare;
        Texture2D HUDTextureFull;
        Texture2D HUDAlpha;
        RenderTarget2D HUDBuffer;

        public HealthBar(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public void loadTextures(ContentManager content)
        {
            HUDTextureBare = content.Load<Texture2D>("Drawing/HUD/Textures/bar"); //empty bar with border
            HUDTextureFull = content.Load<Texture2D>("Drawing/HUD/Textures/bar2"); //full health gradient
            HUDAlpha = content.Load<Texture2D>("Drawing/HUD/Textures/bar3"); //sliding overlay to mask health gradient

            HUDBuffer = new RenderTarget2D(graphics.GraphicsDevice, HUDTextureBare.Width, HUDTextureBare.Height, true, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        /// <summary>
        /// Renders the health bar texture in the HUDBuffer render target
        /// </summary>
        public void saveTexture(SpriteBatch spriteBatch, int health)
        {
            //spriteBatch.End();
            GraphicsDevice gd = graphics.GraphicsDevice;
            MouseState ms = Mouse.GetState();

            // Set the render target to our hud buffer
            gd.SetRenderTarget(HUDBuffer);
            //gd.SetRenderTarget(0, HUDBuffer);
            //gd.Clear(Color.Black);

            // Draw the full gauge to our buffer, no blending
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.Draw(HUDTextureFull, new Vector2(-7, ScreenManager.pixelsY(0))/*, new Rectangle(0, HUDTextureHeight, HUDTexture.Width, HUDTextureHeight)*/, Color.White);
            spriteBatch.End();


            // Then draw the gauge mask in the correct place
            // Note: We change the render states from before so we need to draw them in a separate spriteBatch.begin/end
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            //gd.BlendState.ColorSourceBlend = Blend.InverseSourceColor;
            //gd.BlendState.ColorDestinationBlend = Blend.InverseSourceColor;
            //gd.RenderState.SourceBlend = Blend.InverseSourceColor;
            //gd.RenderState.DestinationBlend = Blend.InverseSourceColor;

            //BlendState old = spriteBatch.GraphicsDevice.BlendState;
            BlendState b = new BlendState();
            //b.ColorSourceBlend = Blend.InverseSourceColor;
            //b.ColorDestinationBlend = Blend.InverseDestinationColor;
            spriteBatch.GraphicsDevice.BlendState = b;

            int healthLimit = ScreenManager.pixelsX(0) + HUDTextureBare.Width * health / GoodVibe.MAX_HEALTH;
            spriteBatch.Draw(HUDAlpha, new Vector2(/*Math.Max(ms.X, ScreenManager.pixelsX(0))*/ healthLimit, ScreenManager.pixelsY(0)), /*new Rectangle(0, HUDTextureHeight * 2, HUDTexture.Width, HUDTextureHeight),*/ Color.White);
            spriteBatch.End();
            //spriteBatch.GraphicsDevice.BlendState = old;


            // Have to resolve the render target to use it    
            //gd.ResolveRenderTarget(0);
            

            // Set the render target back to the original display           
            gd.SetRenderTarget(null);            
            //gd.SetRenderTarget(0, null);
            

            // Finally draw the hud and the gauge on top of it using alphablending (on by default)
            //spriteBatch.Begin();
        }

        /// <summary>
        /// Draws the health bar for the player on screen
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {

            Vector2 pos = new Vector2(ScreenManager.pixelsX(20), ScreenManager.pixelsY(10));
            spriteBatch.Draw(HUDTextureBare, pos, new Rectangle(0, 0, HUDTextureBare.Width, HUDTextureBare.Height), Color.White);
            //spriteBatch.Draw(HUDTexture, pos, new Rectangle(0, 0, HUDTexture.Width, HUDTextureHeight), Color.White);


            // Use GetTexture to return the texture of our RenderTarget2D
            spriteBatch.Draw((Texture2D)HUDBuffer, pos, Color.White);
            //spriteBatch.End();

            //spriteBatch.Begin();
        }
    }
}