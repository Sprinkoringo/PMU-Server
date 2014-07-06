namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using PMU.DatabaseConnector;
    using PMU.Core;
    using System.Threading;
    using Server.Database;
    using DataManager.Maps;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class MapManager
    {
        #region Fields

        static Dictionary<string, IMap> activeMaps;
        static internal readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        public static int StandardMapCount { get; private set; }

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Methods

        public static void Initialize() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                //method for getting count
                string query = "SELECT COUNT(mapID) FROM map_standard_data";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                StandardMapCount = row["COUNT(mapID)"].ValueString.ToInt();
                activeMaps = new Dictionary<string, IMap>();
            }
        }

        /// <summary>
        /// Loads a standard map
        /// </summary>
        /// <param name="mapID">The map id of the map to load</param>
        public static Map LoadStandardMap(DatabaseConnection dbConnection, string mapID) {
            DataManager.Maps.Map loadedMap = MapDataManager.LoadStandardMap(dbConnection.Database, mapID);
            Map map = new Map(loadedMap);

            //Extra checks on weather
            if (map.Indoors) {
                map.Weather = Enums.Weather.None;
            } else if (Globals.ServerWeather != Enums.Weather.Ambiguous) {
                map.Weather = Globals.ServerWeather;
            }

            //check from number 0 NPCs
            for (int i = 0; i < map.Npc.Count; i++) {
                if (map.Npc[i].NpcNum < 1) {
                    map.Npc.RemoveAt(i);
                    i--;
                }
            }

            return map;
        }

        public static void SaveStandardMap(DatabaseConnection dbConnection, string mapID, Map map) {
            MapDataManager.SaveStandardMap(dbConnection.Database, mapID, map.baseMap);
        }

        public static RDungeonMap LoadRDungeonMap(DatabaseConnection dbConnection, string mapID) {
            DataManager.Maps.RDungeonMap loadedMap = MapDataManager.LoadRDungeonMap(dbConnection.Database, mapID);
            RDungeonMap map = new RDungeonMap(loadedMap);

            return map;
        }

        public static void SaveRDungeonMap(DatabaseConnection dbConnection, string mapID, RDungeonMap map) {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                map.ActiveNpc[i].Save();
            }
            MapDataManager.SaveRDungeonMap(dbConnection.Database, mapID, map.baseMap);
        }

        public static InstancedMap LoadInstancedMap(DatabaseConnection dbConnection, string mapID) {
            DataManager.Maps.InstancedMap loadedMap = MapDataManager.LoadInstancedMap(dbConnection.Database, mapID);
            InstancedMap map = new InstancedMap(loadedMap);

            return map;
        }

        public static void SaveInstancedMap(DatabaseConnection dbConnection, string mapID, InstancedMap map) {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                map.ActiveNpc[i].Save();
            }
            MapDataManager.SaveInstancedMap(dbConnection.Database, mapID, map.baseMap);
        }

        public static int GetTotalPlayersOnMap(DatabaseConnection dbConnection, string mapID) {
            string query = "SELECT COUNT(pmu_players.location.Map) " +
                "FROM pmu_players.location " +
                "WHERE pmu_players.location.Map = \'" + dbConnection.Database.VerifyValueString(mapID) + "\'";
            int playerCount = Convert.ToInt32(dbConnection.Database.ExecuteQuery(query));
            return playerCount;
        }

        public static House LoadHouseMap(DatabaseConnection dbConnection, string mapID) {
            DataManager.Maps.HouseMap loadedMap = MapDataManager.LoadHouseMap(dbConnection.Database, mapID);
            if (loadedMap != null) {
                House map = new House(loadedMap);

                return map;
            } else {
                return null;
            }
        }

        public static void SaveHouseMap(DatabaseConnection dbConnection, string mapID, House map) {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                map.ActiveNpc[i].Save();
            }
            MapDataManager.SaveHouseMap(dbConnection.Database, mapID, map.baseMap);
        }

        public static void UpdateActiveMap(string mapID, IMap map) {
            rwLock.EnterWriteLock();
            try {
                activeMaps[mapID] = map;
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public static IMap RetrieveActiveMap(string mapID) {
            rwLock.EnterReadLock();
            try {
                return UnsafeRetrieveActiveMap(mapID);
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public static IMap UnsafeRetrieveActiveMap(string mapID) {
            IMap map;
            if (activeMaps.TryGetValue(mapID, out map)) {
                return map;
            } else {
                return null;
            }
        }

        internal static IMap UnsafeRetrieveActiveMapQuick(string mapID) {
            IMap map;
            if (activeMaps.TryGetValue(mapID, out map)) {
                return map;
            } else {
                return null;
            }
        }

        public static int CountActiveMaps() {
            rwLock.EnterReadLock();
            try {
                return activeMaps.Count;
            } finally {
                rwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Removes an active map to stop processing data on it
        /// </summary>
        /// <param name="mapID">The map to remove</param>
        public static void RemoveActiveMap(string mapID) {
            rwLock.EnterUpgradeableReadLock();
            try {
                if (activeMaps.ContainsKey(mapID)) {
                    // Enter write lock
                    rwLock.EnterWriteLock();
                    try {
                        UnsafeRemoveActiveMap(mapID);
                    } finally {
                        rwLock.ExitWriteLock();
                    }
                    // Exit write lock
                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        public static void UnsafeRemoveActiveMap(string mapID) {
            activeMaps.Remove(mapID);
        }

        /// <summary>
        /// Marks a new map as active to start processing data on it
        /// </summary>
        /// <param name="map">The map to add</param>
        public static void AddActiveMap(IMap map) {
            rwLock.EnterUpgradeableReadLock();
            try {
                // Make sure a map with the same ID isn't already active
                if (activeMaps.ContainsKey(map.MapID) == false) {
                    // Enter write lock
                    rwLock.EnterWriteLock();
                    try {
                        UnsafeAddActiveMap(map);
                    } finally {
                        rwLock.ExitWriteLock();
                    }
                    // Exit write lock
                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Marks a map as active and starts processing data on it. Does not check if the map is already
        /// loaded!
        /// </summary>
        /// <param name="map">The map to add</param>
        public static void UnsafeAddActiveMap(IMap map) {
            map.ActivationTime = Core.GetTickCount();
            activeMaps.Add(map.MapID, map);
        }

        /// <summary>
        /// Returns whether or not a map is active
        /// </summary>
        /// <param name="mapID">The map to look for</param>
        /// <returns>True if the map is already active; otherwise, false</returns>
        public static bool IsMapActive(string mapID) {
            rwLock.EnterReadLock();
            try {
                return UnsafeIsMapActive(mapID);
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public static bool UnsafeIsMapActive(string mapID) {
            IMap map = null;
            if (activeMaps.TryGetValue(mapID, out map)) {
                return true;
            } else {
                return false;
            }
        }

        public static IMap RetrieveMap(int mapNum) {
            return RetrieveMap(mapNum, false);
        }

        public static IMap RetrieveMap(int mapNum, bool autoActivate) {
            return RetrieveMap(MapManager.GenerateMapID(mapNum), autoActivate);
        }

        /// <summary>
        /// Generates a new, unused map id
        /// </summary>
        /// <param name="idPrefix">The prefix for the generated id</param>
        /// <returns>The generated map id</returns>
        public static string GenerateMapID(string idPrefix) {
            string testID;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                while (true) {
                    // Generate a new ID
                    testID = idPrefix + "-" + Security.PasswordGen.Generate(16);
                    // Check if a map with the same ID already exists
                    if (!DataManager.Maps.MapDataManager.IsMapIDInUse(dbConnection.Database, testID)) {
                        // If it doesn't, our generated ID is useable!
                        return testID;
                    }
                    // If the map does exist, try to generate a new ID
                }
            }
        }

        /// <summary>
        /// Generates a new map id based off of the map number
        /// </summary>
        /// <param name="mapNum">The map number to use when generating the id</param>
        /// <returns>The generated map id</returns>
        public static string GenerateMapID(int mapNum) {
            return "s" + mapNum.ToString();
        }

        public static string GenerateHouseID(string houseOwnerID, int room) {
            return House.ID_PREFIX + "-" + houseOwnerID + "-" + room;
        }

        public static IMap[] ToArray() {
            IMap[] activeMapsCopy;

            rwLock.EnterReadLock();
            try {
                activeMapsCopy = new IMap[activeMaps.Count];
                activeMaps.Values.CopyTo(activeMapsCopy, 0);
            } finally {
                rwLock.ExitReadLock();
            }

            return activeMapsCopy;
        }

        public static IMap RetrieveMap(string mapID) {
            return RetrieveMap(mapID, false);
        }

        public static IMap RetrieveMap(string mapID, bool autoActivate) {
            if (mapID.StartsWith("s")) {
                IMap map;

                bool mapLoaded = false;
                rwLock.EnterUpgradeableReadLock();
                try {
                    map = MapManager.UnsafeRetrieveActiveMap(mapID);
                    if (map == null) {
                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                            map = MapManager.LoadStandardMap(dbConnection, mapID);
                        }
                        mapLoaded = true;
                        if (autoActivate) {
                            rwLock.EnterWriteLock();
                            try {
                                MapManager.UnsafeAddActiveMap(map);
                            } finally {
                                rwLock.ExitWriteLock();
                            }
                        }
                    }
                } finally {
                    rwLock.ExitUpgradeableReadLock();
                }

                if (mapLoaded && autoActivate) {
                    // Spawn everything!
                    map.SpawnNpcs();
                    map.SpawnItems();
                }

                return map;
            } else if (mapID.StartsWith("i")) {
                // Check if the requested map is already active and loaded
                IMap map = null;

                rwLock.EnterUpgradeableReadLock();
                try {
                    map = MapManager.UnsafeRetrieveActiveMap(mapID);
                    // Null if map is not loaded
                    if (map == null) {
                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                            map = LoadInstancedMap(dbConnection, mapID);
                        }
                        bool result = map != null;
                        // Attempt to load the map
                        if (result == true) {
                            if (autoActivate) {
                                // Auto-activate, if needed
                                rwLock.EnterWriteLock();
                                try {
                                    MapManager.UnsafeAddActiveMap(map);
                                } finally {
                                    rwLock.ExitWriteLock();
                                }
                            }
                            Scripting.ScriptManager.InvokeSub("OnMapReloaded", map);
                        } else {
                            map = null;
                        }
                    }
                } finally {
                    rwLock.ExitUpgradeableReadLock();
                }

                if (map == null) {
                    // Uh oh! We couldn't load the map! Default to the start map
                    map = RetrieveMap(MapManager.GenerateMapID(Settings.Crossroads));
                }

                return map;
            } else if (mapID.StartsWith("rd")) {
                // Check if the requested map is already active and loaded
                IMap map = null;

                rwLock.EnterUpgradeableReadLock();
                try {
                    map = MapManager.UnsafeRetrieveActiveMap(mapID);
                    // Null if map is not loaded
                    if (map == null) {
                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                            map = LoadRDungeonMap(dbConnection, mapID);

                        }
                        bool result = map != null;
                        // Attempt to load the map
                        if (result == true) {
                            if (autoActivate) {
                                // Auto-activate, if needed
                                rwLock.EnterWriteLock();
                                try {
                                    MapManager.UnsafeAddActiveMap(map);
                                } finally {
                                    rwLock.ExitWriteLock();
                                }
                            }
                            Scripting.ScriptManager.InvokeSub("OnMapReloaded", map);
                        } else {
                            map = null;
                        }
                    }
                } finally {
                    rwLock.ExitUpgradeableReadLock();
                }

                if (map == null) {
                    // Uh oh! We couldn't load the map! Default to the start map
                    map = RetrieveMap(MapManager.GenerateMapID(Settings.Crossroads));
                }

                return map;
            } else if (mapID.StartsWith("h")) {
                // Check if the requested map is already active and loaded
                IMap map = null;
                bool mapLoaded = false;

                rwLock.EnterUpgradeableReadLock();
                try {
                    map = MapManager.UnsafeRetrieveActiveMap(mapID);
                    // Null if map is not loaded
                    if (map == null) {
                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                            map = LoadHouseMap(dbConnection, mapID);
                        }
                        bool result = map != null;
                        mapLoaded = true;
                        // Attempt to load the map
                        if (result == true) {
                            if (autoActivate) {
                                // Auto-activate, if needed
                                rwLock.EnterWriteLock();
                                try {
                                    MapManager.UnsafeAddActiveMap(map);
                                } finally {
                                    rwLock.ExitWriteLock();
                                }
                            }
                        } else {
                            map = null;
                        }
                    }
                } finally {
                    rwLock.ExitUpgradeableReadLock();
                }

                //if (map == null) {
                //    // Uh oh! We couldn't load the map! Default to the start map
                //    map = RetrieveMap(MapManager.GenerateMapID(Settings.Crossroads));
                //}

                return map;
            } else if (mapID.StartsWith("void")) {
                IMap map = null;
                bool mapLoaded = false;

                rwLock.EnterUpgradeableReadLock();
                try {
                    map = MapManager.UnsafeRetrieveActiveMap(mapID);
                    if (map == null) {
                        string charID = mapID.Replace("void-", "");
                        Network.Client client = Network.ClientManager.FindClientFromCharID(charID);
                        if (client != null) {
                            map = new Void(client.Player);
                            mapLoaded = true;
                            if (autoActivate) {
                                rwLock.EnterWriteLock();
                                try {
                                    MapManager.UnsafeAddActiveMap(map);
                                } finally {
                                    rwLock.ExitWriteLock();
                                }
                            }
                        } else {
                            return null;
                        }
                    }
                } finally {
                    rwLock.ExitUpgradeableReadLock();
                }

                if (map != null && mapLoaded && autoActivate) {
                    // Spawn everything!
                    map.SpawnNpcs();
                    map.SpawnItems();
                }

                return map;
            } else {
                return RetrieveMap(MapManager.GenerateMapID(Settings.Crossroads));
            }
        }

        public static string RetrieveBorderingMapID(IMap map, Enums.MapID mapID) {
            switch (mapID) {
                case Enums.MapID.Active: {
                        return map.MapID;
                    }
                case Enums.MapID.Left: {
                        if (map.Left > 0) {
                            return GenerateMapID(map.Left);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Right: {
                        if (map.Right > 0) {
                            return GenerateMapID(map.Right);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Up: {
                        if (map.Up > 0) {
                            return GenerateMapID(map.Up);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Down: {
                        if (map.Down > 0) {
                            return GenerateMapID(map.Down);
                        } else {
                            return null;
                        }
                    }

                case Enums.MapID.TopLeft: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                if (mapAbove.Left > 0) {
                                    return GenerateMapID(mapAbove.Left);
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.TopRight: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                if (map.Right > 0) {
                                    return GenerateMapID(mapAbove.Right);
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomLeft: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                if (mapBelow.Left > 0) {
                                    return GenerateMapID(mapBelow.Left);
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomRight: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                if (mapBelow.Right > 0) {
                                    return GenerateMapID(mapBelow.Right);
                                }
                            }
                        }
                        return null;
                    }
            }
            return null;
        }

        public static bool IsBorderingMapLoaded(IMap map, Enums.MapID mapID) {
            switch (mapID) {
                case Enums.MapID.Active: {
                        return IsMapActive(map.MapID);
                    }

                case Enums.MapID.Left: {
                        return IsMapActive(GenerateMapID(map.Left));
                    }
                case Enums.MapID.Right: {
                        return IsMapActive(GenerateMapID(map.Right));
                    }
                case Enums.MapID.Up: {
                        return IsMapActive(GenerateMapID(map.Up));
                    }
                case Enums.MapID.Down: {
                        return IsMapActive(GenerateMapID(map.Down));
                    }

                case Enums.MapID.TopLeft: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                return IsMapActive(GenerateMapID(mapAbove.Left));
                            }
                        }
                        return false;
                    }
                case Enums.MapID.TopRight: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                return IsMapActive(GenerateMapID(mapAbove.Right));
                            }
                        }
                        return false;
                    }
                case Enums.MapID.BottomLeft: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                return IsMapActive(GenerateMapID(mapBelow.Left));
                            }
                        }
                        return false;
                    }
                case Enums.MapID.BottomRight: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                return IsMapActive(GenerateMapID(mapBelow.Right));
                            }
                        }
                        return false;
                    }
            }
            return false;
        }

        public static void LoadBorderingMaps(IMap map) {
            for (int i = 1; i < 9; i++) {
                RetrieveBorderingMap(map, (Enums.MapID)i, true);
            }
        }

        public static IMap LoadBorderingMap(IMap map, Enums.MapID mapID) {
            return RetrieveMap(RetrieveBorderingMapID(map, mapID), true);
        }

        internal static IMap RetrieveActiveBorderingMap(IMap map, Enums.MapID mapID) {
            switch (mapID) {
                case Enums.MapID.Active: {
                        return map;
                    }

                case Enums.MapID.Left: {
                        if (map.Left > 0) {
                            return RetrieveActiveMap(GenerateMapID(map.Left));
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Right: {
                        if (map.Right > 0) {
                            return RetrieveActiveMap(GenerateMapID(map.Right));
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Up: {
                        if (map.Up > 0) {
                            return RetrieveActiveMap(GenerateMapID(map.Up));
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Down: {
                        if (map.Down > 0) {
                            return RetrieveActiveMap(GenerateMapID(map.Down));
                        } else {
                            return null;
                        }
                    }

                case Enums.MapID.TopLeft: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                if (mapAbove.Left > 0) {
                                    IMap mapTopLeft = RetrieveActiveMap(GenerateMapID(mapAbove.Left));
                                    if (mapTopLeft != null) {
                                        return mapTopLeft;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.TopRight: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveActiveMap(GenerateMapID(map.Up));
                            if (mapAbove != null) {
                                if (mapAbove.Right > 0) {
                                    IMap mapTopRight = RetrieveActiveMap(GenerateMapID(mapAbove.Right));
                                    if (mapTopRight != null) {
                                        return mapTopRight;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomLeft: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                if (mapBelow.Left > 0) {
                                    IMap mapBottomLeft = RetrieveActiveMap(GenerateMapID(mapBelow.Left));
                                    if (mapBottomLeft != null) {
                                        return mapBottomLeft;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomRight: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveActiveMap(GenerateMapID(map.Down));
                            if (mapBelow != null) {
                                if (mapBelow.Right > 0) {
                                    IMap mapBottomRight = RetrieveActiveMap(GenerateMapID(mapBelow.Right));
                                    if (mapBottomRight != null) {
                                        return mapBottomRight;
                                    }
                                }
                            }
                        }
                        return null;
                    }
            }
            return null;
        }

        public static IMap RetrieveBorderingMap(IMap map, Enums.MapID mapID, bool autoActivate) {
            switch (mapID) {
                case Enums.MapID.Active: {
                        return map;
                    }

                case Enums.MapID.Left: {
                        if (map.Left > 0) {
                            return RetrieveMap(map.Left, autoActivate);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Right: {
                        if (map.Right > 0) {
                            return RetrieveMap(map.Right, autoActivate);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Up: {
                        if (map.Up > 0) {
                            return RetrieveMap(map.Up, autoActivate);
                        } else {
                            return null;
                        }
                    }
                case Enums.MapID.Down: {
                        if (map.Down > 0) {
                            return RetrieveMap(map.Down, autoActivate);
                        } else {
                            return null;
                        }
                    }

                case Enums.MapID.TopLeft: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveMap(map.Up, autoActivate);
                            if (mapAbove != null) {
                                if (mapAbove.Left > 0) {
                                    IMap mapTopLeft = RetrieveMap(mapAbove.Left, autoActivate);
                                    if (mapTopLeft != null) {
                                        return mapTopLeft;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.TopRight: {
                        if (map.Up > 0) {
                            IMap mapAbove = RetrieveMap(map.Up, autoActivate);
                            if (mapAbove != null) {
                                if (mapAbove.Right > 0) {
                                    IMap mapTopRight = RetrieveMap(mapAbove.Right, autoActivate);
                                    if (mapTopRight != null) {
                                        return mapTopRight;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomLeft: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveMap(map.Down, autoActivate);
                            if (mapBelow != null) {
                                if (mapBelow.Left > 0) {
                                    IMap mapBottomLeft = RetrieveMap(mapBelow.Left, autoActivate);
                                    if (mapBottomLeft != null) {
                                        return mapBottomLeft;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case Enums.MapID.BottomRight: {
                        if (map.Down > 0) {
                            IMap mapBelow = RetrieveMap(map.Down, autoActivate);
                            if (mapBelow != null) {
                                if (mapBelow.Right > 0) {
                                    IMap mapBottomRight = RetrieveMap(mapBelow.Right, autoActivate);
                                    if (mapBottomRight != null) {
                                        return mapBottomRight;
                                    }
                                }
                            }
                        }
                        return null;
                    }
            }
            return null;
        }

        public static Enums.MapID GetOppositeMapID(Enums.MapID mapID) {
            switch (mapID) {
                case Enums.MapID.Active: {
                        return Enums.MapID.Active;
                    }
                case Enums.MapID.Left: {
                        return Enums.MapID.Right;
                    }
                case Enums.MapID.Right: {
                        return Enums.MapID.Left;
                    }
                case Enums.MapID.Up: {
                        return Enums.MapID.Down;
                    }
                case Enums.MapID.Down: {
                        return Enums.MapID.Up;
                    }
                case Enums.MapID.TopLeft: {
                        return Enums.MapID.BottomRight;
                    }
                case Enums.MapID.TopRight: {
                        return Enums.MapID.BottomLeft;
                    }
                case Enums.MapID.BottomLeft: {
                        return Enums.MapID.TopRight;
                    }
                case Enums.MapID.BottomRight: {
                        return Enums.MapID.TopLeft;
                    }
                default: {
                        return Enums.MapID.TempActive;
                    }
            }
        }

        #endregion Methods
    }
}