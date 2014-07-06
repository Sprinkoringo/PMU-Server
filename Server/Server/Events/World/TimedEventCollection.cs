using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    public class TimedEventCollection
    {
        List<ITimedEvent> timedEvents;

        public int Count {
            get { return timedEvents.Count; }
        }

        public TimedEventCollection() {
            timedEvents = new List<ITimedEvent>();
        }

        public void Add(ITimedEvent timedEvent) {
            this.timedEvents.Add(timedEvent);
        }

        public void Remove(ITimedEvent timedEvent) {
            this.timedEvents.Remove(timedEvent);
        }

        public ITimedEvent this[int index] {
            get { return timedEvents[index]; }
        }

        public ITimedEvent this[string id] {
            get {
                for (int i = 0; i < timedEvents.Count; i++) {
                    if (timedEvents[i].ID == id) {
                        return timedEvents[i];
                    }
                }
                return null;
            }
        }
    }
}
