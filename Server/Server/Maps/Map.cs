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