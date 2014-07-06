using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMU.Sockets;

namespace Server.Processing
{
    public class PlayerEvent
    {
        string data;
        string[] parameters;

        public string Data {
            get { return data; }
        }

        public string[] Parameters {
            get { return parameters; }
        }

        public PlayerEvent(string data) {
            this.data = data;
            this.parameters = data.Split(TcpPacket.SEP_CHAR);
        }

        public string this[int index] {
            get { return parameters[index]; }
        }
    }
}
