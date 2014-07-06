using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Dungeons
{
    public class StandardDungeonMap : IDungeonMap
    {
        public Enums.MapType DungeonMapType { get { return Enums.MapType.Standard; } }

        public bool IsBadGoalMap { get; set; }
        public Enums.JobDifficulty Difficulty { get; set; }

        public int MapNum { get; set; }
    }
}
