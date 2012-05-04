using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Resonance
{
    class ThreadRun
    {
        private Thread t;
        private ThreadClass thread;
        private bool isRunning;

        public ThreadRun()
        {
            isRunning = false;
        }

        /// <summary>
        /// Carrys out the function in another thread.
        /// </summary>
        /// <param name="functionToLoad">Function to run in a separate thread.</param>
        /// <param name="info">Information about what is being ran</param>
        public void run(Action runFunction, string info)
        {
            isRunning = true;
            thread = new ThreadClass(runFunction, finished);
            t = new Thread(new ThreadStart(thread.startThread));
            t.Name = "Run: " + info;
            t.Start();
        }

        /// <summary>
        /// Called when the function has finished in the separate thread.
        /// </summary>
        private void finished()
        {
            isRunning = false;
        }

        public bool Running
        {
            get
            {
                return isRunning;
            }
        }
    }

    /// <summary>
    /// Class used for the separate thread.
    /// </summary>
    class ThreadClass
    {
        private Action runFunction;
        private Action finishFunction;

        /// <summary>
        /// Creates object that has a thread to carry out and thread to call once finished. 
        /// </summary>
        /// <param name="functionToLoad">Function to carry out</param>
        /// <param name="functionCalledWhenDone">Function to call once complete</param>
        public ThreadClass(Action runFunction, Action finishFunction)
        {
            this.runFunction = runFunction;
            this.finishFunction = finishFunction;
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
            runFunction();
            finishFunction();
        }
    }
}
