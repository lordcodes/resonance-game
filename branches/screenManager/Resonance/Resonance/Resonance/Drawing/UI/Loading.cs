using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Loading
    {
        private static bool loading = false;
        private static Thread t;
        private static ThreadObject thread;

        public static bool isLoading
        {
            get
            {
                return loading;
            }
        }

        /// <summary>
        /// Carrys out the function in another thread.
        /// </summary>
        /// <param name="functionToLoad">Function to load in a separate thread.</param>
        /// <param name="info">Information about what is being loaded</param>
        public static void load(ItemDelegate functionToLoad, string info)
        {
            loading = true;
            thread = new ThreadObject(functionToLoad, finished);
            t = new Thread(new ThreadStart(thread.startThread));
            t.Name = "Load: "+info;
            t.Start();
        }

        /// <summary>
        /// Called when the function has finished in the separate thread.
        /// </summary>
        private static void finished()
        {
            loading = false;
        }

    }

    /// <summary>
    /// Class used for the separate thread.
    /// </summary>
    class ThreadObject
    {
        private ItemDelegate functionToLoad;
        private ItemDelegate functionCalledWhenDone;

        /// <summary>
        /// Creates object that has a thread to carry out and thread to call once finished. 
        /// </summary>
        /// <param name="functionToLoad">Function to carry out</param>
        /// <param name="functionCalledWhenDone">Function to call once complete</param>
        public ThreadObject(ItemDelegate functionToLoad, ItemDelegate functionCalledWhenDone)
        {
            this.functionToLoad = functionToLoad;
            this.functionCalledWhenDone = functionCalledWhenDone;
        }

        /// <summary>
        /// Called once to start the thread.
        /// </summary>
        public void startThread()
        {
            #if WINDOWS
            #else
                Thread.CurrentThread.SetProcessorAffinity(new int[] { 4 });
                Thread.CurrentThread.IsBackground = true;
            #endif
            long start = DateTime.Now.Ticks;
            functionToLoad();
            double loadTime = (double)(DateTime.Now.Ticks - start) / 10000000;
            DebugDisplay.update("LOAD TIME(S)", loadTime.ToString());
            functionCalledWhenDone();
        }
    }
}
