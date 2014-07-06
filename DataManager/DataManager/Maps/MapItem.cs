using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class MapItem
    {
        public int Num { get; set; }
        public int Value { get; set; }
        public bool Sticky { get; set; }
        public bool Hidden { get; set; }
        public string Tag { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string PlayerFor { get; set; }
        public int TimeRemaining { get; set; }
    }
}
