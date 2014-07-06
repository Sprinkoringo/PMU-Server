using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V2
{
    public class DungeonManager
    {
        public static void SaveDungeon(int dungeonNum, Dungeon dungeon)
        {
            
            string Filepath = IO.Paths.DungeonsFolder + "dungeon" + dungeonNum.ToString() + ".dat";
            
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Filepath))
            {
                writer.WriteLine("DungeonData|V2");
                writer.WriteLine("Data|" + dungeon.Name + "|" + dungeon.AllowsRescue + "|");
                for (int i = 0; i < dungeon.StandardMaps.Count; i++)
                {
                    writer.WriteLine("SMap|" + dungeon.StandardMaps[i].Difficulty + "|" + dungeon.StandardMaps[i].IsBadGoalMap + "|"
                         + dungeon.StandardMaps[i].MapNum + "|");

                }
                for (int i = 0; i < dungeon.RandomMaps.Count; i++)
                {
                    writer.WriteLine("Map|" + dungeon.RandomMaps[i].Difficulty + "|" + dungeon.RandomMaps[i].IsBadGoalMap + "|"
                         + dungeon.RandomMaps[i].RDungeonIndex + "|" + dungeon.RandomMaps[i].RDungeonFloor + "|");

                }
            }
        }
    }
}
