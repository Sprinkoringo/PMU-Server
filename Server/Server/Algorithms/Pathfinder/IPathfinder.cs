using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Algorithms.Pathfinder
{
    public interface IPathfinder
    {
        PathfinderResult FindPath(int startX, int startY, int endX, int endY);
    }
}
