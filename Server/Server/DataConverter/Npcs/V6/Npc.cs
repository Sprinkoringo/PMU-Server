using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Npcs.V6
{
    public class Npc
    {
        public string Name { get; set; }
        public string AttackSay { get; set; }

        public int Sprite { get; set; }

        public Enums.NpcBehavior Behavior { get; set; }
        public int Range { get; set; }

        public int Species { get; set; }
        public bool SpawnsAtDay { get; set; }
        public bool SpawnsAtNight { get; set; }
        public bool SpawnsAtDawn { get; set; }
        public bool SpawnsAtDusk { get; set; }
        public NpcDrop[] Drops { get; set; }

        public int RecruitRate { get; set; }

        public int[] Moves { get; set; }

        public string AIScript { get; set; }

        public Npc() {
            Name = "";
            AttackSay = "";
            Drops = new NpcDrop[Constants.MAX_NPC_DROPS];
            for (int i = 0; i < Constants.MAX_NPC_DROPS; i++) {
                Drops[i] = new NpcDrop();
            }
            Moves = new int[4];
        }
    }
}
