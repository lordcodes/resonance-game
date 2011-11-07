using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class SoundEffectTrack
    {
        ContentManager content;
        List<SoundEffect> sounds;

        public SoundEffectTrack(ContentManager newContent)
        {
            content = newContent;
            sounds = new List<SoundEffect>();
            SoundEffect effect = content.Load<SoundEffect>("Sounds/deathfire");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/proxmine");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/ohyoubad");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/zaps");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/whatashame");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/rottenwaytodie");
            sounds.Add(effect);
            effect = content.Load<SoundEffect>("Sounds/woops");
            sounds.Add(effect);
        }

        public void playSound(int index)
        {
            sounds[index].Play();
        }
    }
}
