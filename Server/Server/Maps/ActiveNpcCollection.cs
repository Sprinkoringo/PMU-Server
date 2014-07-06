using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class ActiveNpcCollection
    {
        DataManager.Maps.MapDump rawMap;
        MapNpc[] mapNpcs;

        public ActiveNpcCollection(DataManager.Maps.MapDump rawMap) {
            this.rawMap = rawMap;
            mapNpcs = new MapNpc[rawMap.ActiveNpc.Length];
            for (int i = 0; i < mapNpcs.Length; i++) {
                mapNpcs[i] = new MapNpc(rawMap.MapID, rawMap.ActiveNpc[i]);
            }
        }

        public int Length {
            get { return mapNpcs.Length; }
        }

        public MapNpc this[int index] {
            get {
                return mapNpcs[index];
            }
            set {
                mapNpcs[index] = value;
                rawMap.ActiveNpc[index] = value.RawNpc;
            }
        }

        public System.Collections.IEnumerator GetEnumerator() {
            return mapNpcs.GetEnumerator();
        }
    }
}
