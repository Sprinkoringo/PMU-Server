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
