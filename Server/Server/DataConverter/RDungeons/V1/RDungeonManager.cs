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
using System.Text;
using Server.RDungeons;

namespace Server.DataConverter.RDungeons.V1
{
	/// <summary>
	/// Description of RDungeonManager.
	/// </summary>
	public class RDungeonManager
	{
		public static RDungeon LoadRDungeon(int dungeonNum) {
            RDungeon dungeon = new RDungeon(dungeonNum);
            string FilePath = IO.Paths.RDungeonsFolder + "rdungeon" + dungeonNum.ToString() + ".dat";
            using (System.IO.StreamReader reader = new System.IO.StreamReader(FilePath)) {
                while (!(reader.EndOfStream)) {
                    string[] parse = reader.ReadLine().Split('|');
                    switch (parse[0].ToLower()) {
                        case "rdungeondata":
                            if (parse[1].ToLower() != "v1") {
                                reader.Close();
                                reader.Dispose();
                                return null;
                            }
                            break;
                        case "data":
                            dungeon.DungeonName = parse[1];
                            dungeon.Direction = (Enums.Direction)parse[2].ToInt();
                            dungeon.MaxFloors = parse[3].ToInt();
                            dungeon.Recruitment = parse[4].ToBool();
                            dungeon.Exp = parse[5].ToBool();
                            break;
                        case "terrain": {
                                #region Terrain
                                dungeon.StairsX = parse[1].ToInt();
                                dungeon.StairsSheet = parse[2].ToInt();

                                dungeon.mGroundX = parse[3].ToInt();
                                dungeon.mGroundSheet = parse[4].ToInt();

                                dungeon.mTopLeftX = parse[5].ToInt();
                                dungeon.mTopLeftSheet = parse[6].ToInt();
                                dungeon.mTopCenterX = parse[7].ToInt();
                                dungeon.mTopCenterSheet = parse[8].ToInt();
                                dungeon.mTopRightX = parse[9].ToInt();
                                dungeon.mTopRightSheet = parse[10].ToInt();

                                dungeon.mCenterLeftX = parse[11].ToInt();
                                dungeon.mCenterLeftSheet = parse[12].ToInt();
                                dungeon.mCenterCenterX = parse[13].ToInt();
                                dungeon.mCenterCenterSheet = parse[14].ToInt();
                                dungeon.mCenterRightX = parse[15].ToInt();
                                dungeon.mCenterRightSheet = parse[16].ToInt();

                                dungeon.mBottomLeftX = parse[17].ToInt();
                                dungeon.mBottomLeftSheet = parse[18].ToInt();
                                dungeon.mBottomCenterX = parse[19].ToInt();
                                dungeon.mBottomCenterSheet = parse[20].ToInt();
                                dungeon.mBottomRightX = parse[21].ToInt();
                                dungeon.mBottomRightSheet = parse[22].ToInt();

                                dungeon.mInnerTopLeftX = parse[23].ToInt();
                                dungeon.mInnerTopLeftSheet = parse[24].ToInt();
                                dungeon.mInnerBottomLeftX = parse[25].ToInt();
                                dungeon.mInnerBottomLeftSheet = parse[26].ToInt();

                                dungeon.mInnerTopRightX = parse[27].ToInt();
                                dungeon.mInnerTopRightSheet = parse[28].ToInt();
                                dungeon.mInnerBottomRightX = parse[29].ToInt();
                                dungeon.mInnerBottomRightSheet = parse[30].ToInt();

                                if (parse.Length > 32) {
                                    dungeon.mWaterX = parse[31].ToInt();
                                    dungeon.mWaterSheet = parse[32].ToInt();
                                    dungeon.mWaterAnimX = parse[33].ToInt();
                                    dungeon.mWaterAnimSheet = parse[34].ToInt();
                                    dungeon.mIsolatedWallX = parse[35].ToInt();
                                    dungeon.mIsolatedWallSheet = parse[36].ToInt();

                                    dungeon.mColumnTopX = parse[37].ToInt();
                                    dungeon.mColumnTopSheet = parse[38].ToInt();
                                    dungeon.mColumnCenterX = parse[39].ToInt();
                                    dungeon.mColumnCenterSheet = parse[40].ToInt();
                                    dungeon.mColumnBottomX = parse[41].ToInt();
                                    dungeon.mColumnBottomSheet = parse[42].ToInt();

                                    dungeon.mRowLeftX = parse[43].ToInt();
                                    dungeon.mRowLeftSheet = parse[44].ToInt();
                                    dungeon.mRowCenterX = parse[45].ToInt();
                                    dungeon.mRowCenterSheet = parse[46].ToInt();
                                    if (parse.Length > 48) {
                                        dungeon.mRowRightX = parse[47].ToInt();
                                        dungeon.mRowRightSheet = parse[48].ToInt();
                                        if (parse.Length > 50) {
                                            dungeon.mShoreTopLeftX = parse[49].ToInt();
                                            dungeon.mShoreTopLeftSheet = parse[50].ToInt();
                                            dungeon.mShoreTopRightX = parse[51].ToInt();
                                            dungeon.mShoreTopRightSheet = parse[52].ToInt();
                                            dungeon.mShoreBottomRightX = parse[53].ToInt();
                                            dungeon.mShoreBottomRightSheet = parse[54].ToInt();
                                            dungeon.mShoreBottomLeftX = parse[55].ToInt();
                                            dungeon.mShoreBottomLeftSheet = parse[56].ToInt();
                                            dungeon.mShoreDiagonalForwardX = parse[57].ToInt();
                                            dungeon.mShoreDiagonalForwardSheet = parse[58].ToInt();
                                            dungeon.mShoreDiagonalBackX = parse[59].ToInt();
                                            dungeon.mShoreDiagonalBackSheet = parse[60].ToInt();

                                            dungeon.mShoreTopX = parse[61].ToInt();
                                            dungeon.mShoreTopSheet = parse[62].ToInt();
                                            dungeon.mShoreRightX = parse[63].ToInt();
                                            dungeon.mShoreRightSheet = parse[64].ToInt();
                                            dungeon.mShoreBottomX = parse[65].ToInt();
                                            dungeon.mShoreBottomSheet = parse[66].ToInt();
                                            dungeon.mShoreLeftX = parse[67].ToInt();
                                            dungeon.mShoreLeftSheet = parse[68].ToInt();
                                            dungeon.mShoreVerticalX = parse[69].ToInt();
                                            dungeon.mShoreVerticalSheet = parse[70].ToInt();
                                            dungeon.mShoreHorizontalX = parse[71].ToInt();
                                            dungeon.mShoreHorizontalSheet = parse[72].ToInt();

                                            dungeon.mShoreInnerTopLeftX = parse[73].ToInt();
                                            dungeon.mShoreInnerTopLeftSheet = parse[74].ToInt();
                                            dungeon.mShoreInnerTopRightX = parse[75].ToInt();
                                            dungeon.mShoreInnerTopRightSheet = parse[76].ToInt();
                                            dungeon.mShoreInnerBottomRightX = parse[77].ToInt();
                                            dungeon.mShoreInnerBottomRightSheet = parse[78].ToInt();
                                            dungeon.mShoreInnerBottomLeftX = parse[79].ToInt();
                                            dungeon.mShoreInnerBottomLeftSheet = parse[80].ToInt();

                                            dungeon.mShoreInnerTopX = parse[81].ToInt();
                                            dungeon.mShoreInnerTopSheet = parse[82].ToInt();
                                            dungeon.mShoreInnerRightX = parse[83].ToInt();
                                            dungeon.mShoreInnerRightSheet = parse[84].ToInt();
                                            dungeon.mShoreInnerBottomX = parse[85].ToInt();
                                            dungeon.mShoreInnerBottomSheet = parse[86].ToInt();
                                            dungeon.mShoreInnerLeftX = parse[87].ToInt();
                                            dungeon.mShoreInnerLeftSheet = parse[88].ToInt();

                                            dungeon.mShoreSurroundedX = parse[89].ToInt();
                                            dungeon.mShoreSurroundedSheet = parse[90].ToInt();
                                        }
                                    }
                                }
                                #endregion
                            }
                            break;
                        case "floor": {
                                RDungeonFloor floor = new RDungeonFloor();
                                floor.WeatherIntensity = parse[1].ToInt();
                                floor.Weather = (Enums.Weather)parse[2].ToInt();
                                floor.Music = parse[3];
                                floor.GoalType = (Enums.RFloorGoalType)parse[4].ToInt();
                                floor.GoalMap = parse[5].ToInt();
                                floor.GoalX = parse[6].ToInt();
                                floor.GoalY = parse[7].ToInt();
                                floor.ItemSpawnRate = parse[8].ToInt();
                                int maxTraps = parse[9].ToInt();

                                int n = 10;
                                for (int i = 0; i < 15; i++) {
                                    floor.Npc[i] = parse[n].ToInt();
                                    n++;
                                }
                                for (int i = 0; i < 8; i++) {
                                    floor.Items[i] = parse[n].ToInt();
                                    n++;
                                }
                                for (int i = 0; i < maxTraps; i++) {
                                    floor.Traps.Add(parse[n].ToInt());
                                    n++;
                                }
                                dungeon.Floors.Add(floor);
                            }
                            break;
                        case "cratersettings": {
                                dungeon.Options.Craters = parse[1].ToInt();
                                dungeon.Options.CraterMinLength = parse[2].ToInt();
                                dungeon.Options.CraterMaxLength = parse[3].ToInt();
                                dungeon.Options.CraterFuzzy = parse[4].ToBool();
                            }
                            break;
                    }
                }
            }
            return dungeon;
	}
	
