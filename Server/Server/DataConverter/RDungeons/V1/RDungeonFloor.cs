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
using System.Text;
using Server.RDungeons;

namespace Server.DataConverter.RDungeons.V1
{
	public class RDungeonFloor
    {
        public Enums.Weather Weather { get; set; }
        public int WeatherIntensity { get; set; }

        public int[] Npc { get; set; }
        public List<int> Traps { get; set; }

        public int ItemSpawnRate { get; set; }
        public int[] Items { get; set; }

        public Enums.RFloorGoalType GoalType { get; set; }

        public int GoalMap { get; set; }
        public int GoalX { get; set; }
        public int GoalY { get; set; }

        public string Music { get; set; }

        public RDungeonFloor() {
            Traps = new List<int>();
            Npc = new int[15];
            Items = new int[8];
        }
    }
}
