using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;
using Server.Network;

namespace Server.RDungeons
{
    public class RDungeonFloorGen
    {
        public static RDungeonMap GenerateFloor(Client client, int dungeonIndex, int floorNum, GeneratorOptions options) {
            RDungeon dungeon = RDungeonManager.RDungeons[dungeonIndex];
            RDungeonFloor floor = dungeon.Floors[floorNum];

            //decide on a chamber
            RDungeonChamberReq req = null;
            int chamber = -1;
            if (options.Chambers.Count > 0) {
                chamber = Math.Rand(0, options.Chambers.Count);
                req = (RDungeonChamberReq)Scripting.ScriptManager.InvokeFunction("GetChamberReq", options.Chambers[chamber].ChamberNum, options.Chambers[chamber].String1, options.Chambers[chamber].String2, options.Chambers[chamber].String3);
            }

            // Generate the ASCII map

            DungeonArrayFloor arrayFloor = new DungeonArrayFloor(options, req);
            //int[,] arrayMap = ASCIIFloorGen.GenASCIIMap(options);

            //ASCIIFloorGen.TextureDungeon(arrayMap);
            

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();



            // Prepare the base map
            DataManager.Maps.RDungeonMap rDungeonMap = new DataManager.Maps.RDungeonMap(MapManager.GenerateMapID(RDungeonMap.ID_PREFIX));
            rDungeonMap.MaxX = arrayFloor.MapArray.GetUpperBound(0);
            rDungeonMap.MaxY = arrayFloor.MapArray.GetUpperBound(1);
            rDungeonMap.Tile = new DataManager.Maps.Tile[rDungeonMap.MaxX + 1, rDungeonMap.MaxY + 1];
            rDungeonMap.RDungeonIndex = dungeonIndex;
            rDungeonMap.RDungeonFloor = floorNum;

            RDungeonMap map = new RDungeonMap(rDungeonMap);

            for (int x = 0; x <= rDungeonMap.Tile.GetUpperBound(0); x++) {
                for (int y = 0; y <= rDungeonMap.Tile.GetUpperBound(1); y++) {
                    AmbiguateTile(map.Tile[x,y]);
                }
            }

            map.Name = GenerateName(dungeonIndex, floorNum);

            map.Revision = 0;
            map.OriginalDarkness = floor.Darkness;
            map.HungerEnabled = true;
            map.RecruitEnabled = dungeon.Recruitment;
            map.ExpEnabled = dungeon.Exp;
            map.TimeLimit = dungeon.WindTimer;
            map.DungeonIndex = dungeon.DungeonIndex;
            map.MinNpcs = floor.NpcMin;
            map.MaxNpcs = floor.NpcMax;
            map.NpcSpawnTime = floor.NpcSpawnTime;

            

            if (Globals.ServerWeather != Enums.Weather.Ambiguous) {
                map.Weather = Globals.ServerWeather;
            } else if (floor.Weather.Count > 0) {
                map.Weather = floor.Weather[Server.Math.Rand(0, floor.Weather.Count)];
            }
            
            map.Music = floor.Music;

            //texture chamber
            if (arrayFloor.Chamber.X != -1 && arrayFloor.Chamber.Y != -1) {
                Scripting.ScriptManager.InvokeSub("CreateChamber", map, arrayFloor, options.Chambers[chamber].ChamberNum, options.Chambers[chamber].String1, options.Chambers[chamber].String2, options.Chambers[chamber].String3);
            }


            DungeonArrayFloor.TextureDungeon(arrayFloor.MapArray);


            for (int x = 0; x <= rDungeonMap.Tile.GetUpperBound(0); x++) {
                for (int y = 0; y <= rDungeonMap.Tile.GetUpperBound(1); y++) {
                    map.Tile[x, y].RDungeonMapValue = arrayFloor.MapArray[x, y];
                }
            }

            TextureDungeonMap(map);


            //block borders
            for (int y = 0; y <= map.MaxY; y++) {
                map.Tile[0, y].Type = Enums.TileType.Blocked;
                map.Tile[map.MaxX, y].Type = Enums.TileType.Blocked;
            }
            for (int x = 0; x <= map.MaxX; x++) {
                map.Tile[x, 0].Type = Enums.TileType.Blocked;
                map.Tile[x, map.MaxY].Type = Enums.TileType.Blocked;
            }

            watch.Stop();

            foreach (MapNpcPreset npc in floor.Npcs) {
                MapNpcPreset newNpc = new MapNpcPreset();
                newNpc.NpcNum = npc.NpcNum;
                newNpc.MinLevel = npc.MinLevel;
                newNpc.MaxLevel = npc.MaxLevel;
                newNpc.SpawnX = npc.SpawnX;
                newNpc.SpawnY = npc.SpawnY;
                newNpc.AppearanceRate = npc.AppearanceRate;
                newNpc.StartStatus = npc.StartStatus;
                newNpc.StartStatusCounter = npc.StartStatusCounter;
                newNpc.StartStatusChance = npc.StartStatusChance;
                map.Npc.Add(newNpc);
            }

            map.SpawnItems();
            int n = Server.Math.Rand(options.ItemMin, options.ItemMax + 1);
            if (floor.Items.Count == 0) {
                n = 0;
            } else if (n > Constants.MAX_MAP_ITEMS) {
                n = Constants.MAX_MAP_ITEMS;
            }
            int slot = 0;
            int itemX = -1, itemY = -1;
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < 100; j++) {
                    bool spawned = false;
                    if (Server.Math.Rand(0, 100) < floor.Items[slot].AppearanceRate) {
                        arrayFloor.GenItem(floor.Items[slot].OnGround, floor.Items[slot].OnWater, floor.Items[slot].OnWall, ref itemX, ref itemY);
                        if (itemX > -1) {
                            map.SpawnItem(floor.Items[slot].ItemNum, Server.Math.Rand(floor.Items[slot].MinAmount, floor.Items[slot].MaxAmount + 1),
                                          (Server.Math.Rand(0, 100) < floor.Items[slot].StickyRate), floor.Items[slot].Hidden, floor.Items[slot].Tag, itemX, itemY, null);
                            spawned = true;
                        }

                    }
                    slot++;
                    if (slot >= floor.Items.Count) slot = 0;
                    if (spawned) break;
                }
            }




