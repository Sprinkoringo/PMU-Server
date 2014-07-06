using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Server.Network;

namespace Server
{
    public class Globals
    {
        private static string serverStatus;

        public static Enums.Weather ServerWeather { get; set; }
        //public static int WeatherIntensity { get; set; }

        public static Enums.Time ServerTime { get; set; }

        public static Forms.MainUI MainUI { get; set; }

        public static Command CommandLine { get; set; }

        public static bool ServerClosed { get; set; }
        public static bool GMOnly { get; set; }

        public static bool FoolsMode { get; set; }

        public static bool PacketCaching { get; set; }

        public static Stopwatch LiveTime { get; set; }

        public static string ServerStatus {
            get { return serverStatus; }
            set {
                serverStatus = value;
                PacketHitList hitList = null;
                PacketHitList.MethodStart(ref hitList);
                hitList.AddPacketToAll(PacketBuilder.CreateServerStatus());
                PacketHitList.MethodEnded(ref hitList);
            }
        }
    }
}
