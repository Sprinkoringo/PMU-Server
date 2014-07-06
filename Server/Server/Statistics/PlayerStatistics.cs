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
