using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class MapStatus
    {

        public MapStatus() {

        }

        public MapStatus(string name, int counter, string tag, int graphicEffect) {
            Name = name;
            Counter = counter;
            Tag = tag;
            GraphicEffect = graphicEffect;

        }

        public string Name { get; set; }
        public int Counter { get; set; }
        public String Tag { get; set; }
        public int GraphicEffect { get; set; }
    }
}
