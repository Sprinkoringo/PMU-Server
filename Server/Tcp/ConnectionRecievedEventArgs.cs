namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class ConnectionRecievedEventArgs : EventArgs
    {
        #region Fields

        private Socket mClient;
        private int mIndex;
        private IPEndPoint mRemoteEndPoint;

        #endregion Fields

        #region Constructors

        internal ConnectionRecievedEventArgs(EndPoint remoteEndPoint, Socket client, int index)
        {
            mRemoteEndPoint = (IPEndPoint) remoteEndPoint;
            mClient = client;
            mIndex = index;
        }

        #endregion Constructors

        #region Properties

        public Socket Client
        {
            get { return mClient; }
        }

        public int Index
        {
            get { return mIndex; }
        }

        public IPAddress RemoteIP
        {
            get { return mRemoteEndPoint.Address; }
        }

        public int RemotePort
        {
            get { return mRemoteEndPoint.Port; }
        }

        #endregion Properties
    }
}