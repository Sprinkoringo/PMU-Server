using System;
using System.Collections.Generic;
using System.Text;
using Server.DataConverter.RDungeons.V3;
using Server.Maps;

namespace Server.DataConverter
{
	/// <summary>
	/// Description of RDungeonConverter.
	/// </summary>
	public class RDungeonConverter
	{
		public static void ConvertV1ToV2(int num) {
			DataConverter.RDungeons.V2.RDungeon rdungeonV2 = new Server.DataConverter.RDungeons.V2.RDungeon(num);

          DataConverter.RDungeons.V1.RDungeon rdungeonV1 = Server.DataConverter.RDungeons.V1.RDungeonManager.LoadRDungeon(num);
			
          rdungeonV2.DungeonName = rdungeonV1.DungeonName;
        rdungeonV2.Direction = rdungeonV1.Direction;
        rdungeonV2.MaxFloors = rdungeonV1.MaxFloors;
        rdungeonV2.Recruitment = rdungeonV1.Recruitment;
        rdungeonV2.Exp = rdungeonV1.Exp;
        rdungeonV2.WindTimer = 1000;
          
        for (int i = 0; i < rdungeonV1.Floors.Count; i++) {
        	DataConverter.RDungeons.V2.RDungeonFloor floor = new Server.DataConverter.RDungeons.V2.RDungeonFloor();
        	
        	//floor.Options.TrapFrequency = rdungeonV1.Options.TrapFrequency;
        	floor.Options.TrapMin = rdungeonV1.Options.TrapMin;
        	floor.Options.TrapMax = rdungeonV1.Options.TrapMax;
        	floor.Options.RoomWidthMin = rdungeonV1.Options.RoomWidthMin;
        	floor.Options.RoomWidthMax = rdungeonV1.Options.RoomWidthMax;
        	floor.Options.RoomLengthMin = rdungeonV1.Options.RoomLengthMin;
        	floor.Options.RoomLengthMax = rdungeonV1.Options.RoomLengthMax;
        	floor.Options.HallTurnMin = rdungeonV1.Options.HallTurnMin;
        	floor.Options.HallTurnMax = rdungeonV1.Options.HallTurnMax;
        	floor.Options.HallVarMin = rdungeonV1.Options.HallVarMin;
        	floor.Options.HallVarMax = rdungeonV1.Options.HallVarMax;
        	floor.Options.WaterFrequency = rdungeonV1.Options.WaterFrequency;
        	floor.Options.Craters = rdungeonV1.Options.Craters;
        	floor.Options.CraterMinLength = rdungeonV1.Options.CraterMinLength;
        	floor.Options.CraterMaxLength = rdungeonV1.Options.CraterMaxLength;
        	floor.Options.CraterFuzzy = rdungeonV1.Options.CraterFuzzy;
        	
        	floor.Darkness = -1;
        	floor.GoalType = rdungeonV1.Floors[i].GoalType;
        	floor.GoalMap = rdungeonV1.Floors[i].GoalMap;
        	floor.GoalX = rdungeonV1.Floors[i].GoalX;
        	floor.GoalY = rdungeonV1.Floors[i].GoalY;
        	floor.Music = rdungeonV1.Floors[i].Music;
        	
        	floor.StairsX = rdungeonV1.StairsX;
        floor.StairsSheet = rdungeonV1.StairsSheet;

        floor.mGroundX = rdungeonV1.mGroundX;
        floor.mGroundSheet = rdungeonV1.mGroundSheet;

        floor.mTopLeftX = rdungeonV1.mTopLeftX;
        floor.mTopLeftSheet = rdungeonV1.mTopLeftSheet;
        floor.mTopCenterX = rdungeonV1.mTopCenterX;
        floor.mTopCenterSheet = rdungeonV1.mTopCenterSheet;
        floor.mTopRightX = rdungeonV1.mTopRightX;
        floor.mTopRightSheet = rdungeonV1.mTopRightSheet;

        floor.mCenterLeftX = rdungeonV1.mCenterLeftX;
        floor.mCenterLeftSheet = rdungeonV1.mCenterLeftSheet;
        floor.mCenterCenterX = rdungeonV1.mCenterCenterX;
        floor.mCenterCenterSheet = rdungeonV1.mCenterCenterSheet;
        floor.mCenterRightX = rdungeonV1.mCenterRightX;
        floor.mCenterRightSheet = rdungeonV1.mCenterRightSheet;

        floor.mBottomLeftX = rdungeonV1.mBottomLeftX;
        floor.mBottomLeftSheet = rdungeonV1.mBottomLeftSheet;
        floor.mBottomCenterX = rdungeonV1.mBottomCenterX;
        floor.mBottomCenterSheet = rdungeonV1.mBottomCenterSheet;
        floor.mBottomRightX = rdungeonV1.mBottomRightX;
        floor.mBottomRightSheet = rdungeonV1.mBottomRightSheet;

        floor.mInnerTopLeftX = rdungeonV1.mInnerTopLeftX;
        floor.mInnerTopLeftSheet = rdungeonV1.mInnerTopLeftSheet;
        floor.mInnerBottomLeftX = rdungeonV1.mInnerBottomLeftX;
        floor.mInnerBottomLeftSheet = rdungeonV1.mInnerBottomLeftSheet;
        floor.mInnerTopRightX = rdungeonV1.mInnerTopRightX;
        floor.mInnerTopRightSheet = rdungeonV1.mInnerTopRightSheet;
        floor.mInnerBottomRightX = rdungeonV1.mInnerBottomRightX;
        floor.mInnerBottomRightSheet = rdungeonV1.mInnerBottomRightSheet;

        floor.mIsolatedWallX = rdungeonV1.mIsolatedWallX;
        floor.mIsolatedWallSheet = rdungeonV1.mIsolatedWallSheet;
        
        floor.mColumnTopX = rdungeonV1.mColumnTopX;
        floor.mColumnTopSheet = rdungeonV1.mColumnTopSheet;
        floor.mColumnCenterX = rdungeonV1.mColumnCenterX;
        floor.mColumnCenterSheet = rdungeonV1.mColumnCenterSheet;
        floor.mColumnBottomX = rdungeonV1.mColumnBottomX;
        floor.mColumnBottomSheet = rdungeonV1.mColumnBottomSheet;

        floor.mRowLeftX = rdungeonV1.mRowLeftX;
        floor.mRowLeftSheet = rdungeonV1.mRowLeftSheet;
        floor.mRowCenterX = rdungeonV1.mRowCenterX;
        floor.mRowCenterSheet = rdungeonV1.mRowCenterSheet;
        floor.mRowRightX = rdungeonV1.mRowRightX;
        floor.mRowRightSheet = rdungeonV1.mRowRightSheet;
        
        floor.mWaterX = rdungeonV1.mWaterX;
        floor.mWaterSheet = rdungeonV1.mWaterSheet;
        floor.mWaterAnimX = rdungeonV1.mWaterAnimX;
        floor.mWaterAnimSheet = rdungeonV1.mWaterAnimSheet;

        floor.mShoreTopLeftX = rdungeonV1.mShoreTopLeftX;
        floor.mShoreTopLeftSheet = rdungeonV1.mShoreTopLeftSheet;
        floor.mShoreTopRightX = rdungeonV1.mShoreTopRightX;
        floor.mShoreTopRightSheet = rdungeonV1.mShoreTopRightSheet;
        floor.mShoreBottomRightX = rdungeonV1.mShoreBottomRightX;
        floor.mShoreBottomRightSheet = rdungeonV1.mShoreBottomRightSheet;
        floor.mShoreBottomLeftX = rdungeonV1.mShoreBottomLeftX;
        floor.mShoreBottomLeftSheet = rdungeonV1.mShoreBottomLeftSheet;
        floor.mShoreDiagonalForwardX = rdungeonV1.mShoreDiagonalForwardX;
        floor.mShoreDiagonalForwardSheet = rdungeonV1.mShoreDiagonalForwardSheet;
        floor.mShoreDiagonalBackX = rdungeonV1.mShoreDiagonalBackX;
        floor.mShoreDiagonalBackSheet = rdungeonV1.mShoreDiagonalBackSheet;
        floor.mShoreTopX = rdungeonV1.mShoreTopX;
        floor.mShoreTopSheet = rdungeonV1.mShoreTopSheet;
        floor.mShoreRightX = rdungeonV1.mShoreRightX;
        floor.mShoreRightSheet = rdungeonV1.mShoreRightSheet;
        floor.mShoreBottomX = rdungeonV1.mShoreBottomX;
        floor.mShoreBottomSheet = rdungeonV1.mShoreBottomSheet;
        floor.mShoreLeftX = rdungeonV1.mShoreLeftX;
        floor.mShoreLeftSheet = rdungeonV1.mShoreLeftSheet;
        floor.mShoreVerticalX = rdungeonV1.mShoreVerticalX;
        floor.mShoreVerticalSheet = rdungeonV1.mShoreVerticalSheet;
        floor.mShoreHorizontalX = rdungeonV1.mShoreHorizontalX;
        floor.mShoreHorizontalSheet = rdungeonV1.mShoreHorizontalSheet;
        floor.mShoreInnerTopLeftX = rdungeonV1.mShoreInnerTopLeftX;
        floor.mShoreInnerTopLeftSheet = rdungeonV1.mShoreInnerTopLeftSheet;
        floor.mShoreInnerTopRightX = rdungeonV1.mShoreInnerTopRightX;
        floor.mShoreInnerTopRightSheet = rdungeonV1.mShoreInnerTopRightSheet;
        floor.mShoreInnerBottomRightX = rdungeonV1.mShoreInnerBottomRightX;
        floor.mShoreInnerBottomRightSheet = rdungeonV1.mShoreInnerBottomRightSheet;
        floor.mShoreInnerBottomLeftX = rdungeonV1.mShoreInnerBottomLeftX;
        floor.mShoreInnerBottomLeftSheet = rdungeonV1.mShoreInnerBottomLeftSheet;
        floor.mShoreInnerTopX = rdungeonV1.mShoreInnerTopX;
        floor.mShoreInnerTopSheet = rdungeonV1.mShoreInnerTopSheet;
        floor.mShoreInnerRightX = rdungeonV1.mShoreInnerRightX;
        floor.mShoreInnerRightSheet = rdungeonV1.mShoreInnerRightSheet;
        floor.mShoreInnerBottomX = rdungeonV1.mShoreInnerBottomX;
        floor.mShoreInnerBottomSheet = rdungeonV1.mShoreInnerBottomSheet;
        floor.mShoreInnerLeftX = rdungeonV1.mShoreInnerLeftX;
        floor.mShoreInnerLeftSheet = rdungeonV1.mShoreInnerLeftSheet;
        floor.mShoreSurroundedX = rdungeonV1.mShoreSurroundedX;
        floor.mShoreSurroundedSheet = rdungeonV1.mShoreSurroundedSheet;
        
        floor.ItemSpawnRate = rdungeonV1.Floors[i].ItemSpawnRate;
        for (int j = 0; j < 8; j++) {
        	floor.Items[j] = rdungeonV1.Floors[i].Items[j];
        }
        for (int j = 0; j < 15; j++) {
        	floor.Npc[j].NpcNum = rdungeonV1.Floors[i].Npc[j];
        	floor.Npc[j].MinLevel = 1;
        }
        foreach (int j in rdungeonV1.Floors[i].Traps) {
        	floor.Traps.Add(j);
        }
        
        floor.Weather.Add(rdungeonV1.Floors[i].Weather);
        	
        rdungeonV2.Floors.Add(floor);
        }
        
          
			
          Server.DataConverter.RDungeons.V2.RDungeonManager.SaveRDungeon(rdungeonV2,num);
		}
		
