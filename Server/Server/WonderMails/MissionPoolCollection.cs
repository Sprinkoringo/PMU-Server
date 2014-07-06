using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.WonderMails {
    public class MissionPoolCollection {

        #region Fields

        List<MissionPool> missionPools;

        #endregion Fields

        #region Constructors

        public MissionPoolCollection() {
            missionPools = new List<MissionPool>();
        }

        #endregion Constructors

        #region Properties

        public List<MissionPool> MissionPools {
            get { return missionPools; }
        }

        public int Count {
            get { return missionPools.Count; }
        }

        #endregion Properties

        #region Indexers

        public MissionPool this[int index] {
            get { return missionPools[index]; }
            set { missionPools[index] = value; }
        }

        #endregion Indexers

    }
}
