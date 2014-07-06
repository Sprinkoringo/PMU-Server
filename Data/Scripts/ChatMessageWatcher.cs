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