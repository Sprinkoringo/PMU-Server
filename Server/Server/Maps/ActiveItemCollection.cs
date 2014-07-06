using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class ActiveItemCollection
    {
        DataManager.Maps.MapDump rawMap;
        MapItem[] mapItems;

        public ActiveItemCollection(DataManager.Maps.MapDump rawMap) {
            this.rawMap = rawMap;
            mapItems = new MapItem[rawMap.ActiveItem.Length];
            for (int i = 0; i < mapItems.Length; i++) {
                mapItems[i] = new MapItem(rawMap.ActiveItem[i]);
                if (mapItems[i].Num == 0) {
                    mapItems[i].Num = -1;
                }
            }
        }

        public int Length {
            get { return mapItems.Length; }
        }

        public MapItem this[int index] {
            get {
                return mapItems[index];
            }
            set {
                mapItems[index] = value;
                rawMap.ActiveItem[index] = value.RawItem;
            }
        }

        public System.Collections.IEnumerator GetEnumerator() {
            return mapItems.GetEnumerator();
        }
    }
}
