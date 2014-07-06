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