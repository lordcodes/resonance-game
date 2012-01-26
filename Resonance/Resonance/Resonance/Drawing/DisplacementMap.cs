using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class DisplacementMap
    {
        public const float WAVE_HEIGHT = 1f;
        public const float WAVE_WIDTH = 1f;
        public const float WAVE_SPEED = 0.2f;

        private GraphicsDevice graphicsDevice;
        private int width;
        private int height;
        private float[] buffer;
        private float[] damageBuffer;
        private Vector2 lastPosition;
        private Texture2D dispMap;
        private int waveCount = 0;
        private static Dictionary<string, float> distanceValues = new Dictionary<string, float>();
        private Wave[] waves = new Wave[5];

        public void reset()
        {
            for (int i = 0; i < buffer.Length; i++) buffer[i] = damageBuffer[i];
            dispMap = new Texture2D(graphicsDevice, width, height, true, SurfaceFormat.Single);
            dispMap.SetData<float>(buffer, 0, buffer.Length);
        }

        public DisplacementMap(GraphicsDevice nGraphics, int nWidth, int nHeight)
        {
            graphicsDevice = nGraphics;
            width = nWidth;
            height = nHeight;
            damageBuffer = new float[width * height];
            buffer = new float[width * height];
            lastPosition = new Vector2(-1,-1);
            for (int i = 0; i < buffer.Length; i++)buffer[i] = 0f;
            for (int i = 0; i < damageBuffer.Length; i++)damageBuffer[i] = 0f;
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
            addHole(position.X, position.Y);
            if (waveCount >= waves.Length) waveCount = 0;
            waves[waveCount] = new Wave(position);
            waveCount++;
        }

        public Texture2D getMap()
        {
            return dispMap;
        }

        public void createMap()
        {
            float dis;
            float ndepth;
            if (waveCount > 0)
            {
                float depth;
                for (int xi = 0; xi < width; xi++)
                {
                    for (int yi = 0; yi < width; yi++)
                    {
                        depth = 0;
                        for (int i = 0; i < waves.Length; i++)
                        {
                            if (waves[i] != null)
                            {
                                dis = trigDistance(xi, yi, waves[i].Epicenter);
                                ndepth = waveDepth(dis, waves[i].Distance, waves[i].Height);
                                depth += ndepth;
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
        }

        public void update(Vector2 position)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                if (waves[i] != null)
                {
                    waves[i].update();
                    if (waves[i].Distance > 90 || !waves[i].IsActive)
                    {
                        waves[i] = null;
                        waveCount--;
                        if (waveCount < 0) waveCount = 0;
                    }
                }
            }

            if (waveCount <= 0) reset();
            createMap();
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
            result = (float)(height * Math.Pow(Math.E,-(((x-peakCenter)*(x-peakCenter))/(2*(waveWidth*waveWidth)))));
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

        public bool IsActive { get { return isActive; } }
        public float Height { get { return height; } }
        public Vector2 Epicenter { get { return epicenter; } }
        public float Distance { get { return distance; } }

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
            distance += speed;
            height -= 0.01f;
            if (height < 0)
            {
                height = 0;
                isActive = false;
            }
        }
    }
}
