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
using Server.Maps;
using Server.Items;
using Server.Stories;
using Server.Missions;
using Server.RDungeons;
using Server.Evolutions;
using Server.Players;
using Server.Combat;
using Server.WonderMails;
using Server.Network;
using PMU.Sockets;
using Server.Players.Parties;

namespace Server.AI
{
    public class MovementProcessor
    {
        internal static void PlayerCollide(Client client, Client collidedWith, int playerCount) {
            Scripting.ScriptManager.InvokeSub("PlayerCollide", client, collidedWith, playerCount);
        }

        public static bool IsBlocked(Client client, IMap map, int x, int y) {
            switch (map.Tile[x, y].Type) {
                case Enums.TileType.Blocked:
                    return true;
                case Enums.TileType.Sign:
                    return true;
                case Enums.TileType.ScriptedSign:
                    return true;
                case Enums.TileType.LevelBlock:
                    if (client.Player.GetActiveRecruit().Level > map.Tile[x, y].Data1) {
                        return false;
                    } else {
                        return true;
                    }
                case Enums.TileType.SpriteBlock:
                    if (map.Tile[x, y].Data1 == 1) {
                        if (client.Player.GetActiveRecruit().Sprite == map.Tile[x, y].Data2 || client.Player.GetActiveRecruit().Sprite == map.Tile[x, y].Data3) {
                            return false;
                        } else {
                            return true;
                        }
                    } else if (map.Tile[x, y].Data1 == 2) {
                        if (client.Player.GetActiveRecruit().Sprite != map.Tile[x, y].Data2 && client.Player.GetActiveRecruit().Sprite != map.Tile[x, y].Data3) {
                            return false;
                        } else {
                            return true;
                        }
                    } else {
                        return true;
                    }
                case Enums.TileType.MobileBlock:
                    int mobilityList = map.Tile[x, y].Data1;
                    for (int i = 0; i < 16; i++)
                    {
                        if (mobilityList % 2 == 1 && !client.Player.GetActiveRecruit().Mobility[i])
                        {
                            return true;
                        }
                        mobilityList /= 2;
                    }
                    // use mobility
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsBlocked(IMap map, int x, int y) {
            switch (map.Tile[x, y].Type) {
                case Enums.TileType.Blocked:
                case Enums.TileType.Sign:
                case Enums.TileType.ScriptedSign:
                case Enums.TileType.LevelBlock:
                case Enums.TileType.SpriteBlock:
                case Enums.TileType.MobileBlock:
                    return true;
                default:
                    return false;
            }
        }

        public static bool WillWalkOnWarp(Client client, Enums.Direction direction) {

            switch (direction) {
                case (Enums.Direction.Up): {
                        if (client.Player.Y <= 0 && client.Player.Map.Up > 0) {
                            return true;
                        } else if (client.Player.Map.Tile[client.Player.X, client.Player.Y - 1].Type == Enums.TileType.Warp) {
                            return true;
                        }
                    }
                    break;
                case (Enums.Direction.Down): {
                        if (client.Player.Y >= client.Player.Map.MaxY && client.Player.Map.Down > 0) {
                            return true;
                        } else if (client.Player.Map.Tile[client.Player.X, client.Player.Y + 1].Type == Enums.TileType.Warp) {
                            return true;
                        }
                    }
                    break;
                case (Enums.Direction.Left): {
                        if (client.Player.X <= 0 && client.Player.Map.Left > 0) {
                            return true;
                        } else if (client.Player.Map.Tile[client.Player.X - 1, client.Player.Y].Type == Enums.TileType.Warp) {
                            return true;
                        }
                    }
                    break;
                case (Enums.Direction.Right): {
                        if (client.Player.X >= client.Player.Map.MaxX && client.Player.Map.Right > 0) {
                            return true;
                        } else if (client.Player.Map.Tile[client.Player.X + 1, client.Player.Y].Type == Enums.TileType.Warp) {
                            return true;
                        }
                    }
                    break;

            }
            return false;

        }

        internal static void ProcessMovement(Client client, Enums.Direction direction, Enums.Speed speed, bool critical) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);
            try {
                Player player = client.Player;

                IMap map = player.GetCurrentMap();

                map.ProcessingPaused = false;


                int X = player.X;
                int Y = player.Y;

                if (!NetworkManager.IsPlaying(client)) {
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (Ranks.IsDisallowed(client, Enums.Rank.Moniter) || client.Player.ProtectionOff) {
                    player.Hunted = true;
                    PacketBuilder.AppendHunted(client, hitlist);
                }

                PacketList packetListToOthers = new PacketList();

                #region Mobility Checking

                if (player.MovementLocked) {
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (player.LoadingStory) {
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (player.Dead) {
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (player.GetActiveRecruit().StatusAilment == Enums.StatusAilment.Freeze || player.GetActiveRecruit().StatusAilment == Enums.StatusAilment.Sleep) {
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;

                }

                player.Direction = direction;
                PacketBuilder.AppendPlayerDir(client, packetListToOthers);
                //PacketBuilder.AppendPlayerDirExcept(client, hitlist);

                //if (player.GetActiveRecruit().StatusAilment == Enums.StatusAilment.Paralyze && speed > Enums.Speed.Walking) {
                //    PacketBuilder.AppendOwnXY(client, hitlist);
                //    PacketHitList.MethodEnded(ref hitlist);
                //    return;

                //}

                if (player.SpeedLimit < speed && player.Map.Tile[player.X, player.Y].Type != Enums.TileType.Slippery) {
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (player.Map.Tile[player.X, player.Y].Type == Enums.TileType.Slow && (Enums.Speed)player.Map.Tile[player.X, player.Y].Data2 > speed) {
                    int mobilityList = map.Tile[player.X, player.Y].Data1;
                    for (int i = 0; i < 16; i++) {
                        if (mobilityList % 2 == 1 && !client.Player.GetActiveRecruit().Mobility[i]) {
                            if (critical) {
                                PacketBuilder.AppendPlayerLock(client, hitlist, false);
                            }
                            PacketBuilder.AppendOwnXY(client, hitlist);
                            PacketHitList.MethodEnded(ref hitlist);
                            return;
                        }
                        mobilityList /= 2;
                    }
                }

                #endregion Mobility Checking

                #region X/Y Checking
                if (map.MaxX <= 0)
                    map.MaxX = 19;
                if (map.MaxY <= 0)
                    map.MaxY = 14;

                if (player.X > map.MaxX) {
                    player.X = map.MaxX;
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }
                if (player.Y > map.MaxY) {
                    player.Y = map.MaxY;
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }
                if (player.X < 0) {
                    player.X = 0;
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }
                if (player.Y < 0) {
                    player.Y = 0;
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketBuilder.AppendOwnXY(client, hitlist);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }
                #endregion

                #region Block Checking

                switch (direction) {
                    case Enums.Direction.Up:
                        if (player.Y > 0) {
                            if (IsBlocked(client, map, player.X, player.Y - 1) == false) {
                                if (map.Tile[player.X, player.Y - 1].Type != Enums.TileType.Key && map.Tile[player.X, player.Y - 1].Type != Enums.TileType.Door) {
                                    player.Y -= 1;
                                    PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                } else {
                                    if (map.Tile[player.X, player.Y - 1].DoorOpen == true) {
                                        player.Y -= 1;
                                        PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                    } else {
                                        if (critical) {
                                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                        }
                                        PacketBuilder.AppendOwnXY(client, hitlist);
                                        PacketHitList.MethodEnded(ref hitlist);
                                        return;
                                    }
                                }
                            } else {
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketBuilder.AppendOwnXY(client, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        } else {
                            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Up)) {
                                SeamlessWorldHelper.SwitchSeamlessMaps(client, map.Up, player.X, Constants.MAX_MAP_Y);
                                PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            } else if (map.Up > 0) {
                                Messenger.PlayerWarp(client, map.Up, player.X, Constants.MAX_MAP_Y);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        }
                        break;
                    case Enums.Direction.Down:
                        if (player.Y < map.MaxY) {
                            if (IsBlocked(client, map, player.X, player.Y + 1) == false) {
                                if (map.Tile[player.X, player.Y + 1].Type != Enums.TileType.Key && map.Tile[player.X, player.Y + 1].Type != Enums.TileType.Door) {
                                    player.Y += 1;
                                    PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                } else {
                                    if (map.Tile[player.X, player.Y + 1].DoorOpen == true) {
                                        player.Y += 1;
                                        PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                    } else {
                                        if (critical) {
                                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                        }
                                        PacketBuilder.AppendOwnXY(client, hitlist);
                                        PacketHitList.MethodEnded(ref hitlist);
                                        return;
                                    }
                                }
                            } else {
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketBuilder.AppendOwnXY(client, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        } else {
                            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Down)) {
                                SeamlessWorldHelper.SwitchSeamlessMaps(client, map.Down, player.X, 0);
                                PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            } else if (map.Down > 0) {
                                Messenger.PlayerWarp(client, map.Down, player.X, 0);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        }
                        break;
                    case Enums.Direction.Left:
                        if (player.X > 0) {
                            if (IsBlocked(client, map, player.X - 1, player.Y) == false) {
                                if (map.Tile[player.X - 1, player.Y].Type != Enums.TileType.Key && map.Tile[player.X - 1, player.Y].Type != Enums.TileType.Door) {
                                    player.X -= 1;
                                    PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                } else {
                                    if (map.Tile[player.X - 1, player.Y].DoorOpen == true) {
                                        player.X -= 1;
                                        PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                    } else {
                                        if (critical) {
                                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                        }
                                        PacketBuilder.AppendOwnXY(client, hitlist);
                                        PacketHitList.MethodEnded(ref hitlist);
                                        return;
                                    }
                                }
                            } else {
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketBuilder.AppendOwnXY(client, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        } else {
                            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Left)) {
                                SeamlessWorldHelper.SwitchSeamlessMaps(client, map.Left, Constants.MAX_MAP_X, player.Y);
                                PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            } else if (map.Left > 0) {
                                Messenger.PlayerWarp(client, map.Left, Constants.MAX_MAP_X, player.Y);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        }
                        break;
                    case Enums.Direction.Right:
                        if (player.X < map.MaxX) {
                            if (IsBlocked(client, map, player.X + 1, player.Y) == false) {
                                if (map.Tile[player.X + 1, player.Y].Type != Enums.TileType.Key && map.Tile[player.X + 1, player.Y].Type != Enums.TileType.Door) {
                                    player.X += 1;
                                    PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                } else {
                                    if (map.Tile[player.X + 1, player.Y].DoorOpen == true) {
                                        player.X += 1;
                                        PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                    } else {
                                        if (critical) {
                                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                        }
                                        PacketBuilder.AppendOwnXY(client, hitlist);
                                        PacketHitList.MethodEnded(ref hitlist);
                                        return;
                                    }
                                }
                            } else {
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketBuilder.AppendOwnXY(client, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        } else {
                            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Right)) {
                                SeamlessWorldHelper.SwitchSeamlessMaps(client, map.Right, 0, player.Y);
                                PacketBuilder.AppendPlayerMove(client, hitlist, direction, speed);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            } else if (map.Right > 0) {
                                Messenger.PlayerWarp(client, map.Right, 0, player.Y);
                                if (critical) {
                                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return;
                            }
                        }
                        break;
                }
                #endregion

                player.RefreshSeenCharacters(hitlist);

                #region More Collision Checking

                //if (map.Tile[player.X, player.Y].Players.Players == null) {
                //    map.Tile[player.X, player.Y].Players.Players = new List<string>();
                //}
                //if (map.Tile[player.X, player.Y].Players.Players.Contains(client.Player.CharID) == false) {
                //    map.Tile[player.X, player.Y].Players.PlayerCount += 1;
                //    map.Tile[player.X, player.Y].Players.Players.Add(client.Player.CharID);
                //}
                //if (map.Tile[player.X, player.Y].Players.PlayerCount > 1) {
                //    map.Tile[player.X, player.Y].Players.LastPlayer = map.Tile[player.X, player.Y].Players.Players[map.Tile[player.X, player.Y].Players.Players.Count - 2];
                //    map.Tile[player.X, player.Y].Players.HasPlayer = true;
                //    PlayerCollide(client, map.Tile[player.X, player.Y].Players);
                //}
                //if (map.Tile[player.X, player.Y].Players.Players.Count == 1) {
                //    map.Tile[player.X, player.Y].Players.LastPlayer = "";
                //}

                {
                    Client lastCollision = null;
                    int collisionCounter = 0;
                    foreach (Client i in map.GetClients()) {
                        if (i != client) {
                            if (i.Player.X == client.Player.X && i.Player.Y == client.Player.Y) {
                                lastCollision = i;
                                collisionCounter++;
                            }
                        }
                    }
                    if (lastCollision != null) {
                        PlayerCollide(client, lastCollision, collisionCounter);
                    }
                }

                #endregion

                Scripting.ScriptManager.InvokeSub("OnStep", map, client.Player.GetActiveRecruit(), speed, hitlist);

                // Trigger event checking
                for (int i = 0; i < player.TriggerEvents.Count; i++) {
                    switch (player.TriggerEvents[i].Trigger) {
                        case Events.Player.TriggerEvents.TriggerEventTrigger.SteppedOnTile: {
                                if (player.TriggerEvents[i].CanInvokeTrigger()) {
                                    player.TriggerEvents[i].InvokeTrigger();
                                }
                            }
                            break;
                        case Events.Player.TriggerEvents.TriggerEventTrigger.StepCounter: {
                                ((Events.Player.TriggerEvents.StepCounterTriggerEvent)player.TriggerEvents[i]).StepsCounted++;
                                if (player.TriggerEvents[i].CanInvokeTrigger()) {
                                    player.TriggerEvents[i].InvokeTrigger();
                                }
                            }
                            break;
                    }
                }

                #region Tile Checks

                //healing tiles code
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Heal) {
                    //if (!player.PK) {
                    player.SetHP(player.GetMaxHP());
                    hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("The entire team was fully healed!", Text.BrightGreen));
                    //} else {
                    //    hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Outlaws can't use healing tiles!", Text.BrightRed));
                    //}
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                //Check for kill tile, and if so kill them
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Kill) {
                    player.SetHP(0);
                    Messenger.PlayerMsg(client, "You have fainted!", Text.BrightRed);

                    Scripting.ScriptManager.InvokeSub("OnDeath", client, Enums.KillType.Tile);

                    player.SetHP(player.GetMaxHP());
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                //check for doors
                if (player.X + 1 <= map.MaxX) {
                    if (map.Tile[player.X + 1, player.Y].Type == Enums.TileType.Door) {
                        X += 1;
                        Y = player.Y;

                        if (map.Tile[X, Y].DoorOpen == false) {
                            map.Tile[X, Y].DoorOpen = true;
                            map.Tile[X, Y].DoorTimer = Core.GetTickCount();

                            hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", X.ToString(), Y.ToString(), "1"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("door.wav"));
                        }
                    }
                }
                if (player.X - 1 <= map.MaxX && player.X - 1 > -1) {
                    if (map.Tile[player.X - 1, player.Y].Type == Enums.TileType.Door) {
                        X -= 1;
                        Y = player.Y;

                        if (map.Tile[X, Y].DoorOpen == false) {
                            map.Tile[X, Y].DoorOpen = true;
                            map.Tile[X, Y].DoorTimer = Core.GetTickCount();

                            hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", X.ToString(), Y.ToString(), "1"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("door.wav"));
                        }
                    }
                }
                if (player.Y - 1 <= map.MaxY && player.Y - 1 > -1) {
                    if (map.Tile[player.X, player.Y - 1].Type == Enums.TileType.Door) {
                        X = player.X;
                        Y -= 1;

                        if (map.Tile[X, Y].DoorOpen == false) {
                            map.Tile[X, Y].DoorOpen = true;
                            map.Tile[X, Y].DoorTimer = Core.GetTickCount();

                            hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", X.ToString(), Y.ToString(), "1"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("door.wav"));
                        }
                    }
                }
                if (player.Y + 1 <= map.MaxY) {
                    if (map.Tile[player.X, player.Y + 1].Type == Enums.TileType.Door) {
                        X = player.X;
                        Y += 1;

                        if (map.Tile[X, Y].DoorOpen == false) {
                            map.Tile[X, Y].DoorOpen = true;
                            map.Tile[X, Y].DoorTimer = Core.GetTickCount();

                            hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", X.ToString(), Y.ToString(), "1"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("door.wav"));
                        }
                    }
                }

                // Check to see if the tile is a warp tile, and if so warp them
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Warp) {
                    int mapNum = map.Tile[player.X, player.Y].Data1;
                    X = map.Tile[player.X, player.Y].Data2;
                    Y = map.Tile[player.X, player.Y].Data3;
                    bool xyWarp = false;
                    if (player.Map.MapType == Enums.MapType.Standard) {
                        if (((Map)player.Map).MapNum == mapNum) {
                            xyWarp = true;
                        }
                    } else if (player.Map.MapType == Enums.MapType.Instanced) {
                        if (((InstancedMap)player.Map).MapBase == mapNum) {
                            xyWarp = true;
                        }
                    }
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    if (xyWarp) {
                        Messenger.PlayerXYWarp(client, X, Y);
                    } else {
                        Messenger.PlayerWarp(client, mapNum, X, Y);
                    }

                    return;
                }

                // Check for key trigger open
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.KeyOpen) {
                    X = map.Tile[player.X, player.Y].Data1;
                    Y = map.Tile[player.X, player.Y].Data2;

                    if (map.Tile[X, Y].Type == Enums.TileType.Key & map.Tile[X, Y].DoorOpen == false) {
                        map.Tile[X, Y].DoorOpen = true;
                        map.Tile[X, Y].DoorTimer = Core.GetTickCount();

                        hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("mapkey", X.ToString(), Y.ToString(), "1"));
                        if (map.Tile[player.X, player.Y].String1.Trim() == "") {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("A path was opened!", Text.White));
                        } else {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(map.Tile[player.X, player.Y].String1.Trim(), Text.White));
                        }
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("key.wav"));
                        PacketHitList.MethodEnded(ref hitlist);
                        return;
                    }
                }

                // Check for shop
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Shop) {
                    if (map.Tile[player.X, player.Y].Data1 > 0) {

                        Messenger.SendShopMenu(client, map.Tile[player.X, player.Y].Data1);
                    } else {
                        Messenger.PlayerMsg(client, "There is no shop here.", Text.BrightRed);
                    }
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check for guild shop
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Guild) {
                    if (client.Player.GuildAccess > 0) {
                        //manage or view guild
                        Messenger.SendGuildMenu(client);
                    } else {
                        //register guild
                        if (client.Player.ExplorerRank >= Guilds.GuildManager.MIN_RANK) {
                            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("Guilds can be created for " + Guilds.GuildManager.CREATE_PRICE + " " + Items.ItemManager.Items[1].Name + " per founder.", Text.Yellow));
                            Messenger.SendGuildCreation(client);
                        } else {
                            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("Register a guild here.  You can create a guild when your explorer rank is " + Guilds.GuildManager.MIN_RANK + " or higher.", Text.Green));
                        }
                    }
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check for Link Shop
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.LinkShop) {
                    int priceItem = map.Tile[player.X, player.Y].Data1;
                    int priceAmount = map.Tile[player.X, player.Y].Data2;
                    bool earlierEvo = (map.Tile[player.X, player.Y].Data3 == 1);
                    Messenger.SendRecallMenu(client, earlierEvo);
                    if (priceItem > 0) {

                        if (Items.ItemManager.Items[priceItem].StackCap <= 0 && Items.ItemManager.Items[priceItem].Type != Enums.ItemType.Currency) {
                            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("Moves will be taught in exchange for a " + Items.ItemManager.Items[priceItem].Name + ".", Text.Yellow));
                        } else if (priceAmount > 0) {
                            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("Moves will be taught in exchange for " + priceAmount + " " + Items.ItemManager.Items[priceItem].Name + ".", Text.Yellow));
                        }
                    }
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check if player stepped on sprite changing tile
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.SpriteChange) {
                    if (player.GetActiveRecruit().Sprite == map.Tile[player.X, player.Y].Data1) {
                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You already have this sprite!", Text.BrightRed));
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                        PacketHitList.MethodEnded(ref hitlist);
                        return;
                    } else {
                        if (map.Tile[player.X, player.Y].Data2 == 0) {
                            hitlist.AddPacket(client, TcpPacket.CreatePacket("spritechange", "0"));
                        } else {
                            if (ItemManager.Items[map.Tile[player.X, player.Y].Data2].Type == Enums.ItemType.Currency) {
                                hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("This sprite will cost you " + map.Tile[player.X, player.Y].Data3 + " " + ItemManager.Items[map.Tile[player.X, player.Y].Data2].Name.Trim() + "!", Text.Yellow));
                            } else {
                                hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("This sprite will cost you a " + ItemManager.Items[map.Tile[player.X, player.Y].Data2].Name.Trim() + "!", Text.Yellow));
                            }
                            hitlist.AddPacket(client, TcpPacket.CreatePacket("spritechange", "1"));
                        }
                    }
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check if player stepped on notice tile
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Notice) {
                    if (map.Tile[player.X, player.Y].String1.Trim() != "") {
                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg(map.Tile[player.X, player.Y].String1.Trim(), Text.Black));
                    }
                    if (map.Tile[player.X, player.Y].String2.Trim() != "") {
                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg(map.Tile[player.X, player.Y].String2.Trim(), Text.Grey));
                    }
                    hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("sound", map.Tile[player.X, player.Y].String3));
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check if player stepped on sound tile
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Sound) {
                    hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("sound", map.Tile[player.X, player.Y].String1));
                    if (critical) {
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    }
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                //Check if player stepped on a scripted tile
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Scripted) {
                    Scripting.ScriptManager.InvokeSub("ScriptedTile", map, client.Player.GetActiveRecruit(), map.Tile[player.X, player.Y].Data1, map.Tile[player.X, player.Y].String1, map.Tile[player.X, player.Y].String2, map.Tile[player.X, player.Y].String3, hitlist);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                // Check if player stepped on Bank tile
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Bank) {
                    Messenger.OpenBank(client);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Assembly) {
                    Messenger.SendAllRecruits(client);
                    Messenger.OpenAssembly(client);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }
                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Evolution) {
                    EvolutionManager.StartEvolution(player);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (map.Tile[player.X, player.Y].Type == Enums.TileType.Story) {
                    StoryManager.PlayStory(client, map.Tile[player.X, player.Y].Data1);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (map.Tile[player.X, player.Y].Type == Enums.TileType.MissionBoard) {
                    Messenger.OpenMissionBoard(client);
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketHitList.MethodEnded(ref hitlist);
                    return;
                }

                if (map.Tile[player.X, player.Y].Type == Enums.TileType.RDungeonGoal) {
                    if (client.Player.PartyID == null) {

                        RDungeonMap rmap = map as RDungeonMap;
                        if (rmap != null) {
                            RDungeon dungeon = RDungeonManager.RDungeons[rmap.RDungeonIndex];
                            int floor = rmap.RDungeonFloor;
                            if (dungeon.Floors[floor].GoalType == Enums.RFloorGoalType.NextFloor) {
                                if (floor + 1 < dungeon.Floors.Count) {
                                    player.WarpToRDungeon(rmap.RDungeonIndex, floor + 1);
                                } else {
                                    //client.Player.dungeonIndex = -1;
                                    //client.Player.dungeonFloor = -1;
                                    hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You have completed the dungeon!", Text.Yellow));
                                    Messenger.PlayerWarp(client, Settings.Crossroads, Settings.StartX, Settings.StartY);
                                }
                            } else if (dungeon.Floors[floor].GoalType == Enums.RFloorGoalType.Map) {
                                //client.Player.dungeonIndex = -1;
                                //client.Player.dungeonFloor = -1;
                                Messenger.PlayerWarp(client, dungeon.Floors[floor].GoalMap, dungeon.Floors[floor].GoalX, dungeon.Floors[floor].GoalY);
                            } else {
                                Scripting.ScriptManager.InvokeSub("RDungeonScriptGoal", client, dungeon.Floors[floor].GoalMap, dungeon.Floors[floor].GoalX, dungeon.Floors[floor].GoalY);
                            }
                        }
                        PacketBuilder.AppendPlayerLock(client, hitlist, false);
                        PacketHitList.MethodEnded(ref hitlist);
                        return;
                    } else {
                        bool warp = true;
                        Party party = PartyManager.FindPlayerParty(client);
                        IMap sourceMap = client.Player.Map;
                        foreach (Client member in party.GetOnlineMemberClients()) {
                            if (/*!member.Player.Dead && */member.Player.MapID == client.Player.MapID && (member.Player.X != client.Player.X || member.Player.Y != client.Player.Y)) {
                                warp = false;
                            }
                        }

                        if (warp) {
                            RDungeonMap rmap = map as RDungeonMap;
                            foreach (Client member in party.GetOnlineMemberClients()) {
                                if (member.Player.Map != sourceMap) continue;
                                if (rmap != null) {
                                    RDungeon dungeon = RDungeonManager.RDungeons[rmap.RDungeonIndex];
                                    int floor = rmap.RDungeonFloor;
                                    if (dungeon.Floors[floor].GoalType == Enums.RFloorGoalType.NextFloor) {
                                        if (floor + 1 < dungeon.Floors.Count) {
                                            member.Player.WarpToRDungeon(rmap.RDungeonIndex, floor + 1);
                                        } else {
                                            //client.Player.dungeonIndex = -1;
                                            //client.Player.dungeonFloor = -1;
                                            hitlist.AddPacket(member, PacketBuilder.CreateChatMsg("You have completed the dungeon!", Text.Yellow));
                                            Messenger.PlayerWarp(member, Settings.Crossroads, Settings.StartX, Settings.StartY);
                                        }
                                    } else if (dungeon.Floors[floor].GoalType == Enums.RFloorGoalType.Map) {
                                        //client.Player.dungeonIndex = -1;
                                        //client.Player.dungeonFloor = -1;
                                        Messenger.PlayerWarp(member, dungeon.Floors[floor].GoalMap, dungeon.Floors[floor].GoalX, dungeon.Floors[floor].GoalY);
                                    } else {
                                        //client.Player.dungeonIndex = -1;
                                        //client.Player.dungeonFloor = -1;
                                        Scripting.ScriptManager.InvokeSub("RDungeonScriptGoal", member, dungeon.Floors[floor].GoalMap, dungeon.Floors[floor].GoalX, dungeon.Floors[floor].GoalY);
                                    }
                                }
                            }
                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                            PacketHitList.MethodEnded(ref hitlist);
                            return;
                        } else {
                            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("All surviving players on the floor must be on the goal in order to continue.", Text.WhiteSmoke));
                            PacketBuilder.AppendPlayerLock(client, hitlist, false);
                            PacketHitList.MethodEnded(ref hitlist);
                            return;
                        }
                    }
                }

                for (int i = client.Player.ActiveGoalPoints.Count - 1; i >= 0; i--) {
                    bool missionComplete = false;
                    if (player.X == client.Player.ActiveGoalPoints[i].GoalX && player.Y == client.Player.ActiveGoalPoints[i].GoalY) {
                        switch (player.JobList.JobList[client.Player.ActiveGoalPoints[i].JobListIndex].Mission.MissionType) {
                            case Enums.MissionType.Rescue: {
                                    missionComplete = true;
                                }
                                break;
                            case Enums.MissionType.ItemRetrieval: {
                                    int itemVal = player.HasItem(player.JobList.JobList[client.Player.ActiveGoalPoints[i].JobListIndex].Mission.Data1);
                                    if (itemVal > 0) {
                                        player.TakeItem(player.JobList.JobList[client.Player.ActiveGoalPoints[i].JobListIndex].Mission.Data1, 1);
                                        missionComplete = true;
                                    } else {
                                        missionComplete = false;
                                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You've reached the target Pokémon!  But you don't have the requested item...", Text.Grey));
                                    }
                                    //else {
                                    //    missionComplete = false;
                                    //    hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You don't have enough " + ItemManager.Items[player.ActiveMission.WonderMail.Data1].Name + "!", Text.BrightRed));
                                    //}
                                }
                                break;
                            case Enums.MissionType.Escort: {
                                    if (player.IsInTeam(-2 - client.Player.ActiveGoalPoints[i].JobListIndex)) {
                                        player.RemoveFromTeam(player.FindTeamSlot(-2 - client.Player.ActiveGoalPoints[i].JobListIndex));
                                        missionComplete = true;
                                    } else {
                                        missionComplete = false;
                                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You've reached the target Pokémon!  But you don't have the escort with you...", Text.Grey));
                                    }
                                }
                                break;
                        }
                    }
                    if (missionComplete) {
                        client.Player.HandleMissionComplete(client.Player.ActiveGoalPoints[i].JobListIndex);
                        client.Player.ActiveGoalPoints.RemoveAt(i);
                        break;
                    }
                }

                //if (player.ActiveMission != null) {
                //    if (Generator.IsGoalMap(player.ActiveMission.WonderMail, map) && player.X == player.ActiveMission.GoalX && player.Y == player.ActiveMission.GoalY) {
                //        bool missionComplete = false;
                //        switch (player.ActiveMission.WonderMail.MissionType) {
                //            case Enums.MissionType.Rescue: {
                //                    missionComplete = true;
                //                }
                //                break;
                //            case Enums.MissionType.ItemRetrieval: {
                //                    int itemVal = player.HasItem(player.ActiveMission.WonderMail.Data1);
                //                    if (itemVal > 0 && itemVal >= player.ActiveMission.WonderMail.Data2) {
                //                        player.TakeItem(player.ActiveMission.WonderMail.Data2, player.ActiveMission.WonderMail.Data2);
                //                        missionComplete = true;
                //                    } else {
                //                        missionComplete = false;
                //                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You don't have enough " + ItemManager.Items[player.ActiveMission.WonderMail.Data1].Name + "!", Text.BrightRed));
                //                    }
                //                }
                //                break;
                //            case Enums.MissionType.Escort: {
                //                    if (player.IsInTeam(-2)) {
                //                        player.RemoveFromTeam(player.FindTeamSlot(-2), false);
                //                        missionComplete = true;
                //                    } else {
                //                        missionComplete = false;
                //                    }
                //                }
                //                break;
                //        }
                //        if (missionComplete) {
                //            client.Player.HandleMissionComplete();
                //        }
                //        PacketHitList.MethodEnded(ref hitlist);
                //        return;
                //    }
                //} else if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].MissionType == Enums.MissionType.ItemRetrieval) {
                //    if (mapNum == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data1 && player.mX == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data2 && player.mY == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data3) {
                //        int itemVal = player.HasItem(Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data4);
                //        if (itemVal > 0 && itemVal >= Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data5) {
                //            player.TakeItem(Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data4, Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data5);
                //            if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].EndStory != 0) {
                //                Program.ClassMan.mStories.PlayStory(client, Program.ClassMan.mMissions.mMissions[player.mActiveMission].EndStory - 1);
                //            } else {
                //                Messenger.PlayerMsg(client, "You have completed the mission!", Text.Yellow);
                //            }
                //            if (player.mMissions.ContainsKey(player.mActiveMission)) {
                //                player.mMissions[player.mActiveMission] = true;
                //            } else {
                //                player.mMissions.Add(player.mActiveMission, true);
                //            }
                //            CXmlAttributes attrib = new CXmlAttributes();
                //            attrib.Add("completed", "True");
                //            player.mMissionsXML.SaveNode("mission" + player.mActiveMission, "MissionData", attrib);
                //            if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardItem != 0) {
                //                player.GiveItem(Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardItem, Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardCount);
                //            }
                //            if (Globals.Scripting == true) {
                //                Program.ClassMan.mScripts.InvokeSub("OnMissionComplete", client, player.mActiveMission);
                //            }
                //            player.mActiveMission = -1;
                //            Messenger.SendDataTo(client, "MISSIONCOMPLETE" + TcpManager.SEP_CHAR + TcpManager.END_CHAR);
                //        } else {
                //            Messenger.PlayerMsg(client, "You dont have enough " + ItemManager.Items[Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data4].Name.Trim() + "!", Text.BrightRed);
                //        }
                //    }
                //}
                //    } else if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].MissionType == Enums.MissionType.Escort) {
                //        if (mapNum == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data1 && player.mX == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data2 && player.mY == Program.ClassMan.mMissions.mMissions[player.mActiveMission].Data3) {
                //            if (player.IsInTeam(-2)) {
                //                if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].EndStory != 0) {
                //                    Program.ClassMan.mStories.PlayStory(client, Program.ClassMan.mMissions.mMissions[player.mActiveMission].EndStory - 1);
                //                } else {
                //                    Messenger.PlayerMsg(client, "You have completed the mission!", Text.Yellow);
                //                }
                //                if (player.mMissions.ContainsKey(player.mActiveMission)) {
                //                    player.mMissions[player.mActiveMission] = true;
                //                } else {
                //                    player.mMissions.Add(player.mActiveMission, true);
                //                }
                //                CXmlAttributes attrib = new CXmlAttributes();
                //                attrib.Add("completed", "True");
                //                player.mMissionsXML.SaveNode("mission" + player.mActiveMission, "MissionData", attrib);
                //                if (Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardItem != 0) {
                //                    player.GiveItem(Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardItem, Program.ClassMan.mMissions.mMissions[player.mActiveMission].RewardCount);
                //                }
                //                if (Globals.Scripting == true) {
                //                    Program.ClassMan.mScripts.InvokeSub("OnMissionComplete", client, player.mActiveMission);
                //                }
                //                player.mActiveMission = -1;
                //                Messenger.SendDataTo(client, "MISSIONCOMPLETE" + TcpManager.SEP_CHAR + TcpManager.END_CHAR);
                //                player.RemoveFromTeam(player.FindTeamSlot(-2));
                //            }
                //        }
                //    }
                //}
                //}
                #endregion

                if (critical) {
                    PacketBuilder.AppendPlayerLock(client, hitlist, false);
                    PacketBuilder.AppendOwnXY(client, hitlist);
                }

            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex, "Player movement processing, Index: " + client.Player.CharID + " Direction: " + direction.ToString() + " Speed: " + speed.ToString() + " Map: " + client.Player.MapID + " X: " + client.Player.X.ToString() + " Y: " + client.Player.Y.ToString());
            }
            PacketHitList.MethodEnded(ref hitlist);
        }


        public static bool CanNpcMove(IMap map, int mapNpcNum, Enums.Direction direction) {
            // Check for subscript out of range
            if (mapNpcNum < 0 || mapNpcNum > Constants.MAX_MAP_NPCS || direction < Enums.Direction.Up || direction > Enums.Direction.Right) {
                return false;
            }

            //check movement-limiting status
            if (map.ActiveNpc[mapNpcNum].StatusAilment == Enums.StatusAilment.Freeze || map.ActiveNpc[mapNpcNum].StatusAilment == Enums.StatusAilment.Sleep) {
                return false;
            }

            //check attack cooldown
            if (map.ActiveNpc[mapNpcNum].AttackTimer.Tick > Core.GetTickCount().Tick) {
                return false;
            }

            //scramble intended movement if comfused
            if (map.ActiveNpc[mapNpcNum].Confused) {
                direction = (Enums.Direction)Math.Rand(0, 4);

            }

            int X = map.ActiveNpc[mapNpcNum].X;
            int Y = map.ActiveNpc[mapNpcNum].Y;

            switch (direction) {
                case Enums.Direction.Up:
                    // Check to make sure not outside of boundries
                    if (Y > 0) {
                        Y--;
                    } else {
                        return false;
                    }
                    break;
                case Enums.Direction.Down:
                    // Check to make sure not outside of boundries
                    if (Y < map.MaxY) {
                        Y++;
                    } else {
                        return false;
                    }
                    break;
                case Enums.Direction.Left:
                    // Check to make sure not outside of boundries
                    if (X > 0) {
                        X--;
                    } else {
                        return false;
                    }
                    break;
                case Enums.Direction.Right:
                    // Check to make sure not outside of boundries
                    if (X < map.MaxX) {
                        X++;
                    } else {
                        return false;
                    }
                    break;
            }

            // Check to make sure that the tile is walkable
            if (map.Tile[X, Y].Type == Enums.TileType.MobileBlock || map.Tile[X, Y].Type == Enums.TileType.Slippery || map.Tile[X, Y].Type == Enums.TileType.Slow) {
                int mobilityList = map.Tile[X, Y].Data1;
                for (int i = 0; i < 16; i++) {
                    if (mobilityList % 2 == 1 && !map.ActiveNpc[mapNpcNum].Mobility[i]) {
                        return false;
                    }
                    mobilityList /= 2;
                }
            } else if (map.Tile[X, Y].Type != Enums.TileType.Hallway && map.Tile[X, Y].Type != Enums.TileType.Walkable && map.Tile[X, Y].Type != Enums.TileType.Item && map.Tile[X, Y].Type != Enums.TileType.Scripted) {
                return false;
            }

            // Check to make sure that there is not a player in the way
            foreach (Client i in map.GetClients()) {
                if ((i.Player.X == X) && (i.Player.Y == Y)) {
                    return false;
                }
            }

            // Check to make sure that there is not another npc in the way
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if ((i != mapNpcNum) && (map.ActiveNpc[i].Num > 0) && (map.ActiveNpc[i].X == X) && (map.ActiveNpc[i].Y == Y)) {
                    return false;
                }
            }

            //change direction
            map.ActiveNpc[mapNpcNum].Direction = direction;

            return true;
        }

        public static void NpcMove(PacketHitList packetList, IMap map, int mapNpcNum, Enums.Direction direction, Enums.Speed movement) {
            // Check for subscript out of range
            if (mapNpcNum < 0 || mapNpcNum > Constants.MAX_MAP_NPCS || direction < Enums.Direction.Up || direction > Enums.Direction.Right || movement < Enums.Speed.Standing || movement > Enums.Speed.Running) {
                return;
            }





            //if (map.ActiveNpc[mapNpcNum].StatusAilment == Enums.StatusAilment.Paralyze && movement > Enums.Speed.Walking) {
            //    movement = Enums.Speed.Walking;
            //}

            if (map.ActiveNpc[mapNpcNum].SpeedLimit < movement) {
                movement = map.ActiveNpc[mapNpcNum].SpeedLimit;
            }

            if (movement == Enums.Speed.Standing) return;

            int dataByte = 0;
            BytePacker packer = new BytePacker();
            packer.AddItem(Constants.MAX_MAP_NPCS, mapNpcNum);
            packer.AddItem(4, (int)direction);
            packer.AddItem(7, (int)movement);
            dataByte = packer.PackItems();

            switch (direction) {
                case Enums.Direction.Up:
                    map.ActiveNpc[mapNpcNum].Y--;
                    break;
                case Enums.Direction.Down:
                    map.ActiveNpc[mapNpcNum].Y++;
                    break;
                case Enums.Direction.Left:
                    map.ActiveNpc[mapNpcNum].X--;
                    break;
                case Enums.Direction.Right:
                    map.ActiveNpc[mapNpcNum].X++;
                    break;
            }
            //packetList.AddPacketToMap(map, TcpPacket.CreatePacket("nm", dataByte));
            PacketBuilder.AppendNpcMove(map, map.ActiveNpc[mapNpcNum], packetList, dataByte);

            map.ActiveNpc[mapNpcNum].AttackTimer = new TickCount(map.ActiveNpc[mapNpcNum].AttackTimer.Tick + 1000 * DetermineSpeed(movement) / 30 + 1);

            //map.ActiveNpc[mapNpcNum].HPStepCounter++;
            //if (map.ActiveNpc[mapNpcNum].HPStepCounter >= 5) {
            //    map.ActiveNpc[mapNpcNum].HPStepCounter = 0;

            Scripting.ScriptManager.InvokeSub("OnStep", map, map.ActiveNpc[mapNpcNum], movement, packetList);

            //}
            //Npcs can step on scripted tiles (which means TRAPS)
            if (map.Tile[map.ActiveNpc[mapNpcNum].X, map.ActiveNpc[mapNpcNum].Y].Type == Enums.TileType.Scripted) {
                Scripting.ScriptManager.InvokeSub("ScriptedTile", map, map.ActiveNpc[mapNpcNum], map.Tile[map.ActiveNpc[mapNpcNum].X, map.ActiveNpc[mapNpcNum].Y].Data1, map.Tile[map.ActiveNpc[mapNpcNum].X, map.ActiveNpc[mapNpcNum].Y].String1, map.Tile[map.ActiveNpc[mapNpcNum].X, map.ActiveNpc[mapNpcNum].Y].String2, map.Tile[map.ActiveNpc[mapNpcNum].X, map.ActiveNpc[mapNpcNum].Y].String3, packetList);
                return;
            }



        }

        public static void ChangeNpcDir(PacketHitList packetList, IMap map, int mapNpcNum, Enums.Direction direction) {
            // Check for subscript out of range
            if (mapNpcNum < 0 || mapNpcNum > Constants.MAX_MAP_NPCS || direction < Enums.Direction.Up || direction > Enums.Direction.Right) {
                return;
            }
            MapNpc mapNpc = map.ActiveNpc[mapNpcNum];
            mapNpc.Direction = direction;
            packetList.AddPacketToSurroundingPlayers(map, PacketBuilder.CreateNpcDir(mapNpc));
            //packetList.AddPacketToMap(map, TcpPacket.CreatePacket("npcdir", MapNpcNum.ToString(), ((int)Direction).ToString()));
        }

        public static int DetermineSpeed(Enums.Speed speed) {
            switch (speed) {
                case (Enums.Speed.Standing): {
                        return 0;
                    }
                case (Enums.Speed.SuperSlow): {
                        return 2;
                    }
                case (Enums.Speed.Slow): {
                        return 3;
                    }
                case (Enums.Speed.Walking): {
                        return 4;
                    }
                case (Enums.Speed.Running): {
                        return 8;
                    }
                case (Enums.Speed.Fast): {
                        return 16;
                    }
                case (Enums.Speed.SuperFast): {
                        return 24;
                    }
                default: {
                        return 4;
                    }
            }
        }

        public static bool CanCharacterIdentifyCharacter(IMap viewerMap, ICharacter viewer, ICharacter target) {
            if (!CanCharacterIdentifyDestination(viewerMap, viewer, target.X, target.Y)) return false;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                if (target.MapID != viewerMap.MapID) return false;
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                if (((MapNpc)target).Num <= 0) return false;
            }

            return (bool)Scripting.ScriptManager.InvokeFunction("ScriptedCharacterIdentifyCharacter", viewerMap, viewer, target);
        }

        public static bool CanCharacterSeeCharacter(IMap viewerMap, ICharacter viewer, ICharacter target) {
            if (!CanCharacterSeeDestination(viewerMap, viewer, target.X, target.Y)) return false;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                if (target.MapID != viewerMap.MapID) return false;
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                if (((MapNpc)target).Num <= 0) return false;
            }

            return (bool)Scripting.ScriptManager.InvokeFunction("ScriptedCharacterSeeCharacter", viewerMap, viewer, target);
        }

        public static bool WillCharacterSeeCharacter(IMap viewerMap, ICharacter viewer, IMap targetMap, Enums.MapID targetMapID, ICharacter target) {
            if (!WillCharacterSeeDestination(viewerMap, viewer, targetMap, targetMapID, target.X, target.Y)) return false;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                //if (target.MapID != viewerMap.MapID) return false;
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                if (((MapNpc)target).Num <= 0) return false;
            }

            return (bool)Scripting.ScriptManager.InvokeFunction("ScriptedWillCharacterSee", viewerMap, viewer, target);
        }

        public static bool CanCharacterSeeDestination(IMap map, ICharacter viewer, int targetX, int targetY) {
            int viewerX = viewer.X;
            int viewerY = viewer.Y;

            //adjust for screen endings
            if (viewerX < 10) viewerX = 10;
            if (viewerX > map.MaxX - 9) viewerX = map.MaxX - 9;
            if (viewerY < 7) viewerY = 7;
            if (viewerY > map.MaxY - 7) viewerY = map.MaxY - 7;

            //check to see if the target would be in the viewer's screen
            if (targetX > viewerX + 9) return false;
            if (targetX < viewerX - 10) return false;
            if (targetY > viewerY + 7) return false;
            if (targetY < viewerY - 7) return false;

            int darkness;
            if (viewer.Darkness > -2) {
                darkness = viewer.Darkness;
            } else {
                darkness = map.Darkness;
            }

            if (darkness > -1) {
                int distance = (int)System.Math.Floor(2 * System.Math.Sqrt(System.Math.Pow(viewer.X - targetX, 2) + System.Math.Pow(viewer.Y - targetY, 2)));
                if (distance > darkness - 1) return false;
            }

            return true;
        }

        public static bool WillCharacterSeeDestination(IMap map, ICharacter viewer, IMap targetMap, Enums.MapID targetMapID, int targetX, int targetY) {//checks to see if it's one tile beyond sight
            //int viewerX = viewer.X;
            //int viewerY = viewer.Y;

            ////adjust for screen endings
            //if (viewerX < 10) viewerX = 10;
            //if (viewerX > map.MaxX - 9) viewerX = map.MaxX - 9;
            //if (viewerY < 7) viewerY = 7;
            //if (viewerY > map.MaxY - 7) viewerY = map.MaxY - 7;

            ////check to see if the target would be in the viewer's screen
            //if (targetX > viewerX + 10) return false;
            //if (targetX < viewerX - 11) return false;
            //if (targetY > viewerY + 8) return false;
            //if (targetY < viewerY - 8) return false;

            //if (map.Darkness > -1) {
            //    int distance = (int)System.Math.Floor(System.Math.Sqrt(System.Math.Pow(viewer.X - targetX, 2) + System.Math.Pow(viewer.Y - targetY, 2)));
            //    if (distance * 2 > map.Darkness + 1) return false;
            //}

            //return true;

            int upDistance = 0, downDistance = 0, leftDistance = 0, rightDistance = 0;

            switch (viewer.Direction) {
                case Enums.Direction.Up: {
                        upDistance = 9;
                        downDistance = 1;
                        leftDistance = 2;
                        rightDistance = 2;
                    }
                    break;
                case Enums.Direction.Down: {
                        upDistance = 1;
                        downDistance = 9;
                        leftDistance = 2;
                        rightDistance = 2;
                    }
                    break;
                case Enums.Direction.Left: {
                        upDistance = 2;
                        downDistance = 2;
                        leftDistance = 9;
                        rightDistance = 1;
                    }
                    break;
                case Enums.Direction.Right: {
                        upDistance = 2;
                        downDistance = 2;
                        leftDistance = 1;
                        rightDistance = 9;
                    }
                    break;
            }


            int darkness;
            if (viewer.Darkness > -2) {
                darkness = viewer.Darkness;
            } else {
                darkness = map.Darkness;
            }

            if (darkness > -1) {
                return SeamlessWorldHelper.IsInSight(map, viewer.X, viewer.Y, targetMap, targetMapID, targetX, targetY, darkness / 2 + leftDistance, darkness / 2 + rightDistance, darkness / 2 + upDistance, darkness / 2 + downDistance);
            } else {
                return SeamlessWorldHelper.IsInSight(map, viewer.X, viewer.Y, targetMap, targetMapID, targetX, targetY, 10 + leftDistance, 10 + rightDistance, 7 + upDistance, 8 + downDistance);
            }
        }

        public static bool CanCharacterIdentifyDestination(IMap map, ICharacter viewer, int targetX, int targetY) {//checks to see if it's one tile beyond sight
            int viewerX = viewer.X;
            int viewerY = viewer.Y;

            //adjust for screen endings
            if (viewerX < 10) viewerX = 10;
            if (viewerX > map.MaxX - 9) viewerX = map.MaxX - 9;
            if (viewerY < 7) viewerY = 7;
            if (viewerY > map.MaxY - 7) viewerY = map.MaxY - 7;

            //check to see if the target would be in the viewer's screen
            if (targetX > viewerX + 10) return false;
            if (targetX < viewerX - 11) return false;
            if (targetY > viewerY + 8) return false;
            if (targetY < viewerY - 8) return false;


            int darkness;
            if (viewer.Darkness > -2) {
                darkness = viewer.Darkness;
            } else {
                darkness = map.Darkness;
            }

            if (darkness > -1) {
                int distance = (int)System.Math.Ceiling(2 * System.Math.Sqrt(System.Math.Pow(viewer.X - targetX, 2) + System.Math.Pow(viewer.Y - targetY, 2)));
                if (distance > darkness - 1) return false;
            }

            return true;
        }

        //public static void NpcMove(int index, int mapNpcNum, Enums.Direction direction, Enums.Speed movement) {
        //    // Check for subscript out of range
        //    if (mapNpcNum < 0 | mapNpcNum > Constants.MAX_MAP_NPCS || direction < Enums.Direction.Up || direction > Enums.Direction.Right || movement < Enums.Speed.Standing || movement > Enums.Speed.Running) {
        //        return;
        //    }

        //    Map map = PlayerManager.Players[index].GetCurrentMap();

        //    map.ActiveNpc[mapNpcNum].Direction = direction;

        //    switch (direction) {
        //        case Enums.Direction.Up:
        //            map.ActiveNpc[mapNpcNum].Y--;
        //            Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("npcmove", mapNpcNum.ToString(), map.ActiveNpc[mapNpcNum].X.ToString(), map.ActiveNpc[mapNpcNum].Y.ToString(), ((int)map.ActiveNpc[mapNpcNum].Direction).ToString(), ((int)movement).ToString()));
        //            break;
        //        case Enums.Direction.Down:
        //            map.ActiveNpc[mapNpcNum].Y++;
        //            Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("npcmove", mapNpcNum.ToString(), map.ActiveNpc[mapNpcNum].X.ToString(), map.ActiveNpc[mapNpcNum].Y.ToString(), ((int)map.ActiveNpc[mapNpcNum].Direction).ToString(), ((int)movement).ToString()));
        //            break;
        //        case Enums.Direction.Left:
        //            map.ActiveNpc[mapNpcNum].X--;
        //            Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("npcmove", mapNpcNum.ToString(), map.ActiveNpc[mapNpcNum].X.ToString(), map.ActiveNpc[mapNpcNum].Y.ToString(), ((int)map.ActiveNpc[mapNpcNum].Direction).ToString(), ((int)movement).ToString()));
        //            break;
        //        case Enums.Direction.Right:
        //            map.ActiveNpc[mapNpcNum].X++;
        //            Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("npcmove", mapNpcNum.ToString(), map.ActiveNpc[mapNpcNum].X.ToString(), map.ActiveNpc[mapNpcNum].Y.ToString(), ((int)map.ActiveNpc[mapNpcNum].Direction).ToString(), ((int)movement).ToString()));
        //            break;
        //    }
        //}

        //public static void ChangeNpcDir(int index, int MapNpcNum, Enums.Direction Direction) {
        //    // Check for subscript out of range
        //    if (MapNpcNum < 0 | MapNpcNum > Constants.MAX_MAP_NPCS | Direction < Enums.Direction.Up | Direction > Enums.Direction.Right) {
        //        return;
        //    }

        //    Map map = PlayerManager.Players[index].GetCurrentMap();

        //    map.ActiveNpc[MapNpcNum].Direction = Direction;
        //    Messenger.SendDataToPlayerMap(index, TcpPacket.CreatePacket("npcdir", MapNpcNum.ToString(), ((int)Direction).ToString()));
        //}
    }
}
