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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Moves.V5 {
    public class MoveManager {

        public static Move LoadMove(int moveNum) {

            Move move = new Move();
            string[] parse = null;
            using (System.IO.StreamReader read = new System.IO.StreamReader(IO.Paths.MovesFolder + "move" + moveNum + ".dat")) {
                while (!(read.EndOfStream)) {
                    parse = read.ReadLine().Split('|');
                    switch (parse[0].ToLower()) {
                        case "movedata":
                            if (parse[1].ToLower() != "v5") {
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
                            move.HitTime = parse[13].ToInt();
                            move.HitFreeze = parse[14].ToBool();
                            move.AdditionalEffectData1 = parse[15].ToInt();
                            move.AdditionalEffectData2 = parse[16].ToInt();
                            move.AdditionalEffectData3 = parse[17].ToInt();
                            move.PerPlayer = parse[18].ToBool();
                            
                            move.KeyItem = parse[19].ToInt();

                            move.Sound = parse[20].ToInt();

                            move.AttackerAnim.AnimationType = (Enums.MoveAnimationType)parse[21].ToInt();
                            move.AttackerAnim.AnimationIndex = parse[22].ToInt();
                            move.AttackerAnim.FrameSpeed = parse[23].ToInt();
                            move.AttackerAnim.Repetitions = parse[24].ToInt();

                            move.TravelingAnim.AnimationType = (Enums.MoveAnimationType)parse[25].ToInt();
                            move.TravelingAnim.AnimationIndex = parse[26].ToInt();
                            move.TravelingAnim.FrameSpeed = parse[27].ToInt();
                            move.TravelingAnim.Repetitions = parse[28].ToInt();

                            move.DefenderAnim.AnimationType = (Enums.MoveAnimationType)parse[29].ToInt();
                            move.DefenderAnim.AnimationIndex = parse[30].ToInt();
                            move.DefenderAnim.FrameSpeed = parse[31].ToInt();
                            move.DefenderAnim.Repetitions = parse[32].ToInt();

                            break;
                    }
                }
            }

            return move;
        }


        public static void SaveMove(Move move, int moveNum) {
            
            string FileName = IO.Paths.MovesFolder + "move" + moveNum + ".dat";
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(FileName)) {
                write.WriteLine("MoveData|V5");
                write.WriteLine("Data|" + move.Name + "|" + move.MaxPP + "|" + ((int)move.EffectType).ToString() + "|" + ((int)move.Element).ToString() + "|" + ((int)move.MoveCategory).ToString() + "|"
                    + ((int)move.RangeType).ToString() + "|" + move.Range + "|" + ((int)move.TargetType).ToString()
                    + "|" + move.Data1 + "|" + move.Data2 + "|" + move.Data3 + "|" + move.Accuracy + "|" + move.HitTime + "|" + move.HitFreeze.ToIntString() + "|"
                    + move.AdditionalEffectData1 + "|" + move.AdditionalEffectData2 + "|" + move.AdditionalEffectData3 + "|"
                    + move.PerPlayer.ToIntString() + "|" + move.KeyItem + "|" + move.Sound + "|"
                    + ((int)move.AttackerAnim.AnimationType).ToString() + "|" + move.AttackerAnim.AnimationIndex.ToString() + "|" + move.AttackerAnim.FrameSpeed.ToString() + "|" + move.AttackerAnim.Repetitions.ToString() + "|"
                    + ((int)move.TravelingAnim.AnimationType).ToString() + "|" + move.TravelingAnim.AnimationIndex.ToString() + "|" + move.TravelingAnim.FrameSpeed.ToString() + "|" + move.TravelingAnim.Repetitions.ToString() + "|"
                    + ((int)move.DefenderAnim.AnimationType).ToString() + "|" + move.DefenderAnim.AnimationIndex.ToString() + "|" + move.DefenderAnim.FrameSpeed.ToString() + "|" + move.DefenderAnim.Repetitions.ToString() + "|");
            }
        }

    }
}
