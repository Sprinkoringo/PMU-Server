using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class WarpDestination
    {
        public string MapID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public WarpDestination(string mapID, int x, int y) {
            this.MapID = mapID;
            this.X = x;
            this.Y = y;
        }
    }
}
