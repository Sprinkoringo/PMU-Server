using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.RDungeons {
    public class DungeonArrayHall {

        public bool Open { get; set; }

        public List<DungeonPoint> TurnPoints { get; set; }

        public DungeonArrayHall() {
            TurnPoints = new List<DungeonPoint>();
        }

    }
}
