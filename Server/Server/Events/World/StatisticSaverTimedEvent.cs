using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    class StatisticSaverTimedEvent : ITimedEvent
    {
        int interval;
        TickCount storedTime;
        string id;

        public StatisticSaverTimedEvent(string id) {
            this.id = id;
        }

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
            Server.Statistics.PerformanceStatistics.DumpStatistics();

            storedTime = currentTick;
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.interval = interval;
            this.storedTime = currentTick;
        }

        public string ID {
            get { return id; }
        }
    }
}
