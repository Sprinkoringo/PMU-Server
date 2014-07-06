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
