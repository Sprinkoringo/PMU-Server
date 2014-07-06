using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Maps
{
    public class RDungeonMap : BasicMap, IMap
    {
        internal DataManager.Maps.RDungeonMap baseMap;
        Object lockObject = new object();

        public const string ID_PREFIX = "rd";

        public RDungeonMap(DataManager.Maps.RDungeonMap baseMap)
            : base(baseMap) {
            this.baseMap = baseMap;
        }

        public Enums.MapType MapType {
            get { return Enums.MapType.RDungeonMap; }
        }

        public string IDPrefix {
            get { return ID_PREFIX; }
        }

        public bool Cacheable {
            get { return false; }
        }

        public int RDungeonIndex {
            get { return baseMap.RDungeonIndex; }
        }

        public int RDungeonFloor {
            get { return baseMap.RDungeonFloor; }
        }

        public int StartX {
            get { return baseMap.StartX; }
            set { baseMap.StartX = value; }
        }
        public int StartY {
            get { return baseMap.StartY; }
            set { baseMap.StartY = value; }
        }

        public bool IsProcessingComplete() {
            return true;
        }

        public void Save() {
            lock (lockObject) {
                using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                    MapManager.SaveRDungeonMap(dbConnection, MapID, this);
                }
            }
        }
    }
}
