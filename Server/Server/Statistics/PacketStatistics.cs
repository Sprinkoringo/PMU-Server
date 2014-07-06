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