		public static void ConvertV2ToV3(int num) {
			DataConverter.RDungeons.V3.RDungeon rdungeonV3 = new Server.DataConverter.RDungeons.V3.RDungeon(num);

          DataConverter.RDungeons.V2.RDungeon rdungeonV2 = Server.DataConverter.RDungeons.V2.RDungeonManager.LoadRDungeon(num);
			
          rdungeonV3.DungeonName = rdungeonV2.DungeonName;
        rdungeonV3.Direction = rdungeonV2.Direction;
        rdungeonV3.MaxFloors = rdungeonV2.MaxFloors;
        rdungeonV3.Recruitment = rdungeonV2.Recruitment;
        rdungeonV3.Exp = rdungeonV2.Exp;
        rdungeonV3.WindTimer = 1000;
          
        for (int i = 0; i < rdungeonV2.Floors.Count; i++) {
        	DataConverter.RDungeons.V3.RDungeonFloor floor = new DataConverter.RDungeons.V3.RDungeonFloor();
        	
        	floor.Options.TrapMin = rdungeonV2.Floors[i].Options.TrapMin;
        	floor.Options.TrapMax = rdungeonV2.Floors[i].Options.TrapMax;
        	floor.Options.ItemMin = 5;
        	floor.Options.ItemMax = 8;
        	floor.Options.RoomWidthMin = rdungeonV2.Floors[i].Options.RoomWidthMin;
        	floor.Options.RoomWidthMax = rdungeonV2.Floors[i].Options.RoomWidthMax;
        	floor.Options.RoomLengthMin = rdungeonV2.Floors[i].Options.RoomLengthMin;
        	floor.Options.RoomLengthMax = rdungeonV2.Floors[i].Options.RoomLengthMax;
        	floor.Options.HallTurnMin = rdungeonV2.Floors[i].Options.HallTurnMin;
        	floor.Options.HallTurnMax = rdungeonV2.Floors[i].Options.HallTurnMax;
        	floor.Options.HallVarMin = rdungeonV2.Floors[i].Options.HallVarMin;
        	floor.Options.HallVarMax = rdungeonV2.Floors[i].Options.HallVarMax;
        	floor.Options.WaterFrequency = rdungeonV2.Floors[i].Options.WaterFrequency;
        	floor.Options.Craters = rdungeonV2.Floors[i].Options.Craters;
        	floor.Options.CraterMinLength = rdungeonV2.Floors[i].Options.CraterMinLength;
        	floor.Options.CraterMaxLength = rdungeonV2.Floors[i].Options.CraterMaxLength;
        	floor.Options.CraterFuzzy = rdungeonV2.Floors[i].Options.CraterFuzzy;
        	
        	floor.Darkness = -1;
        	floor.GoalType = rdungeonV2.Floors[i].GoalType;
        	floor.GoalMap = rdungeonV2.Floors[i].GoalMap;
        	floor.GoalX = rdungeonV2.Floors[i].GoalX;
        	floor.GoalY = rdungeonV2.Floors[i].GoalY;
        	floor.Music = rdungeonV2.Floors[i].Music;
        	
        	floor.StairsX = rdungeonV2.Floors[i].StairsX;
        floor.StairsSheet = rdungeonV2.Floors[i].StairsSheet;

        floor.mGroundX = rdungeonV2.Floors[i].mGroundX;
        floor.mGroundSheet = rdungeonV2.Floors[i].mGroundSheet;

        floor.mTopLeftX = rdungeonV2.Floors[i].mTopLeftX;
        floor.mTopLeftSheet = rdungeonV2.Floors[i].mTopLeftSheet;
        floor.mTopCenterX = rdungeonV2.Floors[i].mTopCenterX;
        floor.mTopCenterSheet = rdungeonV2.Floors[i].mTopCenterSheet;
        floor.mTopRightX = rdungeonV2.Floors[i].mTopRightX;
        floor.mTopRightSheet = rdungeonV2.Floors[i].mTopRightSheet;

        floor.mCenterLeftX = rdungeonV2.Floors[i].mCenterLeftX;
        floor.mCenterLeftSheet = rdungeonV2.Floors[i].mCenterLeftSheet;
        floor.mCenterCenterX = rdungeonV2.Floors[i].mCenterCenterX;
        floor.mCenterCenterSheet = rdungeonV2.Floors[i].mCenterCenterSheet;
        floor.mCenterRightX = rdungeonV2.Floors[i].mCenterRightX;
        floor.mCenterRightSheet = rdungeonV2.Floors[i].mCenterRightSheet;

        floor.mBottomLeftX = rdungeonV2.Floors[i].mBottomLeftX;
        floor.mBottomLeftSheet = rdungeonV2.Floors[i].mBottomLeftSheet;
        floor.mBottomCenterX = rdungeonV2.Floors[i].mBottomCenterX;
        floor.mBottomCenterSheet = rdungeonV2.Floors[i].mBottomCenterSheet;
        floor.mBottomRightX = rdungeonV2.Floors[i].mBottomRightX;
        floor.mBottomRightSheet = rdungeonV2.Floors[i].mBottomRightSheet;

        floor.mInnerTopLeftX = rdungeonV2.Floors[i].mInnerTopLeftX;
        floor.mInnerTopLeftSheet = rdungeonV2.Floors[i].mInnerTopLeftSheet;
        floor.mInnerBottomLeftX = rdungeonV2.Floors[i].mInnerBottomLeftX;
        floor.mInnerBottomLeftSheet = rdungeonV2.Floors[i].mInnerBottomLeftSheet;
        floor.mInnerTopRightX = rdungeonV2.Floors[i].mInnerTopRightX;
        floor.mInnerTopRightSheet = rdungeonV2.Floors[i].mInnerTopRightSheet;
        floor.mInnerBottomRightX = rdungeonV2.Floors[i].mInnerBottomRightX;
        floor.mInnerBottomRightSheet = rdungeonV2.Floors[i].mInnerBottomRightSheet;

        floor.mIsolatedWallX = rdungeonV2.Floors[i].mIsolatedWallX;
        floor.mIsolatedWallSheet = rdungeonV2.Floors[i].mIsolatedWallSheet;
        
        floor.mColumnTopX = rdungeonV2.Floors[i].mColumnTopX;
        floor.mColumnTopSheet = rdungeonV2.Floors[i].mColumnTopSheet;
        floor.mColumnCenterX = rdungeonV2.Floors[i].mColumnCenterX;
        floor.mColumnCenterSheet = rdungeonV2.Floors[i].mColumnCenterSheet;
        floor.mColumnBottomX = rdungeonV2.Floors[i].mColumnBottomX;
        floor.mColumnBottomSheet = rdungeonV2.Floors[i].mColumnBottomSheet;

        floor.mRowLeftX = rdungeonV2.Floors[i].mRowLeftX;
        floor.mRowLeftSheet = rdungeonV2.Floors[i].mRowLeftSheet;
        floor.mRowCenterX = rdungeonV2.Floors[i].mRowCenterX;
        floor.mRowCenterSheet = rdungeonV2.Floors[i].mRowCenterSheet;
        floor.mRowRightX = rdungeonV2.Floors[i].mRowRightX;
        floor.mRowRightSheet = rdungeonV2.Floors[i].mRowRightSheet;
        
        
        floor.mWaterX = rdungeonV2.Floors[i].mWaterX;
        floor.mWaterSheet = rdungeonV2.Floors[i].mWaterSheet;
        floor.mWaterAnimX = rdungeonV2.Floors[i].mWaterAnimX;
        floor.mWaterAnimSheet = rdungeonV2.Floors[i].mWaterAnimSheet;

        floor.mShoreTopLeftX = rdungeonV2.Floors[i].mShoreTopLeftX;
        floor.mShoreTopLeftSheet = rdungeonV2.Floors[i].mShoreTopLeftSheet;
        floor.mShoreTopRightX = rdungeonV2.Floors[i].mShoreTopRightX;
        floor.mShoreTopRightSheet = rdungeonV2.Floors[i].mShoreTopRightSheet;
        floor.mShoreBottomRightX = rdungeonV2.Floors[i].mShoreBottomRightX;
        floor.mShoreBottomRightSheet = rdungeonV2.Floors[i].mShoreBottomRightSheet;
        floor.mShoreBottomLeftX = rdungeonV2.Floors[i].mShoreBottomLeftX;
        floor.mShoreBottomLeftSheet = rdungeonV2.Floors[i].mShoreBottomLeftSheet;
        floor.mShoreDiagonalForwardX = rdungeonV2.Floors[i].mShoreDiagonalForwardX;
        floor.mShoreDiagonalForwardSheet = rdungeonV2.Floors[i].mShoreDiagonalForwardSheet;
        floor.mShoreDiagonalBackX = rdungeonV2.Floors[i].mShoreDiagonalBackX;
        floor.mShoreDiagonalBackSheet = rdungeonV2.Floors[i].mShoreDiagonalBackSheet;
        floor.mShoreTopX = rdungeonV2.Floors[i].mShoreTopX;
        floor.mShoreTopSheet = rdungeonV2.Floors[i].mShoreTopSheet;
        floor.mShoreRightX = rdungeonV2.Floors[i].mShoreRightX;
        floor.mShoreRightSheet = rdungeonV2.Floors[i].mShoreRightSheet;
        floor.mShoreBottomX = rdungeonV2.Floors[i].mShoreBottomX;
        floor.mShoreBottomSheet = rdungeonV2.Floors[i].mShoreBottomSheet;
        floor.mShoreLeftX = rdungeonV2.Floors[i].mShoreLeftX;
        floor.mShoreLeftSheet = rdungeonV2.Floors[i].mShoreLeftSheet;
        floor.mShoreVerticalX = rdungeonV2.Floors[i].mShoreVerticalX;
        floor.mShoreVerticalSheet = rdungeonV2.Floors[i].mShoreVerticalSheet;
        floor.mShoreHorizontalX = rdungeonV2.Floors[i].mShoreHorizontalX;
        floor.mShoreHorizontalSheet = rdungeonV2.Floors[i].mShoreHorizontalSheet;
        floor.mShoreInnerTopLeftX = rdungeonV2.Floors[i].mShoreInnerTopLeftX;
        floor.mShoreInnerTopLeftSheet = rdungeonV2.Floors[i].mShoreInnerTopLeftSheet;
        floor.mShoreInnerTopRightX = rdungeonV2.Floors[i].mShoreInnerTopRightX;
        floor.mShoreInnerTopRightSheet = rdungeonV2.Floors[i].mShoreInnerTopRightSheet;
        floor.mShoreInnerBottomRightX = rdungeonV2.Floors[i].mShoreInnerBottomRightX;
        floor.mShoreInnerBottomRightSheet = rdungeonV2.Floors[i].mShoreInnerBottomRightSheet;
        floor.mShoreInnerBottomLeftX = rdungeonV2.Floors[i].mShoreInnerBottomLeftX;
        floor.mShoreInnerBottomLeftSheet = rdungeonV2.Floors[i].mShoreInnerBottomLeftSheet;
        floor.mShoreInnerTopX = rdungeonV2.Floors[i].mShoreInnerTopX;
        floor.mShoreInnerTopSheet = rdungeonV2.Floors[i].mShoreInnerTopSheet;
        floor.mShoreInnerRightX = rdungeonV2.Floors[i].mShoreInnerRightX;
        floor.mShoreInnerRightSheet = rdungeonV2.Floors[i].mShoreInnerRightSheet;
        floor.mShoreInnerBottomX = rdungeonV2.Floors[i].mShoreInnerBottomX;
        floor.mShoreInnerBottomSheet = rdungeonV2.Floors[i].mShoreInnerBottomSheet;
        floor.mShoreInnerLeftX = rdungeonV2.Floors[i].mShoreInnerLeftX;
        floor.mShoreInnerLeftSheet = rdungeonV2.Floors[i].mShoreInnerLeftSheet;
        floor.mShoreSurroundedX = rdungeonV2.Floors[i].mShoreSurroundedX;
        floor.mShoreSurroundedSheet = rdungeonV2.Floors[i].mShoreSurroundedSheet;
        
        floor.GroundTile.Type = Enums.TileType.Walkable;
        floor.HallTile.Type = Enums.TileType.Hallway;
        floor.WaterTile.Type = Enums.TileType.MobileBlock;
        floor.WaterTile.Data1 = 2;
        floor.WallTile.Type = Enums.TileType.MobileBlock;
        floor.WallTile.Data1 = 16;
        
        floor.NpcSpawnTime = 10;
        floor.NpcMin = 5;
        floor.NpcMax = 8;
        
        RDungeonItem item;
        RDungeonNpc npc;
        Tile specialTile;
        
        for (int j = 0; j < 16; j++) {
        	if (rdungeonV2.Floors[i].Items[j] > 0) {
        		item = new RDungeonItem();
        		item.ItemNum = rdungeonV2.Floors[i].Items[j];
        		item.MaxAmount = rdungeonV2.Floors[i].ItemSpawnRate;
        		item.OnGround = true;
        		item.AppearanceRate = 100;
        		floor.Items.Add(item);
        	}
        	
        }
        for (int j = 0; j < 15; j++) {
        	if (rdungeonV2.Floors[i].Npc[j].NpcNum > 0) {
        		npc = new RDungeonNpc();
        		npc.NpcNum = rdungeonV2.Floors[i].Npc[j].NpcNum;
        		npc.MinLevel = rdungeonV2.Floors[i].Npc[j].MinLevel;
        		npc.MaxLevel = rdungeonV2.Floors[i].Npc[j].MinLevel;
        		npc.AppearanceRate = 100;
        		floor.Npcs.Add(npc);
        	}
        	
        }
        foreach (int j in rdungeonV2.Floors[i].Traps) {
        	specialTile = new Tile(new DataManager.Maps.Tile());
        	specialTile.Type = Enums.TileType.Scripted;
        	specialTile.Ground = rdungeonV2.Floors[i].mGroundX;
        	specialTile.GroundSet = rdungeonV2.Floors[i].mGroundSheet;
        	specialTile.Data1 = TrapNumToScript(j);
        	floor.SpecialTiles.Add(specialTile);
        }
        
        foreach (Enums.Weather j in rdungeonV2.Floors[i].Weather) {
        	floor.Weather.Add(j);
        }
        
        	
        rdungeonV3.Floors.Add(floor);
        }
        
          
			
          Server.DataConverter.RDungeons.V3.RDungeonManager.SaveRDungeon(rdungeonV3,num);
		}

