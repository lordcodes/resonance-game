using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    interface VibeInterface
    {
        /// <summary>
        /// Changes vibe health by the given change value (positive or negative int)
        /// </summary>
        /// <param name="change">the amount to change the health</param>
        void AdjustHealth(int change);

        /// <summary>
        /// Sets the vibe health to a specific value
        /// </summary>
        /// <param name="value"></param>
        void SetHealth(int value);

        /// <summary>
        /// Returns the health of a vibe
        /// </summary>
        /// <returns></returns>
        int GetHealth();
    }
}
