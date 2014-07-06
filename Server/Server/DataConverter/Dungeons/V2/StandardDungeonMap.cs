using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V2
{
    public class StandardDungeonMap
    {
        public Enums.MapType DungeonMapType { get { return Enums.MapType.Standard; } }

        public bool IsBadGoalMap { get; set; }
        public int Difficulty { get; set; }

        public int MapNum { get; set; }
    }
}
