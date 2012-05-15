using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Resonance
{
    class MusicTrack
    {
        public bool SONG_ENDED                      = false;
        public bool SONG_MANUALLY_STOPPED           = true;
        public bool SONG_MANUALLY_PAUSED            = false;
        public bool SONG_STARTED_IN_THE_FIRST_PLACE = false;

        ContentManager content;
        Song song;
        PlayState state;
        long beatLength;
        long halfBeatLength;
        long quarterBeatLength;
        long offset;
        NoteMode mode;
        long startTime;

        long whenPaused;
        int lastI;

        int currentBeatCount;
        int beats   = 0;
        int offbeat = 0;

        //public const long WINDOW = 80000000;//45000000;
        public const long WINDOW = 160000000;//45000000;
        const long EXTRA_OFF = 250000000;
        //const long EXTRA_OFF = 0;

        public enum NoteMode { WHOLE, HALF, QUARTER };
        enum PlayState { PLAYING, PAUSED, STOPPED };

        public MusicTrack(ContentManager newContent) 
        {
            content = newContent;
            song = content.Load<Song>("Music/song");
            MediaPlayer.IsRepeating = false;
            state = PlayState.STOPPED;
            String path = newContent.RootDirectory + "/Music/song.timing";

            //mode = NoteMode.QUARTER;
                 if (GameScreen.DIFFICULTY == GameScreen.BEGINNER) mode = NoteMode.WHOLE;
            else if (GameScreen.DIFFICULTY <= GameScreen.MEDIUM)   mode = NoteMode.HALF;
            else                                       mode = NoteMode.QUARTER;

            StreamReader reader = new StreamReader(path);
            beatLength = Convert.ToInt32(reader.ReadLine());
            halfBeatLength = beatLength >> 1;
            quarterBeatLength = halfBeatLength >> 1;
            offset = Convert.ToInt64(reader.ReadLine());
            reader.Close();
            lastI = 0;
            currentBeatCount = 0;

            //DebugDisplay.update("Half Beat Length", halfBeatLength.ToString());
            //DebugDisplay.update("Window Size     ", WINDOW.ToString());
        }

        public void reset()
        {
            song = content.Load<Song>("Music/song");
            state = PlayState.STOPPED;
            if (GameMode.MUSIC_REPEAT) MediaPlayer.IsRepeating = true; else MediaPlayer.IsRepeating = false;
        }

        /// <summary>
        /// Play the track
        /// </summary>
        public void playTrack()
        {
            if (state == PlayState.STOPPED)
            {
                SONG_MANUALLY_STOPPED = false;

                currentBeatCount = 0;
                state = PlayState.PLAYING;
                startTime = DateTime.Now.Ticks * 100;
                MediaPlayer.Play(song);                
            }
            else if (state == PlayState.PAUSED)
            {
                SONG_MANUALLY_PAUSED = false;

                state = PlayState.PLAYING;
                startTime = ((DateTime.Now.Ticks * 100) - whenPaused) + startTime;
                MediaPlayer.Resume();
            }
        }

        private void loopInTime() {
            while (true) {
                Console.Out.WriteLine("WOOPA");
                inTime2(MusicTrack.NoteMode.WHOLE);
                Console.Out.WriteLine("WOOPB");
            }
        }

        /// <summary>
        /// Stop the track
        /// </summary>
        public void stopTrack()
        {
            if (state == PlayState.PLAYING || state == PlayState.PAUSED)
            {
                SONG_MANUALLY_STOPPED = true;

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
                SONG_MANUALLY_PAUSED = true;

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

        public void update() {
            if (MediaPlayer.State == MediaState.Playing) SONG_STARTED_IN_THE_FIRST_PLACE = true;
            if ((MediaPlayer.State == MediaState.Stopped) && (!SONG_MANUALLY_STOPPED) && MusicHandler.getTrack().SONG_STARTED_IN_THE_FIRST_PLACE)
            {
                SONG_ENDED = true;
            }
        }

        /// <summary>
        /// Detect if you are in time to the beat
        /// DEPRECATED use inTime2()
        /// </summary>
        public float inTime()
        {
            if (state == PlayState.PLAYING)
            {
                long time = (DateTime.Now.Ticks * 100) - startTime + EXTRA_OFF;
                float scoreWeight = -1f;
                for (; ; lastI++)
                {
                    long beatTime;
                    long lastBeatTime;

                    if (mode == NoteMode.WHOLE)
                    {
                        beatTime = offset + (lastI * beatLength);
                        lastBeatTime = beatTime - beatLength;
                    }
                    else if (mode == NoteMode.HALF)
                    {
                        beatTime = offset + (lastI * halfBeatLength);
                        lastBeatTime = beatTime - halfBeatLength;
                    }
                    else
                    {
                        beatTime = offset + (lastI * quarterBeatLength);
                        lastBeatTime = beatTime - quarterBeatLength;
                    }

                    if (time < beatTime)
                    {
                        if (time > (beatTime - WINDOW))
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT1");

                            //return true;
                            long numerator = time - beatTime + WINDOW;
                            long window    = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div  = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            scoreWeight = (float) Math.Cos(div2);
                        }
                        if (time < lastBeatTime + WINDOW)
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT2");
                            //return true;
                            long numerator = lastBeatTime + WINDOW - time;
                            long window = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            float result = (float) Math.Cos(div2);
                            if (result > scoreWeight) scoreWeight = result;
                        }
                        if (scoreWeight == -1f) offbeat++;

                        //DebugDisplay.update("Beats    ",   beats.ToString());
                        //DebugDisplay.update("Offbeats ", offbeat.ToString());
                        break;
                    }
                }

                return scoreWeight;
            } else {
                // Not playing. Return false.
                //return false;
                return -1f;
            }
        }

        public Song Song
        {
            get { return song; }
        }

        private static List<long> nexts = new List<long>();
        private static List<long> lasts = new List<long>();

        static bool blap = false;

        public float inTime2(NoteMode m)
        {
            mode = m;

            beatLength        = 500000000;
            halfBeatLength    = 250000000;
            quarterBeatLength = 125000000;

            //mode = NoteMode.QUARTER;
            if (state == PlayState.PLAYING)
            {
                long time = (DateTime.Now.Ticks * 100) - startTime + 50000000 - 125000000;
                float scoreWeight = -1f;
                lastI = 0;
                for (; ; lastI++)
                {
                    long beatTime;
                    long lastBeatTime;

                    if (mode == NoteMode.WHOLE)
                    {
                        beatTime = (lastI * beatLength);
                        lastBeatTime = beatTime - beatLength;
                    }
                    else if (mode == NoteMode.HALF)
                    {
                        beatTime = (lastI * halfBeatLength);
                        lastBeatTime = beatTime - halfBeatLength;
                    }
                    else
                    {
                        beatTime = (lastI * quarterBeatLength);
                        lastBeatTime = beatTime - quarterBeatLength;
                    }

                    if (time < beatTime)
                    {
                        if (time > (beatTime - WINDOW))
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT1");

                            //return true;
                            long numerator = time - beatTime + WINDOW;
                            long window    = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div  = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            scoreWeight = (float) Math.Cos(div2);
                        }
                        if (time < lastBeatTime + WINDOW)
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT2");
                            //return true;
                            long numerator = lastBeatTime + WINDOW - time;
                            long window = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            float result = (float) Math.Cos(div2);
                            if (result > scoreWeight) scoreWeight = result;
                        }
                        if (scoreWeight == -1f) offbeat++;

                        //DebugDisplay.update("Beats    ",   beats.ToString());
                        //DebugDisplay.update("Offbeats ", offbeat.ToString());
                        break;
                    }
                }
                //DebugDisplay.update("TIMING:", scoreWeight.ToString());

                return scoreWeight;
            } else {
                // Not playing. Return false.
                //return false;
                return -1f;
            }
        }

        bool goingup = false;
        bool prevgoingup = false;
        float prevscore = 0f;

        public bool timesong() {
            long longWindow = 250000000;

            beatLength        = 500000000;
            halfBeatLength    = 250000000;
            quarterBeatLength = 125000000;

            mode = NoteMode.HALF;
            if (state == PlayState.PLAYING)
            {
                lastI = 0;
                long time = (DateTime.Now.Ticks * 100) - startTime + 50000000 - 125000000;
                float scoreWeight = -1f;
                for (; ; lastI++)
                {
                    long beatTime;
                    long lastBeatTime;

                    if (mode == NoteMode.WHOLE)
                    {
                        beatTime = (lastI * beatLength);
                        lastBeatTime = beatTime - beatLength;
                    }
                    else if (mode == NoteMode.HALF)
                    {
                        beatTime = (lastI * halfBeatLength);
                        lastBeatTime = beatTime - halfBeatLength;
                    }
                    else
                    {
                        beatTime = (lastI * quarterBeatLength);
                        lastBeatTime = beatTime - quarterBeatLength;
                    }

                    if (time < beatTime)
                    {
                        if (time > (beatTime - WINDOW))
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT1");

                            //return true;
                            long numerator = time - beatTime + WINDOW;
                            long window    = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div  = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            scoreWeight = (float) Math.Cos(div2);
                        }
                        if (time < lastBeatTime + WINDOW)
                        {
                            beats++;
                            //HIT
                            //Console.WriteLine("HIT2");
                            //return true;
                            long numerator = lastBeatTime + WINDOW - time;
                            long window = WINDOW;// >> 15;
                            //numerator >>= 15;
                            float div = (numerator / (float) window);
                            double div2 = ((1f - div) * (Math.PI / 2d));
                            float result = (float) Math.Cos(div2);
                            if (result > scoreWeight) scoreWeight = result;
                        }
                        if (scoreWeight == -1f) offbeat++;

                        //DebugDisplay.update("Beats    ",   beats.ToString());
                        //DebugDisplay.update("Offbeats ", offbeat.ToString());
                        break;
                    }
                }
                //DebugDisplay.update("TIMING:", scoreWeight.ToString());
                if (prevscore > scoreWeight) {
                    goingup = false;
                } else {
                    goingup = true;
                }

                // The following 'if' statement fires on every beat.
                if (/*!blap && scoreWeight > 0.85*/ prevgoingup && !goingup) { blap = true; /*MusicHandler.playSound(MusicHandler.RED);*/ }
                if (scoreWeight == -1) blap = false;
                prevscore = scoreWeight;

                bool ans = prevgoingup && !goingup;
                prevgoingup = goingup;
                return ans;
            } else {
                // Not playing. Return false.
                //return false;
                return false;
            }
        }
    }
}
