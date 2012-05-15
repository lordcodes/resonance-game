using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    public class DisplacementMap
    {
        // Variables that control the wave dimensions, changing wave width will probably give weird looking results.
        public const float WAVE_HEIGHT = 1f;
        public const float WAVE_WIDTH = 1f;
        public const float WAVE_SPEED = 0.4f;

        private GraphicsDevice graphicsDevice;
        private int width;
        private int height;
        private float[] buffer;
        private List<float[]> masterBuffer;
        private float[] damageBuffer;
        private float[] emptyBuffer;
        private Vector2 lastPosition;
        private Vector2 gVPosition;
        private Texture2D dispMap = null;
        private int waveCount = 0;
        private static Dictionary<string, float> distanceValues = new Dictionary<string, float>();
        private Wave[] waves = new Wave[1];

        private int half;
        private int max;

        /// <summary>
        /// Reset the displacement map so everything is flat
        /// </summary>
        public void reset()
        {
            for (int i = 0; i < buffer.Length; i++) buffer[i] = damageBuffer[i];
            dispMap = new Texture2D(graphicsDevice, width, height, true, SurfaceFormat.Single);
            dispMap.SetData<float>(buffer, 0, buffer.Length);
        }

        public void setGD(GraphicsDevice nGraphics)
        {
            graphicsDevice = nGraphics;
        }


        public DisplacementMap(List<float[]> masterBuffer, int nWidth, int nHeight)
        {
            width = nWidth;
            height = nHeight;
            damageBuffer = new float[width * height];
            buffer = new float[width * height];
            emptyBuffer = new float[width * height];
            lastPosition = new Vector2(-1, -1);
            for (int i = 0; i < buffer.Length; i++) buffer[i] = 0f;
            for (int i = 0; i < damageBuffer.Length; i++) damageBuffer[i] = 0f;
            for (int i = 0; i < emptyBuffer.Length; i++) emptyBuffer[i] = 0f;

            this.masterBuffer = masterBuffer;

            half = (int)Math.Round((double)Graphics.DISP_WIDTH / 2);
            max = Graphics.DISP_WIDTH * Graphics.DISP_WIDTH;
        }


        public void addHole(float x , float y)
        {
            int index = (int)Math.Round(x) + (int)Math.Round(y * width);
            if (index < damageBuffer.Length && index >= 0)
            {
                damageBuffer[index] = -0.5f;
            }
        }

        public void addWave(Vector2 position)
        {
            //addHole(position.X, position.Y);
            if (waveCount >= waves.Length) waveCount = 0;
            waves[waveCount] = new Wave(position, masterBuffer.Count);
            waveCount++;
        }

        public Texture2D getMap()
        {
            return dispMap;
        }

        private Texture2D getTexture(float[] buffer)
        {
            Color[] foregroundColors = new Color[width * height];
            for (int i = 0; i < buffer.Length ; i++)
            {
                foregroundColors[i] = new Color(buffer[i], buffer[i], buffer[i]);
            }
            Texture2D newMap = new Texture2D(graphicsDevice, width, height, true, SurfaceFormat.Color);
            newMap.SetData<Color>(foregroundColors, 0, foregroundColors.Length);
            return newMap;
        }

        public List<Texture2D> getTextures()
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int i = 0; i < masterBuffer.Count; i++)
            {
                textures.Add(getTexture(masterBuffer[i]));
            }
            return textures;
        }

        public void createMap()
        {
            int itcount = 0;
            int buff = 17;
            
            int xlow = (int)gVPosition.X - buff;
            int xhigh = (int)gVPosition.X + buff;
            int ylow = (int)gVPosition.Y - buff;
            int yhigh = (int)gVPosition.Y + buff;
            /*
            int xlow = 0;
            int xhigh = width;
            int ylow = 0;
            int yhigh = width;
            */


            if (waveCount > 0)
            {
                float depth;
                for (int xi = xlow; xi < xhigh; xi++)
                {
                    itcount++;
                    for (int yi = ylow; yi < yhigh; yi++)
                    {
                        depth = 0;
                        for (int i = 0; i < waves.Length; i++)
                        {
                            itcount++;
                            if (waves[i] != null)
                            {
                                int index = (xi+half-waves[i].X) + (yi+half-waves[i].Y) * width;
                                if (index >= 0 && index < max)
                                {
                                    depth += masterBuffer[waves[i].Frame][index]*0.5f;
                                }
                            }
                        }
                        updateBuffer(xi, yi, depth);
                    }
                }
                dispMap = new Texture2D(graphicsDevice, width, height, true, SurfaceFormat.Single);
                dispMap.SetData<float>(buffer, 0, buffer.Length);
            }
            else
            {
                reset();
            }
            //DebugDisplay.update("DMAPI",itcount+"");
        }

        public void update(Vector2 position)
        {
            gVPosition = position;
            bool update = false;
            int currentwc = waveCount;
            for (int i = 0; i < waves.Length; i++)
            {
                if (waves[i] != null)
                {
                    update = true;
                    waves[i].update();
                    if (waves[i].Distance > 90 || !waves[i].IsActive)
                    {
                        waves[i] = null;
                        waveCount--;
                        if (waveCount < 0) waveCount = 0;
                    }
                }
            }

            if (waveCount != currentwc && waveCount <= 0) reset();
            if (update) createMap();
        }

        private float waveDepth(float distance, float distanceFrom, float height)
        {
            float depth = 0;
            depth = gaussianFunction(distance, distanceFrom, height);
            return depth;
        }

        private float gaussianFunction(float x, float distanceFrom, float height)
        {
            float result = 0;
            float peakCenter = distanceFrom;
            float waveWidth = WAVE_WIDTH;
            result = (float)(height * Math.Pow(Math.E, -(((x - peakCenter) * (x - peakCenter)) / (2 * (waveWidth * waveWidth)))));
            //result = 0;
            return result;
        }

        private float trigDistance(int x, int y, Vector2 position)
        {
            float distance = 0f;
            float dx = x - position.X;
            float dy = y - position.Y;
            distance = (float)Math.Sqrt((dx*dx)+(dy*dy));
            return distance;
        }

        private void updateBuffer(float x, float y, float value)
        {
            int index = (int)Math.Round(x) + (int)Math.Round(y * width);
            if (index < buffer.Length && index>= 0)
            {
                buffer[index] = value+damageBuffer[index];
            }
        }
    }

    class Wave
    {

        private Vector2 epicenter;
        private bool isActive;
        private float distance;
        private float speed;
        private float height;
        private int frames = 0;
        private int frameCounter = 0;

        public int X
        {
            get
            {
                return (int)epicenter.X;
            }
        }
        public int Y
        {
            get
            {
                return (int)epicenter.Y;
            }
        }

        public bool IsActive
        {
            get
            {
                if (frames != 0 && frameCounter >= frames) return false;
                return isActive;
            }
        }
        public float Height { get { return height; } }
        public int Frame { get { return frameCounter; } }
        public Vector2 Epicenter { get { return epicenter; } }
        public float Distance { get { return distance; } }

        public Wave(Vector2 epicenter, int frames)
        {
            this.epicenter = epicenter;
            isActive = true;
            distance = 0;
            speed = DisplacementMap.WAVE_SPEED;
            height = DisplacementMap.WAVE_HEIGHT;
            this.frames = frames;
        }

        public Wave(Vector2 epicenter)
        {
            this.epicenter = epicenter;
            isActive = true;
            distance = 0;
            speed = DisplacementMap.WAVE_SPEED;
            height = DisplacementMap.WAVE_HEIGHT;
        }

        public void update()
        {
            if (frames > 0) frameCounter++;
            distance += speed;
            height -= 0.005f;
            if (height < 0)
            {
                height = 0;
                isActive = false;
            }
        }
    }
}
