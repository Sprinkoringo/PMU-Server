using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Missions
{
    public class Mission
    {
        public int Index { get; set; }
        public bool Hidden { get; set; }
        public string Name { get; set; }

        public Mission() {
            Hidden = false;
        }
    }
}
