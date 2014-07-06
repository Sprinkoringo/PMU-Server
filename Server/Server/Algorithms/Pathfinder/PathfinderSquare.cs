using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Algorithms.Pathfinder
{
    public class PathfinderSquare
    {
        public int DistanceSteps { get; set; }
        public bool IsPath { get; set; }
        public Enums.TileType TileType { get; set; }
    }
}
