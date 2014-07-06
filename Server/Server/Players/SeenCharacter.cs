using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Players {
    public class SeenCharacter {

        public bool InSight { get; set; }

        //most often made changes that are to be sent only when relevant
        public bool LocationOutdated { get; set; } //X, Y, Direction
        public bool ConditionOutdated { get; set; } //HP, StatusAilment, Confusion, VolatileStatus
    }
}
