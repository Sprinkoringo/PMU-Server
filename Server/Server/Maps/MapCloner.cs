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

namespace Server.Maps
{
    public class MapCloner
    {
        public static void CloneMapTiles(IMap sourceMap, IMap destinationMap) {
            destinationMap.Name = sourceMap.Name;
            CloneMapTileProperties(sourceMap, destinationMap);
            for (int Y = 0; Y <= destinationMap.MaxY; Y++) {
                for (int X = 0; X <= destinationMap.MaxX; X++) {
                    if (destinationMap.Tile[X, Y] == null) {
                        destinationMap.Tile[X, Y] = new Tile(new DataManager.Maps.Tile());
                    }
                    destinationMap.Tile[X, Y].Ground = sourceMap.Tile[X, Y].Ground;
                    destinationMap.Tile[X, Y].GroundAnim = sourceMap.Tile[X, Y].GroundAnim;
                    destinationMap.Tile[X, Y].Mask = sourceMap.Tile[X, Y].Mask;
                    destinationMap.Tile[X, Y].Anim = sourceMap.Tile[X, Y].Anim;
                    destinationMap.Tile[X, Y].Mask2 = sourceMap.Tile[X, Y].Mask2;
                    destinationMap.Tile[X, Y].M2Anim = sourceMap.Tile[X, Y].M2Anim;
                    destinationMap.Tile[X, Y].Fringe = sourceMap.Tile[X, Y].Fringe;
                    destinationMap.Tile[X, Y].FAnim = sourceMap.Tile[X, Y].FAnim;
                    destinationMap.Tile[X, Y].Fringe2 = sourceMap.Tile[X, Y].Fringe2;
                    destinationMap.Tile[X, Y].F2Anim = sourceMap.Tile[X, Y].F2Anim;
                    destinationMap.Tile[X, Y].Type = sourceMap.Tile[X, Y].Type;
                    destinationMap.Tile[X, Y].Data1 = sourceMap.Tile[X, Y].Data1;
                    destinationMap.Tile[X, Y].Data2 = sourceMap.Tile[X, Y].Data2;
                    destinationMap.Tile[X, Y].Data3 = sourceMap.Tile[X, Y].Data3;
                    destinationMap.Tile[X, Y].String1 = sourceMap.Tile[X, Y].String1;
                    destinationMap.Tile[X, Y].String2 = sourceMap.Tile[X, Y].String2;
                    destinationMap.Tile[X, Y].String3 = sourceMap.Tile[X, Y].String3;
                    destinationMap.Tile[X, Y].RDungeonMapValue = sourceMap.Tile[X, Y].RDungeonMapValue;
                    destinationMap.Tile[X, Y].GroundSet = sourceMap.Tile[X, Y].GroundSet;
                    destinationMap.Tile[X, Y].GroundAnimSet = sourceMap.Tile[X, Y].GroundAnimSet;
                    destinationMap.Tile[X, Y].MaskSet = sourceMap.Tile[X, Y].MaskSet;
                    destinationMap.Tile[X, Y].AnimSet = sourceMap.Tile[X, Y].AnimSet;
                    destinationMap.Tile[X, Y].Mask2Set = sourceMap.Tile[X, Y].Mask2Set;
                    destinationMap.Tile[X, Y].M2AnimSet = sourceMap.Tile[X, Y].M2AnimSet;
                    destinationMap.Tile[X, Y].FringeSet = sourceMap.Tile[X, Y].FringeSet;
                    destinationMap.Tile[X, Y].FAnimSet = sourceMap.Tile[X, Y].FAnimSet;
                    destinationMap.Tile[X, Y].Fringe2Set = sourceMap.Tile[X, Y].Fringe2Set;
                    destinationMap.Tile[X, Y].F2AnimSet = sourceMap.Tile[X, Y].F2AnimSet;
                }
            }
        }

