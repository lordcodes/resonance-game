using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Shockwave : Object
    {
        // Constants
        public static double INITIAL_RADIUS = 0.05;
        public static double GROWTH_RATE = 0.6;

        // Size at which wave 'dies'
        public static double MAX_RADIUS = 10;

        // Colours
        public const int REST   = 0;

        public const int RED    = 1;
        public const int YELLOW = 2;
        public const int BLUE   = 3;
        public const int GREEN  = 4;
        public const int CYMBAL = 5;

        // Fields

        // Current radius of the shockwave
        private double radius;

        // List of Bad Vibes which this shockwave has already hit
        List<BadVibe> bVibes;
        int numHit = 0;

        // Location
        Vector3 position;
        int colour;

        // WorldTransform
        Matrix transform;

        float scoreWeight = 0f;

        private static int WAVE_POOL_SIZE = 12;
        private static List<Shockwave> wavePool;

        public Shockwave(int modelNum, string name, Vector3 pos, Matrix t, int colour, float sWeight)
            : base(modelNum, name, pos)
        {
            bVibes = new List<BadVibe>(10);
            this.init(pos, t, colour, sWeight);
        }

        public void init(Vector3 pos, Matrix t, int colour, float sWeight) {
            scoreWeight = sWeight + 0.05f;
            this.colour = colour;
            position = new Vector3(pos.X, pos.Y, pos.Z);
            transform = t;
            radius = INITIAL_RADIUS;
            Matrix scale = Matrix.CreateScale((float)INITIAL_RADIUS*2, 1.0f, (float)INITIAL_RADIUS*2);
            transform = Matrix.Multiply(transform, scale);
            Matrix translate = Matrix.CreateTranslation((float)-(INITIAL_RADIUS*2-1) * position.X, 0.0f, (float)-(INITIAL_RADIUS*2-1) * position.Z);
            transform = Matrix.Multiply(transform, translate);
            bVibes.Clear();
            numHit = 0;
        }

        public static void fillPool() {
            wavePool = new List<Shockwave>(WAVE_POOL_SIZE);
            for (int i = 0; i < WAVE_POOL_SIZE; i++) {
                wavePool.Add(new Shockwave(GameModels.SHOCKWAVE, "", Vector3.Zero, Matrix.Identity, 0, 0f));
            }
        }

        public static void addWave(Shockwave w) {
            wavePool.Add(w);
        }

        public static Shockwave getWave(Vector3 pos, Matrix t, int colour, float scoreWeight) {
            if (wavePool.Count > 0) {
                Shockwave w = wavePool.Last();
                wavePool.Remove(w);
                w.init(pos, t, colour, scoreWeight);
                return w;
            } else {
                return new Shockwave(GameModels.SHOCKWAVE, "", pos, t, colour, scoreWeight);
            }
        }

        // Methods

        public void grow()
        {
            radius *= (1 + GROWTH_RATE);

            Matrix scale;
            Matrix.CreateScale((float) (1.0f + GROWTH_RATE), 1.0f, (float) (1.0f + GROWTH_RATE), out scale);
            Matrix.Multiply(ref transform, ref scale, out transform);
            Matrix translate;
            Matrix.CreateTranslation((float)-GROWTH_RATE * position.X, 0.0f, (float)-GROWTH_RATE * position.Z, out translate);
            Matrix.Multiply(ref transform, ref translate, out transform);

            if (GameScreen.mode.MODE == GameMode.OBJECTIVES && ObjectiveManager.currentObjective() == ObjectiveManager.KILL_BOSS && GameScreen.getGV().DeflectShield > 0)
            {
                BulletManager.deflectBullet(colour - 1, position, radius);
            }
            else
            {
                BulletManager.destroyBullet(colour - 1, position, radius);
            }
        }

        public void checkBadVibes() 
        {
            List<Object> bvs = ScreenManager.game.World.returnObjectSubset<BadVibe>();

            for(int i = 0; i < bvs.Count; i++)
            {
                BadVibe bv = (BadVibe)bvs[i];
                if (bv.Status != BadVibe.State.DEAD && !bVibes.Contains(bv))
                {
                    double dist = Vector3.Distance(Position, bv.Body.Position);

                    //DebugDisplay.update("RADIUS", bv.Size + " : " + Radius + " : " + dist);

                    if (bv.Size + Radius >= dist)
                    {
                        //Collision
                        bVibes.Add(bv);

                        Vector3 blast = bv.Body.Position - position;
                        if (bv.damage(colour, blast, scoreWeight))
                        {
                            numHit++;
                        }
                    }
                }
            }
        }

        public void checkCheckpoints()
        {
            List<Object> points = ScreenManager.game.World.returnObjectSubset<Checkpoint>();

            Vector3 gvPos = GameScreen.getGV().Body.Position;
            for (int i = 0; i < points.Count; i++)
            {
                Checkpoint point = (Checkpoint)points[i];
                
                double dist = Vector3.Distance(Position, point.Position);

                if (Radius + 1 >= dist)
                {
                    if (point.hitPoint(colour-1))
                    {
                        //ScreenManager.game.World.fadeObjectAway(point, 0.6f);
                        point.ModelInstance.fadeAway(0.6f);
                        GameScreen.getGV().adjustScore(150);
                    }
                }
            }
        }

        // Properties
        public double Radius
        {
            get
            {
                return radius;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public Matrix Transform
        {
            get
            {
                return transform;
            }
        }

        public int NumberHit
        {
            get
            {
                return numHit;
            }
        }

        public int Colour
        {
            get
            {
                return colour;
            }
        }

    }
}
