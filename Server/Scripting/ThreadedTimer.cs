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
using System.Threading;

namespace Server.Scripting
{
    public class ThreadedTimer
    {
        #region Fields

        bool autoStart;
        bool autoStop;
        int hitCount;
        int interval;
        string methodName;
        object[] @params;
        Timer timer;

        #endregion Fields

        #region Constructors

        public ThreadedTimer(string methodName, int inverval, bool autoStart, bool autoStop, params object[] param) {
            this.methodName = methodName;
            this.interval = inverval;
            this.autoStart = autoStart;
            this.autoStop = autoStop;
            this.@params = param;
            if (autoStart) {
                timer = new Timer(new TimerCallback(TimerCallback), null, 0, inverval);
            } else {
                timer = new Timer(new TimerCallback(TimerCallback), null, Timeout.Infinite, Timeout.Infinite);
            }
        }

        #endregion Constructors

        #region Methods

        public int GetHitCount() {
            return hitCount;
        }

        public void Start() {
            timer.Change(interval, interval);
        }

        public void Stop() {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerCallback(object obj) {
            if (autoStop)
                Stop();
            ScriptManager.InvokeSubSimple(methodName, @params);
            hitCount++;
        }

        public void UpdateParams(params object[] param) {
            this.@params = param;
        }

        internal void Dispose() {
            timer.Dispose();
        }

        #endregion Methods
    }
}
