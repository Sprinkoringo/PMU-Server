using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Dungeons
{
    public class DungeonCollection
    {
        #region Fields

        ListPair<int, Dungeon> dungeons;

        #endregion Fields

        #region Constructors

        public DungeonCollection() {
            dungeons = new ListPair<int, Dungeon>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Dungeon> Dungeons {
            get { return dungeons; }
        }

        public int Count {
            get { return dungeons.Count; }
        }

        #endregion Properties

        #region Indexers

        public Dungeon this[int index] {
            get { return dungeons[index]; }
            set { dungeons[index] = value; }
        }

        #endregion Indexers
    }
}