        public static void CloneTile(IMap map, int x, int y, Tile tile) {
            tile.Ground = map.Tile[x, y].Ground;
            tile.GroundAnim = map.Tile[x, y].GroundAnim;
            tile.Mask = map.Tile[x, y].Mask;
            tile.Anim = map.Tile[x, y].Anim;
            tile.Mask2 = map.Tile[x, y].Mask2;
            tile.M2Anim = map.Tile[x, y].M2Anim;
            tile.Fringe = map.Tile[x, y].Fringe;
            tile.FAnim = map.Tile[x, y].FAnim;
            tile.Fringe2 = map.Tile[x, y].Fringe2;
            tile.F2Anim = map.Tile[x, y].F2Anim;
            tile.Type = map.Tile[x, y].Type;
            tile.Data1 = map.Tile[x, y].Data1;
            tile.Data2 = map.Tile[x, y].Data2;
            tile.Data3 = map.Tile[x, y].Data3;
            tile.String1 = map.Tile[x, y].String1;
            tile.String2 = map.Tile[x, y].String2;
            tile.String3 = map.Tile[x, y].String3;
            tile.RDungeonMapValue = map.Tile[x, y].RDungeonMapValue;
            tile.GroundSet = map.Tile[x, y].GroundSet;
            tile.GroundAnimSet = map.Tile[x, y].GroundAnimSet;
            tile.MaskSet = map.Tile[x, y].MaskSet;
            tile.AnimSet = map.Tile[x, y].AnimSet;
            tile.Mask2Set = map.Tile[x, y].Mask2Set;
            tile.M2AnimSet = map.Tile[x, y].M2AnimSet;
            tile.FringeSet = map.Tile[x, y].FringeSet;
            tile.FAnimSet = map.Tile[x, y].FAnimSet;
            tile.Fringe2Set = map.Tile[x, y].Fringe2Set;
            tile.F2AnimSet = map.Tile[x, y].F2AnimSet;
            
        }

        public static void CloneMapNpcs(IMap sourceMap, IMap destinationMap) {
            for (int i = 0; i < sourceMap.Npc.Count; i++) {
                MapNpcPreset newNpc = new MapNpcPreset();
                newNpc.NpcNum = sourceMap.Npc[i].NpcNum;
                newNpc.MinLevel = sourceMap.Npc[i].MinLevel;
                newNpc.MaxLevel = sourceMap.Npc[i].MaxLevel;
                newNpc.SpawnX = sourceMap.Npc[i].SpawnX;
                newNpc.SpawnY = sourceMap.Npc[i].SpawnY;
                newNpc.AppearanceRate = sourceMap.Npc[i].AppearanceRate;
                newNpc.StartStatus = sourceMap.Npc[i].StartStatus;
                newNpc.StartStatusCounter = sourceMap.Npc[i].StartStatusCounter;
                newNpc.StartStatusChance = sourceMap.Npc[i].StartStatusChance;

                destinationMap.Npc.Add(newNpc);
            }
        }

        public static void CloneMapGeneralProperties(IMap sourceMap, IMap destinationMap) {
            destinationMap.Up = sourceMap.Up;
            destinationMap.Down = sourceMap.Down;
            destinationMap.Left = sourceMap.Left;
            destinationMap.Right = sourceMap.Right;
            destinationMap.HungerEnabled = sourceMap.HungerEnabled;
            destinationMap.RecruitEnabled = sourceMap.RecruitEnabled;
            destinationMap.ExpEnabled = sourceMap.ExpEnabled;
            destinationMap.TimeLimit = sourceMap.TimeLimit;
            destinationMap.Indoors = sourceMap.Indoors;
            destinationMap.Moral = sourceMap.Moral;
            destinationMap.Music = sourceMap.Music;
            destinationMap.Name = sourceMap.Name;
            destinationMap.OriginalDarkness = sourceMap.OriginalDarkness;
            destinationMap.Weather = sourceMap.Weather;
            destinationMap.DungeonIndex = sourceMap.DungeonIndex;
            destinationMap.MinNpcs = sourceMap.MinNpcs;
            destinationMap.MaxNpcs = sourceMap.MaxNpcs;
            destinationMap.NpcSpawnTime = sourceMap.NpcSpawnTime;
        }

        public static void CloneMapTileProperties(IMap sourceMap, IMap destinationMap) {
            destinationMap.Tile = new TileCollection(destinationMap.BaseMap, sourceMap.MaxX, sourceMap.MaxY);
        }

        public static InstancedMap CreateInstancedMap(IMap sourceMap) {
            DataManager.Maps.InstancedMap dmInstancedMap = new DataManager.Maps.InstancedMap(MapManager.GenerateMapID("i"));
            InstancedMap iMap = new InstancedMap(dmInstancedMap);
            CloneMapTileProperties(sourceMap, iMap);
            CloneMapTiles(sourceMap, iMap);
            CloneMapGeneralProperties(sourceMap, iMap);
            CloneMapNpcs(sourceMap, iMap);
            return iMap;
        }
    }
}
