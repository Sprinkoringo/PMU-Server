using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class RDungeonMap : MapDump
    {
        public RDungeonMap(string mapID)
            : base(mapID, 20, 30) {

        }

        public int RDungeonIndex { get; set; }
        public int RDungeonFloor { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
    }
}
