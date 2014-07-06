using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Server.Exceptions
{
    public class ErrorLogger
    {
        public static void WriteToErrorLog(Exception exception, string optionalInfo) {
            try {
                string date = DateTime.Now.ToShortTimeString();
                string filePath = IO.Paths.LogsFolder + "ErrorLog-" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
                using (StreamWriter writer = new StreamWriter(filePath, true)) {
                    writer.WriteLine("--- " + DateTime.Now.ToLongTimeString() + " ---");
                    writer.WriteLine("Exception: " + exception.ToString());
                    if (!string.IsNullOrEmpty(optionalInfo)) {
                        writer.WriteLine("Additional Data: " + optionalInfo);
                    }
                    writer.WriteLine();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void WriteToErrorLog(Exception exception) {
            WriteToErrorLog(exception, "");
        }

    }
}
