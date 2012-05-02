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

        private static bool AUTO_MUSIC = false; //TODO: hemmmm

        private static MusicTrack bgMusic;
        private static AudioEngine audioEngine;
        private static WaveBank waveBank;
        private static SoundBank soundBank;
        
        
        private static Cue heartBeat;
        private static bool playHeartBeat = false;

        public static void init(ContentManager newContent)
        {
            bgMusic = new MusicTrack(newContent);
            audioEngine = new AudioEngine("Content/SoundProject.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sound Bank.xsb");
            heartBeat = soundBank.GetCue("heart-beat");
            audioEngine.SetGlobalVariable("Microsoft Reverb", 50);
            
            if (AUTO_MUSIC) bgMusic.playTrack();
        }

        public static void reset()
        {
            bgMusic.reset();
        }

        /// <summary>
        /// Gets access to the background music track
        /// </summary>
        /// <returns>bgMusic the background music track</returns>
        public static MusicTrack getTrack()
        {
            return bgMusic;
        }

        public static Cue getCue(string name)
        {
            return soundBank.GetCue(name);
        }

        public static Cue playSound(string sound)
        {
            Cue soundCue = soundBank.GetCue(sound);
           
            soundCue.Play();
            return soundCue;
        }

        /// <summary>
        /// Play a sound and choose the volume
        /// </summary>
        /// <param name="sound">the string for the sound file</param>
        /// <param name="volume">value between -94 and +6 (0 is default sound level)</param>
        /// <returns></returns>
        public static Cue playSound(string sound, float volume)
        {
            Cue soundCue = soundBank.GetCue(sound);
            adjustVolume(soundCue, volume);
            soundCue.Play();
            return soundCue;
        }

        public static void adjustVolume(Cue soundCue, float volume)
        {
            /*float vol = 0;
            if (volume < -94) vol = -94;
            else if (volume > 6) vol = 6;
            else vol = volume;*/
            //soundBank.GetCue(soundCue.Name).SetVariable("Volume", volume);
            soundCue.SetVariable("Volume", volume);
        }

        public static bool HeartBeat
        {
            set
            {
                playHeartBeat = value;
            }
        }

        public static AudioEngine AudioEngine
        {
            get { return audioEngine; }
        }

        public static void Update()
        {            
            bgMusic.update();
            if (playHeartBeat && heartBeat.IsPaused) heartBeat.Resume();
            else if(!playHeartBeat && heartBeat.IsPlaying) heartBeat.Pause();
        }
    }
}
