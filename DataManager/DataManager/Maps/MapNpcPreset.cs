using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class MapNpcPreset
    {
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }


        public int NpcNum { get; set; }

        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }

        public int AppearanceRate { get; set; }

        public int StartStatus { get; set; }
        public int StartStatusCounter { get; set; }
        public int StartStatusChance { get; set; }


        public MapNpcPreset() {
            NpcNum = 0;
            SpawnX = -1;
            SpawnY = -1;
            MinLevel = -1;
            MaxLevel = -1;
            AppearanceRate = 100;
        }
    }
}
