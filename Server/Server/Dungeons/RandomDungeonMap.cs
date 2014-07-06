using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Dungeons
{
    public class RandomDungeonMap : IDungeonMap
    {
        public Enums.MapType DungeonMapType { get { return Enums.MapType.RDungeonMap; } }

        public bool IsBadGoalMap { get; set; }
        public Enums.JobDifficulty Difficulty { get; set; }

        public int RDungeonIndex { get; set; }
        public int RDungeonFloor { get; set; }
    }
}
