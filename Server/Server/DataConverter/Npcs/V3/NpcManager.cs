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


namespace Server.DataConverter.Npcs.V3
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
                            if (parse[1].ToLower() != "v1" && parse[1].ToLower() != "v2" && parse[1].ToLower() != "v3") {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data": {
                                npc.Name = parse[1];
                                npc.AttackSay = parse[2];
                                npc.Sprite = parse[3].ToInt();
                                npc.SpawnSecs = parse[4].ToInt();
                                npc.Behavior = (Enums.NpcBehavior)parse[5].ToInt();
                                npc.Range = parse[6].ToInt();
                                npc.Str = parse[7].ToInt();
                                npc.Def = parse[8].ToInt();
                                npc.Speed = parse[9].ToInt();
                                npc.Magi = parse[10].ToInt();
                                npc.Big = parse[11].ToBool();
                                npc.MaxHp = parse[12].ToInt();
                                npc.Exp = parse[13].ToUlng();
                                npc.SpawnTime = parse[14].ToInt();
                                npc.Element = parse[15].ToInt();
                                if (parse.Length > 17) {
                                    npc.Spell = parse[16].ToInt();
                                }
                                if (parse.Length > 18) {
                                    npc.Frequency = parse[17].ToInt();
                                }
                                if (parse.Length > 19) {
                                    npc.AIScript = parse[18];
                                }
                            }
                            break;
                        case "recruit":
                            npc.RecruitRate = parse[1].ToInt();
                            npc.RecruitLevel = parse[2].ToInt();
                            //Check if size is in the NPC file... if not add it...
                            if (parse.Length > 4) {
                                npc.Size = parse[3].ToInt();
                            } else {
                                npc.Size = 1;
                            }
                            if (parse.Length > 5) {
                                npc.RecruitClass = parse[4].ToInt();
                            } else {
                                npc.RecruitClass = 0;
                            }
                            break;
                        case "items": {
                                if (parse[1].ToInt() < npc.Drops.Length) {
                                    npc.Drops[parse[1].ToInt()].ItemNum = parse[2].ToInt();
                                    npc.Drops[parse[1].ToInt()].ItemValue = parse[3].ToInt();
                                    npc.Drops[parse[1].ToInt()].Chance = parse[4].ToInt();
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
                Write.WriteLine("NpcData|V3");
                Write.WriteLine("Data" + "|" + npc.Name + "|" + npc.AttackSay + "|" + npc.Sprite + "|" + npc.SpawnSecs + "|" + (int)npc.Behavior + "|" + npc.Range + "|" + npc.Str + "|" + npc.Def + "|" + npc.Speed + "|" + npc.Magi + "|" + npc.Big + "|" + npc.MaxHp + "|" + npc.Exp + "|" + npc.SpawnTime + "|" + npc.Element + "|" + npc.Spell + "|" + npc.Frequency + "|" + npc.AIScript + "|");
                Write.WriteLine("recruit" + "|" + npc.RecruitRate + "|" + npc.RecruitLevel + "|" + npc.Size + "|" + npc.RecruitClass + "|");
                for (int z = 0; z < Constants.MAX_NPC_DROPS; z++) {
                    Write.WriteLine("items" + "|" + z + "|" + npc.Drops[z].ItemNum + "|" + npc.Drops[z].ItemValue + "|" + npc.Drops[z].Chance + "|");
                }
            }
        }

        #endregion Methods
    }
}