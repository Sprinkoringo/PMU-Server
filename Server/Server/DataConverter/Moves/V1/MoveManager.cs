namespace Server.DataConverter.Moves.V1
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MoveManager
    {
        #region Methods

        public static Move LoadMove(int moveNum) {
            Move move = new Move();
            string[] parse = null;
            using (System.IO.StreamReader read = new System.IO.StreamReader(IO.Paths.MovesFolder + "move" + moveNum + ".dat")) {
                while (!(read.EndOfStream)) {
                    parse = read.ReadLine().Split('|');
                    switch (parse[0].ToLower()) {
                        case "spelldata":
                            if (parse[1].ToLower() != "v1") {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data":
                            move.Name = parse[1];
                            move.LevelReq = parse[2].ToInt();
                            move.AE = parse[3].ToBool();
                            move.Big = parse[4].ToBool();
                            move.ClassReq = parse[5].ToInt();
                            move.ClassReq2 = parse[6].ToInt();
                            move.ClassReq3 = parse[7].ToInt();
                            move.Data1 = parse[8].ToInt();
                            move.Data2 = parse[9].ToInt();
                            move.Data3 = parse[10].ToInt();
                            move.Element = parse[11].ToInt();
                            move.MPCost = parse[12].ToInt();
                            move.Range = parse[13].ToInt();
                            move.Sound = parse[14].ToInt();
                            move.SpellAnim = parse[15].ToInt();
                            move.SpellDone = parse[16].ToInt();
                            move.SpellTime = parse[17].ToInt();
                            move.Type = (Enums.MoveType)parse[18].ToInt();
                            if (parse.Length > 20) {
                                move.IsKey = parse[19].ToBool();
                                move.KeyItem = parse[20].ToInt();
                            }
                            break;
                    }
                }
            }
            return move;
        }

        public static void SaveMove(Move move, int moveNum) {
            string FileName = IO.Paths.MovesFolder + "move" + moveNum + ".dat";
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(FileName)) {
                write.WriteLine("SpellData|V1");
                write.WriteLine("Data|" + move.Name + "|" + move.LevelReq + "|" + move.AE + "|" + move.Big + "|" + move.ClassReq + "|" + move.ClassReq2 + "|" + move.ClassReq3 + "|" + move.Data1 + "|" + move.Data2 + "|" + move.Data3 + "|" + move.Element + "|" + move.MPCost + "|" + move.Range + "|" + move.Sound + "|" + move.SpellAnim + "|" + move.SpellDone + "|" + move.SpellTime + "|" + (int)move.Type + "|" + move.IsKey.ToIntString() + "|" + move.KeyItem.ToString() + "|");
            }
        }

        #endregion Methods
    }
}