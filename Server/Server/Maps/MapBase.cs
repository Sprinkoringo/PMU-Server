namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using DataManager.Maps;

    public class MapBase
    {
        RawMap rawMap;

        #region Constructors

        public MapBase(RawMap rawMap) {
            this.rawMap = rawMap;

            Tile = new TileCollection(rawMap);
            Npc = new MapNpcPresetCollection(rawMap);
        }

        public MapBase(string mapID) {
            rawMap = new RawMap(mapID);

            rawMap.MaxX = 19;
            rawMap.MaxY = 14;
            rawMap.Tile = new DataManager.Maps.Tile[rawMap.MaxX + 1, rawMap.MaxY + 1];
        }

        #endregion Constructors

        #region Properties

        public int Down {
            get { return rawMap.Down; }
            set { rawMap.Down = value; }
        }

        public bool Indoors {
            get { return rawMap.Indoors; }
            set { rawMap.Indoors = value; }
        }

        public int Left {
            get { return rawMap.Left; }
            set { rawMap.Left = value; }
        }

        public string MapID {
            get { return rawMap.MapID; }
            set { rawMap.MapID = value; }
        }

        public int MaxX {
            get { return rawMap.MaxX; }
            set { rawMap.MaxX = value; }
        }

        public int MaxY {
            get { return rawMap.MaxY; }
            set { rawMap.MaxY = value; }
        }

        public Enums.MapMoral Moral {
            get { return (Enums.MapMoral)rawMap.Moral; }
            set { rawMap.Moral = (byte)value; }
        }

        public string Music {
            get { return rawMap.Music; }
            set { rawMap.Music = value; }
        }

        public string Name {
            get { return rawMap.Name; }
            set { rawMap.Name = value; }
        }

        public int MinNpcs {
            get { return rawMap.MinNpcs; }
            set { rawMap.MinNpcs = value; }
        }
        public int MaxNpcs {
            get { return rawMap.MaxNpcs; }
            set { rawMap.MaxNpcs = value; }
        }

        public int NpcSpawnTime {
            get { return rawMap.NpcSpawnTime; }
            set { rawMap.NpcSpawnTime = value; }
        }

        public MapNpcPresetCollection Npc {
            get;
            set;
        }

        public int OriginalDarkness {
            get { return rawMap.Darkness; }
            set {
                rawMap.Darkness = value;
                Darkness = value;
            }
        }

        public int Darkness {
            get;
            set;
        }

        public int TimeLimit {
            get { return rawMap.TimeLimit; }
            set { rawMap.TimeLimit = value; }
        }

        public int Revision {
            get { return rawMap.Revision; }
            set { rawMap.Revision = value; }
        }

        public int Right {
            get { return rawMap.Right; }
            set { rawMap.Right = value; }
        }

        public TileCollection Tile {
            get;
            set;
        }

        public int Up {
            get { return rawMap.Up; }
            set { rawMap.Up = value; }
        }

        public virtual Enums.Weather Weather {
            get { return (Enums.Weather)rawMap.Weather; }
            set { rawMap.Weather = (byte)value; }
        }

        public int DungeonIndex {
            get { return rawMap.DungeonIndex; }
            set { rawMap.DungeonIndex = value; }
        }

        public bool HungerEnabled {
            get { return rawMap.HungerEnabled; }
            set { rawMap.HungerEnabled = value; }
        }

        public bool RecruitEnabled {
            get { return rawMap.RecruitEnabled; }
            set { rawMap.RecruitEnabled = value; }
        }

        public bool ExpEnabled {
            get { return rawMap.ExpEnabled; }
            set { rawMap.ExpEnabled = value; }
        }
        #endregion Properties
    }
}