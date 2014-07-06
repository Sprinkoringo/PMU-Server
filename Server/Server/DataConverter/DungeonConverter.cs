using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter
{
    public class DungeonConverter
    {
        public static void ConvertV1toV2(int num)
        {
            Dungeons.V1.Dungeon dungeonV1 = Dungeons.V1.DungeonManager.LoadDungeon(num);
            Dungeons.V2.Dungeon dungeonV2 = new Dungeons.V2.Dungeon();

            dungeonV2.Name = dungeonV1.Name;
            dungeonV2.AllowsRescue = true;
            foreach (Dungeons.V1.DungeonMap dungeonMap in dungeonV1.Maps)
            {
                Dungeons.V2.StandardDungeonMap standardMap = new Dungeons.V2.StandardDungeonMap();
                standardMap.Difficulty = dungeonMap.Difficulty;
                standardMap.IsBadGoalMap = dungeonMap.IsBadGoalMap;
                standardMap.MapNum = dungeonMap.MapNumber;
                dungeonV2.StandardMaps.Add(standardMap);
            }

            Dungeons.V2.DungeonManager.SaveDungeon(num, dungeonV2);
        }
    }
}
