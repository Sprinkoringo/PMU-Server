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
