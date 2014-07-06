using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    public interface ITimedEvent
    {
        bool TimeElapsed(TickCount currentTick);
        TickCount StoredTime { get; }
        int Interval { get; }
        void OnTimeElapsed(TickCount currentTick);
        void SetInterval(TickCount currentTick, int interval);
        string ID { get; }
    }
}
