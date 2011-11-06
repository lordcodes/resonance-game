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
    class MusicTrack
    {
        ContentManager content;
        Song song;
        PlayState state;

        enum PlayState { PLAYING, PAUSED, STOPPED };

        public MusicTrack(ContentManager newContent) 
        {
            content = newContent;
            song = content.Load<Song>("Music/song");
            state = PlayState.STOPPED;
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
    }
}
