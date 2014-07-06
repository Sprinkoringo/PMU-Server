using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;

namespace Server.DataConverter.RDungeons.V3
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
                            if (parse[1].ToLower() != "v3") {
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
                                floor.Options.TrapMin = parse[1].ToInt();
                                floor.Options.TrapMax = parse[2].ToInt();
                                floor.Options.ItemMin = parse[3].ToInt();
                                floor.Options.ItemMax = parse[4].ToInt();
                                floor.Options.RoomWidthMin = parse[5].ToInt();
                                floor.Options.RoomWidthMax = parse[6].ToInt();
                                floor.Options.RoomLengthMin = parse[7].ToInt();
                                floor.Options.RoomLengthMax = parse[8].ToInt();
                                floor.Options.HallTurnMin = parse[9].ToInt();
                                floor.Options.HallTurnMax = parse[10].ToInt();
                                floor.Options.HallVarMin = parse[11].ToInt();
                                floor.Options.HallVarMax = parse[12].ToInt();
                                floor.Options.WaterFrequency = parse[13].ToInt();
                                floor.Options.Craters = parse[14].ToInt();
                                floor.Options.CraterMinLength = parse[15].ToInt();
                                floor.Options.CraterMaxLength = parse[16].ToInt();
                                floor.Options.CraterFuzzy = parse[17].ToBool();
                                
                                floor.Darkness = parse[18].ToInt();
                                floor.GoalType = (Enums.RFloorGoalType)parse[19].ToInt();
                                floor.GoalMap = parse[20].ToInt();
                                floor.GoalX = parse[21].ToInt();
                                floor.GoalY = parse[22].ToInt();
                                floor.Music = parse[23];
                                
                                #region terrain
                                
                                #region wall
                                floor.StairsX = parse[24].ToInt();
                                floor.StairsSheet = parse[25].ToInt();

                                floor.mGroundX = parse[26].ToInt();
                                floor.mGroundSheet = parse[27].ToInt();

                                floor.mTopLeftX = parse[28].ToInt();
                                floor.mTopLeftSheet = parse[29].ToInt();
                                floor.mTopCenterX = parse[30].ToInt();
                                floor.mTopCenterSheet = parse[31].ToInt();
                                floor.mTopRightX = parse[32].ToInt();
                                floor.mTopRightSheet = parse[33].ToInt();

                                floor.mCenterLeftX = parse[34].ToInt();
                                floor.mCenterLeftSheet = parse[35].ToInt();
                                floor.mCenterCenterX = parse[36].ToInt();
                                floor.mCenterCenterSheet = parse[37].ToInt();
                                floor.mCenterRightX = parse[38].ToInt();
                                floor.mCenterRightSheet = parse[39].ToInt();

                                floor.mBottomLeftX = parse[40].ToInt();
                                floor.mBottomLeftSheet = parse[41].ToInt();
                                floor.mBottomCenterX = parse[42].ToInt();
                                floor.mBottomCenterSheet = parse[43].ToInt();
                                floor.mBottomRightX = parse[44].ToInt();
                                floor.mBottomRightSheet = parse[45].ToInt();

                                floor.mInnerTopLeftX = parse[46].ToInt();
                                floor.mInnerTopLeftSheet = parse[47].ToInt();
                                floor.mInnerBottomLeftX = parse[48].ToInt();
                                floor.mInnerBottomLeftSheet = parse[49].ToInt();

                                floor.mInnerTopRightX = parse[50].ToInt();
                                floor.mInnerTopRightSheet = parse[51].ToInt();
                                floor.mInnerBottomRightX = parse[52].ToInt();
                                floor.mInnerBottomRightSheet = parse[53].ToInt();
                                
                                floor.mColumnTopX = parse[54].ToInt();
                                floor.mColumnTopSheet = parse[55].ToInt();
                                floor.mColumnCenterX = parse[56].ToInt();
                                floor.mColumnCenterSheet = parse[57].ToInt();
                                floor.mColumnBottomX = parse[58].ToInt();
                                floor.mColumnBottomSheet = parse[59].ToInt();

                                floor.mRowLeftX = parse[60].ToInt();
                                floor.mRowLeftSheet = parse[61].ToInt();
                                floor.mRowCenterX = parse[62].ToInt();
                                floor.mRowCenterSheet = parse[63].ToInt();
                                floor.mRowRightX = parse[64].ToInt();
                                floor.mRowRightSheet = parse[65].ToInt();
                                
                                floor.mIsolatedWallX = parse[66].ToInt();
                                floor.mIsolatedWallSheet = parse[67].ToInt();
                                
                                floor.mGroundAltX = parse[68].ToInt();
                                floor.mGroundAltSheet = parse[69].ToInt();
                                floor.mGroundAlt2X = parse[70].ToInt();
                                floor.mGroundAlt2Sheet = parse[71].ToInt();

                                floor.mTopLeftAltX = parse[72].ToInt();
                                floor.mTopLeftAltSheet = parse[73].ToInt();
                                floor.mTopCenterAltX = parse[74].ToInt();
                                floor.mTopCenterAltSheet = parse[75].ToInt();
                                floor.mTopRightAltX = parse[76].ToInt();
                                floor.mTopRightAltSheet = parse[77].ToInt();

                                floor.mCenterLeftAltX = parse[78].ToInt();
                                floor.mCenterLeftAltSheet = parse[79].ToInt();
                                floor.mCenterCenterAltX = parse[80].ToInt();
                                floor.mCenterCenterAltSheet = parse[81].ToInt();
                                floor.mCenterCenterAlt2X = parse[82].ToInt();
                                floor.mCenterCenterAlt2Sheet = parse[83].ToInt();
                                floor.mCenterRightAltX = parse[84].ToInt();
                                floor.mCenterRightAltSheet = parse[85].ToInt();

                                floor.mBottomLeftAltX = parse[86].ToInt();
                                floor.mBottomLeftAltSheet = parse[87].ToInt();
                                floor.mBottomCenterAltX = parse[88].ToInt();
                                floor.mBottomCenterAltSheet = parse[89].ToInt();
                                floor.mBottomRightAltX = parse[90].ToInt();
                                floor.mBottomRightAltSheet = parse[91].ToInt();

                                floor.mInnerTopLeftAltX = parse[92].ToInt();
                                floor.mInnerTopLeftAltSheet = parse[93].ToInt();
                                floor.mInnerBottomLeftAltX = parse[94].ToInt();
                                floor.mInnerBottomLeftAltSheet = parse[95].ToInt();

                                floor.mInnerTopRightAltX = parse[96].ToInt();
                                floor.mInnerTopRightAltSheet = parse[97].ToInt();
                                floor.mInnerBottomRightAltX = parse[98].ToInt();
                                floor.mInnerBottomRightAltSheet = parse[99].ToInt();
                                
                                floor.mColumnTopAltX = parse[100].ToInt();
                                floor.mColumnTopAltSheet = parse[101].ToInt();
                                floor.mColumnCenterAltX = parse[102].ToInt();
                                floor.mColumnCenterAltSheet = parse[103].ToInt();
                                floor.mColumnBottomAltX = parse[104].ToInt();
                                floor.mColumnBottomAltSheet = parse[105].ToInt();

                                floor.mRowLeftAltX = parse[106].ToInt();
                                floor.mRowLeftAltSheet = parse[107].ToInt();
                                floor.mRowCenterAltX = parse[108].ToInt();
                                floor.mRowCenterAltSheet = parse[109].ToInt();
                                floor.mRowRightAltX = parse[110].ToInt();
                                floor.mRowRightAltSheet = parse[111].ToInt();
                                
                                floor.mIsolatedWallAltX = parse[112].ToInt();
                                floor.mIsolatedWallAltSheet = parse[113].ToInt();
                                
                                #endregion
                                
                                #region water
                                
                                floor.mWaterX = parse[114].ToInt();
                                floor.mWaterSheet = parse[115].ToInt();
                                floor.mWaterAnimX = parse[116].ToInt();
                                floor.mWaterAnimSheet = parse[117].ToInt();
                                
                                floor.mShoreTopLeftX = parse[118].ToInt();
                                floor.mShoreTopLeftSheet = parse[119].ToInt();
                                floor.mShoreTopRightX = parse[120].ToInt();
                                floor.mShoreTopRightSheet = parse[121].ToInt();
                                floor.mShoreBottomRightX = parse[122].ToInt();
                                floor.mShoreBottomRightSheet = parse[123].ToInt();
                                floor.mShoreBottomLeftX = parse[124].ToInt();
                                floor.mShoreBottomLeftSheet = parse[125].ToInt();
                                floor.mShoreDiagonalForwardX = parse[126].ToInt();
                                floor.mShoreDiagonalForwardSheet = parse[127].ToInt();
                                floor.mShoreDiagonalBackX = parse[128].ToInt();
                                floor.mShoreDiagonalBackSheet = parse[129].ToInt();

                                floor.mShoreTopX = parse[130].ToInt();
                                floor.mShoreTopSheet = parse[131].ToInt();
                                floor.mShoreRightX = parse[132].ToInt();
                                floor.mShoreRightSheet = parse[133].ToInt();
                                floor.mShoreBottomX = parse[134].ToInt();
                                floor.mShoreBottomSheet = parse[135].ToInt();
                                floor.mShoreLeftX = parse[136].ToInt();
                                floor.mShoreLeftSheet = parse[137].ToInt();
                                floor.mShoreVerticalX = parse[138].ToInt();
                                floor.mShoreVerticalSheet = parse[139].ToInt();
                                floor.mShoreHorizontalX = parse[140].ToInt();
                                floor.mShoreHorizontalSheet = parse[141].ToInt();

                                floor.mShoreInnerTopLeftX = parse[142].ToInt();
                                floor.mShoreInnerTopLeftSheet = parse[143].ToInt();
                                floor.mShoreInnerTopRightX = parse[144].ToInt();
                                floor.mShoreInnerTopRightSheet = parse[145].ToInt();
                                floor.mShoreInnerBottomRightX = parse[146].ToInt();
                                floor.mShoreInnerBottomRightSheet = parse[147].ToInt();
                                floor.mShoreInnerBottomLeftX = parse[148].ToInt();
                                floor.mShoreInnerBottomLeftSheet = parse[149].ToInt();

                                floor.mShoreInnerTopX = parse[150].ToInt();
                                floor.mShoreInnerTopSheet = parse[151].ToInt();
                                floor.mShoreInnerRightX = parse[152].ToInt();
                                floor.mShoreInnerRightSheet = parse[153].ToInt();
                                floor.mShoreInnerBottomX = parse[154].ToInt();
                                floor.mShoreInnerBottomSheet = parse[155].ToInt();
                                floor.mShoreInnerLeftX = parse[156].ToInt();
                                floor.mShoreInnerLeftSheet = parse[157].ToInt();

                                floor.mShoreSurroundedX = parse[158].ToInt();
                                floor.mShoreSurroundedSheet = parse[159].ToInt();
                                
                                floor.mShoreTopLeftAnimX = parse[160].ToInt();
                                floor.mShoreTopLeftAnimSheet = parse[161].ToInt();
                                floor.mShoreTopRightAnimX = parse[162].ToInt();
                                floor.mShoreTopRightAnimSheet = parse[163].ToInt();
                                floor.mShoreBottomRightAnimX = parse[164].ToInt();
                                floor.mShoreBottomRightAnimSheet = parse[165].ToInt();
                                floor.mShoreBottomLeftAnimX = parse[166].ToInt();
                                floor.mShoreBottomLeftAnimSheet = parse[167].ToInt();
                                floor.mShoreDiagonalForwardAnimX = parse[168].ToInt();
                                floor.mShoreDiagonalForwardAnimSheet = parse[169].ToInt();
                                floor.mShoreDiagonalBackAnimX = parse[170].ToInt();
                                floor.mShoreDiagonalBackAnimSheet = parse[171].ToInt();

                                floor.mShoreTopAnimX = parse[172].ToInt();
                                floor.mShoreTopAnimSheet = parse[173].ToInt();
                                floor.mShoreRightAnimX = parse[174].ToInt();
                                floor.mShoreRightAnimSheet = parse[175].ToInt();
                                floor.mShoreBottomAnimX = parse[176].ToInt();
                                floor.mShoreBottomAnimSheet = parse[177].ToInt();
                                floor.mShoreLeftAnimX = parse[178].ToInt();
                                floor.mShoreLeftAnimSheet = parse[179].ToInt();
                                floor.mShoreVerticalAnimX = parse[180].ToInt();
                                floor.mShoreVerticalAnimSheet = parse[181].ToInt();
                                floor.mShoreHorizontalAnimX = parse[182].ToInt();
                                floor.mShoreHorizontalAnimSheet = parse[183].ToInt();

                                floor.mShoreInnerTopLeftAnimX = parse[184].ToInt();
                                floor.mShoreInnerTopLeftAnimSheet = parse[185].ToInt();
                                floor.mShoreInnerTopRightAnimX = parse[186].ToInt();
                                floor.mShoreInnerTopRightAnimSheet = parse[187].ToInt();
                                floor.mShoreInnerBottomRightAnimX = parse[188].ToInt();
                                floor.mShoreInnerBottomRightAnimSheet = parse[189].ToInt();
                                floor.mShoreInnerBottomLeftAnimX = parse[190].ToInt();
                                floor.mShoreInnerBottomLeftAnimSheet = parse[191].ToInt();

                                floor.mShoreInnerTopAnimX = parse[192].ToInt();
                                floor.mShoreInnerTopAnimSheet = parse[193].ToInt();
                                floor.mShoreInnerRightAnimX = parse[194].ToInt();
                                floor.mShoreInnerRightAnimSheet = parse[195].ToInt();
                                floor.mShoreInnerBottomAnimX = parse[196].ToInt();
                                floor.mShoreInnerBottomAnimSheet = parse[197].ToInt();
                                floor.mShoreInnerLeftAnimX = parse[198].ToInt();
                                floor.mShoreInnerLeftAnimSheet = parse[199].ToInt();

                                floor.mShoreSurroundedAnimX = parse[200].ToInt();
                                floor.mShoreSurroundedAnimSheet = parse[201].ToInt();
                                            
                                #endregion
                                #endregion
                                
                                floor.GroundTile.Type = (Enums.TileType)parse[202].ToInt();
                                floor.GroundTile.Data1 = parse[203].ToInt();
                                floor.GroundTile.Data2 = parse[204].ToInt();
                                floor.GroundTile.Data3 = parse[205].ToInt();
                                floor.GroundTile.String1 = parse[206];
                                floor.GroundTile.String2 = parse[207];
                                floor.GroundTile.String3 = parse[208];
                                
                                floor.HallTile.Type = (Enums.TileType)parse[209].ToInt();
                                floor.HallTile.Data1 = parse[210].ToInt();
                                floor.HallTile.Data2 = parse[211].ToInt();
                                floor.HallTile.Data3 = parse[212].ToInt();
                                floor.HallTile.String1 = parse[213];
                                floor.HallTile.String2 = parse[214];
                                floor.HallTile.String3 = parse[215];
                                
                                floor.WaterTile.Type = (Enums.TileType)parse[216].ToInt();
                                floor.WaterTile.Data1 = parse[217].ToInt();
                                floor.WaterTile.Data2 = parse[218].ToInt();
                                floor.WaterTile.Data3 = parse[219].ToInt();
                                floor.WaterTile.String1 = parse[220];
                                floor.WaterTile.String2 = parse[221];
                                floor.WaterTile.String3 = parse[222];
                                
                                floor.WallTile.Type = (Enums.TileType)parse[223].ToInt();
                                floor.WallTile.Data1 = parse[224].ToInt();
                                floor.WallTile.Data2 = parse[225].ToInt();
                                floor.WallTile.Data3 = parse[226].ToInt();
                                floor.WallTile.String1 = parse[227];
                                floor.WallTile.String2 = parse[228];
                                floor.WallTile.String3 = parse[229];
                                
                                floor.NpcSpawnTime = parse[230].ToInt();
                                floor.NpcMin = parse[231].ToInt();
                                floor.NpcMax = parse[232].ToInt();
                                
                                int maxItems = parse[233].ToInt();
                                int maxNpcs = parse[234].ToInt();
                                int maxSpecialTiles = parse[235].ToInt();
                                int maxWeather = parse[236].ToInt();

                                int n = 237;
                                
                                RDungeonItem item;
                                RDungeonNpc npc;
                                Tile specialTile;
                                
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
                                		npc = new RDungeonNpc();
                                    npc.NpcNum = parse[n].ToInt();
                                    npc.MinLevel = parse[n + 1].ToInt();
                                    npc.MaxLevel = parse[n + 2].ToInt();
                                    npc.AppearanceRate = parse[n + 3].ToInt();
                                    floor.Npcs.Add(npc);
                                    n+= 4;
                                }
                                
                                for (int i = 0; i < maxSpecialTiles; i++) {
                                		specialTile = new Tile(new DataManager.Maps.Tile());
                                		specialTile.Type = (Enums.TileType)parse[n].ToInt();
                                		specialTile.Data1 = parse[n + 1].ToInt();
                                		specialTile.Data2 = parse[n + 2].ToInt();
                                		specialTile.Data3 = parse[n + 3].ToInt();
                                		specialTile.String1 = parse[n + 4];
                                		specialTile.String2 = parse[n + 5];
                                		specialTile.String3 = parse[n + 6];
                                		specialTile.Ground = parse[n + 7].ToInt();
                                		specialTile.GroundSet = parse[n + 8].ToInt();
                                		//specialTile.GroundAnim = parse[n + 9].ToInt();
                                		//specialTile.GroundAnimSet = parse[n + 10].ToInt();
                                		specialTile.Mask = parse[n + 11].ToInt();
                                		specialTile.MaskSet = parse[n + 12].ToInt();
                                		specialTile.Anim	 = parse[n + 13].ToInt();
                                		specialTile.AnimSet = parse[n + 14].ToInt();
                                		specialTile.Mask2 = parse[n + 15].ToInt();
                                		specialTile.Mask2Set = parse[n + 16].ToInt();
                                		specialTile.M2Anim = parse[n + 17].ToInt();
                                		specialTile.M2AnimSet = parse[n + 18].ToInt();
                                		specialTile.Fringe = parse[n + 19].ToInt();
                                		specialTile.FringeSet = parse[n + 20].ToInt();
                                		specialTile.FAnim = parse[n + 21].ToInt();
                                		specialTile.FAnimSet = parse[n + 22].ToInt();
                                		specialTile.Fringe2 = parse[n + 23].ToInt();
                                		specialTile.Fringe2Set = parse[n + 24].ToInt();
                                		specialTile.F2Anim = parse[n + 25].ToInt();
                                		specialTile.F2AnimSet = parse[n + 26].ToInt();
                                		specialTile.RDungeonMapValue = parse[n + 27].ToInt();
                                		floor.SpecialTiles.Add(specialTile);
                                    n+= 28;
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
                writer.WriteLine("RDungeonData|V3");
                writer.WriteLine("Data|" + rdungeon.DungeonName + "|" + ((int)rdungeon.Direction).ToString() + "|" + rdungeon.MaxFloors.ToString() + "|" + rdungeon.Recruitment.ToString() + "|" + rdungeon.Exp.ToString() + "|"+ rdungeon.WindTimer.ToString() + "|");
                for (int i = 0; i < rdungeon.Floors.Count; i++) {
                	string data = "Floor|"
                		+ rdungeon.Floors[i].Options.TrapMin.ToString() + "|" + rdungeon.Floors[i].Options.TrapMax.ToString() + "|"
                		+ rdungeon.Floors[i].Options.ItemMin.ToString() + "|" + rdungeon.Floors[i].Options.ItemMax.ToString() + "|"
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
                		
                		+ rdungeon.Floors[i].Items.Count + "|" + rdungeon.Floors[i].Npcs.Count + "|" + rdungeon.Floors[i].SpecialTiles.Count + "|" + rdungeon.Floors[i].Weather.Count + "|";
                		
                		
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
                    }
                    for (int trap = 0; trap < rdungeon.Floors[i].SpecialTiles.Count; trap++) {
                        data += ((int)rdungeon.Floors[i].SpecialTiles[trap].Type).ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Data1.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Data2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Data3.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].String1 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].String2 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].String3 + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Ground.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].GroundSet.ToString() + "|";
                        data += "0" + "|";//rdungeon.Floors[i].SpecialTiles[trap].GroundAnim.ToString() + "|";
                        data += "0" + "|";//rdungeon.Floors[i].SpecialTiles[trap].GroundAnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Mask.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].MaskSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].AnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Mask2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Mask2Set.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].M2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].M2AnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Fringe.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].FringeSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].FAnim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].FAnimSet.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Fringe2.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].Fringe2Set.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].F2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].F2Anim.ToString() + "|";
                        data += rdungeon.Floors[i].SpecialTiles[trap].RDungeonMapValue.ToString() + "|";
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
