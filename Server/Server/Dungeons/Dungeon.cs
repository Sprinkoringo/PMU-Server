using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Dungeons
{
    public class Dungeon
    {
        private List<StandardDungeonMap> standardMaps;
        private List<RandomDungeonMap> randomMaps;

        public string Name { get; set; }

        public List<StandardDungeonMap> StandardMaps {
            get { return standardMaps; }
        }
        public List<RandomDungeonMap> RandomMaps
        {
            get { return randomMaps; }
        }

        public bool AllowsRescue { get; set; }

        public ListPair<int, string> ScriptList { get; set; }

        public Dungeon() {
            standardMaps = new List<StandardDungeonMap>();
            randomMaps = new List<RandomDungeonMap>();
            Name = "";
            ScriptList = new ListPair<int, string>();
        }
    }
}
