using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.AI;

namespace Server.Events.World
{
    class MapGCTimedEvent : ITimedEvent
    {
        int interval;
        TickCount storedTime;
        string id;
        MapGC mapGC;

        public MapGCTimedEvent(string id, MapGC mapGC) {
            this.id = id;
            this.mapGC = mapGC;
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

            mapGC.SetWaitEvent();
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.storedTime = currentTick;
            this.interval = interval;
        }
    }
}
