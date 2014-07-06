using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    class DayCycleTimedEvent : ITimedEvent
    {
        int interval;
        TickCount storedTime;
        string id;

        public DayCycleTimedEvent(string id) {
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
            if (Server.Globals.ServerTime == Enums.Time.Day) {
                Server.Globals.ServerTime = Enums.Time.Dusk;
                SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Dusk) {
                Server.Globals.ServerTime = Enums.Time.Night;
                SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Night) {
                Server.Globals.ServerTime = Enums.Time.Dawn;
                SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Dawn) {
                Server.Globals.ServerTime = Enums.Time.Day;
                SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds);
            }
            storedTime = currentTick;
            Network.Messenger.SendGameTimeToAll();
            foreach (Network.Client client in Network.ClientManager.GetClients()) {
                client.Player.MissionBoard.GenerateMission();
            }
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.storedTime = currentTick;
            this.interval = interval;
        }
    }
}
