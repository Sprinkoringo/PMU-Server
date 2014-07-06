using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Scripting
{
    public class TimerCollection
    {
        ListPair<string, ThreadedTimer> timers;

        public TimerCollection() {
            timers = new ListPair<string, ThreadedTimer>();
        }

        public ThreadedTimer CreateTimer(string timerID, string methodName, int interval, bool autoStart, bool autoStop, params object[] param) {
            ThreadedTimer timer = new ThreadedTimer(methodName, interval, autoStart, autoStop, param);
            timers.Add(timerID, timer);
            return timer;
        }

        public ThreadedTimer GetTimer(string timerID) {
            if (timers.ContainsKey(timerID)) {
                return timers[timerID];
            } else {
                return null;
            }
        }

        public void RemoveTimer(string timerID) {
            if (TimerExists(timerID)) {
                timers[timerID].Stop();
                timers[timerID].Dispose();
                timers.RemoveAtKey(timerID);
            }
        }

        public bool TimerExists(string timerID) {
            return timers.ContainsKey(timerID);
        }

        public void RemoveAllTimers() {
            for (int i = timers.Count - 1; i > 0; i--) {
                timers[timers.KeyByIndex(i)].Stop();
                timers[timers.KeyByIndex(i)].Dispose();
                timers.RemoveAt(i);
            }
        }
    }
}
