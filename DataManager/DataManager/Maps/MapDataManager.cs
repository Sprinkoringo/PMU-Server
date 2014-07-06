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
using PMU.DatabaseConnector.MySql;
using PMU.Core;
using PMU.DatabaseConnector;

namespace DataManager.Maps
{
    public class MapDataManager
    {
        public static void LoadRawMap(MySql database, string mapID, RawMap rawMap) {
            string query = "SELECT map_general.Revision, map_general.MaxX, map_general.MaxY, " +
                "map_switchovers.UpMap, map_switchovers.DownMap, map_switchovers.LeftMap, map_switchovers.RightMap, " +
                "map_data.Name, map_data.Moral, map_data.Music, map_data.Indoors, map_data.Weather, " +
                "map_data.Darkness, map_data.HungerEnabled, map_data.RecruitmentEnabled, map_data.ExpEnabled, map_data.TimeLimit, " +
                "map_data.MinNpcs, map_data.MaxNpcs, map_data.NpcSpawnTime " +
                "FROM map_data " +
                "JOIN map_general ON map_general.MapID = map_data.MapID " +
                "JOIN map_switchovers ON map_switchovers.MapID = map_data.MapID " +
                "WHERE map_data.MapID = \'" + database.VerifyValueString(mapID) + "\'";

            {
                // Load general data + switchovers
                DataColumnCollection row = database.RetrieveRow(query);
                if (row != null) {
                    int counter = 0;
                    rawMap.Revision = Convert.ToInt32(row[counter++].Value);
                    rawMap.MaxX = Convert.ToInt32(row[counter++].Value);
                    rawMap.MaxY = Convert.ToInt32(row[counter++].Value);
                    rawMap.Up = Convert.ToInt32(row[counter++].Value);
                    rawMap.Down = Convert.ToInt32(row[counter++].Value);
                    rawMap.Left = Convert.ToInt32(row[counter++].Value);
                    rawMap.Right = Convert.ToInt32(row[counter++].Value);
                    rawMap.Name = row[counter++].ValueString;
                    rawMap.Moral = Convert.ToByte(row[counter++].Value);
                    rawMap.Music = row[counter++].ValueString;
                    rawMap.Indoors = row[counter++].ValueString.ToBool();
                    rawMap.Weather = Convert.ToByte(row[counter++].Value);
                    rawMap.Darkness = Convert.ToInt32(row[counter++].Value);
                    rawMap.HungerEnabled = row[counter++].ValueString.ToBool();
                    rawMap.RecruitEnabled = row[counter++].ValueString.ToBool();
                    rawMap.ExpEnabled = row[counter++].ValueString.ToBool();
                    rawMap.TimeLimit = Convert.ToInt32(row[counter++].Value);
                    rawMap.MinNpcs = Convert.ToInt32(row[counter++].Value);
                    rawMap.MaxNpcs = Convert.ToInt32(row[counter++].Value);
                    rawMap.NpcSpawnTime = Convert.ToInt32(row[counter++].Value);

                    rawMap.Tile = new Tile[rawMap.MaxX + 1, rawMap.MaxY + 1];
                } else {
                    throw new Exception("Raw map data not found.");
                }
            }

            // Load map npc presets
            query = "SELECT map_npcs.Number, map_npcs.SpawnX, map_npcs.SpawnY, map_npcs.MinLevel, map_npcs.MaxLevel, " +
                "map_npcs.AppearanceRate, map_npcs.StartStatus, map_npcs.StartStatusCounter, map_npcs.StartStatusChance " +
                "FROM map_npcs WHERE map_npcs.MapID = \'" + database.VerifyValueString(mapID) + "\' ORDER BY map_npcs.Slot";
            foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                MapNpcPreset mapNpc = new MapNpcPreset();
                int counter = 0;
                mapNpc.NpcNum = Convert.ToInt32(row[counter++].Value);
                mapNpc.SpawnX = Convert.ToInt32(row[counter++].Value);
                mapNpc.SpawnY = Convert.ToInt32(row[counter++].Value);
                mapNpc.MinLevel = Convert.ToInt32(row[counter++].Value);
                mapNpc.MaxLevel = Convert.ToInt32(row[counter++].Value);
                mapNpc.AppearanceRate = Convert.ToInt32(row[counter++].Value);
                mapNpc.StartStatus = Convert.ToInt32(row[counter++].Value);
                mapNpc.StartStatusCounter = Convert.ToInt32(row[counter++].Value);
                mapNpc.StartStatusChance = Convert.ToInt32(row[counter++].Value);

                rawMap.Npc.Add(mapNpc);
            }

