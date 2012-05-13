using System;

namespace Resonance
{
    class GameStats
    {
        private static int score;
        private static int bvsKilled;
        private static int bvsAtOnce;
        private static TimeSpan round1;
        private static TimeSpan round2;
        private static int round3;
        private static TimeSpan round4;
        private static TimeSpan round5;
        private static int[] scoreBonus;
        
        public static void init()
        {
            score = 0;
            bvsKilled = 0;
            bvsAtOnce = 0;
            round1 = TimeSpan.Zero;
            round2 = TimeSpan.Zero;
            round3 = 0;
            round4 = TimeSpan.Zero;
            round5 = TimeSpan.Zero;
            scoreBonus = new int[] { 0, 0, 0, 0, 0 };
        }

        public static void addBV() { bvsKilled++; }

        public static void multiKill(int number)
        {
            bvsAtOnce = Math.Max(bvsAtOnce, number);
        }

        public static int Score
        {
            get { return score; }
            set { score += value; }
        }

        public static int BVsKilled
        {
            get { return bvsKilled; }
        }

        public static int Multikill
        {
            get { return bvsAtOnce; }
        }

        public static TimeSpan Round1
        {
            get { return round1; }
            set { round1 = value; }
        }

        public static TimeSpan Round2
        {
            get { return round2; }
            set { round2 = value; }
        }

        public static int Round3
        {
            get { return round3; }
            set { round3 += value; }
        }

        public static TimeSpan Round4
        {
            get { return round4; }
            set { round4 = value; }
        }

        public static TimeSpan Round5
        {
            get { return round5; }
            set { round5 = value; }
        }

        public static int getScoreBonus(int i)
        {
            return scoreBonus[i];
        }

        public static void setScoreBonus(int i, int value)
        {
            scoreBonus[i] = value;
        }
    }
}
