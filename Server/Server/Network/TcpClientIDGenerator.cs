using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMU.Sockets.Tcp;

namespace Server.Network
{
    public class TcpClientIDGenerator : ITcpIDGenerator<TcpClientIdentifier>
    {
        public TcpClientIdentifier GenerateID(TcpClient tcpClient) {
            return new TcpClientIdentifier(tcpClient.Socket.RemoteEndPoint);
        }
    }
}
