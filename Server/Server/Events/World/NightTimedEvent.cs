using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    class NightTimedEvent : ITimedEvent
    {
        int interval;
        TickCount storedTime;

        public bool TimeElapsed(TickCount currentTick) {
            if (currentTick.Elapsed(storedTime, this.interval)) {
                return true;
            } else {
                return false;
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
                Server.Globals.ServerTime = Enums.Time.Night;
            } else if (Server.Globals.ServerTime == Enums.Time.Night) {
                Server.Globals.ServerTime = Enums.Time.Day;
            }
            storedTime = currentTick;
            Network.Messenger.SendGameTimeToAll();
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.storedTime = currentTick;
            this.interval = interval;
        }
    }
}
