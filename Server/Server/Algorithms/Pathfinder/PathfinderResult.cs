using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Algorithms.Pathfinder
{
    public class PathfinderResult
    {
        private List<Enums.Direction> path;
        private int tileCount;
        private bool isPath;

        public List<Enums.Direction> Path {
            get { return path; }
        }

        public int TileCount {
            get { return tileCount; }
        }

        public bool IsPath {
            get { return isPath; }
        }

        internal PathfinderResult(List<Enums.Direction> path, bool isPath) {
            this.path = path;
            tileCount = path.Count;
            this.isPath = isPath;
        }
    }
}
