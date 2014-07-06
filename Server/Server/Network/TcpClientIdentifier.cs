using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Server.Network
{
    public class TcpClientIdentifier
    {
        EndPoint endPoint;

        public EndPoint EndPoint {
            get { return endPoint; }
        }

        public TcpClientIdentifier(EndPoint endPoint) {
            this.endPoint = endPoint;
        }
    }
}
