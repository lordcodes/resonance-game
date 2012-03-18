using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {
    class WeatherManager {

        static GoodVibe gVRef;

        static bool  paused = false;

        static float cloudCover;      // Amount on cloud cover 0 - 1.
        static float cloudHeaviness;  // Opacity of cloud 0 - 1.
        static int   rainfall;        // Avg raindrops per sec.
        static float raindropSize;    // Size of raindrops.
        static float rainVol;         // Rain volume
        static float lightningFreq;   // Avg lightning per sec.
        static float lightningVol;    // Volume of thunder
        static long  thunderOffset;   // Avg temporal thunder offset after lightning.
        static float lightningAlpha;  // Alpha (brightness) of lightning. 

        const  float maxCloudCover     = 1.8f;
        const  float maxCloudHeaviness = 0.75f;
        const  int   maxRainfall       = 15;
        const  float maxRaindropSize   = 0.4f;
        const  float maxRainVol        = 10f;
        const  float maxLightningFreq  = 0.3f;
        const  float minLightningFreq  = 0.1f;
        const  float maxLightningVol   = 20f;
        const  long  maxThunderOffset  = 20000000; // 2 secs
        const  float maxLightningAlpha = 2.8f;

        public const float cloudStart           = 0.85f;
        public const float rainStart            = 0.70f;
        public const float rainMaxAt            = 0.57f;
        public const float gentleLightningStart = 0.55f;
        public const float quietLightningStart  = 0.4f;
        public const float midLightningStart    = 0.25f;
        public const float loudLightningStart   = 0.1f;

        // Max no of ticks which have to pass between 2 lightning strikes.
        private const long maxLightningSep = 10000000; // 1 second.

        // True if thunder / lightning is happening. Reset to false when lightning ends and maxLightningSep has passed. 
        private static bool lightningHappening;

        private static long lastLightning = 0;
        private static long lastLightningStarted = -1;
        private static long lightningLength = 1000000; // 0.1 seconds

        private static Cue lCue;
        private static Cue rCue;
        private static Cue lRCue;

        // Random number generator
        static Random gen;

        private static Rain rain;

        private static bool thunderStarted;

        public static void initialise() {
            gVRef = GameScreen.getGV();
            gen = new Random();

            paused = false;

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

            thunderStarted = false;

            rain = new Rain(new Vector3(0f, 10f, 0f));

            Drawing.setAmbientLight(new Vector3(0.1f, 0.1f, 0.1f));
        }

        public static void setParams() {
            float health = gVRef.healthFraction();
            float factor;

            if (health < cloudStart) {
                // Set cloudCover and cloudHeaviness
                factor = 1f / cloudStart;
                cloudHeaviness = maxCloudHeaviness - (factor * health * maxCloudHeaviness);
                cloudCover = maxCloudCover - (factor * health * maxCloudCover);
                if (health < rainStart) {
                    // Set rainfall and randropSize
                    factor = 1f / rainStart;
                    rainfall = maxRainfall - (int) (factor * health * (float) maxRainfall);
                    raindropSize = maxRaindropSize - (factor * health * maxRaindropSize);

                    rainVol = maxRainVol - (factor * health * maxRainVol);
                    rain.setEmissionsPerUpdate(rainfall);
                    rain.setRaindropSize(raindropSize);

                    rain.setPos(new Vector3 (gVRef.Body.Position.X, 10f, gVRef.Body.Position.Z));
                    if (health < quietLightningStart) {
                        // Set lightningFreq etc
                        factor = 1f / quietLightningStart;
                        lightningFreq = maxLightningFreq - (factor * health * (maxLightningFreq - minLightningFreq)) + minLightningFreq;
                        lightningVol  = maxLightningVol - (factor * health * maxLightningVol);
                        //DebugDisplay.update("Lightning vol", lightningVol.ToString());
                        //DebugDisplay.update("Lightning frq", lightningFreq.ToString());
                        thunderOffset = (long) (maxThunderOffset * factor * health);
                        lightningAlpha = maxLightningAlpha - (factor * health * maxLightningAlpha);
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
            int x = gen.Next();

            if (health < loudLightningStart) {
                if (x % 3 == 0) {
                    lCue = GameScreen.musicHandler.playSound("thunderLoud1");
                } else if (x % 3 == 1) {
                    lCue = GameScreen.musicHandler.playSound("thunderLoud2");
                } else {
                    lCue = GameScreen.musicHandler.playSound("thunderLoud3");
                }
            } else if (health < midLightningStart) {
                if (x % 3 == 0) {
                    lCue = GameScreen.musicHandler.playSound("thunderMid1");
                } else if (x % 3 == 1) {
                    lCue = GameScreen.musicHandler.playSound("thunderMid2");
                } else {
                    lCue = GameScreen.musicHandler.playSound("thunderMid3");
                }
            } else if (health < quietLightningStart) {
                if (x % 2 == 0) {
                    lCue = GameScreen.musicHandler.playSound("thunderQuiet1");
                } else {
                    lCue = GameScreen.musicHandler.playSound("thunderQuiet2");
                }
            }
        }

        public static void update() {

            if (!paused) {
                setParams();
                float health = gVRef.healthFraction();

                // Cloud

                // Rain

                if (health < rainStart) {
                    if (rCue == null || !rCue.IsPlaying) {
                        //rainVol = -90f;
                        if (health < gentleLightningStart) {
                            rCue = GameScreen.musicHandler.playSound("rainAndThunder", rainVol);
                            GameScreen.musicHandler.adjustVolume(rCue, (int) rainVol);
                        } else {
                            rCue = GameScreen.musicHandler.playSound("rainLight", rainVol);
                            GameScreen.musicHandler.adjustVolume(rCue, (int) rainVol);
                        }
                    }

                    if ((rCue != null) && (rCue.IsPlaying)) {
                        GameScreen.musicHandler.adjustVolume(rCue, (int) rainVol);
                    }

                     //DebugDisplay.update("rainVol", rainVol.ToString());
                }

                // Lightning

                if ((health < quietLightningStart) && !lightningHappening) {
                    // Flash, then wait for offset before playing sound.

                    lastLightningStarted = DateTime.Now.Ticks;
                    thunderStarted = false;
                    lightningHappening = true;
                } else {
                    if (lCue != null && (!lCue.IsPlaying && thunderStarted)) {
                        lightningHappening = false;
                    }
                }

                // Thunder
                if (lightningHappening && !thunderStarted) {
                    if (DateTime.Now.Ticks > lastLightningStarted + thunderOffset) {
                        playLightning();
                        thunderStarted = true;
                    }
                }
            }
        }

        public static void drawLightning(SpriteBatch s, Texture2D tex) {
            if (lastLightningStarted > DateTime.Now.Ticks - lightningLength) {
                Drawing.setAmbientLight(new Vector3(lightningAlpha, lightningAlpha, lightningAlpha + 1));
            } else {
                //Drawing.setAmbientLight(new Vector3(0.01f, 0.01f, 0.01f));
                Drawing.setAmbientLight(new Vector3(0.1f - cloudCover, 0.1f - cloudCover, 0.1f - cloudCover));
            }
        }

        public static void pause(bool status) {
            paused = status;

            if (paused) {
                try {
                    // Please don't change the order of these 3!
                    rCue.Pause();
                    lRCue.Pause();
                    lCue.Pause();
                } catch (Exception e) {
                }
            } else {
                try {
                    // Please don't change the order of these 3!
                    rCue.Resume();
                    lRCue.Resume();
                    lCue.Resume();
                } catch (Exception e) {}
            }
        }
    }
}
