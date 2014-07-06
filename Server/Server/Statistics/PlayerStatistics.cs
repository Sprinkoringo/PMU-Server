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
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

using Server.Players;


namespace Server.Statistics
{
    public class PlayerStatistics
    {
        Player owner;

        public TimeSpan TotalPlayTime {
            get { return owner.PlayerData.TotalPlayTime; }
        }

        public DateTime LoginTime {
            get { return owner.PlayerData.LastLogin; }
        }

        public PlayerStatistics(Player owner) {
            this.owner = owner;
        }

        public void HandleLogin(string os, string dotNetVersion, string macAddress, IPAddress ipAddress) {
            owner.PlayerData.LastLogin = DateTime.UtcNow;

            owner.PlayerData.LastOS = os;
            owner.PlayerData.LastDotNetVersion = dotNetVersion;
            owner.PlayerData.LastMacAddressUsed = macAddress;
            owner.PlayerData.LastIPAddressUsed = ipAddress.ToString();
        }

        public void HandleLogout() {
            owner.PlayerData.LastLogout = DateTime.UtcNow;
        }

        public void Save() {
            if (owner.PlayerData.TotalPlayTime == null) {
                owner.PlayerData.TotalPlayTime = new TimeSpan();
            }

            owner.PlayerData.TotalPlayTime += DetermineLastPlayTime();
        }

        public TimeSpan DetermineLastPlayTime() {
            if (owner.PlayerData.LastLogin != DateTime.MinValue && owner.PlayerData.LastLogout != DateTime.MinValue) {
                return owner.PlayerData.LastLogout - owner.PlayerData.LastLogin;
            } else {
                return TimeSpan.Zero;
            }
        }

    }
}
