using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class AI
    {
        private const int PATHFIND_RATE = 60;
        private int pathIteration = 0;
        
        private List<Vector3> path;
        private int step;

        private ThreadRun thread;

        public AI()
        {
            thread = new ThreadRun();
        }

        public void runAStar(BadVibe enemy, Vector3 target)
        {
            if (!thread.Running)
            {
                thread.run(delegate { aStar(enemy.Body.Position, target); }, enemy.returnIdentifier() + " path-finding.");
            }
        }

        private void aStar(Vector3 from, Vector3 to)
        {
            PathFind pathFind = new PathFind();
            List<Vector3> tempPath = pathFind.find(from, to);
            if (tempPath != null)
            {
                step = 0;
                path = tempPath;
            }
        }

        public Vector3 calculateStep(BadVibe bv)
        {
            Vector3 gvPos = GameScreen.getGV().Body.Position;
            if (pathIteration == 0)
            {
                //Do path-finding to get target
                runAStar(bv, gvPos);
                if (path == null)
                {
                    pathIteration++;
                    return bv.Body.Position;
                }
            }
            else
            {
                if (path == null)
                {
                    pathIteration++;
                    return gvPos;
                }
                //Move along path
                if (desinationReached(bv.Body.Position)) step++;
            }
            pathIteration++;

            if (pathIteration == (PATHFIND_RATE-1)) pathIteration = 0;

            return path[step];
            //return path[path.Count - 1];
        }

        private bool desinationReached(Vector3 bv)
        {
            if (Math.Abs(path[step].X - bv.X) < 0.5f)
            {
                if (Math.Abs(path[step].Z - bv.Z) < 0.5f)
                {
                    return true;
                }
            }
            return false;
        }

        /*public List<Vector3> getPath()
        {
            if (!thread.Running)
            {
                return path;
            }
            else return null;
        }*/

        public bool isRunning()
        {
            return thread.Running;
        }
    }
}
