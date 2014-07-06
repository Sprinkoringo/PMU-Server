using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V2
{
    public class RandomDungeonMap
    {
        public Enums.MapType DungeonMapType { get { return Enums.MapType.RDungeonMap; } }

        public bool IsBadGoalMap { get; set; }
        public int Difficulty { get; set; }

        public int RDungeonIndex { get; set; }
        public int RDungeonFloor { get; set; }
    }
}
