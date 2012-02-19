using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Resonance;

namespace Resonance
{
    public class FireTextureEffect : TextureEffect
    {
        public FireTextureEffect(float width, float height, Vector3 position)
            : base(width, height, position, true, GameModels.getTextureAnimationInstance("fireAnimation"))
        {
        }
    }
}
