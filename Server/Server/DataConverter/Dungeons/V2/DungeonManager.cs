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
