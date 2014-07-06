using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.RDungeons {
    public class RDungeonChamberReq {
        //passed to the random dungeon generator to know what the chamber requires

        public int MinX { get; set; }
        public int MinY { get; set; }

        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public int TopPassage { get; set; }
        public int BottomPassage { get; set; }
        public int LeftPassage { get; set; }
        public int RightPassage { get; set; }

        public Enums.Acceptance TopAcceptance { get; set; }
        public Enums.Acceptance BottomAcceptance { get; set; }
        public Enums.Acceptance LeftAcceptance { get; set; }
        public Enums.Acceptance RightAcceptance { get; set; }

        public Enums.Acceptance Start { get; set; }
        public Enums.Acceptance End { get; set; }

        public RDungeonChamberReq() {
            MinX = 2;
            MaxX = 2;
            MinY = 2;
            MaxY = 2;
            TopPassage = -1;
            TopAcceptance = Enums.Acceptance.Maybe;
            BottomPassage = -1;
            BottomAcceptance = Enums.Acceptance.Maybe;
            LeftPassage = -1;
            LeftAcceptance = Enums.Acceptance.Maybe;
            RightPassage = -1;
            RightAcceptance = Enums.Acceptance.Maybe;
        }
    }
}
