namespace Server.Players.Parties
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Server.Network;

    public class PartyMember
    {
        #region Fields

        string playerID;
        bool expShared;

        #endregion Fields

        #region Constructors

        public PartyMember(string playerID) {
            this.playerID = playerID;
            expShared = true;
        }

        #endregion Constructors

        #region Properties

        public string PlayerID {
            get { return playerID; }
        }

        public bool ExpShared {
            get { return expShared; }
            set { expShared = value; }
        }

        #endregion Properties
    }
}