namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Server.Items;
    using Server.Npcs;
    using Server.Players;

    public class InstancedMap : BasicMap, IMap
    {
        internal DataManager.Maps.InstancedMap baseMap;

        Object lockObject = new object();

        #region Constructors

        public InstancedMap(DataManager.Maps.InstancedMap baseMap)
            : base(baseMap) {
            this.baseMap = baseMap;
        }

        public InstancedMap(string mapID) 
            : this(new DataManager.Maps.InstancedMap(mapID, Constants.MAX_MAP_NPCS, Constants.MAX_MAP_ITEMS)) {
        }

        #endregion Constructors

        public Enums.MapType MapType {
            get { return Enums.MapType.Instanced; }
        }

        public bool Cacheable {
            get { return false; }
        }

        public string IDPrefix {
            get { return "i"; }
        }

        public int MapBase {
            get { return baseMap.MapBase; }
            set { baseMap.MapBase = value; }
        }

        public bool IsProcessingComplete() {
            //for (int i = 0; i < Npc.Length; i++) {
            //    if (Npc[i].NpcNum > 0) {
            //        if (ActiveNpc[i].Num == 0) {
            //            // An npc is still dead, so processing of this map is incomplete
            //            return false;
            //        }
            //    }
            //}
            return true;
        }

        public void Save() {
            lock (lockObject) {
                using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                    MapManager.SaveInstancedMap(dbConnection, MapID, this);
                }
            }
        }
    }
}