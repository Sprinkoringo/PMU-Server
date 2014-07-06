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
