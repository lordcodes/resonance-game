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
using System.IO;

namespace Resonance
{
    class MusicTrack
    {
        ContentManager content;
        Song song;
        PlayState state;
        int beatLength;
        int offset;
        NoteMode mode;
        long startTime;

        long whenPaused;
        int lastI;

        const int WINDOW = 45000000;

        enum NoteMode { WHOLE, HALF, QUARTER };
        enum PlayState { PLAYING, PAUSED, STOPPED };

        public MusicTrack(ContentManager newContent) 
        {
            content = newContent;
            song = content.Load<Song>("Music/song");
            state = PlayState.STOPPED;
            String path = newContent.RootDirectory + "/Music/carcrash.timing";
            mode = NoteMode.QUARTER;

            StreamReader reader = new StreamReader(path);
            beatLength = Convert.ToInt32(reader.ReadLine());
            offset = Convert.ToInt32(reader.ReadLine());
            reader.Close();
            lastI = 0;
        }

        /// <summary>
        /// Play the track
        /// </summary>
        public void playTrack()
        {
            if (state == PlayState.STOPPED)
            {
                state = PlayState.PLAYING;
                startTime = DateTime.Now.Ticks / 100;
                MediaPlayer.Play(song);
            }
            else if (state == PlayState.PAUSED)
            {
                state = PlayState.PLAYING;
                startTime = ((DateTime.Now.Ticks / 100) - whenPaused) + startTime;
                MediaPlayer.Resume();
            }
        }

        /// <summary>
        /// Stop the track
        /// </summary>
        public void stopTrack()
        {
            if (state == PlayState.PLAYING || state == PlayState.PAUSED)
            {
                state = PlayState.STOPPED;
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Pause the track
        /// </summary>
        public void pauseTrack()
        {
            if (state == PlayState.PLAYING)
            {
                state = PlayState.PAUSED;
                whenPaused = DateTime.Now.Ticks / 100;
                MediaPlayer.Pause();
            }
        }

        public bool inTime()
        {
            for (; ; lastI++)
            {
                long time = DateTime.Now.Ticks / 100;
                long beatTime;
                long lastBeatTime;

                if (mode == NoteMode.WHOLE)
                {
                    beatTime = startTime + offset + (lastI + beatLength);
                    lastBeatTime = startTime + offset + ((lastI - 1) * beatLength);
                }
                else if (mode == NoteMode.HALF)
                {
                    beatTime = startTime + offset + (lastI + beatLength / 2);
                    lastBeatTime = startTime + offset + ((lastI - 1) * beatLength / 2);
                }
                else
                {
                    beatTime = startTime + offset + (lastI + beatLength / 4);
                    lastBeatTime = startTime + offset + ((lastI - 1) * beatLength / 4);
                }

                if (time < beatTime)
                {
                    if (time > (beatTime - WINDOW))
                    {
                        //HIT
                        Console.WriteLine("HIT");
                    }
                    else if (time < lastBeatTime + WINDOW)
                    {
                        //HIT
                        Console.WriteLine("HIT");
                    }
                    else
                    {
                        //MISS
                        Console.WriteLine("MISS");
                    }
                    break;
                }
            }


            return true;
        }
    }
}
