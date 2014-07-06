using System;
using System.Collections.Generic;
using System.Text;
using Server.RDungeons;

namespace Server.DataConverter.RDungeons.V3
{
    public class RDungeon
    {
        

        
        public string DungeonName { get; set; }
        public Enums.Direction Direction { get; set; }
        public int MaxFloors { get; set; }
        public bool Recruitment { get; set; }
        public bool Exp { get; set; }
        public int WindTimer { get; set;}

        public List<RDungeonFloor> Floors { get; set; }

        

        public int DungeonIndex;

        public RDungeon(int dungeonIndex) {
            DungeonIndex = dungeonIndex;
            DungeonName = "";
            Floors = new List<RDungeonFloor>();
            
        }
    }
}
