using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Moves.V3
{
    public class MoveManager
    {
        #region Methods

        public static Move LoadMove(int moveNum)
        {
            Move move = new Move();
            string[] parse = null;
            using (System.IO.StreamReader read = new System.IO.StreamReader(IO.Paths.MovesFolder + "move" + moveNum + ".dat"))
            {
                while (!(read.EndOfStream))
                {
                    parse = read.ReadLine().Split('|');
                    switch (parse[0].ToLower())
                    {
                        case "movedata":
                            if (parse[1].ToLower() != "v3")
                            {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data":
                            move.Name = parse[1];
                            move.MaxPP = parse[2].ToInt();
                            move.EffectType = (Enums.MoveType)parse[3].ToInt();
                            move.Element = (Enums.PokemonType)parse[4].ToInt();
                            move.MoveCategory = (Enums.MoveCategory)parse[5].ToInt();
                            move.RangeType = (Enums.MoveRange)parse[6].ToInt();
                            move.Range = parse[7].ToInt();
                            move.TargetType = (Enums.MoveTarget)parse[8].ToInt();
                            
                            move.Data1 = parse[9].ToInt();
                            move.Data2 = parse[10].ToInt();
                            move.Data3 = parse[11].ToInt();
                            move.Accuracy = parse[12].ToInt();
                            move.AdditionalEffectData1 = parse[13].ToInt();
                            move.AdditionalEffectData2 = parse[14].ToInt();
                            move.AdditionalEffectData3 = parse[15].ToInt();
                            move.PerPlayer = parse[16].ToBool();
                            
                            move.IsKey = parse[17].ToBool();
                            move.KeyItem = parse[18].ToInt();

                            move.Sound = parse[19].ToInt();
                            
                            move.Big = parse[20].ToBool();
                            move.SpellAnim = parse[21].ToInt();
                            move.SpellTime = parse[22].ToInt();
                            move.SpellDone = parse[23].ToInt();

                            break;
                    }
                }
            }
            return move;
        }

        public static void SaveMove(Move move, int moveNum)
        {
            string FileName = IO.Paths.MovesFolder + "move" + moveNum + ".dat";
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(FileName))
            {
                write.WriteLine("MoveData|V3");
                write.WriteLine("Data|" + move.Name + "|" + move.MaxPP + "|" + ((int)move.EffectType).ToString() + "|" + ((int)move.Element).ToString() + "|" + ((int)move.MoveCategory).ToString() + "|" + ((int)move.RangeType).ToString() + "|" + move.Range + "|" + ((int)move.TargetType).ToString() + "|" + move.Data1 + "|" + move.Data2 + "|" + move.Data3 + "|" + move.Accuracy + "|" + move.AdditionalEffectData1 + "|" + move.AdditionalEffectData2 + "|" + move.AdditionalEffectData3 + "|" + move.PerPlayer.ToIntString() + "|" + move.IsKey.ToIntString() + "|" + move.KeyItem + "|" + move.Sound + "|" + move.Big.ToIntString() + "|" + move.SpellAnim + "|" + move.SpellTime + "|" + move.SpellDone + "|");
            }
        }

        #endregion Methods
    }
}
