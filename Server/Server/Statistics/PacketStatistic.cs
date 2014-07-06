using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Statistics {
    public class PacketStatistic {

        public int TimesReceived { get; set; }

        public long TotalProcessTime { get; set; }

        public long TotalDataSplittingTime { get; set; }
        public long CPUUsage { get; set; }

        public PacketStatistic(long processTime, long dataSplittingTime, long cpuUsage) {
            TimesReceived = 1;
            TotalProcessTime = processTime;
            TotalDataSplittingTime = dataSplittingTime;
            CPUUsage = cpuUsage;
        }

        public PacketStatistic() {
            TimesReceived = 0;
            TotalProcessTime = 0;
            TotalDataSplittingTime = 0;
            CPUUsage = 0;
        }
    }
}
