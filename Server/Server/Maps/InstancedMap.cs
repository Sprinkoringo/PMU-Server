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