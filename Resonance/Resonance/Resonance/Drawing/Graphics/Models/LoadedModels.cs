using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Resonance
{
    class LoadedModels
    {
        private static ImportedGameModels importedGameModels = null;

        public static ImportedGameModels get(ContentManager Content)
        {
            if (importedGameModels == null)
            {
                importedGameModels = Content.Load<ImportedGameModels>("Drawing/modelDetails");
            }
            return importedGameModels;
        }
    }
}
