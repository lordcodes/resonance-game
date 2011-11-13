using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;

namespace Resonance
{
    class Object : DrawableGameComponent
    {
        private string identifier;
        private float xWorldCord;
        private float yWorldCord;
        private float zWorldCord;
        private int gameModelNum;
        private Capsule body;

        public Object(int modelNum, Game game) : base(game)
        {
            xWorldCord = 0;
            yWorldCord = 0;
            zWorldCord = 0;
            gameModelNum = modelNum;
        }

        public void addIdentifier(string name)
        {
            identifier = name;
        }

        public string returnIdentifier()
        {
            return identifier;
        }

        public Object(int modelNum, Game game, float x, float y, float z) : base(game)
        {
            xWorldCord = x;
            yWorldCord = y;
            zWorldCord = z;
            gameModelNum = modelNum;
        }

        public void setCords(float x, float y, float z)
        {
            setXWorldCord(x);
            setYWorldCord(y);
            setZWorldCord(z);
        }

        public void setXWorldCord(float x)
        {
            xWorldCord = x;
        }

        public void setYWorldCord(float y)
        {
            yWorldCord = y;
        }

        public void setZWorldCord(float z)
        {
            zWorldCord = z;
        }

        public float[] getCords()
        {
            float[] cords = new float[3];
            cords[0] = xWorldCord;
            cords[1] = yWorldCord;
            cords[2] = zWorldCord;
            return cords;
        }

        public float getXWorldCord()
        {
            return xWorldCord;
        }

        public float getZWorldCord()
        {
            return zWorldCord;
        }

        public float getYWorldCord()
        {
            return yWorldCord;
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 pos = new Vector3(xWorldCord, yWorldCord, zWorldCord);
            Drawing.Draw(gameModelNum, Matrix.CreateTranslation(pos));
            base.Draw(gameTime);
        }
    }
}
