namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    class DataAsyncState
    {
        #region Fields

        private string mBuffer;
        private System.Net.Sockets.Socket mClient;
        private bool mCompleted;
        private byte[] mData;
        private int mIndex;
        private string mPrevData;
        private int mRecievedBytes;
        private IAsyncResult mResult;
        private int mTotalBytes;

        #endregion Fields

        #region Constructors

        internal DataAsyncState(int bufferSize, System.Net.Sockets.Socket client, IAsyncResult ar, int index)
        {
            mData = new byte[bufferSize];
            mClient = client;
            mBuffer = "";
            mRecievedBytes = 0;
            mTotalBytes = -1;
            mPrevData = "";
            mCompleted = false;
            mResult = ar;
            mIndex = index;
        }

        internal DataAsyncState(int bufferSize, System.Net.Sockets.Socket client, int index)
        {
            mData = new byte[bufferSize];
            mClient = client;
            mBuffer = "";
            mRecievedBytes = 0;
            mTotalBytes = -1;
            mPrevData = "";
            mCompleted = false;
            mResult = null;
            mIndex = index;
        }

        internal DataAsyncState(int bufferSize, System.Net.Sockets.Socket client, string dataBuffer, int recievedBytes, int totalBytes, string prevData, int index)
        {
            mData = new byte[bufferSize];
            mClient = client;
            mBuffer = dataBuffer;
            mRecievedBytes = recievedBytes;
            mTotalBytes = totalBytes;
            mPrevData = prevData;
            mCompleted = false;
            mIndex = index;
        }

        #endregion Constructors

        #region Properties

        internal IAsyncResult AsyncResult
        {
            get { return mResult; }
        }

        internal string Buffer
        {
            get { return mBuffer; }
            set { mBuffer = value; }
        }

        internal System.Net.Sockets.Socket Client
        {
            get { return mClient; }
        }

        internal bool Completed
        {
            get { return mCompleted; }
            set { mCompleted = value; }
        }

        internal byte[] Data
        {
            get { return mData; }
        }

        internal int Index
        {
            get { return mIndex; }
        }

        internal string PrevData
        {
            get { return mPrevData; }
            set { mPrevData = value; }
        }

        internal int RecievedBytes
        {
            get { return mRecievedBytes; }
            set { mRecievedBytes = value; }
        }

        internal int TotalBytes
        {
            get { return mTotalBytes; }
            set { mTotalBytes = value; }
        }

        #endregion Properties
    }
}