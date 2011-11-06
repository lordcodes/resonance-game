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
    class SongTiming
    {
        int beatLength;
        int offset;
        int i;
        NoteMode mode;

        enum NoteMode { WHOLE, HALF, QUARTER };

        public SongTiming(ContentManager newContent)
        {
            String path = newContent.RootDirectory + "/Music/carcrash.timing";
            mode = NoteMode.QUARTER;

            StreamReader reader = new StreamReader(path);
            beatLength = Convert.ToInt32(reader.ReadLine());
            offset = Convert.ToInt32(reader.ReadLine());
            reader.Close();
            i = 0;
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
