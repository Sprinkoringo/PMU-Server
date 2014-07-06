using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Dungeons
{
    public interface IDungeonMap
    {
        Enums.MapType DungeonMapType { get; }
        bool IsBadGoalMap { get; set; }
        Enums.JobDifficulty Difficulty { get; set; }
        
    }
}
