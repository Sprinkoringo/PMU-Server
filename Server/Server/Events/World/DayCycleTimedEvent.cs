﻿/*The MIT License (MIT)

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
using System.Linq;
using System.Text;

namespace Server.Events.World
{
    class DayCycleTimedEvent : ITimedEvent
    {
        int interval;
        TickCount storedTime;
        string id;

        public DayCycleTimedEvent(string id) {
            this.id = id;
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
            if (Server.Globals.ServerTime == Enums.Time.Day) {
                Server.Globals.ServerTime = Enums.Time.Dusk;
                SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Dusk) {
                Server.Globals.ServerTime = Enums.Time.Night;
                SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Night) {
                Server.Globals.ServerTime = Enums.Time.Dawn;
                SetInterval(currentTick, (int)new TimeSpan(2, 0, 0).TotalMilliseconds);
            } else if (Server.Globals.ServerTime == Enums.Time.Dawn) {
                Server.Globals.ServerTime = Enums.Time.Day;
                SetInterval(currentTick, (int)new TimeSpan(4, 0, 0).TotalMilliseconds);
            }
            storedTime = currentTick;
            Network.Messenger.SendGameTimeToAll();
            foreach (Network.Client client in Network.ClientManager.GetClients()) {
                client.Player.MissionBoard.GenerateMission();
            }
        }

        public void SetInterval(TickCount currentTick, int interval) {
            this.storedTime = currentTick;
            this.interval = interval;
        }
    }
}
