using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class InstancedMap : MapDump
    {
        public InstancedMap(string mapID) : base(mapID, 20, 30) { }

        public InstancedMap(string mapID, int maxMapNpcs, int maxMapItem) :
            base(mapID, maxMapNpcs, maxMapItem) {
        }

        public int MapBase { get; set; }
    }
}
