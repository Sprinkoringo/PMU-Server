using System;
using System.Collections.Generic;
using System.Text;

namespace Server.RDungeons
{
    public class RDungeon
    {
        public string DungeonName { get; set; }
        public Enums.Direction Direction { get; set; }
        public bool Recruitment { get; set; }
        public bool Exp { get; set; }
        public int WindTimer { get; set;}
        public int DungeonIndex { get; set; }

        public List<RDungeonFloor> Floors { get; set; }

        

        public int RDungeonIndex;


        public RDungeon(int rDungeonIndex) {
            RDungeonIndex = rDungeonIndex;
            DungeonName = "";
            Floors = new List<RDungeonFloor>();
            
        }
    }
}
