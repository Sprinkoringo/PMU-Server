namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    public class DisconnectedEventArgs : EventArgs
    {
        #region Fields

        private Socket mClient;
        private int mIndex;

        #endregion Fields

        #region Constructors

        internal DisconnectedEventArgs(System.Net.Sockets.Socket client, int index)
        {
            mClient = client;
            mIndex = index;
        }

        #endregion Constructors

        #region Properties

        public System.Net.Sockets.Socket Client
        {
            get { return mClient; }
        }

        public int Index
        {
            get { return mIndex; }
        }

        #endregion Properties
    }
}