            // Load map tiles
            query = "SELECT map_tiles.X, map_tiles.Y, map_tiles.Ground, map_tiles.GroundAnim, map_tiles.Mask, map_tiles.MaskAnim, " +
                "map_tiles.Mask2, map_tiles.Mask2Anim, map_tiles.Fringe, map_tiles.FringeAnim, map_tiles.Fringe2, " +
                "map_tiles.Fringe2Anim, map_tiles.Type, map_tiles.Data1, map_tiles.Data2, map_tiles.Data3, map_tiles.String1, " +
                "map_tiles.String2, map_tiles.String3, map_tiles.Light, map_tiles.GroundTileset, map_tiles.GroundAnimTileset, " +
                "map_tiles.MaskTileset, map_tiles.MaskAnimTileset, map_tiles.Mask2Tileset, map_tiles.Mask2AnimTileset, " +
                "map_tiles.FringeTileset, map_tiles.FringeAnimTileset, map_tiles.Fringe2Tileset, map_tiles.Fringe2AnimTileset " +
                "FROM map_tiles WHERE map_tiles.MapID = \'" + database.VerifyValueString(mapID) + "\'";
            foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                Tile tile = new Tile();
                int counter = 0;
                int x = Convert.ToInt32(row[counter++].Value);
                int y = Convert.ToInt32(row[counter++].Value);
                tile.Ground = Convert.ToInt32(row[counter++].Value);
                tile.GroundAnim = Convert.ToInt32(row[counter++].Value);
                tile.Mask = Convert.ToInt32(row[counter++].Value);
                tile.Anim = Convert.ToInt32(row[counter++].Value);
                tile.Mask2 = Convert.ToInt32(row[counter++].Value);
                tile.M2Anim = Convert.ToInt32(row[counter++].Value);
                tile.Fringe = Convert.ToInt32(row[counter++].Value);
                tile.FAnim = Convert.ToInt32(row[counter++].Value);
                tile.Fringe2 = Convert.ToInt32(row[counter++].Value);
                tile.F2Anim = Convert.ToInt32(row[counter++].Value);
                tile.Type = Convert.ToInt32(row[counter++].Value);
                tile.Data1 = Convert.ToInt32(row[counter++].Value);
                tile.Data2 = Convert.ToInt32(row[counter++].Value);
                tile.Data3 = Convert.ToInt32(row[counter++].Value);
                tile.String1 = row[counter++].ValueString;
                tile.String2 = row[counter++].ValueString;
                tile.String3 = row[counter++].ValueString;
                tile.RDungeonMapValue = Convert.ToInt32(row[counter++].Value);
                tile.GroundSet = Convert.ToInt32(row[counter++].Value);
                tile.GroundAnimSet = Convert.ToInt32(row[counter++].Value);
                tile.MaskSet = Convert.ToInt32(row[counter++].Value);
                tile.AnimSet = Convert.ToInt32(row[counter++].Value);
                tile.Mask2Set = Convert.ToInt32(row[counter++].Value);
                tile.M2AnimSet = Convert.ToInt32(row[counter++].Value);
                tile.FringeSet = Convert.ToInt32(row[counter++].Value);
                tile.FAnimSet = Convert.ToInt32(row[counter++].Value);
                tile.Fringe2Set = Convert.ToInt32(row[counter++].Value);
                tile.F2AnimSet = Convert.ToInt32(row[counter++].Value);

                rawMap.Tile[x, y] = tile;
            }

