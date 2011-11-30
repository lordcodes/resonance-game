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
        long beatLength;
        long offset;
        NoteMode mode;
        long startTime;

        long whenPaused;
        int lastI;

        int currentBeatCount;

        const long WINDOW = 45000000;

        enum NoteMode { WHOLE, HALF, QUARTER };
        enum PlayState { PLAYING, PAUSED, STOPPED };

        public MusicTrack(ContentManager newContent) 
        {
            content = newContent;
            song = content.Load<Song>("Music/song");
            state = PlayState.STOPPED;
            String path = newContent.RootDirectory + "/Music/song.timing";
            mode = NoteMode.QUARTER;

            StreamReader reader = new StreamReader(path);
            beatLength = Convert.ToInt32(reader.ReadLine());
            offset = Convert.ToInt32(reader.ReadLine());
            reader.Close();
            lastI = 0;
            currentBeatCount = 0;
        }

        /// <summary>
        /// Play the track
        /// </summary>
        public void playTrack()
        {
            if (state == PlayState.STOPPED)
            {
                currentBeatCount = 0;
                state = PlayState.PLAYING;
                startTime = DateTime.Now.Ticks * 100;
                MediaPlayer.Play(song);
            }
            else if (state == PlayState.PAUSED)
            {
                state = PlayState.PLAYING;
                startTime = ((DateTime.Now.Ticks * 100) - whenPaused) + startTime;
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
                whenPaused = DateTime.Now.Ticks * 100;
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Detect if it is on the next quarter beat of music.
        /// </summary>
        public bool nextQuarterBeat()
        {
            long time = DateTime.Now.Ticks * 100;

            if (time > startTime + offset + (currentBeatCount * beatLength/4))
            {
                currentBeatCount++;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Detect if you are in time to the beat
        /// </summary>
        public void inTime()
        {
            if (state == PlayState.PLAYING)
            {
                for (; ; lastI++)
                {
                    long time = DateTime.Now.Ticks * 100;
                    long beatTime;
                    long lastBeatTime;

                    if (mode == NoteMode.WHOLE)
                    {
                        beatTime = startTime + offset + (lastI * beatLength);
                        lastBeatTime = startTime + offset + ((lastI - 1) * beatLength);
                    }
                    else if (mode == NoteMode.HALF)
                    {
                        beatTime = startTime + offset + (lastI * beatLength / 2);
                        lastBeatTime = startTime + offset + ((lastI - 1) * beatLength / 2);
                    }
                    else
                    {
                        beatTime = startTime + offset + (lastI * beatLength / 4);
                        lastBeatTime = startTime + offset + ((lastI - 1) * beatLength / 4);
                    }

                    if (time < beatTime)
                    {
                        if (time > (beatTime - WINDOW))
                        {
                            //HIT
                            //Console.WriteLine("HIT1");
                        }
                        else if (time < lastBeatTime + WINDOW)
                        {
                            //HIT
                            //Console.WriteLine("HIT2");
                        }
                        else
                        {
                            //MISS
                            //Console.WriteLine("MISS");
                        }
                        break;
                    }
                }
            }
        }
    }
}
