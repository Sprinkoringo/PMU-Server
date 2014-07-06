using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    class ClientKeepAliveTimedEvent : ITimedEvent
    {
         int interval;
        TickCount storedTime;
        string id;

        public ClientKeepAliveTimedEvent(string id) {
            this.id = id;
        }

        public bool TimeElapsed(TickCount currentTick) {
            if (currentTick.Elapsed(storedTime, this.interval)) {
                return true;
            } else {
                return false;
            }
        }

        public string ID {
            get {
                return id;
            }
        }

        public TickCount StoredTime {
            get { return storedTime; }
        }

        public int Interval {
            get { return interval; }
        }

        public void OnTimeElapsed(TickCount currentTick) {
            storedTime = currentTick;
            return;
            PMU.Sockets.IPacket packet = PMU.Sockets.TcpPacket.CreatePacket("keepalive");
            foreach (Network.Client client in Network.ClientManager.GetAllClients()) {
                Network.Messenger.SendDataTo(client, packet);
            }
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.storedTime = currentTick;
            this.interval = interval;
        }
    }
}
