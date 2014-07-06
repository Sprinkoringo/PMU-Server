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


namespace Server.DataConverter.Moves.V2
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
                        case "movedata":
                            if (parse[1].ToLower() != "v2") {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data":
                            move.Name = parse[1];
                            move.LevelReq = parse[2].ToInt();
                            move.Range = (Enums.MoveRange)parse[3].ToInt();
                            move.TargetType = (Enums.MoveTarget)parse[4].ToInt();
                            move.MoveType = (Enums.PokemonType)parse[5].ToInt();
                            move.EffectType = (Enums.MoveType)parse[6].ToInt();
                            move.MaxPP = parse[7].ToInt();
                            move.Data1 = parse[8].ToInt();
                            move.Data2 = parse[9].ToInt();
                            move.Data3 = parse[10].ToInt();
                            move.Big = parse[11].ToBool();
                            move.Sound = parse[12].ToInt();
                            move.SpellAnim = parse[13].ToInt();
                            move.SpellDone = parse[14].ToInt();
                            move.SpellTime = parse[15].ToInt();
                            move.IsKey = parse[16].ToBool();
                            move.KeyItem = parse[17].ToInt();
                            break;
                    }
                }
            }
            return move;
        }

        public static void SaveMove(Move move, int moveNum) {
            string FileName = IO.Paths.MovesFolder + "move" + moveNum + ".dat";
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(FileName)) {
                write.WriteLine("MoveData|V2");
                write.WriteLine("Data|" + move.Name + "|" + move.LevelReq + "|" + ((int)move.Range).ToString() + "|" + ((int)move.TargetType).ToString() + "|" + ((int)move.MoveType).ToString() + "|" + ((int)move.EffectType).ToString() + "|" + move.MaxPP + "|" + move.Data1 + "|" + move.Data2 + "|" + move.Data3 + "|" + move.Big.ToIntString() + "|" + move.Sound + "|" + move.SpellAnim + "|" + move.SpellDone + "|" + move.SpellTime + "|" + move.IsKey.ToIntString() + "|" + move.KeyItem + "|");
            }
        }

        #endregion Methods
    }
}