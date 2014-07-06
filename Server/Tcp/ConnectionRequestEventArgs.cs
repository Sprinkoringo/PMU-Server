namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class ConnectionRequestEventArgs : EventArgs
    {
        #region Fields

        private Boolean mCancel;
        private Socket mClient;
        private IPAddress mClientIP;

        #endregion Fields

        #region Constructors

        internal ConnectionRequestEventArgs(Socket newClient)
        {
            mClient = newClient;
            mClientIP = ((IPEndPoint)newClient.RemoteEndPoint).Address;
            mCancel = false;
        }

        #endregion Constructors

        #region Properties

        public Boolean Cancel
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        public Socket Client
        {
            get { return mClient; }
        }

        public IPAddress ClientIP
        {
            get { return mClientIP; }
        }

        #endregion Properties
    }
}