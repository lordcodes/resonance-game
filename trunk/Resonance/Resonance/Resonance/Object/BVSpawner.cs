using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using BEPUphysics.Collidables;
using Microsoft.Xna.Framework;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.Entities;

namespace Resonance
{
    class BVSpawner : StaticObject
    {
        private int totalBadVibes;
        private int spawnRadius;
        private int totalActive;
        private List<BadVibe> badVibes;

        public BVSpawner(int modelNum, String name, Vector3 pos, int TotalBadVibes, int SpawnRadious, int TotalActive)
            : base(modelNum, name, pos)
        {
            badVibes = new List<BadVibe>();
            totalBadVibes = TotalBadVibes;
            spawnRadius = SpawnRadious;
            totalActive = TotalActive;
        }

        public void addBadVibe(BadVibe bv)
        {
            badVibes.Add(bv);
        }

        public void removeBadVibe(BadVibe bv)
        {
            badVibes.Remove(bv);
        }

        public int getTotalBadVibes()
        {
            return totalBadVibes;
        }

        public int getTotalActive()
        {
            return badVibes.Count;
        }

    }
}
