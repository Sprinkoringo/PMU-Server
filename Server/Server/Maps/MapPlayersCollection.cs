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
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Server.Network;

    public class MapPlayersCollection
    {
        #region Fields

        List<MapPlayer> playersOnMap;
        ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        #endregion Fields

        #region Constructors

        public MapPlayersCollection() {
            playersOnMap = new List<MapPlayer>();
        }

        #endregion Constructors

        #region Properties

        public int Count {
            get {
                rwLock.EnterReadLock();
                try {
                    return playersOnMap.Count;
                } finally {
                    rwLock.ExitReadLock();
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Add(string playerID) {
            rwLock.EnterUpgradeableReadLock();
            try {
                if (UnsafeIndexOf(playerID) == -1) {

                    rwLock.EnterWriteLock();
                    try {
                        playersOnMap.Add(new MapPlayer(playerID, ClientManager.FindClientFromCharID(playerID)));
                    } finally {
                        rwLock.ExitWriteLock();
                    }

                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        public void Clear() {
            rwLock.EnterWriteLock();
            try {
                playersOnMap.Clear();
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public MapPlayer GetItemByIndex(int index) {
            rwLock.EnterReadLock();
            try {
                return playersOnMap[index];
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public IEnumerable<MapPlayer> GetPlayers() {
            MapPlayer[] playersOnMapCopy;

            rwLock.EnterReadLock();
            try {
                playersOnMapCopy = playersOnMap.ToArray();
            } finally {
                rwLock.ExitReadLock();
            }

            for (int i = 0; i < playersOnMapCopy.Length; i++) {
                yield return playersOnMapCopy[i];
            }
        }

        public MapPlayer[] ToArray() {
            MapPlayer[] playersOnMapCopy;

            rwLock.EnterReadLock();
            try {
                playersOnMapCopy = playersOnMap.ToArray();
            } finally {
                rwLock.ExitReadLock();
            }

            return playersOnMapCopy;
        }

        public void Remove(string playerID) {
            rwLock.EnterUpgradeableReadLock();
            try {
                int index = UnsafeIndexOf(playerID);
                if (index > -1) {

                    rwLock.EnterWriteLock();
                    try {
                        playersOnMap.RemoveAt(index);
                    } finally {
                        rwLock.ExitWriteLock();
                    }

                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        private int UnsafeIndexOf(string playerID) {
            for (int i = 0; i < playersOnMap.Count; i++) {
                if (playersOnMap[i].PlayerID == playerID) {
                    return i;
                }
            }
            return -1;
        }

        #endregion Methods
    }
}