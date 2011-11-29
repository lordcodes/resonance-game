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
    class MusicHandler
    {
        MusicTrack bgMusic;
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        public MusicHandler(ContentManager newContent)
        {
            bgMusic = new MusicTrack(newContent);
            audioEngine = new AudioEngine("Content/SoundProject.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sound Bank.xsb");
        }

        /// <summary>
        /// Gets access to the background music track
        /// </summary>
        /// <returns>bgMusic the background music track</returns>
        public MusicTrack getTrack()
        {
            return bgMusic;
        }

        public void playCollision() {
            soundBank.PlayCue("clunk");
        }

        public void Update()
        {
            audioEngine.Update();
        }
    }
}
