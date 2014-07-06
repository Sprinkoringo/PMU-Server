using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Dungeons.V1
{
    public class Dungeon
    {
        private List<DungeonMap> maps;
        public string Name { get; set; }

        public List<DungeonMap> Maps
        {
            get { return maps; }
        }

        public Dungeon()
        {
            maps = new List<DungeonMap>();
            Name = "";
        }
    }
}
