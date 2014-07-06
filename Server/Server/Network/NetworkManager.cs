/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


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
