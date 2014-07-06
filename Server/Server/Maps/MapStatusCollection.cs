using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class MapStatusCollection
    {
        DataManager.Maps.MapDump mapDump;
        List<MapStatus> mapStatusList;

        public MapStatusCollection(DataManager.Maps.MapDump mapDump) {
            this.mapDump = mapDump;

            mapStatusList = new List<MapStatus>(mapDump.TempStatus.Count);
            for (int i = 0; i < mapDump.TempStatus.Count; i++) {
                mapStatusList.Add(new MapStatus(mapDump.TempStatus[i]));
            }
        }


        public int Count {
            get { return mapStatusList.Count; }
        }

        public MapStatus this[int index] {
            get {
                return mapStatusList[index];
            }
            set {
                mapStatusList[index] = value;
                mapDump.TempStatus[index] = value.RawStatus;
            }
        }

        public void Add(MapStatus mapStatus) {
            mapStatusList.Add(mapStatus);
            mapDump.TempStatus.Add(mapStatus.RawStatus);
        }


        public void Remove(string name) {
            MapStatus mapStatus = null;
            foreach (MapStatus status in mapStatusList) {
                if (status.Name == name) {
                    mapStatus = status;
                    break;
                }
            }
            if (mapStatus != null) {
                mapStatusList.Remove(mapStatus);
                mapDump.TempStatus.Remove(mapStatus.RawStatus);
            }
        }

        public MapStatus GetStatus(string name) {
            foreach (MapStatus status in mapStatusList) {
                if (status.Name == name) {
                    return status;
                }
            }
            return null;
        }

    }
}
