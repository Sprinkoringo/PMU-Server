using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;

namespace Server.DataConverter.RDungeons.V4
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
                            if (parse[1].ToLower() != "v4") {
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
                            dungeon.RDungeonIndex = parse[7].ToInt();
                            break;
                        case "floor": {
                                RDungeonFloor floor = new RDungeonFloor();
                                floor.Options.TrapMin = parse[1].ToInt();
                                floor.Options.TrapMax = parse[2].ToInt();
                                floor.Options.ItemMin = parse[3].ToInt();
                                floor.Options.ItemMax = parse[4].ToInt();
                                floor.Options.Intricacy = parse[5].ToInt();
                                floor.Options.RoomWidthMin = parse[6].ToInt();
                                floor.Options.RoomWidthMax = parse[7].ToInt();
                                floor.Options.RoomLengthMin = parse[8].ToInt();
                                floor.Options.RoomLengthMax = parse[9].ToInt();
                                floor.Options.HallTurnMin = parse[10].ToInt();
                                floor.Options.HallTurnMax = parse[11].ToInt();
                                floor.Options.HallVarMin = parse[12].ToInt();
                                floor.Options.HallVarMax = parse[13].ToInt();
                                floor.Options.WaterFrequency = parse[14].ToInt();
                                floor.Options.Craters = parse[15].ToInt();
                                floor.Options.CraterMinLength = parse[16].ToInt();
                                floor.Options.CraterMaxLength = parse[17].ToInt();
                                floor.Options.CraterFuzzy = parse[18].ToBool();
                                floor.Options.MinChambers = parse[19].ToInt();
                                floor.Options.MaxChambers = parse[20].ToInt();
                                
                                floor.Darkness = parse[21].ToInt();
                                floor.GoalType = (Enums.RFloorGoalType)parse[22].ToInt();
                                floor.GoalMap = parse[23].ToInt();
                                floor.GoalX = parse[24].ToInt();
                                floor.GoalY = parse[25].ToInt();
                                floor.Music = parse[26];
                                
                                #region terrain
                                
                                #region wall
                                floor.StairsX = parse[27].ToInt();
                                floor.StairsSheet = parse[28].ToInt();

                                floor.mGroundX = parse[29].ToInt();
                                floor.mGroundSheet = parse[30].ToInt();

                                floor.mTopLeftX = parse[31].ToInt();
                                floor.mTopLeftSheet = parse[32].ToInt();
                                floor.mTopCenterX = parse[33].ToInt();
                                floor.mTopCenterSheet = parse[34].ToInt();
                                floor.mTopRightX = parse[35].ToInt();
                                floor.mTopRightSheet = parse[36].ToInt();

                                floor.mCenterLeftX = parse[37].ToInt();
                                floor.mCenterLeftSheet = parse[38].ToInt();
                                floor.mCenterCenterX = parse[39].ToInt();
                                floor.mCenterCenterSheet = parse[40].ToInt();
                                floor.mCenterRightX = parse[41].ToInt();
                                floor.mCenterRightSheet = parse[42].ToInt();

                                floor.mBottomLeftX = parse[43].ToInt();
                                floor.mBottomLeftSheet = parse[44].ToInt();
                                floor.mBottomCenterX = parse[45].ToInt();
                                floor.mBottomCenterSheet = parse[46].ToInt();
                                floor.mBottomRightX = parse[47].ToInt();
                                floor.mBottomRightSheet = parse[48].ToInt();

                                floor.mInnerTopLeftX = parse[49].ToInt();
                                floor.mInnerTopLeftSheet = parse[50].ToInt();
                                floor.mInnerBottomLeftX = parse[51].ToInt();
                                floor.mInnerBottomLeftSheet = parse[52].ToInt();

                                floor.mInnerTopRightX = parse[53].ToInt();
                                floor.mInnerTopRightSheet = parse[54].ToInt();
                                floor.mInnerBottomRightX = parse[55].ToInt();
                                floor.mInnerBottomRightSheet = parse[56].ToInt();
                                
                                floor.mColumnTopX = parse[57].ToInt();
                                floor.mColumnTopSheet = parse[58].ToInt();
                                floor.mColumnCenterX = parse[59].ToInt();
                                floor.mColumnCenterSheet = parse[60].ToInt();
                                floor.mColumnBottomX = parse[61].ToInt();
                                floor.mColumnBottomSheet = parse[62].ToInt();

                                floor.mRowLeftX = parse[63].ToInt();
                                floor.mRowLeftSheet = parse[64].ToInt();
                                floor.mRowCenterX = parse[65].ToInt();
                                floor.mRowCenterSheet = parse[66].ToInt();
                                floor.mRowRightX = parse[67].ToInt();
                                floor.mRowRightSheet = parse[68].ToInt();
                                
                                floor.mIsolatedWallX = parse[69].ToInt();
                                floor.mIsolatedWallSheet = parse[70].ToInt();
                                
                                floor.mGroundAltX = parse[71].ToInt();
                                floor.mGroundAltSheet = parse[72].ToInt();
                                floor.mGroundAlt2X = parse[73].ToInt();
                                floor.mGroundAlt2Sheet = parse[74].ToInt();

                                floor.mTopLeftAltX = parse[75].ToInt();
                                floor.mTopLeftAltSheet = parse[76].ToInt();
                                floor.mTopCenterAltX = parse[77].ToInt();
                                floor.mTopCenterAltSheet = parse[78].ToInt();
                                floor.mTopRightAltX = parse[79].ToInt();
                                floor.mTopRightAltSheet = parse[80].ToInt();

                                floor.mCenterLeftAltX = parse[81].ToInt();
                                floor.mCenterLeftAltSheet = parse[82].ToInt();
                                floor.mCenterCenterAltX = parse[83].ToInt();
                                floor.mCenterCenterAltSheet = parse[84].ToInt();
                                floor.mCenterCenterAlt2X = parse[85].ToInt();
                                floor.mCenterCenterAlt2Sheet = parse[86].ToInt();
                                floor.mCenterRightAltX = parse[87].ToInt();
                                floor.mCenterRightAltSheet = parse[88].ToInt();

                                floor.mBottomLeftAltX = parse[89].ToInt();
                                floor.mBottomLeftAltSheet = parse[90].ToInt();
                                floor.mBottomCenterAltX = parse[91].ToInt();
                                floor.mBottomCenterAltSheet = parse[92].ToInt();
                                floor.mBottomRightAltX = parse[93].ToInt();
                                floor.mBottomRightAltSheet = parse[94].ToInt();

                                floor.mInnerTopLeftAltX = parse[95].ToInt();
                                floor.mInnerTopLeftAltSheet = parse[96].ToInt();
                                floor.mInnerBottomLeftAltX = parse[97].ToInt();
                                floor.mInnerBottomLeftAltSheet = parse[98].ToInt();

                                floor.mInnerTopRightAltX = parse[99].ToInt();
                                floor.mInnerTopRightAltSheet = parse[100].ToInt();
                                floor.mInnerBottomRightAltX = parse[101].ToInt();
                                floor.mInnerBottomRightAltSheet = parse[102].ToInt();
                                
                                floor.mColumnTopAltX = parse[103].ToInt();
                                floor.mColumnTopAltSheet = parse[104].ToInt();
                                floor.mColumnCenterAltX = parse[105].ToInt();
                                floor.mColumnCenterAltSheet = parse[106].ToInt();
                                floor.mColumnBottomAltX = parse[107].ToInt();
                                floor.mColumnBottomAltSheet = parse[108].ToInt();

                                floor.mRowLeftAltX = parse[109].ToInt();
                                floor.mRowLeftAltSheet = parse[110].ToInt();
                                floor.mRowCenterAltX = parse[111].ToInt();
                                floor.mRowCenterAltSheet = parse[112].ToInt();
                                floor.mRowRightAltX = parse[113].ToInt();
                                floor.mRowRightAltSheet = parse[114].ToInt();
                                
                                floor.mIsolatedWallAltX = parse[115].ToInt();
                                floor.mIsolatedWallAltSheet = parse[116].ToInt();
                                
                                #endregion
                                
                                #region water
                                
                                floor.mWaterX = parse[117].ToInt();
                                floor.mWaterSheet = parse[118].ToInt();
                                floor.mWaterAnimX = parse[119].ToInt();
                                floor.mWaterAnimSheet = parse[120].ToInt();
                                
                                floor.mShoreTopLeftX = parse[121].ToInt();
                                floor.mShoreTopLeftSheet = parse[122].ToInt();
                                floor.mShoreTopRightX = parse[123].ToInt();
                                floor.mShoreTopRightSheet = parse[124].ToInt();
                                floor.mShoreBottomRightX = parse[125].ToInt();
                                floor.mShoreBottomRightSheet = parse[126].ToInt();
                                floor.mShoreBottomLeftX = parse[127].ToInt();
                                floor.mShoreBottomLeftSheet = parse[128].ToInt();
                                floor.mShoreDiagonalForwardX = parse[129].ToInt();
                                floor.mShoreDiagonalForwardSheet = parse[130].ToInt();
                                floor.mShoreDiagonalBackX = parse[131].ToInt();
                                floor.mShoreDiagonalBackSheet = parse[132].ToInt();

                                floor.mShoreTopX = parse[133].ToInt();
                                floor.mShoreTopSheet = parse[134].ToInt();
                                floor.mShoreRightX = parse[135].ToInt();
                                floor.mShoreRightSheet = parse[136].ToInt();
                                floor.mShoreBottomX = parse[137].ToInt();
                                floor.mShoreBottomSheet = parse[138].ToInt();
                                floor.mShoreLeftX = parse[139].ToInt();
                                floor.mShoreLeftSheet = parse[140].ToInt();
                                floor.mShoreVerticalX = parse[141].ToInt();
                                floor.mShoreVerticalSheet = parse[142].ToInt();
                                floor.mShoreHorizontalX = parse[143].ToInt();
                                floor.mShoreHorizontalSheet = parse[144].ToInt();

                                floor.mShoreInnerTopLeftX = parse[145].ToInt();
                                floor.mShoreInnerTopLeftSheet = parse[146].ToInt();
                                floor.mShoreInnerTopRightX = parse[147].ToInt();
                                floor.mShoreInnerTopRightSheet = parse[148].ToInt();
                                floor.mShoreInnerBottomRightX = parse[149].ToInt();
                                floor.mShoreInnerBottomRightSheet = parse[150].ToInt();
                                floor.mShoreInnerBottomLeftX = parse[151].ToInt();
                                floor.mShoreInnerBottomLeftSheet = parse[152].ToInt();

                                floor.mShoreInnerTopX = parse[153].ToInt();
                                floor.mShoreInnerTopSheet = parse[154].ToInt();
                                floor.mShoreInnerRightX = parse[155].ToInt();
                                floor.mShoreInnerRightSheet = parse[156].ToInt();
                                floor.mShoreInnerBottomX = parse[157].ToInt();
                                floor.mShoreInnerBottomSheet = parse[158].ToInt();
                                floor.mShoreInnerLeftX = parse[159].ToInt();
                                floor.mShoreInnerLeftSheet = parse[160].ToInt();

                                floor.mShoreSurroundedX = parse[161].ToInt();
                                floor.mShoreSurroundedSheet = parse[162].ToInt();
                                
                                floor.mShoreTopLeftAnimX = parse[163].ToInt();
                                floor.mShoreTopLeftAnimSheet = parse[164].ToInt();
                                floor.mShoreTopRightAnimX = parse[165].ToInt();
                                floor.mShoreTopRightAnimSheet = parse[166].ToInt();
                                floor.mShoreBottomRightAnimX = parse[167].ToInt();
                                floor.mShoreBottomRightAnimSheet = parse[168].ToInt();
                                floor.mShoreBottomLeftAnimX = parse[169].ToInt();
                                floor.mShoreBottomLeftAnimSheet = parse[170].ToInt();
                                floor.mShoreDiagonalForwardAnimX = parse[171].ToInt();
                                floor.mShoreDiagonalForwardAnimSheet = parse[172].ToInt();
                                floor.mShoreDiagonalBackAnimX = parse[173].ToInt();
                                floor.mShoreDiagonalBackAnimSheet = parse[174].ToInt();

                                floor.mShoreTopAnimX = parse[175].ToInt();
                                floor.mShoreTopAnimSheet = parse[176].ToInt();
                                floor.mShoreRightAnimX = parse[177].ToInt();
                                floor.mShoreRightAnimSheet = parse[178].ToInt();
                                floor.mShoreBottomAnimX = parse[179].ToInt();
                                floor.mShoreBottomAnimSheet = parse[180].ToInt();
                                floor.mShoreLeftAnimX = parse[181].ToInt();
                                floor.mShoreLeftAnimSheet = parse[182].ToInt();
                                floor.mShoreVerticalAnimX = parse[183].ToInt();
                                floor.mShoreVerticalAnimSheet = parse[184].ToInt();
                                floor.mShoreHorizontalAnimX = parse[185].ToInt();
                                floor.mShoreHorizontalAnimSheet = parse[186].ToInt();

                                floor.mShoreInnerTopLeftAnimX = parse[187].ToInt();
                                floor.mShoreInnerTopLeftAnimSheet = parse[188].ToInt();
                                floor.mShoreInnerTopRightAnimX = parse[189].ToInt();
                                floor.mShoreInnerTopRightAnimSheet = parse[190].ToInt();
                                floor.mShoreInnerBottomRightAnimX = parse[191].ToInt();
                                floor.mShoreInnerBottomRightAnimSheet = parse[192].ToInt();
                                floor.mShoreInnerBottomLeftAnimX = parse[193].ToInt();
                                floor.mShoreInnerBottomLeftAnimSheet = parse[194].ToInt();

                                floor.mShoreInnerTopAnimX = parse[195].ToInt();
                                floor.mShoreInnerTopAnimSheet = parse[196].ToInt();
                                floor.mShoreInnerRightAnimX = parse[197].ToInt();
                                floor.mShoreInnerRightAnimSheet = parse[198].ToInt();
                                floor.mShoreInnerBottomAnimX = parse[199].ToInt();
                                floor.mShoreInnerBottomAnimSheet = parse[200].ToInt();
                                floor.mShoreInnerLeftAnimX = parse[201].ToInt();
                                floor.mShoreInnerLeftAnimSheet = parse[202].ToInt();

                                floor.mShoreSurroundedAnimX = parse[203].ToInt();
                                floor.mShoreSurroundedAnimSheet = parse[204].ToInt();
                                            
                                #endregion
                                #endregion
                                
                                floor.GroundTile.Type = (Enums.TileType)parse[205].ToInt();
                                floor.GroundTile.Data1 = parse[206].ToInt();
                                floor.GroundTile.Data2 = parse[207].ToInt();
                                floor.GroundTile.Data3 = parse[208].ToInt();
                                floor.GroundTile.String1 = parse[209];
                                floor.GroundTile.String2 = parse[210];
                                floor.GroundTile.String3 = parse[211];
                                
                                floor.HallTile.Type = (Enums.TileType)parse[212].ToInt();
                                floor.HallTile.Data1 = parse[213].ToInt();
                                floor.HallTile.Data2 = parse[214].ToInt();
                                floor.HallTile.Data3 = parse[215].ToInt();
                                floor.HallTile.String1 = parse[216];
                                floor.HallTile.String2 = parse[217];
                                floor.HallTile.String3 = parse[218];
                                
                                floor.WaterTile.Type = (Enums.TileType)parse[219].ToInt();
                                floor.WaterTile.Data1 = parse[220].ToInt();
                                floor.WaterTile.Data2 = parse[221].ToInt();
                                floor.WaterTile.Data3 = parse[222].ToInt();
                                floor.WaterTile.String1 = parse[223];
                                floor.WaterTile.String2 = parse[224];
                                floor.WaterTile.String3 = parse[225];
                                
                                floor.WallTile.Type = (Enums.TileType)parse[226].ToInt();
                                floor.WallTile.Data1 = parse[227].ToInt();
                                floor.WallTile.Data2 = parse[228].ToInt();
                                floor.WallTile.Data3 = parse[229].ToInt();
                                floor.WallTile.String1 = parse[230];
                                floor.WallTile.String2 = parse[231];
                                floor.WallTile.String3 = parse[232];
                                
                                floor.NpcSpawnTime = parse[233].ToInt();
                                floor.NpcMin = parse[234].ToInt();
                                floor.NpcMax = parse[235].ToInt();
                                
                                int maxItems = parse[236].ToInt();
                                int maxNpcs = parse[237].ToInt();
                                int maxSpecialTiles = parse[238].ToInt();
                                int maxWeather = parse[239].ToInt();
                                int maxChambers = parse[240].ToInt();

                                int n = 241;
                                
                                RDungeonItem item;
                                MapNpcPreset npc;
                                RDungeonTrap specialTile;
                                Server.RDungeons.RDungeonPresetChamber presetChamber;
                                
                                for (int i = 0; i < maxItems; i++) {
                                		item = new RDungeonItem();
                                    item.ItemNum = parse[n].ToInt();
                                    item.MinAmount = parse[n+1].ToInt();
                                    item.MaxAmount = parse[n+2].ToInt();
                                    item.AppearanceRate = parse[n+3].ToInt();
                                    item.StickyRate = parse[n+4].ToInt();
                                    item.Tag = parse[n+5];
                                    item.Hidden = parse[n+6].ToBool();
                                    item.OnGround = parse[n+7].ToBool();
                                    item.OnWater = parse[n+8].ToBool();
                                    item.OnWall = parse[n+9].ToBool();
                                    floor.Items.Add(item);
                                    n += 10;
                                }
                                
                                for (int i = 0; i < maxNpcs; i++) {
                                    npc = new MapNpcPreset();
                                    npc.NpcNum = parse[n].ToInt();
                                    npc.MinLevel = parse[n + 1].ToInt();
                                    npc.MaxLevel = parse[n + 2].ToInt();
                                    npc.AppearanceRate = parse[n + 3].ToInt();
                                    npc.StartStatus = (Enums.StatusAilment)parse[n + 4].ToInt();
                                    npc.StartStatusCounter = parse[n + 5].ToInt();
                                    npc.StartStatusChance = parse[n + 6].ToInt();
                                    floor.Npcs.Add(npc);
                                    n+= 7;
                                }
                                
                                for (int i = 0; i < maxSpecialTiles; i++) {
                                		specialTile = new RDungeonTrap();
                                		specialTile.SpecialTile.Type = (Enums.TileType)parse[n].ToInt();
                                		specialTile.SpecialTile.Data1 = parse[n + 1].ToInt();
                                		specialTile.SpecialTile.Data2 = parse[n + 2].ToInt();
                                		specialTile.SpecialTile.Data3 = parse[n + 3].ToInt();
                                		specialTile.SpecialTile.String1 = parse[n + 4];
                                		specialTile.SpecialTile.String2 = parse[n + 5];
                                		specialTile.SpecialTile.String3 = parse[n + 6];
                                		specialTile.SpecialTile.Ground = parse[n + 7].ToInt();
                                		specialTile.SpecialTile.GroundSet = parse[n + 8].ToInt();
                                		specialTile.SpecialTile.GroundAnim = parse[n + 9].ToInt();
                                		specialTile.SpecialTile.GroundAnimSet = parse[n + 10].ToInt();
                                		specialTile.SpecialTile.Mask = parse[n + 11].ToInt();
                                		specialTile.SpecialTile.MaskSet = parse[n + 12].ToInt();
                                		specialTile.SpecialTile.Anim	 = parse[n + 13].ToInt();
                                		specialTile.SpecialTile.AnimSet = parse[n + 14].ToInt();
                                		specialTile.SpecialTile.Mask2 = parse[n + 15].ToInt();
                                		specialTile.SpecialTile.Mask2Set = parse[n + 16].ToInt();
                                		specialTile.SpecialTile.M2Anim = parse[n + 17].ToInt();
                                		specialTile.SpecialTile.M2AnimSet = parse[n + 18].ToInt();
                                		specialTile.SpecialTile.Fringe = parse[n + 19].ToInt();
                                		specialTile.SpecialTile.FringeSet = parse[n + 20].ToInt();
                                		specialTile.SpecialTile.FAnim = parse[n + 21].ToInt();
                                		specialTile.SpecialTile.FAnimSet = parse[n + 22].ToInt();
                                		specialTile.SpecialTile.Fringe2 = parse[n + 23].ToInt();
                                		specialTile.SpecialTile.Fringe2Set = parse[n + 24].ToInt();
                                		specialTile.SpecialTile.F2Anim = parse[n + 25].ToInt();
                                		specialTile.SpecialTile.F2AnimSet = parse[n + 26].ToInt();
                                        specialTile.SpecialTile.RDungeonMapValue = parse[n + 27].ToInt();
                                        specialTile.AppearanceRate = parse[n + 28].ToInt();
                                		floor.SpecialTiles.Add(specialTile);
                                    n+= 29;
                                }
                                
                                for (int i = 0; i < maxWeather; i++) {
                                    floor.Weather.Add((Enums.Weather)parse[n].ToInt());
                                    n++;
                                }
                                for (int i = 0; i < maxChambers; i++)
                                {
                                    presetChamber = new Server.RDungeons.RDungeonPresetChamber();
                                    presetChamber.ChamberNum = parse[n].ToInt();
                                    presetChamber.String1 = parse[n + 1];
                                    presetChamber.String2 = parse[n + 2];
                                    presetChamber.String3 = parse[n + 3];
                                    n += 3;
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
                writer.WriteLine("RDungeonData|V4");
                writer.WriteLine("Data|" + rdungeon.DungeonName + "|" + ((int)rdungeon.Direction).ToString() + "|" + rdungeon.MaxFloors.ToString() + "|" + rdungeon.Recruitment.ToString() + "|" + rdungeon.Exp.ToString() + "|" + rdungeon.WindTimer.ToString() + "|" + rdungeon.WindTimer.ToString() + "|" + rdungeon.DungeonIndex.ToString() + "|");
                for (int i = 0; i < rdungeon.Floors.Count; i++) {
                	string data = "Floor|"
                		+ rdungeon.Floors[i].Options.TrapMin.ToString() + "|" + rdungeon.Floors[i].Options.TrapMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.ItemMin.ToString() + "|" + rdungeon.Floors[i].Options.ItemMax.ToString() + "|"
                        + rdungeon.Floors[i].Options.Intricacy.ToString() + "|"
                		+ rdungeon.Floors[i].Options.RoomWidthMin.ToString() + "|" + rdungeon.Floors[i].Options.RoomWidthMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.RoomLengthMin.ToString() + "|" + rdungeon.Floors[i].Options.RoomLengthMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.HallTurnMin.ToString() + "|" + rdungeon.Floors[i].Options.HallTurnMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.HallVarMin.ToString() + "|" + rdungeon.Floors[i].Options.HallVarMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.WaterFrequency.ToString() + "|" + rdungeon.Floors[i].Options.Craters.ToString() + "|"
                		+ rdungeon.Floors[i].Options.CraterMinLength.ToString() + "|" + rdungeon.Floors[i].Options.CraterMaxLength.ToString() + "|"
                		+ rdungeon.Floors[i].Options.CraterFuzzy.ToIntString() + "|"
                        + rdungeon.Floors[i].Options.MinChambers.ToString() + "|" + rdungeon.Floors[i].Options.MaxChambers.ToString() + "|"
                        + rdungeon.Floors[i].Darkness.ToString() + "|"
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
                		+ rdungeon.Floors[i].mGroundAltX.ToString() + "|" + rdungeon.Floors[i].mGroundAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mGroundAlt2X.ToString() + "|" + rdungeon.Floors[i].mGroundAlt2Sheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopLeftAltX.ToString() + "|" + rdungeon.Floors[i].mTopLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopCenterAltX.ToString() + "|" + rdungeon.Floors[i].mTopCenterAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mTopRightAltX.ToString() + "|" + rdungeon.Floors[i].mTopRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterLeftAltX.ToString() + "|" + rdungeon.Floors[i].mCenterLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterCenterAltX.ToString() + "|" + rdungeon.Floors[i].mCenterCenterAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterCenterAlt2X.ToString() + "|" + rdungeon.Floors[i].mCenterCenterAlt2Sheet.ToString() + "|"
                		+ rdungeon.Floors[i].mCenterRightAltX.ToString() + "|" + rdungeon.Floors[i].mCenterRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomLeftAltX.ToString() + "|" + rdungeon.Floors[i].mBottomLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomCenterAltX.ToString() + "|" + rdungeon.Floors[i].mBottomCenterAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mBottomRightAltX.ToString() + "|" + rdungeon.Floors[i].mBottomRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerTopLeftAltX.ToString() + "|" + rdungeon.Floors[i].mInnerTopLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerBottomLeftAltX.ToString() + "|" + rdungeon.Floors[i].mInnerBottomLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerTopRightAltX.ToString() + "|" + rdungeon.Floors[i].mInnerTopRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mInnerBottomRightAltX.ToString() + "|" + rdungeon.Floors[i].mInnerBottomRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnTopAltX.ToString() + "|" + rdungeon.Floors[i].mColumnTopAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnCenterAltX.ToString() + "|" + rdungeon.Floors[i].mColumnCenterAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mColumnBottomAltX.ToString() + "|" + rdungeon.Floors[i].mColumnBottomAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowLeftAltX.ToString() + "|" + rdungeon.Floors[i].mRowLeftAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowCenterAltX.ToString() + "|" + rdungeon.Floors[i].mRowCenterAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mRowRightAltX.ToString() + "|" + rdungeon.Floors[i].mRowRightAltSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mIsolatedWallAltX.ToString() + "|" + rdungeon.Floors[i].mIsolatedWallAltSheet.ToString() + "|"
                		
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
                		
                		+ rdungeon.Floors[i].mShoreTopLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreTopLeftAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreTopRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreTopRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomLeftAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreDiagonalForwardAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreDiagonalForwardAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreDiagonalBackAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreDiagonalBackAnimSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreTopAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreTopAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreBottomAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreBottomAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreLeftAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreVerticalAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreVerticalAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreHorizontalAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreHorizontalAnimSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreInnerTopLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopLeftAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerTopRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomLeftAnimSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreInnerTopAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerTopAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerRightAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerRightAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerBottomAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerBottomAnimSheet.ToString() + "|"
                		+ rdungeon.Floors[i].mShoreInnerLeftAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreInnerLeftAnimSheet.ToString() + "|"
                		
                		+ rdungeon.Floors[i].mShoreSurroundedAnimX.ToString() + "|" + rdungeon.Floors[i].mShoreSurroundedAnimSheet.ToString() + "|"
                		
                		+ ((int)rdungeon.Floors[i].GroundTile.Type).ToString() + "|"
                		+ rdungeon.Floors[i].GroundTile.Data1.ToString() + "|" + rdungeon.Floors[i].GroundTile.Data2.ToString() + "|" + rdungeon.Floors[i].GroundTile.Data3.ToString() + "|"
                		+ rdungeon.Floors[i].GroundTile.String1 + "|" + rdungeon.Floors[i].GroundTile.String2 + "|" + rdungeon.Floors[i].GroundTile.String3 + "|"
                		
                		+ ((int)rdungeon.Floors[i].HallTile.Type).ToString() + "|"
                		+ rdungeon.Floors[i].HallTile.Data1.ToString() + "|" + rdungeon.Floors[i].HallTile.Data2.ToString() + "|" + rdungeon.Floors[i].HallTile.Data3.ToString() + "|"
                		+ rdungeon.Floors[i].HallTile.String1 + "|" + rdungeon.Floors[i].HallTile.String2 + "|" + rdungeon.Floors[i].HallTile.String3 + "|"
                                
                		+ ((int)rdungeon.Floors[i].WaterTile.Type).ToString() + "|"
                		+ rdungeon.Floors[i].WaterTile.Data1.ToString() + "|" + rdungeon.Floors[i].WaterTile.Data2.ToString() + "|" + rdungeon.Floors[i].WaterTile.Data3.ToString() + "|"
                		+ rdungeon.Floors[i].WaterTile.String1 + "|" + rdungeon.Floors[i].WaterTile.String2 + "|" + rdungeon.Floors[i].WaterTile.String3 + "|"
                                
                		+ ((int)rdungeon.Floors[i].WallTile.Type).ToString() + "|"
                		+ rdungeon.Floors[i].WallTile.Data1.ToString() + "|" + rdungeon.Floors[i].WallTile.Data2.ToString() + "|" + rdungeon.Floors[i].WallTile.Data3.ToString() + "|"
                		+ rdungeon.Floors[i].WallTile.String1 + "|" + rdungeon.Floors[i].WallTile.String2 + "|" + rdungeon.Floors[i].WallTile.String3 + "|"
                		
                		+ rdungeon.Floors[i].NpcSpawnTime.ToString() + "|" + rdungeon.Floors[i].NpcMin.ToString() + "|" + rdungeon.Floors[i].NpcMax.ToString() + "|"
                		
                		+ rdungeon.Floors[i].Items.Count + "|" + rdungeon.Floors[i].Npcs.Count + "|" + rdungeon.Floors[i].SpecialTiles.Count + "|"
                        + rdungeon.Floors[i].Weather.Count + "|" + rdungeon.Floors[i].Options.Chambers.Count + "|";
                		
                		
                		for (int item = 0; item < rdungeon.Floors[i].Items.Count; item++) {
                        data += rdungeon.Floors[i].Items[item].ItemNum.ToString() + "|";
                        data += rdungeon.Floors[i].Items[item].MinAmount.ToString() + "|";
                        data += rdungeon.Floors[i].Items[item].MaxAmount.ToString() + "|";
                        data += rdungeon.Floors[i].Items[item].AppearanceRate.ToString() + "|";
                        data += rdungeon.Floors[i].Items[item].StickyRate.ToString() + "|";
                        data += rdungeon.Floors[i].Items[item].Tag + "|";
                        data += rdungeon.Floors[i].Items[item].Hidden.ToIntString() + "|";
                        data += rdungeon.Floors[i].Items[item].OnGround.ToIntString() + "|";
                        data += rdungeon.Floors[i].Items[item].OnWater.ToIntString() + "|";
                        data += rdungeon.Floors[i].Items[item].OnWall.ToIntString() + "|";
                    }
                    for (int npc = 0; npc < rdungeon.Floors[i].Npcs.Count; npc++) {
                        data += rdungeon.Floors[i].Npcs[npc].NpcNum.ToString() + "|";
                        data += rdungeon.Floors[i].Npcs[npc].MinLevel.ToString() + "|";
                        data += rdungeon.Floors[i].Npcs[npc].MaxLevel.ToString() + "|";
                        data += rdungeon.Floors[i].Npcs[npc].AppearanceRate.ToString() + "|";
                        data += ((int)rdungeon.Floors[i].Npcs[npc].StartStatus).ToString() + "|";
                        data += rdungeon.Floors[i].Npcs[npc].StartStatusCounter.ToString() + "|";
                        data += rdungeon.Floors[i].Npcs[npc].StartStatusChance.ToString() + "|";
                    }
                    for (int trap = 0; trap < rdungeon.Floors[i].SpecialTiles.Count; trap++) {
                        data += ((int)rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Type).ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Data1.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Data2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Data3.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.String1 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.String2 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.String3 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Ground.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.GroundSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.GroundAnim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.GroundAnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Mask.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.MaskSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.AnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Mask2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Mask2Set.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.M2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.M2AnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Fringe.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.FringeSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.FAnim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.FAnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Fringe2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.Fringe2Set.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.F2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.F2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].SpecialTile.RDungeonMapValue.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].AppearanceRate.ToString() + "|";
                    }
                    for (int weather = 0; weather < rdungeon.Floors[i].Weather.Count; weather++) {
                        data += ((int)rdungeon.Floors[i].Weather[weather]).ToString() + "|";
                    }
                    for (int chamber = 0; chamber < rdungeon.Floors[i].Options.Chambers.Count; chamber++)
                    {
                        data += rdungeon.Floors[i].Options.Chambers[i].ChamberNum.ToString() + "|";
                        data += rdungeon.Floors[i].Options.Chambers[i].String1 + "|";
                        data += rdungeon.Floors[i].Options.Chambers[i].String2 + "|";
                        data += rdungeon.Floors[i].Options.Chambers[i].String3 + "|";
                    }
                	
                    writer.WriteLine(data);

                }
        }
	}
}
}
