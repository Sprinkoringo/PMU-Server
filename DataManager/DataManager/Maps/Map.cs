using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class Map : MapDump
    {
        public Map(string mapID)
            : base(mapID, 20, 30) {
        }

        public bool Instanced { get; set; }
    }
}
