namespace Server.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.Core;

    public class MissionCollection
    {
        #region Fields

        ListPair<int, Mission> missions;

        #endregion Fields

        #region Constructors

        public MissionCollection()
        {
            missions = new ListPair<int, Mission>();
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get { return missions.Count; }
        }

        public ListPair<int, Mission> Missions
        {
            get { return missions; }
        }

        #endregion Properties

        #region Indexers

        public Mission this[int index]
        {
            get { return missions[index]; }
        }

        #endregion Indexers
    }
}