

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
    using Server.Tournaments;
    using DataManager.Players;
    using Server.Database;
    public class PacketBuilder
    {

        #region Logging In
        /*
        public static TcpPacket CreateAllArrowsPacket() {
            TcpPacket packet = new TcpPacket("allarrowsdata");
            for (int i = 1; i <= Constants.MAX_ARROWS; i++) {
                packet.AppendParameters(i.ToString(),
                                        Arrows.ArrowManagerBase.Arrows[i - 1].Name,
                                        Arrows.ArrowManagerBase.Arrows[i - 1].Pic.ToString(),
                                        Arrows.ArrowManagerBase.Arrows[i - 1].Range.ToString(),
                                        Arrows.ArrowManagerBase.Arrows[i - 1].Amount.ToString());
            }
            packet.FinalizePacket();
            return packet;
        }
        
        
        public static TcpPacket CreateAllDungeonsPacket() {
            TcpPacket packet = new TcpPacket("alldungeons");
            packet.AppendParameter(Dungeons.DungeonManager.Dungeons.Count.ToString());
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons.Count; i++) {
                packet.AppendParameters(Dungeons.DungeonManager.Dungeons[i].Name);
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllPokemonPacket() {
            TcpPacket packet = new TcpPacket("allpokemon");
            packet.AppendParameter(Constants.TOTAL_POKEMON);
            for (int i = 1; i <= Constants.TOTAL_POKEMON; i++) {
                packet.AppendParameters(Pokedex.Pokedex.GetPokemon(i).Name);
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllEmotionsPacket() {
            TcpPacket packet = new TcpPacket("allemoticonsdata");
            for (int i = 0; i <= Settings.MaxEmoticons; i++) {
                packet.AppendParameters(i.ToString(),
                                        Emoticons.EmoticonManagerBase.Emoticons[i].Command,
                                        Emoticons.EmoticonManagerBase.Emoticons[i].Pic.ToString());
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllEvosPacket() {
            TcpPacket packet = new TcpPacket("allevosdata");
            for (int i = 0; i <= Evolutions.EvolutionManager.Evolutions.MaxEvos; i++) {
                packet.AppendParameters(i.ToString(),
                                        Evolutions.EvolutionManager.Evolutions[i].Name);
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllItemsPacket() {
            TcpPacket packet = new TcpPacket("allitemsdata");
            for (int i = 1; i <= Items.ItemManager.Items.MaxItems; i++) {
                if (Items.ItemManager.Items[i].Name.Trim() != "") {
                    packet.AppendParameters(i.ToString(),
                                            Items.ItemManager.Items[i].Name.Trim(),
                                            Items.ItemManager.Items[i].Desc.Trim(),
                                            Items.ItemManager.Items[i].Pic.ToString(),
                                            ((int)Items.ItemManager.Items[i].Type).ToString(),
                                            Items.ItemManager.Items[i].Data1.ToString(),
                                            Items.ItemManager.Items[i].Data2.ToString(),
                                            Items.ItemManager.Items[i].Data3.ToString(),
                                            Items.ItemManager.Items[i].Price.ToString(),
                                            Items.ItemManager.Items[i].Stackable.ToIntString(),
                                            Items.ItemManager.Items[i].Bound.ToIntString(),
                                            Items.ItemManager.Items[i].Loseable.ToIntString(),
                                            Items.ItemManager.Items[i].Rarity.ToString(),
                                            Items.ItemManager.Items[i].AttackReq.ToString(),
                                            Items.ItemManager.Items[i].DefenseReq.ToString(),
                                            Items.ItemManager.Items[i].SpAtkReq.ToString(),
                                            Items.ItemManager.Items[i].SpDefReq.ToString(),
                                            Items.ItemManager.Items[i].SpeedReq.ToString(),
                                            Items.ItemManager.Items[i].ScriptedReq.ToString(),
                                            Items.ItemManager.Items[i].AddHP.ToString(),
                                            Items.ItemManager.Items[i].AddPP.ToString(),
                                            Items.ItemManager.Items[i].AddAttack.ToString(),
                                            Items.ItemManager.Items[i].AddDefense.ToString(),
                                            Items.ItemManager.Items[i].AddSpAtk.ToString(),
                                            Items.ItemManager.Items[i].AddSpDef.ToString(),
                                            Items.ItemManager.Items[i].AddSpeed.ToString(),
                                            Items.ItemManager.Items[i].AddEXP.ToString(),
                                            Items.ItemManager.Items[i].AttackSpeed.ToString(),
                                            Items.ItemManager.Items[i].RecruitBonus.ToString());
                } else {
                    packet.AppendParameters(i.ToString(), 0.ToString());
                }
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllNpcsPacket() {
            TcpPacket packet = new TcpPacket("allnpcsdata");
            for (int i = 1; i <= Npcs.NpcManager.Npcs.MaxNpcs; i++) {
                packet.AppendParameters(i.ToString(),
                                        Npcs.NpcManager.Npcs[i].Name.Trim(),
                                        Npcs.NpcManager.Npcs[i].Sprite.ToString());
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllRDungeonsPacket() {
            TcpPacket packet = new TcpPacket("allrdungeons");
            packet.AppendParameter(RDungeons.RDungeonManager.RDungeons.Count);
            for (int i = 0; i < RDungeons.RDungeonManager.RDungeons.Count; i++) {
                packet.AppendParameters(RDungeons.RDungeonManager.RDungeons[i].DungeonName.Trim());
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllShopsPacket() {
            TcpPacket packet = new TcpPacket("allshopsdata");
            for (int i = 1; i <= Shops.ShopManager.Shops.MaxShops; i++) {
                packet.AppendParameters(i.ToString(),
                                        Shops.ShopManager.Shops[i].Name.Trim());
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllSpellsPacket() {
            TcpPacket packet = new TcpPacket("allspellsdata");
            for (int i = 1; i <= Moves.MoveManager.Moves.MaxMoves; i++) {
                packet.AppendParameters(
                    i.ToString(),
                    Moves.MoveManager.Moves[i].Name.Trim(),
                    ((int)Moves.MoveManager.Moves[i].RangeType).ToString(),
                    Moves.MoveManager.Moves[i].Range.ToString(),
                    ((int)Moves.MoveManager.Moves[i].TargetType).ToString()
                    );
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateAllStoryNamesPacket() {
            TcpPacket packet = new TcpPacket("allstoriesdata");
            for (int i = 0; i <= Stories.StoryManager.Stories.MaxStories; i++) {
                if (!string.IsNullOrEmpty(Stories.StoryManager.Stories[i].Name)) {
                    packet.AppendParameters(Stories.StoryManager.Stories[i].Name);
                } else {
                    packet.AppendParameters("");
                }
            }
            packet.FinalizePacket();
            return packet;
        }

         */

        public static void AppendMaxInfo(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("maxinfo", Settings.GameName,
                                                      Items.ItemManager.Items.MaxItems.ToString(),
                                                      Npcs.NpcManager.Npcs.MaxNpcs.ToString(), Shops.ShopManager.Shops.MaxShops.ToString(),
                                                      Moves.MoveManager.Moves.MaxMoves.ToString(), Constants.MAX_MAP_ITEMS.ToString(),
                                                      Constants.MAX_MAP_X.ToString(), Constants.MAX_MAP_Y.ToString(),
                                                      /*Settings.MaxEmoticons*/"10", Evolutions.EvolutionManager.Evolutions.MaxEvos.ToString(),
                                                      Stories.StoryManager.Stories.MaxStories.ToString()));
        }

        public static void AppendConnectionID(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("myconid", client.ConnectionID.ToString()));
        }

        //public static void AppendAllArrows(Client client, PacketHitList hitlist) {
        //    TcpPacket packet = new TcpPacket("allarrowsdata");
        //    for (int i = 1; i <= Constants.MAX_ARROWS; i++) {
        //        packet.AppendParameters(i.ToString(),
        //                                Arrows.ArrowManagerBase.Arrows[i - 1].Name,
        //                                Arrows.ArrowManagerBase.Arrows[i - 1].Pic.ToString(),
        //                                Arrows.ArrowManagerBase.Arrows[i - 1].Range.ToString(),
        //                                Arrows.ArrowManagerBase.Arrows[i - 1].Amount.ToString());
        //    }
        //    packet.FinalizePacket();
        //    hitlist.AddPacket(client, packet);
        //}
        public static void AppendAllDungeons(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("alldungeons");
            packet.AppendParameter(Dungeons.DungeonManager.Dungeons.Count.ToString());
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons.Count; i++) {
                packet.AppendParameters(Dungeons.DungeonManager.Dungeons[i].Name);
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllPokemon(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allpokemon");
            packet.AppendParameter(Constants.TOTAL_POKEMON);
            for (int i = 1; i <= Constants.TOTAL_POKEMON; i++) {
                packet.AppendParameters(Pokedex.Pokedex.GetPokemon(i).Name,
                    Pokedex.Pokedex.GetPokemon(i).Forms.Count.ToString());
                //foreach (Pokedex.PokemonForm form in Pokedex.Pokedex.GetPokemon(i).Forms) {
                //    packet.AppendParameter(form.Sprite[0, 0]);
                //}
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllEvos(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allevosdata");
            for (int i = 0; i <= Evolutions.EvolutionManager.Evolutions.MaxEvos; i++) {
                packet.AppendParameters(i.ToString(),
                                        Evolutions.EvolutionManager.Evolutions[i].Name);
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllItems(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allitemsdata");
            for (int i = 0; i < Items.ItemManager.Items.MaxItems; i++) {
                if (Items.ItemManager.Items[i].Name.Trim() != "") {
                    packet.AppendParameters(i.ToString(),
                                            Items.ItemManager.Items[i].Name.Trim(),
                                            Items.ItemManager.Items[i].Desc.Trim(),
                                            Items.ItemManager.Items[i].Pic.ToString(),
                                            ((int)Items.ItemManager.Items[i].Type).ToString(),
                                            Items.ItemManager.Items[i].Data1.ToString(),
                                            Items.ItemManager.Items[i].Data2.ToString(),
                                            Items.ItemManager.Items[i].Data3.ToString(),
                                            Items.ItemManager.Items[i].Price.ToString(),
                                            Items.ItemManager.Items[i].StackCap.ToString(),
                                            Items.ItemManager.Items[i].Bound.ToIntString(),
                                            Items.ItemManager.Items[i].Loseable.ToIntString(),
                                            Items.ItemManager.Items[i].Rarity.ToString(),
                                            Items.ItemManager.Items[i].ReqData1.ToString(),
                                            Items.ItemManager.Items[i].ReqData2.ToString(),
                                            Items.ItemManager.Items[i].ReqData3.ToString(),
                                            Items.ItemManager.Items[i].ReqData4.ToString(),
                                            Items.ItemManager.Items[i].ReqData5.ToString(),
                                            Items.ItemManager.Items[i].ScriptedReq.ToString(),
                                            Items.ItemManager.Items[i].AddHP.ToString(),
                                            Items.ItemManager.Items[i].AddPP.ToString(),
                                            Items.ItemManager.Items[i].AddAttack.ToString(),
                                            Items.ItemManager.Items[i].AddDefense.ToString(),
                                            Items.ItemManager.Items[i].AddSpAtk.ToString(),
                                            Items.ItemManager.Items[i].AddSpDef.ToString(),
                                            Items.ItemManager.Items[i].AddSpeed.ToString(),
                                            Items.ItemManager.Items[i].AddEXP.ToString(),
                                            Items.ItemManager.Items[i].AttackSpeed.ToString(),
                                            Items.ItemManager.Items[i].RecruitBonus.ToString());
                } else {
                    packet.AppendParameters(i.ToString(), 0.ToString());
                }
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllNpcs(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allnpcsdata");
            for (int i = 1; i <= Npcs.NpcManager.Npcs.MaxNpcs; i++) {
                packet.AppendParameters(i.ToString(),
                                        Npcs.NpcManager.Npcs[i].Name.Trim());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllRDungeons(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allrdungeons");
            packet.AppendParameter(RDungeons.RDungeonManager.RDungeons.Count);
            for (int i = 0; i < RDungeons.RDungeonManager.RDungeons.Count; i++) {
                packet.AppendParameters(RDungeons.RDungeonManager.RDungeons[i].DungeonName.Trim());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllShops(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allshopsdata");
            for (int i = 1; i <= Shops.ShopManager.Shops.MaxShops; i++) {
                packet.AppendParameters(i.ToString(),
                                        Shops.ShopManager.Shops[i].Name.Trim());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllSpells(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allspellsdata");
            for (int i = 1; i <= Moves.MoveManager.Moves.MaxMoves; i++) {
                packet.AppendParameters(
                    i.ToString(),
                    Moves.MoveManager.Moves[i].Name.Trim(),
                    ((int)Moves.MoveManager.Moves[i].RangeType).ToString(),
                    Moves.MoveManager.Moves[i].Range.ToString(),
                    ((int)Moves.MoveManager.Moves[i].TargetType).ToString(),
                    Moves.MoveManager.Moves[i].HitTime.ToString(),
                    Moves.MoveManager.Moves[i].HitFreeze.ToIntString());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendAllStoryNames(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("allstoriesdata");
            for (int i = 0; i <= Stories.StoryManager.Stories.MaxStories; i++) {
                if (!string.IsNullOrEmpty(Stories.StoryManager.Stories[i].Name)) {
                    packet.AppendParameters(Stories.StoryManager.Stories[i].Name);
                } else {
                    packet.AppendParameters("");
                }
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendWhosOnline(Client client, PacketHitList hitlist) {
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
            hitlist.AddPacket(client, CreateChatMsg(s, Text.Grey));
        }

        #endregion

        #region Map Loading

        /*
        public static TcpPacket CreateMapDataPacket(IMap map) {
            TcpPacket packet = new TcpPacket("mapdata");
            int x, y;
            if (map.MaxX == 0)
                map.MaxX = 19;
            if (map.MaxY == 0)
                map.MaxY = 14;

            packet.AppendParameters(map.MapID,
                                    map.Name.Trim(),
                                    map.Revision.ToString(),
                                    ((int)map.Moral).ToString(),
                                    map.Up.ToString(),
                                    map.Down.ToString(),
                                    map.Left.ToString(),
                                    map.Right.ToString(),
                                    map.Music,
                                    map.Indoors.ToIntString(),
                                    ((int)map.Weather).ToString(),
                                    map.MaxX.ToString(),
                                    map.MaxY.ToString(),
                                    map.Darkness.ToString(),
                                    map.HungerEnabled.ToIntString(),
                                    map.RecruitEnabled.ToIntString(),
                                    map.ExpEnabled.ToIntString(),
                                    map.TimeLimit.ToString(),
                                    map.MinNpcs.ToString(),
                                    map.MaxNpcs.ToString(),
                                    map.NpcSpawnTime.ToString(),
                                    map.Cacheable.ToIntString());

            if (map.MapType == Enums.MapType.Instanced) {
                string mapBase = MapManager.GenerateMapID(((InstancedMap)map).MapBase);
                packet.AppendParameter(mapBase);
            } else {
                packet.AppendParameter("");
            }

            for (y = 0; y <= map.MaxY; y++) {
                for (x = 0; x <= map.MaxX; x++) {
                    packet.AppendParameters(map.Tile[x, y].Ground.ToString(),
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
                                            map.Tile[x, y].Light.ToString(),
                                            map.Tile[x, y].GroundSet.ToString(),
                                            map.Tile[x, y].GroundAnimSet.ToString(),
                                            map.Tile[x, y].MaskSet.ToString(),
                                            map.Tile[x, y].AnimSet.ToString(),
                                            map.Tile[x, y].Mask2Set.ToString(),
                                            map.Tile[x, y].M2AnimSet.ToString(),
                                            map.Tile[x, y].FringeSet.ToString(),
                                            map.Tile[x, y].FAnimSet.ToString(),
                                            map.Tile[x, y].Fringe2Set.ToString(),
                                            map.Tile[x, y].F2AnimSet.ToString());
                }
            }

            packet.AppendParameter(map.Npc.Count);

            for (x = 0; x < map.Npc.Count; x++) {
                packet.AppendParameters(map.Npc[x].NpcNum,
                                        map.Npc[x].SpawnX,
                                        map.Npc[x].SpawnY,
                                        map.Npc[x].MinLevel,
                                        map.Npc[x].MaxLevel,
                                        map.Npc[x].AppearanceRate,
                                        (int)map.Npc[x].StartStatus,
                                        map.Npc[x].StartStatusCounter,
                                        map.Npc[x].StartStatusChance);
            }

            packet.AppendParameter(map.TempChange.ToIntString());

            packet.FinalizePacket();

            return packet;
        }

        public static TcpPacket CreateWeatherPacket(IMap map) {
            return TcpPacket.CreatePacket("weather", ((int)map.Weather).ToString());
        }

        public static TcpPacket CreateMapItemsPacket(IMap map) {
            TcpPacket packet = new TcpPacket("mapitemdata");
            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {//server will lie to the client and send all hidden items as blank
                if (!map.ActiveItem[i].Hidden) {
                    packet.AppendParameters(map.ActiveItem[i].Num.ToString(), map.ActiveItem[i].Value.ToString(), map.ActiveItem[i].Sticky.ToIntString(), map.ActiveItem[i].X.ToString(), map.ActiveItem[i].Y.ToString());
                } else {
                    packet.AppendParameters("0", "0", "0", "0", "0");
                }
            }
            packet.FinalizePacket();
            return packet;
        }

        public static TcpPacket CreateMapNpcsPacket(IMap map) {
            TcpPacket packet = new TcpPacket("mapnpcdata");
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

            return packet;
        }

        public static TcpPacket CreateMapDone(PacketHitList hitlist, Client client) {
            return TcpPacket.CreatePacket("mapdone");
        }

        public static void AppendJoinMapPackets(Client client, PacketHitList packetList) {
            try {
                IMap map = client.Player.Map;
                // Send all players on current map to client
                foreach (Client i in map.GetClients()) {
                    Player player2 = i.Player;
                    if (player2 != null) {
                        if (i.IsPlaying() && i != client && player2.MapID == client.Player.MapID) {

                            TcpPacket packet = TcpPacket.CreatePacket("playerdata",
                                                                      i.ConnectionID.ToString(), i.Player.Name, i.Player.GetActiveRecruit().Sprite.ToString(),
                                                                      i.Player.MapID, i.Player.X.ToString(), i.Player.Y.ToString(),
                                                                      ((int)i.Player.Direction).ToString(), ((int)i.Player.Access).ToString(),
                                                                      i.Player.PK.ToIntString(), i.Player.GuildName, ((int)i.Player.GuildAccess).ToString(),
                                                                      i.Player.Status);
                            if (i.Player.ConfusionStepCounter > 0) {
                                packet.AppendParameter("1");
                            } else {
                                packet.AppendParameter("0");
                            }
                            packet.AppendParameters((int)i.Player.GetActiveRecruit().StatusAilment, i.Player.GetActiveRecruit().VolatileStatus.Count);
                            for (int j = 0; j < i.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                                packet.AppendParameter(i.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
                            }

                            packetList.AddPacket(client, packet);
                        }
                    }
                }

                //SendActiveTeamToMap(client, player.mMap);
                // Send client's player data to everyone on the map including himself
                AppendPlayerData(client, packetList);
            } catch (Exception ex) {
                if (client.Player.Name == "Pikachu") {
                    Messenger.PlayerMsg(client, ex.ToString(), Text.BrightRed);
                }
            }
        } 
        */

        public static void AppendMapData(Client client, PacketHitList hitlist, IMap map) {
            AppendMapData(client, hitlist, map, Enums.MapID.Active);
        }

        public static void AppendMapData(Client client, PacketHitList hitlist, IMap map, Enums.MapID mapType) {
            TcpPacket packet = new TcpPacket("mapdata");
            int x, y;
            if (map.MaxX == 0)
                map.MaxX = 19;
            if (map.MaxY == 0)
                map.MaxY = 14;

            packet.AppendParameter((int)mapType);

            packet.AppendParameters(map.MapID,
                                    map.Name.Trim(),
                                    map.Revision.ToString(),
                                    ((int)map.Moral).ToString(),
                                    map.Up.ToString(),
                                    map.Down.ToString(),
                                    map.Left.ToString(),
                                    map.Right.ToString(),
                                    map.Music,
                                    map.Indoors.ToIntString(),
                                    ((int)map.Weather).ToString(),
                                    map.MaxX.ToString(),
                                    map.MaxY.ToString(),
                                    map.Darkness.ToString(),
                                    map.HungerEnabled.ToIntString(),
                                    map.RecruitEnabled.ToIntString(),
                                    map.ExpEnabled.ToIntString(),
                                    map.TimeLimit.ToString(),
                                    map.MinNpcs.ToString(),
                                    map.MaxNpcs.ToString(),
                                    map.NpcSpawnTime.ToString(),
                                    map.Cacheable.ToIntString());

            if (map.MapType == Enums.MapType.Instanced) {
                string mapBase = MapManager.GenerateMapID(((InstancedMap)map).MapBase);
                packet.AppendParameter(mapBase);
            } else {
                packet.AppendParameter("");
            }

            for (y = 0; y <= map.MaxY; y++) {
                for (x = 0; x <= map.MaxX; x++) {
                    packet.AppendParameters(map.Tile[x, y].Ground.ToString(),
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
                                            map.Tile[x, y].F2AnimSet.ToString());
                }
            }

            packet.AppendParameter(map.Npc.Count);

            for (x = 0; x < map.Npc.Count; x++) {
                packet.AppendParameters(map.Npc[x].NpcNum,
                                        map.Npc[x].SpawnX,
                                        map.Npc[x].SpawnY,
                                        map.Npc[x].MinLevel,
                                        map.Npc[x].MaxLevel,
                                        map.Npc[x].AppearanceRate,
                                        (int)map.Npc[x].StartStatus,
                                        map.Npc[x].StartStatusCounter,
                                        map.Npc[x].StartStatusChance);
            }

            packet.AppendParameter(map.TempChange.ToIntString());

            packet.FinalizePacket();

            hitlist.AddPacket(client, packet);
        }

        public static void AppendWeather(Client client, PacketHitList hitlist, IMap map) {

            hitlist.AddPacket(client, TcpPacket.CreatePacket("weather", ((int)map.Weather).ToString()));
        }

        public static void AppendMapDarkness(Client client, PacketHitList hitlist, IMap map) {

            hitlist.AddPacket(client, TcpPacket.CreatePacket("darkness", (map.Darkness).ToString()));
        }

        public static void AppendMapItems(Client client, PacketHitList hitlist, IMap map, bool temp) {
            TcpPacket packet = new TcpPacket("mapitemdata");
            packet.AppendParameter(map.MapID);
            packet.AppendParameter(temp.ToIntString());
            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {//server will lie to the client and send all hidden items as blank
                if (!map.ActiveItem[i].Hidden) {
                    packet.AppendParameters(map.ActiveItem[i].Num.ToString(), map.ActiveItem[i].Value.ToString(), map.ActiveItem[i].Sticky.ToIntString(), map.ActiveItem[i].X.ToString(), map.ActiveItem[i].Y.ToString());
                } else {
                    packet.AppendParameters("0", "0", "0", "0", "0");
                }
            }
            packet.FinalizePacket();

            hitlist.AddPacket(client, packet);
        }

        public static void AppendMapNpcs(Client client, PacketHitList hitlist, IMap map, bool temp) {
            TcpPacket packet = new TcpPacket("mapnpcdata");
            packet.AppendParameter(map.MapID);
            packet.AppendParameter(temp.ToIntString());
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if ((int)map.ActiveNpc[i].Direction > 3) map.ActiveNpc[i].Direction = Enums.Direction.Right;
                packet.AppendParameters(map.ActiveNpc[i].Num.ToString(), map.ActiveNpc[i].Sprite.ToString(), map.ActiveNpc[i].Form.ToString(), ((int)map.ActiveNpc[i].Shiny).ToString(), ((int)map.ActiveNpc[i].Sex).ToString(),
                    map.ActiveNpc[i].X.ToString(), map.ActiveNpc[i].Y.ToString(), ((int)map.ActiveNpc[i].Direction).ToString(), ((int)map.ActiveNpc[i].StatusAilment).ToString());
                if (map.ActiveNpc[i].Num > 0 && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Friendly && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Shopkeeper && NpcManager.Npcs[map.ActiveNpc[i].Num].Behavior != Enums.NpcBehavior.Scripted) {
                    packet.AppendParameters("1");
                } else {
                    packet.AppendParameters("0");
                }
            }
            packet.FinalizePacket();

            hitlist.AddPacket(client, packet);
        }

        public static void AppendMapDone(Client client, PacketHitList hitlist) {
            //client.Player.GettingMap = false;
            hitlist.AddPacket(client, TcpPacket.CreatePacket("mapdone"));
        }


        #endregion


        #region Player Data

        /*
        public static void AppendJoinMapPackets(Client client, PacketHitList packetList) {
            IMap map = client.Player.Map;
            // Send all players on current map to client
            foreach (Client i in map.GetClients()) {
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
                        packetList.AddPacket(client, packet);
                    }
                }
            }

            //SendActiveTeamToMap(client, player.mMap);
            // Send client's player data to everyone on the map including himself
            AppendPlayerDataPackets(client, packetList);
        }
        
        public static TcpPacket CreateActiveStatusAilment(Client client) {
            return TcpPacket.CreatePacket("statusailment", ((int)client.Player.GetActiveRecruit().StatusAilment).ToString());

        }

        public static TcpPacket CreatePlayerStatusAilment(Client client) {

            return TcpPacket.CreatePacket("playerstatused", client.ConnectionID.ToString(), ((int)client.Player.GetActiveRecruit().StatusAilment).ToString());

        }

        public static void AppendActiveTeam(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("activeteam");
            Player player = client.Player;
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameters(player.Team[i].Name, Pokedex.Pokedex.GetPokemon(player.Team[i].Species).Mugshot.ToString(),
                                            player.Team[i].HP.ToString(), player.Team[i].MaxHP.ToString(),
                                            Server.Math.CalculatePercent(player.Team[i].Exp, Exp.ExpManager.Exp[player.Team[i].Level - 1]).ToString(),
                                            player.Team[i].Level.ToString(), ((int)player.Team[i].StatusAilment).ToString(), player.Team[i].HeldItemSlot.ToString());
                } else {
                    packet.AppendParameter("notloaded");
                }
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }
        
        public static TcpPacket CreateActiveTeamStatus(Client client) {
            TcpPacket packet = new TcpPacket("teamstatus");
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameter(((int)client.Player.Team[i].StatusAilment).ToString());
                } else {
                    packet.AppendParameter("0");
                }
            }
            packet.FinalizePacket();
            return packet;
        }


        public static TcpPacket CreateActiveVolatileStatus(Client client) {
            TcpPacket packet = TcpPacket.CreatePacket("volatilestatus", client.Player.GetActiveRecruit().VolatileStatus.Count.ToString());

            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                packet.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }

            return packet;

        }


        public static TcpPacket CreatePlayerVolatileStatus(Client client) {
            TcpPacket packet = TcpPacket.CreatePacket("playervolatilestatus", client.ConnectionID.ToString(), client.Player.GetActiveRecruit().VolatileStatus.Count.ToString());

            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                packet.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }

            return packet;

        }

        public static TcpPacket CreateActiveConfusion(Client client) {
            bool status;
            if (client.Player.ConfusionStepCounter > 0) {
                status = true;
            } else {
                status = false;
            }
            return TcpPacket.CreatePacket("confusion", status.ToIntString());


        }


        public static TcpPacket CreatePlayerConfusion(Client client) {
            bool status;
            if (client.Player.ConfusionStepCounter > 0) {
                status = true;
            } else {
                status = false;
            }
            return TcpPacket.CreatePacket("playerconfused", client.ConnectionID.ToString(), status.ToIntString());

        }

        */

        public static void AppendServerStatus(Client client, PacketHitList packetList) {
            TcpPacket packet = CreateServerStatus();
            packetList.AddPacket(client, packet);
        }

        public static void AppendPlayerData(Client client, PacketHitList packetList) {
            AppendPlayerData(client, true, packetList);
        }

        public static void AppendPlayerData(Client client, bool sendMyPlayerData, PacketHitList packetList) {
            
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
            packetList.AddPacketToSurroundingPlayersBut(client, client.Player.Map, packet);
            //packetList.AddPacketToMapBut(client, client.Player.MapID, packet);

            Recruit recruit = client.Player.GetActiveRecruit();
            int mobility = 0;
            for (int i = 15; i >= 0; i--) {
                if (recruit.Mobility[i]) {
                    mobility += (int)System.Math.Pow(2, i);
                }
            }
            if (sendMyPlayerData) {
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
                packetList.AddPacket(client, myPacket);
            }
        }

        public static void AppendInventory(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("playerinv");
            packet.AppendParameter(client.Player.MaxInv);
            for (int i = 1; i <= client.Player.MaxInv; i++) {
                packet.AppendParameters(client.Player.Inventory[i].Num.ToString(),
                    client.Player.Inventory[i].Amount.ToString(),
                    client.Player.Inventory[i].Sticky.ToIntString());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendWornEquipment(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("playerhelditem");
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameter(client.Player.Team[i].HeldItemSlot);
                } else {
                    packet.AppendParameter("0");
                }
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendMobility(Client client, PacketHitList hitlist) {
            int mobility = 0;
            for (int i = 15; i >= 0; i--) {
                if (client.Player.GetActiveRecruit().Mobility[i]) {
                    mobility += (int)System.Math.Pow(2, i);
                }
            }
            hitlist.AddPacket(client, TcpPacket.CreatePacket("mobility", mobility.ToString()));
        }

        public static void AppendTimeMultiplier(Client client, PacketHitList hitlist) {

            hitlist.AddPacket(client, TcpPacket.CreatePacket("timemultiplier", client.Player.GetActiveRecruit().TimeMultiplier));
        }

        public static void AppendDarkness(Client client, PacketHitList hitlist) {

            hitlist.AddPacket(client, TcpPacket.CreatePacket("selfdarkness", client.Player.GetActiveRecruit().Darkness));
        }

        public static void AppendVisibility(Client client, PacketHitList hitlist, bool visible) {

            hitlist.AddPacket(client, TcpPacket.CreatePacket("visibility", visible.ToIntString()));
        }

        public static void AppendSpeedLimit(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("speedlimit", ((int)client.Player.SpeedLimit).ToString()));

        }

        public static void AppendSprite(Client client, PacketHitList hitlist) {
            Recruit player = client.Player.GetActiveRecruit();
            hitlist.AddPacket(client, TcpPacket.CreatePacket("sprite", (player.Sprite).ToString(), (player.Form).ToString(), ((int)player.Shiny).ToString(), ((int)player.Sex).ToString()));

            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, TcpPacket.CreatePacket("playersprite", client.ConnectionID.ToString(),
                player.Sprite.ToString(), player.Form.ToString(), ((int)player.Shiny).ToString(), ((int)player.Sex).ToString()), Enums.OutdateType.Condition);
        }

        public static void AppendDead(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("dead", client.Player.Dead.ToIntString()));

            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, TcpPacket.CreatePacket("playerdead", client.ConnectionID.ToString(), client.Player.Dead.ToIntString()), Enums.OutdateType.Condition);
        }

        public static void AppendHunted(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("hunted", client.Player.Hunted.ToIntString()));

            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, TcpPacket.CreatePacket("playerhunted", client.ConnectionID.ToString(), client.Player.Hunted.ToIntString()), Enums.OutdateType.Condition);
        }

        public static void AppendActiveTeam(Client client, PacketHitList hitlist) {
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
            hitlist.AddPacket(client, packet);
        }


        public static void AppendActiveTeamNum(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("activeteamnum", client.Player.ActiveSlot.ToString()));
        }


        public static void AppendBelly(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("recruitbelly", client.Player.GetActiveRecruit().Belly.ToString(),
                client.Player.GetActiveRecruit().MaxBelly.ToString()));
        }

        public static TcpPacket CreatePartyMemberDataPacket(Party playerParty, Client member, int slot) {
            return TcpPacket.CreatePacket("partymemberdata", slot.ToString(), member.Player.Name, member.Player.GetActiveRecruit().Species.ToString(), member.Player.GetActiveRecruit().Form.ToString(), ((int)member.Player.GetActiveRecruit().Shiny).ToString(), ((int)member.Player.GetActiveRecruit().Sex).ToString(),
                                                  member.Player.GetActiveRecruit().Exp.ToString(), member.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                  member.Player.GetActiveRecruit().HP.ToString(), member.Player.GetActiveRecruit().MaxHP.ToString());

        }

        public static void AppendPartyMemberData(Client member, PacketHitList hitlist, int slot) {
            if (!string.IsNullOrEmpty(member.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(member);
                hitlist.AddPacketToParty(playerParty, TcpPacket.CreatePacket("partymemberdata", slot.ToString(), member.Player.Name, member.Player.GetActiveRecruit().Species.ToString(), member.Player.GetActiveRecruit().Form.ToString(), ((int)member.Player.GetActiveRecruit().Shiny).ToString(), ((int)member.Player.GetActiveRecruit().Sex).ToString(),
                                                      member.Player.GetActiveRecruit().Exp.ToString(), member.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      member.Player.GetActiveRecruit().HP.ToString(), member.Player.GetActiveRecruit().MaxHP.ToString()));
            }
        }

        public static void AppendPartyMemberDataFor(Client member, PacketHitList hitlist, int slot) {
            if (!string.IsNullOrEmpty(member.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(member);
                hitlist.AddPacket(member, TcpPacket.CreatePacket("partymemberdata", slot.ToString(), member.Player.Name, member.Player.GetActiveRecruit().Species.ToString(), member.Player.GetActiveRecruit().Form.ToString(), ((int)member.Player.GetActiveRecruit().Shiny).ToString(), ((int)member.Player.GetActiveRecruit().Sex).ToString(),
                                                      member.Player.GetActiveRecruit().Exp.ToString(), member.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      member.Player.GetActiveRecruit().HP.ToString(), member.Player.GetActiveRecruit().MaxHP.ToString()));
            }
        }

        public static void AppendMovePPUpdate(Client client, PacketHitList hitlist, int moveSlot) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("moveppupdate", moveSlot.ToString(),
                                                      client.Player.GetActiveRecruit().Moves[moveSlot].CurrentPP.ToString(),
                                                      client.Player.GetActiveRecruit().Moves[moveSlot].MaxPP.ToString()));
        }

        public static void AppendEXP(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("playerexp", client.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      client.Player.GetActiveRecruit().Exp.ToString()));
            if (!string.IsNullOrEmpty(client.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(client);
                AppendPartyMemberData(client, hitlist, playerParty.GetMemberSlot(client.Player.CharID));
                //CreateDataForParty(playerParty, client, playerParty.Members.GetMemberSlot(client.Player.CharID));
            }
        }

        public static void AppendHP(Client client, PacketHitList hitlist) {
            AppendHP(client, client.Player.ActiveSlot, hitlist);
        }

        public static void AppendHP(Client client, int recruitIndex, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("playerhp", recruitIndex.ToString(), client.Player.Team[recruitIndex].MaxHP.ToString(),
                                                      client.Player.Team[recruitIndex].HP.ToString()));
            if (!string.IsNullOrEmpty(client.Player.PartyID)) {
                Party playerParty = PartyManager.FindPlayerParty(client);
                AppendPartyMemberData(client, hitlist, playerParty.GetMemberSlot(client.Player.CharID));
            }
        }

        public static void AppendStats(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("playerstatspacket", client.Player.GetActiveRecruit().Atk.ToString(),
                                                      client.Player.GetActiveRecruit().Def.ToString(), client.Player.GetActiveRecruit().Spd.ToString(),
                                                      client.Player.GetActiveRecruit().SpclAtk.ToString(), client.Player.GetActiveRecruit().SpclDef.ToString(), client.Player.GetActiveRecruit().GetNextLevel().ToString(),
                                                      client.Player.GetActiveRecruit().Exp.ToString(), client.Player.GetActiveRecruit().Level.ToString()));
        }

        public static void AppendPlayerMoves(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("moves");
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                RecruitMove move = client.Player.GetActiveRecruit().Moves[i];
                packet.AppendParameters(move.MoveNum.ToString(), move.CurrentPP.ToString(), move.MaxPP.ToString(), move.Sealed.ToIntString());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }


        public static void AppendAvailableExpKitModules(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("kitmodules");
            packet.AppendParameter(client.Player.AvailableExpKitModules.Count);
            for (int i = 0; i < client.Player.AvailableExpKitModules.Count; i++) {
                packet.AppendParameter((int)client.Player.AvailableExpKitModules[i].Type);
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public static void AppendJobList(Client client, PacketHitList hitlist) {
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
            hitlist.AddPacket(client, packet);
        }

        public static void AppendPlayerMissionExp(Client client, PacketHitList hitlist) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("missionexp", client.Player.MissionExp.ToString(), ((int)client.Player.ExplorerRank).ToString()));
        }

        public static void AppendJoinMap(Client client, PacketHitList hitlist) {
            client.Player.RecreateSeenCharacters();
            // Send all players on current map to client
            foreach (Client i in ClientManager.GetClients()) {
                Player player2 = i.Player;
                if (player2 != null) {
                    if (i.IsPlaying() && i != client && player2.MapID == client.Player.MapID) {
                        i.Player.SeeNewCharacter(client.Player.GetActiveRecruit());
                        TcpPacket packet = TcpPacket.CreatePacket("playerdata",
                                                                  i.ConnectionID.ToString(), player2.Name, player2.GetActiveRecruit().Sprite.ToString(),
                                                                  player2.GetActiveRecruit().Form.ToString(), ((int)player2.GetActiveRecruit().Shiny).ToString(), ((int)player2.GetActiveRecruit().Sex).ToString(),
                                                                  player2.MapID, player2.X.ToString(), player2.Y.ToString(),
                                                                  ((int)player2.Direction).ToString(), ((int)player2.Access).ToString(), player2.Hunted.ToIntString(),
                                                                  player2.Dead.ToIntString(), player2.GuildName, ((int)player2.GuildAccess).ToString(),
                                                                  player2.Status, "0");
                        
                        packet.AppendParameters((int)player2.GetActiveRecruit().StatusAilment, player2.GetActiveRecruit().VolatileStatus.Count);
                        for (int j = 0; j < player2.GetActiveRecruit().VolatileStatus.Count; j++) {
                            packet.AppendParameter(player2.GetActiveRecruit().VolatileStatus[j].Emoticon);
                        }
                        hitlist.AddPacket(client, packet);
                    }
                }
            }


        }


        public static void AppendStatusAilment(Client client, PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("teamstatus");
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded == true) {
                    packet.AppendParameter(((int)client.Player.Team[i].StatusAilment).ToString());
                } else {
                    packet.AppendParameter("0");
                }
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);

            hitlist.AddPacket(client, TcpPacket.CreatePacket("statusailment", ((int)client.Player.GetActiveRecruit().StatusAilment).ToString()));


            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerStatusAilment(client), Enums.OutdateType.Condition);
        }

        public static void AppendVolatileStatus(Client client, PacketHitList hitlist) {
            TcpPacket myPacket = TcpPacket.CreatePacket("volatilestatus", client.Player.GetActiveRecruit().VolatileStatus.Count.ToString());

            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                myPacket.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }

            hitlist.AddPacket(client, myPacket);

            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerVolatileStatus(client), Enums.OutdateType.Condition);
        }



        public static void AppendEmote(Client client, int emote, int speed, int rounds, PacketHitList hitlist) {
            TcpPacket myPacket = TcpPacket.CreatePacket("emote", emote.ToString(), speed.ToString(), rounds.ToString());

            hitlist.AddPacket(client, myPacket);

            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerEmote(client, emote, speed, rounds), Enums.OutdateType.Condition);
        }

        public static void AppendConfusion(Client client, PacketHitList hitlist) {
            
            hitlist.AddPacket(client, TcpPacket.CreatePacket("confusion", client.Player.GetActiveRecruit().Confused.ToIntString()));

            //hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerConfusion(client), Enums.OutdateType.Condition);
        }

        public static void AppendPlayerMove(Client client, PacketHitList hitlist, Enums.Direction direction, Enums.Speed speed) {
            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerMovePacket(client, direction, speed), Enums.OutdateType.Location);
        }


        public static void AppendPlayerDir(Client client, PacketHitList hitlist) {
            hitlist.AddPacketToMap(client.Player.Map, CreatePlayerDir(client));
        }

        public static void AppendPlayerDirExcept(Client client, PacketHitList hitlist) {
            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerDir(client), Enums.OutdateType.Location);
            //hitlist.AddPacketToMapBut(client, client.Player.Map, CreatePlayerDir(client));
        }

        public static void AppendPlayerDir(Client client, PacketList packetList) {
            packetList.AddPacket(CreatePlayerDir(client));
        }

        public static void AppendPlayerXY(Client client, PacketHitList hitlist) {
            hitlist.AddPacketToOthers(client.Player.GetActiveRecruit(), client.Player.Map, CreatePlayerXY(client), Enums.OutdateType.Location);
            //hitlist.AddPacketToMapBut(client, client.Player.Map, CreatePlayerXY(client));
            AppendOwnXY(client, hitlist);
        }

        public static void AppendPlayerLock(Client client, PacketHitList hitlist, bool locked) {
            hitlist.AddPacket(client, TcpPacket.CreatePacket("movementlock", locked.ToIntString()));
        }

        public static void AppendOwnXY(Client client, PacketHitList hitlist) {

            hitlist.AddPacket(client, CreatePlayerXY(client));
            client.Player.RefreshSeenCharacters(hitlist);

        }

        public static void AppendAttackPacket(Client attacker, PacketHitList hitlist) {
            hitlist.AddPacketToOthers(attacker.Player.GetActiveRecruit(), attacker.Player.Map, CreateAttackPacket(attacker), Enums.OutdateType.None);
            //hitlist.AddPacketToMapBut(attacker, attacker.Player.Map, CreateAttackPacket(attacker));
        }


        #region CreatePackets
        //for need-to-know sending

        public static TcpPacket CreateServerStatus() {
            TcpPacket packet = TcpPacket.CreatePacket("serverstatus", Globals.ServerStatus ?? "");
            return packet;
        }

        public static TcpPacket CreateMyPlayerData(Client client) {
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

            myPacket.AppendParameter(recruit.VolatileStatus.Count);
            for (int j = 0; j < recruit.VolatileStatus.Count; j++) {
                myPacket.AppendParameter(recruit.VolatileStatus[j].Emoticon);
            }
            return myPacket;

        }

        public static TcpPacket CreatePlayerActivation(Client client, bool active) {
            // 
            return TcpPacket.CreatePacket("playeractive", client.ConnectionID.ToString(), active.ToIntString());
        }

        public static TcpPacket CreatePlayerStatusAilment(Client client) {

            return TcpPacket.CreatePacket("playerstatused", client.ConnectionID.ToString(), ((int)client.Player.GetActiveRecruit().StatusAilment).ToString());
        }

        public static TcpPacket CreatePlayerVolatileStatus(Client client) {
            TcpPacket packet = TcpPacket.CreatePacket("playervolatilestatus", client.ConnectionID.ToString(), client.Player.GetActiveRecruit().VolatileStatus.Count.ToString());

            for (int j = 0; j < client.Player.GetActiveRecruit().VolatileStatus.Count; j++) {
                packet.AppendParameter(client.Player.GetActiveRecruit().VolatileStatus[j].Emoticon);
            }

            return packet;

        }

        public static TcpPacket CreatePlayerEmote(Client client, int emote, int speed, int rounds) {
            return TcpPacket.CreatePacket("playeremote", client.ConnectionID.ToString(), emote.ToString(), speed.ToString(), rounds.ToString());
        }

        //public static TcpPacket CreatePlayerConfusion(Client client) {
        //    bool status;
        //    if (client.Player.Confusion > 0) {
        //        status = true;
        //    } else {
        //        status = false;
        //    }
        //    return TcpPacket.CreatePacket("playerconfused", client.ConnectionID.ToString(), status.ToIntString());
        //
        //}


        public static TcpPacket CreatePlayerMovePacket(Client client, Enums.Direction direction, Enums.Speed speed) {
            return TcpPacket.CreatePacket("playermove", client.ConnectionID.ToString(), client.Player.MapID, client.Player.X.ToString(), client.Player.Y.ToString(), ((int)direction).ToString(), ((int)speed).ToString());
        }

        public static TcpPacket CreatePlayerDir(Client client) {
            return TcpPacket.CreatePacket("playerdir", client.ConnectionID.ToString(), ((int)client.Player.Direction).ToString());
        }

        public static TcpPacket CreatePlayerXY(Client client) {
            return TcpPacket.CreatePacket("playerxy", client.ConnectionID.ToString(), client.Player.X.ToString(), client.Player.Y.ToString());
        }

        public static TcpPacket CreateAttackPacket(Client attacker) {
            // 
            return TcpPacket.CreatePacket("attack", attacker.ConnectionID.ToString());
        }

        #endregion

        #endregion

        #region NPC Data

        #region CreatePackets
        //for need-to-know sending

        public static TcpPacket CreateNpcActivation(MapNpc mapNpc, bool active) {
            return TcpPacket.CreatePacket("npcactive", mapNpc.MapID, mapNpc.MapSlot.ToString(), active.ToIntString());
        }

        public static TcpPacket CreateNpcHP(MapNpc mapNpc) {
            return TcpPacket.CreatePacket("npchp", mapNpc.MapID, mapNpc.MapSlot.ToString(), mapNpc.HP.ToString(), mapNpc.MaxHP.ToString());
        }

        public static TcpPacket CreateNpcVolatileStatus(MapNpc mapNpc) {
                TcpPacket packet = TcpPacket.CreatePacket("npcvolatilestatus", mapNpc.MapID, mapNpc.MapSlot.ToString(), mapNpc.VolatileStatus.Count.ToString());

                for (int j = 0; j < mapNpc.VolatileStatus.Count; j++) {
                    packet.AppendParameter(mapNpc.VolatileStatus[j].Emoticon);
                }

                return packet;
        }

        //public static TcpPacket CreateNpcConfusion(MapNpc mapNpc) {
        //    
        //    return TcpPacket.CreatePacket("npcconfuse", mapNpc.MapID, mapNpc.MapSlot.ToString(), mapNpc.Confused.ToIntString());
        //}

        public static TcpPacket CreateNpcStatusAilment(MapNpc npc) {
            return TcpPacket.CreatePacket("npcstatus", npc.MapID, npc.MapSlot.ToString(), ((int)npc.StatusAilment).ToString());

        }

        public static TcpPacket CreateNpcSpawn(MapNpc mapNpc) {
                TcpPacket packet = new TcpPacket("spawnnpc");
                packet.AppendParameters(mapNpc.MapID, mapNpc.MapSlot.ToString(), mapNpc.Num.ToString(), mapNpc.Sprite.ToString(),
                    mapNpc.Form.ToString(), ((int)mapNpc.Shiny).ToString(), ((int)mapNpc.Sex).ToString(),
                    mapNpc.X.ToString(), mapNpc.Y.ToString(), ((int)mapNpc.Direction).ToString(), ((int)mapNpc.StatusAilment).ToString());
                if (mapNpc.Num > 0) {
                    Npc npc = NpcManager.Npcs[mapNpc.Num];
                    if (npc.Behavior != Enums.NpcBehavior.Friendly && npc.Behavior != Enums.NpcBehavior.Shopkeeper && npc.Behavior != Enums.NpcBehavior.Scripted) {
                        packet.AppendParameters("1");
                    } else {
                        packet.AppendParameters("0");
                    }
                }
                packet.FinalizePacket();
                return packet;
        }

        public static TcpPacket CreateNpcSprite(MapNpc mapNpc) {
            return TcpPacket.CreatePacket("npcsprite", mapNpc.MapID, mapNpc.MapSlot.ToString(),
                mapNpc.Sprite.ToString(), mapNpc.Form.ToString(), ((int)mapNpc.Shiny).ToString(), ((int)mapNpc.Sex).ToString());
        }

        public static TcpPacket CreateNpcXY(MapNpc mapNpc) {
            return TcpPacket.CreatePacket("npcxy", mapNpc.MapID, mapNpc.MapSlot.ToString(), mapNpc.X.ToString(), mapNpc.Y.ToString());
        }

        public static TcpPacket CreateNpcDir(MapNpc mapNpc) {
            return TcpPacket.CreatePacket("npcdir", mapNpc.MapID, mapNpc.MapSlot.ToString(), ((int)mapNpc.Direction).ToString());
        }

        public static TcpPacket CreateNpcAttack(MapNpc mapNpc) {
            return TcpPacket.CreatePacket("npcattack", mapNpc.MapID, mapNpc.MapSlot.ToString());
        }

        #endregion

        public static void AppendNpcHP(IMap map, PacketHitList hitlist, int mapNpcNum) {
            MapNpc mapNpc = map.ActiveNpc[mapNpcNum];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcHP(mapNpc), Enums.OutdateType.Condition);
        }

        public static void AppendNpcVolatileStatus(IMap map, PacketHitList hitlist, int mapNpcNum) {
            MapNpc mapNpc = map.ActiveNpc[mapNpcNum];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcVolatileStatus(mapNpc), Enums.OutdateType.Condition);
        }

        //public static void AppendNpcConfusion(IMap map, PacketHitList hitlist, int mapNpcNum) {
        //    MapNpc mapNpc = map.ActiveNpc[mapNpcNum];
        //    hitlist.AddPacketToOthers(mapNpc, map, CreateNpcConfusion(mapNpc), Enums.OutdateType.Condition);
        //}

        public static void AppendNpcStatusAilment(MapNpc npc, PacketHitList hitlist) {
            hitlist.AddPacketToOthers(npc, Maps.MapManager.RetrieveActiveMap(npc.MapID), CreateNpcStatusAilment(npc), Enums.OutdateType.Condition);
        }


        public static void AppendNpcStatusAilment(IMap map, PacketHitList hitlist, int mapSlot) {
            AppendNpcStatusAilment(map.ActiveNpc[mapSlot], hitlist);
        }


        public static void AppendNpcSpawn(IMap map, PacketHitList hitlist, int mapNpcSlot) {
            MapNpc mapNpc = map.ActiveNpc[mapNpcSlot];
            foreach (Client i in map.GetSurroundingClients(map)) {
                i.Player.SeeNewCharacter(mapNpc);
            }
            hitlist.AddPacketToSurroundingPlayers(map, CreateNpcSpawn(mapNpc));
        }

        public static void AppendNpcXY(IMap map, PacketHitList hitlist, int mapSlot) {
            MapNpc mapNpc = map.ActiveNpc[mapSlot];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcXY(mapNpc), Enums.OutdateType.Location);
        }

        public static void AppendNpcSprite(IMap map, PacketHitList hitlist, int mapSlot) {
            MapNpc mapNpc = map.ActiveNpc[mapSlot];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcSprite(mapNpc), Enums.OutdateType.Condition);
        }

        public static void AppendNpcMove(IMap map, Combat.ICharacter target, PacketHitList hitlist, int data) {
            if (map != null) {
                TcpPacket packet = TcpPacket.CreatePacket("nm", data.ToString(), map.MapID);
                // Go through all of the clients on the current map
                foreach (Client i in map.GetClients()) {
                    AppendNpcMoveInternal(map, target, hitlist, packet, map, Enums.MapID.Active, i);
                }
                // Go through all of the clients on the surrounding maps
                for (int n = 1; n < 9; n++) {
                    string borderingMapID = MapManager.RetrieveBorderingMapID(map, (Enums.MapID)n);
                    if (!string.IsNullOrEmpty(borderingMapID)) {
                        IMap borderingMap = MapManager.RetrieveActiveMap(borderingMapID);
                        if (borderingMap != null && SeamlessWorldHelper.IsMapSeamless(map, borderingMap)) {
                            foreach (Client i in borderingMap.GetClients()) {
                                AppendNpcMoveInternal(map, target, hitlist, packet, borderingMap, (Enums.MapID)n, i);
                            }
                        }
                    }
                }
            }
        }


        private static void AppendNpcMoveInternal(IMap map, Combat.ICharacter target, PacketHitList hitlist, TcpPacket packet, IMap borderingMap, Enums.MapID borderingMapID, Client targetClient) {
            if (targetClient.Player.GetActiveRecruit() != target) {
                if (AI.MovementProcessor.WillCharacterSeeCharacter(targetClient.Player.Map, targetClient.Player.GetActiveRecruit(), map, MapManager.GetOppositeMapID(borderingMapID), target)) {
                    if (!targetClient.Player.IsSeenCharacterSeen(target)) {
                        targetClient.Player.OutdateCharacter(target, Enums.OutdateType.Location);
                        targetClient.Player.AddActivationIfCharacterNotSeen(hitlist, target);
                    } else {
                        hitlist.AddPacket(targetClient, packet);
                    }
                } else {
                    if (targetClient.Player.IsSeenCharacterSeen(target)) {
                        hitlist.AddPacket(targetClient, packet);
                        //unsee character
                    } else {
                        targetClient.Player.OutdateCharacter(target, Enums.OutdateType.Location);
                    }
                }
            }
        }

        public static void AppendNpcDir(IMap map, PacketHitList hitlist, int mapSlot) {
            MapNpc mapNpc = map.ActiveNpc[mapSlot];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcDir(mapNpc), Enums.OutdateType.Location);
        }

        public static void AppendNpcAttack(IMap map, PacketHitList hitlist, int mapSlot) {
            MapNpc mapNpc = map.ActiveNpc[mapSlot];
            hitlist.AddPacketToOthers(mapNpc, map, CreateNpcAttack(mapNpc), Enums.OutdateType.None);
        }

        #endregion

        #region Often used for combat

        public static TcpPacket CreateBattleMsg(string msg, Color color) {
            return TcpPacket.CreatePacket("battlemsg", msg, color.ToArgb().ToString());
        }

        public static TcpPacket CreateChatMsg(string Msg, Color Color) {
            return TcpPacket.CreatePacket("msg", Msg, Color.ToArgb().ToString());
        }

        public static TcpPacket CreateSpeechBubble(string msg, Client client) {
            return TcpPacket.CreatePacket("speechbubble", msg, client.ConnectionID.ToString());
        }


        public static TcpPacket CreateBattleDivider() {
            return TcpPacket.CreatePacket("battledivider");
        }


        public static void AppendAdminMsg(PacketHitList hitlist, string Msg, Color Color) {
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying() && Ranks.IsAllowed(i, Enums.Rank.Moniter)) {
                    hitlist.AddPacket(i, TcpPacket.CreatePacket("msg", Msg, Color.ToArgb().ToString()));
                }
            }
        }


        public static TcpPacket CreateSoundPacket(string sound) {
            if (!string.IsNullOrEmpty(sound)) {
                return TcpPacket.CreatePacket("sound", sound);
            } else {
                return null;
            }

        }

        public static TcpPacket CreateSpellAnim(int SpellNum, int X, int Y) {
            return CreateSpellAnim(MoveManager.Moves[SpellNum].DefenderAnim, X, Y, X, Y, 0);
        }

        public static TcpPacket CreateSpellAnim(MoveAnimation animation, int X1, int Y1, int arg1, int arg2, int arg3) {
            if (animation.AnimationIndex > -1 && animation.FrameSpeed > 0 && animation.Repetitions > 0) {
                return TcpPacket.CreatePacket("scriptspellanim", ((int)animation.AnimationType).ToString(), animation.AnimationIndex.ToString(),
                    animation.FrameSpeed.ToString(), animation.Repetitions.ToString(), X1.ToString(), Y1.ToString(), arg1.ToString(), arg2.ToString(), arg3.ToString());
            } else {
                return null;
            }
        }


        public static TcpPacket CreateTilePacket(int x, int y, IMap map) {
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

            return packet;
        }

        public static TcpPacket CreateItemSpawnPacket(int mapItemSlot, int itemNum, int itemVal, bool sticky, int x, int y) {
            return TcpPacket.CreatePacket("spawnitem", mapItemSlot.ToString(), itemNum.ToString(), itemVal.ToString(), sticky.ToIntString(), x.ToString(), y.ToString());
        }

        public static TcpPacket CreateGameTimePacket() {
            return TcpPacket.CreatePacket("gametime", ((int)Globals.ServerTime).ToString());
        }

        #endregion

        #region Tournaments

        public static TcpPacket CreateTournamentListingPacket() {
            TcpPacket packet = new TcpPacket();
            int validTournyCount = 0;
            for (int i = 0; i < TournamentManager.Tournaments.Count; i++) {
                Tournament tourny = TournamentManager.Tournaments[i];
                if (tourny.TournamentStarted == false) {
                    validTournyCount++;
                    packet.AppendParameter(tourny.Name);
                    packet.AppendParameter(tourny.ID);
                    for (int n = 0; n < tourny.RegisteredMembers.Count; n++) {
                        if (tourny.RegisteredMembers[n].Admin) {
                            packet.AppendParameter(tourny.RegisteredMembers[n].Client.Player.Name);
                            break;
                        }
                    }
                }
            }
            packet.PrependParameters("tournamentlisting", validTournyCount.ToString());

            return packet;
        }

        public static TcpPacket CreateTournamentSpectateListingPacket() {
            TcpPacket packet = new TcpPacket();
            int validTournyCount = 0;
            for (int i = 0; i < TournamentManager.Tournaments.Count; i++) {
                Tournament tourny = TournamentManager.Tournaments[i];
                if (tourny.TournamentStarted == false) {
                    validTournyCount++;
                    packet.AppendParameter(tourny.Name);
                    packet.AppendParameter(tourny.ID);
                    for (int n = 0; n < tourny.RegisteredMembers.Count; n++) {
                        if (tourny.RegisteredMembers[n].Admin) {
                            packet.AppendParameter(tourny.RegisteredMembers[n].Client.Player.Name);
                            break;
                        }
                    }
                }
            }
            packet.PrependParameters("tournamentspectatelisting", validTournyCount.ToString());

            return packet;
        }

        public static TcpPacket CreateTournamentRulesEditorPacket(TournamentRules rules) {
            TcpPacket packet = new TcpPacket("tournamentruleseditor");

            packet.AppendParameters(
                rules.SleepClause.ToIntString(),
                rules.AccuracyClause.ToIntString(),
                rules.SpeciesClause.ToIntString(),
                rules.FreezeClause.ToIntString(),
                rules.OHKOClause.ToIntString(),
                rules.SelfKOClause.ToIntString());

            return packet;
        }

        public static TcpPacket CreateTournamentRulesPacket(Tournament tournament) {
            TcpPacket packet = new TcpPacket("tournamentrules");

            packet.AppendParameters(
                tournament.Rules.SleepClause.ToIntString(),
                 tournament.Rules.AccuracyClause.ToIntString(),
                 tournament.Rules.SpeciesClause.ToIntString(),
                 tournament.Rules.FreezeClause.ToIntString(),
                 tournament.Rules.OHKOClause.ToIntString(),
                 tournament.Rules.SelfKOClause.ToIntString());

            return packet;
        }

        public static TcpPacket CreatePlayerAllRecruitsPacket(Client client) {
            TcpPacket packet = new TcpPacket();
            int recruitCount = 0;
            int[] team = new int[4] { -1, -1, -1, -1 };
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                foreach (AssemblyRecruitData assemblyRecruit in PlayerDataManager.LoadPlayerAssemblyRecruits(dbConnection.Database, client.Player.CharID)) {
                    recruitCount++;
                    if (client.Player.IsInTeam(assemblyRecruit.RecruitIndex) == false) {
                        int size = 1;
                        //int mugshot = -1;
                        int species = assemblyRecruit.Species;
                        //int sprite = 0;
                        //if (species > -1 && species <= Constants.TOTAL_POKEMON) {
                        //    Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(species, assemblyRecruit.Form);
                        //    mugshot = pokemon.Mugshot[assemblyRecruit.Shiny, assemblyRecruit.Sex];
                        //    sprite = pokemon.Sprite[assemblyRecruit.Shiny, assemblyRecruit.Sex];
                        //}
                        packet.AppendParameters(assemblyRecruit.RecruitIndex, species, (int)assemblyRecruit.Sex, assemblyRecruit.Form, (int)assemblyRecruit.Shiny, assemblyRecruit.Level);
                        packet.AppendParameter(assemblyRecruit.Name);
                    } else {
                        int teamSlot = client.Player.FindTeamSlot(assemblyRecruit.RecruitIndex);
                        team[teamSlot] = assemblyRecruit.RecruitIndex;
                        //int mugshot = Pokedex.Pokedex.GetPokemonForm(client.Player.Team[teamSlot].Species, client.Player.Team[teamSlot].Form).Mugshot[assemblyRecruit.Shiny, assemblyRecruit.Sex];
                        if (teamSlot > -1 && teamSlot < 4) {
                            packet.AppendParameters(assemblyRecruit.RecruitIndex.ToString(), client.Player.Team[teamSlot].Sprite.ToString(), ((int)client.Player.Team[teamSlot].Sex).ToString(),
                                                        client.Player.Team[teamSlot].Form.ToString(), ((int)client.Player.Team[teamSlot].Shiny).ToString(), client.Player.Team[teamSlot].Level.ToString(), client.Player.Team[teamSlot].Name);
                        }
                    }
                }
            }
            packet.PrependParameters("allrecruits", recruitCount.ToString(), team[0].ToString(), team[1].ToString(), team[2].ToString(), team[3].ToString());
            return packet;
        }

        #endregion
    }
}
