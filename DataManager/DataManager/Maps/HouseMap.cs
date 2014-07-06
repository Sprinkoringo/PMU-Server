using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class HouseMap : MapDump
    {
        public HouseMap(string mapID)
            : base(mapID, 20, 30) {

        }

        public string Owner { get; set; }
        public int Room { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
    }
}
