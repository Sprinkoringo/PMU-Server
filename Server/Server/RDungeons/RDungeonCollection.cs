using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.RDungeons
{
    public class RDungeonCollection
    {
        #region Fields

        ListPair<int, RDungeon> rdungeons;

        #endregion Fields

        #region Constructors

        public RDungeonCollection() {
            rdungeons = new ListPair<int, RDungeon>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, RDungeon> RDungeons {
            get { return rdungeons; }
        }

        public int Count {
            get { return rdungeons.Count; }
        }

        #endregion Properties

        #region Indexers

        public RDungeon this[int index] {
            get { return rdungeons[index]; }
            set { rdungeons[index] = value; }
        }

        #endregion Indexers
    }
}
