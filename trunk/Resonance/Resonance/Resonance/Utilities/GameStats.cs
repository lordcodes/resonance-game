using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resonance
{
    class GameStats
    {
        private int score;
        private int bvsKilled;
        private int bvsAtOnce;
        private int healthCritical;
        private int nitroUses;
        private int shieldUses;
        private int freezeUses;
        private int powerups;

        public GameStats()
        {
            score = 0;
            bvsKilled = 0;
            bvsAtOnce = 0;
            healthCritical = 0;
            nitroUses = 0;
            shieldUses = 0;
            freezeUses = 0;
            powerups = 0;
        }

        public void addScore(int value)
        {
            score += value;
        }

        public void addBV() { bvsKilled++; }
        public void criticalHealth() { healthCritical++; }
        public void usedNitro() { nitroUses++; }
        public void usedShield() { shieldUses++; }
        public void usedFreeze() { freezeUses++; }
        public void gotPowerup() { powerups++; }

        public void multiKill(int number)
        {
            bvsAtOnce = Math.Max(bvsAtOnce, number);
        }

        public int Score
        {
            get { return score; }
        }

        public int BVsKilled
        {
            get { return bvsKilled; }
        }

        public int Multikill
        {
            get { return bvsAtOnce; }
        }

        public int HealthCritical
        {
            get { return healthCritical; }
        }

        public int NitroUses
        {
            get { return nitroUses; }
        }

        public int ShieldUses
        {
            get { return shieldUses; }
        }

        public int FreezeUses
        {
            get { return freezeUses; }
        }

        public int Powerups
        {
            get { return powerups; }
        }
    }
}
