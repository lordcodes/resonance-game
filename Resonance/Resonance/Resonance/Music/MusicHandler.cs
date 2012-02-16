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
        public static string GREEN = "TomLow";
        public static string BLUE = "TomMiddle";
        public static string YELLOW = "TomHigh";
        public static string RED = "Snare";
        public static string CYMBAL = "Crash";
        public static string CHINK = "chink";
        public static string DING = "ding";
        public static string SHIMMER = "shimmer";

        private static bool AUTO_MUSIC = false;

        MusicTrack bgMusic;
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        
        
        private Cue heartBeat;
        bool playHeartBeat = false;

        public MusicHandler(ContentManager newContent)
        {
            bgMusic = new MusicTrack(newContent);
            audioEngine = new AudioEngine("Content/SoundProject.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sound Bank.xsb");
            heartBeat = soundBank.GetCue("heart-beat");
            audioEngine.SetGlobalVariable("Microsoft Reverb", 50);
            
            if (AUTO_MUSIC) bgMusic.playTrack();
        }

        /// <summary>
        /// Gets access to the background music track
        /// </summary>
        /// <returns>bgMusic the background music track</returns>
        public MusicTrack getTrack()
        {
            return bgMusic;
        }

        public Cue playSound(string sound)
        {
            Cue soundCue = soundBank.GetCue(sound);
           

            soundCue.Play();
            return soundCue;
        }

        /// <summary>
        /// Play a sound and choose the volume
        /// </summary>
        /// <param name="sound">the string for the sound file</param>
        /// <param name="volume">value between 0 and 100</param>
        /// <returns></returns>
        public Cue playSound(string sound, int volume)
        {
            Cue soundCue = soundBank.GetCue(sound);
            soundCue = adjustVolume(soundCue, volume);
            soundCue.Play();
            return soundCue;
        }

        public Cue adjustVolume(Cue soundCue, int volume)
        {
            float vol = (float)(volume - 94);

            soundCue.SetVariable("Volume", vol);
            return soundCue;
        }

        public bool HeartBeat
        {
            set
            {
                playHeartBeat = value;
            }
        }

        public void Update()
        {
            audioEngine.Update();
            
            bgMusic.update();
            if (playHeartBeat && heartBeat.IsPaused) heartBeat.Resume();
            else if(!playHeartBeat && heartBeat.IsPlaying) heartBeat.Pause();
        }
    }
}
