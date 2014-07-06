using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMU.Core;
using PMU.DatabaseConnector.MySql;
using PMU.DatabaseConnector;
using Server.Database;
using System.Collections.Concurrent;

namespace Server.Statistics
{
    class PacketStatistics
    {

        static ConcurrentDictionary<string, PacketStatistic> statistics;

        public static void Initialize() {
            statistics = new ConcurrentDictionary<string, PacketStatistic>();
        }


        public static void AddStatistic(string header, long processTime, long dataSplittingTime, long cpuTime) {
            PacketStatistic stats = statistics.GetOrAdd(header, new PacketStatistic());

            stats.TimesReceived++;
            stats.TotalProcessTime += processTime;
            stats.TotalDataSplittingTime += dataSplittingTime;
            stats.CPUUsage += cpuTime;
        }

        public static void ClearStatistics() {
            statistics.Clear();
        }

        public static void DumpStatistics() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                dbConnection.Database.ExecuteNonQuery("TRUNCATE TABLE packet_statistics");

                foreach (string key in statistics.Keys) {
                    PacketStatistic stats;
                    if (statistics.TryGetValue(key, out stats)) {
                        dbConnection.Database.UpdateOrInsert("packet_statistics", new IDataColumn[] {
                        dbConnection.Database.CreateColumn(true, "Header", key),
                        dbConnection.Database.CreateColumn(false, "Amount", stats.TimesReceived.ToString()),
                        dbConnection.Database.CreateColumn(false, "AverageTime", (stats.TotalProcessTime / stats.TimesReceived * 1000 / (double)System.Diagnostics.Stopwatch.Frequency).ToString()),
                        dbConnection.Database.CreateColumn(false, "AverageDataSplittingTime", (stats.TotalDataSplittingTime / stats.TimesReceived * 1000 / (double)System.Diagnostics.Stopwatch.Frequency).ToString()),
                        dbConnection.Database.CreateColumn(false, "AverageCPUUsage", (stats.CPUUsage / stats.TimesReceived).ToString())
                    });
                    }
                }
            }
        }
    }
}
