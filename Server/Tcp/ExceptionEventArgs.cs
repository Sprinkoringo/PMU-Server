namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    public class ErrorRecievedEventArgs : EventArgs
    {
        #region Fields

        private string mDetails;
        private SocketError mError;
        private string mMsg;

        #endregion Fields

        #region Constructors

        internal ErrorRecievedEventArgs(SocketError ex, string msg, string details)
        {
            mError = ex;
            mMsg = msg;
            mDetails = details;
        }

        #endregion Constructors

        #region Properties

        public string Details
        {
            get { return mDetails; }
        }

        public SocketError Error
        {
            get { return mError; }
        }

        public string Message
        {
            get { return mMsg; }
        }

        #endregion Properties
    }
}