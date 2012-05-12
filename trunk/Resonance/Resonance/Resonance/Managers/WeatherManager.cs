using System;
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

        const  float maxCloudCover     = 1.2f;  // Change to set max clods in final game.
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
        public const float midLightningStart    = 0.3f;
        public const float loudLightningStart   = 0.2f;

        // Max no of ticks which have to pass between 2 lightning strikes.
        private const long maxLightningSep = 10000000; // 1 second.

        // True if thunder / lightning is happening. Reset to false when lightning ends and maxLightningSep has passed. 
        private static bool lightningHappening;

        private static bool    forceAmbLight;
        private static Vector3 forcedAmbLight;

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
            forceAmbLight = false;

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

            rain = (Rain) ParticleEmitterManager.getEmitter<Rain>();
            if (rain == null) rain = new Rain();
            rain.init(new Vector3(0f, 18f, 0f));

            Drawing.setAmbientLight(new Vector3(0.1f, 0.1f, 0.1f));
            Drawing.setSaturation(new Vector3(1f, 1f, 1f));
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

        public static void playLightning() {
            float health = gVRef.healthFraction();
            int x = gen.Next();

            if (health < loudLightningStart) {
                if (x % 3 == 0) {
                    lCue = MusicHandler.getCue("thunderLoud1");
                    lCue.Play();
                } else if (x % 3 == 1) {
                    lCue = MusicHandler.getCue("thunderLoud2");
                    lCue.Play();
                } else {
                    lCue = MusicHandler.getCue("thunderLoud3");
                    lCue.Play();
                }
            } else if (health < midLightningStart) {
                if (x % 3 == 0) {
                    lCue = MusicHandler.getCue("thunderMid1");
                    lCue.Play();
                } else if (x % 3 == 1) {
                    lCue = MusicHandler.getCue("thunderMid2");
                    lCue.Play();
                } else {
                    lCue = MusicHandler.getCue("thunderMid3");
                    lCue.Play();
                }
            } else if (health < quietLightningStart) {
                if (x % 2 == 0) {
                    lCue = MusicHandler.getCue("thunderQuiet1");
                    lCue.Play();
                } else {
                    lCue = MusicHandler.getCue("thunderQuiet2");
                    lCue.Play();
                }
            }
        }

        public static void update() {
            //MusicHandler.getTrack().inTime2();
            if (!paused) {
                setParams();
                float health = gVRef.healthFraction();

                // Cloud

                // Rain

                if (health < rainStart) {
                    if (rCue == null || !rCue.IsPlaying) {
                        //rainVol = -90f;
                        if (health < gentleLightningStart) {
                            rCue = MusicHandler.getCue("rainAndThunder");
                            MusicHandler.adjustVolume(rCue, (int)rainVol);
                            rCue.Play();
                        } else {
                            rCue = MusicHandler.getCue("rainLight");
                            MusicHandler.adjustVolume(rCue, (int)rainVol);
                            rCue.Play();
                        }
                    }

                    if ((rCue != null) && (rCue.IsPlaying)) {
                        MusicHandler.adjustVolume(rCue, (int)rainVol);
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
            if (lastLightningStarted > DateTime.Now.Ticks - lightningLength)
            {
                Drawing.setAmbientLight(new Vector3(lightningAlpha, lightningAlpha, lightningAlpha + 1));
            } else {
                if (!forceAmbLight)
                {
                    Drawing.setAmbientLight(new Vector3(0.1f - cloudCover, 0.1f - cloudCover, 0.1f - cloudCover));
                    Drawing.setSaturation(new Vector3(1f - cloudCover, 1f - cloudCover, 1f - cloudCover));
                }
                else
                {
                    Drawing.setAmbientLight(forcedAmbLight);
                }
            }
        }

        /// <summary>
        /// Returns the current default ambient light level.
        /// </summary>
        /// <returns></returns>
        public static Vector3 getCurrentAmbientLight() {
            if (!forceAmbLight) return new Vector3(0.1f - cloudCover, 0.1f - cloudCover, 0.1f - cloudCover);
            else return forcedAmbLight;
        }

        public static void forceAmbientLight(Vector3 lt) {
            forceAmbLight  = true;
            forcedAmbLight = lt;
        }

        public static void pause(bool status) {
            if (!paused && status) {
                try {
                    if (rCue  != null)  rCue.Pause();
                    if (lRCue != null) lRCue.Pause();
                    if (lCue  != null)  lCue.Pause();
                } catch (Exception e) {}
            } else if (paused && !status) {
                try {
                    if (rCue  != null)  rCue.Resume();
                    if (lRCue != null) lRCue.Resume();
                    if (lCue  != null)  lCue.Resume();
                } catch (Exception e) {}
            }
            paused = status;
        }

        public static bool isPaused() {
            return paused;
        }
    }
}
