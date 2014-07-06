/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


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