            rawMap.MapID = mapID;
        }

        public static void LoadMapDump(MySql database, string mapID, MapDump mapDump) {
            // First, load the raw map data
            LoadRawMap(database, mapID, mapDump);

            mapID = database.VerifyValueString(mapID);

            // Load map state data
            string query = "SELECT mapstate_data.ActivationTime, mapstate_data.ProcessingPaused, mapstate_data.SpawnMarker, " +
                "mapstate_data.NpcSpawnWait, mapstate_data.TempChange, mapstate_data.CurrentWeather " +
                "FROM mapstate_data WHERE mapstate_data.MapID = \'" + mapID + "\'"; ;
            {
                DataColumnCollection row = database.RetrieveRow(query);
                if (row != null) {
                    int counter = 0;
                    mapDump.ActivationTime = Convert.ToInt32(row[counter++].Value);
                    mapDump.ProcessingPaused = row[counter++].ValueString.ToBool();
                    mapDump.SpawnMarker = Convert.ToInt32(row[counter++].Value);
                    mapDump.NpcSpawnWait = Convert.ToInt32(row[counter++].Value);
                    mapDump.TempChange = row[counter++].ValueString.ToBool();
                    mapDump.CurrentWeather = Convert.ToByte(row[counter++].Value);
                } else {
                    throw new Exception("Map state data not found");
                }
            }


            { // Load map active items
                query = "SELECT mapstate_activeitem.Slot, mapstate_activeitem.Number, mapstate_activeitem.Value, mapstate_activeitem.Sticky, " +
                   "mapstate_activeitem.Tag, mapstate_activeitem.Hidden, mapstate_activeitem.X, mapstate_activeitem.Y, " +
                   "mapstate_activeitem.TimeRemaining, mapstate_activeitem.ItemOwner " +
                   "FROM mapstate_activeitem WHERE mapstate_activeitem.MapID = \'" + mapID + "\'";
                foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                    MapItem mapItem = new MapItem();
                    int counter = 0;
                    int slot = Convert.ToInt32(row[counter++].Value);
                    mapItem.Num = Convert.ToInt32(row[counter++].Value);
                    mapItem.Value = Convert.ToInt32(row[counter++].Value);
                    mapItem.Sticky = row[counter++].ValueString.ToBool();
                    mapItem.Tag = row[counter++].ValueString;
                    mapItem.Hidden = row[counter++].ValueString.ToBool();
                    mapItem.X = Convert.ToInt32(row[counter++].Value);
                    mapItem.Y = Convert.ToInt32(row[counter++].Value);
                    mapItem.TimeRemaining = Convert.ToInt32(row[counter++].Value);
                    mapItem.PlayerFor = row[counter++].ValueString;

                    mapDump.ActiveItem[slot] = mapItem;
                }
            }

            { // Load map active npcs
                query = "SELECT mapstate_activenpc_data.MapNpcSlot, mapstate_activenpc_data.Name, mapstate_activenpc_data.Shiny, mapstate_activenpc_data.Form, " +
                   "mapstate_activenpc_data.Level, mapstate_activenpc_data.NpcNumber, mapstate_activenpc_data.Sex, " +
                   "mapstate_activenpc_data.AttackTimer, mapstate_activenpc_data.PauseTimer, mapstate_activenpc_data.StatusAilment, " +
                   "mapstate_activenpc_data.StatusAilmentCounter, mapstate_activenpc_data.HPStepCounter, " +
                   "mapstate_activenpc_data.Target, " +
                   "mapstate_activenpc_location.X, mapstate_activenpc_location.Y, mapstate_activenpc_location.Direction, " +
                   "mapstate_activenpc_stats.IQ, mapstate_activenpc_stats.HP, mapstate_activenpc_stats.HPRemainder, " +
                   "mapstate_activenpc_stats.MaxHPBonus, mapstate_activenpc_stats.AtkBonus, mapstate_activenpc_stats.DefBonus, " +
                   "mapstate_activenpc_stats.SpdBonus, mapstate_activenpc_stats.SpclAtkBonus, mapstate_activenpc_stats.SpclDefBonus, " +
                   "mapstate_activenpc_stats.AttackBuff, " +
                   "mapstate_activenpc_stats.DefenseBuff, mapstate_activenpc_stats.SpAtkBuff, mapstate_activenpc_stats.SpDefBuff, mapstate_activenpc_stats.SpeedBuff, " +
                   "mapstate_activenpc_stats.AccuracyBuff, mapstate_activenpc_stats.EvasionBuff " +
                   "FROM mapstate_activenpc_data " +
                   "LEFT OUTER JOIN mapstate_activenpc_location ON mapstate_activenpc_data.MapID = mapstate_activenpc_location.MapID AND mapstate_activenpc_data.MapNpcSlot = mapstate_activenpc_location.MapNpcSlot " +
                   "JOIN mapstate_activenpc_stats ON mapstate_activenpc_data.MapID = mapstate_activenpc_stats.MapID AND mapstate_activenpc_data.MapNpcSlot = mapstate_activenpc_stats.MapNpcSlot " +
                   "WHERE mapstate_activenpc_data.MapID = \'" + mapID + "\'";
                foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                    int counter = 0;
                    int slot = Convert.ToInt32(row[counter++].Value);
                    MapNpc npc = new MapNpc(slot);
                    npc.Name = row[counter++].ValueString;
                    npc.Shiny = Convert.ToByte(row[counter++].Value);
                    npc.Form = Convert.ToInt32(row[counter++].Value);
                    npc.Level = Convert.ToInt32(row[counter++].Value);
                    npc.Num = Convert.ToInt32(row[counter++].Value);
                    //npc.Sprite = Convert.ToInt32(row[counter++].Value);
                    npc.Sex = Convert.ToByte(row[counter++].Value);
                    //npc.Type1 = Convert.ToByte(row[counter++].Value);
                    //npc.Type2 = Convert.ToByte(row[counter++].Value);
                    //npc.Ability1 = row[counter++].ValueString;
                    //npc.Ability2 = row[counter++].ValueString;
                    //npc.Ability3 = row[counter++].ValueString;
                    //npc.TimeMultiplier = Convert.ToInt32(row[counter++].Value);
                    npc.AttackTimer = Convert.ToInt32(row[counter++].Value);
                    npc.PauseTimer = Convert.ToInt32(row[counter++].Value);
                    npc.StatusAilment = Convert.ToByte(row[counter++].Value);
                    npc.StatusAilmentCounter = Convert.ToInt32(row[counter++].Value);
                    npc.HPStepCounter = Convert.ToInt32(row[counter++].Value);
                    //npc.ConfusionStepCounter = Convert.ToInt32(row[counter++].Value);
                    //npc.SpeedLimit = Convert.ToByte(row[counter++].Value);
                    npc.Target = row[counter++].ValueString;
                    //npc.ItemActive = row[counter++].ValueString.ToBool();
                    

                    // Load location data
                    npc.X = Convert.ToInt32(row[counter++].Value);
                    npc.Y = Convert.ToInt32(row[counter++].Value);
                    npc.Direction = Convert.ToByte(row[counter++].Value);

                    // Load stat data
                    npc.IQ = Convert.ToInt32(row[counter++].Value);
                    npc.HP = Convert.ToInt32(row[counter++].Value);
                    npc.HPRemainder = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseMaxHP = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseAtk = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseDef = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseSpd = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseSpclAtk = Convert.ToInt32(row[counter++].Value);
                    //npc.BaseSpclDef = Convert.ToInt32(row[counter++].Value);
                    //npc.MaxHPBonus = Convert.ToInt32(row[counter++].Value);
                    npc.AtkBonus = Convert.ToInt32(row[counter++].Value);
                    npc.DefBonus = Convert.ToInt32(row[counter++].Value);
                    npc.SpdBonus = Convert.ToInt32(row[counter++].Value);
                    npc.SpclAtkBonus = Convert.ToInt32(row[counter++].Value);
                    npc.SpclDefBonus = Convert.ToInt32(row[counter++].Value);
                    //npc.MaxHPBoost = Convert.ToInt32(row[counter++].Value);
                    //npc.AtkBoost = Convert.ToInt32(row[counter++].Value);
                    //npc.DefBoost = Convert.ToInt32(row[counter++].Value);
                    //npc.SpdBoost = Convert.ToInt32(row[counter++].Value);
                    //npc.SpclAtkBoost = Convert.ToInt32(row[counter++].Value);
                    //npc.SpclDefBoost = Convert.ToInt32(row[counter++].Value);
                    npc.AttackBuff = Convert.ToInt32(row[counter++].Value);
                    npc.DefenseBuff = Convert.ToInt32(row[counter++].Value);
                    npc.SpAtkBuff = Convert.ToInt32(row[counter++].Value);
                    npc.SpDefBuff = Convert.ToInt32(row[counter++].Value);
                    npc.SpeedBuff = Convert.ToInt32(row[counter++].Value);
                    npc.AccuracyBuff = Convert.ToInt32(row[counter++].Value);
                    npc.EvasionBuff = Convert.ToInt32(row[counter++].Value);

                    mapDump.ActiveNpc[slot] = npc;
                }

                query = "SELECT mapstate_activenpc_helditem.MapNpcSlot, mapstate_activenpc_helditem.ItemNumber, mapstate_activenpc_helditem.Amount, " +
                   "mapstate_activenpc_helditem.Sticky, mapstate_activenpc_helditem.Tag " +
                   "FROM mapstate_activenpc_helditem " +
                   "WHERE mapstate_activenpc_helditem.MapID = \'" + mapID + "\'";
                foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                    int counter = 0;
                    int slot = Convert.ToInt32(row[counter++].Value);
                    MapNpc npc = new MapNpc(slot);
                    // Load held item
                    {
                        int heldItemNum = Convert.ToInt32(row[counter++].Value);
                        if (heldItemNum > 0) {
                            // The npc has a held item! Continue loading it
                            npc.HeldItem = new Characters.InventoryItem();
                            npc.HeldItem.Num = heldItemNum;
                            npc.HeldItem.Amount = Convert.ToInt32(row[counter++].Value);
                            npc.HeldItem.Sticky = row[counter++].ValueString.ToBool();
                            npc.HeldItem.Tag = row[counter++].ValueString;
                        } else {
                            // No held item - skip loading the other values
                            npc.HeldItem = new Characters.InventoryItem();
                            counter += 3;
                        }
                    }

                    mapDump.ActiveNpc[slot] = npc;
                }

                query = "SELECT mapstate_activenpc_moves.MapNpcSlot, mapstate_activenpc_moves.MoveSlot, mapstate_activenpc_moves.CurrentPP, " +
                    "mapstate_activenpc_moves.MaxPP, mapstate_activenpc_moves.MoveNum " +
                    "FROM mapstate_activenpc_moves WHERE mapstate_activenpc_moves.MapID = \'" + mapID + "\'";
                foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                    int counter = 0;
                    int slot = Convert.ToInt32(row[counter++].Value);
                    int moveSlot = Convert.ToInt32(row[counter++].Value);

                    Characters.Move move = new Characters.Move();
                    move.CurrentPP = Convert.ToInt32(row[counter++].Value);
                    move.MaxPP = Convert.ToInt32(row[counter++].Value);
                    move.MoveNum = Convert.ToInt32(row[counter++].Value);
                    //move.Sealed = row[counter++].ValueString.ToBool();

                    mapDump.ActiveNpc[slot].Moves[moveSlot] = move;
                }

                //query = "SELECT mapstate_activenpc_mobility.MapNpcSlot, mapstate_activenpc_mobility.MobilitySlot, mapstate_activenpc_mobility.Mobile " +
                //   "FROM mapstate_activenpc_mobility WHERE mapstate_activenpc_mobility.MapID = \'" + mapID + "\'";
                //foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                //    int counter = 0;
                //    int slot = Convert.ToInt32(row[counter++].Value);
                //    int mobilitySlot = Convert.ToInt32(row[counter++].Value);

                //    mapDump.ActiveNpc[slot].Mobility[mobilitySlot] = row[counter++].ValueString.ToBool();
                //}

                query = "SELECT mapstate_activenpc_volatilestatus.MapNpcSlot, mapstate_activenpc_volatilestatus.Name, " +
                    "mapstate_activenpc_volatilestatus.Emoticon, mapstate_activenpc_volatilestatus.Counter, mapstate_activenpc_volatilestatus.Tag " +
                    "FROM mapstate_activenpc_volatilestatus WHERE mapstate_activenpc_volatilestatus.MapID = \'" + mapID + "\' ORDER BY mapstate_activenpc_volatilestatus.StatusIndex";
                foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                    int counter = 0;
                    int slot = Convert.ToInt32(row[counter++].Value);

                    Characters.VolatileStatus status = new Characters.VolatileStatus();
                    status.Name = row[counter++].ValueString;
                    status.Emoticon = Convert.ToInt32(row[counter++].Value);
                    status.Counter = Convert.ToInt32(row[counter++].ValueString);
                    status.Tag = row[counter++].ValueString;

                    mapDump.ActiveNpc[slot].VolatileStatus.Add(status);
                }


            }

            // Load map status
            query = "SELECT mapstate_mapstatus.StatusIndex, mapstate_mapstatus.Name, mapstate_mapstatus.Tag, mapstate_mapstatus.GraphicEffect, mapstate_mapstatus.Tag, mapstate_mapstatus.Counter " +
                "FROM mapstate_mapstatus WHERE mapstate_mapstatus.MapID = \'" + mapID + "\' ORDER BY mapstate_mapstatus.StatusIndex";
            foreach (DataColumnCollection row in database.RetrieveRowsEnumerable(query)) {
                MapStatus status = new MapStatus();
                int counter = 1;

                status.Name = row[counter++].ValueString;
                status.Tag = row[counter++].ValueString;
                status.GraphicEffect = Convert.ToInt32(row[counter++].ValueString);
                status.Counter = Convert.ToInt32(row.FindByName("Counter").Value);

                mapDump.TempStatus.Add(status);
            }
        }

        public static void SaveMapDump(MySql database, string mapID, MapDump mapDump) {
            mapID = database.VerifyValueString(mapID);

            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            SaveRawMap(database, mapID, mapDump);

            // Save map state data
            database.UpdateOrInsert("mapstate_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "ActivationTime", mapDump.ActivationTime.ToString()),
                database.CreateColumn(false, "ProcessingPaused", mapDump.ProcessingPaused.ToIntString()),
                database.CreateColumn(false, "SpawnMarker", mapDump.SpawnMarker.ToString()),
                database.CreateColumn(false, "NpcSpawnWait", mapDump.NpcSpawnWait.ToString()),
                database.CreateColumn(false, "TempChange", mapDump.TempChange.ToIntString()),
                database.CreateColumn(false, "CurrentWeather", mapDump.CurrentWeather.ToString())
            });

            // Save active item data
            MultiRowInsert multiRowInsert = new MultiRowInsert(database, "mapstate_activeitem", "MapID", "Slot", "Number", "Value",
                "Sticky", "Tag", "Hidden", "X", "Y", "TimeRemaining", "ItemOwner");

            for (int i = 0; i < mapDump.ActiveItem.Length; i++) {
                if (mapDump.ActiveItem[i].Num > 0) {
                    MapItem item = mapDump.ActiveItem[i];

                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i, item.Num, item.Value);
                    multiRowInsert.AddColumnData(item.Sticky.ToIntString(), item.Tag, item.Hidden.ToIntString());
                    multiRowInsert.AddColumnData(item.X, item.Y, item.TimeRemaining);
                    multiRowInsert.AddColumnData(item.PlayerFor);

                    multiRowInsert.AddRowClosing();
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npcs
            multiRowInsert.UpdateParameters("mapstate_activenpc_data", new string[] {"MapID", "MapNpcSlot", "Name", "Shiny", "Form", "Level",
                "NpcNumber", "Sex",
                "AttackTimer", "PauseTimer", "StatusAilment", "StatusAilmentCounter", "HPStepCounter",
                "Target" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0) {
                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i);
                    multiRowInsert.AddColumnData(npc.Name, npc.Shiny.ToString());
                    multiRowInsert.AddColumnData(npc.Form, npc.Level, npc.Num, (int)npc.Sex);
                    multiRowInsert.AddColumnData(npc.AttackTimer, npc.PauseTimer, (int)npc.StatusAilment, npc.StatusAilmentCounter,
                        npc.HPStepCounter);
                    multiRowInsert.AddColumnData(npc.Target);

                    multiRowInsert.AddRowClosing();
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc held items
            multiRowInsert.UpdateParameters("mapstate_activenpc_helditem", new string[]  {"MapID", "MapNpcSlot",
                "ItemNumber", "Amount", "Sticky", "Tag" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0 && mapDump.ActiveNpc[i].HeldItem.Num > 0) {
                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i);
                    //multiRowInsert.AddColumnData(npc.ItemActive.ToIntString());
                    //if (npc.HeldItem != null) {
                        // The npc is holding an item - save the held item information
                        multiRowInsert.AddColumnData(npc.HeldItem.Num, npc.HeldItem.Amount);
                        multiRowInsert.AddColumnData(npc.HeldItem.Sticky.ToIntString(), npc.HeldItem.Tag);
                    //} else {
                        // The npc is not holding an item! Save default information
                    //    multiRowInsert.AddColumnData(-1, 0, 0);
                    //    multiRowInsert.AddColumnData("");
                    //}

                    multiRowInsert.AddRowClosing();
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc locations
            multiRowInsert.UpdateParameters("mapstate_activenpc_location", new string[] { "MapID", "MapNpcSlot", "X", "Y", "Direction" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0) {
                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i);
                    multiRowInsert.AddColumnData(npc.X, npc.Y, (int)npc.Direction);

                    multiRowInsert.AddRowClosing();
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc stats
            multiRowInsert.UpdateParameters("mapstate_activenpc_stats", new string[] { "MapID", "MapNpcSlot", "IQ", "HP", "HPRemainder",
                "MaxHPBonus", "AtkBonus", "DefBonus",
                "SpdBonus", "SpclAtkBonus", "SpclDefBonus",
                "AttackBuff", "DefenseBuff", "SpAtkBuff", "SpDefBuff", "SpeedBuff", "AccuracyBuff", "EvasionBuff" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0) {
                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i);
                    multiRowInsert.AddColumnData(npc.IQ, npc.HP, npc.HPRemainder,
                        npc.MaxHPBonus, npc.AtkBonus, npc.DefBonus,
                        npc.SpdBonus, npc.SpclAtkBonus, npc.SpclDefBonus,
                        npc.AttackBuff, npc.DefenseBuff, npc.SpAtkBuff, npc.SpDefBuff,
                        npc.SpeedBuff, npc.AccuracyBuff, npc.EvasionBuff);

                    multiRowInsert.AddRowClosing();
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc moves
            multiRowInsert.UpdateParameters("mapstate_activenpc_moves", new string[] { "MapID", "MapNpcSlot", "MoveSlot", "CurrentPP", "MaxPP", "MoveNum" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0) {
                    for (int n = 0; n < npc.Moves.Length; n++) {
                        if (npc.Moves[n].MoveNum > 0) {
                            multiRowInsert.AddRowOpening();

                            multiRowInsert.AddColumnData(mapID);
                            multiRowInsert.AddColumnData(i, n);
                            multiRowInsert.AddColumnData(npc.Moves[n].CurrentPP, npc.Moves[n].MaxPP, npc.Moves[n].MoveNum);
                            //multiRowInsert.AddColumnData(npc.Moves[n].Sealed.ToIntString());

                            multiRowInsert.AddRowClosing();
                        }
                    }
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc mobility
            //multiRowInsert.UpdateParameters("mapstate_activenpc_mobility", new string[] { "MapID", "MapNpcSlot", "MobilitySlot", "Mobile" });
            //for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
            //    MapNpc npc = mapDump.ActiveNpc[i];

            //    for (int n = 0; n < npc.Mobility.Length; n++) {
            //        multiRowInsert.AddRowOpening();

            //        multiRowInsert.AddColumnData(mapID);
            //        multiRowInsert.AddColumnData(i, n);
            //        multiRowInsert.AddColumnData(npc.Mobility[n].ToIntString());

            //        multiRowInsert.AddRowClosing();
            //    }
            //}
            //database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save active npc volatile status
            multiRowInsert.UpdateParameters("mapstate_activenpc_volatilestatus", new string[] { "MapID", "MapNpcSlot", "StatusIndex", "Name", "Emoticon", "Counter", "Tag" });
            for (int i = 0; i < mapDump.ActiveNpc.Length; i++) {
                MapNpc npc = mapDump.ActiveNpc[i];
                if (mapDump.ActiveNpc[i].Num > 0) {
                    for (int n = 0; n < npc.VolatileStatus.Count; n++) {
                        multiRowInsert.AddRowOpening();

                        multiRowInsert.AddColumnData(mapID);
                        multiRowInsert.AddColumnData(i, n);
                        multiRowInsert.AddColumnData(npc.VolatileStatus[n].Name);
                        multiRowInsert.AddColumnData(npc.VolatileStatus[n].Emoticon);
                        multiRowInsert.AddColumnData(npc.VolatileStatus[n].Counter);
                        multiRowInsert.AddColumnData(npc.VolatileStatus[n].Tag);

                        multiRowInsert.AddRowClosing();
                    }
                }
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            // Save map status
            multiRowInsert.UpdateParameters("mapstate_mapstatus", new string[] { "MapID", "StatusIndex", "Name", "Tag", "GraphicEffect", "Counter" });
            for (int i = 0; i < mapDump.TempStatus.Count; i++) {
                MapStatus status = mapDump.TempStatus[i];

                multiRowInsert.AddRowOpening();

                multiRowInsert.AddColumnData(mapID);
                multiRowInsert.AddColumnData(i);
                multiRowInsert.AddColumnData(status.Name);
                multiRowInsert.AddColumnData(status.Tag);
                multiRowInsert.AddColumnData(status.GraphicEffect);
                multiRowInsert.AddColumnData(status.Counter);

                multiRowInsert.AddRowClosing();
            }
            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static void SaveRawMap(MySql database, string mapID, RawMap rawMap) {

            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            // Delete old map, if it exists
            DeleteMap(database, mapID);

            // Save map general information
            database.UpdateOrInsert("map_general", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "Revision", rawMap.Revision.ToString()),
                database.CreateColumn(false, "MaxX", rawMap.MaxX.ToString()),
                database.CreateColumn(false, "MaxY", rawMap.MaxY.ToString())
            });

            // Save map data
            database.UpdateOrInsert("map_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "Name", rawMap.Name),
                database.CreateColumn(false, "Moral", rawMap.Moral.ToString()),
                database.CreateColumn(false, "Music", rawMap.Music),
                database.CreateColumn(false, "Indoors", rawMap.Indoors.ToIntString()),
                database.CreateColumn(false, "Weather", rawMap.Weather.ToString()),
                database.CreateColumn(false, "Darkness", rawMap.Darkness.ToString()),
                database.CreateColumn(false, "HungerEnabled", rawMap.HungerEnabled.ToIntString()),
                database.CreateColumn(false, "RecruitmentEnabled", rawMap.RecruitEnabled.ToIntString()),
                database.CreateColumn(false, "ExpEnabled", rawMap.ExpEnabled.ToIntString()),
                database.CreateColumn(false, "TimeLimit", rawMap.TimeLimit.ToString()),
                database.CreateColumn(false, "MinNpcs", rawMap.MinNpcs.ToString()),
                database.CreateColumn(false, "MaxNpcs", rawMap.MaxNpcs.ToString()),
                database.CreateColumn(false, "NpcSpawnTime", rawMap.NpcSpawnTime.ToString())
            });

            // Save map switchovers
            database.UpdateOrInsert("map_switchovers", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "UpMap", rawMap.Up.ToString()),
                database.CreateColumn(false, "DownMap", rawMap.Down.ToString()),
                database.CreateColumn(false, "LeftMap", rawMap.Left.ToString()),
                database.CreateColumn(false, "RightMap", rawMap.Right.ToString())
            });

            // Save map npc presets
            MultiRowInsert multiRowInsert = new MultiRowInsert(database, "map_npcs", "MapID", "Slot", "Number", "SpawnX",
                "SpawnY", "MinLevel", "MaxLevel", "AppearanceRate", "StartStatus", "StartStatusCounter", "StartStatusChance");

            if (rawMap.Npc.Count > 0) {
                for (int i = 0; i < rawMap.Npc.Count; i++) {
                    multiRowInsert.AddRowOpening();

                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(i, rawMap.Npc[i].NpcNum, rawMap.Npc[i].SpawnX, rawMap.Npc[i].SpawnY, rawMap.Npc[i].MinLevel,
                        rawMap.Npc[i].MaxLevel, rawMap.Npc[i].AppearanceRate, rawMap.Npc[i].StartStatus, rawMap.Npc[i].StartStatusCounter,
                        rawMap.Npc[i].StartStatusChance);

                    multiRowInsert.AddRowClosing();
                }
                database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());
            }

            // Save map tiles
            multiRowInsert.UpdateParameters("map_tiles", new string[] {"MapID", "X", "Y", "Ground", "GroundAnim", "Mask",
                "MaskAnim", "Mask2", "Mask2Anim", "Fringe", "FringeAnim", "Fringe2", "Fringe2Anim", "Type", "Data1", "Data2", "Data3", "String1",
                "String2", "String3", "Light", "GroundTileset", "GroundAnimTileset", "MaskTileset", "MaskAnimTileset",
                "Mask2Tileset", "Mask2AnimTileset", "FringeTileset", "FringeAnimTileset", "Fringe2Tileset", "Fringe2AnimTileset" });

            for (int x = 0; x <= rawMap.MaxX; x++) {
                for (int y = 0; y <= rawMap.MaxY; y++) {
                    if (rawMap.Tile[x, y] == null) {
                        rawMap.Tile[x, y] = new Tile();
                    }

                    multiRowInsert.AddRowOpening();
                    multiRowInsert.AddColumnData(mapID);
                    multiRowInsert.AddColumnData(x, y);

                    Tile tile = rawMap.Tile[x, y];

                    multiRowInsert.AddColumnData(tile.Ground, tile.GroundAnim, tile.Mask, tile.Anim, tile.Mask2, tile.M2Anim,
                        tile.Fringe, tile.FAnim, tile.Fringe2, tile.F2Anim, tile.Type, tile.Data1, tile.Data2, tile.Data3);
                    multiRowInsert.AddColumnData(tile.String1, tile.String2, tile.String3);
                    multiRowInsert.AddColumnData(tile.RDungeonMapValue, tile.GroundSet, tile.GroundAnimSet, tile.MaskSet, tile.AnimSet,
                        tile.Mask2Set, tile.M2AnimSet, tile.FringeSet, tile.FAnimSet, tile.Fringe2Set, tile.F2AnimSet);

                    multiRowInsert.AddRowClosing();
                }
            }

            database.ExecuteNonQuery(multiRowInsert.GetSqlQuery());

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static void SaveStandardMap(MySql database, string mapID, Map map) {
            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            // Save the raw map
            SaveRawMap(database, mapID, map);

            // Save extra data associated with standard maps
            database.UpdateOrInsert("map_standard_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "Instanced", map.Instanced.ToIntString())
            });

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static Map LoadStandardMap(MySql database, string mapID) {
            Map map = new Map(mapID);
            LoadRawMap(database, mapID, map);

            string query = "SELECT map_standard_data.Instanced FROM map_standard_data WHERE map_standard_data.MapID = \'" + database.VerifyValueString(mapID) + "\'";
            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null) {
                int counter = 0;
                map.Instanced = row[counter++].ValueString.ToBool();
            } else {
                throw new Exception("Standard map data not found");
            }

            return map;
        }

        public static InstancedMap LoadInstancedMap(MySql database, string mapID) {
            InstancedMap map = new InstancedMap(mapID);
            LoadMapDump(database, mapID, map);

            string query = "SELECT map_instanced_data.MapBase FROM map_instanced_data WHERE map_instanced_data.MapID = \'" + database.VerifyValueString(mapID) + "\'";
            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null) {
                int counter = 0;
                map.MapBase = Convert.ToInt32(row[counter++].Value);
            } else {
                throw new Exception("Instanced map data not found");
            }

            return map;
        }

        public static void SaveInstancedMap(MySql database, string mapID, InstancedMap map) {
            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            SaveMapDump(database, mapID, map);

            // Save extra data associated with instanced maps
            database.UpdateOrInsert("map_instanced_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "MapBase", map.MapBase.ToString())
            });

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static HouseMap LoadHouseMap(MySql database, string mapID) {
            if (IsMapIDInUse(database, mapID)) {
                HouseMap map = new HouseMap(mapID);
                LoadMapDump(database, mapID, map);

                string query = "SELECT map_house_data.Owner, map_house_data.Room, map_house_data.StartX, map_house_data.StartY FROM map_house_data WHERE map_house_data.MapID = \'" + database.VerifyValueString(mapID) + "\'";
                DataColumnCollection row = database.RetrieveRow(query);
                if (row != null) {
                    int counter = 0;
                    map.Owner = row[counter++].ValueString;
                    map.Room = Convert.ToInt32(row[counter++].Value);
                    map.StartX = Convert.ToInt32(row[counter++].Value);
                    map.StartY = Convert.ToInt32(row[counter++].Value);
                } else {
                    throw new Exception("House map data not found");
                }

                return map;
            } else {
                return null;
            }
        }

        public static void SaveHouseMap(MySql database, string mapID, HouseMap map) {
            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            SaveMapDump(database, mapID, map);

            // Save extra data associated with instanced maps
            database.UpdateOrInsert("map_house_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "Owner", map.Owner),
                database.CreateColumn(false, "Room", map.Room.ToString()),
                database.CreateColumn(false, "StartX", map.StartX.ToString()),
                database.CreateColumn(false, "StartY", map.StartY.ToString())
            });

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static RDungeonMap LoadRDungeonMap(MySql database, string mapID) {
            RDungeonMap map = new RDungeonMap(mapID);
            LoadMapDump(database, mapID, map);

            string query = "SELECT map_rdungeon_data.RDungeonIndex, map_rdungeon_data.RDungeonFloor, map_rdungeon_data.StartX, map_rdungeon_data.StartY FROM map_rdungeon_data WHERE map_rdungeon_data.MapID = \'" + database.VerifyValueString(mapID) + "\'";
            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null) {
                int counter = 0;
                map.RDungeonIndex = Convert.ToInt32(row[counter++].Value);
                map.RDungeonFloor = Convert.ToInt32(row[counter++].Value);
                map.StartX = Convert.ToInt32(row[counter++].Value);
                map.StartY = Convert.ToInt32(row[counter++].Value);
            } else {
                throw new Exception("RDungeon map data not found");
            }

            return map;
        }

        public static void SaveRDungeonMap(MySql database, string mapID, RDungeonMap map) {
            bool localTransaction = false;
            if (database.IsTransactionActive == false) {
                database.BeginTransaction();
                localTransaction = true;
            }

            SaveMapDump(database, mapID, map);

            // Save extra data associated with instanced maps
            database.UpdateOrInsert("map_rdungeon_data", new IDataColumn[] {
                database.CreateColumn(false, "MapID", mapID),
                database.CreateColumn(false, "RDungeonIndex", map.RDungeonIndex.ToString()),
                database.CreateColumn(false, "RDungeonFloor", map.RDungeonFloor.ToString()),
                database.CreateColumn(false, "StartX", map.StartX.ToString()),
                database.CreateColumn(false, "StartY", map.StartY.ToString())
            });

            if (localTransaction) {
                database.EndTransaction();
            }
        }

        public static string GetMapName(MySql database, string mapID) {
            string query = "SELECT map_data.Name FROM map_data WHERE map_data.MapID = \'" + mapID + "\'";
            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null) {
                return row[0].ValueString;
            } else {
                return null;
            }
        }

        public static void DeleteMap(MySql database, string mapID) {
            // All map data will be deleted because foreign keys in the database are linked to the map_general table
            string query = "DELETE FROM map_general WHERE MapID = \'" + mapID + "\'";
            database.ExecuteNonQuery(query);
        }

        public static bool IsMapIDInUse(MySql database, string mapID) {
            string query = "SELECT MapID FROM map_general WHERE MapID = \'" + mapID + "\'";
            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null) {
                return row["MapID"].ValueString == mapID;
            } else {
                return false;
            }
        }

    }
}
