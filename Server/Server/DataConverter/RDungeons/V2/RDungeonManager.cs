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

namespace Server.DataConverter.RDungeons.V2
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
                            if (parse[1].ToLower() != "v2") {
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
                            dungeon.WindTimer = parse[6].ToInt();
                            break;
                        case "floor": {
                                RDungeonFloor floor = new RDungeonFloor();
                                //floor.Options.TrapFrequency = parse[1].ToInt();
                                floor.Options.TrapMin = parse[2].ToInt();
                                floor.Options.TrapMax = parse[3].ToInt();
                                floor.Options.RoomWidthMin = parse[4].ToInt();
                                floor.Options.RoomWidthMax = parse[5].ToInt();
                                floor.Options.RoomLengthMin = parse[6].ToInt();
                                floor.Options.RoomLengthMax = parse[7].ToInt();
                                floor.Options.HallTurnMin = parse[8].ToInt();
                                floor.Options.HallTurnMax = parse[9].ToInt();
                                floor.Options.HallVarMin = parse[10].ToInt();
                                floor.Options.HallVarMax = parse[11].ToInt();
                                floor.Options.WaterFrequency = parse[12].ToInt();
                                floor.Options.Craters = parse[13].ToInt();
                                floor.Options.CraterMinLength = parse[14].ToInt();
                                floor.Options.CraterMaxLength = parse[15].ToInt();
                                floor.Options.CraterFuzzy = parse[16].ToBool();
                                
                                floor.Darkness = parse[17].ToInt();
                                floor.GoalType = (Enums.RFloorGoalType)parse[18].ToInt();
                                floor.GoalMap = parse[19].ToInt();
                                floor.GoalX = parse[20].ToInt();
                                floor.GoalY = parse[21].ToInt();
                                floor.Music = parse[22];
                                
                                #region terrain
                                
                                #region wall
                                floor.StairsX = parse[23].ToInt();
                                floor.StairsSheet = parse[24].ToInt();

                                floor.mGroundX = parse[25].ToInt();
                                floor.mGroundSheet = parse[26].ToInt();

                                floor.mTopLeftX = parse[27].ToInt();
                                floor.mTopLeftSheet = parse[28].ToInt();
                                floor.mTopCenterX = parse[29].ToInt();
                                floor.mTopCenterSheet = parse[30].ToInt();
                                floor.mTopRightX = parse[31].ToInt();
                                floor.mTopRightSheet = parse[32].ToInt();

                                floor.mCenterLeftX = parse[33].ToInt();
                                floor.mCenterLeftSheet = parse[34].ToInt();
                                floor.mCenterCenterX = parse[35].ToInt();
                                floor.mCenterCenterSheet = parse[36].ToInt();
                                floor.mCenterRightX = parse[37].ToInt();
                                floor.mCenterRightSheet = parse[38].ToInt();

                                floor.mBottomLeftX = parse[39].ToInt();
                                floor.mBottomLeftSheet = parse[40].ToInt();
                                floor.mBottomCenterX = parse[41].ToInt();
                                floor.mBottomCenterSheet = parse[42].ToInt();
                                floor.mBottomRightX = parse[43].ToInt();
                                floor.mBottomRightSheet = parse[44].ToInt();

                                floor.mInnerTopLeftX = parse[45].ToInt();
                                floor.mInnerTopLeftSheet = parse[46].ToInt();
                                floor.mInnerBottomLeftX = parse[47].ToInt();
                                floor.mInnerBottomLeftSheet = parse[48].ToInt();

                                floor.mInnerTopRightX = parse[49].ToInt();
                                floor.mInnerTopRightSheet = parse[50].ToInt();
                                floor.mInnerBottomRightX = parse[51].ToInt();
                                floor.mInnerBottomRightSheet = parse[52].ToInt();
                                
                                floor.mColumnTopX = parse[53].ToInt();
                                floor.mColumnTopSheet = parse[54].ToInt();
                                floor.mColumnCenterX = parse[55].ToInt();
                                floor.mColumnCenterSheet = parse[56].ToInt();
                                floor.mColumnBottomX = parse[57].ToInt();
                                floor.mColumnBottomSheet = parse[58].ToInt();

                                floor.mRowLeftX = parse[59].ToInt();
                                floor.mRowLeftSheet = parse[60].ToInt();
                                floor.mRowCenterX = parse[61].ToInt();
                                floor.mRowCenterSheet = parse[62].ToInt();
                                floor.mRowRightX = parse[63].ToInt();
                                floor.mRowRightSheet = parse[64].ToInt();
                                
                                floor.mIsolatedWallX = parse[65].ToInt();
                                floor.mIsolatedWallSheet = parse[66].ToInt();
                                
                                #endregion
                                
                                #region water
                                
                                floor.mWaterX = parse[67].ToInt();
                                floor.mWaterSheet = parse[68].ToInt();
                                floor.mWaterAnimX = parse[69].ToInt();
                                floor.mWaterAnimSheet = parse[70].ToInt();
                                
                                floor.mShoreTopLeftX = parse[71].ToInt();
                                floor.mShoreTopLeftSheet = parse[72].ToInt();
                                floor.mShoreTopRightX = parse[73].ToInt();
                                floor.mShoreTopRightSheet = parse[74].ToInt();
                                floor.mShoreBottomRightX = parse[75].ToInt();
                                floor.mShoreBottomRightSheet = parse[76].ToInt();
                                floor.mShoreBottomLeftX = parse[77].ToInt();
                                floor.mShoreBottomLeftSheet = parse[78].ToInt();
                                floor.mShoreDiagonalForwardX = parse[79].ToInt();
                                floor.mShoreDiagonalForwardSheet = parse[80].ToInt();
                                floor.mShoreDiagonalBackX = parse[81].ToInt();
                                floor.mShoreDiagonalBackSheet = parse[82].ToInt();

                                floor.mShoreTopX = parse[83].ToInt();
                                floor.mShoreTopSheet = parse[84].ToInt();
                                floor.mShoreRightX = parse[85].ToInt();
                                floor.mShoreRightSheet = parse[86].ToInt();
                                floor.mShoreBottomX = parse[87].ToInt();
                                floor.mShoreBottomSheet = parse[88].ToInt();
                                floor.mShoreLeftX = parse[89].ToInt();
                                floor.mShoreLeftSheet = parse[90].ToInt();
                                floor.mShoreVerticalX = parse[91].ToInt();
                                floor.mShoreVerticalSheet = parse[92].ToInt();
                                floor.mShoreHorizontalX = parse[93].ToInt();
                                floor.mShoreHorizontalSheet = parse[94].ToInt();

                                floor.mShoreInnerTopLeftX = parse[95].ToInt();
                                floor.mShoreInnerTopLeftSheet = parse[96].ToInt();
                                floor.mShoreInnerTopRightX = parse[97].ToInt();
                                floor.mShoreInnerTopRightSheet = parse[98].ToInt();
                                floor.mShoreInnerBottomRightX = parse[99].ToInt();
                                floor.mShoreInnerBottomRightSheet = parse[100].ToInt();
                                floor.mShoreInnerBottomLeftX = parse[101].ToInt();
                                floor.mShoreInnerBottomLeftSheet = parse[102].ToInt();

                                floor.mShoreInnerTopX = parse[103].ToInt();
                                floor.mShoreInnerTopSheet = parse[104].ToInt();
                                floor.mShoreInnerRightX = parse[105].ToInt();
                                floor.mShoreInnerRightSheet = parse[106].ToInt();
                                floor.mShoreInnerBottomX = parse[107].ToInt();
                                floor.mShoreInnerBottomSheet = parse[108].ToInt();
                                floor.mShoreInnerLeftX = parse[109].ToInt();
                                floor.mShoreInnerLeftSheet = parse[110].ToInt();

                                floor.mShoreSurroundedX = parse[111].ToInt();
                                floor.mShoreSurroundedSheet = parse[112].ToInt();
                                            
                                #endregion
                                #endregion
                                
                                floor.ItemSpawnRate = parse[113].ToInt();
                                int maxTraps = parse[114].ToInt();
                                int maxWeather = parse[115].ToInt();

                                int n = 116;
                                
                                for (int i = 0; i < 16; i++) {
                                    floor.Items[i] = parse[n].ToInt();
                                    n++;
                                }
                                
                                for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                    floor.Npc[i].NpcNum = parse[n].ToInt();
                                    floor.Npc[i].MinLevel = parse[n + 1].ToInt();
                                    n+= 2;
                                }
                                for (int i = 0; i < maxTraps; i++) {
                                    floor.Traps.Add(parse[n].ToInt());
                                    n++;
                                }
                                for (int i = 0; i < maxWeather; i++) {
                                    floor.Weather.Add((Enums.Weather)parse[n].ToInt());
                                    n++;
                                }
                                dungeon.Floors.Add(floor);
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
                writer.WriteLine("RDungeonData|V2");
                writer.WriteLine("Data|" + rdungeon.DungeonName + "|" + ((int)rdungeon.Direction).ToString() + "|" + rdungeon.MaxFloors.ToString() + "|" + rdungeon.Recruitment.ToString() + "|" + rdungeon.Exp.ToString() + "|"+ rdungeon.WindTimer.ToString() + "|");
                for (int i = 0; i < rdungeon.Floors.Count; i++) {
                	string data = "Floor|" + "0" + "|"
                		+ rdungeon.Floors[i].Options.TrapMin.ToString() + "|" + rdungeon.Floors[i].Options.TrapMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.RoomWidthMin.ToString() + "|" + rdungeon.Floors[i].Options.RoomWidthMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.RoomLengthMin.ToString() + "|" + rdungeon.Floors[i].Options.RoomLengthMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.HallTurnMin.ToString() + "|" + rdungeon.Floors[i].Options.HallTurnMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.HallVarMin.ToString() + "|" + rdungeon.Floors[i].Options.HallVarMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.WaterFrequency.ToString() + "|" + rdungeon.Floors[i].Options.Craters.ToString() + "|"
                		+ rdungeon.Floors[i].Options.CraterMinLength.ToString() + "|" + rdungeon.Floors[i].Options.CraterMaxLength.ToString() + "|"
                		+ rdungeon.Floors[i].Options.CraterFuzzy.ToIntString() + "|" + rdungeon.Floors[i].Darkness.ToString() + "|"
                		+ ((int)rdungeon.Floors[i].GoalType).ToString() + "|" + rdungeon.Floors[i].GoalMap.ToString() + "|"
                		+ rdungeon.Floors[i].GoalX.ToString() + "|" + rdungeon.Floors[i].GoalY.ToString() + "|"
                		+ rdungeon.Floors[i].Music + "|"
                		
                		+ rdungeon.Floors[i].StairsX.ToString() + "|" + rdungeon.Floors[i].StairsSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mGroundX.ToString() + "|" + rdungeon.Floors[i].mGroundSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopLeftX.ToString() + "|" + rdungeon.Floors[i].mTopLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopCenterX.ToString() + "|" + rdungeon.Floors[i].mTopCenterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopRightX.ToString() + "|" + rdungeon.Floors[i].mTopRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterLeftX.ToString() + "|" + rdungeon.Floors[i].mCenterLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterCenterX.ToString() + "|" + rdungeon.Floors[i].mCenterCenterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterRightX.ToString() + "|" + rdungeon.Floors[i].mCenterRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomLeftX.ToString() + "|" + rdungeon.Floors[i].mBottomLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomCenterX.ToString() + "|" + rdungeon.Floors[i].mBottomCenterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomRightX.ToString() + "|" + rdungeon.Floors[i].mBottomRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerTopLeftX.ToString() + "|" + rdungeon.Floors[i].mInnerTopLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerBottomLeftX.ToString() + "|" + rdungeon.Floors[i].mInnerBottomLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerTopRightX.ToString() + "|" + rdungeon.Floors[i].mInnerTopRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerBottomRightX.ToString() + "|" + rdungeon.Floors[i].mInnerBottomRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnTopX.ToString() + "|" + rdungeon.Floors[i].mColumnTopSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnCenterX.ToString() + "|" + rdungeon.Floors[i].mColumnCenterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnBottomX.ToString() + "|" + rdungeon.Floors[i].mColumnBottomSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowLeftX.ToString() + "|" + rdungeon.Floors[i].mRowLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowCenterX.ToString() + "|" + rdungeon.Floors[i].mRowCenterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowRightX.ToString() + "|" + rdungeon.Floors[i].mRowRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mIsolatedWallX.ToString() + "|" + rdungeon.Floors[i].mIsolatedWallSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mWaterX.ToString() + "|" + rdungeon.Floors[i].mWaterSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mWaterAnimX.ToString() + "|" + rdungeon.Floors[i].mWaterAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreTopLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreTopLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreTopRightX.ToString() + "|" + rdungeon.Floors[i].mShoreTopRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomRightX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreDiagonalForwardX.ToString() + "|" + rdungeon.Floors[i].mShoreDiagonalForwardSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreDiagonalBackX.ToString() + "|" + rdungeon.Floors[i].mShoreDiagonalBackSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreTopX.ToString() + "|" + rdungeon.Floors[i].mShoreTopSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreRightX.ToString() + "|" + rdungeon.Floors[i].mShoreRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreVerticalX.ToString() + "|" + rdungeon.Floors[i].mShoreVerticalSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreHorizontalX.ToString() + "|" + rdungeon.Floors[i].mShoreHorizontalSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreInnerTopLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopLeftSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerTopRightX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomRightX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomLeftSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreInnerTopX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerRightX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerRightSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerLeftX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerLeftSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreSurroundedX.ToString() + "|" + rdungeon.Floors[i].mShoreSurroundedSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].ItemSpawnRate.ToString() + "|" + rdungeon.Floors[i].Traps.Count.ToString() + "|" + rdungeon.Floors[i].Weather.Count.ToString() + "|";
                		
                		for (int item = 0; item < 16; item++) {
                        data += rdungeon.Floors[i].Items[item].ToString() + "|";
                    }
                    for (int npc = 0; npc < Constants.MAX_MAP_NPCS; npc++) {
                		data += (rdungeon.Floors[i].Npc[npc].NpcNum.ToString() + "|" + rdungeon.Floors[i].Npc[npc].MinLevel.ToString() + "|");
                    }
                    for (int trap = 0; trap < rdungeon.Floors[i].Traps.Count; trap++) {
                        data += rdungeon.Floors[i].Traps[trap].ToString() + "|";
                    }
                    for (int weather = 0; weather < rdungeon.Floors[i].Weather.Count; weather++) {
                        data += ((int)rdungeon.Floors[i].Weather[weather]).ToString() + "|";
                    }
                	
                    writer.WriteLine(data);

                }
        }
	}
}
}