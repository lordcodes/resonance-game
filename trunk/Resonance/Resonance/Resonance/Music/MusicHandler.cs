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

        private static AudioEmitter emitter;
        private static AudioListener listener;
        
        
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
            MediaPlayer.Volume = 0.7f;

            emitter = new AudioEmitter();
            listener = new AudioListener();
            listener.Position = Vector3.Zero;
            emitter.Position = new Vector3(0, 100, 0);

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

        public static void playSound(string sound)
        {
            Cue soundCue = soundBank.GetCue(sound);         
            soundCue.Play();
        }

        public static void adjustVolume(Cue soundCue, float volume)
        {
            float vol = volume + 94;
            Vector3 pos = emitter.Position;
            pos.Y = vol;
            emitter.Position = pos;
            soundCue.Apply3D(listener, emitter);
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
