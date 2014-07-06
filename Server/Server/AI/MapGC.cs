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
using Server.Maps;
using System.Threading;

namespace Server.AI
{
    public class MapGC
    {
        internal System.Collections.Concurrent.ConcurrentQueue<string> cleanupQueue;
        ManualResetEvent resetEvent;

        public void SetWaitEvent() {
            resetEvent.Set();
        }

        public MapGC() {
            cleanupQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();
            resetEvent = new ManualResetEvent(false);
        }

        public void AddMaps(List<string> mapList) {
            foreach (string map in mapList) {
                cleanupQueue.Enqueue(map);
            }
        }

        public void Cleanup() {
            while (true) { // Keep this thread alive!

                resetEvent.WaitOne(); // Wait until we should do another GC
                resetEvent.Reset();

                // Go through all queued items
                while (true) {
                    string mapID = null;
                    bool hasItem = cleanupQueue.TryDequeue(out mapID);
                    if (!hasItem) {
                        break; // Break out of the loop
                    }

                    using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                        if (MapManager.GetTotalPlayersOnMap(dbConnection, mapID) == 0) {
                            DataManager.Maps.MapDataManager.DeleteMap(dbConnection.Database, mapID);
                        }
                    }
                }
                
            }
            /*
             * old code for reference:
             * //lock (MapManager.ActiveMapLockObject) {
                bool shouldRemove = false;
                if (map.PlayersOnMap.Count == 0 && tickCount.Elapsed(map.ActivationTime, AIProcessor.MapTTL)) {
                    if (map.IsProcessingComplete()) {
                        shouldRemove = true;

                        for (int i = 1; i < 9; i++) {

                            IMap borderingMap = MapManager.RetrieveActiveBorderingMap(map, (Enums.MapID)i);
                            if (borderingMap != null) {
                                if (borderingMap.PlayersOnMap.Count > 0) {
                                    shouldRemove = false;
                                }
                            }
                        }

                        if (shouldRemove) {
                            MapManager.rwLock.EnterWriteLock();
                            try {
                                if (map.MapType == Enums.MapType.House) {
                                    // Only save the map here when logging out - otherwise, the map is saved when the player is saved
                                    map.Save();
                                }
                                if ((map.MapType == Enums.MapType.RDungeonMap || map.MapType == Enums.MapType.Instanced)) {
                                    using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                                        if (MapManager.GetTotalPlayersOnMap(dbConnection, map.MapID) == 0) {
                                            DataManager.Maps.MapDataManager.DeleteMap(dbConnection.Database, map.MapID);
                                        }
                                    }
                                }

                                MapManager.UnsafeRemoveActiveMap(map.MapID);
                            } finally {
                                MapManager.rwLock.ExitWriteLock();
                            }
                        }
                    }
                }
                //}
             */
        }
    }
}
