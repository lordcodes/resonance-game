using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    public class LoadedContent
    {
        private static ImportedGameModels importedGameModels = null;
        private static DisplacementMap dispMap = null;

        public static ImportedGameModels getModels(ContentManager Content)
        {
            if (importedGameModels == null)
            {
                importedGameModels = Content.Load<ImportedGameModels>("Drawing/modelDetails");
            }
            return importedGameModels;
        }

        public static DisplacementMap getDisp(ImportedGameModels importedModels, GraphicsDevice graphicsDevice)
        {
            if (dispMap == null)
            {
                //dispMap = importedModels.dispMap;
                dispMap = new DisplacementMap(Graphics.DISP_WIDTH, Graphics.DISP_WIDTH);
            }
            dispMap.setGD(graphicsDevice);
            dispMap.reset();
            return dispMap;
        }
    }
}
