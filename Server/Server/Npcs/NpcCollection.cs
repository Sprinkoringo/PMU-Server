using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Npcs
{
    public class NpcCollection
    {
        #region Fields

        ListPair<int, Npc> npcs;
        int maxNpcs;

        #endregion Fields

        #region Constructors

        public NpcCollection(int maxNpcs) {
            if (maxNpcs == 0)
                maxNpcs = 50;
            this.maxNpcs = maxNpcs;
            npcs = new ListPair<int, Npc>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Npc> Npcs {
            get { return npcs; }
        }

        public int MaxNpcs {
            get { return maxNpcs; }
        }

        #endregion Properties

        #region Indexers

        public Npc this[int index] {
            get { return npcs[index]; }
            set { npcs[index] = value; }
        }

        #endregion Indexers
    }
}
