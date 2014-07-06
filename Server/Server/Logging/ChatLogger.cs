using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Logging
{
    public class ChatLogger
    {
        public static void AppendToChatLog(string chatChannel, string text) {
            Logger.AppendToLog("/Chat Logs/" + chatChannel + "/" + DateTime.Now.ToShortDateString().Replace("/", "-") + "/log.txt", text, true);
        }
    }
}
