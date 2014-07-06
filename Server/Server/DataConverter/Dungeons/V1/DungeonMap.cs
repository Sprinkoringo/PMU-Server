using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V1
{
    public class DungeonMap
    {
        public int MapNumber { get; set; }
        public bool IsBadGoalMap { get; set; }
        public int Difficulty { get; set; }
        public string GoalName { get; set; }
        public int FloorNum { get; set; }
    }
}
