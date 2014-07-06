using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Maps
{
    public class MapCollection
    {
        #region Fields

        ListPair<int, Map> maps;
        int maxMaps;

        #endregion Fields

        #region Constructors

        public MapCollection(int maxMaps)
        {
            if (maxMaps == 0)
                maxMaps = 50;
            this.maxMaps = maxMaps;
            maps = new ListPair<int, Map>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Map> Maps
        {
            get { return maps; }
        }

        public int MaxMaps
        {
            get { return maxMaps; }
        }

        #endregion Properties

        #region Indexers

        public Map this[int index]
        {
            get { return maps[index]; }
            set { maps[index] = value; }
        }

        public Map this[string mapID] {
            get { return maps[mapID.Substring(1).ToInt()]; }
            set { maps[mapID.Substring(1).ToInt()] = value; }
        }

        #endregion Indexers
    }
}
