namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class DataRecievedEventArgs : EventArgs
    {
        #region Fields

        private string mData;
        private int mDataSize;
        private int mIndex;
        private IPEndPoint mRemoteEndPoint;
        private Socket mSender;

        #endregion Fields

        #region Constructors

        internal DataRecievedEventArgs(string data, EndPoint remoteEndPoint, Socket client, int size, int index) {
            mData = data;
            mDataSize = size;

            mRemoteEndPoint = (IPEndPoint)remoteEndPoint;
            mSender = client;

            mIndex = index;
        }

        #endregion Constructors

        #region Properties

        public string Data {
            get { return mData; }
        }

        public int DataSize {
            get { return mDataSize; }
        }

        public int Index {
            get { return mIndex; }
        }

        public IPAddress RemoteIP {
            get { return mRemoteEndPoint.Address; }
        }

        public int RemotePort {
            get { return mRemoteEndPoint.Port; }
        }

        public Socket Sender {
            get { return mSender; }
        }

        #endregion Properties
    }
}