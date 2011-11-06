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
        int i;
        NoteMode mode;

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
            i = 0;
        }

        /// <summary>
        /// Play the track
        /// </summary>
        public void playTrack()
        {
            if (state == PlayState.STOPPED)
            {
                state = PlayState.PLAYING;
                MediaPlayer.Play(song);
            }
            else if (state == PlayState.PAUSED)
            {
                state = PlayState.PLAYING;
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
                MediaPlayer.Pause();
            }
        }

        public bool inTime()
        {
            for (; ; i++)
            {
                long time = DateTime.Now.Ticks / 100;
                long beatTime;
                long lastBeatTime;

                if (mode == NoteMode.WHOLE)
                {

                    //beatTime = startTime + testOff + (lastI * testATD);
                    //lastBeatTime = startTime + testOff + ((lastI - 1) * testATD);
                }
                else if (mode == NoteMode.HALF)
                {
                    //beatTime = startTime + testOff + (lastI * testATD / 2);
                    //lastBeatTime = startTime + testOff + ((lastI - 1) * testATD / 2);
                }
                else
                {
                    //beatTime = startTime + testOff + (lastI * testATD / 4);
                    //lastBeatTime = startTime + testOff + ((lastI - 1) * testATD / 4);
                }

                /*if (time < beatTime) {
	                if (time > (beatTime - WINDOW)) {
	                System.out.println("HIT!");
	            } else if (time < lastBeatTime + WINDOW) {
	                System.out.println("HIT!");
	                lastI--;
	            } else {
	                System.out.println("MISS");
	            }

	            break;*/
            }


            return true;
        }
    }
}
