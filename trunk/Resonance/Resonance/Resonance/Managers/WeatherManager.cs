using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Resonance {
    class WeatherManager {

        static GoodVibe gVRef;

        static float cloudCover;      // Amount on cloud cover 0 - 1.
        static float cloudHeaviness;  // Opacity of cloud 0 - 1.
        static int   rainfall;        // Avg raindrops per sec.
        static float raindropSize;    // Size of raindrops.
        static float lightningFreq;   // Avg lightning per sec.
        static float lightningVol;    // Volume of thunder
        static long  thunderOffset;   // Avg temporal thunder offset after lightning.
        static float lightningAlpha;  // Alpha (brightness) of lightning. 

        const  float maxCloudCover     = 1f;
        const  float maxCloudHeaviness = 0.75f;
        const  int   maxRainfall       = 25;
        const  float maxRaindropSize   = 1f;
        const  float maxLightningFreq  = 0.3f;
        const  float minLightningFreq  = 0.1f;
        const  float maxLightningVol   = 10f;
        const  long  maxThunderOffset  = 300000000000;
        const  float maxLightningAlpha = 0.9f;

        public const float cloudStart          = 0.80f;
        public const float rainStart           = 0.60f;
        public const float quietLightningStart = 0.4f;
        public const float midLightningStart   = 0.25f;
        public const float loudLightningStart  = 0.1f;

        // Max no of ticks which have to pass between 2 lightning strikes.
        private const long maxLightningSep = 100000000000; // 1 second.

        // True if thunder / lightning is happening. Reset to false when lightning ends and maxLightningSep has passed. 
        private static bool lightningHappening;

        private static long lastLightning = 0;

        private static Cue lCue;

        // Random number generator
        static Random gen;

        public static void initialise() {
            gVRef = GameScreen.getGV();
            gen = new Random();

            cloudCover     = 0f;
            cloudHeaviness = 0f;
            rainfall       =  0;
            raindropSize   = 0f;
            lightningFreq  = 0f;
            lightningVol   = 0f;
            thunderOffset  = maxThunderOffset;
            lightningAlpha = 0f;

            lightningHappening = false;
            lastLightning = -1;
            lCue = null;
        }

        public static void setParams() {
            float health = gVRef.healthFraction();
            float factor;

            if (health < cloudStart) {
                // Set cloudCover and cloudHeaviness
                factor = 1f / cloudStart;
                cloudHeaviness = maxCloudHeaviness - (factor * health * maxCloudHeaviness);
                if (health < rainStart) {
                    // Set rainfall and randropSize
                    factor = 1f / rainStart;
                    rainfall = maxRainfall - (int) (factor * health * (float) maxRainfall);
                    raindropSize = maxRaindropSize - (factor * health * maxRaindropSize);
                    if (health < quietLightningStart) {
                        // Set lightningFreq etc
                        factor = 1f / quietLightningStart;
                        lightningFreq = maxLightningFreq - (factor * health * (maxLightningFreq - minLightningFreq)) + minLightningFreq;
                        lightningVol  = maxLightningVol - (factor * health * maxLightningVol);
                        thunderOffset = (long) (maxThunderOffset * factor * health);
                    } else {
                        lightningFreq = 0f;
                    }
                } else {
                    rainfall = 0;
                }
            } else {
                cloudCover = 0f;
                cloudHeaviness = 0f;
            }
        }

        private static void playLightning() {
            float health = gVRef.healthFraction();

            if (health < loudLightningStart) {
                int x = gen.Next();
                if (x % 3 == 0) {
                    lCue = ScreenManager.game.Music.playSound("thunderLoud1");
                } else if (x % 3 == 1) {
                    lCue = ScreenManager.game.Music.playSound("thunderLoud2");
                } else {
                    lCue = ScreenManager.game.Music.playSound("thunderLoud3");
                }
            } else if (health < midLightningStart) {
                lCue = ScreenManager.game.Music.playSound("thunderMid1");
            } else if (health < quietLightningStart) {
                lCue = ScreenManager.game.Music.playSound("thunderQuiet2");
            }
        }

        public static void update() {
            setParams();

            // Cloud

            // Rain

            // Lightning

            if (!lightningHappening) {
                playLightning();
                lightningHappening = true;
            } else {
                if (lCue == null || !lCue.IsPlaying) {
                    lightningHappening = false;
                }
            }
        }
    }
}
