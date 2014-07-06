using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMU.Sockets.Tcp;
using System.Net;

namespace Server.Network
{
    public class NetworkManager
    {
        static TcpListener<TcpClientIdentifier> tcpListener;

        public static TcpListener<TcpClientIdentifier> TcpListener {
            get { return NetworkManager.tcpListener; }
        }

        public static void Initialize() {
            ClientManager.Initialize();

            tcpListener = new TcpListener<TcpClientIdentifier>(new TcpClientIDGenerator());
            tcpListener.ConnectionReceived += new EventHandler<ConnectionReceivedEventArgs>(tcpListener_ConnectionReceived);
        }

        static void tcpListener_ConnectionReceived(object sender, ConnectionReceivedEventArgs e) {
            ClientManager.AddNewClient((TcpClientIdentifier)e.ID, e.TcpClient);
        }

        public static TcpClient GetTcpClient(TcpClientIdentifier clientID) {
            return tcpListener.ClientCollection.GetTcpClient(clientID);
        }

        public static bool IsPlaying(Client client) {
            return true;
        }

    }
}
