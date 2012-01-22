using System;

namespace Resonance
{
#if WINDOWS || XBOX
    static class Program
    {
        public static Game game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (game = new Game())
            {
                game.Run();
            }
        }
    }
#endif
}

