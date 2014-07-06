using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Players
{
    public class RecruitData
    {
        public bool UsingTempStats { get; set; }
        public string Name { get; set; }
        public bool Nickname { get; set; }
        public int NpcBase { get; set; }
        public int Species { get; set; }
        public int Form { get; set; }
        public int Shiny { get; set; }
        public byte Sex { get; set; }
        public int HeldItemSlot { get; set; }
        public int Level { get; set; }
        public ulong Exp { get; set; }
        public int HP { get; set; }
        public int StatusAilment { get; set; }
        public int StatusAilmentCounter { get; set; }
        public int IQ { get; set; }
        public int Belly { get; set; }
        public int MaxBelly { get; set; }
        public int AtkBonus { get; set; }
        public int DefBonus { get; set; }
        public int SpeedBonus { get; set; }
        public int SpclAtkBonus { get; set; }
        public int SpclDefBonus { get; set; }

        public Characters.Move[] Moves { get; set; }
        public List<Characters.VolatileStatus> VolatileStatus { get; set; }


        public RecruitData() {
            HeldItemSlot = -1;
            Moves = new Characters.Move[4];
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i] = new Characters.Move();
            }
            VolatileStatus = new List<Characters.VolatileStatus>();
        }

    }
}
