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
