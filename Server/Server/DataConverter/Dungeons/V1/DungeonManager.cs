/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


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
