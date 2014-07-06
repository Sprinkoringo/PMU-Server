namespace Server.Events.World
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TimedEventManager
    {
        #region Fields

        static TimedEventCollection timedEvents;

        #endregion Fields

        #region Properties

        public static TimedEventCollection TimedEvents {
            get { return timedEvents; }
        }

        #endregion Properties

        #region Methods

        public static void Initialize() {
            timedEvents = new TimedEventCollection();
        }

        public static void LoadBasicEvents(TickCount currentTick) {
            TimeSpan time = new TimeSpan(0, 0, 0, 0, Core.GetTickCount().Tick);
            int hour = (int)time.TotalHours % 12;
            
            DayCycleTimedEvent dayCycleEvent = new DayCycleTimedEvent("DayCycle");
            
            if (hour < 4) {
                Server.Globals.ServerTime = Enums.Time.Day;
                int tickDiff = (int)(time.TotalMilliseconds - new TimeSpan((int)time.TotalHours - hour, 0, 0).TotalMilliseconds);
                dayCycleEvent.SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds - tickDiff);
            } else if (hour < 6) {
                Server.Globals.ServerTime = Enums.Time.Dusk;
                int tickDiff = (int)(time.TotalMilliseconds - new TimeSpan((int)time.TotalHours - hour + 4, 0, 0).TotalMilliseconds);
                dayCycleEvent.SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds - tickDiff);
            } else if (hour < 10) {
                Server.Globals.ServerTime = Enums.Time.Night;
                int tickDiff = (int)(time.TotalMilliseconds - new TimeSpan((int)time.TotalHours - hour + 6, 0, 0).TotalMilliseconds);
                dayCycleEvent.SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds - tickDiff);
            } else {
                Server.Globals.ServerTime = Enums.Time.Dawn;
                int tickDiff = (int)(time.TotalMilliseconds - new TimeSpan((int)time.TotalHours - hour + 10, 0, 0).TotalMilliseconds);
                dayCycleEvent.SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds - tickDiff);
            }
            
            timedEvents.Add(dayCycleEvent);

            StatisticSaverTimedEvent statisticSaverEvent = new StatisticSaverTimedEvent("StatisticSaver");
            statisticSaverEvent.SetInterval(currentTick, 3600000);

            timedEvents.Add(statisticSaverEvent);

            ClientKeepAliveTimedEvent clientKeepAliveEvent = new ClientKeepAliveTimedEvent("ClientKeepAlive");
            clientKeepAliveEvent.SetInterval(currentTick, 600000);

            timedEvents.Add(clientKeepAliveEvent);
        }

        //public static void AddMapStatusTimedEvent(string id, int time, Server.Maps.IMap map, TickCount currentTick) {
        //    for (int i = 0; i < timedEvents.Count; i++) {
        //        if (timedEvents[i].ID == id) return;
        //    }
        //    MapStatusTimedEvent mapEvent = new MapStatusTimedEvent(id, map);
        //    mapEvent.SetInterval(currentTick, (int)new TimeSpan(0, 0, time).TotalMilliseconds);
        //    timedEvents.Add(mapEvent);
        //}

        public static void Process(TickCount currentTick) {
            //lock (timedEvents) {
                for (int i = 0; i < timedEvents.Count; i++) {
                    if (timedEvents[i].TimeElapsed(currentTick)) {
                        timedEvents[i].OnTimeElapsed(currentTick);
                    }
                }
            //}
        }

        public static void RemoveEvent(ITimedEvent timedEvent) {
            //lock (timedEvents) {
                timedEvents.Remove(timedEvent);
            //}
        }

        #endregion Methods
    }
}