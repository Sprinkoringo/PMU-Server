namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Map : BasicMap, IMap
    {
        internal DataManager.Maps.Map baseMap;
        Object lockObject = new object();


        #region Constructors

        public Map(DataManager.Maps.Map baseMap)
            : base(baseMap) {
                this.baseMap = baseMap;
        }

        #endregion Constructors

        #region Properties

        public bool Cacheable {
            get { return true; }
        }

        public string IDPrefix {
            get { return "s"; }
        }

        public bool Instanced {
            get { return baseMap.Instanced; }
            set { baseMap.Instanced = value; }
        }

        public int MapNum {
            get { return MapID.Remove(0, IDPrefix.Length).ToInt(); }
        }

        public Enums.MapType MapType {
            get { return Enums.MapType.Standard; }
        }

        #endregion Properties

        #region Methods

        public bool IsProcessingComplete() {
            if (Npc.Count < 1) return true;

            int npcsActive = 0;

            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (ActiveNpc[i].Num > 0) {
                    npcsActive++;
                    // An npc is still dead, so processing of this map is incomplete

                }
            }

            if (npcsActive >= MaxNpcs) {
                return true;
            } else {
                return false;
            }
        }

        #endregion Methods

        public void Save() {
            lock (lockObject) {
                using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                    MapManager.SaveStandardMap(dbConnection, MapID, this);
                }
            }
        }
    }
}