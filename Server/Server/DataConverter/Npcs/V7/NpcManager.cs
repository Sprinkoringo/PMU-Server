/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


namespace Server.DataConverter.Npcs.V7 {
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class NpcManager {
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
                            if (parse[1].ToLower() != "v7") {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data": {
                                npc.Name = parse[1];
                                npc.AttackSay = parse[2];
                                npc.Form = parse[3].ToInt();
                                npc.Behavior = (Enums.NpcBehavior)parse[4].ToInt();
                                npc.ShinyChance = parse[5].ToInt();
                                npc.Species = parse[6].ToInt();
                                npc.SpawnsAtDay = parse[7].ToBool();
                                npc.SpawnsAtNight = parse[8].ToBool();
                                npc.SpawnsAtDawn = parse[9].ToBool();
                                npc.SpawnsAtDusk = parse[10].ToBool();
                                npc.AIScript = parse[11];
                                npc.RecruitRate = parse[12].ToInt();
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
                                    npc.Drops[dropNum].Tag = parse[4];
                                    npc.Drops[dropNum].Chance = parse[5].ToInt();
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
                Write.WriteLine("NpcData|V7");
                Write.WriteLine("Data" + "|" + npc.Name + "|" + npc.AttackSay + "|" + npc.Form + "|" + (int)npc.Behavior + "|" + npc.ShinyChance + "|" + npc.Species + "|" + npc.SpawnsAtDay.ToIntString() + "|" + npc.SpawnsAtNight.ToIntString() + "|" + npc.SpawnsAtDawn.ToIntString() + "|" + npc.SpawnsAtDusk.ToIntString() + "|" + npc.AIScript + "|" + npc.RecruitRate + "|");
                Write.Write("Moves|");
                for (int i = 0; i < npc.Moves.Length; i++) {
                    Write.Write(npc.Moves[i].ToString() + "|");
                }
                Write.WriteLine();
                for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                    Write.WriteLine("Drop" + "|" + z + "|" + npc.Drops[z].ItemNum + "|" + npc.Drops[z].ItemValue + "|" + npc.Drops[z].Tag + "|" + npc.Drops[z].Chance + "|");
                }
            }
        }

        #endregion Methods
    }
}
