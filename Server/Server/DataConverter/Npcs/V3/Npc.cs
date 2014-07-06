using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Npcs.V3
{
    public class Npc
    {
        public string Name { get; set; }
        public string AttackSay { get; set; }

        public int Sprite { get; set; }
        public int SpawnSecs { get; set; }

        public Enums.NpcBehavior Behavior { get; set; }
        public int Range { get; set; }

        public int Str { get; set; }
        public int Def { get; set; }
        public int Speed { get; set; }
        public int Magi { get; set; }
        public bool Big { get; set; }
        public int MaxHp { get; set; }
        public ulong Exp { get; set; }
        public int SpawnTime { get; set; }
        public NpcDrop[] Drops { get; set; }

        public int Element { get; set; }
        public int RecruitRate { get; set; }
        public int RecruitLevel { get; set; }
        public int Size { get; set; }
        public int RecruitClass { get; set; }

        public int Spell { get; set; }
        public int Frequency { get; set; }

        public string AIScript { get; set; }

        public Npc() {
            Name = "";
            AttackSay = "";
            Drops = new NpcDrop[Constants.MAX_NPC_DROPS];
            for (int i = 0; i < Constants.MAX_NPC_DROPS; i++) {
                Drops[i] = new NpcDrop();
            }
        }
    }
}
