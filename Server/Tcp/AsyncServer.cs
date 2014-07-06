namespace Server.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class AsyncServer
    {
        #region Fields

        //private exDictionary<int, System.Net.Sockets.Socket> mClients;
        internal Socket[] clients;

        int bufferSize = 1024;
        ConnectionRecievedHandler connectionCallback;
        ClientDisconnectHandler connectionDropCallback;
        DataRecievedHandler dataCallback;
        Encoding encoding;
        ErrorRecievedHandler errorHandler;
        Boolean listening;
        IPAddress localIP;
        Int32 localPort;
        Boolean mClosing;
        Int32 maxConnections;
        Socket serverSocket;

        #endregion Fields

        #region Constructors

        internal AsyncServer(ErrorRecievedHandler err, DataRecievedHandler dataCallback, ClientDisconnectHandler clientCallback,
            ConnectionRecievedHandler connectionCallback)
        {
            localIP = GetLocalIP();
            encoding = Encoding.Default;
            errorHandler = err;
            this.dataCallback = dataCallback;
            connectionDropCallback = clientCallback;
            this.connectionCallback = connectionCallback;
        }

        #endregion Constructors

        #region Delegates

        public delegate void ClientDisconnectHandler(Object sender, DisconnectedEventArgs e);

        public delegate void ConnectionRecievedHandler(Object sender, ConnectionRecievedEventArgs e);

        public delegate void DataRecievedHandler(Object sender, DataRecievedEventArgs e);

        public delegate void ErrorRecievedHandler(Object sender, ErrorRecievedEventArgs e);

        #endregion Delegates

        #region Properties

        protected internal int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        protected internal IPAddress LocalIP
        {
            get { return localIP; }
        }

        protected internal Int32 LocalPort
        {
            get { return localPort; }
        }

        #endregion Properties

        #region Methods

        public void Close()
        {
            try {
                if (mClosing)
                    return;

                mClosing = true;

                serverSocket.Close();

                mClosing = false;
                listening = false;
            } catch { }
        }

        public void Close(System.Net.Sockets.Socket client)
        {
            try {
                if (client.Connected) {
                    client.Close();
                    int index = -1;
                    index = Array.IndexOf(clients, client);
                    //mClients[index] = null;
                    connectionDropCallback(this, new DisconnectedEventArgs(client, index));
                }
            } catch {
            }
        }

        public void Listen(int port, int maxConnections)
        {
            try {
                serverSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
                localPort = port;
                this.maxConnections = maxConnections;
                serverSocket.Bind(iep);
                serverSocket.Listen(10);
                listening = true;
                clients = new Socket[maxConnections];
                serverSocket.BeginAccept(new AsyncCallback(ListenCallback), serverSocket);
            } catch {
            }
        }

        /// <summary>
        /// Sends text data to the specified client
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="remoteClient">The client to send the data to</param>
        public void SendData(string data, Socket remoteClient)
        {
            try {
                if (remoteClient != null && remoteClient.Connected) {
                    byte[] message = encoding.GetBytes(data);
                    remoteClient.BeginSend(message, 0, message.Length, SocketFlags.None,
                        new AsyncCallback(SendCallback), remoteClient);
                }
            } catch { }
        }

        public void SetClientReady(int index)
        {
            if (clients[index] != null) {
                clients[index] = null;
            }
        }

        internal Socket GetClient(int index)
        {
            return clients[index];
        }

        internal int GetClientIndex(Socket client)
        {
            return Array.IndexOf(clients, client);
        }

        private void DoDisconnect(Socket client)
        {
            try {
                int index = Array.IndexOf(clients, client);
                connectionDropCallback(this, new DisconnectedEventArgs(client, index));
                //mClients[index] = null;
                if (client != null && client.Connected) {
                    client.Close();
                }
            } catch {
            }
        }

        private int FindOpenIndex()
        {
            for (int i = 0; i < maxConnections; i++) {
                if (clients[i] == null) {
                    return i;
                }
            }
            return -1;
        }

        private IPAddress GetLocalIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry local = Dns.GetHostEntry(hostName);
            return local.AddressList[0];
        }

        private void ListenCallback(IAsyncResult ar)
        {
            try {
                System.Net.Sockets.Socket listener = ar.AsyncState as System.Net.Sockets.Socket;
                if (listener == null) {
                    //throw (new Exception("Listener Socket no longer exists"));
                    Exceptions.ErrorLogger.WriteToErrorLog(new Exception("Listener socket no longer exists"), "Listener socket no longer exists");
                }

                //CErrorLogger.WriteToErrorLog(new Exception("Listen callback hit"), "Listen callback hit");

                System.Net.Sockets.Socket client = listener.EndAccept(ar);

                if (listening != true) {
                    listener.Close();
                    return;
                }

                int index = FindOpenIndex();
                if (index == -1) {
                    Exceptions.ErrorLogger.WriteToErrorLog(new Exception("Server Full"), "Server Full");
                    client.Close();
                } else {
                    clients[index] = client;
                    connectionCallback(this, new ConnectionRecievedEventArgs(client.RemoteEndPoint, client, index));

                    DataAsyncState state = new DataAsyncState(bufferSize, client, index);
                    if (client.Connected) {
                        client.BeginReceive(state.Data, 0, bufferSize, SocketFlags.None,
                                      new AsyncCallback(RecieveCallback), state);
                    }
                }
                listener.BeginAccept(new AsyncCallback(ListenCallback), listener);
            } catch (ObjectDisposedException ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex, "Disposed exception, server listener.");
                serverSocket.BeginAccept(new AsyncCallback(ListenCallback), serverSocket);
                return;
            } catch (SocketException socketex) {
                Exceptions.ErrorLogger.WriteToErrorLog(socketex, "Socket exception, server listener.");
                RaiseError(socketex.Message, "", (SocketError)socketex.ErrorCode);
                serverSocket.BeginAccept(new AsyncCallback(ListenCallback), serverSocket);
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex, "Normal exception, server listener.");
                serverSocket.BeginAccept(new AsyncCallback(ListenCallback), serverSocket);
            }
        }

        private void RaiseError(string msg, string details, SocketError error)
        {
            try {
                errorHandler(this, new ErrorRecievedEventArgs(error, msg, details));
            } catch { }
        }

        private void RecieveCallback(IAsyncResult ar)
        {
            DataAsyncState state = ar.AsyncState as DataAsyncState;
            try {
                if (state.Client != null && state.Client.Connected) {
                    int packetSize;
                    packetSize = state.Client.EndReceive(ar);
                    if (packetSize < 1) {
                        // Close the client connection since it disconnected.
                        DoDisconnect(state.Client);
                        return;
                    }
                    string stringData = encoding.GetString(state.Data, 0, packetSize);
                    dataCallback(this, new DataRecievedEventArgs(stringData, state.Client.RemoteEndPoint, state.Client, packetSize, state.Index));
                    DataAsyncState state2 = new DataAsyncState(bufferSize, state.Client, state.Index);
                    state.Client.BeginReceive(state2.Data, 0, bufferSize, SocketFlags.None,
                                      new AsyncCallback(RecieveCallback), state2);
                }
            } catch (SocketException sockex) {
                if ((SocketError)sockex.ErrorCode != SocketError.Success) {
                    RaiseError(sockex.Message, "", (SocketError)sockex.ErrorCode);
                    DoDisconnect(state.Client);
                }
            } catch (ObjectDisposedException) {
                DoDisconnect(state.Client);
                return;
            } catch {
                DoDisconnect(state.Client);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket remote = ar.AsyncState as Socket;
            try {
                if (remote != null && remote.Connected) {
                    int sent = remote.EndSend(ar);
                }
            } catch (SocketException sockex) {
                if ((SocketError)sockex.ErrorCode != SocketError.Success) {
                    RaiseError(sockex.Message, "", (SocketError)sockex.ErrorCode);
                }
            } catch (ObjectDisposedException) {
                return;
            } catch (Exception) {
            }
        }

        private string Substring2(string String, int Start, int End)
        {
            //If start is after the end then just flip the values
            if (Start > End) {
                Start = Start ^ End;
                End = Start ^ End;
                Start = Start ^ End;
            }
            if (End > String.Length) End = String.Length;
            //if the end is outside of the string, just make it the end of the string
            return String.Substring(Start, End - Start);
        }

        #endregion Methods
    }
}