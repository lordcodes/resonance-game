using Microsoft.Xna.Framework;

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
