using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Maps
{
    public class MapDump : RawMap
    {
        int MaxMapNpcs = 20;
        int MaxMapItems = 30;

        #region Constructors

        public MapDump(string mapID, int maxMapNpcs, int maxMapItems)
            : base(mapID) {
            this.MaxMapNpcs = maxMapNpcs;
            this.MaxMapItems = maxMapItems;

            this.TempStatus = new List<MapStatus>();
            this.IsSaving = false;
            this.ActiveNpc = new MapNpc[MaxMapNpcs];
            this.ActiveItem = new MapItem[MaxMapItems];
            for (int i = 0; i < MaxMapNpcs; i++) {
                this.ActiveNpc[i] = new MapNpc(i);
            }
            for (int i = 0; i < MaxMapItems; i++) {
                this.ActiveItem[i] = new MapItem();
            }
            Darkness = -1;
            TimeLimit = -1;
        }

        #endregion Constructors

        #region Properties

        public int ActivationTime {
            get;
            set;
        }

        public bool ProcessingPaused {
            get;
            set;
        }

        public MapItem[] ActiveItem {
            get;
            set;
        }

        public MapNpc[] ActiveNpc {
            get;
            set;
        }

        public int SpawnMarker { get; set; }

        public int NpcSpawnWait { get; set; }

        public bool IsSaving {
            get;
            set;
        }

        public List<MapStatus> TempStatus { get; set; }

        public bool TempChange { get; set; }

        public byte CurrentWeather { get; set; }

        #endregion Properties
    }
}