	public static void SaveRDungeon(RDungeon rdungeon, int dungeonNum) {
            
            string Filepath = IO.Paths.RDungeonsFolder + "rdungeon" + dungeonNum.ToString() + ".dat";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Filepath)) {
                writer.WriteLine("RDungeonData|V1");
                writer.WriteLine("Data|" + rdungeon.DungeonName + "|" + ((int)rdungeon.Direction).ToString() + "|" + rdungeon.MaxFloors.ToString() + "|" + rdungeon.Recruitment.ToString() + "|" + rdungeon.Exp.ToString() + "|");
                writer.WriteLine("Terrain|" + rdungeon.StairsX.ToString() + "|" + rdungeon.StairsSheet.ToString() + "|" +
                                 rdungeon.mGroundX.ToString() + "|" + rdungeon.mGroundSheet.ToString() + "|" +

                                 rdungeon.mTopLeftX.ToString() + "|" + rdungeon.mTopLeftSheet.ToString() + "|" +
                                 rdungeon.mTopCenterX.ToString() + "|" + rdungeon.mTopCenterSheet.ToString() + "|" +
                                 rdungeon.mTopRightX.ToString() + "|" + rdungeon.mTopRightSheet.ToString() + "|" +

                                 rdungeon.mCenterLeftX.ToString() + "|" + rdungeon.mCenterLeftSheet.ToString() + "|" +
                                 rdungeon.mCenterCenterX.ToString() + "|" + rdungeon.mCenterCenterSheet.ToString() + "|" +
                                 rdungeon.mCenterRightX.ToString() + "|" + rdungeon.mCenterRightSheet.ToString() + "|" +

                                 rdungeon.mBottomLeftX.ToString() + "|" + rdungeon.mBottomLeftSheet.ToString() + "|" +
                                 rdungeon.mBottomCenterX.ToString() + "|" + rdungeon.mBottomCenterSheet.ToString() + "|" +
                                 rdungeon.mBottomRightX.ToString() + "|" + rdungeon.mBottomRightSheet.ToString() + "|" +

                                 rdungeon.mInnerTopLeftX.ToString() + "|" + rdungeon.mInnerTopLeftSheet.ToString() + "|" +
                                 rdungeon.mInnerBottomLeftX.ToString() + "|" + rdungeon.mInnerBottomLeftSheet.ToString() + "|" +
                                 rdungeon.mInnerTopRightX.ToString() + "|" + rdungeon.mInnerTopRightSheet.ToString() + "|" +
                                 rdungeon.mInnerBottomRightX.ToString() + "|" + rdungeon.mInnerBottomRightSheet.ToString() + "|" +
                                 rdungeon.mWaterX.ToString() + "|" + rdungeon.mWaterSheet.ToString() + "|" +
                                 rdungeon.mWaterAnimX.ToString() + "|" + rdungeon.mWaterAnimSheet.ToString() + "|" +
                                 rdungeon.mIsolatedWallX.ToString() + "|" + rdungeon.mIsolatedWallSheet.ToString() + "|" +

                                 rdungeon.mColumnTopX.ToString() + "|" + rdungeon.mColumnTopSheet.ToString() + "|" +
                                 rdungeon.mColumnCenterX.ToString() + "|" + rdungeon.mColumnCenterSheet.ToString() + "|" +
                                 rdungeon.mColumnBottomX.ToString() + "|" + rdungeon.mColumnBottomSheet.ToString() + "|" +

                                 rdungeon.mRowLeftX.ToString() + "|" + rdungeon.mRowLeftSheet.ToString() + "|" +
                                 rdungeon.mRowCenterX.ToString() + "|" + rdungeon.mRowCenterSheet.ToString() + "|" +
                                 rdungeon.mRowRightX.ToString() + "|" + rdungeon.mRowRightSheet.ToString() + "|");
                for (int i = 0; i < rdungeon.Floors.Count; i++) {
                    string data = "Floor|" + rdungeon.Floors[i].WeatherIntensity.ToString() + "|" + ((int)rdungeon.Floors[i].Weather).ToString() + "|" + rdungeon.Floors[i].Music + "|" +
                        ((int)rdungeon.Floors[i].GoalType).ToString() + "|" + rdungeon.Floors[i].GoalMap.ToString() + "|" + rdungeon.Floors[i].GoalX.ToString() + "|" + rdungeon.Floors[i].GoalY.ToString() + "|" +
                        rdungeon.Floors[i].ItemSpawnRate.ToString() + "|" + rdungeon.Floors[i].Traps.Count.ToString() + "|";

                    for (int npc = 0; npc < 15; npc++) {
                        data += rdungeon.Floors[i].Npc[npc].ToString() + "|";
                    }
                    for (int item = 0; item < 8; item++) {
                        data += rdungeon.Floors[i].Items[item].ToString() + "|";
                    }
                    for (int trap = 0; trap < rdungeon.Floors[i].Traps.Count; trap++) {
                        data += rdungeon.Floors[i].Traps[trap].ToString() + "|";
                    }
                    writer.WriteLine(data);

                }
                writer.WriteLine("CraterSettings|" + rdungeon.Options.Craters + "|" + rdungeon.Options.CraterMinLength + "|" + rdungeon.Options.CraterMaxLength + "|" + rdungeon.Options.CraterFuzzy.ToIntString() + "|");
            }
        }
}
}