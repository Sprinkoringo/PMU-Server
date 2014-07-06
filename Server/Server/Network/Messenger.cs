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


namespace Server.Network
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Xml;

    using Items;

    using Maps;

    using Missions;

    using Moves;

    using Npcs;

    using Players;

    using Scripting;

    using Shops;

    using Stories;

    using WonderMails;
    using RDungeons;
    using Dungeons;
    using Evolutions;
    using Emoticons;
    using PMU.Sockets;
    using Server.Players.Parties;
    using PMU.Core;
    using DataManager.Players;
    using Server.Database;

    public class Messenger
    {
        #region Method

        public static void AdminMsg(string Msg, Color Color) {
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying() && Ranks.IsAllowed(i, Enums.Rank.Moniter)) {
                    SendDataTo(i, TcpPacket.CreatePacket("msg", Msg, Color.ToArgb().ToString()));
                }
            }
        }

        public static void AlertMsg(Client client, string msg) {
            SendDataTo(client, TcpPacket.CreatePacket("alertmsg", msg));
            client.CloseConnection();
        }

        public static void AskQuestion(Client client, string questionID, string text, int storySprite) {
            AskQuestion(client, questionID, text, storySprite, new string[] { "Yes", "No" });
        }

        public static void AskQuestion(Client client, string questionID, string text, int storySprite, string[] choices) {
            client.Player.QuestionID = questionID;
            TcpPacket packet = new TcpPacket("askquestion");
            packet.AppendParameters(text, storySprite.ToString(), questionID);
            packet.AppendParameter(choices.Length);
            foreach (string i in choices) {
                packet.AppendParameter(i);
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
            SendHunted(client);
        }

        public static void BattleMsg(Client client, string msg, Color color) {
            SendDataTo(client, TcpPacket.CreatePacket("battlemsg", msg, color.ToArgb().ToString()));
        }

        //earlier concept before packethitlist was created
        //public static void MultiBattleMsg(Client client, List<Combat.BattleMessage> msg) {
        //    TcpPacket packet = TcpPacket.CreatePacket("multibattlemsg");

        //    foreach (Combat.BattleMessage i in msg) {
        //        packet.AppendParameters(i.Message, i.Color.ToArgb().ToString());
        //    }
        //    SendDataTo(client, packet);
        //}



        public static void GlobalMsg(string msg, Color color) {
            SendDataToAll(TcpPacket.CreatePacket("msg", msg, color.ToArgb().ToString()));
        }

        public static void HackingAttempt(Client client, string reason) {
            AdminMsg("[Hacking Attempt] Name: " + client.Player.AccountName + "/" + client.Player.Name + " Reason: " + reason, Text.Cyan);
        }

        public static void MapMsg(string mapID, string Msg, Color Color) {
            SendDataToMap(mapID, TcpPacket.CreatePacket("msg", Msg, Color.ToArgb().ToString()));
        }

        public static void DisplaySpeechBubble(string mapID, string Msg, Client client) {
            SendDataToMap(mapID, TcpPacket.CreatePacket("speechbubble", Msg, client.ConnectionID.ToString()));
        }

        public static void PartyMsg(Party party, string msg, Color color) {
            foreach (Client i in party.GetOnlineMemberClients()) {
                if (i.IsPlaying()) {
                    Messenger.PlayerMsg(i, msg, color);
                }
            }
        }

        public static void RefreshMap(Client client) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);
            PacketBuilder.AppendMapData(client, hitlist, client.Player.Map, Enums.MapID.TempActive);
            PacketBuilder.AppendMapDone(client, hitlist);
            hitlist.AddPacket(client, PacketBuilder.CreateMyPlayerData(client));
            PacketBuilder.AppendActiveTeam(client, hitlist);
            PacketBuilder.AppendMapItems(client, hitlist, client.Player.Map, false);
            PacketBuilder.AppendMapNpcs(client, hitlist, client.Player.Map, false);
            PacketBuilder.AppendWeather(client, hitlist, client.Player.Map);
            //PacketBuilder.AppendMapDone(client, hitlist);
            hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("Map refreshed.", Text.WhiteSmoke));
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void NeedMapCheck(Client client, bool val) {
            Player player = client.Player;
            player.MovementLocked = true;

            IMap map = player.Map;

            PacketHitList packetList = null;
            PacketHitList.MethodStart(ref packetList);
            // Get yes/no value
            if (val == true) {
                PacketBuilder.AppendMapData(client, packetList, map, Enums.MapID.TempActive);
            }
            PacketBuilder.AppendMapItems(client, packetList, map, true);
            PacketBuilder.AppendPlayerData(client, packetList);
            PacketBuilder.AppendMapDone(client, packetList);
            PacketBuilder.AppendMapNpcs(client, packetList, map, true);
            PacketBuilder.AppendJoinMap(client, packetList);
            PacketBuilder.AppendWeather(client, packetList, map);

            Scripting.ScriptManager.InvokeSub("OnMapLoaded", client, map, packetList);

            // Send map npc HP, confusion, status ailment, volatile status
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (map.ActiveNpc[i].Num > 0) {
                    PacketBuilder.AppendNpcHP(map, packetList, i);

                    //PacketBuilder.AppendNpcConfusion(map, packetList, i);
                    PacketBuilder.AppendNpcStatusAilment(map.ActiveNpc[i], packetList);
                    PacketBuilder.AppendNpcVolatileStatus(map, packetList, i);
                }
            }

            PacketHitList.MethodEnded(ref packetList);
            player.GettingMap = false;

            //SendWornEquipmentFromMap(client);
            player.MovementLocked = false;

        }

        public static void NeedMapCheck(Client client, bool[] results) {
            Player player = client.Player;
            //player.MovementLocked = true;

            IMap map = player.Map;

            PacketHitList packetList = null;
            PacketHitList.MethodStart(ref packetList);

            int[] mapIDs = new int[8] { map.Up, map.Down, map.Left, map.Right, 0, 0, 0, 0 };

            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Up)) {
                if (MapManager.IsBorderingMapLoaded(map, Enums.MapID.Up)) {
                    IMap mapData = MapManager.RetrieveBorderingMap(map, Enums.MapID.Up, true);
                    if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Left)) {
                        mapIDs[4] = mapData.Left;
                    }
                    if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Right)) {
                        mapIDs[6] = mapData.Right;
                    }
                }
            } else {
                mapIDs[0] = 0;
            }
            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Down)) {
                if (MapManager.IsBorderingMapLoaded(map, Enums.MapID.Down)) {
                    IMap mapData = MapManager.RetrieveBorderingMap(map, Enums.MapID.Down, true);
                    if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Left)) {
                        mapIDs[5] = mapData.Left;
                    }
                    if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Right)) {
                        mapIDs[7] = mapData.Right;
                    }
                }
            } else {
                mapIDs[1] = 0;
            }

            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Left) == false) {
                mapIDs[2] = 0;
            }
            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Right) == false) {
                mapIDs[3] = 0;
            }

            for (int i = 0; i < results.Length; i++) {
                if (results[i] == false) {
                    if (i == 0) {
                        PacketBuilder.AppendMapData(client, packetList, map, (Enums.MapID)(i + 9));
                    } else {
                        if (mapIDs[i - 1] > 0) {
                            IMap mapData = MapManager.RetrieveMap(mapIDs[i - 1]);
                            PacketBuilder.AppendMapData(client, packetList, mapData, (Enums.MapID)(i + 9));
                        }
                    }
                }
            }

            bool isSeamless = false;
            for (int i = 0; i < mapIDs.Length; i++) {
                if (mapIDs[i] > 0) {
                    isSeamless = true;
                    break;
                }
            }

            PacketBuilder.AppendMapItems(client, packetList, map, true);
            PacketBuilder.AppendMapNpcs(client, packetList, map, true);

            if (isSeamless) {
                // Append bordering map npcs/items
                for (int i = 0; i < mapIDs.Length; i++) {
                    if (mapIDs[i] > 0) {
                        IMap borderingMap = MapManager.RetrieveMap(mapIDs[i]);
                        // Append items and npcs
                        PacketBuilder.AppendMapItems(client, packetList, borderingMap, true);
                        PacketBuilder.AppendMapNpcs(client, packetList, borderingMap, true);
                    }
                }
            }

            if (!isSeamless) {
                PacketBuilder.AppendPlayerData(client, packetList);
            } else {
                PacketBuilder.AppendPlayerData(client, false, packetList);
            }
            PacketBuilder.AppendMapDone(client, packetList);
            PacketBuilder.AppendJoinMap(client, packetList);
            PacketBuilder.AppendWeather(client, packetList, map);

            Scripting.ScriptManager.InvokeSub("OnMapLoaded", client, map, packetList);

            // Send map npc HP, confusion, status ailment, volatile status
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (map.ActiveNpc[i].Num > 0) {
                    PacketBuilder.AppendNpcHP(map, packetList, i);

                    //PacketBuilder.AppendNpcConfusion(map, packetList, i);
                    PacketBuilder.AppendNpcStatusAilment(map.ActiveNpc[i], packetList);
                    PacketBuilder.AppendNpcVolatileStatus(map, packetList, i);
                }
            }

            PacketHitList.MethodEnded(ref packetList);
            player.GettingMap = false;

            //SendWornEquipmentFromMap(client);
            //player.MovementLocked = false;
        }

        public static void OpenAssembly(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("openassembly"));
        }

        public static void OpenBank(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("openbank"));
        }

        // 
        //public static void OpenMissionBoard(Client client) {
        //    //int incompleteMission = MissionManager.FindIncompleteMission(client);
        //    //string TcpPacket = "OPENMISSIONBOARD" + Tcp.TcpManager.SEP_CHAR + incompleteMission.ToString() + Tcp.TcpManager.SEP_CHAR;
        //    //if (incompleteMission != -1) {
        //    //    TcpPacket += ResourceManager.MissionInfo[MissionManager.Missions[incompleteMission].MissionInfoIndex].Name + Tcp.TcpManager.SEP_CHAR +
        //    //        MissionManager.Missions[incompleteMission].Difficulty + Tcp.TcpManager.SEP_CHAR + ResourceManager.PokemonInfo[MissionManager.Missions[incompleteMission].CreatorInfoIndex].Name +
        //    //        Tcp.TcpManager.SEP_CHAR + ResourceManager.PokemonInfo[MissionManager.Missions[incompleteMission].CreatorInfoIndex].StorySprite.ToString();
        //    //}
        //    //TcpPacket += Tcp.TcpManager.SEP_CHAR + Tcp.TcpManager.END_CHAR;
        //    //SendDataTo(client, TcpPacket);
        //    if (Mi
        //    int slot = Server.Math.Rand(0, 4);
        //    MissionBoard board = client.Player.MissionBoard;
        //    if (board.ActiveMissions[slot] == null || string.IsNullOrEmpty(board.ActiveMissions[slot].RawJob.Code)) {
        //        bool noMissions = false;
        //        for (int i = 0; i < board.ActiveMissions.Length; i++) {
        //            if (board.ActiveMissions[i] != null && !string.IsNullOrEmpty(board.ActiveMissions[i].RawJob.Code)) {
        //                slot = i;
        //                OpenMissionBoard(client, slot);
        //                noMissions = false;
        //                break;
        //            } else {
        //                noMissions = true;
        //            }
        //        }
        //        if (noMissions) {
        //            PlayerMsg(client, "There are no more missions available today!", Text.BrightRed);
        //        }
        //    } else {
        //        OpenMissionBoard(client, slot);
        //    }
        //}

        public static void OpenMissionBoard(Client client) {
            if (client.Player.MissionBoard.BoardMissions.Count == 0) {
                PlayerMsg(client, "There are no missions available right now.  Come back later and check again.", Text.Grey);
            } else {
                TcpPacket packet = new TcpPacket("missionboard");
                MissionBoard board = client.Player.MissionBoard;
                packet.AppendParameter(client.Player.MissionBoard.BoardMissions.Count);
                for (int i = 0; i < client.Player.MissionBoard.BoardMissions.Count; i++) {
                    WonderMail mission = client.Player.MissionBoard.BoardMissions[i];
                    packet.AppendParameters(mission.Title, mission.Summary, mission.GoalName);
                    packet.AppendParameters(WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].MissionClients[mission.MissionClientIndex].Species,
                        WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].MissionClients[mission.MissionClientIndex].Form,
                        (int)mission.MissionType, mission.Data1, mission.Data2, (int)mission.Difficulty,
                        WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].Rewards[mission.RewardIndex].ItemNum,
                        WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].Rewards[mission.RewardIndex].Amount, mission.Mugshot);
                }
                packet.FinalizePacket();
                SendDataTo(client, packet);
            }
        }

        public static void SendAddedMission(Client client) {
            TcpPacket packet = new TcpPacket("missionadded");
            MissionBoard board = client.Player.MissionBoard;
            
            WonderMail mission = client.Player.MissionBoard.BoardMissions[0];
            packet.AppendParameters(mission.Title, mission.Summary, mission.GoalName);
            packet.AppendParameters(WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].MissionClients[mission.MissionClientIndex].Species,
                WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].MissionClients[mission.MissionClientIndex].Form,
                (int)mission.MissionType, mission.Data1, mission.Data2, (int)mission.Difficulty,
                WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].Rewards[mission.RewardIndex].ItemNum,
                WonderMailManager.Missions.MissionPools[(int)mission.Difficulty - 1].Rewards[mission.RewardIndex].Amount, mission.Mugshot);
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendRemovedMission(Client client, int slot) {
            TcpPacket packet = new TcpPacket("missionremoved");
            packet.AppendParameter(slot);
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void OpenTradeMenu(Client client) {
            Client tradePartner = ClientManager.FindClientFromCharID(client.Player.TradePartner);
            if (tradePartner != null) {
                TcpPacket packet = new TcpPacket("opentrademenu");
                packet.AppendParameter(tradePartner.Player.Name);
                packet.FinalizePacket();
                SendDataTo(client, packet);
                TcpPacket packet2 = new TcpPacket("opentrademenu");
                packet2.AppendParameter(client.Player.Name);
                packet2.FinalizePacket();
                SendDataTo(tradePartner, packet2);
            }
        }

        public static void SendTradeSetItemUpdate(Client client, int itemNum, int amount, bool thisSide) {
            SendDataTo(client, TcpPacket.CreatePacket("tradesetitemupdate", itemNum.ToString(), amount.ToString(), thisSide.ToIntString()));
        }

        public static void SendTradeComplete(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("tradecomplete"));
        }

        public static void SendTradeUnconfirm(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("unconfirmtrade"));
        }

        public static void SendEndTrade(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("endtrade"));
        }

        //public static void SendNpcAttack(string mapID, int mapSlot) {
        //    SendDataToMap(mapID, TcpPacket.CreatePacket("npcattack", mapSlot));
        //}

        public static void PlainMsg(Client client, string msg, Enums.PlainMsgType num) {
            SendDataTo(client, TcpPacket.CreatePacket("plainmsg", msg, ((int)num).ToString()));
        }

        public static void PlayerMsg(Client client, string msg, Color color) {
            SendDataTo(client, TcpPacket.CreatePacket("msg", msg, color.ToArgb().ToString()));
        }

        public static void OpenVisitHouseMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("visithouse"));
        }

        public static void OpenAddShopMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("addhouseshop", House.SHOP_PRICE));
        }

        public static void OpenAddNoticeMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("addhousenotice", House.NOTICE_PRICE, House.WORD_PRICE));
        }
        public static void OpenAddSoundMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("addhousesound", House.SOUND_PRICE));
        }

        public static void OpenAddSignMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("addhousesign", House.WORD_PRICE));
        }

        public static void OpenChangeWeatherMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("changehouseweather", House.WEATHER_PRICE));
        }

        public static void OpenChangeDarknessMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("changehousedarkness", House.LIGHT_PRICE));
        }

        public static void OpenChangeBoundsMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("changehousebounds", House.TILE_PRICE));
        }

        public static void PlayerWarpToVoid(Client client) {
            IMap map = MapManager.RetrieveMap("void-" + client.Player.CharID);
            PlayerWarp(client, map, 10, 7);
        }

        public static void PlayerWarpToHouse(Client client, string houseOwnerID, int room) {
            PlayerWarpToHouse(client, houseOwnerID, room, true);
        }

        public static void PlayerWarpToHouse(Client client, string houseOwnerID, int room, bool tileCheck) {
            string houseID = MapManager.GenerateHouseID(houseOwnerID, room);
            IMap map = MapManager.RetrieveMap(houseID, true);
            bool mapModified = false;
            if (map == null) {
                // Make an empty house
                DataManager.Maps.HouseMap rawHouse = new DataManager.Maps.HouseMap(houseID);
                rawHouse.Owner = houseOwnerID;
                rawHouse.Room = room;
                map = new House(rawHouse);
                map.Moral = Enums.MapMoral.House;
                mapModified = true;
            }
            if (string.IsNullOrEmpty(map.Name)) {
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    map.Name = Players.PlayerManager.RetrieveCharacterName(dbConnection, houseOwnerID) + "'s House";
                    mapModified = true;
                }
            }
            if (mapModified) {
                map.Save();
            }
            PlayerWarp(client, map, ((House)map).StartX, ((House)map).StartY, tileCheck);
        }

        public static void PlayerXYWarp(Client client, int x, int y) {
            PlayerWarp(client, client.Player.Map, x, y);
        }

        public static void PlayerWarp(Client client, string mapID, int x, int y) {
            PlayerWarp(client, mapID, x, y, true);
        }

        public static void PlayerWarp(Client client, string mapID, int x, int y, bool tileCheck) {
            PlayerWarp(client, mapID, x, y, tileCheck, true);
        }

        public static void PlayerWarp(Client client, string mapID, int x, int y, bool tileCheck, bool playSound) {
            if (mapID.StartsWith("void")) {
                string charID = mapID.Replace("void-", "");
                if (charID != client.Player.CharID) {
                    // You can't go to someone elses void
                    return;
                }
            }

            PlayerWarp(client, MapManager.RetrieveMap(mapID, true), x, y, tileCheck, playSound);
        }

        public static void PlayerWarp(Client client, int mapNum, int x, int y) {
            PlayerWarp(client, mapNum, x, y, true);
        }

        public static void PlayerWarp(Client client, int mapNum, int x, int y, bool tileCheck) {
            PlayerWarp(client, mapNum, x, y, tileCheck, true);
        }

        public static void PlayerWarp(Client client, int mapNum, int x, int y, bool tileCheck, bool playSound) {
            PlayerWarp(client, MapManager.RetrieveMap(mapNum, true), x, y, tileCheck, playSound);
        }

        public static void PlayerWarp(Client client, IMap map, int x, int y) {
            PlayerWarp(client, map, x, y, true);
        }

        public static void PlayerWarp(Client client, IMap map, int x, int y, bool tileCheck) {
            PlayerWarp(client, map, x, y, tileCheck, true);
        }



        public static void PlayerWarp(Client client, IMap map, int x, int y, bool tileCheck, bool playSound) {
            
            if (client.Player.LoggedIn && map.MapType == Enums.MapType.Void) {
                Maps.Void @void = map as Maps.Void;
                if (@void.PlayerOwner.CharID != client.Player.CharID) {
                    return;
                }
            }
            if (x > map.MaxX)
                x = map.MaxX;
            if (x < 0)
                x = 0;
            if (y > map.MaxY)
                y = map.MaxY;
            if (y < 0)
                y = 0;

            // Save old map to send erase player data to
            IMap oldMap = client.Player.Map;

            if (oldMap != null && oldMap.MapID == map.MapID && client.Player.LoggedIn == true) {
                PacketHitList hitlist = null;
                PacketHitList.MethodStart(ref hitlist);

                client.Player.X = x;
                client.Player.Y = y;

                PacketBuilder.AppendPlayerXY(client, hitlist);

                PacketHitList.MethodEnded(ref hitlist);

                if (tileCheck) {
                    if (map.Tile[x, y].Type == Enums.TileType.Story) {
                        StoryManager.PlayStory(client, map.Tile[x, y].Data1);
                    }
                }
                return;
            }

            if (oldMap != null && oldMap.MapType == Enums.MapType.Void) {
                Maps.Void @void = oldMap as Maps.Void;
                if (@void.SafeExit == false) {
                    return;
                }
            }

            if (map.MapType == Enums.MapType.Standard) {
                Map sMap = map as Map;
                if (sMap.Instanced && client.Player.InUninstancedWarp == false) {
                    InstancedMap iMap = null;
                    
                    //party check
                    if (client.Player.IsInParty()) {
                        Party playerParty = PartyManager.FindParty(client.Player.PartyID);
                        foreach (Client i in playerParty.GetOnlineMemberClients()) {
                            IMap partyPlayerMap = i.Player.Map;
                            if (i != client && partyPlayerMap.MapType == Enums.MapType.Instanced) {
                                if (((InstancedMap)partyPlayerMap).MapBase == sMap.MapNum) {
                                    iMap = (InstancedMap)partyPlayerMap;
                                    break;
                                }
                            }
                        }
                    }



                    if (iMap == null) {
                        iMap = MapCloner.CreateInstancedMap(sMap);
                        //MapManager.RemoveActiveMap(sMap.MapID);
                        MapManager.AddActiveMap(iMap);
                        iMap.MapBase = sMap.MapNum;
                        map = iMap;
                        iMap.SpawnNpcs();
                        iMap.SpawnItems();
                    }
                    map = iMap;
                }
            }

            ScriptManager.InvokeSub("OnMapLoad", client, client.Player.Map, map, client.Player.LoggedIn);

            if (map.MapType == Enums.MapType.Void) {
                Scripting.ScriptManager.InvokeSub("OnPlayerVoidLoad", client, map as Server.Maps.Void);
            }

            if (oldMap == null || oldMap.MapID != map.MapID || client.Player.LoggedIn == false) {
                // Sets it so we know to process npcs on the map
                map.PlayersOnMap.Add(client.Player.CharID);
                MapManager.AddActiveMap(map); 
 
                client.Player.MapID = map.MapID;

                if (client.Player.LoggedIn) {
                    SendLeaveMap(client, oldMap, true, false);
                }

                client.Player.X = x;
                client.Player.Y = y;

                MapManager.LoadBorderingMaps(map);

                //ReDim Program.ClassMan.mMaps[mapNum].Tile(0 To Program.ClassMan.mMaps[mapNum].MapX, 0 To Program.ClassMan.mMaps[mapNum].MapY) As TileRec

                client.Player.GettingMap = true;
                if (playSound) {
                    PlaySoundToMap(map.MapID, "warp.wav");
                }
                Messenger.SendPlayerData(client);
                SendCheckForMap(client);

                //SendInventory(client);// removed
                //SendWornEquipment(client);

                if (tileCheck) {
                    if (map.Tile[x, y].Type == Enums.TileType.Story) {
                        StoryManager.PlayStory(client, map.Tile[x, y].Data1);
                    }
                }
            }

            //Determine goal X and Y if on destination floor
            client.Player.ActiveGoalPoints.Clear();
            bool destinationReached = false;
            int targetsMissed = 0;
            for (int i = 0; i < client.Player.JobList.JobList.Count; i++) {
                if (client.Player.JobList.JobList[i].Accepted == Enums.JobStatus.Taken && Generator.IsGoalMap(client.Player.JobList.JobList[i].Mission, map)) {
                    destinationReached = true;
                    GoalPoint goalPoint = new GoalPoint();
                    goalPoint.DetermineGoalPoint(map);
                    goalPoint.JobListIndex = i;
                    if (goalPoint.GoalX == -1 || goalPoint.GoalY == -1) {
                        targetsMissed++;
                    } else {
                        client.Player.ActiveGoalPoints.Add(goalPoint);
                    }
                }
            }
            if (destinationReached) {
                PlayerMsg(client, "You have reached a destination floor!", Text.BrightBlue);
                if (targetsMissed > 0) {
                    PlayerMsg(client, "...But the target seems to be missing...", Text.Blue);
                }
            }
            SendActiveMissionGoalPoints(client);
            

            // Trigger event checking
            for (int i = 0; i < client.Player.TriggerEvents.Count; i++) {
                if (client.Player.TriggerEvents[i].Trigger == Events.Player.TriggerEvents.TriggerEventTrigger.MapLoad) {
                    if (client.Player.TriggerEvents[i].CanInvokeTrigger()) {
                        client.Player.TriggerEvents[i].InvokeTrigger();
                    }
                }
            }

            //turn off hunt to prevent ambushes
            client.Player.Hunted = false;
            Messenger.SendHunted(client);

            //if (Settings.Scripting == true && client.Player.LoggedIn == true) {
            //    ScriptManager.InvokeSub("OnMapJoined", client, client.Player.Map);
            //}

            //client.Player.GettingMap = true;
        }

        //public static void SendOpenJobList(Client client) {
        //    SendDataTo(client, TcpPacket.CreatePacket("openjoblist"));
        //}

        public static void SendActiveMissionGoalPoints(Client client) {
            TcpPacket packet = new TcpPacket("missiongoal");
            packet.AppendParameter(client.Player.ActiveGoalPoints.Count);
            foreach (GoalPoint point in client.Player.ActiveGoalPoints) {
                packet.AppendParameters(point.JobListIndex, point.GoalX, point.GoalY);
            }
            SendDataTo(client, packet);
        }

        //public static void SendStartMission(Client client, WonderMail wonderMail, int jobListSlot, bool resuming) {
        //    if (client.Player.ActiveMission == null) {
        //        TcpPacket packet = new TcpPacket("startmission");
        //        packet.AppendParameter(jobListSlot);
        //        MissionBoard board = client.Player.MissionBoard;
        //        if (!resuming) {
        //            if (wonderMail.MissionType == Enums.MissionType.Escort) {
        //                if (client.Player.FindOpenTeamSlot() == -1) {
        //                    PlayerMsg(client, "You do not have room in your team! Unable to start the mission.", Text.BrightRed);
        //                    return;
        //                }
        //            }
        //        }
        //        //Point goalPoint = Generator.DetermineGoalPoint(wonderMail);
        //        //if (goalPoint.X > -1 && goalPoint.Y > -1) {
        //        // Set the active mission
        //        Mission mission = new Mission();
        //        mission.WonderMail = wonderMail;
        //        //mission.GoalX = goalPoint.X;
        //        //mission.GoalY = goalPoint.Y;
        //        client.Player.ActiveMission = mission;
        //        client.Player.JobList.JobList[jobListSlot].Accepted = true;
        //        if (!resuming) {
        //            // If it's an escort mission, add the client to the team
        //            if (wonderMail.MissionType == Enums.MissionType.Escort) {
        //                //client.Player.AddToTeamTemp(client.Player.FindOpenTeamSlot(), wonderMail.Data1, 5);
        //                client.Player.AddToTeamTemp(client.Player.FindOpenTeamSlot(),
        //                    WonderMailSystem.PokemonInfo[wonderMail.CreatorInfoIndex].NpcNum, 5);
        //                SendActiveTeam(client);
        //            }
        //        }
        //        // Play the initial story
        //        if (!resuming) {
        //            if (wonderMail.StartStoryIndex > 0) {
        //                StoryManager.PlayStory(client, wonderMail.StartStoryIndex - 1);
        //            } else {
        //                PlayerMsg(client, "You have started the mission!", Text.Yellow);
        //            }
        //        }
        //        //    packet.AppendParameters(MapManager.GenerateMapID(wonderMail.Goal), goalPoint.X.ToString(), goalPoint.Y.ToString());
        //        //} else {
        //        //    PlayerMsg(client, "Unable to start mission. Unable to determine goal location.", Text.BrightRed);
        //        //    return;
        //        //}
        //        packet.FinalizePacket();
        //        SendDataTo(client, packet);
        //    } else {
        //        PlayerMsg(client, "You are already doing a mission!", Text.BrightRed);
        //    }
        //}

        //public static void SendMissionDescription(Client client, WonderMail wonderMail) {
        //    TcpPacket packet = new TcpPacket("missiondescription");
        //    // Append basic info
        //    packet.AppendParameters(WonderMails.WonderMailSystem.MissionInfo[wonderMail.MissionInfoIndex].Name,
        //                            Generator.GetGoalName(wonderMail), wonderMail.Difficulty.ToString(),
        //                            Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[WonderMails.WonderMailSystem.PokemonInfo[wonderMail.CreatorInfoIndex].NpcNum].Species).Name,
        //                            (Pokedex.Pokedex.GetPokemonForm(NpcManager.Npcs[WonderMails.WonderMailSystem.PokemonInfo[wonderMail.CreatorInfoIndex].NpcNum].Species).Mugshot[0, 0]).ToString(),
        //                            MissionManager.CreateMissionCompletionEventString(wonderMail),
        //                            MissionManager.CreateMissionRewardString(wonderMail));
        //    // Append detailed info
        //    packet.AppendParameters(((int)wonderMail.MissionType).ToString());
        //    SendDataTo(client, packet);
        //}

        public static void SendCancelJob(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("canceljob"));
        }

        //public static void SendMissionCode(Client client, WonderMail wonderMail) {
        //    TcpPacket packet = new TcpPacket("missioncode");
        //    packet.AppendParameters("Give this code to other players!", wonderMail.Code);
        //    SendDataTo(client, packet);
        //}

        public static void SendPlayerMissionExp(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("missionexp", client.Player.MissionExp.ToString(), ((int)client.Player.ExplorerRank).ToString()));
        }

        public static void SendHunted(Client client) {
            PacketHitList hitlist = new PacketHitList();
            PacketBuilder.AppendHunted(client, hitlist);
            SendData(hitlist);
        }

        public static void SendActiveTeam(Client client) {
            TcpPacket packet = new TcpPacket("activeteam");
            Player player = client.Player;
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameters(player.Team[i].Name, player.Team[i].Species.ToString(), player.Team[i].Form.ToString(), ((int)player.Team[i].Shiny).ToString(), ((int)player.Team[i].Sex).ToString(),
                                            player.Team[i].HP.ToString(), player.Team[i].MaxHP.ToString(),
                                            Server.Math.CalculatePercent(player.Team[i].Exp, Exp.ExpManager.Exp[player.Team[i].Level - 1]).ToString(),
                                            player.Team[i].Level.ToString(), ((int)player.Team[i].StatusAilment).ToString(), player.Team[i].HeldItemSlot.ToString());
                } else {
                    packet.AppendParameter("notloaded");
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendActivePets(Client client) {
            TcpPacket packet = new TcpPacket("activepets");
            packet.AppendParameter(client.ConnectionID);
            for (int i = 1; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded) {
                    packet.AppendParameters(client.Player.Team[i].Sprite.ToString());
                } else {
                    packet.AppendParameters("-1");
                }
            }
            packet.FinalizePacket();
            SendDataToMap(client.Player.MapID, packet);
        }

        public static void SendWornEquipment(Client client) {
            TcpPacket packet = new TcpPacket("playerhelditem");
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameter(client.Player.Team[i].HeldItemSlot);
                } else {
                    packet.AppendParameter("0");
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }
        /*
        public static void SendActiveTeamStatus(Client client) {
            TcpPacket packet = new TcpPacket("teamstatus");
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameter(((int)client.Player.Team[i].StatusAilment).ToString());
                } else {
                    packet.AppendParameter("0");
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }*/

        public static void SendJobList(Client client) {
            // 
            TcpPacket packet = new TcpPacket("joblist");
            Player player = client.Player;
            packet.AppendParameter(player.JobList.JobList.Count.ToString());
            for (int i = 0; i < player.JobList.JobList.Count; i++) {
                WonderMailJob job = player.JobList.JobList[i];
                packet.AppendParameters(job.Mission.Title, job.Mission.Summary, job.Mission.GoalName);
                packet.AppendParameters(WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].MissionClients[job.Mission.MissionClientIndex].Species,
                    WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].MissionClients[job.Mission.MissionClientIndex].Form,
                    (int)job.Mission.MissionType, job.Mission.Data1, job.Mission.Data2, (int)job.Mission.Difficulty,
                    WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].Rewards[job.Mission.RewardIndex].ItemNum,
                    WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].Rewards[job.Mission.RewardIndex].Amount, job.Mission.Mugshot);
                //MapGeneralInfo mapInfo = MapManager.RetrieveMapGeneralInfo(player.JobList.JobList[i].Mission.Goal);
                packet.AppendParameters((int)player.JobList.JobList[i].Accepted);
                if (player.JobList.JobList[i].SendsRemaining > 0) {
                    packet.AppendParameters(1);
                } else {
                    packet.AppendParameters(0);
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendNewJob(Client client) {
            TcpPacket packet = new TcpPacket("newjob");
            Player player = client.Player;

            WonderMailJob job = player.JobList.JobList[player.JobList.JobList.Count - 1];
            packet.AppendParameters(job.Mission.Title, job.Mission.Summary, job.Mission.GoalName);
            packet.AppendParameters(WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].MissionClients[job.Mission.MissionClientIndex].Species,
                WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].MissionClients[job.Mission.MissionClientIndex].Form,
                (int)job.Mission.MissionType, job.Mission.Data1, job.Mission.Data2, (int)job.Mission.Difficulty,
                WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].Rewards[job.Mission.RewardIndex].ItemNum,
                WonderMailManager.Missions.MissionPools[(int)job.Mission.Difficulty - 1].Rewards[job.Mission.RewardIndex].Amount, job.Mission.Mugshot);
            //MapGeneralInfo mapInfo = MapManager.RetrieveMapGeneralInfo(player.JobList.JobList[i].Mission.Goal);
            packet.AppendParameters((int)player.JobList.JobList[player.JobList.JobList.Count - 1].Accepted);
            if (player.JobList.JobList[player.JobList.JobList.Count - 1].SendsRemaining > 0) {
                packet.AppendParameters(1);
            } else {
                packet.AppendParameters(0);
            }

            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendDeleteJob(Client client, int slot) {
            TcpPacket packet = new TcpPacket("deletejob");
            
            packet.AppendParameter(slot);

            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendJobAccept(Client client, int slot) {
            TcpPacket packet = new TcpPacket("acceptjob");
            Player player = client.Player;

            packet.AppendParameters(slot, (int)player.JobList.JobList[slot].Accepted);

            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendRemainingJobSends(Client client, int slot) {
            TcpPacket packet = new TcpPacket("jobsends");
            Player player = client.Player;

            packet.AppendParameter(slot);
            if (player.JobList.JobList[slot].SendsRemaining > 0) {
                packet.AppendParameter(1);
            } else {
                packet.AppendParameter(0);
            }

            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        /*
        public static void SendActiveTeamNum(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("activeteamnum", client.Player.ActiveSlot.ToString()));
        }
        */
        public static void SendAllRecruits(Client client) {
            SendDataTo(client, PacketBuilder.CreatePlayerAllRecruitsPacket(client), true, false);
        }

        public static void SendBank(Client client) {
            TcpPacket packet = new TcpPacket("playerbank");
            for (int i = 1; i <= client.Player.MaxBank; i++) {
                if (client.Player.Bank.ContainsKey(i)) {
                    packet.AppendParameters(client.Player.Bank[i].Num,
                                            client.Player.Bank[i].Amount);
                } else {
                    packet.AppendParameters(0, 0);
                }
            }
            SendDataTo(client, packet);
        }

        public static void SendBankUpdate(Client client, int bankSlot) {
            SendDataTo(client, TcpPacket.CreatePacket("playerbankupdate", bankSlot.ToString(), client.Player.Bank[bankSlot].Num.ToString(),
                                                      client.Player.Bank[bankSlot].Amount.ToString(),
                /* client.Player.mBank[bankSlot].Dur.ToString() */ "0"));
        }

        public static void SendBattleDivider(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("battledivider"));
        }

        public static void SendChars(DatabaseConnection dbConnection, Client client) {
            TcpPacket packet = new TcpPacket("allchars");
            string[] characterNames = PlayerDataManager.RetrieveAccountCharacters(dbConnection.Database, client.Player.AccountName);
            for (int i = 0; i < characterNames.Length; i++) {
                packet.AppendParameters(characterNames[i]);
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendCheckForMap(Client client) {
            IMap map = client.Player.GetCurrentMap();

            string emptyMapString = "nm-1";

            TcpPacket packet = new TcpPacket("checkformap");
            packet.AppendParameters(map.MapID, map.Revision.ToString(), map.TempChange.ToIntString());

            int[] borderMaps = new int[8] { map.Up, map.Down, map.Left, map.Right, 0, 0, 0, 0 };

            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Up)) {
                IMap mapData = MapManager.RetrieveMap(map.Up);
                if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Left)) {
                    borderMaps[4] = mapData.Left;
                }
                if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Right)) {
                    borderMaps[6] = mapData.Right;
                }
            } else {
                borderMaps[0] = 0;
            }
            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Down)) {
                IMap mapData = MapManager.RetrieveMap(map.Down);
                if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Left)) {
                    borderMaps[5] = mapData.Left;
                }
                if (SeamlessWorldHelper.IsMapSeamless(mapData, Enums.MapID.Right)) {
                    borderMaps[7] = mapData.Right;
                }
            } else {
                borderMaps[1] = 0;
            }

            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Left) == false) {
                borderMaps[2] = 0;
            }
            if (SeamlessWorldHelper.IsMapSeamless(map, Enums.MapID.Right) == false) {
                borderMaps[3] = 0;
            }

            for (int i = 0; i < borderMaps.Length; i++) {
                if (borderMaps[i] > 0) {
                    IMap borderMap = MapManager.RetrieveMap(borderMaps[i]);
                    if (borderMap != null) {
                        packet.AppendParameters(borderMap.MapID, borderMap.Revision.ToString(), borderMap.TempChange.ToIntString());
                    } else {
                        packet.AppendParameter(emptyMapString);
                    }
                } else {
                    packet.AppendParameter(emptyMapString);
                }
            }

            SendDataTo(client, packet);
            //SendDataTo(client, TcpPacket.CreatePacket("checkformap", map.MapID, map.Revision.ToString(), map.TempChange.ToIntString()));
        }

        public static void SendMapNameUpdate(Client client, int mapNum, string mapName) {
            SendDataTo(client, TcpPacket.CreatePacket("mapnameupdated", mapNum.ToString(), mapName));
        }

        public static void SendDataTo(Client client, IPacket packet) {
            //if (client.TcpClient.Socket.Connected) {
            //    if (packet.PacketString.StartsWith("nm")) {
            //        client.Player.DataTransfered += System.Text.Encoding.Default.GetByteCount(packet.PacketString);
            //    }
            //    switch (packet.ConnectionType) {
            //        case ConnectionType.Tcp: {
            //                client.TcpClient.Send(PMU.Core.ByteEncoder.StringToByteArray(client.PacketSecurity.EncryptPacket(packet)));
            //            }
            //            break;
            //        case ConnectionType.Udp: {
            //            }
            //            break;
            //    }
            //}
            //TcpManager.Server.Send(client.Player.PacketSecurity.EncryptPacket(packet), TcpManager.Server.GetClient(client));
            SendDataTo(client, packet, false, false);
        }

        public static void SendDataTo(Client client, IPacket packet, bool compress, bool encrypt) {
            SendDataTo(client, ByteEncoder.StringToByteArray(packet.PacketString), compress, encrypt, false);
        }

        public static void SendDataTo(Client client, byte[] packet, bool compress, bool encrypt, bool isPacketList) {
            if (client.TcpClient.Socket.Connected) {
                byte[] customHeader = new byte[client.TcpClient.CustomHeaderSize];
                if (encrypt) {
                    packet = client.PacketSecurity.EncryptPacket(packet);
                    customHeader[1] = 1;
                }

                if (packet.Length > 2000) {
                    if (compress == false) {
                        compress = true;
                    }
                }

                if (compress) {
                    packet = client.PacketSecurity.CompressPacket(packet);
                    customHeader[0] = 1;
                }

                if (isPacketList) {
                    customHeader[2] = 1;
                } else {
                    customHeader[2] = 0;
                }
                client.TcpClient.Send(packet, customHeader);
            }
        }

        public static void SendDataTo(string playerID, IPacket packet) {
            Client client = ClientManager.FindClientFromCharID(playerID);
            if (client != null) {
                SendDataTo(client, packet);
            }
        }

        public static void SendDataTo(Client client, PacketList packetList) {
            SendDataTo(client, packetList.CombinePackets(), false, false, true);
        }

        public static void SendData(PacketHitList packetHitList) {
            for (int i = 0; i < packetHitList.HitList.Count; i++) {
                SendDataTo(packetHitList.HitList.Keys[i], packetHitList.HitList.Values[i]);
            }
        }

        public static void SendDataToAll(IPacket packet) {
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying()) {
                    SendDataTo(i, packet);
                }
            }
        }

        public static void SendDataToAllBut(Client client, IPacket packet) {
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying() && i != client) {
                    SendDataTo(i, packet);
                }
            }
        }

        public static void SendDataToParty(string partyID, IPacket packet) {
            SendDataToParty(PartyManager.FindParty(partyID), packet);
        }

        public static void SendDataToParty(Players.Parties.Party party, IPacket packet) {
            foreach (Client i in party.GetOnlineMemberClients()) {
                if (i.IsPlaying()) {
                    SendDataTo(i, packet);
                }
            }
        }

        //public static void SendDataToMap(int mapNum, TcpPacket packet) {
        //    if (mapNum != -2) {
        //        for (int i = 0; i < PlayerManager.Players.MaxPlayers; i++) {
        //            if (Tcp.TcpManager.IsPlaying(i) && PlayerManager.Players[i].Map == mapNum) {
        //                SendDataTo(i, packet);
        //            }
        //        }
        //    }
        //}

        public static void SendDataToMap(string mapID, IPacket packet) {
            IMap map = MapManager.RetrieveActiveMap(mapID);
            if (map != null) {
                foreach (Client i in map.GetClients()) {
                    SendDataTo(i, packet);
                }
            }
            //if (mapNum != -2) {
            //    for (int i = 0; i < PlayerManager.Players.MaxPlayers; i++) {
            //        if (Tcp.TcpManager.IsPlaying(i) && PlayerManager.Players[i].Map == mapNum) {
            //            SendDataTo(i, packet);
            //        }
            //    }
            //}
        }

        //public static void SendDataToMapBut(ConnectedClient client, int mapNum, TcpPacket packet) {
        //    if (mapNum != -2) {
        //        for (int i = 0; i < PlayerManager.Players.MaxPlayers; i++) {
        //            if (Tcp.TcpManager.IsPlaying(i) && PlayerManager.Players[i].Map == mapNum && i != client) {
        //                SendDataTo(i, packet);
        //            }
        //        }
        //    }
        //}

        public static void SendDataToMapBut(Client client, string mapID, IPacket packet) {
            // 
            IMap map = MapManager.RetrieveActiveMap(mapID);
            if (map != null) {
                foreach (Client mapClient in map.GetClients()) {
                    if (mapClient != client) {
                        SendDataTo(mapClient, packet);
                    }
                }
                //for (int i = 0; i < map.PlayersOnMap.Count; i++) {
                //    TcpClientIdentifier clientID = PlayerID.FindTcpID(map.PlayersOnMap[i]);
                //    if (clientID != null) {
                //        if (clientID != client.TcpID) {
                //            SendDataTo(map.PlayersOnMap[i], packet);
                //        }
                //    }
                //}
            }
            //for (int i = 0; i < PlayerManager.Players.MaxPlayers; i++) {
            //    if (Tcp.TcpManager.IsPlaying(i) && PlayerManager.Players[i].Map == mapNum && i != client) {
            //        SendDataTo(i, packet);
            //    }
            //}
        }
        /*
        public static void SendDataToPlayerMap(ConnectedClient client, TcpPacket packet) {
            if (client.Player.Map != -2) {
                SendDataToMap(client.Player.Map, packet);
            } else {
                SendDataTo(client, packet);
            }
        }

        public static void SendDataToPlayerMapBut(ConnectedClient client, TcpPacket packet) {
            if (client.Player.Map != -2) {
                SendDataToMapBut(client, client.Player.Map, packet);
            }
        }

        public static TcpPacket CreatePartyMemberDataPacket(PartyMember member, int slot) {
            Client memberClient = ClientManager.GetClient(Players.PlayerID.FindTcpID(member.PlayerID));
            if (memberClient != null) {
                return PacketBuilder.CreatePartyMemberDataPacket(memberClient, slot);
            } else {
                return null;
            }
        }

        public static void SendPartyMemberData(Party party, PartyMember member, int slot) {
            Client memberClient = ClientManager.GetClient(Players.PlayerID.FindTcpID(member.PlayerID));
            if (memberClient != null) {
                SendDataToParty(party, PacketBuilder.CreatePartyMemberDataPacket(memberClient, slot));
            }
        }
        
        public static void SendPartyMemberData(Client client, Client member, int slot) {
            SendDataTo(client, PacketBuilder.CreatePartyMemberDataPacket(member, slot));
        }

        public static void SendPartyMemberData(Client client, PartyMember member, int slot) {
            Client memberClient = ClientManager.GetClient(Players.PlayerID.FindTcpID(member.PlayerID));
            if (memberClient != null) {Send
                SendDataTo(client, CreatePartyMemberDataPacket(member, slot));
            }
        }

        */


        public static void SendPartyMemberData(Party party, Client member, int slot) {
            SendDataToParty(party, PacketBuilder.CreatePartyMemberDataPacket(party, member, slot));
        }

        public static void SendLeftParty(Party party, int slot) {
            SendDataToParty(party, TcpPacket.CreatePacket("clearpartyslot", slot.ToString()));
        }

        public static void SendDisbandPartyTo(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("disbandparty"));
        }

        public static void SendHP(Client client) {
            SendHP(client, client.Player.ActiveSlot);
        }

        public static void SendHP(Client client, int recruitIndex) {
            SendDataTo(client, TcpPacket.CreatePacket("playerhp", recruitIndex.ToString(), client.Player.Team[recruitIndex].MaxHP.ToString(),
                                                      client.Player.Team[recruitIndex].HP.ToString()));
            if (!string.IsNullOrEmpty(client.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(client);
                SendPartyMemberData(playerParty, client, playerParty.GetMemberSlot(client.Player.CharID));
            }
        }

        /*
        public static void SendEXP(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("playerexp", client.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      client.Player.GetActiveRecruit().Exp.ToString()));
            if (!string.IsNullOrEmpty(client.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(client);
                SendPartyMemberData(playerParty, client, playerParty.Members.GetMemberSlot(client.Player.CharID));
            }
        }

        */
        public static void SendBelly(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("recruitbelly", client.Player.GetActiveRecruit().Belly.ToString(),
                client.Player.GetActiveRecruit().MaxBelly.ToString()));
        }
        public static void SendInventory(Client client) {
            TcpPacket packet = new TcpPacket("playerinv");
            packet.AppendParameter(client.Player.MaxInv);
            for (int i = 1; i <= client.Player.MaxInv; i++) {
                packet.AppendParameters(client.Player.Inventory[i].Num.ToString(),
                    client.Player.Inventory[i].Amount.ToString(),
                    client.Player.Inventory[i].Sticky.ToIntString());
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }


        public static void SendInventoryUpdate(Client client, int invSlot) {
            SendDataTo(client, TcpPacket.CreatePacket("playerinvupdate", invSlot.ToString(),
                client.Player.Inventory[invSlot].Num.ToString(),
                client.Player.Inventory[invSlot].Amount.ToString(),
                client.Player.Inventory[invSlot].Sticky.ToIntString()));
        }

        public static void SendInventoryUpdate(Client client, int invSlot, PacketHitList hitList) {
            PacketHitList.MethodStart(ref hitList);

            hitList.AddPacket(client, TcpPacket.CreatePacket("playerinvupdate", invSlot.ToString(),
                client.Player.Inventory[invSlot].Num.ToString(),
                client.Player.Inventory[invSlot].Amount.ToString(),
                client.Player.Inventory[invSlot].Sticky.ToIntString()));

            PacketHitList.MethodEnded(ref hitList);
        }

        public static void SendJoinGame(Client client) {
            try {
                PacketHitList packetList = null;
                PacketHitList.MethodStart(ref packetList);

                PacketBuilder.AppendMaxInfo(client, packetList);
                PacketBuilder.AppendConnectionID(client, packetList);
                client.Player.CheckAllHeldItems();

                //PacketBuilder.AppendAllEmotions(client, packetList);
                //PacketBuilder.AppendAllArrows(client, packetList);
                PacketBuilder.AppendAllNpcs(client, packetList);
                PacketBuilder.AppendAllShops(client, packetList);
                PacketBuilder.AppendAllSpells(client, packetList);
                PacketBuilder.AppendAllEvos(client, packetList);
                PacketBuilder.AppendAllStoryNames(client, packetList);
                PacketBuilder.AppendAllItems(client, packetList);
                PacketBuilder.AppendAllRDungeons(client, packetList);
                PacketBuilder.AppendAllDungeons(client, packetList);
                PacketBuilder.AppendAllPokemon(client, packetList);
                PacketBuilder.AppendInventory(client, packetList);
                PacketBuilder.AppendActiveTeam(client, packetList);
                PacketBuilder.AppendActiveTeamNum(client, packetList);
                PacketBuilder.AppendPlayerData(client, packetList);
                PacketHitList.MethodEnded(ref packetList);

                PartyManager.LoadCharacterParty(client);

                Scripting.ScriptManager.InvokeSub("JoinGame", client);

                if (client.Player.CurrentSegment != -1 && client.Player.CurrentChapter != null) {
                    if (Stories.StoryManager.Stories[client.Player.CurrentChapter.ID.ToInt()].ExitAndContinue.Count > 0) {
                        int goodSegment = -1;
                        Story story = Stories.StoryManager.Stories[client.Player.CurrentChapter.ID.ToInt()];
                        for (int i = client.Player.CurrentSegment; i >= 0; i--) {
                            if (story.ExitAndContinue.Contains(i)) {
                                goodSegment = i;
                                break;
                            }
                        }
                        if (goodSegment > -1) {
                            client.Player.SegmentToStart = goodSegment;
                            PlayerWarp(client, client.Player.MapID, client.Player.X, client.Player.Y, false);
                            Stories.StoryManager.ResumeStory(client, client.Player.CurrentChapter.ID.ToInt());
                        } else {
                            client.Player.CurrentChapter = null;
                            client.Player.CurrentSegment = -1;
                            PlayerWarp(client, client.Player.MapID, client.Player.X, client.Player.Y);
                        }
                    } else {
                        client.Player.CurrentChapter = null;
                        client.Player.CurrentSegment = -1;
                        PlayerWarp(client, client.Player.MapID, client.Player.X, client.Player.Y);
                    }
                } else {
                    client.Player.CurrentChapter = null;
                    client.Player.CurrentSegment = -1;
                    PlayerWarp(client, client.Player.MapID, client.Player.X, client.Player.Y);
                }


                PacketHitList hitlist = null;
                PacketHitList.MethodStart(ref hitlist);

                if (!string.IsNullOrEmpty(Settings.MOTD.Trim()))
                    hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("MOTD: " + Settings.MOTD, Text.BrightBlue));

                PacketBuilder.AppendWhosOnline(client, hitlist);

                PacketBuilder.AppendAvailableExpKitModules(client, hitlist);

                // TODO: April Fools mode
                hitlist.AddPacket(client, TcpPacket.CreatePacket("foolsmode", Server.Globals.FoolsMode.ToIntString()));
                hitlist.AddPacket(client, TcpPacket.CreatePacket("ingame"));

#if DEBUG
                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                    hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("The server is currently in debug mode!", Text.BrightRed));
                }