        public static void ConvertV3ToV4(int num)
        {
            DataConverter.RDungeons.V4.RDungeon rdungeonV4 = new Server.DataConverter.RDungeons.V4.RDungeon(num);

            DataConverter.RDungeons.V3.RDungeon rdungeonV3 = Server.DataConverter.RDungeons.V3.RDungeonManager.LoadRDungeon(num);

            rdungeonV4.DungeonName = rdungeonV3.DungeonName;
            rdungeonV4.Direction = rdungeonV3.Direction;
            rdungeonV4.MaxFloors = rdungeonV3.MaxFloors;
            rdungeonV4.Recruitment = rdungeonV3.Recruitment;
            rdungeonV4.Exp = rdungeonV3.Exp;
            rdungeonV4.WindTimer = rdungeonV3.WindTimer;
            rdungeonV4.DungeonIndex = 0;

            for (int i = 0; i < rdungeonV3.Floors.Count; i++)
            {
                DataConverter.RDungeons.V4.RDungeonFloor floor = new DataConverter.RDungeons.V4.RDungeonFloor();

                floor.Options.TrapMin = rdungeonV3.Floors[i].Options.TrapMin;
                floor.Options.TrapMax = rdungeonV3.Floors[i].Options.TrapMax;
                floor.Options.ItemMin = 5;
                floor.Options.ItemMax = 8;
                floor.Options.Intricacy = 6;
                floor.Options.RoomWidthMin = rdungeonV3.Floors[i].Options.RoomWidthMin;
                floor.Options.RoomWidthMax = rdungeonV3.Floors[i].Options.RoomWidthMax;
                floor.Options.RoomLengthMin = rdungeonV3.Floors[i].Options.RoomLengthMin;
                floor.Options.RoomLengthMax = rdungeonV3.Floors[i].Options.RoomLengthMax;
                floor.Options.HallTurnMin = rdungeonV3.Floors[i].Options.HallTurnMin;
                floor.Options.HallTurnMax = rdungeonV3.Floors[i].Options.HallTurnMax;
                floor.Options.HallVarMin = rdungeonV3.Floors[i].Options.HallVarMin;
                floor.Options.HallVarMax = rdungeonV3.Floors[i].Options.HallVarMax;
                floor.Options.WaterFrequency = rdungeonV3.Floors[i].Options.WaterFrequency;
                floor.Options.Craters = rdungeonV3.Floors[i].Options.Craters;
                floor.Options.CraterMinLength = rdungeonV3.Floors[i].Options.CraterMinLength;
                floor.Options.CraterMaxLength = rdungeonV3.Floors[i].Options.CraterMaxLength;
                floor.Options.CraterFuzzy = rdungeonV3.Floors[i].Options.CraterFuzzy;
                floor.Options.MinChambers = 0;
                floor.Options.MaxChambers = 0;

                floor.Darkness = -1;
                floor.GoalType = rdungeonV3.Floors[i].GoalType;
                floor.GoalMap = rdungeonV3.Floors[i].GoalMap;
                floor.GoalX = rdungeonV3.Floors[i].GoalX;
                floor.GoalY = rdungeonV3.Floors[i].GoalY;
                floor.Music = rdungeonV3.Floors[i].Music;

                floor.StairsX = rdungeonV3.Floors[i].StairsX;
                floor.StairsSheet = rdungeonV3.Floors[i].StairsSheet;

                floor.mGroundX = rdungeonV3.Floors[i].mGroundX;
                floor.mGroundSheet = rdungeonV3.Floors[i].mGroundSheet;

                floor.mTopLeftX = rdungeonV3.Floors[i].mTopLeftX;
                floor.mTopLeftSheet = rdungeonV3.Floors[i].mTopLeftSheet;
                floor.mTopCenterX = rdungeonV3.Floors[i].mTopCenterX;
                floor.mTopCenterSheet = rdungeonV3.Floors[i].mTopCenterSheet;
                floor.mTopRightX = rdungeonV3.Floors[i].mTopRightX;
                floor.mTopRightSheet = rdungeonV3.Floors[i].mTopRightSheet;

                floor.mCenterLeftX = rdungeonV3.Floors[i].mCenterLeftX;
                floor.mCenterLeftSheet = rdungeonV3.Floors[i].mCenterLeftSheet;
                floor.mCenterCenterX = rdungeonV3.Floors[i].mCenterCenterX;
                floor.mCenterCenterSheet = rdungeonV3.Floors[i].mCenterCenterSheet;
                floor.mCenterRightX = rdungeonV3.Floors[i].mCenterRightX;
                floor.mCenterRightSheet = rdungeonV3.Floors[i].mCenterRightSheet;

                floor.mBottomLeftX = rdungeonV3.Floors[i].mBottomLeftX;
                floor.mBottomLeftSheet = rdungeonV3.Floors[i].mBottomLeftSheet;
                floor.mBottomCenterX = rdungeonV3.Floors[i].mBottomCenterX;
                floor.mBottomCenterSheet = rdungeonV3.Floors[i].mBottomCenterSheet;
                floor.mBottomRightX = rdungeonV3.Floors[i].mBottomRightX;
                floor.mBottomRightSheet = rdungeonV3.Floors[i].mBottomRightSheet;

                floor.mInnerTopLeftX = rdungeonV3.Floors[i].mInnerTopLeftX;
                floor.mInnerTopLeftSheet = rdungeonV3.Floors[i].mInnerTopLeftSheet;
                floor.mInnerBottomLeftX = rdungeonV3.Floors[i].mInnerBottomLeftX;
                floor.mInnerBottomLeftSheet = rdungeonV3.Floors[i].mInnerBottomLeftSheet;
                floor.mInnerTopRightX = rdungeonV3.Floors[i].mInnerTopRightX;
                floor.mInnerTopRightSheet = rdungeonV3.Floors[i].mInnerTopRightSheet;
                floor.mInnerBottomRightX = rdungeonV3.Floors[i].mInnerBottomRightX;
                floor.mInnerBottomRightSheet = rdungeonV3.Floors[i].mInnerBottomRightSheet;

                floor.mIsolatedWallX = rdungeonV3.Floors[i].mIsolatedWallX;
                floor.mIsolatedWallSheet = rdungeonV3.Floors[i].mIsolatedWallSheet;

                floor.mColumnTopX = rdungeonV3.Floors[i].mColumnTopX;
                floor.mColumnTopSheet = rdungeonV3.Floors[i].mColumnTopSheet;
                floor.mColumnCenterX = rdungeonV3.Floors[i].mColumnCenterX;
                floor.mColumnCenterSheet = rdungeonV3.Floors[i].mColumnCenterSheet;
                floor.mColumnBottomX = rdungeonV3.Floors[i].mColumnBottomX;
                floor.mColumnBottomSheet = rdungeonV3.Floors[i].mColumnBottomSheet;

                floor.mRowLeftX = rdungeonV3.Floors[i].mRowLeftX;
                floor.mRowLeftSheet = rdungeonV3.Floors[i].mRowLeftSheet;
                floor.mRowCenterX = rdungeonV3.Floors[i].mRowCenterX;
                floor.mRowCenterSheet = rdungeonV3.Floors[i].mRowCenterSheet;
                floor.mRowRightX = rdungeonV3.Floors[i].mRowRightX;
                floor.mRowRightSheet = rdungeonV3.Floors[i].mRowRightSheet;

                floor.mGroundAltX= rdungeonV3.Floors[i].mGroundAltX;
                floor.mGroundAltSheet = rdungeonV3.Floors[i].mGroundAltSheet;
                floor.mGroundAlt2X = rdungeonV3.Floors[i].mGroundAlt2X;
                floor.mGroundAlt2Sheet = rdungeonV3.Floors[i].mGroundAlt2Sheet;

                floor.mTopLeftAltX= rdungeonV3.Floors[i].mTopLeftAltX;
                floor.mTopLeftAltSheet = rdungeonV3.Floors[i].mTopLeftAltSheet;
                floor.mTopCenterAltX= rdungeonV3.Floors[i].mTopCenterAltX;
                floor.mTopCenterAltSheet = rdungeonV3.Floors[i].mTopCenterAltSheet;
                floor.mTopRightAltX= rdungeonV3.Floors[i].mTopRightAltX;
                floor.mTopRightAltSheet = rdungeonV3.Floors[i].mTopRightAltSheet;

                floor.mCenterLeftAltX= rdungeonV3.Floors[i].mCenterLeftAltX;
                floor.mCenterLeftAltSheet = rdungeonV3.Floors[i].mCenterLeftAltSheet;
                floor.mCenterCenterAltX= rdungeonV3.Floors[i].mCenterCenterAltX;
                floor.mCenterCenterAltSheet = rdungeonV3.Floors[i].mCenterCenterAltSheet;
                floor.mCenterCenterAlt2X = rdungeonV3.Floors[i].mCenterCenterAlt2X;
                floor.mCenterCenterAlt2Sheet = rdungeonV3.Floors[i].mCenterCenterAlt2Sheet;
                floor.mCenterRightAltX= rdungeonV3.Floors[i].mCenterRightAltX;
                floor.mCenterRightAltSheet = rdungeonV3.Floors[i].mCenterRightAltSheet;

                floor.mBottomLeftAltX= rdungeonV3.Floors[i].mBottomLeftAltX;
                floor.mBottomLeftAltSheet = rdungeonV3.Floors[i].mBottomLeftAltSheet;
                floor.mBottomCenterAltX= rdungeonV3.Floors[i].mBottomCenterAltX;
                floor.mBottomCenterAltSheet = rdungeonV3.Floors[i].mBottomCenterAltSheet;
                floor.mBottomRightAltX= rdungeonV3.Floors[i].mBottomRightAltX;
                floor.mBottomRightAltSheet = rdungeonV3.Floors[i].mBottomRightAltSheet;

                floor.mInnerTopLeftAltX= rdungeonV3.Floors[i].mInnerTopLeftAltX;
                floor.mInnerTopLeftAltSheet = rdungeonV3.Floors[i].mInnerTopLeftAltSheet;
                floor.mInnerBottomLeftAltX= rdungeonV3.Floors[i].mInnerBottomLeftAltX;
                floor.mInnerBottomLeftAltSheet = rdungeonV3.Floors[i].mInnerBottomLeftAltSheet;
                floor.mInnerTopRightAltX= rdungeonV3.Floors[i].mInnerTopRightAltX;
                floor.mInnerTopRightAltSheet = rdungeonV3.Floors[i].mInnerTopRightAltSheet;
                floor.mInnerBottomRightAltX= rdungeonV3.Floors[i].mInnerBottomRightAltX;
                floor.mInnerBottomRightAltSheet = rdungeonV3.Floors[i].mInnerBottomRightAltSheet;

                floor.mIsolatedWallAltX= rdungeonV3.Floors[i].mIsolatedWallAltX;
                floor.mIsolatedWallAltSheet = rdungeonV3.Floors[i].mIsolatedWallAltSheet;

                floor.mColumnTopAltX= rdungeonV3.Floors[i].mColumnTopAltX;
                floor.mColumnTopAltSheet = rdungeonV3.Floors[i].mColumnTopAltSheet;
                floor.mColumnCenterAltX= rdungeonV3.Floors[i].mColumnCenterAltX;
                floor.mColumnCenterAltSheet = rdungeonV3.Floors[i].mColumnCenterAltSheet;
                floor.mColumnBottomAltX= rdungeonV3.Floors[i].mColumnBottomAltX;
                floor.mColumnBottomAltSheet = rdungeonV3.Floors[i].mColumnBottomAltSheet;

                floor.mRowLeftAltX= rdungeonV3.Floors[i].mRowLeftAltX;
                floor.mRowLeftAltSheet = rdungeonV3.Floors[i].mRowLeftAltSheet;
                floor.mRowCenterAltX= rdungeonV3.Floors[i].mRowCenterAltX;
                floor.mRowCenterAltSheet = rdungeonV3.Floors[i].mRowCenterAltSheet;
                floor.mRowRightAltX= rdungeonV3.Floors[i].mRowRightAltX;
                floor.mRowRightAltSheet = rdungeonV3.Floors[i].mRowRightAltSheet;


                floor.mWaterX = rdungeonV3.Floors[i].mWaterX;
                floor.mWaterSheet = rdungeonV3.Floors[i].mWaterSheet;
                floor.mWaterAnimX = rdungeonV3.Floors[i].mWaterAnimX;
                floor.mWaterAnimSheet = rdungeonV3.Floors[i].mWaterAnimSheet;

                floor.mShoreTopLeftX = rdungeonV3.Floors[i].mShoreTopLeftX;
                floor.mShoreTopLeftSheet = rdungeonV3.Floors[i].mShoreTopLeftSheet;
                floor.mShoreTopRightX = rdungeonV3.Floors[i].mShoreTopRightX;
                floor.mShoreTopRightSheet = rdungeonV3.Floors[i].mShoreTopRightSheet;
                floor.mShoreBottomRightX = rdungeonV3.Floors[i].mShoreBottomRightX;
                floor.mShoreBottomRightSheet = rdungeonV3.Floors[i].mShoreBottomRightSheet;
                floor.mShoreBottomLeftX = rdungeonV3.Floors[i].mShoreBottomLeftX;
                floor.mShoreBottomLeftSheet = rdungeonV3.Floors[i].mShoreBottomLeftSheet;
                floor.mShoreDiagonalForwardX = rdungeonV3.Floors[i].mShoreDiagonalForwardX;
                floor.mShoreDiagonalForwardSheet = rdungeonV3.Floors[i].mShoreDiagonalForwardSheet;
                floor.mShoreDiagonalBackX = rdungeonV3.Floors[i].mShoreDiagonalBackX;
                floor.mShoreDiagonalBackSheet = rdungeonV3.Floors[i].mShoreDiagonalBackSheet;
                floor.mShoreTopX = rdungeonV3.Floors[i].mShoreTopX;
                floor.mShoreTopSheet = rdungeonV3.Floors[i].mShoreTopSheet;
                floor.mShoreRightX = rdungeonV3.Floors[i].mShoreRightX;
                floor.mShoreRightSheet = rdungeonV3.Floors[i].mShoreRightSheet;
                floor.mShoreBottomX = rdungeonV3.Floors[i].mShoreBottomX;
                floor.mShoreBottomSheet = rdungeonV3.Floors[i].mShoreBottomSheet;
                floor.mShoreLeftX = rdungeonV3.Floors[i].mShoreLeftX;
                floor.mShoreLeftSheet = rdungeonV3.Floors[i].mShoreLeftSheet;
                floor.mShoreVerticalX = rdungeonV3.Floors[i].mShoreVerticalX;
                floor.mShoreVerticalSheet = rdungeonV3.Floors[i].mShoreVerticalSheet;
                floor.mShoreHorizontalX = rdungeonV3.Floors[i].mShoreHorizontalX;
                floor.mShoreHorizontalSheet = rdungeonV3.Floors[i].mShoreHorizontalSheet;
                floor.mShoreInnerTopLeftX = rdungeonV3.Floors[i].mShoreInnerTopLeftX;
                floor.mShoreInnerTopLeftSheet = rdungeonV3.Floors[i].mShoreInnerTopLeftSheet;
                floor.mShoreInnerTopRightX = rdungeonV3.Floors[i].mShoreInnerTopRightX;
                floor.mShoreInnerTopRightSheet = rdungeonV3.Floors[i].mShoreInnerTopRightSheet;
                floor.mShoreInnerBottomRightX = rdungeonV3.Floors[i].mShoreInnerBottomRightX;
                floor.mShoreInnerBottomRightSheet = rdungeonV3.Floors[i].mShoreInnerBottomRightSheet;
                floor.mShoreInnerBottomLeftX = rdungeonV3.Floors[i].mShoreInnerBottomLeftX;
                floor.mShoreInnerBottomLeftSheet = rdungeonV3.Floors[i].mShoreInnerBottomLeftSheet;
                floor.mShoreInnerTopX = rdungeonV3.Floors[i].mShoreInnerTopX;
                floor.mShoreInnerTopSheet = rdungeonV3.Floors[i].mShoreInnerTopSheet;
                floor.mShoreInnerRightX = rdungeonV3.Floors[i].mShoreInnerRightX;
                floor.mShoreInnerRightSheet = rdungeonV3.Floors[i].mShoreInnerRightSheet;
                floor.mShoreInnerBottomX = rdungeonV3.Floors[i].mShoreInnerBottomX;
                floor.mShoreInnerBottomSheet = rdungeonV3.Floors[i].mShoreInnerBottomSheet;
                floor.mShoreInnerLeftX = rdungeonV3.Floors[i].mShoreInnerLeftX;
                floor.mShoreInnerLeftSheet = rdungeonV3.Floors[i].mShoreInnerLeftSheet;
                floor.mShoreSurroundedX = rdungeonV3.Floors[i].mShoreSurroundedX;
                floor.mShoreSurroundedSheet = rdungeonV3.Floors[i].mShoreSurroundedSheet;

                floor.mShoreTopLeftAnimX = rdungeonV3.Floors[i].mShoreTopLeftAnimX;
                floor.mShoreTopLeftAnimSheet = rdungeonV3.Floors[i].mShoreTopLeftAnimSheet;
                floor.mShoreTopRightAnimX = rdungeonV3.Floors[i].mShoreTopRightAnimX;
                floor.mShoreTopRightAnimSheet = rdungeonV3.Floors[i].mShoreTopRightAnimSheet;
                floor.mShoreBottomRightAnimX = rdungeonV3.Floors[i].mShoreBottomRightAnimX;
                floor.mShoreBottomRightAnimSheet = rdungeonV3.Floors[i].mShoreBottomRightAnimSheet;
                floor.mShoreBottomLeftAnimX = rdungeonV3.Floors[i].mShoreBottomLeftAnimX;
                floor.mShoreBottomLeftAnimSheet = rdungeonV3.Floors[i].mShoreBottomLeftAnimSheet;
                floor.mShoreDiagonalForwardAnimX = rdungeonV3.Floors[i].mShoreDiagonalForwardAnimX;
                floor.mShoreDiagonalForwardAnimSheet = rdungeonV3.Floors[i].mShoreDiagonalForwardAnimSheet;
                floor.mShoreDiagonalBackAnimX = rdungeonV3.Floors[i].mShoreDiagonalBackAnimX;
                floor.mShoreDiagonalBackAnimSheet = rdungeonV3.Floors[i].mShoreDiagonalBackAnimSheet;
                floor.mShoreTopAnimX = rdungeonV3.Floors[i].mShoreTopAnimX;
                floor.mShoreTopAnimSheet = rdungeonV3.Floors[i].mShoreTopAnimSheet;
                floor.mShoreRightAnimX = rdungeonV3.Floors[i].mShoreRightAnimX;
                floor.mShoreRightAnimSheet = rdungeonV3.Floors[i].mShoreRightAnimSheet;
                floor.mShoreBottomAnimX = rdungeonV3.Floors[i].mShoreBottomAnimX;
                floor.mShoreBottomAnimSheet = rdungeonV3.Floors[i].mShoreBottomAnimSheet;
                floor.mShoreLeftAnimX = rdungeonV3.Floors[i].mShoreLeftAnimX;
                floor.mShoreLeftAnimSheet = rdungeonV3.Floors[i].mShoreLeftAnimSheet;
                floor.mShoreVerticalAnimX = rdungeonV3.Floors[i].mShoreVerticalAnimX;
                floor.mShoreVerticalAnimSheet = rdungeonV3.Floors[i].mShoreVerticalAnimSheet;
                floor.mShoreHorizontalAnimX = rdungeonV3.Floors[i].mShoreHorizontalAnimX;
                floor.mShoreHorizontalAnimSheet = rdungeonV3.Floors[i].mShoreHorizontalAnimSheet;
                floor.mShoreInnerTopLeftAnimX = rdungeonV3.Floors[i].mShoreInnerTopLeftAnimX;
                floor.mShoreInnerTopLeftAnimSheet = rdungeonV3.Floors[i].mShoreInnerTopLeftAnimSheet;
                floor.mShoreInnerTopRightAnimX = rdungeonV3.Floors[i].mShoreInnerTopRightAnimX;
                floor.mShoreInnerTopRightAnimSheet = rdungeonV3.Floors[i].mShoreInnerTopRightAnimSheet;
                floor.mShoreInnerBottomRightAnimX = rdungeonV3.Floors[i].mShoreInnerBottomRightAnimX;
                floor.mShoreInnerBottomRightAnimSheet = rdungeonV3.Floors[i].mShoreInnerBottomRightAnimSheet;
                floor.mShoreInnerBottomLeftAnimX = rdungeonV3.Floors[i].mShoreInnerBottomLeftAnimX;
                floor.mShoreInnerBottomLeftAnimSheet = rdungeonV3.Floors[i].mShoreInnerBottomLeftAnimSheet;
                floor.mShoreInnerTopAnimX = rdungeonV3.Floors[i].mShoreInnerTopAnimX;
                floor.mShoreInnerTopAnimSheet = rdungeonV3.Floors[i].mShoreInnerTopAnimSheet;
                floor.mShoreInnerRightAnimX = rdungeonV3.Floors[i].mShoreInnerRightAnimX;
                floor.mShoreInnerRightAnimSheet = rdungeonV3.Floors[i].mShoreInnerRightAnimSheet;
                floor.mShoreInnerBottomAnimX = rdungeonV3.Floors[i].mShoreInnerBottomAnimX;
                floor.mShoreInnerBottomAnimSheet = rdungeonV3.Floors[i].mShoreInnerBottomAnimSheet;
                floor.mShoreInnerLeftAnimX = rdungeonV3.Floors[i].mShoreInnerLeftAnimX;
                floor.mShoreInnerLeftAnimSheet = rdungeonV3.Floors[i].mShoreInnerLeftAnimSheet;
                floor.mShoreSurroundedAnimX = rdungeonV3.Floors[i].mShoreSurroundedAnimX;
                floor.mShoreSurroundedAnimSheet = rdungeonV3.Floors[i].mShoreSurroundedAnimSheet;

                floor.GroundTile = rdungeonV3.Floors[i].GroundTile;
                floor.HallTile = rdungeonV3.Floors[i].HallTile;
                floor.WaterTile = rdungeonV3.Floors[i].WaterTile;
                floor.WallTile = rdungeonV3.Floors[i].WallTile;

                floor.NpcSpawnTime = rdungeonV3.Floors[i].NpcSpawnTime;
                floor.NpcMin = rdungeonV3.Floors[i].NpcMin;
                floor.NpcMax = rdungeonV3.Floors[i].NpcMax;

                RDungeons.V4.RDungeonItem item;
                MapNpcPreset npc;
                RDungeons.V4.RDungeonTrap specialTile;

                for (int j = 0; j < rdungeonV3.Floors[i].Items.Count; j++)
                {
                    if (rdungeonV3.Floors[i].Items[j].ItemNum > 0)
                    {
                        item = new RDungeons.V4.RDungeonItem();
                        item.ItemNum = rdungeonV3.Floors[i].Items[j].ItemNum;
                        item.MinAmount = rdungeonV3.Floors[i].Items[j].MinAmount;
                        item.MaxAmount = rdungeonV3.Floors[i].Items[j].MaxAmount;
                        item.AppearanceRate = rdungeonV3.Floors[i].Items[j].AppearanceRate;
                        item.StickyRate = rdungeonV3.Floors[i].Items[j].StickyRate;
                        item.Tag = rdungeonV3.Floors[i].Items[j].Tag;
                        item.Hidden = rdungeonV3.Floors[i].Items[j].Hidden;
                        item.OnGround = rdungeonV3.Floors[i].Items[j].OnGround;
                        item.OnWater = rdungeonV3.Floors[i].Items[j].OnWater;
                        item.OnWall = rdungeonV3.Floors[i].Items[j].OnWall;
                        floor.Items.Add(item);
                    }

                }
                for (int j = 0; j < rdungeonV3.Floors[i].Npcs.Count; j++)
                {
                    if (rdungeonV3.Floors[i].Npcs[j].NpcNum > 0)
                    {
                        npc = new MapNpcPreset();
                        npc.NpcNum = rdungeonV3.Floors[i].Npcs[j].NpcNum;
                        npc.MinLevel = rdungeonV3.Floors[i].Npcs[j].MinLevel;
                        npc.MaxLevel = rdungeonV3.Floors[i].Npcs[j].MinLevel;
                        npc.AppearanceRate = rdungeonV3.Floors[i].Npcs[j].AppearanceRate;
                        floor.Npcs.Add(npc);
                    }

                }
                foreach (Tile j in rdungeonV3.Floors[i].SpecialTiles)
                {
                    specialTile = new RDungeons.V4.RDungeonTrap();
                    specialTile.SpecialTile = j;
                    specialTile.AppearanceRate = 100;
                    floor.SpecialTiles.Add(specialTile);
                }

                foreach (Enums.Weather j in rdungeonV3.Floors[i].Weather)
                {
                    floor.Weather.Add(j);
                }


                rdungeonV4.Floors.Add(floor);
            }



            Server.DataConverter.RDungeons.V4.RDungeonManager.SaveRDungeon(rdungeonV4, num);
        }
		
		private static int TrapNumToScript(int id) {
            switch (id) {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 4;
                case 3:
                    return 5;
                case 4:
                    return 6;
                case 5:
                    return 7;
                case 6:
                    return 23;
                case 7:
                    return 25;
                case 8:
                    return 28;
                case 9:
                    return 49;
                case 10:
                    return 53;
                case 11:
                    return 50;
                case 12:
                    return 51;
                case 13:
                    return 52;
                case 14:
                    return 42;
                case 15:
                    return 39;
                case 16:
                    return 43;
                case 17:
                    return 44;
                case 18:
                    return 26;
                default:
                    return 0;
            }
        }
		
	}
}
