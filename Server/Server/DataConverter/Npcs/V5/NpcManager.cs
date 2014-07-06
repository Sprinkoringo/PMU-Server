namespace Server.DataConverter.Npcs.V5
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class NpcManager
    {
        #region Methods

        public static Npc LoadNpc(int npcNum) {
            Npc npc = new Npc();
            string FileName = IO.Paths.NpcsFolder + "npc" + npcNum + ".dat";
            string s;
            string[] parse;
            using (System.IO.StreamReader read = new System.IO.StreamReader(FileName)) {
                while (!(read.EndOfStream)) {
                    s = read.ReadLine();
                    parse = s.Split('|');
                    switch (parse[0].ToLower()) {
                        case "npcdata":
                            if (parse[1].ToLower() != "v5") {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data": {
                                npc.Name = parse[1];
                                npc.AttackSay = parse[2];
                                npc.Sprite = parse[3].ToInt();
                                npc.SpawnRate = parse[4].ToInt();
                                npc.Behaviour = (Enums.NpcBehavior)parse[5].ToInt();
                                npc.Range = parse[6].ToInt();
                                npc.Species = parse[7].ToInt();
                                npc.BigSprite = parse[8].ToBool();
                                npc.SpawnTime = parse[9].ToInt();
                                npc.AIScript = parse[10];
                                npc.RecruitRate = parse[11].ToInt();
                                npc.RecruitLevel = parse[12].ToInt();
                            }
                            break;
                        case "moves": {
                                int n = 1;
                                for (int i = 0; i < npc.Moves.Length; i++) {
                                    npc.Moves[i] = parse[n].ToInt();

                                    n += 1;
                                }
                            }
                            break;
                        case "drop": {
                                int dropNum = parse[1].ToInt();
                                if (dropNum < npc.Drops.Length) {
                                    npc.Drops[dropNum].ItemNum = parse[2].ToInt();
                                    npc.Drops[dropNum].ItemValue = parse[3].ToInt();
                                    npc.Drops[dropNum].Chance = parse[4].ToInt();
                                }
                            }
                            break;
                    }
                }
            }
            return npc;
        }

        public static void SaveNpc(Npc npc, int npcNum) {
            string FileName = IO.Paths.NpcsFolder + "npc" + npcNum.ToString() + ".dat";
            using (System.IO.StreamWriter Write = new System.IO.StreamWriter(FileName)) {
                Write.WriteLine("NpcData|V5");
                Write.WriteLine("Data" + "|" + npc.Name + "|" + npc.AttackSay + "|" + npc.Sprite + "|" + npc.SpawnRate + "|" + (int)npc.Behaviour + "|" + npc.Range + "|" + npc.Species + "|" + npc.BigSprite + "|" + npc.SpawnTime + "|" + npc.AIScript + "|" + npc.RecruitRate + "|" + npc.RecruitLevel + "|");
                Write.Write("Moves|");
                for (int i = 0; i < npc.Moves.Length; i++) {
                    Write.Write(npc.Moves[i].ToString() + "|");
                }
                Write.WriteLine();
                for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                    Write.WriteLine("Drop" + "|" + z + "|" + npc.Drops[z].ItemNum + "|" + npc.Drops[z].ItemValue + "|" + npc.Drops[z].Chance + "|");
                }
            }
        }

        #endregion Methods
    }
}