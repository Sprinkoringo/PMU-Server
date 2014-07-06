using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class MapNpcPresetCollection
    {
        DataManager.Maps.RawMap rawMap;
        List<MapNpcPreset> npcPresets;

        public MapNpcPresetCollection(DataManager.Maps.RawMap rawMap) {
            this.rawMap = rawMap;
            npcPresets = new List<MapNpcPreset>();
            for (int i = 0; i < rawMap.Npc.Count; i++) {
                npcPresets.Add(new MapNpcPreset(rawMap.Npc[i]));
            }
        }

        public int Count {
            get { return npcPresets.Count; }
        }

        public void Add(MapNpcPreset npcPreset) {
            npcPresets.Add(npcPreset);
            rawMap.Npc.Add(npcPreset.RawNpcPreset);
        }

        public void RemoveAt(int index) {
            npcPresets.RemoveAt(index);
            rawMap.Npc.RemoveAt(index);
        }

        public void Remove(MapNpcPreset npcPreset) {
            npcPresets.Remove(npcPreset);
            rawMap.Npc.Remove(npcPreset.RawNpcPreset);
        }

        public MapNpcPreset this[int index] {
            get {
                return npcPresets[index];
            }
            set {
                npcPresets[index] = value;
                rawMap.Npc[index] = value.RawNpcPreset;
            }
        }

        public IEnumerator<MapNpcPreset> GetEnumerator() {
            return npcPresets.GetEnumerator();
        }
    }
}
