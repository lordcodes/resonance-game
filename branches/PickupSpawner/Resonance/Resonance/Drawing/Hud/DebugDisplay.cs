using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class DebugDisplay
    {
        private static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>
        /// This is called when you would like to display/update debug info.
        /// </summary>
        /// <param name="name">The name of the debug info</param>
        /// <param name="message">The value of the debug info</param>
        public static void update(String name, String message)
        {
            if (dictionary.ContainsKey(name)) dictionary[name] = message;
            else dictionary.Add(name, message);
        }

        /// <summary>
        /// Create single string of debug info
        /// </summary>
        /// <returns>Single string of debug info</returns>
        public static string getString()
        {
            string text = "DEBUG INFO\n";

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                text += pair.Key+":"+pair.Value+"\n";
            }

            return text;
        }
    }
}
