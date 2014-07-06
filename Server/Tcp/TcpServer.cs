namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class TcpServer
    {
        #region Fields

        private AsyncServer mServer;

        #endregion Fields

        #region Constructors

        public TcpServer()
        {
            mServer = new AsyncServer(new AsyncServer.ErrorRecievedHandler(OnErrorRecieved), new AsyncServer.DataRecievedHandler(OnDataRecieved
                ), new AsyncServer.ClientDisconnectHandler(OnClientDisconnected), new AsyncServer.ConnectionRecievedHandler(OnConnectionRecieved));
        }

        #endregion Constructors

        #region Events

        public event AsyncServer.ClientDisconnectHandler ClientDisconnected;

        public event AsyncServer.ConnectionRecievedHandler ConnectionRecieved;

        public event AsyncServer.DataRecievedHandler DataRecieved;

        public event AsyncServer.ErrorRecievedHandler ErrorRecieved;

        #endregion Events

        #region Properties

        public int BufferSize
        {
            get { return mServer.BufferSize; }
            set { mServer.BufferSize = value; }
        }

        public IPAddress LocalIP
        {
            get { return mServer.LocalIP; }
        }

        public int LocalPort
        {
            get { return mServer.LocalPort; }
        }

        #endregion Properties

        #region Methods

        public void Close()
        {
            mServer.Close();
        }

        public void Close(Socket client)
        {
            mServer.Close(client);
        }

        public bool IsConnected(int index)
        {
            if (mServer.GetClient(index) != null) {
                return mServer.GetClient(index).Connected;
            } else
                return false;
        }

        public void Listen(int port, int maxConnections)
        {
            mServer.Listen(port, maxConnections);
        }

        public void Send(string data, Socket client)
        {
            mServer.SendData(data, client);
        }

        public void SetClientReady(int index)
        {
            mServer.SetClientReady(index);
        }

        public void VerifyClient(int index)
        {
            if (mServer.clients[index] != null && mServer.clients[index].Connected == false) {
                mServer.SetClientReady(index);
            }
        }

        public Socket GetClient(int index)
        {
            return mServer.GetClient(index);
        }

        public int GetClientIndex(Socket client)
        {
            return mServer.GetClientIndex(client);
        }

        void OnClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, e);
        }

        void OnConnectionRecieved(object sender, ConnectionRecievedEventArgs e)
        {
            if (ConnectionRecieved != null)
                ConnectionRecieved(this, e);
        }

        void OnDataRecieved(object sender, DataRecievedEventArgs e)
        {
            if (DataRecieved != null)
                DataRecieved(this, e);
        }

        void OnErrorRecieved(object sender, ErrorRecievedEventArgs e)
        {
            if (ErrorRecieved != null)
                ErrorRecieved(this, e);
        }

        #endregion Methods
    }
}