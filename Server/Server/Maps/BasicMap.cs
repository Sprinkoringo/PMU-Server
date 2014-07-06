namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    using Server.Items;
    using Server.Npcs;
    using Server.Players;
    using Server.Network;
    using PMU.Sockets;
    using System.Threading;

    public class BasicMap : MapBase
    {
        DataManager.Maps.MapDump mapDump;

        public DataManager.Maps.MapDump BaseMap {
            get { return mapDump; }
        }

        #region Constructors

        public BasicMap(DataManager.Maps.MapDump mapDump)
            : base(mapDump) {
            this.mapDump = mapDump;

            this.PlayersOnMap = new MapPlayersCollection();
            this.TempStatus = new MapStatusCollection(mapDump);

            this.IsSaving = false;

            this.ActiveNpc = new ActiveNpcCollection(mapDump);
            this.ActiveItem = new ActiveItemCollection(mapDump);
            this.Darkness = OriginalDarkness;
        }

        //public BasicMap(string mapID)
        //    : base(mapID) {
        //    this.IsSaving = false;
            
        //    Darkness = -1;
        //    TimeLimit = -1;
        //}

        #endregion Constructors

        #region Properties

        public TickCount ActivationTime {
            get;
            set;
        }

        public bool ProcessingPaused {
            get;
            set;
        }

        public ActiveItemCollection ActiveItem {
            get;
            set;
        }

        public ActiveNpcCollection ActiveNpc {
            get;
            set;
        }

        public int SpawnMarker { get; set; }

        public TickCount NpcSpawnWait { get; set; }

        public bool IsSaving {
            get;
            set;
        }

        public MapPlayersCollection PlayersOnMap {
            get;
            set;
        }

        public MapStatusCollection TempStatus { get; set; }

        public bool TempChange { get; set; }

        public override Enums.Weather Weather {
            get {
                return base.Weather;
            }
            set {
                Enums.Weather oldWeather = base.Weather;
                base.Weather = value;
                if (MapManager.IsMapActive(MapID)) {
                    Scripting.ScriptManager.InvokeFunction("OnWeatherChange", MapManager.RetrieveActiveMap(MapID), oldWeather, value);
                }
            }
        }

        #endregion Properties

        #region Methods

        public void ClearActiveNpc(int npcSlot) {
            ActiveNpc[npcSlot] = new MapNpc(MapID, npcSlot);
        }

        public void ClearActiveItem(int itemSlot) {
            ActiveItem[itemSlot] = new MapItem(new DataManager.Maps.MapItem());
            ActiveItem[itemSlot].Num = -1;
        }

        public int FindOpenItemSlot() {
            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                if (ActiveItem[i].Num == -1) {
                    return i;
                }
            }
            return -1;
        }

        public void SpawnItem(int itemNum, int itemVal, bool sticky, bool hidden, string tag, int x, int y, Client playerFor) {
            // Check for subscript out of range
            if (itemNum < 0 || itemNum > ItemManager.Items.MaxItems) {
                return;
            }

            // Find open map item slot
            int openSlot = FindOpenItemSlot();

            this.SpawnItemSlot(openSlot, itemNum, itemVal, sticky, hidden, tag, x, y, playerFor);
        }


        public void SpawnItemSlot(int itemSlot, int itemNum, int itemVal, bool sticky, bool hidden, string tag, int x, int y, Client playerFor) {
            // Check for subscript out of range
            if (itemSlot < 0 || itemSlot > Constants.MAX_MAP_ITEMS || itemNum < -1 || itemNum > ItemManager.Items.MaxItems) {
                return;
            }

            if (itemSlot != -1) {
                ActiveItem[itemSlot].Num = itemNum;
                ActiveItem[itemSlot].Value = itemVal;
                ActiveItem[itemSlot].Sticky = sticky;
                ActiveItem[itemSlot].Hidden = hidden;
                ActiveItem[itemSlot].Tag = tag;
                if (playerFor == null) {
                    ActiveItem[itemSlot].TimeDropped = new TickCount(0);
                    ActiveItem[itemSlot].PlayerFor = "";
                } else {
                    ActiveItem[itemSlot].TimeDropped = Core.GetTickCount();
                    ActiveItem[itemSlot].PlayerFor = playerFor.Player.CharID;
                }

                ActiveItem[itemSlot].X = x;
                ActiveItem[itemSlot].Y = y;
                if (!hidden) {
                    Messenger.SendDataToMap(MapID, PacketBuilder.CreateItemSpawnPacket(itemSlot, itemNum, itemVal, sticky, x, y));
                }
            }
        }



        public void SpawnItems() {
            // Spawn what we have
            for (int Y = 0; Y <= MaxY; Y++) {
                for (int X = 0; X <= MaxX; X++) {
                    // Check if the tile type is an item or a saved tile in case someone drops something

                    if ((Tile[X, Y].Type == Enums.TileType.Item)) {
                        // Check to see if its a currency and if they set the value to 0 set it to 1 automatically
                        if ((ItemManager.Items[Tile[X, Y].Data1].Type == Enums.ItemType.Currency || ItemManager.Items[Tile[X, Y].Data1].StackCap > 0) && Tile[X, Y].Data2 <= 0) {
                            SpawnItem(Tile[X, Y].Data1, 1, Tile[X, Y].Data3.ToString().ToBool(), Tile[X, Y].String1.ToBool(), Tile[X, Y].String2, X, Y, null);
                        } else {
                            SpawnItem(Tile[X, Y].Data1, Tile[X, Y].Data2, Tile[X, Y].Data3.ToString().ToBool(), Tile[X, Y].String1.ToBool(), Tile[X, Y].String2, X, Y, null);
                        }
                    } else if (Tile[X, Y].Type == Enums.TileType.DropShop && Tile[X, Y].Data2 > 0) {
                        if ((ItemManager.Items[Tile[X, Y].Data2].Type == Enums.ItemType.Currency || ItemManager.Items[Tile[X, Y].Data2].StackCap > 0) && Tile[X, Y].Data2 <= 0) {
                            SpawnItem(Tile[X, Y].Data2, 1, false, false, Tile[X, Y].String2, X, Y, null);
                        } else {
                            SpawnItem(Tile[X, Y].Data2, Tile[X, Y].Data3, false, false, Tile[X, Y].String2, X, Y, null);
                        }
                    }
                }
            }
        }

        public int FindOpenNpcSlot() {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (ActiveNpc[i].Num == 0) {
                    return i;
                }
            }
            return -1;
        }

        public void SpawnNpc() {
            SpawnNpc(false);
        }

        public void SpawnNpc(bool checkSight) {
            if (Npc.Count <= 0) return;

            if (SpawnMarker >= Npc.Count) {
                SpawnMarker = Npc.Count - 1;
            }

            for (int i = 0; i < 100; i++) {
                if (Server.Math.Rand(0, 100) < Npc[SpawnMarker].AppearanceRate && WillSpawnNow(Npc[SpawnMarker])) {
                    break;
                }
                SpawnMarker++;
                if (SpawnMarker >= Npc.Count) {
                    SpawnMarker = 0;
                }
            }

            SpawnNpc(Npc[SpawnMarker], checkSight);

            SpawnMarker++;

            if (SpawnMarker >= Npc.Count) {
                SpawnMarker = 0;
            }
        }

        public bool WillSpawnNow(MapNpcPreset npc) {
            int NPCNum = npc.NpcNum;
            if (NPCNum <= 0) return false;

            switch (Globals.ServerTime) {
                case Enums.Time.Dawn: {
                        if (NpcManager.Npcs[NPCNum].SpawnsAtDawn) {
                            return true;
                        }
                    }
                    break;
                case Enums.Time.Day: {
                        if (NpcManager.Npcs[NPCNum].SpawnsAtDay) {
                            return true;
                        }
                    }
                    break;
                case Enums.Time.Dusk: {
                        if (NpcManager.Npcs[NPCNum].SpawnsAtDusk) {
                            return true;
                        }
                    }
                    break;
                case Enums.Time.Night: {
                        if (NpcManager.Npcs[NPCNum].SpawnsAtNight) {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public void SpawnNpc(MapNpcPreset npc) {
            SpawnNpc(npc, false);
        }

        public void SpawnNpc(MapNpcPreset npc, bool checkSight) {
            int NPCNum = 0;
            int X = 0;
            int Y = 0;
            bool Spawned = false;

            // Check for empty NPC slot
            int npcSlot = FindOpenNpcSlot();

            if (npcSlot < 0 || npcSlot >= Constants.MAX_MAP_NPCS) {
                return;
            }

            NPCNum = npc.NpcNum;
            if (NPCNum > 0) {

                ActiveNpc[npcSlot].Num = NPCNum;
                ActiveNpc[npcSlot].Target = null;

                ActiveNpc[npcSlot].Name = NpcManager.Npcs[NPCNum].Name;
                ActiveNpc[npcSlot].Form = NpcManager.Npcs[NPCNum].Form;

                //if (NpcManager.Npcs[NPCNum].ShinyChance != 0 && Server.Math.Rand(0, NpcManager.Npcs[NPCNum].ShinyChance) == 0) ActiveNpc[npcSlot].Shiny = true;
                ActiveNpc[npcSlot].Sex = Pokedex.Pokedex.GetPokemonForm(NpcManager.Npcs[NPCNum].Species, ActiveNpc[npcSlot].Form).GenerateLegalSex();

                ActiveNpc[npcSlot].AttackTimer = new TickCount(Core.GetTickCount().Tick);
                ActiveNpc[npcSlot].PauseTimer = new TickCount(Core.GetTickCount().Tick);

                //if (Npc[npcSlot].MinLevel == -1) {
                //Npc[npcSlot].MinLevel = NpcManager.Npcs[Npc[npcSlot].NpcNum].RecruitLevel;
                if (npc.MinLevel <= 0) {
                    npc.MinLevel = 1;
                    npc.MaxLevel = 1;
                }// else {
                //        Console.WriteLine("Npc found!");
                //    }
                //}
                ActiveNpc[npcSlot].Level = Server.Math.Rand(npc.MinLevel, npc.MaxLevel + 1);
                //set initial stats
                ActiveNpc[npcSlot].CalculateOriginalSprite();
                ActiveNpc[npcSlot].CalculateOriginalStats();
                ActiveNpc[npcSlot].CalculateOriginalType();
                ActiveNpc[npcSlot].CalculateOriginalAbility();
                //ActiveNpc[npcSlot].CalculateOriginalMobility();


                ActiveNpc[npcSlot].HP = ActiveNpc[npcSlot].MaxHP;

                ActiveNpc[npcSlot].Direction = (Enums.Direction)Server.Math.Rand(0, 4);

                ActiveNpc[npcSlot].GenerateMoveset();

                if (Moral == Enums.MapMoral.None) {
                    ActiveNpc[npcSlot].GenerateHeldItem();
                }

                if (Server.Math.Rand(0, 100) < npc.StartStatusChance) {
                    ActiveNpc[npcSlot].StatusAilment = npc.StartStatus;
                    ActiveNpc[npcSlot].StatusAilmentCounter = npc.StartStatusCounter;
                }

                if (npc.SpawnX < 0 | npc.SpawnY < 0) {
                    // We'll try 100 times to randomly place the sprite
                    for (int i = 1; i <= 50; i++) {
                        if (Tile[X, Y].Type == Enums.TileType.Walkable || Tile[X, Y].Type == Enums.TileType.Slow) {

                            if (checkSight) {
                                bool seen = false;
                                foreach (Client client in GetClients()) {
                                    if (CanCharacterSeeDestination(client.Player.GetActiveRecruit(), X, Y)) {
                                        seen = true;
                                        break;
                                    }
                                }
                                if (!seen) {
                                    ActiveNpc[npcSlot].X = X;
                                    ActiveNpc[npcSlot].Y = Y;
                                    Spawned = true;
                                    break;
                                }
                            } else {
                                ActiveNpc[npcSlot].X = X;
                                ActiveNpc[npcSlot].Y = Y;
                                Spawned = true;
                                break;
                            }
                        }
                    }

                    for (int i = 1; i <= 50; i++) {
                        X = Server.Math.Rand(0, MaxX + 1);
                        Y = Server.Math.Rand(0, MaxY + 1);

                        // Check if the tile is walkable
                        if (Tile[X, Y].Type == Enums.TileType.Walkable) {

                            ActiveNpc[npcSlot].X = X;
                            ActiveNpc[npcSlot].Y = Y;
                            Spawned = true;
                            break;
                        }
                    }
                } else {
                    // We no longer subtract one because Rand is ListIndex -1.
                    ActiveNpc[npcSlot].X = npc.SpawnX;
                    ActiveNpc[npcSlot].Y = npc.SpawnY;
                    Spawned = true;
                }

                // Didn't spawn, so now we'll just try to find a free tile
                if (!Spawned) {
                    for (Y = 0; Y <= MaxY; Y++) {
                        for (X = 0; X <= MaxX; X++) {
                            if (Tile[X, Y].Type == Enums.TileType.Walkable) {
                                ActiveNpc[npcSlot].X = X;
                                ActiveNpc[npcSlot].Y = Y;
                                Spawned = true;
                            }
                        }
                    }
                }

                // If we suceeded in spawning then send it to everyone
                if (Spawned) {
                    PacketHitList hitlist = null;
                    PacketHitList.MethodStart(ref hitlist);
                    PacketBuilder.AppendNpcSpawn(MapManager.RetrieveActiveMap(MapID), hitlist, npcSlot);

                    Scripting.ScriptManager.InvokeFunction("OnNpcSpawn", MapManager.RetrieveActiveMap(MapID), npc, ActiveNpc[npcSlot], hitlist);

                    PacketHitList.MethodEnded(ref hitlist);
                }


            }

        }





        public void SpawnNpcs() {
            int startNum = Server.Math.Rand(MinNpcs, MaxNpcs + 1);
            for (int i = 0; i < startNum; i++) {
                SpawnNpc();
            }
        }

        public void RemoveNpc(int mapNpcNum) {
            Npc[mapNpcNum] = new MapNpcPreset();
        }

        public bool IsNpcSlotEmpty(int mapNpcNum) {
            return (ActiveNpc[mapNpcNum].Num == 0);
        }

        //private void Update(MapBase o)
        //{
        //    Type type = o.GetType();
        //    while (type != null) {
        //        UpdateForType(type, o);
        //        type = type.BaseType;
        //    }
        //}

        //private void UpdateForType(Type type, MapBase source)
        //{
        //    FieldInfo[] myObjectFields = type.GetFields(
        //        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        //    foreach (FieldInfo fi in myObjectFields) {
        //        fi.SetValue(this, fi.GetValue(source));
        //    }
        //}

        public void RemakePlayersList() {
            PlayersOnMap.Clear();
            foreach (Client i in ClientManager.GetClients()) {
                if (i.IsPlaying() && i.Player.MapID == this.MapID) {
                    PlayersOnMap.Add(i.Player.CharID);
                }
            }
        }

        public IEnumerable<Client> GetClients() {
            foreach (MapPlayer playerOnMap in PlayersOnMap.GetPlayers()) {
                //Client client = ClientManager.FindClientFromCharID(playerOnMap);
                if (playerOnMap.Client != null && playerOnMap.Client.IsPlaying() && playerOnMap.Client.Player.MapID == this.MapID) {
                    yield return playerOnMap.Client;
                }
            }
        }

        public IEnumerable<Client> GetSurroundingClients(IMap map) {
            // Return all of the clients on the current map
            foreach (Client i in map.GetClients()) {
                yield return i;
            }
            // Return all of the clients on the surrounding maps
            for (int n = 1; n < 9; n++) {
                bool borderingMapActive = MapManager.IsBorderingMapLoaded(map, (Enums.MapID)n);
                if (borderingMapActive && SeamlessWorldHelper.IsMapSeamless(map, (Enums.MapID)n)) {
                    IMap borderingMap = MapManager.RetrieveBorderingMap(map, (Enums.MapID)n, true);
                    if (borderingMap != null) {
                        foreach (Client i in borderingMap.GetClients()) {
                            yield return i;
                        }
                    }
                }
            }
        }

        public void SetAttribute(int x, int y, Enums.TileType tileType, int data1, int data2, int data3,
            string string1, string string2, string string3) {
            Tile[x, y].Data1 = data1;
            Tile[x, y].Data2 = data2;
            Tile[x, y].Data3 = data3;
            Tile[x, y].Type = tileType;
            Tile[x, y].String1 = string1;
            Tile[x, y].String2 = string2;
            Tile[x, y].String3 = string3;
        }

        public void SetTile(int x, int y, int tileX, int tileY, int tileset, int layer) {
            SetTile(x, y, tileY * 14 + tileX, tileset, layer);
        }

        public void SetTile(int x, int y, int tileNum, int tileset, int layer) {
            switch (layer) {
                case 0:
                    Tile[x, y].Ground = tileNum;
                    Tile[x, y].GroundSet = tileset;
                    break;
                case 1:
                    Tile[x, y].Mask = tileNum;
                    Tile[x, y].MaskSet = tileset;
                    break;
                case 2:
                    Tile[x, y].Anim = tileNum;
                    Tile[x, y].AnimSet = tileset;
                    break;
                case 3:
                    Tile[x, y].Mask2 = tileNum;
                    Tile[x, y].Mask2Set = tileset;
                    break;
                case 4:
                    Tile[x, y].M2Anim = tileNum;
                    Tile[x, y].M2AnimSet = tileset;
                    break;
                case 5:
                    Tile[x, y].Fringe = tileNum;
                    Tile[x, y].FringeSet = tileset;
                    break;
                case 6:
                    Tile[x, y].FAnim = tileNum;
                    Tile[x, y].FAnimSet = tileset;
                    break;
                case 7:
                    Tile[x, y].Fringe2 = tileNum;
                    Tile[x, y].Fringe2Set = tileset;
                    break;
                case 8:
                    Tile[x, y].F2Anim = tileNum;
                    Tile[x, y].F2AnimSet = tileset;
                    break;
            }
        }

        public void SetNpcSpawnPoint(int npcSlot, int spawnX, int spawnY) {
            Npc[npcSlot].SpawnX = spawnX;
            Npc[npcSlot].SpawnY = spawnY;
        }

        public void SetNpc(int npcSlot, int npcNum) {
            Npc[npcSlot].NpcNum = npcNum;
        }

        private bool CanCharacterSeeDestination(Combat.ICharacter viewer, int targetX, int targetY) {
            int viewerX = viewer.X;
            int viewerY = viewer.Y;

            //adjust for screen endings
            if (viewerX < 10) viewerX = 10;
            if (viewerX > MaxX - 9) viewerX = MaxX - 9;
            if (viewerY < 7) viewerY = 7;
            if (viewerY > MaxY - 7) viewerY = MaxY - 7;

            //check to see if the target would be in the viewer's screen
            if (targetX > viewerX + 9) return false;
            if (targetX < viewerX - 10) return false;
            if (targetY > viewerY + 7) return false;
            if (targetY < viewerY - 7) return false;

            int darkness;
            if (viewer.Darkness > -2) {
                darkness = viewer.Darkness;
            } else {
                darkness = Darkness;
            }

            if (darkness > -1) {
                int distance = (int)System.Math.Floor(2 * System.Math.Sqrt(System.Math.Pow(viewer.X - targetX, 2) + System.Math.Pow(viewer.Y - targetY, 2)));
                if (distance > darkness - 1) return false;
            }

            return true;
        }

        #endregion Methods
    }
}