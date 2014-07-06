/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


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
