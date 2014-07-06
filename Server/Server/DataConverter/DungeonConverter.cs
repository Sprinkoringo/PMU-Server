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
