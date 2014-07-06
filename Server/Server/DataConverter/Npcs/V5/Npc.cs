using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Npcs.V5
{
    public class Npc
    {
        public string Name { get; set; }
        public string AttackSay { get; set; }

        public int Sprite { get; set; }
        public int SpawnRate { get; set; }

        public Enums.NpcBehavior Behaviour { get; set; }
        public int Range { get; set; }

        public int Species { get; set; }
        public bool BigSprite { get; set; }
        public int SpawnTime { get; set; }
        public NpcDrop[] Drops { get; set; }

        public int RecruitRate { get; set; }
        public int RecruitLevel { get; set; }

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
