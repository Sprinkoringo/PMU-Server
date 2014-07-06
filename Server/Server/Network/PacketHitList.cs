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

using PMU.Core;
using PMU.Sockets;
using Server.Maps;
using Server.Players;

namespace Server.Network
{

    public class PacketHitList
    {
        Object lockObject = new object();
        bool cachingEnabled;
        PacketHitListCache hitListCache = null;
        bool buildingFromCache = false;

        public ListPair<Client, PacketList> HitList { get; set; }
        public int CallLevel { get; set; }

        public bool CachingEnabled {
            get { return cachingEnabled; }
            set {
                cachingEnabled = value;
                if (cachingEnabled) {
                    if (hitListCache == null) {
                        hitListCache = new PacketHitListCache();
                    }
                } else {
                    if (hitListCache != null && hitListCache.Count > 0) {
                        hitListCache.BuildHitList(this);
                    }
                }
            }
        }

        private void BuildHitListFromCache() {
            buildingFromCache = true;
            hitListCache.BuildHitList(this);
            buildingFromCache = false;
        }

        public PacketHitList() {
            HitList = new ListPair<Client, PacketList>();

            CachingEnabled = false; //Globals.PacketCaching;
        }

        public void AddHitList(PacketHitList hitlist) {
            for (int i = 0; i < hitlist.HitList.Count; i++) {
                AddPacketList(hitlist.HitList.KeyByIndex(i), hitlist.HitList.ValueByIndex(i));
            }

        }

        public void AddPacketList(Client client, PacketList packetList) {
            foreach (TcpPacket packet in packetList.Packets) {
                AddPacket(client, packet);
            }
        }

        public void AddPacket(Client client, TcpPacket packet) {
            AddPacket(client, packet, buildingFromCache);
        }

        public void AddPacket(Client client, TcpPacket packet, bool skipCaching) {
            if (client != null && packet != null) {
                if (!cachingEnabled || skipCaching) {
                    // Caching is disabled, add directly to the client's packetlist
                    lock (lockObject) {
                        int index = HitList.IndexOfKey(client);
                        PacketList packetList = null;
                        if (index == -1) {
                            packetList = new PacketList();
                            HitList.Add(client, packetList);
                        } else {
                            packetList = HitList.ValueByIndex(index);
                        }
                        packetList.AddPacket(packet);
                    }
                } else {
                    // Caching is enabled, add to the cache
                    hitListCache.AddCacheItem(new object[] { PacketHitListTargets.Client, client, packet });
                }

            }

        }

        public static void MethodStart(ref PacketHitList hitlist) {
            if (hitlist == null) {
                hitlist = new PacketHitList();
            } else {
                hitlist.CallLevel++;
            }
        }

        public static void MethodEnded(ref PacketHitList hitlist) {
            if (hitlist.CallLevel == 0) {
                if (hitlist.CachingEnabled) {
                    hitlist.BuildHitListFromCache();
                }
                Messenger.SendData(hitlist);
            } else {
                hitlist.CallLevel--;
            }
        }

        //public void AddPacketToMap(string mapID, TcpPacket packet) {
        //    AddPacketToMap(MapManager.RetrieveActiveMap(mapID), packet);
        //}

        //public void AddPacketToMap(string mapID, TcpPacket packet, int sourceX, int sourceY, int range) {
        //    AddPacketToMap(MapManager.RetrieveActiveMap(mapID), packet, sourceX, sourceY, range);
        //}

        public void AddPacketToMap(IMap map, TcpPacket packet) {
            AddPacketToMap(map, packet, 0, 0, -1);
        }

        public void AddPacketToMap(IMap map, TcpPacket packet, int sourceX, int sourceY, int range) {
            AddPacketToMapBut(null, map, packet, sourceX, sourceY, range);
        }

        public void AddPacketToSurroundingPlayersBut(Client client, IMap centralMap, TcpPacket packet) {
            AddPacketToPlayersInSightBut(client, centralMap, packet, 0, 0, -1);
        }

        public void AddPacketToSurroundingPlayers(IMap centralMap, TcpPacket packet) {
            AddPacketToPlayersInSightBut(null, centralMap, packet, 0, 0, -1);
        }

        public void AddPacketToPlayersInSight(IMap centralMap, TcpPacket packet, int sourceX, int sourceY, int range) {
            AddPacketToPlayersInSightBut(null, centralMap, packet, sourceX, sourceY, range);
        }

        public void AddPacketToPlayersInSightBut(Client sender, IMap centralMap, TcpPacket packet, int sourceX, int sourceY, int range) {
            if (centralMap != null) {

                if (!cachingEnabled) {
                    foreach (Client i in centralMap.GetClients()) {
                        AddPacketToPlayersInSightButInternal(sender, centralMap, packet, sourceX, sourceY, range, centralMap, Enums.MapID.Active, i);
                    }
                    // Go through all of the clients on the surrounding maps
                    for (int n = 1; n < 9; n++) {
                        string borderingMapID = MapManager.RetrieveBorderingMapID(centralMap, (Enums.MapID)n);
                        if (!string.IsNullOrEmpty(borderingMapID)) {
                            IMap borderingMap = MapManager.RetrieveActiveMap(borderingMapID);
                            if (borderingMap != null && SeamlessWorldHelper.IsMapSeamless(centralMap, borderingMap)) {
                                foreach (Client i in borderingMap.GetClients()) {
                                    AddPacketToPlayersInSightButInternal(sender, centralMap, packet, sourceX, sourceY, range, borderingMap, (Enums.MapID)n, i);
                                }
                            }
                        }
                    }

                } else {
                    hitListCache.AddCacheItem(new object[] { PacketHitListTargets.PlayersInSightBut, sender, centralMap, packet, sourceX, sourceY, range });
                }
            }
        }