            //map.SpawnNpcs();
            map.NpcSpawnWait = new TickCount(Core.GetTickCount().Tick);

            return map;
        }

        public static string GenerateName(int dungeonIndex, int floorNum) {
            RDungeon dungeon = RDungeonManager.RDungeons[dungeonIndex];
            if (dungeon.Direction == Enums.Direction.Up) {
                return dungeon.DungeonName + " " + (floorNum + 1).ToString() + "F";
            } else {
                return dungeon.DungeonName + " B" + (floorNum + 1).ToString() + "F";
            }
        }

        public static void AmbiguateTile(Tile tile) {
            tile.Ground = 0;
            tile.GroundSet = 10;
            tile.GroundAnim = 0;
            tile.GroundAnimSet = 10;
            tile.Mask = 0;
            tile.MaskSet = 10;
            tile.Anim = 0;
            tile.AnimSet = 10;
            tile.Mask2 = 0;
            tile.Mask2Set = 10;
            tile.M2Anim = 0;
            tile.M2AnimSet = 10;
            tile.Fringe = 0;
            tile.FringeSet = 10;
            tile.FAnim = 0;
            tile.FAnimSet = 10;
            tile.Fringe2 = 0;
            tile.Fringe2Set = 10;
            tile.F2Anim = 0;
            tile.F2AnimSet = 10;

            tile.Type = Enums.TileType.Ambiguous;
            tile.Data1 = 0;
            tile.Data2 = 0;
            tile.Data3 = 0;
            tile.String1 = "";
            tile.String2 = "";
            tile.String3 = "";
        }

        public static void TextureDungeonMap(RDungeonMap map) {

            TextureDungeonMap(map, 0, 0, map.MaxX, map.MaxY);

        }

        public static void TextureDungeonMap(RDungeonMap map, int startX, int startY, int endX, int endY) {

            RDungeonFloor floor = RDungeonManager.RDungeons[map.RDungeonIndex].Floors[map.RDungeonFloor];
            int trapSpawnMarker = 0;

            for (int y = startY; y <= endY; y++) {
                for (int x = startX; x <= endX; x++) {

                    int value = map.Tile[x, y].RDungeonMapValue % 1024;


                    switch (value) {
                        case 4:
                        case 5: {//hall and door
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.HallTile.Type;
                                    tile.Data1 = floor.HallTile.Data1;
                                    tile.Data2 = floor.HallTile.Data2;
                                    tile.Data3 = floor.HallTile.Data3;
                                    tile.String1 = floor.HallTile.String1;
                                    tile.String2 = floor.HallTile.String2;
                                    tile.String3 = floor.HallTile.String3;
                                }


                            }
                            break;
                        case 3: {//floor
                                Maps.Tile tile = map.Tile[x, y];

                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    if (floor.mGroundAlt2X != 0 && Server.Math.Rand(0, 4) == 0) {
                                        tile.Ground = floor.mGroundAlt2X;
                                        tile.GroundSet = floor.mGroundAlt2Sheet;
                                    } else if (floor.mCenterCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Ground = floor.mGroundAltX;
                                        tile.GroundSet = floor.mGroundAltSheet;
                                    } else {
                                        tile.Ground = floor.mGroundX;
                                        tile.GroundSet = floor.mGroundSheet;
                                    }
                                }

                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.GroundTile.Type;
                                    tile.Data1 = floor.GroundTile.Data1;
                                    tile.Data2 = floor.GroundTile.Data2;
                                    tile.Data3 = floor.GroundTile.Data3;
                                    tile.String1 = floor.GroundTile.String1;
                                    tile.String2 = floor.GroundTile.String2;
                                    tile.String3 = floor.GroundTile.String3;
                                }
                            }
                            break;
                        case 7: {//end
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    tile.Mask = floor.StairsX;
                                    tile.MaskSet = floor.StairsSheet;
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = Enums.TileType.RDungeonGoal;
                                }
                            }
                            break;
                        case 6: {//start
                                Maps.Tile tile = map.Tile[x, y];

                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    if (floor.mGroundAlt2X != 0 && Server.Math.Rand(0, 4) == 0) {
                                        tile.Ground = floor.mGroundAlt2X;
                                        tile.GroundSet = floor.mGroundAlt2Sheet;
                                    } else if (floor.mCenterCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Ground = floor.mGroundAltX;
                                        tile.GroundSet = floor.mGroundAltSheet;
                                    } else {
                                        tile.Ground = floor.mGroundX;
                                        tile.GroundSet = floor.mGroundSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.GroundTile.Type;
                                    tile.Data1 = floor.GroundTile.Data1;
                                    tile.Data2 = floor.GroundTile.Data2;
                                    tile.Data3 = floor.GroundTile.Data3;
                                    tile.String1 = floor.GroundTile.String1;
                                    tile.String2 = floor.GroundTile.String2;
                                    tile.String3 = floor.GroundTile.String3;
                                }
                                map.StartX = x;
                                map.StartY = y;
                            }
                            break;
                        case 8: {//trap
                                Maps.Tile tile = map.Tile[x, y];
                                
                                if (floor.SpecialTiles.Count > 0) {
                                    for (int i = 0; i < 200; i++) {
                                        if (Math.Rand(0, 100) < floor.SpecialTiles[trapSpawnMarker].AppearanceRate) {
                                            break;
                                        }
                                        trapSpawnMarker = (trapSpawnMarker + 1) % floor.SpecialTiles.Count;
                                    }

                                    Maps.Tile tile2 = floor.SpecialTiles[trapSpawnMarker];
                                    
                                    if (tile.Ground == 0 && tile.GroundSet == 10) {
                                        tile.Ground = tile2.Ground;
                                        tile.GroundSet = tile2.GroundSet;
                                    }
                                    if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                        tile.GroundAnim = tile2.GroundAnim;
                                        tile.GroundAnimSet = tile2.GroundAnimSet;
                                    }
                                    if (tile.Mask == 0 && tile.MaskSet == 10) {
                                        tile.Mask = tile2.Mask;
                                        tile.MaskSet = tile2.MaskSet;
                                    }
                                    if (tile.Anim == 0 && tile.AnimSet == 10) {
                                        tile.Anim = tile2.Anim;
                                        tile.AnimSet = tile2.AnimSet;
                                    }
                                    if (tile.Mask2 == 0 && tile.Mask2Set == 10) {
                                        tile.Mask2 = tile2.Mask2;
                                        tile.Mask2Set = tile2.Mask2Set;
                                    }
                                    if (tile.M2Anim == 0 && tile.M2AnimSet == 10) {
                                        tile.M2Anim = tile2.M2Anim;
                                        tile.M2AnimSet = tile2.M2AnimSet;
                                    }
                                    if (tile.Fringe == 0 && tile.FringeSet == 10) {
                                        tile.Fringe = tile2.Fringe;
                                        tile.FringeSet = tile2.FringeSet;
                                    }
                                    if (tile.FAnim == 0 && tile.FAnimSet == 10) {
                                        tile.FAnim = tile2.FAnim;
                                        tile.FAnimSet = tile2.FAnimSet;
                                    }
                                    if (tile.Fringe2 == 0 && tile.Fringe2Set == 10) {
                                        tile.Fringe2 = tile2.Fringe2;
                                        tile.Fringe2Set = tile2.Fringe2Set;
                                    }
                                    if (tile.F2Anim == 0 && tile.F2AnimSet == 10) {
                                        tile.F2Anim = tile2.F2Anim;
                                        tile.F2AnimSet = tile2.F2AnimSet;
                                    }

                                    if (tile.Type == Enums.TileType.Ambiguous) {
                                        tile.Type = tile2.Type;
                                        tile.Data1 = tile2.Data1;
                                        tile.Data2 = tile2.Data2;
                                        tile.Data3 = tile2.Data3;
                                        tile.String1 = tile2.String1;
                                        tile.String2 = tile2.String2;
                                        tile.String3 = tile2.String3;
                                    }
                                    

                                    trapSpawnMarker = (trapSpawnMarker + 1) % floor.SpecialTiles.Count;
                                } else {
                                    //just make a regular floor

                                    if (tile.Ground == 0 && tile.GroundSet == 10) {
                                        if (floor.mGroundAlt2X != 0 && Server.Math.Rand(0, 4) == 0) {
                                            tile.Ground = floor.mGroundAlt2X;
                                            tile.GroundSet = floor.mGroundAlt2Sheet;
                                        } else if (floor.mCenterCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                            tile.Ground = floor.mGroundAltX;
                                            tile.GroundSet = floor.mGroundAltSheet;
                                        } else {
                                            tile.Ground = floor.mGroundX;
                                            tile.GroundSet = floor.mGroundSheet;
                                        }
                                    }
                                    if (tile.Type == Enums.TileType.Ambiguous) {
                                        tile.Type = floor.GroundTile.Type;
                                        tile.Data1 = floor.GroundTile.Data1;
                                        tile.Data2 = floor.GroundTile.Data2;
                                        tile.Data3 = floor.GroundTile.Data3;
                                        tile.String1 = floor.GroundTile.String1;
                                        tile.String2 = floor.GroundTile.String2;
                                        tile.String3 = floor.GroundTile.String3;
                                    }
                                }
                            }
                            break;
                        case 256: {//isolated O
                                Maps.Tile tile = map.Tile[x, y];

                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mIsolatedWallAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mIsolatedWallAltX;
                                        tile.MaskSet = floor.mIsolatedWallAltSheet;
                                    } else {
                                        tile.Mask = floor.mIsolatedWallX;
                                        tile.MaskSet = floor.mIsolatedWallSheet;
                                    }
                                }
                                
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 272: {//column top A
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mColumnTopAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mColumnTopAltX;
                                        tile.MaskSet = floor.mColumnTopAltSheet;
                                    } else {
                                        tile.Mask = floor.mColumnTopX;
                                        tile.MaskSet = floor.mColumnTopSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 273: {//column middle H
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mColumnCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mColumnCenterAltX;
                                        tile.MaskSet = floor.mColumnCenterAltSheet;
                                    } else {
                                        tile.Mask = floor.mColumnCenterX;
                                        tile.MaskSet = floor.mColumnCenterSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 257: {//column bottom U
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mColumnBottomAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mColumnBottomAltX;
                                        tile.MaskSet = floor.mColumnBottomAltSheet;
                                    } else {
                                        tile.Mask = floor.mColumnBottomX;
                                        tile.MaskSet = floor.mColumnBottomSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 260: {//row left C
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mRowLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mRowLeftAltX;
                                        tile.MaskSet = floor.mRowLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mRowLeftX;
                                        tile.MaskSet = floor.mRowLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 324: {//row middle Z
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mRowCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mRowCenterAltX;
                                        tile.MaskSet = floor.mRowCenterAltSheet;
                                    } else {
                                        tile.Mask = floor.mRowCenterX;
                                        tile.MaskSet = floor.mRowCenterSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 320: {//row right D
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mRowRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mRowRightAltX;
                                        tile.MaskSet = floor.mRowRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mRowRightX;
                                        tile.MaskSet = floor.mRowRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 455: {//top center _
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mTopCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mTopCenterAltX;
                                        tile.MaskSet = floor.mTopCenterAltSheet;
                                    } else {
                                        tile.Mask = floor.mTopCenterX;
                                        tile.MaskSet = floor.mTopCenterSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 497: {//center left ]
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mCenterLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mCenterLeftAltX;
                                        tile.MaskSet = floor.mCenterLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mCenterLeftX;
                                        tile.MaskSet = floor.mCenterLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 380: {//bottom center T
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mBottomCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mBottomCenterAltX;
                                        tile.MaskSet = floor.mBottomCenterAltSheet;
                                    } else {
                                        tile.Mask = floor.mBottomCenterX;
                                        tile.MaskSet = floor.mBottomCenterSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 287: {// center right [
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mCenterRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mCenterRightAltX;
                                        tile.MaskSet = floor.mCenterRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mCenterRightX;
                                        tile.MaskSet = floor.mCenterRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 449: {//top left J
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mTopLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mTopLeftAltX;
                                        tile.MaskSet = floor.mTopLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mTopLeftX;
                                        tile.MaskSet = floor.mTopLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 263: {//top right L
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mTopRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mTopRightAltX;
                                        tile.MaskSet = floor.mTopRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mTopRightX;
                                        tile.MaskSet = floor.mTopRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 284: {//bottom right F
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mBottomRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mBottomRightAltX;
                                        tile.MaskSet = floor.mBottomRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mBottomRightX;
                                        tile.MaskSet = floor.mBottomRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 368: {//bottom left 7
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mBottomLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mBottomLeftAltX;
                                        tile.MaskSet = floor.mBottomLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mBottomLeftX;
                                        tile.MaskSet = floor.mBottomLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 503: {//inner top left
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mInnerTopLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mInnerTopLeftAltX;
                                        tile.MaskSet = floor.mInnerTopLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mInnerTopLeftX;
                                        tile.MaskSet = floor.mInnerTopLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 479: {//inner top right
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mInnerTopRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mInnerTopRightAltX;
                                        tile.MaskSet = floor.mInnerTopRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mInnerTopRightX;
                                        tile.MaskSet = floor.mInnerTopRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 383: {//inner bottom right
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mInnerBottomRightAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mInnerBottomRightAltX;
                                        tile.MaskSet = floor.mInnerBottomRightAltSheet;
                                    } else {
                                        tile.Mask = floor.mInnerBottomRightX;
                                        tile.MaskSet = floor.mInnerBottomRightSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 509: {//inner bottom left
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mInnerBottomLeftAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mInnerBottomLeftAltX;
                                        tile.MaskSet = floor.mInnerBottomLeftAltSheet;
                                    } else {
                                        tile.Mask = floor.mInnerBottomLeftX;
                                        tile.MaskSet = floor.mInnerBottomLeftSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        case 511: {//perfect wall X
                                Maps.Tile tile = map.Tile[x, y];
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mGroundX;
                                    tile.GroundSet = floor.mGroundSheet;
                                }
                                if (tile.Mask == 0 && tile.MaskSet == 10) {
                                    if (floor.mCenterCenterAlt2X != 0 && Server.Math.Rand(0, 4) == 0) {
                                        tile.Mask = floor.mCenterCenterAlt2X;
                                        tile.MaskSet = floor.mCenterCenterAlt2Sheet;
                                    } else if (floor.mCenterCenterAltX != 0 && Server.Math.Rand(0, 3) == 0) {
                                        tile.Mask = floor.mCenterCenterAltX;
                                        tile.MaskSet = floor.mCenterCenterAltSheet;
                                    } else {
                                        tile.Mask = floor.mCenterCenterX;
                                        tile.MaskSet = floor.mCenterCenterSheet;
                                    }
                                }
                                if (tile.Type == Enums.TileType.Ambiguous) {
                                    tile.Type = floor.WallTile.Type;
                                    tile.Data1 = floor.WallTile.Data1;
                                    tile.Data2 = floor.WallTile.Data2;
                                    tile.Data3 = floor.WallTile.Data3;
                                    tile.String1 = floor.WallTile.String1;
                                    tile.String2 = floor.WallTile.String2;
                                    tile.String3 = floor.WallTile.String3;
                                }
                            }
                            break;
                        #region Items
                        //moved to genitem
                        #endregion
                    }


                    if (map.Tile[x, y].RDungeonMapValue > 511 && map.Tile[x, y].RDungeonMapValue < 1024) {
                        Maps.Tile tile = map.Tile[x, y];

                        if (tile.Type == Enums.TileType.Ambiguous) {
                            tile.Type = floor.WaterTile.Type;
                            tile.Data1 = floor.WaterTile.Data1;
                            tile.Data2 = floor.WaterTile.Data2;
                            tile.Data3 = floor.WaterTile.Data3;
                            tile.String1 = floor.WaterTile.String1;
                            tile.String2 = floor.WaterTile.String2;
                            tile.String3 = floor.WaterTile.String3;
                        }


                        switch (map.Tile[x, y].RDungeonMapValue % 16) {
                            case 0: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreSurroundedX;
                                    tile.GroundSet = floor.mShoreSurroundedSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreSurroundedAnimX;
                                    tile.GroundAnimSet = floor.mShoreSurroundedAnimSheet;
                                }
                                }
                                break;
                            case 1: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerBottomX;
                                    tile.GroundSet = floor.mShoreInnerBottomSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerBottomAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerBottomAnimSheet;
                                }
                                }
                                break;
                            case 2: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerLeftX;
                                    tile.GroundSet = floor.mShoreInnerLeftSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerLeftAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerLeftAnimSheet;
                                }
                                }
                                break;
                            case 3: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerTopX;
                                    tile.GroundSet = floor.mShoreInnerTopSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerTopAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerTopAnimSheet;
                                }
                                }
                                break;
                            case 4: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerRightX;
                                    tile.GroundSet = floor.mShoreInnerRightSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerRightAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerRightAnimSheet;
                                }
                                }
                                break;
                            case 5: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerBottomLeftX;
                                    tile.GroundSet = floor.mShoreInnerBottomLeftSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerBottomLeftAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerBottomLeftAnimSheet;
                                }
                                }
                                break;
                            case 6: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreVerticalX;
                                    tile.GroundSet = floor.mShoreVerticalSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreVerticalAnimX;
                                    tile.GroundAnimSet = floor.mShoreVerticalAnimSheet;
                                }
                                }
                                break;
                            case 7: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerBottomRightX;
                                    tile.GroundSet = floor.mShoreInnerBottomRightSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerBottomRightAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerBottomRightAnimSheet;
                                }
                                }
                                break;
                            case 8: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerTopLeftX;
                                    tile.GroundSet = floor.mShoreInnerTopLeftSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerTopLeftAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerTopLeftAnimSheet;
                                }
                                }
                                break;
                            case 9: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreHorizontalX;
                                    tile.GroundSet = floor.mShoreHorizontalSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreHorizontalAnimX;
                                    tile.GroundAnimSet = floor.mShoreHorizontalAnimSheet;
                                }
                                }
                                break;
                            case 10: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreInnerTopRightX;
                                    tile.GroundSet = floor.mShoreInnerTopRightSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreInnerTopRightAnimX;
                                    tile.GroundAnimSet = floor.mShoreInnerTopRightAnimSheet;
                                }
                                }
                                break;
                            case 11: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreTopX;
                                    tile.GroundSet = floor.mShoreTopSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreTopAnimX;
                                    tile.GroundAnimSet = floor.mShoreTopAnimSheet;
                                }
                                }
                                break;
                            case 12: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreRightX;
                                    tile.GroundSet = floor.mShoreRightSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreRightAnimX;
                                    tile.GroundAnimSet = floor.mShoreRightAnimSheet;
                                }
                                }
                                break;
                            case 13: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreBottomX;
                                    tile.GroundSet = floor.mShoreBottomSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreBottomAnimX;
                                    tile.GroundAnimSet = floor.mShoreBottomAnimSheet;
                                }
                                }
                                break;
                            case 14: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mShoreLeftX;
                                    tile.GroundSet = floor.mShoreLeftSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mShoreLeftAnimX;
                                    tile.GroundAnimSet = floor.mShoreLeftAnimSheet;
                                }
                                }
                                break;
                            case 15: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mWaterX;
                                    tile.GroundSet = floor.mWaterSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mWaterAnimX;
                                    tile.GroundAnimSet = floor.mWaterAnimSheet;
                                }
                                }
                                break;
                            default: {
                                if (tile.Ground == 0 && tile.GroundSet == 10) {
                                    tile.Ground = floor.mWaterX;
                                    tile.GroundSet = floor.mWaterSheet;
                                }
                                if (tile.GroundAnim == 0 && tile.GroundAnimSet == 10) {
                                    tile.GroundAnim = floor.mWaterAnimX;
                                    tile.GroundAnimSet = floor.mWaterAnimSheet;
                                }
                                }
                                break;

                        }

                        switch ((map.Tile[x, y].RDungeonMapValue % 64) / 16) {
                            case 1: {
                                if ((map.Tile[x, y].RDungeonMapValue % 16) == 5 || (map.Tile[x, y].RDungeonMapValue % 16) == 13 ||
                                    (map.Tile[x, y].RDungeonMapValue % 16) == 14 || (map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask == 0 && tile.MaskSet == 10) {
                                            tile.Mask = floor.mShoreTopRightX;
                                            tile.MaskSet = floor.mShoreTopRightSheet;
                                        }
                                        if (tile.Anim == 0 && tile.AnimSet == 10) {
                                            tile.Anim = floor.mShoreTopRightAnimX;
                                            tile.AnimSet = floor.mShoreTopRightAnimSheet;
                                        }
                                    }
                                }
                                break;
                            case 2: {
                                if ((map.Tile[x, y].RDungeonMapValue % 16) == 10 || (map.Tile[x, y].RDungeonMapValue % 16) == 11 ||
                                    (map.Tile[x, y].RDungeonMapValue % 16) == 12 || (map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask == 0 && tile.MaskSet == 10) {
                                            tile.Mask = floor.mShoreBottomLeftX;
                                            tile.MaskSet = floor.mShoreBottomLeftSheet;
                                        }
                                        if (tile.Anim == 0 && tile.AnimSet == 10) {
                                            tile.Anim = floor.mShoreBottomLeftAnimX;
                                            tile.AnimSet = floor.mShoreBottomLeftAnimSheet;
                                        }
                                    }
                                }
                                break;
                            case 3: {
                                if ((map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask == 0 && tile.MaskSet == 10) {
                                            tile.Mask = floor.mShoreDiagonalForwardX;
                                            tile.MaskSet = floor.mShoreDiagonalForwardSheet;
                                        }
                                        if (tile.Anim == 0 && tile.AnimSet == 10) {
                                            tile.Anim = floor.mShoreDiagonalForwardAnimX;
                                            tile.AnimSet = floor.mShoreDiagonalForwardAnimSheet;
                                        }
                                    }
                                }
                                break;
                        }

                        switch ((map.Tile[x, y].RDungeonMapValue - 512) / 64) {
                            case 1: {
                                    if ((map.Tile[x, y].RDungeonMapValue % 16) == 8 || (map.Tile[x, y].RDungeonMapValue % 16) == 11 ||
                                        (map.Tile[x, y].RDungeonMapValue % 16) == 14 || (map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask2 == 0 && tile.Mask2Set == 10) {
                                            tile.Mask2 = floor.mShoreBottomRightX;
                                            tile.Mask2Set = floor.mShoreBottomRightSheet;
                                        }
                                        if (tile.M2Anim == 0 && tile.M2AnimSet == 10) {
                                            tile.M2Anim = floor.mShoreBottomRightAnimX;
                                            tile.M2AnimSet = floor.mShoreBottomRightAnimSheet;
                                        }
                                    }
                                }
                                break;
                            case 2: {
                                    if ((map.Tile[x, y].RDungeonMapValue % 16) == 7 || (map.Tile[x, y].RDungeonMapValue % 16) == 12 ||
                                        (map.Tile[x, y].RDungeonMapValue % 16) == 13 || (map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask2 == 0 && tile.Mask2Set == 10) {
                                            tile.Mask2 = floor.mShoreTopLeftX;
                                            tile.Mask2Set = floor.mShoreTopLeftSheet;
                                        }
                                        if (tile.M2Anim == 0 && tile.M2AnimSet == 10) {
                                            tile.M2Anim = floor.mShoreTopLeftAnimX;
                                            tile.M2AnimSet = floor.mShoreTopLeftAnimSheet;
                                        }
                                    }
                                }
                                break;
                            case 3: {
                                    if ((map.Tile[x, y].RDungeonMapValue % 16) == 15) {
                                        if (tile.Mask2 == 0 && tile.Mask2Set == 10) {
                                            tile.Mask2 = floor.mShoreDiagonalBackX;
                                            tile.Mask2Set = floor.mShoreDiagonalBackSheet;
                                        }
                                        if (tile.M2Anim == 0 && tile.M2AnimSet == 10) {
                                            tile.M2Anim = floor.mShoreDiagonalBackAnimX;
                                            tile.M2AnimSet = floor.mShoreDiagonalBackAnimSheet;
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    Maps.Tile checkedTile = map.Tile[x, y];
                    if (checkedTile.Ground == 0 && checkedTile.GroundSet == 10) {
                        checkedTile.Ground = floor.mGroundX;
                        checkedTile.GroundSet = floor.mGroundSheet;
                    }
                    if (checkedTile.GroundAnim == 0 && checkedTile.GroundAnimSet == 10) {
                        checkedTile.GroundAnim = 0;
                        checkedTile.GroundAnimSet = 0;
                    }
                    if (checkedTile.Mask == 0 && checkedTile.MaskSet == 10) {
                        checkedTile.Mask = 0;
                        checkedTile.MaskSet = 0;
                    }
                    if (checkedTile.Anim == 0 && checkedTile.AnimSet == 10) {
                        checkedTile.Anim = 0;
                        checkedTile.AnimSet = 0;
                    }
                    if (checkedTile.Mask2 == 0 && checkedTile.Mask2Set == 10) {
                        checkedTile.Mask2 = 0;
                        checkedTile.Mask2Set = 0;
                    }
                    if (checkedTile.M2Anim == 0 && checkedTile.M2AnimSet == 10) {
                        checkedTile.M2Anim = 0;
                        checkedTile.M2AnimSet = 0;
                    }
                    if (checkedTile.Fringe == 0 && checkedTile.FringeSet == 10) {
                        checkedTile.Fringe = 0;
                        checkedTile.FringeSet = 0;
                    }
                    if (checkedTile.FAnim == 0 && checkedTile.FAnimSet == 10) {
                        checkedTile.FAnim = 0;
                        checkedTile.FAnimSet = 0;
                    }
                    if (checkedTile.Fringe2 == 0 && checkedTile.Fringe2Set == 10) {
                        checkedTile.Fringe2 = 0;
                        checkedTile.Fringe2Set = 0;
                    }
                    if (checkedTile.F2Anim == 0 && checkedTile.F2AnimSet == 10) {
                        checkedTile.F2Anim = 0;
                        checkedTile.F2AnimSet = 0;
                    }
                    if (checkedTile.Type == Enums.TileType.Ambiguous) {
                        checkedTile.Type = Enums.TileType.Walkable;
                    }
                }
            }
        }

    }
}
