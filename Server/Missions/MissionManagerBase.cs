namespace Server.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MissionManagerBase
    {
        #region Fields

        static MissionCollection missions;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Methods


        public static void Initialize() {
            missions = new MissionCollection();
        }



        #endregion Methods

        public static MissionCollection Missions {
            get { return missions; }
        }

        
    }
}