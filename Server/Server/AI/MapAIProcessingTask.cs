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
using Server.Maps;
using Server.Network;
using PMU.Sockets;
using Server.Npcs;
using Server.Combat;

namespace Server.AI
{
    public class MapAIProcessingTask
    {
        IMap map;

        public MapAIProcessingTask(IMap map) {
            this.map = map;
        }

        public void ProcessAIThreadPoolCallback(Object state) {
            ProcessAI();
        }

        public void ProcessAI() {
            try {
                TickCount tickCount = Core.GetTickCount();

                PacketHitList packetList = null;
                PacketHitList.MethodStart(ref packetList);
                if (map.IsSaving == false) {
                    

                    // Keys/Other timed tile stuff
                    for (int x = 0; x <= map.MaxX; x++) {
                        for (int y = 0; y <= map.MaxY; y++) {
                            if (map.Tile[x, y] != null) {
                                if (tickCount.Elapsed(map.Tile[x, y].DoorTimer, 5000)) {
                                    if ((map.Tile[x, y].Type == Enums.TileType.Key || map.Tile[x, y].Type == Enums.TileType.Door) && map.Tile[x, y].DoorOpen == true) {
                                        map.Tile[x, y].DoorOpen = false;
                                        packetList.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "0"));
                                    }
                                }
                            }
                        }
                    }

                    Scripting.ScriptManager.InvokeSub("OnMapTick", map);

                    if (map.ProcessingPaused == false) {
                        
                        

                        int livingNpcs = 0;

                        for (int mapNpcSlot = 0; mapNpcSlot < Constants.MAX_MAP_NPCS; mapNpcSlot++) {
                            int npcNum = map.ActiveNpc[mapNpcSlot].Num;
                            MapNpc mapNpc = map.ActiveNpc[mapNpcSlot];

                            if (npcNum > 0) {
                                livingNpcs++;

                                Npc npc = NpcManager.Npcs[npcNum];

                                if (npc.Behavior != Enums.NpcBehavior.FullyScriptedAI || npc.AIScript == "" || npc.AIScript.ToLower() == "none") {
                                    #region Used for attacking on sight
                                    if (npc.Behavior == Enums.NpcBehavior.AttackOnSight || npc.Behavior == Enums.NpcBehavior.Guard) {
                                        foreach (Client i in map.GetClients()) {
                                            if (i.Player.MapID == map.MapID && map.ActiveNpc[mapNpcSlot].Target == null && !i.Player.Dead
                                                && i.Player.Hunted && map.Tile[i.Player.X, i.Player.Y].Type != Enums.TileType.NPCAvoid && i.Player.Hunted) {
                                                if (MovementProcessor.CanCharacterSeeCharacter(map, map.ActiveNpc[mapNpcSlot], i.Player.GetActiveRecruit())) {
                                                    if (npc.Behavior == Enums.NpcBehavior.AttackOnSight/* || i.Player.PK == true*/) {
                                                        //if (!string.IsNullOrEmpty(npc.AttackSay)) {
                                                        //    packetList.AddPacket(i, PacketBuilder.CreateChatMsg("A " + npc.Name + " : " + npc.AttackSay, Text.Grey));
                                                        //}

                                                        map.ActiveNpc[mapNpcSlot].Target = i;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Used for walking/targetting/picking up items
                                    if (npcNum > 0) {
                                        bool hasNpcWalked;

                                        if (map.ActiveNpc[mapNpcSlot].Target != null && map.ActiveNpc[mapNpcSlot].Target.IsPlaying()
                                            && map.ActiveNpc[mapNpcSlot].Target.Player.MapID == map.MapID
                                            && (map.Tile[map.ActiveNpc[mapNpcSlot].Target.Player.X, map.ActiveNpc[mapNpcSlot].Target.Player.Y].Type == Enums.TileType.NPCAvoid
                                            || !map.ActiveNpc[mapNpcSlot].Target.Player.Hunted || map.ActiveNpc[mapNpcSlot].Target.Player.Dead
                                            || !MovementProcessor.CanCharacterSeeCharacter(map, map.ActiveNpc[mapNpcSlot], map.ActiveNpc[mapNpcSlot].Target.Player.GetActiveRecruit()))) {
                                                map.ActiveNpc[mapNpcSlot].Target = null;
                                        }

                                        Client target = map.ActiveNpc[mapNpcSlot].Target;


                                        if (npc.Behavior != Enums.NpcBehavior.Shopkeeper) {
                                            if (target != null) {
                                                if (target.IsPlaying() && target.Player.MapID == map.MapID) {
                                                    hasNpcWalked = false;
                                                    int dir = Math.Rand(0, 5);

                                                    hasNpcWalked = AIProcessor.MoveNpcInDirection((Enums.Direction)dir, map, target, packetList, mapNpcSlot);
                                                    if (!hasNpcWalked) {
                                                        foreach (Enums.Direction direction in Enum.GetValues(typeof(Enums.Direction))) {
                                                            if (direction != (Enums.Direction)dir) {
                                                                if (AIProcessor.MoveNpcInDirection(direction, map, target, packetList, mapNpcSlot)) {
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (hasNpcWalked == false) {
                                                        if (map.ActiveNpc[mapNpcSlot].X - 1 == target.Player.X && map.ActiveNpc[mapNpcSlot].Y == target.Player.Y) {
                                                            if (map.ActiveNpc[mapNpcSlot].Direction != Enums.Direction.Left) {
                                                                MovementProcessor.ChangeNpcDir(packetList, map, mapNpcSlot, Enums.Direction.Left);
                                                            }
                                                            hasNpcWalked = true;
                                                        }
                                                        if (map.ActiveNpc[mapNpcSlot].X + 1 == target.Player.X && map.ActiveNpc[mapNpcSlot].Y == target.Player.Y) {
                                                            if (map.ActiveNpc[mapNpcSlot].Direction != Enums.Direction.Right) {
                                                                MovementProcessor.ChangeNpcDir(packetList, map, mapNpcSlot, Enums.Direction.Right);
                                                            }
                                                            hasNpcWalked = true;
                                                        }
                                                        if (map.ActiveNpc[mapNpcSlot].X == target.Player.X && map.ActiveNpc[mapNpcSlot].Y - 1 == target.Player.Y) {
                                                            if (map.ActiveNpc[mapNpcSlot].Direction != Enums.Direction.Up) {
                                                                MovementProcessor.ChangeNpcDir(packetList, map, mapNpcSlot, Enums.Direction.Up);
                                                            }
                                                            hasNpcWalked = true;
                                                        }
                                                        if (map.ActiveNpc[mapNpcSlot].X == target.Player.X && map.ActiveNpc[mapNpcSlot].Y + 1 == target.Player.Y) {
                                                            if (map.ActiveNpc[mapNpcSlot].Direction != Enums.Direction.Down) {
                                                                MovementProcessor.ChangeNpcDir(packetList, map, mapNpcSlot, Enums.Direction.Down);
                                                            }
                                                            hasNpcWalked = true;
                                                        }
                                                    }

                                                    if (hasNpcWalked == false) {
                                                        int val = Math.Rand(0, 2);
                                                        if (val == 1) {
                                                            val = Math.Rand(0, 4);
                                                            if (MovementProcessor.CanNpcMove(map, mapNpcSlot, (Enums.Direction)val)) {
                                                                MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                                                            }
                                                        }
                                                    }
                                                } else {
                                                    map.ActiveNpc[mapNpcSlot].Target = null;
                                                }
                                            } else {
                                                int shouldWalk = Math.Rand(0, 2);
                                                if (npc.Behavior != Enums.NpcBehavior.Friendly) {
                                                    for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                                                        if (map.ActiveItem[i].X == map.ActiveNpc[mapNpcSlot].X && map.ActiveItem[i].Y == map.ActiveNpc[mapNpcSlot].Y
                                                            && map.ActiveItem[i].Num > -1 && map.ActiveNpc[mapNpcSlot].HeldItem == null) {
                                                            map.ActiveNpc[mapNpcSlot].MapGetItem();
                                                            shouldWalk = 1;
                                                        }
                                                    }
                                                } else if (npc.Behavior == Enums.NpcBehavior.Friendly) {
                                                    foreach (Client i in map.GetClients()) {
                                                        if (i.Player.X >= map.ActiveNpc[mapNpcSlot].X - 1 && i.Player.X <= map.ActiveNpc[mapNpcSlot].X + 1 &&
                                                            i.Player.Y >= map.ActiveNpc[mapNpcSlot].Y - 1 && i.Player.Y <= map.ActiveNpc[mapNpcSlot].Y + 1) {
                                                                shouldWalk = Math.Rand(0, 10);
                                                                break;
                                                        }
                                                    }
                                                }
                                                if (shouldWalk == 0) {
                                                    shouldWalk = Math.Rand(0, 5);
                                                    if (MovementProcessor.CanNpcMove(map, mapNpcSlot, (Enums.Direction)shouldWalk)) {
                                                        MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Used for attacking players

                                    if (npcNum > 0) {
                                        Client target = map.ActiveNpc[mapNpcSlot].Target;
                                        if (target != null) {
                                            if (target.IsPlaying() && target.Player.MapID == map.MapID) {
                                                // Make sure npcs dont attack more then once a second
                                                if (Core.GetTickCount().Elapsed(map.ActiveNpc[mapNpcSlot].AttackTimer, 1000)) {

                                                    int usableMoveCount = 0;
                                                    for (int i = 0; i < mapNpc.Moves.Length; i++) {
                                                        if (mapNpc.Moves[i].MoveNum > -1 && mapNpc.Moves[i].CurrentPP > 0) {
                                                            usableMoveCount++;
                                                        }
                                                    }
                                                    if (usableMoveCount == 0) {
                                                        if (BattleProcessor.ShouldUseMove(map, mapNpc, -1)) {
                                                            mapNpc.UseMove(-1);
                                                        }
                                                    } else {
                                                        // Try to use a move, up to 50 times
                                                        for (int i = 0; i < 50; i++) {
                                                            int moveSlotToUse = Server.Math.Rand(0, 5) - 1;
                                                            if (moveSlotToUse == -1) {
                                                                // A standard attack
                                                                if (BattleProcessor.ShouldUseMove(map, mapNpc, moveSlotToUse)) {
                                                                    mapNpc.UseMove(moveSlotToUse);
                                                                    break;
                                                                }
                                                            } else {
                                                                if (mapNpc.Moves[moveSlotToUse].MoveNum > -1 && mapNpc.Moves[moveSlotToUse].CurrentPP > 0) {
                                                                    // Use a move
                                                                    if (BattleProcessor.ShouldUseMove(map, mapNpc, moveSlotToUse)) {
                                                                        mapNpc.UseMove(moveSlotToUse);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            } else {
                                                map.ActiveNpc[mapNpcSlot].Target = null;
                                            }
                                        }
                                    }

                                    #endregion

                                    if (npcNum > 0 && map.ActiveNpc[mapNpcSlot].HPChanged) {
                                        map.ActiveNpc[mapNpcSlot].SendHPToMap(packetList, map, mapNpcSlot);
                                    }
                                } else {
                                    // Scripted AI: Reimplement?
                                    //Globals.AIScriptManager.CallAIProcessSub(NpcManager.Npcs[map.Npc[x].NpcNum].AIScript, tickCount, map.MapNum, x);
                                }
                            }
                        }
                        //try and spawn something
                        if (map.NpcSpawnWait == null) {
                            map.NpcSpawnWait = Core.GetTickCount();
                        }
                        if (Core.GetTickCount().Tick > map.NpcSpawnWait.Tick) {
                            if (livingNpcs < map.MaxNpcs) {
                                map.SpawnNpc(true);
                            }
                            map.NpcSpawnWait = new TickCount(map.NpcSpawnWait.Tick + map.NpcSpawnTime * 1000);
                        }
                    }
                }
                //lock (MapManager.ActiveMapLockObject) {
                bool shouldRemove = false;
                if (map.PlayersOnMap.Count == 0 && tickCount.Elapsed(map.ActivationTime, AIProcessor.MapTTL)) {
                    if (map.IsProcessingComplete()) {
                        shouldRemove = true;

                        for (int i = 1; i < 9; i++) {

                            IMap borderingMap = MapManager.RetrieveActiveBorderingMap(map, (Enums.MapID)i);
                            if (borderingMap != null) {
                                if (borderingMap.PlayersOnMap.Count > 0) {
                                    shouldRemove = false;
                                }
                            }
                        }

                        if (shouldRemove) {
                            MapManager.rwLock.EnterWriteLock();
                            try {
                                if (map.MapType == Enums.MapType.House) {
                                    // Only save the map here when logging out - otherwise, the map is saved when the player is saved
                                    map.Save();
                                }
                                if ((map.MapType == Enums.MapType.RDungeonMap || map.MapType == Enums.MapType.Instanced)) {
                                    using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                                        if (MapManager.GetTotalPlayersOnMap(dbConnection, map.MapID) == 0) {
                                            DataManager.Maps.MapDataManager.DeleteMap(dbConnection.Database, map.MapID);
                                        } else {
                                            map.Save();
                                        }
                                    }
                                }

                                MapManager.UnsafeRemoveActiveMap(map.MapID);


                            } finally {
                                MapManager.rwLock.ExitWriteLock();
                            }
                        }
                    }
                }
                //}

                PacketHitList.MethodEnded(ref packetList);
            } catch (Exception ex) {
                Server.Exceptions.ErrorLogger.WriteToErrorLog(ex, "MapAIProcessor");
            }
        }
    }
}