        private void AddPacketToPlayersInSightButInternal(Client sender, IMap centralMap, TcpPacket packet, int sourceX, int sourceY, int range, IMap borderingMap, Enums.MapID borderingMapID, Client targetClient) {
            if (sender == null || sender != targetClient) {
                if (range == -1 || SeamlessWorldHelper.IsInSight(centralMap, sourceX, sourceY, borderingMap, borderingMapID, targetClient.Player.X, targetClient.Player.Y)) {
                    AddPacket(targetClient, packet);
                }
            }
        }

        //public void AddPacketToMapBut(Client client, string mapID, TcpPacket packet) {
        //    AddPacketToMapBut(client, MapManager.RetrieveActiveMap(mapID), packet);
        //}

        //public void AddPacketToMapBut(Client client, string mapID, TcpPacket packet, int sourceX, int sourceY, int range) {
        //    AddPacketToMapBut(client, MapManager.RetrieveActiveMap(mapID), packet, sourceX, sourceY, range);
        //}

        public void AddPacketToMapBut(Client client, IMap map, TcpPacket packet) {
            AddPacketToMapBut(client, map, packet, 0, 0, -1);
        }

        public void AddPacketToMapBut(Client client, IMap map, TcpPacket packet, int sourceX, int sourceY, int range) {
            if (map != null) {
                if (!cachingEnabled) {
                    foreach (Client i in map.GetClients()) {
                        if (client == null || i.TcpID != client.TcpID) {
                            if (range == -1 || IsInRange(range, sourceX, sourceY, i.Player.X, i.Player.Y)) {
                                AddPacket(i, packet);
                            }
                        }
                    }
                } else {
                    hitListCache.AddCacheItem(new object[] { PacketHitListTargets.RangedMapBut, client, map, packet, sourceX, sourceY, range });
                }
            }
        }

        public void AddPacketToAll(TcpPacket packet) {
            if (!cachingEnabled) {
                foreach (Client i in ClientManager.GetClients()) {
                    if (i.IsPlaying()) {
                        AddPacket(i, packet);
                    }
                }
            } else {
                hitListCache.AddCacheItem(new object[] { PacketHitListTargets.All, packet });
            }
        }

        public void AddPacketToParty(string partyID, TcpPacket packet) {
            AddPacketToParty(Players.Parties.PartyManager.FindParty(partyID), packet);
        }

        public void AddPacketToParty(Players.Parties.Party party, TcpPacket packet) {
            if (party != null) {
                if (!cachingEnabled) {
                    foreach (Client i in party.GetOnlineMemberClients()) {
                        if (i.IsPlaying()) {
                            AddPacket(i, packet);
                        }
                    }
                } else {
                    hitListCache.AddCacheItem(new object[] { PacketHitListTargets.Party, party, packet });
                }
            }
        }

        public void AddPacketToOthers(Combat.ICharacter source, IMap map, TcpPacket packet, Enums.OutdateType updateType) {
            if (map != null) {
                if (!cachingEnabled) {
                    for (int i = 0; i < 9; i++) {
                        string borderingMapID = MapManager.RetrieveBorderingMapID(map, (Enums.MapID)i);
                        if (!string.IsNullOrEmpty(borderingMapID)) {
                            IMap borderingMap = null;
                            if (i == 0) {
                                borderingMap = map;
                            } else {
                                borderingMap = MapManager.RetrieveActiveMap(borderingMapID);
                            }
                            if (borderingMap != null) {
                                foreach (Client n in borderingMap.GetClients()) {
                                    if (n.Player.GetActiveRecruit() != source) {
                                        if (AI.MovementProcessor.WillCharacterSeeCharacter(borderingMap, n.Player.GetActiveRecruit(), map, MapManager.GetOppositeMapID((Enums.MapID)i), source)) {
                                            //if (!n.Player.IsSeenCharacterSeen(source)) {
                                                n.Player.AddActivationIfCharacterNotSeen(this, source);
                                            //}
                                            this.AddPacket(n, packet);
                                        } else {
                                            if (updateType == Enums.OutdateType.Location && n.Player.IsSeenCharacterSeen(source)) {
                                                this.AddPacket(n, packet);
                                                //unsee character
                                                n.Player.RemoveUnseenCharacters();
                                            } else {
                                                n.Player.OutdateCharacter(source, updateType);
                                                //outdate character
                                                //}
                                                //} else {
                                                //outdate character
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    hitListCache.AddCacheItem(new object[] { PacketHitListTargets.Others, source, map, packet, updateType });
                }
            }
        }

        public static bool IsInRange(int range, int X1, int Y1, int X2, int Y2) {
            int DistanceX = System.Math.Abs(X1 - X2);
            int DistanceY = System.Math.Abs(Y1 - Y2);

            // Are they in range?
            if (DistanceX <= range && DistanceY <= range) {
                return true;
            } else {
                return false;
            }
        }

    }
}