#endif

                PacketBuilder.AppendPlayerMoves(client, hitlist);

                PacketBuilder.AppendJobList(client, hitlist);
                PacketBuilder.AppendPlayerData(client, hitlist);

                PacketBuilder.AppendStats(client, hitlist);
                PacketBuilder.AppendBelly(client, hitlist);

                PacketBuilder.AppendPlayerMissionExp(client, hitlist);

                client.Player.AppendFriendsList(hitlist);

                hitlist.AddPacket(client, PacketBuilder.CreateGameTimePacket());

                client.Player.RefreshSeenCharacters(hitlist);

                if (!string.IsNullOrEmpty(Globals.ServerStatus)) {
                    hitlist.AddPacket(client, PacketBuilder.CreateServerStatus());
                }

                PacketHitList.MethodEnded(ref hitlist);

                //for (int i = 0; i < client.Player.JobList.JobList.Count; i++) {
                //    if (client.Player.JobList.JobList[i].Accepted) {
                //        Messenger.SendStartMission(client, client.Player.JobList.JobList[i].Mission, i, true);
                //        break;
                //    }
                //}

                client.Player.LoggedIn = true;


                // TODO: Veteran compensation
                //VeteranData veteranData = VeteranCompensationHelper.RetrieveVeteranData(client.Player.AccountName, client.Player.mPassword, client.Player.Name);
                //if (veteranData.IsValidVeteran()) {
                //    client.Player.Veteran = true;
                //    if (veteranData.ExplorerPoints > 0) {
                //        client.Player.MissionExp += veteranData.ExplorerPoints;
                //        Messenger.SendPlayerMissionExp(client);
                //    }
                //    if (!string.IsNullOrEmpty(veteranData.Guild)) {
                //        client.Player.GuildAccess = 3;
                //        client.Player.GuildName = veteranData.Guild;
                //    }

                //    Story story = new Story();
                //    StoryBuilderSegment segment = new StoryBuilderSegment();
                //    StoryBuilder.AppendMapVisibilityAction(segment, false);
                //    StoryBuilder.AppendSaySegment(segment, "PMU Staff: Greetings, " + client.Player.Name + "!", -1, 0, 0);
                //    StoryBuilder.AppendSaySegment(segment, "PMU Staff: We apologize for the loss of your old account. Although we did not want to force you to restart, it was something that had to be done.", -1, 0, 0);
                //    StoryBuilder.AppendSaySegment(segment, "PMU Staff: However, we understand the amount of time that was put into your old account. So, to make it up to you...", -1, 0, 0);
                //    if (veteranData.ExplorerPoints > 0 && !string.IsNullOrEmpty(veteranData.Guild)) {
                //        StoryBuilder.AppendSaySegment(segment, "PMU Staff: You will be given free Explorer Points...", -1, 0, 0);
                //        StoryBuilder.AppendSaySegment(segment, "PMU Staff: ...and your old guild!", -1, 0, 0);
                //        StoryBuilder.AppendSaySegment(segment, "You have been awarded with " + veteranData.ExplorerPoints + " Explorer Points!", -1, 0, 0);
                //        StoryBuilder.AppendSaySegment(segment, "You have been made the leader of " + veteranData.Guild + "!", -1, 0, 0);
                //    } else if (veteranData.ExplorerPoints > 0) {
                //        StoryBuilder.AppendSaySegment(segment, "PMU Staff: You will be given free Explorer Points!", -1, 0, 0);
                //        StoryBuilder.AppendSaySegment(segment, "You have been awarded with " + veteranData.ExplorerPoints + " Explorer Points!", -1, 0, 0);
                //    } else if (!string.IsNullOrEmpty(veteranData.Guild)) {
                //        StoryBuilder.AppendSaySegment(segment, "PMU Staff: You will be given ownership of your old guild!", -1, 0, 0);
                //        StoryBuilder.AppendSaySegment(segment, "You have been made the leader of " + veteranData.Guild + "!", -1, 0, 0);
                //    }
                //    StoryBuilder.AppendSaySegment(segment, "PMU Staff: Have fun playing!", -1, 0, 0);
                //    StoryBuilder.AppendMapVisibilityAction(segment, true);

                //    segment.AppendToStory(story);
                //    StoryManager.PlayStory(client, story);
                //}
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex, "Login, sending join game info");
            }
        }

        /*
        public static void SendJoinMap(Client client) {
            // Send all players on current map to client
            foreach (Client i in ClientManager.GetClients()) {
                Player player2 = i.Player;
                if (player2 != null) {
                    if (i.IsPlaying() && i != client && player2.MapID == client.Player.MapID) {
                        TcpPacket packet = TcpPacket.CreatePacket("playerdata",
                                                                  i.ConnectionID.ToString(), player2.Name, player2.GetActiveRecruit().Sprite.ToString(),
                                                                  player2.MapID, player2.X.ToString(), player2.Y.ToString(),
                                                                  ((int)player2.Direction).ToString(), ((int)player2.Access).ToString(),
                                                                  player2.PK.ToIntString(), player2.GuildName, ((int)player2.GuildAccess).ToString(),
                                                                  player2.Status);
                        if (player2.ConfusionStepCounter > 0) {
                            packet.AppendParameter("1");
                        } else {
                            packet.AppendParameter("0");
                        }
                        packet.AppendParameters((int)player2.GetActiveRecruit().StatusAilment, player2.GetActiveRecruit().VolatileStatus.Count);
                        for (int j = 0; j < player2.GetActiveRecruit().VolatileStatus.Count; j++) {
                            packet.AppendParameter(player2.GetActiveRecruit().VolatileStatus[j].Emoticon);
                        }
                        SendDataTo(client, packet);
                    }
                }
            }

            //SendActiveTeamToMap(client, player.mMap);
            // Send client's player data to everyone on the map including himself
            SendPlayerData(client);
        }
        */

        public static void SendLeaveMap(Client client, IMap map, bool shouldUnsee, bool logout) {
            if (map != null) {
                // Remove the player ID from the maps player list
                map.PlayersOnMap.Remove(client.Player.CharID);
                //if ((map.MapType == Enums.MapType.RDungeonMap || map.MapType == Enums.MapType.Instanced) && map.PlayersOnMap.Count > 0) {
                //    map.Save();
                //}
                if (logout) {
                    shouldUnsee = true;
                } else {
                    if (map.MapType == Enums.MapType.Instanced || map.MapType == Enums.MapType.RDungeonMap) {
                        client.Player.AddMapToDelete(map.MapID);
                    }
                }
                //remove from everyone's seencharacter list
                if (shouldUnsee) {
                    foreach (Client i in map.GetSurroundingClients(map)) {
                        i.Player.UnseeLeftPlayer(client.Player);
                    }
                    PacketHitList hitlist = null;
                    PacketHitList.MethodStart(ref hitlist);

                    //hitlist.AddPacketToPlayersInSightBut(client, map, TcpPacket.CreatePacket("leftmap", client.ConnectionID.ToString()), client.Player.X, client.Player.Y, 30);
                    hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("leftmap", client.ConnectionID.ToString()));

                    PacketHitList.MethodEnded(ref hitlist);
                    //SendDataToMapBut(client, map.MapID, TcpPacket.CreatePacket("leftmap", client.ConnectionID.ToString()));
                }

            }
        }

        public static void SendLeftGame(Client client) {
            SendDataToAllBut(client, TcpPacket.CreatePacket("leftmap", client.ConnectionID.ToString()));
        }

        public static void SendTemporaryTileTo(PacketHitList hitlist, Client client, int x, int y, Tile tile) {
            PacketHitList.MethodStart(ref hitlist);

            TcpPacket packet = new TcpPacket("tiledata");

            packet.AppendParameters(
                x.ToString(),
                y.ToString(),
                tile.Ground.ToString(),
                tile.GroundAnim.ToString(),
                tile.Mask.ToString(),
                tile.Anim.ToString(),
                tile.Mask2.ToString(),
                tile.M2Anim.ToString(),
                tile.Fringe.ToString(),
                tile.FAnim.ToString(),
                tile.Fringe2.ToString(),
                tile.F2Anim.ToString(),
                ((int)tile.Type).ToString(),
                tile.Data1.ToString(),
                tile.Data2.ToString(),
                tile.Data3.ToString(),
                tile.String1,
                tile.String2,
                tile.String3,
                tile.RDungeonMapValue.ToString(),
                tile.GroundSet.ToString(),
                tile.GroundAnimSet.ToString(),
                tile.MaskSet.ToString(),
                tile.AnimSet.ToString(),
                tile.Mask2Set.ToString(),
                tile.M2AnimSet.ToString(),
                tile.FringeSet.ToString(),
                tile.FAnimSet.ToString(),
                tile.Fringe2Set.ToString(),
                tile.F2AnimSet.ToString()
            );

            hitlist.AddPacket(client, packet);
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void SendTile(int x, int y, IMap map) {
            TcpPacket packet = new TcpPacket("tiledata");

            packet.AppendParameters(
                x.ToString(),
                y.ToString(),
                map.Tile[x, y].Ground.ToString(),
                map.Tile[x, y].GroundAnim.ToString(),
                map.Tile[x, y].Mask.ToString(),
                map.Tile[x, y].Anim.ToString(),
                map.Tile[x, y].Mask2.ToString(),
                map.Tile[x, y].M2Anim.ToString(),
                map.Tile[x, y].Fringe.ToString(),
                map.Tile[x, y].FAnim.ToString(),
                map.Tile[x, y].Fringe2.ToString(),
                map.Tile[x, y].F2Anim.ToString(),
                ((int)map.Tile[x, y].Type).ToString(),
                map.Tile[x, y].Data1.ToString(),
                map.Tile[x, y].Data2.ToString(),
                map.Tile[x, y].Data3.ToString(),
                map.Tile[x, y].String1,
                map.Tile[x, y].String2,
                map.Tile[x, y].String3,
                map.Tile[x, y].RDungeonMapValue.ToString(),
                map.Tile[x, y].GroundSet.ToString(),
                map.Tile[x, y].GroundAnimSet.ToString(),
                map.Tile[x, y].MaskSet.ToString(),
                map.Tile[x, y].AnimSet.ToString(),
                map.Tile[x, y].Mask2Set.ToString(),
                map.Tile[x, y].M2AnimSet.ToString(),
                map.Tile[x, y].FringeSet.ToString(),
                map.Tile[x, y].FAnimSet.ToString(),
                map.Tile[x, y].Fringe2Set.ToString(),
                map.Tile[x, y].F2AnimSet.ToString()
            );

            SendDataToMap(map.MapID, packet);
        }


        /*
        public static void SendMap(Client client, int mapNum) {
            SendDataTo(client, PacketBuilder.CreateMapDataPacket(MapManager.RetrieveActiveMap(MapManager.GenerateMapID(mapNum))));
        }

        
        public static void SendMapDone(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("mapdone"));
        }
        
        public static void SendMapItemsTo(Client client) {
            IMap map = client.Player.GetCurrentMap();
            TcpPacket packet = new TcpPacket("mapitemdata");
            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {//server will lie to the client and send all hidden items as blank
                if (!map.ActiveItem[i].Hidden) {
                    packet.AppendParameters(map.ActiveItem[i].Num.ToString(), map.ActiveItem[i].Value.ToString(), map.ActiveItem[i].Sticky.ToIntString(), map.ActiveItem[i].X.ToString(), map.ActiveItem[i].Y.ToString());
                } else {
                    packet.AppendParameters("0", "0", "0", "0", "0");
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }
        

        public static void SendMapItemsToMap(string mapID) {
            IMap map = MapManager.RetrieveActiveMap(mapID);
            if (map != null) {
                TcpPacket packet = new TcpPacket("mapitemdata");
                for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {//server will lie to the clients and send all hidden items as blank
                    if (!map.ActiveItem[i].Hidden) {
                        packet.AppendParameters(map.ActiveItem[i].Num.ToString(), map.ActiveItem[i].Value.ToString(), map.ActiveItem[i].Sticky.ToIntString(), map.ActiveItem[i].X.ToString(), map.ActiveItem[i].Y.ToString());
                    } else {
                        packet.AppendParameters("0", "0", "0", "0", "0");
                    }
                }
                packet.FinalizePacket();
                SendDataToMap(mapID, packet);
            }
        }

        public static void SendMapNpcsTo(Client client) {
            IMap map = client.Player.GetCurrentMap();
            TcpPacket packet = new TcpPacket("MAPNPCDATA");
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if ((int)map.ActiveNpc[i].Direction > 3) map.ActiveNpc[i].Direction = Enums.Direction.Right;
                packet.AppendParameters(map.ActiveNpc[i].Num.ToString(), map.ActiveNpc[i].X.ToString(), map.ActiveNpc[i].Y.ToString(), ((int)map.ActiveNpc[i].Direction).ToString(), ((int)map.ActiveNpc[i].StatusAilment).ToString());
                if (map.ActiveNpc[i].Num > 0 && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Friendly && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Shopkeeper && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Scripted) {
                    packet.AppendParameters("1");
                } else {
                    packet.AppendParameters("0");
                }
            }
            packet.FinalizePacket();

            SendDataTo(client, packet);
        }
        */

        public static void SendScriptedTileInfoTo(Client client, string data) {
            SendDataTo(client, TcpPacket.CreatePacket("scriptedattributename", data));
        }

        public static void SendScriptedSignInfoTo(Client client, string data) {
            SendDataTo(client, TcpPacket.CreatePacket("scriptedsignattributename", data));
        }

        public static void SendMobilityInfoTo(Client client, string data) {
            SendDataTo(client, TcpPacket.CreatePacket("mobilityname", data));
        }


        public static void SendNewCharClasses(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("newcharclasses", Settings.NewCharForm.ToString()));
        }

        public static void SendNews(Client client) {
            string newsString = "";
            for (int i = Server.Settings.News.Count-1; i >= 0; i--)
            {
                newsString += Server.Settings.News[i];
                newsString += '\n';
            }
            SendDataTo(client, TcpPacket.CreatePacket("news", newsString));
        }

        public static void SendAdventureLog(Client client) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                TcpPacket packet = TcpPacket.CreatePacket("adventurelog", (int)client.Player.Statistics.TotalPlayTime.TotalMinutes,
                    client.Player.GetTotalDungeonCompletionCount(), client.Player.MissionCompletions, 0, client.Player.DungeonCompletionCounts.Count);
                for (int i = 0; i < client.Player.DungeonCompletionCounts.Count; i++) {
                    if (DungeonManager.Dungeons.Count > client.Player.DungeonCompletionCounts.KeyByIndex(i)) {
                        packet.AppendParameters(DungeonManager.Dungeons[client.Player.DungeonCompletionCounts.KeyByIndex(i)].Name, client.Player.DungeonCompletionCounts.ValueByIndex(i).ToString());
                    } else {
                        packet.AppendParameters("Unknown Dungeon", client.Player.DungeonCompletionCounts.ValueByIndex(i).ToString());
                    }
                }

                if (client.Player.PlayerData.RecruitListLoaded == false) {
                    PlayerDataManager.LoadPlayerRecruitList(dbConnection.Database, client.Player.PlayerData);
                }

                for (int i = 1; i <= Constants.TOTAL_POKEMON; i++) {
                    if (!client.Player.RecruitList.ContainsKey(i)) {
                        packet.AppendParameter((int)Enums.PokedexStat.Unknown);
                    } else {
                        packet.AppendParameter(client.Player.RecruitList[i]);
                    }
                }
                SendDataTo(client, packet);
            }
        }

        public static void SendOnlineList(Client client) {
            TcpPacket packet = new TcpPacket();
            int n = 0;
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying()) {
                    packet.AppendParameter(i.Player.Name);
                    n++;
                }
            }
            packet.PrependParameters("onlinelist", n.ToString());
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendPlayerData(Client client) {

            TcpPacket packet = TcpPacket.CreatePacket("playerdata",
                                                                  client.ConnectionID.ToString(), client.Player.Name, client.Player.GetActiveRecruit().Sprite.ToString(),
                                                                  client.Player.GetActiveRecruit().Form.ToString(), ((int)client.Player.GetActiveRecruit().Shiny).ToString(), ((int)client.Player.GetActiveRecruit().Sex).ToString(),
                                                                  client.Player.MapID, client.Player.X.ToString(), client.Player.Y.ToString(),
                                                                  ((int)client.Player.Direction).ToString(), ((int)client.Player.Access).ToString(), client.Player.Hunted.ToIntString(),
                                                                  client.Player.Dead.ToIntString(), client.Player.GuildName, ((int)client.Player.GuildAccess).ToString(),
                                                                  client.Player.Status);

            packet.AppendParameters(0, (int)client.Player.GetActiveRecruit().StatusAilment, client.Player.GetActiveRecruit().VolatileStatus.Count);
            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                packet.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            hitlist.AddPacketToSurroundingPlayersBut(client, client.Player.Map, packet);

            PacketHitList.MethodEnded(ref hitlist);
            //SendDataToMapBut(client, client.Player.MapID, packet);
                        Recruit recruit = client.Player.GetActiveRecruit();
            int mobility = 0;
            for (int i = 15; i >= 0; i--) {
                if (recruit.Mobility[i]) {
                    mobility += (int)System.Math.Pow(2, i);
                }
            }
            
                TcpPacket myPacket = TcpPacket.CreatePacket("myplayerdata", client.Player.ActiveSlot.ToString(),
                                                      client.Player.Name, recruit.Sprite.ToString(), recruit.Form.ToString(), ((int)recruit.Shiny).ToString(), ((int)recruit.Sex).ToString(),
                                                      client.Player.MapID, client.Player.X.ToString(), client.Player.Y.ToString(),
                                                      ((int)client.Player.Direction).ToString(), ((int)client.Player.Access).ToString(), client.Player.Hunted.ToIntString(),
                                                      client.Player.Dead.ToIntString(), client.Player.GuildName, ((int)client.Player.GuildAccess).ToString(),
                                                      client.Player.Solid.ToIntString(), client.Player.Status, client.Player.GetActiveRecruit().Confused.ToIntString(),
                                                      ((int)client.Player.GetActiveRecruit().StatusAilment).ToString(), ((int)client.Player.SpeedLimit).ToString(),
                                                      mobility.ToString(), client.Player.GetActiveRecruit().TimeMultiplier.ToString(),
                                                      recruit.Darkness.ToString());

            myPacket.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus.Count);
            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                myPacket.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }
            SendDataTo(client, myPacket);


        }



        public static void SendPlayerGuild(Client client) {

            TcpPacket packet = TcpPacket.CreatePacket("playerguild", client.ConnectionID.ToString(), client.Player.GuildName, ((int)client.Player.GuildAccess).ToString());

            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            hitlist.AddPacketToSurroundingPlayersBut(client, client.Player.Map, packet);

            PacketHitList.MethodEnded(ref hitlist);

            TcpPacket myPacket = TcpPacket.CreatePacket("myplayerguild", client.Player.GuildName, ((int)client.Player.GuildAccess).ToString());

            SendDataTo(client, myPacket);
        }

        public static void SendGuildMenu(Client client) {
            TcpPacket packet = TcpPacket.CreatePacket("guildmenu");

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                List<GuildMemberData> members = DataManager.Players.PlayerDataManager.LoadGuildInfo(dbConnection.Database, client.Player.GuildName);

                packet.AppendParameter(members.Count);
                foreach (GuildMemberData member in members) {
                    if (!String.IsNullOrEmpty(member.LastLogin)) {
                        string[] loginDate = member.LastLogin.Split(' ')[0].Split(':');
                        string login = loginDate[2] + "/" + loginDate[1] + "/" + loginDate[0];
                        packet.AppendParameters(member.Name, member.GuildAccess.ToString(), login);
                    } else {
                        packet.AppendParameters(member.Name, member.GuildAccess.ToString(), "---");
                    }
                    
                }
            }


            SendDataTo(client, packet);
        }

        public static void SendGuildCreation(Client client) {
            TcpPacket packet = new TcpPacket("createguild");
            Party party = PartyManager.FindPlayerParty(client);

            if (party != null) {
                packet.AppendParameter(party.MemberCount);
                foreach (Client member in party.GetOnlineMemberClients()) {
                    packet.AppendParameter(member.Player.Name);
                }
            } else {
                packet.AppendParameters("1", client.Player.Name);
            }
            SendDataTo(client, packet);
        }

        public static void SendFullGuildUpdate(string guildName) {
            TcpPacket packet = TcpPacket.CreatePacket("fullguildupdate");
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                List<GuildMemberData> members = DataManager.Players.PlayerDataManager.LoadGuildInfo(dbConnection.Database, guildName);

                packet.AppendParameter(members.Count);
                foreach (GuildMemberData member in members) {
                    if (!String.IsNullOrEmpty(member.LastLogin)) {
                        string[] loginDate = member.LastLogin.Split(' ')[0].Split(':');
                        string login = loginDate[2] + "/" + loginDate[1] + "/" + loginDate[0];
                        packet.AppendParameters(member.Name, member.GuildAccess.ToString(), login);
                    } else {
                        packet.AppendParameters(member.Name, member.GuildAccess.ToString(), "---");
                    }

                }
            }
            
            foreach (Client target in ClientManager.GetClients()) {
                if (target.Player.GuildName == guildName) {
                    if (target.Player.Map.Tile[target.Player.X, target.Player.Y].Type == Enums.TileType.Guild) {

                        SendDataTo(target, packet);
                    }
                }
            }
        }

        public static void SendGuildUpdate(string guildName, int index, Enums.GuildRank newAccess) {
            //send update to all potential guild members with the menu open
            foreach (Client target in ClientManager.GetClients()) {
                if (target.Player.GuildName == guildName) {
                    if (target.Player.Map.Tile[target.Player.X, target.Player.Y].Type == Enums.TileType.Guild) {
                        SendDataTo(target, TcpPacket.CreatePacket("guildupdate", index, (int)newAccess));
                    }
                }
            }
        }

        public static void SendGuildAdd(string guildName, string name) {
            //send update to all potential guild members with the menu open
            foreach (Client target in ClientManager.GetClients()) {
                if (target.Player.GuildName == guildName) {
                    if (target.Player.Map.Tile[target.Player.X, target.Player.Y].Type == Enums.TileType.Guild) {
                        SendDataTo(target, TcpPacket.CreatePacket("guildadd", name));
                    }
                }
            }
        }

        public static void SendGuildRemove(string guildName, int index) {
            //send update to all potential guild members with the menu open
            foreach (Client target in ClientManager.GetClients()) {
                if (target.Player.GuildName == guildName) {
                    if (target.Player.Map.Tile[target.Player.X, target.Player.Y].Type == Enums.TileType.Guild) {
                        SendDataTo(target, TcpPacket.CreatePacket("guildremove", index));
                    }
                }
            }
        }

        /*
        public static void SendConfusion(Client client) {
            bool status;
            if (client.Player.ConfusionStepCounter > 0) {
                status = true;
            } else {
                status = false;
            }
            SendDataTo(client, TcpPacket.CreatePacket("confusion", status.ToIntString()));
            SendDataToMapBut(client, client.Player.MapID, TcpPacket.CreatePacket("playerconfused", client.ConnectionID.ToString(), status.ToIntString()));

        }

        public static void SendActiveStatusAilment(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("statusailment", ((int)client.Player.GetActiveRecruit().StatusAilment).ToString()));
            SendDataToMapBut(client, client.Player.MapID, TcpPacket.CreatePacket("playerstatused", client.ConnectionID.ToString(), ((int)client.Player.GetActiveRecruit().StatusAilment).ToString()));

        }
        

        public static void SendSpeedLimit(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("speedlimit", ((int)client.Player.SpeedLimit).ToString()));

        }

        
        public static void SendMobility(Client client) {
            int mobility = 0;
            for (int i = 15; i >= 0; i--) {
                if (client.Player.GetActiveRecruit().Mobility[i]) {
                    mobility += (int)System.Math.Pow(2, i);
                }
            }
            SendDataTo(client, TcpPacket.CreatePacket("mobility", mobility.ToString()));

        }
        */

        public static void SendPlayerMoves(Client client) {
            TcpPacket packet = new TcpPacket("moves");
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                RecruitMove move = client.Player.GetActiveRecruit().Moves[i];
                packet.AppendParameters(move.MoveNum.ToString(), move.CurrentPP.ToString(), move.MaxPP.ToString(), move.Sealed.ToIntString());
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        //public static void SendPlayerXY(Client client) {
        //    IMap map = client.Player.Map;
        //    foreach (Client i in map.GetClients()) {
        //        //if (client.Player.IsInSight(i.Player.X, i.Player.Y)) {
        //            SendDataTo(i, TcpPacket.CreatePacket("playerxy", client.ConnectionID.ToString(), client.Player.X.ToString(),
        //                                                             client.Player.Y.ToString()));
        //}
        //    }
        //}

        /*
        public static void SendMovePPUpdate(Client client, int moveSlot) {
            SendDataTo(client, TcpPacket.CreatePacket("moveppupdate", moveSlot.ToString(),
                                                      client.Player.GetActiveRecruit().Moves[moveSlot].CurrentPP.ToString(),
                                                      client.Player.GetActiveRecruit().Moves[moveSlot].MaxPP.ToString()));
        }
        */

        public static void SendStartStoryTo(Client client, int storyNum, int segment) {
            client.Player.CurrentChapter = StoryManager.Stories[storyNum];
            client.Player.CurrentSegment = segment;
            SendDataTo(client, TcpPacket.CreatePacket("startstory", storyNum.ToString(), segment.ToString()));
            client.Player.SegmentToStart = -1;
        }

        public static void SendStartStoryTo(Client client, int storyNum) {
            client.Player.CurrentChapter = StoryManager.Stories[storyNum];
            client.Player.CurrentSegment = 0;
            SendDataTo(client, TcpPacket.CreatePacket("startstory", storyNum.ToString(), "0"));
            client.Player.SegmentToStart = -1;
        }

        public static void ForceEndStoryTo(Client client) {
            client.Player.CurrentChapter = null;
            client.Player.CurrentSegment = -1;
            client.Player.QuestionID = "";
            SendDataTo(client, TcpPacket.CreatePacket("breakstory"));
            client.Player.SegmentToStart = -1;
        }

        public static void SendStats(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("playerstatspacket", client.Player.GetActiveRecruit().Atk.ToString(),
                                                      client.Player.GetActiveRecruit().Def.ToString(), client.Player.GetActiveRecruit().Spd.ToString(),
                                                      client.Player.GetActiveRecruit().SpclAtk.ToString(), client.Player.GetActiveRecruit().SpclDef.ToString(), client.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      client.Player.GetActiveRecruit().Exp.ToString(), client.Player.GetActiveRecruit().Level.ToString()));
        }


        public static void SendShopMenu(Client client, int shopNum) {
            if (Shops.ShopManager.Shops[shopNum].JoinSay.Trim() != "") {
                PlayerMsg(client, Shops.ShopManager.Shops[shopNum].JoinSay.Trim(), Color.Yellow);
            }
            SendDataTo(client, TcpPacket.CreatePacket("openshop"));

        }

        public static void SendRecallMenu(Client client, bool earlierEvo) {
            List<int> validLevelUpMoves = new List<int>();
            List<int> currentMoves = new List<int>();
            for (int i = 0; i < 4; i++) {
                if (client.Player.GetActiveRecruit().Moves[i].MoveNum > 0) {
                    currentMoves.Add(client.Player.GetActiveRecruit().Moves[i].MoveNum);
                }
            }

            if (earlierEvo) {
                int species = EvolutionManager.FindPreEvolution(client.Player.GetActiveRecruit().Species);

                while (species > -1) {

                    Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(species, client.Player.GetActiveRecruit().Form);
                    if (pokemon != null) {
                        for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                            if (pokemon.LevelUpMoves[n].Level <= client.Player.GetActiveRecruit().Level) {
                                int moveNum = pokemon.LevelUpMoves[n].Move;
                                if (!currentMoves.Contains(moveNum) && !validLevelUpMoves.Contains(moveNum)) {
                                    validLevelUpMoves.Add(moveNum);
                                }
                            }
                        }
                    }

                    species = EvolutionManager.FindPreEvolution(species);
                }
            } else {
                Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(client.Player.GetActiveRecruit().Species, client.Player.GetActiveRecruit().Form);
                if (pokemon != null) {
                    for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                        if (pokemon.LevelUpMoves[n].Level <= client.Player.GetActiveRecruit().Level) {
                            int moveNum = pokemon.LevelUpMoves[n].Move;
                            if (!currentMoves.Contains(moveNum) && !validLevelUpMoves.Contains(moveNum)) {
                                validLevelUpMoves.Add(moveNum);
                            }
                        }
                    }
                }
            }

            TcpPacket packet = new TcpPacket("moverecallmenu");

            foreach (int move in validLevelUpMoves) {
                packet.AppendParameter(move);
            }

            packet.FinalizePacket();

            SendDataTo(client, packet);

        }


        public static void SendTrade(Client client, int ShopNum) {
            TcpPacket packet = new TcpPacket();

            //int X = 0;
            //int i = 0;
            int XX = 0;


            //packet.AppendParameters(ShopNum.ToString(), ShopManager.Shops[ShopNum].LeaveSay);

            for (int i = 0; i < Constants.MAX_TRADES; i++) {
                if (ShopManager.Shops[ShopNum].Items[i].GetItem > 0) {
                    XX++;

                    packet.AppendParameters(
                        ShopManager.Shops[ShopNum].Items[i].GiveItem.ToString(),
                        ShopManager.Shops[ShopNum].Items[i].GiveValue.ToString(),
                        ShopManager.Shops[ShopNum].Items[i].GetItem.ToString());
                }
            }

            //for (XX = 0; XX < Constants.MAX_TRADES; XX++) {
            //    if (ShopManager.Shops[ShopNum].Items[XX].GetItem > 0) {

            //        for (int j = 0; j < XX - z - 1; j++) {
            //            packet.AppendParameters("0", "0", "0");

            //        }
            //        z = XX + 1;

            //        packet.AppendParameters(ShopManager.Shops[ShopNum].Items[XX].GiveItem.ToString(), ShopManager.Shops[ShopNum].Items[XX].GiveValue.ToString(), ShopManager.Shops[ShopNum].Items[XX].GetItem.ToString());

            //    }

            //}
            //packet.AppendParameter(z.ToString());
            packet.PrependParameters("trade", ShopNum.ToString(), ShopManager.Shops[ShopNum].LeaveSay,
                XX.ToString());

            packet.FinalizePacket();
            if (ShopManager.Shops[ShopNum].JoinSay != "") {
                PlayerMsg(client, ShopManager.Shops[ShopNum].JoinSay, Text.Yellow);
            }
            if (XX == 0) {
                SendDataTo(client, packet);
                PlayerMsg(client, "This shop has nothing to sell!", Text.BrightRed);
            } else {
                SendDataTo(client, packet);
            }
        }

        public static void SendUpdateStoryTo(Client client, int StoryNum) {
            TcpPacket packet = new TcpPacket("updatestory");
            Story story = StoryManager.Stories[StoryNum];
            packet.AppendParameters(StoryNum.ToString(), StoryManager.Stories[StoryNum].Name.Trim(), StoryManager.Stories[StoryNum].Revision.ToString(), StoryManager.Stories[StoryNum].StoryStart.ToString(), StoryManager.Stories[StoryNum].Segments.Count.ToString());
            for (int i = 0; i < StoryManager.Stories[StoryNum].Segments.Count; i++) {
                packet.AppendParameters(StoryManager.Stories[StoryNum].Segments[i].Parameters.Count.ToString(), ((int)StoryManager.Stories[StoryNum].Segments[i].Action).ToString());
                for (int z = 0; z < StoryManager.Stories[StoryNum].Segments[i].Parameters.Count; z++) {
                    packet.AppendParameters(StoryManager.Stories[StoryNum].Segments[i].Parameters.KeyByIndex(z), StoryManager.Stories[StoryNum].Segments[i].Parameters.ValueByIndex(z));
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet, true, false);
        }

        public static void SendRunStoryTo(Client client, Story story) {
            TcpPacket packet = new TcpPacket("runstory");
            packet.AppendParameters("-2", story.Name == null ? "" : story.Name, story.Revision.ToString(), story.StoryStart.ToString(), story.Segments.Count.ToString());
            for (int i = 0; i < story.Segments.Count; i++) {
                packet.AppendParameters(story.Segments[i].Parameters.Count.ToString(), ((int)story.Segments[i].Action).ToString());
                for (int z = 0; z < story.Segments[i].Parameters.Count; z++) {
                    packet.AppendParameters(story.Segments[i].Parameters.KeyByIndex(z), story.Segments[i].Parameters.ValueByIndex(z));
                }
            }
            packet.FinalizePacket();
            SendDataTo(client, packet, true, false);
        }

        public static void SendLoadingStoryTo(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("loadingstory"));
        }

        public static void SendUpdateStoryNameToAll(int storyNum) {
            SendDataToAll(TcpPacket.CreatePacket("updatestoryname", storyNum.ToString(), StoryManager.Stories[storyNum].Name ?? ""));
        }

        /*
        public static void SendWeatherTo(Client client) {
            //if (Globals.WeatherIntensity <= 0) Globals.WeatherIntensity = 1;
            //if (Globals.ServerWeather != Enums.Weather.None) {
            //	SendDataTo(client, TcpPacket.CreatePacket("weather", ((int)Globals.ServerWeather).ToString(), Globals.WeatherIntensity.ToString()));
            //} else {
            SendDataTo(client, TcpPacket.CreatePacket("weather", ((int)client.Player.Map.Weather).ToString()));
            //}
        }
        */

        public static void SendWeatherToAll() {
            //if (Globals.WeatherIntensity <= 0) Globals.WeatherIntensity = 1;
            //if (Globals.ServerWeather == Enums.Weather.None) {
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying()) {


                    SendDataTo(i, TcpPacket.CreatePacket("weather", ((int)i.Player.Map.Weather).ToString()));

                }
            }
            //} else {
            //	SendDataToAll(TcpPacket.CreatePacket("weather", ((int)Globals.ServerWeather).ToString(), Globals.WeatherIntensity.ToString()));
            //}

        }

        public static void SendGameTimeToAll() {
            SendDataToAll(TcpPacket.CreatePacket("gametime", ((int)Globals.ServerTime).ToString()));
        }

        public static void SendGameTimeTo(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("gametime", ((int)Globals.ServerTime).ToString()));
        }

        public static void SendAvailableExpKitModules(Client client) {
            TcpPacket packet = new TcpPacket("kitmodules");
            packet.AppendParameter(client.Player.AvailableExpKitModules.Count);
            for (int i = 0; i < client.Player.AvailableExpKitModules.Count; i++) {
                packet.AppendParameter((int)client.Player.AvailableExpKitModules[i].Type);
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendWhosOnline(Client client) {
            string s = "";
            int n = 0;
            foreach (Client i in ClientManager.GetClients()) {
                if (i.Player != null) {
                    if (i.IsPlaying() && i != client) {
                        n++;
                    }
                }
            }
            if (n == 0) {
                s = "There are no other players online.";
            } else {
                s = "There are " + n.ToString() + " other players online.";
            }
            PlayerMsg(client, s, Text.Grey);
        }

        public static void SpellAnim(int SpellNum, string mapID, int X, int Y) {
            SendDataToMap(mapID, PacketBuilder.CreateSpellAnim(SpellNum, X, Y));
        }


        //public static void SpellAnimToPlayerMap(int SpellNum, ConnectedClient client, int X, int Y) {
        //    SendDataToMap(client.Player.MapID, TcpPacket.CreatePacket("scriptspellanim", SpellNum.ToString(), MoveManager.Moves[SpellNum].SpellAnim.ToString(), MoveManager.Moves[SpellNum].SpellTime.ToString(), MoveManager.Moves[SpellNum].SpellDone.ToString(), X.ToString(), Y.ToString(), MoveManager.Moves[SpellNum].Big.ToIntString()));
        //}

        public static void StorageMessage(Client client, string msg) {
            SendDataTo(client, TcpPacket.CreatePacket("msg", msg));
        }

        //public static void WarpToInstancedMap(ConnectedClient client, InstancedMap map, int x, int y) {
        //    if (client.Player.Map != -2) {
        //        int oldMap = client.Player.Map;
        //        Messenger.SendLeaveMap(client, oldMap);

        //        // Now we check if there were any players left on the map the player just left, and if not stop processing npcs
        //        if (MapManager.Maps[oldMap].GetTotalPlayers() == 0) {
        //            MapManager.Maps[oldMap].PlayersOnMap = false;
        //        }
        //    }
        //    client.Player.Map = -2;
        //    client.Player.DungeonIndex = -1;
        //    client.Player.DungeonFloor = -1;
        //    if (map.StartX != 0) {
        //        client.Player.X = map.StartX;
        //    } else {
        //        client.Player.X = x;
        //    }
        //    if (map.StartY != 0) {
        //        client.Player.Y = map.StartY;
        //    } else {
        //        client.Player.Y = y;
        //    }
        //    client.Player.mInstancedMapManager.ActiveMap = map;
        //    //client.Player.mInstancedMapManager.ActiveMap.SpawnNpcs();
        //    //client.Player.mInstancedMapManager.ActiveMap.Items();
        //    SendMap(client, map);
        //    SendMapNpcsTo(client);
        //    SendMapItemsTo(client);
        //    SendPlayerData(client);
        //    SendMapDone(client);
        //    SendPlayerXY(client);
        //}

        //public static void SendEditArrowTo(Client client, int ArrowNum) {
        //    TcpPacket packet = new TcpPacket("editarrow");
        //    packet.AppendParameters(ArrowNum.ToString(), ArrowManagerBase.Arrows[ArrowNum].Name.Trim(),
        //        ArrowManagerBase.Arrows[ArrowNum].Pic.ToString(), ArrowManagerBase.Arrows[ArrowNum].Amount.ToString(),
        //       ArrowManagerBase.Arrows[ArrowNum].Range.ToString());
        //    packet.FinalizePacket();
        //   SendDataTo(client, packet);
        //}
        public static void SendEditEmotionTo(Client client, int EmoNum) {
            TcpPacket packet = new TcpPacket("editemoticon");
            packet.AppendParameter(EmoNum);
            packet.AppendClass(EmoticonManagerBase.Emoticons[EmoNum]);
            packet.FinalizePacket();
            //string packet = "EDITEMOTICON" + TcpManager.SEP_CHAR + EmoNum.ToString() + TcpManager.SEP_CHAR +
            //    EmoticonManagerBase.Emoticons[EmoNum].Command.Trim() + TcpManager.SEP_CHAR +
            //    EmoticonManagerBase.Emoticons[EmoNum].Pic + TcpManager.SEP_CHAR + TcpManager.END_CHAR;
            SendDataTo(client, packet);
        }

        public static void SendEditEvoTo(Client client, int EvoNum) {
            TcpPacket packet = new TcpPacket("editevo");
            packet.AppendParameters(Evolutions.EvolutionManager.Evolutions[EvoNum].Name, Evolutions.EvolutionManager.Evolutions[EvoNum].Species.ToString(), Evolutions.EvolutionManager.Evolutions[EvoNum].Branches.Count.ToString());
            foreach (Evolutions.EvolutionBranch i in Evolutions.EvolutionManager.Evolutions[EvoNum].Branches) {
                packet.AppendParameters(i.Name, i.NewSpecies.ToString(), i.ReqScript.ToString(), i.Data1.ToString(), i.Data2.ToString(), i.Data3.ToString());
            }
            //for (int i = 0; i <= EvolutionManager.Evolutions[EvoNum].SplitEvosNum; i++) {
            //    packet.AppendParameters(EvolutionManager.Evolutions[EvoNum].SplitEvos[i].Name, EvolutionManager.Evolutions[EvoNum].SplitEvos[i].NewSprite.ToString(), EvolutionManager.Evolutions[EvoNum].SplitEvos[i].NewClass.ToString(), EvolutionManager.Evolutions[EvoNum].SplitEvos[i].RequiredItem.ToString(), EvolutionManager.Evolutions[EvoNum].SplitEvos[i].MoveLearned.ToString());
            //}
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendEditItemTo(Client client, int itemnum) {
            TcpPacket packet = TcpPacket.CreatePacket("edititem", itemnum);
            SendDataTo(client, packet);
        }

        public static void SendItemEditor(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("itemeditor"));
        }

        public static void SendArrowEditor(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("arroweditor"));
        }

        public static void OpenMoveForgetMenu(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("openmoveforgetmenu"));
        }

        public static void SendNpcAiTypes(Client client) {
            TcpPacket packet = new TcpPacket("npcaitypes");
            packet.AppendParameter(Scripting.ScriptManager.NPCAIType.Count);
            for (int i = 0; i < Scripting.ScriptManager.NPCAIType.Count; i++) {
                packet.AppendParameters(Scripting.ScriptManager.NPCAIType.KeyByIndex(i));
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendNpcEmotion(Client client, int npcSlot, Enums.Emotion emotion) {
            SendDataToMap(client.Player.MapID, TcpPacket.CreatePacket("npcemotion", npcSlot.ToString(), ((int)emotion).ToString()));
        }

        public static void SendRDungeonEditor(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("rdungeoneditor"));
        }

        //public static void SendUpdateArrowToAll(int itemnum) {
        //   TcpPacket packet = new TcpPacket("updatearrow");
        //   packet.AppendParameters(itemnum.ToString(), ArrowManagerBase.Arrows[itemnum].Name.Trim(), ArrowManagerBase.Arrows[itemnum].Pic.ToString(), ArrowManagerBase.Arrows[itemnum].Range.ToString(), ArrowManagerBase.Arrows[itemnum].Amount.ToString());
        //   packet.FinalizePacket();
        //   SendDataToAll(packet);
        //}
        public static void SendUpdateEmoticonToAll(int EmoteNum) {
            TcpPacket packet = new TcpPacket("updateemoticon");
            packet.AppendParameter(EmoteNum);
            packet.AppendClass(EmoticonManagerBase.Emoticons[EmoteNum]);
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateEvoToAll(int EvoNum) {
            TcpPacket packet = new TcpPacket("updateevo");
            packet.AppendParameters(EvoNum.ToString(), EvolutionManager.Evolutions[EvoNum].Name);
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateItemToAll(int itemnum) {
            TcpPacket packet = new TcpPacket("updateitem");
            packet.AppendParameters(
                itemnum.ToString(),
                Items.ItemManager.Items[itemnum].Name.Trim(),
                Items.ItemManager.Items[itemnum].Desc.Trim(),
                Items.ItemManager.Items[itemnum].Pic.ToString(),
                ((int)Items.ItemManager.Items[itemnum].Type).ToString(),
                Items.ItemManager.Items[itemnum].Data1.ToString(),
                Items.ItemManager.Items[itemnum].Data2.ToString(),
                Items.ItemManager.Items[itemnum].Data3.ToString(),
                Items.ItemManager.Items[itemnum].Price.ToString(),
                Items.ItemManager.Items[itemnum].StackCap.ToString(),
                Items.ItemManager.Items[itemnum].Bound.ToIntString(),
                Items.ItemManager.Items[itemnum].Loseable.ToIntString(),
                Items.ItemManager.Items[itemnum].Rarity.ToString(),
                Items.ItemManager.Items[itemnum].ReqData1.ToString(),
                Items.ItemManager.Items[itemnum].ReqData2.ToString(),
                Items.ItemManager.Items[itemnum].ReqData3.ToString(),
                Items.ItemManager.Items[itemnum].ReqData4.ToString(),
                Items.ItemManager.Items[itemnum].ReqData5.ToString(),
                Items.ItemManager.Items[itemnum].ScriptedReq.ToString(),
                Items.ItemManager.Items[itemnum].AddHP.ToString(),
                Items.ItemManager.Items[itemnum].AddPP.ToString(),
                Items.ItemManager.Items[itemnum].AddAttack.ToString(),
                Items.ItemManager.Items[itemnum].AddDefense.ToString(),
                Items.ItemManager.Items[itemnum].AddSpAtk.ToString(),
                Items.ItemManager.Items[itemnum].AddSpDef.ToString(),
                Items.ItemManager.Items[itemnum].AddSpeed.ToString(),
                Items.ItemManager.Items[itemnum].AddEXP.ToString(),
                Items.ItemManager.Items[itemnum].AttackSpeed.ToString(),
                Items.ItemManager.Items[itemnum].RecruitBonus.ToString());

            //itemnum.ToString(),
            //ItemManager.Items[itemnum].Name,
            //ItemManager.Items[itemnum].Pic.ToString(),
            //((int)ItemManager.Items[itemnum].Type).ToString(),
            //ItemManager.Items[itemnum].Data1.ToString(),
            //ItemManager.Items[itemnum].Data2.ToString(),
            //ItemManager.Items[itemnum].Data3.ToString(),
            //ItemManager.Items[itemnum].StrReq.ToString(),
            //ItemManager.Items[itemnum].DefReq.ToString(),
            //ItemManager.Items[itemnum].SpeedReq.ToString(),
            //"0",//used to be classreq
            //((int)ItemManager.Items[itemnum].AccessReq).ToString(),
            //ItemManager.Items[itemnum].AddHP.ToString(),
            //ItemManager.Items[itemnum].AddMP.ToString(),
            //"0",//used to be addSP
            //ItemManager.Items[itemnum].AddAtk.ToString(),
            //ItemManager.Items[itemnum].AddDef.ToString(),
            //ItemManager.Items[itemnum].AddSpclAtk.ToString(),
            //ItemManager.Items[itemnum].AddSpeed.ToString(),
            //ItemManager.Items[itemnum].AddEXP.ToString(),
            //ItemManager.Items[itemnum].Desc,
            //ItemManager.Items[itemnum].AttackSpeed.ToString(),
            //ItemManager.Items[itemnum].Price.ToString(),
            //ItemManager.Items[itemnum].Stackable.ToIntString(),
            //ItemManager.Items[itemnum].Bound.ToIntString(),
            //ItemManager.Items[itemnum].RecruitBonus.ToString());
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateMissionToAll(int MissionIndex) {
            //string TcpPacket = "UPDATEMISSION" + TcpManager.SEP_CHAR + MissionIndex.ToString() + TcpManager.SEP_CHAR;
            //if (Program.ClassMan.mMissions.mMissions.ContainsKey(MissionIndex)) {
            //    TcpPacket = TcpPacket + Program.ClassMan.mMissions[MissionIndex].Name + TcpManager.SEP_CHAR;
            //} else {
            //    TcpPacket = TcpPacket + "" + TcpManager.SEP_CHAR;
            //}
            //TcpPacket = TcpPacket + TcpManager.END_CHAR;
            //SendDataToAll(TcpPacket);
        }

        public static void SendUpdateMoveToAll(int SpellNum) {
            TcpPacket packet = new TcpPacket("updatespell");
            packet.AppendParameters(SpellNum.ToString(),
                    MoveManager.Moves[SpellNum].Name,
                    ((int)MoveManager.Moves[SpellNum].RangeType).ToString(),
                    MoveManager.Moves[SpellNum].Range.ToString(),
                    ((int)MoveManager.Moves[SpellNum].TargetType).ToString(),
                    MoveManager.Moves[SpellNum].HitTime.ToString(),
                    MoveManager.Moves[SpellNum].HitFreeze.ToIntString());
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateNpcToAll(int NPCNum) {
            TcpPacket packet = new TcpPacket("updatenpc");
            packet.AppendParameters(NPCNum.ToString(), NpcManager.Npcs[NPCNum].Name);
            SendDataToAll(packet);
        }

        public static void SendAddNpcToAll(int npcNum)
        {
            TcpPacket packet = new TcpPacket("npcadded");
            packet.AppendParameter(npcNum);
            if (NpcManager.Npcs.Npcs.ContainsKey(npcNum))
            {
                packet.AppendParameters(NpcManager.Npcs[npcNum].Name);
            }
            else
            {
                packet.AppendParameters("");
            }
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendAddRDungeonToAll(int dungeonIndex)
        {
            TcpPacket packet = new TcpPacket("rdungeonadded");
            packet.AppendParameter(dungeonIndex);
            if (RDungeonManager.RDungeons.RDungeons.ContainsKey(dungeonIndex))
            {
                packet.AppendParameters(RDungeonManager.RDungeons[dungeonIndex].DungeonName);
            }
            else
            {
                packet.AppendParameters("");
            }
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateRDungeonToAll(int dungeonIndex) {
            TcpPacket packet = new TcpPacket("rdungeonupdate");
            packet.AppendParameter(dungeonIndex);
            if (RDungeonManager.RDungeons.RDungeons.ContainsKey(dungeonIndex)) {
                packet.AppendParameters(RDungeonManager.RDungeons[dungeonIndex].DungeonName);
            } else {
                packet.AppendParameters("");
            }
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateShopToAll(int ShopNum) {
            TcpPacket packet = new TcpPacket("updateshop");
            packet.AppendParameters(ShopNum.ToString(), ShopManager.Shops[ShopNum].Name);
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendUpdateDungeonToAll(int DungeonIndex) {
            TcpPacket packet = new TcpPacket("updatedungeon");
            packet.AppendParameter(DungeonIndex);
            if (DungeonManager.Dungeons.Dungeons.ContainsKey(DungeonIndex)) {
                packet.AppendParameters(DungeonManager.Dungeons[DungeonIndex].Name);
            } else {
                packet.AppendParameters("");
            }
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        public static void SendAddDungeonToAll(int dungeonIndex) {
            TcpPacket packet = new TcpPacket("dungeonadded");
            packet.AppendParameter(dungeonIndex);
            if (DungeonManager.Dungeons.Dungeons.ContainsKey(dungeonIndex)) {
                packet.AppendParameters(DungeonManager.Dungeons[dungeonIndex].Name);
            } else {
                packet.AppendParameters("");
            }
            packet.FinalizePacket();
            SendDataToAll(packet);
        }

        
        public static void SendEditMissionTo(Client client, int DifficultyNum) {
            TcpPacket packet = new TcpPacket("editmission");
            packet.AppendParameter(DifficultyNum);
            packet.AppendParameter(WonderMailManager.Missions.MissionPools[DifficultyNum].MissionClients.Count);
            for (int i = 0; i < WonderMailManager.Missions.MissionPools[DifficultyNum].MissionClients.Count; i++) {
                packet.AppendParameters(WonderMailManager.Missions.MissionPools[DifficultyNum].MissionClients[i].Species,
                    WonderMailManager.Missions.MissionPools[DifficultyNum].MissionClients[i].Form);
            }
            packet.AppendParameter(WonderMailManager.Missions.MissionPools[DifficultyNum].Enemies.Count);
            for (int i = 0; i < WonderMailManager.Missions.MissionPools[DifficultyNum].Enemies.Count; i++) {
                packet.AppendParameter(WonderMailManager.Missions.MissionPools[DifficultyNum].Enemies[i].NpcNum);
            }
            packet.AppendParameter(WonderMailManager.Missions.MissionPools[DifficultyNum].Rewards.Count);
            for (int i = 0; i < WonderMailManager.Missions.MissionPools[DifficultyNum].Rewards.Count; i++) {
                packet.AppendParameters(WonderMailManager.Missions.MissionPools[DifficultyNum].Rewards[i].ItemNum,
                    WonderMailManager.Missions.MissionPools[DifficultyNum].Rewards[i].Amount);
                packet.AppendParameter(WonderMailManager.Missions.MissionPools[DifficultyNum].Rewards[i].Tag);
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendEditMoveTo(Client client, int MoveNum) {
            TcpPacket packet = new TcpPacket("editmove");
            packet.AppendParameters(MoveNum.ToString(),
                                    MoveManager.Moves[MoveNum].Name,
                //MoveManager.Moves[MoveNum].LevelReq.ToString(),
                                    MoveManager.Moves[MoveNum].MaxPP.ToString(),
                                    ((int)MoveManager.Moves[MoveNum].EffectType).ToString(),
                                    ((int)MoveManager.Moves[MoveNum].Element).ToString(),
                                    ((int)MoveManager.Moves[MoveNum].MoveCategory).ToString(),
                                    ((int)MoveManager.Moves[MoveNum].RangeType).ToString(),
                                    MoveManager.Moves[MoveNum].Range.ToString(),
                                    ((int)MoveManager.Moves[MoveNum].TargetType).ToString(),
                                    MoveManager.Moves[MoveNum].Data1.ToString(),
                                    MoveManager.Moves[MoveNum].Data2.ToString(),
                                    MoveManager.Moves[MoveNum].Data3.ToString(),
                                    MoveManager.Moves[MoveNum].Accuracy.ToString(),
                                    MoveManager.Moves[MoveNum].HitTime.ToString(),
                                    MoveManager.Moves[MoveNum].HitFreeze.ToIntString(),
                                    MoveManager.Moves[MoveNum].AdditionalEffectData1.ToString(),
                                    MoveManager.Moves[MoveNum].AdditionalEffectData2.ToString(),
                                    MoveManager.Moves[MoveNum].AdditionalEffectData3.ToString(),
                                    MoveManager.Moves[MoveNum].PerPlayer.ToIntString(),
                                    MoveManager.Moves[MoveNum].KeyItem.ToString(),
                                    MoveManager.Moves[MoveNum].Sound.ToString(),
                                    ((int)MoveManager.Moves[MoveNum].AttackerAnim.AnimationType).ToString(),
                                    MoveManager.Moves[MoveNum].AttackerAnim.AnimationIndex.ToString(),
                                    MoveManager.Moves[MoveNum].AttackerAnim.FrameSpeed.ToString(),
                                    MoveManager.Moves[MoveNum].AttackerAnim.Repetitions.ToString(),
                                    ((int)MoveManager.Moves[MoveNum].TravelingAnim.AnimationType).ToString(),
                                    MoveManager.Moves[MoveNum].TravelingAnim.AnimationIndex.ToString(),
                                    MoveManager.Moves[MoveNum].TravelingAnim.FrameSpeed.ToString(),
                                    MoveManager.Moves[MoveNum].TravelingAnim.Repetitions.ToString(),
                                    ((int)MoveManager.Moves[MoveNum].DefenderAnim.AnimationType).ToString(),
                                    MoveManager.Moves[MoveNum].DefenderAnim.AnimationIndex.ToString(),
                                    MoveManager.Moves[MoveNum].DefenderAnim.FrameSpeed.ToString(),
                                    MoveManager.Moves[MoveNum].DefenderAnim.Repetitions.ToString());
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendEditNpcTo(Client client, int NPCNum) {
            TcpPacket packet = new TcpPacket("editnpc");
            packet.AppendParameters(
                 NpcManager.Npcs[NPCNum].Name,
                 NpcManager.Npcs[NPCNum].AttackSay,
                 NpcManager.Npcs[NPCNum].Form.ToString(),
                 NpcManager.Npcs[NPCNum].Species.ToString(),
                 NpcManager.Npcs[NPCNum].ShinyChance.ToString(),
                 ((int)NpcManager.Npcs[NPCNum].Behavior).ToString(),
                 NpcManager.Npcs[NPCNum].RecruitRate.ToString(),
                 NpcManager.Npcs[NPCNum].AIScript,
                 NpcManager.Npcs[NPCNum].SpawnsAtDawn.ToIntString(),
                 NpcManager.Npcs[NPCNum].SpawnsAtDay.ToIntString(),
                 NpcManager.Npcs[NPCNum].SpawnsAtDusk.ToIntString(),
                 NpcManager.Npcs[NPCNum].SpawnsAtNight.ToIntString()
             );

            for (int i = 0; i < NpcManager.Npcs[NPCNum].Moves.Length; i++) {
                packet.AppendParameter(NpcManager.Npcs[NPCNum].Moves[i]);
            }
            for (int i = 0; i < NpcManager.Npcs[NPCNum].Drops.Length; i++) {
                packet.AppendParameters(
                    NpcManager.Npcs[NPCNum].Drops[i].ItemNum.ToString(),
                    NpcManager.Npcs[NPCNum].Drops[i].ItemValue.ToString(),
                    NpcManager.Npcs[NPCNum].Drops[i].Chance.ToString(),
                    NpcManager.Npcs[NPCNum].Drops[i].Tag
                    );
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendEditRDungeonTo(Client client, int dungeonIndex) {
            RDungeons.RDungeon dungeon = RDungeons.RDungeonManager.RDungeons[dungeonIndex];
            TcpPacket packet = new TcpPacket("editrdungeon");
            packet.AppendParameter(dungeonIndex.ToString());
            if (dungeon == null)
                dungeon = new RDungeon(dungeonIndex);
            packet.AppendParameters(dungeon.DungeonName, ((int)dungeon.Direction).ToString(), dungeon.Floors.Count.ToString(), dungeon.Recruitment.ToIntString(), dungeon.Exp.ToIntString(), dungeon.WindTimer.ToString(), dungeon.DungeonIndex.ToString());
            for (int i = 0; i < dungeon.Floors.Count; i++) {
                //Generator Options
                packet.AppendParameters(dungeon.Floors[i].Options.TrapMin.ToString(), dungeon.Floors[i].Options.TrapMax.ToString(),
                                        dungeon.Floors[i].Options.ItemMin.ToString(), dungeon.Floors[i].Options.ItemMax.ToString(),
                                        dungeon.Floors[i].Options.Intricacy.ToString(),
                                        dungeon.Floors[i].Options.RoomWidthMin.ToString(), dungeon.Floors[i].Options.RoomWidthMax.ToString(),
                                        dungeon.Floors[i].Options.RoomLengthMin.ToString(), dungeon.Floors[i].Options.RoomLengthMax.ToString(),
                                        dungeon.Floors[i].Options.HallTurnMin.ToString(), dungeon.Floors[i].Options.HallTurnMax.ToString(),
                                        dungeon.Floors[i].Options.HallVarMin.ToString(), dungeon.Floors[i].Options.HallVarMax.ToString(),
                                        dungeon.Floors[i].Options.WaterFrequency.ToString(), dungeon.Floors[i].Options.Craters.ToString(),
                                        dungeon.Floors[i].Options.CraterMinLength.ToString(), dungeon.Floors[i].Options.CraterMaxLength.ToString(),
                                        dungeon.Floors[i].Options.CraterFuzzy.ToIntString(),
                                        dungeon.Floors[i].Options.MinChambers.ToString(), dungeon.Floors[i].Options.MaxChambers.ToString());
                packet.AppendParameters(dungeon.Floors[i].Darkness, (int)dungeon.Floors[i].GoalType, dungeon.Floors[i].GoalMap, dungeon.Floors[i].GoalX, dungeon.Floors[i].GoalY);
                packet.AppendParameter(dungeon.Floors[i].Music);
                //Terrain
                packet.AppendParameters(dungeon.Floors[i].StairsX.ToString(), dungeon.Floors[i].StairsSheet.ToString(),
                                        dungeon.Floors[i].mGroundX.ToString(), dungeon.Floors[i].mGroundSheet.ToString(),

                                        dungeon.Floors[i].mTopLeftX.ToString(), dungeon.Floors[i].mTopLeftSheet.ToString(),
                                        dungeon.Floors[i].mTopCenterX.ToString(), dungeon.Floors[i].mTopCenterSheet.ToString(),
                                        dungeon.Floors[i].mTopRightX.ToString(), dungeon.Floors[i].mTopRightSheet.ToString(),

                                        dungeon.Floors[i].mCenterLeftX.ToString(), dungeon.Floors[i].mCenterLeftSheet.ToString(),
                                        dungeon.Floors[i].mCenterCenterX.ToString(), dungeon.Floors[i].mCenterCenterSheet.ToString(),
                                        dungeon.Floors[i].mCenterRightX.ToString(), dungeon.Floors[i].mCenterRightSheet.ToString(),

                                        dungeon.Floors[i].mBottomLeftX.ToString(), dungeon.Floors[i].mBottomLeftSheet.ToString(),
                                        dungeon.Floors[i].mBottomCenterX.ToString(), dungeon.Floors[i].mBottomCenterSheet.ToString(),
                                        dungeon.Floors[i].mBottomRightX.ToString(), dungeon.Floors[i].mBottomRightSheet.ToString(),

                                        dungeon.Floors[i].mInnerTopLeftX.ToString(), dungeon.Floors[i].mInnerTopLeftSheet.ToString(),
                                        dungeon.Floors[i].mInnerBottomLeftX.ToString(), dungeon.Floors[i].mInnerBottomLeftSheet.ToString(),
                                        dungeon.Floors[i].mInnerTopRightX.ToString(), dungeon.Floors[i].mInnerTopRightSheet.ToString(),
                                        dungeon.Floors[i].mInnerBottomRightX.ToString(), dungeon.Floors[i].mInnerBottomRightSheet.ToString(),
                                        dungeon.Floors[i].mIsolatedWallX.ToString(), dungeon.Floors[i].mIsolatedWallSheet.ToString(),

                                        dungeon.Floors[i].mColumnTopX.ToString(), dungeon.Floors[i].mColumnTopSheet.ToString(),
                                        dungeon.Floors[i].mColumnCenterX.ToString(), dungeon.Floors[i].mColumnCenterSheet.ToString(),
                                        dungeon.Floors[i].mColumnBottomX.ToString(), dungeon.Floors[i].mColumnBottomSheet.ToString(),

                                        dungeon.Floors[i].mRowLeftX.ToString(), dungeon.Floors[i].mRowLeftSheet.ToString(),
                                        dungeon.Floors[i].mRowCenterX.ToString(), dungeon.Floors[i].mRowCenterSheet.ToString(),
                                        dungeon.Floors[i].mRowRightX.ToString(), dungeon.Floors[i].mRowRightSheet.ToString());

                packet.AppendParameters(dungeon.Floors[i].mGroundAltX.ToString(), dungeon.Floors[i].mGroundAltSheet.ToString(),
                                        dungeon.Floors[i].mGroundAlt2X.ToString(), dungeon.Floors[i].mGroundAlt2Sheet.ToString(),

                                        dungeon.Floors[i].mTopLeftAltX.ToString(), dungeon.Floors[i].mTopLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mTopCenterAltX.ToString(), dungeon.Floors[i].mTopCenterAltSheet.ToString(),
                                        dungeon.Floors[i].mTopRightAltX.ToString(), dungeon.Floors[i].mTopRightAltSheet.ToString(),

                                        dungeon.Floors[i].mCenterLeftAltX.ToString(), dungeon.Floors[i].mCenterLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mCenterCenterAltX.ToString(), dungeon.Floors[i].mCenterCenterAltSheet.ToString(),
                                        dungeon.Floors[i].mCenterCenterAlt2X.ToString(), dungeon.Floors[i].mCenterCenterAlt2Sheet.ToString(),
                                        dungeon.Floors[i].mCenterRightAltX.ToString(), dungeon.Floors[i].mCenterRightAltSheet.ToString(),

                                        dungeon.Floors[i].mBottomLeftAltX.ToString(), dungeon.Floors[i].mBottomLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mBottomCenterAltX.ToString(), dungeon.Floors[i].mBottomCenterAltSheet.ToString(),
                                        dungeon.Floors[i].mBottomRightAltX.ToString(), dungeon.Floors[i].mBottomRightAltSheet.ToString(),

                                        dungeon.Floors[i].mInnerTopLeftAltX.ToString(), dungeon.Floors[i].mInnerTopLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mInnerBottomLeftAltX.ToString(), dungeon.Floors[i].mInnerBottomLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mInnerTopRightAltX.ToString(), dungeon.Floors[i].mInnerTopRightAltSheet.ToString(),
                                        dungeon.Floors[i].mInnerBottomRightAltX.ToString(), dungeon.Floors[i].mInnerBottomRightAltSheet.ToString(),
                                        dungeon.Floors[i].mIsolatedWallAltX.ToString(), dungeon.Floors[i].mIsolatedWallAltSheet.ToString(),

                                        dungeon.Floors[i].mColumnTopAltX.ToString(), dungeon.Floors[i].mColumnTopAltSheet.ToString(),
                                        dungeon.Floors[i].mColumnCenterAltX.ToString(), dungeon.Floors[i].mColumnCenterAltSheet.ToString(),
                                        dungeon.Floors[i].mColumnBottomAltX.ToString(), dungeon.Floors[i].mColumnBottomAltSheet.ToString(),

                                        dungeon.Floors[i].mRowLeftAltX.ToString(), dungeon.Floors[i].mRowLeftAltSheet.ToString(),
                                        dungeon.Floors[i].mRowCenterAltX.ToString(), dungeon.Floors[i].mRowCenterAltSheet.ToString(),
                                        dungeon.Floors[i].mRowRightAltX.ToString(), dungeon.Floors[i].mRowRightAltSheet.ToString());

                packet.AppendParameters(dungeon.Floors[i].mWaterX.ToString(), dungeon.Floors[i].mWaterSheet.ToString(),
                                        dungeon.Floors[i].mWaterAnimX.ToString(), dungeon.Floors[i].mWaterAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreTopLeftX.ToString(), dungeon.Floors[i].mShoreTopLeftSheet.ToString(),
                                        dungeon.Floors[i].mShoreTopRightX.ToString(), dungeon.Floors[i].mShoreTopRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomRightX.ToString(), dungeon.Floors[i].mShoreBottomRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomLeftX.ToString(), dungeon.Floors[i].mShoreBottomLeftSheet.ToString(),

                                        dungeon.Floors[i].mShoreDiagonalForwardX.ToString(), dungeon.Floors[i].mShoreDiagonalForwardSheet.ToString(),
                                        dungeon.Floors[i].mShoreDiagonalBackX.ToString(), dungeon.Floors[i].mShoreDiagonalBackSheet.ToString(),

                                        dungeon.Floors[i].mShoreTopX.ToString(), dungeon.Floors[i].mShoreTopSheet.ToString(),
                                        dungeon.Floors[i].mShoreRightX.ToString(), dungeon.Floors[i].mShoreRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomX.ToString(), dungeon.Floors[i].mShoreBottomSheet.ToString(),
                                        dungeon.Floors[i].mShoreLeftX.ToString(), dungeon.Floors[i].mShoreLeftSheet.ToString(),

                                        dungeon.Floors[i].mShoreVerticalX.ToString(), dungeon.Floors[i].mShoreVerticalSheet.ToString(),
                                        dungeon.Floors[i].mShoreHorizontalX.ToString(), dungeon.Floors[i].mShoreHorizontalSheet.ToString(),

                                        dungeon.Floors[i].mShoreInnerTopLeftX.ToString(), dungeon.Floors[i].mShoreInnerTopLeftSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerTopRightX.ToString(), dungeon.Floors[i].mShoreInnerTopRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomRightX.ToString(), dungeon.Floors[i].mShoreInnerBottomRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomLeftX.ToString(), dungeon.Floors[i].mShoreInnerBottomLeftSheet.ToString(),

                                        dungeon.Floors[i].mShoreInnerTopX.ToString(), dungeon.Floors[i].mShoreInnerTopSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerRightX.ToString(), dungeon.Floors[i].mShoreInnerRightSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomX.ToString(), dungeon.Floors[i].mShoreInnerBottomSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerLeftX.ToString(), dungeon.Floors[i].mShoreInnerLeftSheet.ToString(),

                                        dungeon.Floors[i].mShoreSurroundedX.ToString(), dungeon.Floors[i].mShoreSurroundedSheet.ToString());

                packet.AppendParameters(dungeon.Floors[i].mShoreTopLeftAnimX.ToString(), dungeon.Floors[i].mShoreTopLeftAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreTopRightAnimX.ToString(), dungeon.Floors[i].mShoreTopRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomRightAnimX.ToString(), dungeon.Floors[i].mShoreBottomRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomLeftAnimX.ToString(), dungeon.Floors[i].mShoreBottomLeftAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreDiagonalForwardAnimX.ToString(), dungeon.Floors[i].mShoreDiagonalForwardAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreDiagonalBackAnimX.ToString(), dungeon.Floors[i].mShoreDiagonalBackAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreTopAnimX.ToString(), dungeon.Floors[i].mShoreTopAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreRightAnimX.ToString(), dungeon.Floors[i].mShoreRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreBottomAnimX.ToString(), dungeon.Floors[i].mShoreBottomAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreLeftAnimX.ToString(), dungeon.Floors[i].mShoreLeftAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreVerticalAnimX.ToString(), dungeon.Floors[i].mShoreVerticalAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreHorizontalAnimX.ToString(), dungeon.Floors[i].mShoreHorizontalAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreInnerTopLeftAnimX.ToString(), dungeon.Floors[i].mShoreInnerTopLeftAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerTopRightAnimX.ToString(), dungeon.Floors[i].mShoreInnerTopRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomRightAnimX.ToString(), dungeon.Floors[i].mShoreInnerBottomRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomLeftAnimX.ToString(), dungeon.Floors[i].mShoreInnerBottomLeftAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreInnerTopAnimX.ToString(), dungeon.Floors[i].mShoreInnerTopAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerRightAnimX.ToString(), dungeon.Floors[i].mShoreInnerRightAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerBottomAnimX.ToString(), dungeon.Floors[i].mShoreInnerBottomAnimSheet.ToString(),
                                        dungeon.Floors[i].mShoreInnerLeftAnimX.ToString(), dungeon.Floors[i].mShoreInnerLeftAnimSheet.ToString(),

                                        dungeon.Floors[i].mShoreSurroundedAnimX.ToString(), dungeon.Floors[i].mShoreSurroundedAnimSheet.ToString());
                packet.AppendParameters(((int)dungeon.Floors[i].GroundTile.Type).ToString(),
                                dungeon.Floors[i].GroundTile.Data1.ToString(),
                                dungeon.Floors[i].GroundTile.Data2.ToString(),
                                dungeon.Floors[i].GroundTile.Data3.ToString(),
                                dungeon.Floors[i].GroundTile.String1,
                                dungeon.Floors[i].GroundTile.String2,
                                dungeon.Floors[i].GroundTile.String3,

                                ((int)dungeon.Floors[i].HallTile.Type).ToString(),
                                dungeon.Floors[i].HallTile.Data1.ToString(),
                                dungeon.Floors[i].HallTile.Data2.ToString(),
                                dungeon.Floors[i].HallTile.Data3.ToString(),
                                dungeon.Floors[i].HallTile.String1,
                                dungeon.Floors[i].HallTile.String2,
                                dungeon.Floors[i].HallTile.String3,

                                ((int)dungeon.Floors[i].WaterTile.Type).ToString(),
                                dungeon.Floors[i].WaterTile.Data1.ToString(),
                                dungeon.Floors[i].WaterTile.Data2.ToString(),
                                dungeon.Floors[i].WaterTile.Data3.ToString(),
                                dungeon.Floors[i].WaterTile.String1,
                                dungeon.Floors[i].WaterTile.String2,
                                dungeon.Floors[i].WaterTile.String3,

                                ((int)dungeon.Floors[i].WallTile.Type).ToString(),
                                dungeon.Floors[i].WallTile.Data1.ToString(),
                                dungeon.Floors[i].WallTile.Data2.ToString(),
                                dungeon.Floors[i].WallTile.Data3.ToString(),
                                dungeon.Floors[i].WallTile.String1,
                                dungeon.Floors[i].WallTile.String2,
                                dungeon.Floors[i].WallTile.String3,

                                dungeon.Floors[i].NpcSpawnTime.ToString(),
                                dungeon.Floors[i].NpcMin.ToString(),
                                dungeon.Floors[i].NpcMax.ToString());

                packet.AppendParameter(dungeon.Floors[i].Items.Count);
                for (int item = 0; item < dungeon.Floors[i].Items.Count; item++) {
                    packet.AppendParameters(dungeon.Floors[i].Items[item].ItemNum.ToString(),
                                            dungeon.Floors[i].Items[item].MinAmount.ToString(),
                                            dungeon.Floors[i].Items[item].MaxAmount.ToString(),
                                            dungeon.Floors[i].Items[item].AppearanceRate.ToString(),
                                            dungeon.Floors[i].Items[item].StickyRate.ToString(),
                                            dungeon.Floors[i].Items[item].Tag,
                                            dungeon.Floors[i].Items[item].Hidden.ToIntString(),
                                            dungeon.Floors[i].Items[item].OnGround.ToIntString(),
                                            dungeon.Floors[i].Items[item].OnWater.ToIntString(),
                                            dungeon.Floors[i].Items[item].OnWall.ToIntString());
                }

                packet.AppendParameter(dungeon.Floors[i].Npcs.Count);
                for (int npc = 0; npc < dungeon.Floors[i].Npcs.Count; npc++) {
                    packet.AppendParameters(dungeon.Floors[i].Npcs[npc].NpcNum.ToString(),
                                            dungeon.Floors[i].Npcs[npc].MinLevel.ToString(),
                                            dungeon.Floors[i].Npcs[npc].MaxLevel.ToString(),
                                            dungeon.Floors[i].Npcs[npc].AppearanceRate.ToString(),
                                            ((int)dungeon.Floors[i].Npcs[npc].StartStatus).ToString(),
                                            dungeon.Floors[i].Npcs[npc].StartStatusCounter.ToString(),
                                            dungeon.Floors[i].Npcs[npc].StartStatusChance.ToString());
                }

                packet.AppendParameter(dungeon.Floors[i].SpecialTiles.Count);
                for (int trap = 0; trap < dungeon.Floors[i].SpecialTiles.Count; trap++) {
                    packet.AppendParameters(((int)dungeon.Floors[i].SpecialTiles[trap].Type).ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Data1.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Data2.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Data3.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].String1,
                                            dungeon.Floors[i].SpecialTiles[trap].String2,
                                            dungeon.Floors[i].SpecialTiles[trap].String3,
                                            dungeon.Floors[i].SpecialTiles[trap].Ground.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].GroundSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].GroundAnim.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].GroundAnimSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Mask.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].MaskSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Anim.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].AnimSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Mask2.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Mask2Set.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].M2Anim.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].M2AnimSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Fringe.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].FringeSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].FAnim.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].FAnimSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Fringe2.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].Fringe2Set.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].F2Anim.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].F2AnimSet.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].RDungeonMapValue.ToString(),
                                            dungeon.Floors[i].SpecialTiles[trap].AppearanceRate.ToString());
                }

                packet.AppendParameter(dungeon.Floors[i].Weather.Count);
                for (int weather = 0; weather < dungeon.Floors[i].Weather.Count; weather++) {
                    packet.AppendParameters(((int)dungeon.Floors[i].Weather[weather]).ToString());
                }

                packet.AppendParameter(dungeon.Floors[i].Options.Chambers.Count);
                for (int chamber = 0; chamber < dungeon.Floors[i].Options.Chambers.Count; chamber++) {
                    packet.AppendParameters(dungeon.Floors[i].Options.Chambers[chamber].ChamberNum.ToString(),
                                            dungeon.Floors[i].Options.Chambers[chamber].String1,
                                            dungeon.Floors[i].Options.Chambers[chamber].String2,
                                            dungeon.Floors[i].Options.Chambers[chamber].String3);
                }
            }
            packet.FinalizePacket();

            SendDataTo(client, packet, true, false);
        }

        public static void SendEditDungeonTo(Client client, int DungeonIndex) {
            if (DungeonManager.Dungeons.Dungeons.ContainsKey(DungeonIndex)) {
                Dungeons.Dungeon dungeon = DungeonManager.Dungeons[DungeonIndex];
                TcpPacket packet = new TcpPacket("editdungeon");
                packet.AppendParameters(DungeonIndex.ToString(), dungeon.Name, dungeon.AllowsRescue.ToIntString(), dungeon.ScriptList.Count.ToString());
                for (int i = 0; i < dungeon.ScriptList.Count; i++) {
                    packet.AppendParameters(dungeon.ScriptList.KeyByIndex(i).ToString(), dungeon.ScriptList.ValueByIndex(i));
                }
                packet.AppendParameter(dungeon.StandardMaps.Count);
                for (int i = 0; i < dungeon.StandardMaps.Count; i++) {
                    StandardDungeonMap map = dungeon.StandardMaps[i];
                    //MapGeneralInfo mapGeneralInfo = MapManager.RetrieveMapGeneralInfo(System.Math.Max(1, map.MapNum));
                    packet.AppendParameters(((int)map.Difficulty).ToString(), map.IsBadGoalMap.ToIntString(), map.MapNum.ToString());//, mapGeneralInfo.Name);
                }
                packet.AppendParameter(dungeon.RandomMaps.Count);
                for (int i = 0; i < dungeon.RandomMaps.Count; i++) {
                    RandomDungeonMap map = dungeon.RandomMaps[i];
                    packet.AppendParameters(((int)map.Difficulty).ToString(), map.IsBadGoalMap.ToIntString(), map.RDungeonIndex.ToString(), map.RDungeonFloor.ToString());
                }
                packet.FinalizePacket();
                SendDataTo(client, packet);
            }
        }

        public static void SendEditShopTo(Client client, int ShopNum) {
            TcpPacket packet = new TcpPacket("editshop");
            packet.AppendParameters(ShopNum.ToString(),
                                    ShopManager.Shops[ShopNum].Name.Trim(),
                                    ShopManager.Shops[ShopNum].JoinSay.Trim(),
                                    ShopManager.Shops[ShopNum].LeaveSay.Trim());

            for (int z = 0; z < Constants.MAX_TRADES; z++) {

                packet.AppendParameters(ShopManager.Shops[ShopNum].Items[z].GiveItem.ToString(),
                                        ShopManager.Shops[ShopNum].Items[z].GiveValue.ToString(),
                                        ShopManager.Shops[ShopNum].Items[z].GetItem.ToString());
            }

            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendEditStoryTo(Client client, int StoryNum) {
            TcpPacket packet = new TcpPacket("editstory");
            packet.AppendParameters(StoryNum.ToString(),
                                    StoryManager.Stories[StoryNum].Name.Trim(),
                                    StoryManager.Stories[StoryNum].Revision.ToString(),
                                    StoryManager.Stories[StoryNum].StoryStart.ToString(),
                                    StoryManager.Stories[StoryNum].Segments.Count.ToString());
            for (int i = 0; i < StoryManager.Stories[StoryNum].Segments.Count; i++) {
                if (StoryManager.Stories[StoryNum].Segments[i] == null)
                    StoryManager.Stories[StoryNum].Segments[i] = new StorySegment();
                packet.AppendParameters(StoryManager.Stories[StoryNum].Segments[i].Parameters.Count.ToString(),
                                        ((int)StoryManager.Stories[StoryNum].Segments[i].Action).ToString());
                for (int z = 0; z < StoryManager.Stories[StoryNum].Segments[i].Parameters.Count; z++) {
                    packet.AppendParameters(StoryManager.Stories[StoryNum].Segments[i].Parameters.KeyByIndex(z),
                                            StoryManager.Stories[StoryNum].Segments[i].Parameters.ValueByIndex(z));
                }
            }
            packet.AppendParameter(StoryManager.Stories[StoryNum].ExitAndContinue.Count.ToString());
            for (int i = 0; i < StoryManager.Stories[StoryNum].ExitAndContinue.Count; i++) {
                packet.AppendParameters(StoryManager.Stories[StoryNum].ExitAndContinue[i].ToString());
            }
            packet.FinalizePacket();
            SendDataTo(client, packet);
        }

        public static void SendLiveMapEditorTilePlacedData(Client client, int x, int y, int layer, int layerSet, int layerTile) {
            SendDataTo(client, TcpPacket.CreatePacket("mapeditortileplaced", x.ToString(), y.ToString(),
                layer.ToString(), layerSet.ToString(), layerTile.ToString()));
        }

        public static void SendLiveMapEditorAttributePlacedData(Client client, int x, int y, int type, int data1, int data2, int data3,
            string string1, string string2, string string3, int dungeonValue) {
            SendDataTo(client, TcpPacket.CreatePacket("mapeditorattribplaced", x.ToString(), y.ToString(),
                type.ToString(), data1.ToString(), data2.ToString(), data3.ToString(),
                string1, string2, string3, dungeonValue.ToString()));
        }

        public static void SendLiveMapEditorFillLayerData(Client client, int layer, int layerSet, int layerTile) {
            SendDataTo(client, TcpPacket.CreatePacket("mapeditorlayerfill",
                layer.ToString(), layerSet.ToString(), layerTile.ToString()));
        }

        public static void SendLiveMapEditorFillAttributeData(Client client, int type, int data1, int data2, int data3,
            string string1, string string2, string string3, int dungeonValue) {
            SendDataTo(client, TcpPacket.CreatePacket("mapeditorattribfill",
                type.ToString(), data1.ToString(), data2.ToString(), data3.ToString(),
                string1, string2, string3, dungeonValue.ToString()));
        }

        public static void PlaySound(Client client, string sound) {
            if (string.IsNullOrEmpty(sound)) {
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("sound", sound));
            }
        }

        public static void PlaySoundToMap(string mapID, string sound) {
            if (!string.IsNullOrEmpty(sound)) {
                Messenger.SendDataToMap(mapID, TcpPacket.CreatePacket("sound", sound));
            }
        }

        public static void PlayMusic(Client client, string song) {
            Messenger.SendDataTo(client, TcpPacket.CreatePacket("music", song));
        }

        public static void FadeOutMusic(Client client, int milliseconds) {
            Messenger.SendDataTo(client, TcpPacket.CreatePacket("fademusic", milliseconds.ToString()));
        }

        public static void SendMapLatestPropertiesTo(Client client) {
            TcpPacket packet = new TcpPacket("maplatestproperties");
            IMap map = client.Player.Map;
            if (map.MapType == Enums.MapType.Standard) {
                packet.AppendParameter(((Map)map).Instanced.ToIntString());
                packet.AppendParameter(map.Npc.Count);
                for (int i = 0; i < map.Npc.Count; i++) {
                    packet.AppendParameters(map.Npc[i].NpcNum.ToString(),
                        map.Npc[i].SpawnX.ToString(), map.Npc[i].SpawnY.ToString(),
                        map.Npc[i].MinLevel.ToString(), map.Npc[i].MaxLevel.ToString(),
                        map.Npc[i].AppearanceRate.ToString(),
                        ((int)map.Npc[i].StartStatus).ToString(),
                        map.Npc[i].StartStatusCounter.ToString(),
                        map.Npc[i].StartStatusChance.ToString());
                }
                packet.FinalizePacket();
                SendDataTo(client, packet);
            }
        }

        public static void SendHeartBeat(Client client) {
            SendDataTo(client, TcpPacket.CreatePacket("hb"));
        }

        public static void SendTournamentListingTo(Client client, PacketHitList hitList) {
            PacketHitList.MethodStart(ref hitList);

            hitList.AddPacket(client, PacketBuilder.CreateTournamentListingPacket());

            PacketHitList.MethodEnded(ref hitList);
        }

        public static void SendTournamentSpectateListingTo(Client client, PacketHitList hitList) {
            PacketHitList.MethodStart(ref hitList);

            hitList.AddPacket(client, PacketBuilder.CreateTournamentSpectateListingPacket());

            PacketHitList.MethodEnded(ref hitList);
        }

        public static void SendTournamentRulesEditorTo(Client client, PacketHitList hitList) {
            PacketHitList.MethodStart(ref hitList);

            Tournaments.Tournament tourny = client.Player.Tournament;
            if (tourny != null) {
                hitList.AddPacket(client, PacketBuilder.CreateTournamentRulesEditorPacket(tourny.Rules));
            }

            PacketHitList.MethodEnded(ref hitList);
        }

        public static void SendTournamentRulesTo(Client client, PacketHitList hitList, Tournaments.Tournament tournament) {
            PacketHitList.MethodStart(ref hitList);

            if (tournament != null) {
                hitList.AddPacket(client, PacketBuilder.CreateTournamentRulesPacket(tournament));
            }

            PacketHitList.MethodEnded(ref hitList);
        }

        #endregion Methods
    }
}