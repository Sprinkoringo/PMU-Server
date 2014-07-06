using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Moves.V1
{
    public class Move
    {
        public string Name { get; set; }
        public int ClassReq { get; set; }
        public int ClassReq2 { get; set; }
        public int ClassReq3 { get; set; }
        public int LevelReq { get; set; }
        public int MPCost { get; set; }
        public int Sound { get; set; }
        public Enums.MoveType Type { get; set; }
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Range { get; set; }
        public int SpellAnim { get; set; }
        public int SpellTime { get; set; }
        public int SpellDone { get; set; }
        public bool AE { get; set; }
        public bool Big { get; set; }
        public int Element { get; set; }

        public bool IsKey { get; set; }
        public int KeyItem { get; set; }
    }
}
