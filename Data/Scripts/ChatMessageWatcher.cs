using Server;
using Server.Scripting;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using PMU.DatabaseConnector;
using PMU.DatabaseConnector.SQLite;

namespace Script {
    public static class ChatMessageWatcher {
        // Finish this class when the server update that has the OnChatMessageRecieved script method is added
        static SQLite watcherDB;

        public static SQLite WatcherDB {
            get {
                return watcherDB;
            }
        }

        public static void Initialize() {
            //watcherDB = new SQLite(Server.IO.Paths.ScriptsFolder + "ScriptIO/ChatMessageWatcherDB.sqlite", false);
        }

        private static void VerifyDatabase() {
            if (Server.IO.IO.FileExists(Server.IO.Paths.ScriptsFolder + "ScriptIO/ChatMessageWatcherDB.sqlite") == false) {
                System.IO.File.Create(Server.IO.Paths.ScriptsFolder + "ScriptIO/ChatMessageWatcherDB.sqlite").Close();

            }
        }
    }
}