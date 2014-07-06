using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections.Concurrent;

namespace Server.Logging
{
    public class Logger
    {
        const int LOG_THRESHOLD = 1000;
        
        private static int logAmount;

        private static ConcurrentDictionary<string, StringBuilder> logs;

        public static void Initialize() {
            logs = new ConcurrentDictionary<string, StringBuilder>();
        }

        public static void AppendToLog(string logFilePath, string dataToAppend) {
            AppendToLog(logFilePath, dataToAppend, true);
        }

        public static void AppendToLog(string logFilePath, string dataToAppend, bool includeDate) {
            StringBuilder log = logs.GetOrAdd(logFilePath, new StringBuilder());


            if (includeDate) {
                log.AppendLine("[" + DateTime.Now.ToString() + "] " + dataToAppend);
            } else {
                log.AppendLine(dataToAppend);
            }

            logAmount++;

            if (logAmount > LOG_THRESHOLD) {
                SaveLogs();
                logAmount = 0;
            }

            //if (logFilePath.StartsWith("/")) {
            //    logFilePath = IO.Paths.LogsFolder + logFilePath.Substring(1, logFilePath.Length - 1);
            //}
            //if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logFilePath)) == false) {
            //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logFilePath));
            //}
            //using (StreamWriter writer = new StreamWriter(logFilePath, true)) {
            //    if (includeDate) {
            //        dataToAppend = "[" + DateTime.Now.ToString() + "] " + dataToAppend;
            //    }
            //    writer.WriteLine(dataToAppend);
            //}
        }

        public static void SaveLogs() {
            foreach (string key in logs.Keys) {
                StringBuilder log;
                if (logs.TryGetValue(key, out log)) {
                    string logFilePath = key;
                    if (logFilePath.StartsWith("/")) {
                        logFilePath = IO.Paths.LogsFolder + logFilePath.Substring(1, logFilePath.Length - 1);
                    }
                    if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logFilePath)) == false) {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logFilePath));
                    }
                    using (StreamWriter writer = new StreamWriter(logFilePath, true)) {
                        writer.Write(log);
                    }
                }

            }
            logs.Clear();
        }
    }
}
