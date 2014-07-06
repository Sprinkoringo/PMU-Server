using System;
using System.Collections.Generic;
using System.Text;
using Server;

namespace Server.DataConverter
{
    public class NpcConverter
    {
        public static void ConvertV3ToV4(int num) {
            DataConverter.Npcs.V4.Npc npcV4 = new Server.DataConverter.Npcs.V4.Npc();

            DataConverter.Npcs.V3.Npc npcV3 = Server.DataConverter.Npcs.V3.NpcManager.LoadNpc(num);
            npcV4.Name = npcV3.Name;
            npcV4.AttackSay = npcV3.AttackSay;

            npcV4.Sprite = npcV3.Sprite;
            npcV4.SpawnSecs = npcV3.SpawnSecs;
            npcV4.Behavior = npcV3.Behavior;
            npcV4.Range = npcV3.Range;
            Server.Pokedex.Pokemon pokemon = Server.Pokedex.Pokedex.FindBySprite(npcV3.Sprite);
            if (pokemon != null) {
                npcV4.Species = pokemon.ID;
            } else {
                npcV4.Species = 0;
            }
            npcV4.Big = npcV3.Big;
            npcV4.SpawnTime = npcV3.SpawnTime;

            for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                npcV4.Drops[z].ItemNum = npcV3.Drops[z].ItemNum;
                npcV4.Drops[z].ItemValue = npcV3.Drops[z].ItemValue;
                npcV4.Drops[z].Chance = npcV3.Drops[z].Chance;
            }

            npcV4.RecruitRate = npcV3.RecruitRate;
            npcV4.RecruitLevel = npcV3.RecruitLevel;

            npcV4.Spell = npcV3.Spell;
            npcV4.Frequency = npcV3.Frequency;

            npcV4.AIScript = npcV3.AIScript;

            Npcs.V4.NpcManager.SaveNpc(npcV4, num);
        }

        public static void ConvertV4ToV5(int num) {
            DataConverter.Npcs.V5.Npc npcV5 = new Server.DataConverter.Npcs.V5.Npc();

            DataConverter.Npcs.V4.Npc npcV4 = Server.DataConverter.Npcs.V4.NpcManager.LoadNpc(num);
            npcV5.Name = npcV4.Name;
            npcV5.AttackSay = npcV4.AttackSay;

            npcV5.Sprite = npcV4.Sprite;
            npcV5.SpawnRate = npcV4.SpawnSecs;
            npcV5.Behaviour = npcV4.Behavior;
            npcV5.Range = npcV4.Range;
            npcV5.Species = npcV4.Species;
            npcV5.BigSprite = npcV4.Big;
            npcV5.SpawnTime = npcV4.SpawnTime;

            for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                npcV5.Drops[z].ItemNum = npcV4.Drops[z].ItemNum;
                npcV5.Drops[z].ItemValue = npcV4.Drops[z].ItemValue;
                npcV5.Drops[z].Chance = npcV4.Drops[z].Chance;
            }

            npcV5.RecruitRate = npcV4.RecruitRate;
            npcV5.RecruitLevel = npcV4.RecruitLevel;

            npcV5.Moves[0] = npcV4.Spell;

            npcV5.AIScript = npcV4.AIScript;

            Npcs.V5.NpcManager.SaveNpc(npcV5, num);
        }

        public static void ConvertV5ToV6(int num)
        {
            DataConverter.Npcs.V6.Npc npcV6 = new Server.DataConverter.Npcs.V6.Npc();

            DataConverter.Npcs.V5.Npc npcV5 = Server.DataConverter.Npcs.V5.NpcManager.LoadNpc(num);
            npcV6.Name = npcV5.Name;
            npcV6.AttackSay = npcV5.AttackSay;

            npcV6.Sprite = npcV5.Sprite;
            npcV6.Behavior = npcV5.Behaviour;
            npcV6.Range = npcV5.Range;
            npcV6.Species = npcV5.Species;

            for (int z = 0; z < Constants.MAX_NPC_DROPS; z++)
            {
                npcV6.Drops[z].ItemNum = npcV5.Drops[z].ItemNum;
                npcV6.Drops[z].ItemValue = npcV5.Drops[z].ItemValue;
                npcV6.Drops[z].Chance = npcV5.Drops[z].Chance;
            }

            npcV6.RecruitRate = npcV5.RecruitRate;

            npcV6.Moves[0] = npcV5.Moves[0];
            npcV6.Moves[1] = npcV5.Moves[1];
            npcV6.Moves[2] = npcV5.Moves[2];
            npcV6.Moves[3] = npcV5.Moves[3];

            if (npcV5.SpawnTime == 2)
            {
                npcV6.SpawnsAtNight = true;
            }
            else
            {
                npcV6.SpawnsAtDay = true;
            }

            npcV6.AIScript = npcV5.AIScript;

            Npcs.V6.NpcManager.SaveNpc(npcV6, num);
        }

        public static void ConvertV6ToV7(int num) {
            DataConverter.Npcs.V7.Npc npcV7 = new Server.DataConverter.Npcs.V7.Npc();

            DataConverter.Npcs.V6.Npc npcV6 = Server.DataConverter.Npcs.V6.NpcManager.LoadNpc(num);
            npcV7.Name = npcV6.Name;
            npcV7.AttackSay = npcV6.AttackSay;
            npcV7.Behavior = npcV6.Behavior;

            npcV7.Form = 0;
            npcV7.ShinyChance = 0;
            npcV7.Species = npcV6.Species;

            for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                npcV7.Drops[z].ItemNum = npcV6.Drops[z].ItemNum;
                npcV7.Drops[z].ItemValue = npcV6.Drops[z].ItemValue;
                npcV7.Drops[z].Chance = npcV6.Drops[z].Chance;
            }

            npcV7.RecruitRate = npcV6.RecruitRate;

            npcV7.Moves[0] = npcV6.Moves[0];
            npcV7.Moves[1] = npcV6.Moves[1];
            npcV7.Moves[2] = npcV6.Moves[2];
            npcV7.Moves[3] = npcV6.Moves[3];

            npcV7.SpawnsAtDawn = npcV6.SpawnsAtDawn;
            npcV7.SpawnsAtDay = npcV6.SpawnsAtDay;
            npcV7.SpawnsAtDusk = npcV6.SpawnsAtDusk;
            npcV7.SpawnsAtNight = npcV6.SpawnsAtNight;

            npcV7.AIScript = npcV6.AIScript;

            Npcs.V7.NpcManager.SaveNpc(npcV7, num);
        }
    }
}
