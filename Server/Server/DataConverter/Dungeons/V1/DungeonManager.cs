using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V1
{
    public class DungeonManager
    {
        public static Dungeon LoadDungeon(int dungeonNum)
        {
            Dungeon dungeon = new Dungeon();
            string FilePath = IO.Paths.DungeonsFolder + "dungeon" + dungeonNum.ToString() + ".dat";
            using (System.IO.StreamReader reader = new System.IO.StreamReader(FilePath))
            {
                while (!(reader.EndOfStream))
                {
                    string[] parse = reader.ReadLine().Split('|');
                    switch (parse[0].ToLower())
                    {
                        case "dungeondata":
                            {
                                if (parse[1].ToLower() != "v1")
                                {
                                    reader.Close();
                                    //reader.Dispose();
                                    return null;
                                }
                            }
                            break;
                        case "data":
                            {
                                dungeon.Name = parse[1];
                            }
                            break;
                        case "map":
                            {
                                DungeonMap map = new DungeonMap();
                                map.MapNumber = parse[1].ToInt();
                                map.Difficulty = parse[2].ToInt();
                                map.IsBadGoalMap = parse[3].ToBool();
                                map.GoalName = parse[4];
                                map.FloorNum = parse[5].ToInt();
                                dungeon.Maps.Add(map);
                            }
                            break;
                    }
                }
            }
            return dungeon;
        }


        public static void SaveDungeon(int dungeonNum, Dungeon dungeon)
        {
            string Filepath = IO.Paths.DungeonsFolder + "dungeon" + dungeonNum.ToString() + ".dat";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Filepath))
            {
                writer.WriteLine("DungeonData|V1");
                writer.WriteLine("Data|" + dungeon.Name + "|");
                for (int i = 0; i < dungeon.Maps.Count; i++)
                {
                    writer.WriteLine("Map|" + dungeon.Maps[i].MapNumber.ToString() + "|" + dungeon.Maps[i].Difficulty + "|"
                                     + dungeon.Maps[i].IsBadGoalMap.ToString() + "|" + dungeon.Maps[i].GoalName + "|"
                                     + dungeon.Maps[i].FloorNum.ToString() + "|");
                }
            }
        }


    }
}
