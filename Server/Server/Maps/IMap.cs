using System;
using System.Collections.Generic;
using System.Text;
using Server.Network;

namespace Server.Maps
{
    public interface IMap
    {
        #region Properties

        DataManager.Maps.MapDump BaseMap {
            get;
        }

        TickCount ActivationTime {
            get;
            set;
        }

        bool ProcessingPaused {
            get;
            set;
        }

        Enums.MapType MapType {
            get;
        }

        ActiveItemCollection ActiveItem {
            get;
            set;
        }

        int MinNpcs { get; set; }
        int MaxNpcs { get; set; }

        int NpcSpawnTime { get; set; }
        TickCount NpcSpawnWait { get; set; }

        ActiveNpcCollection ActiveNpc {
            get;
            set;
        }

        int SpawnMarker { get; set; }

        bool IsSaving {
            get;
            set;
        }

        MapPlayersCollection PlayersOnMap {
            get;
        }

        int Down {
            get;
            set;
        }

        bool Indoors {
            get;
            set;
        }

        int Left {
            get;
            set;
        }

        string MapID {
            get;
            set;
        }

        string IDPrefix { get; }

        int MaxX {
            get;
            set;
        }

        int MaxY {
            get;
            set;
        }

        Enums.MapMoral Moral {
            get;
            set;
        }

        string Music {
            get;
            set;
        }

        string Name {
            get;
            set;
        }

        MapNpcPresetCollection Npc {
            get;
            set;
        }

        int OriginalDarkness {
            get;
            set;
        }

        int Darkness {
            get;
            set;
        }

        int Revision {
            get;
            set;
        }

        int Right {
            get;
            set;
        }

        TileCollection Tile {
            get;
            set;
        }

        int Up {
            get;
            set;
        }

        Enums.Weather Weather {
            get;
            set;
        }

        int TimeLimit {
            get;
            set;
        }

        int DungeonIndex {
            get;
            set;
        }

        bool HungerEnabled {
            get;
            set;
        }

        bool RecruitEnabled {
            get;
            set;
        }

        bool ExpEnabled {
            get;
            set;
        }

        bool TempChange {
            get;
            set;
        }

        bool Cacheable {
            get;
        }

        MapStatusCollection TempStatus { get; set; }

        #endregion Properties

        void ClearActiveItem(int itemSlot);
        void ClearActiveNpc(int npcSlot);
        int FindOpenItemSlot();
        int FindOpenNpcSlot();
        void SpawnItemSlot(int itemSlot, int itemNum, int itemVal, bool sticky, bool hidden, string tag, int x, int y, Client playerFor);
        void SpawnItem(int itemNum, int itemVal, bool sticky, bool hidden, string tag, int x, int y, Client playerFor);
        void SpawnItems();
        void SpawnNpc();
        void SpawnNpc(bool checkSight);
        void SpawnNpc(MapNpcPreset npc);
        void SpawnNpcs();
        void RemoveNpc(int npcSlot);
        bool WillSpawnNow(MapNpcPreset npc);
        bool IsNpcSlotEmpty(int npcSlot);
        void RemakePlayersList();
        bool IsProcessingComplete();
        IEnumerable<Client> GetClients();
        IEnumerable<Client> GetSurroundingClients(IMap map);
        void SetAttribute(int x, int y, Enums.TileType tileType, int data1, int data2, int data3,
            string string1, string string2, string string3);
        void SetTile(int x, int y, int tileNum, int tileset, int layer);
        void SetTile(int x, int y, int tileX, int tileY, int tileset, int layer);
        void SetNpcSpawnPoint(int npcSlot, int spawnX, int spawnY);
        void SetNpc(int npcSlot, int npcNum);
        void Save();
    }
}
