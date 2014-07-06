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
using System.Linq;
using System.Text;
using PMU.DatabaseConnector;
using Server.Database;
using Server.Network;

namespace Server.Statistics
{
    class PerformanceStatistics
    {
        public static void GatherStatistics() {
            
        }

        public static void DumpStatistics() {
            int playerCount = 0;
            int staffCount = 0;
            foreach (Client i in ClientManager.GetClients()) {
                if (i.Player != null) {
                    if (i.IsPlaying()) {
                        playerCount++;
                        if (Players.Ranks.IsAllowed(i, Enums.Rank.Moniter)) {
                            staffCount++;
                        }
                    }
                }
            }
            int partyCount = Server.Players.Parties.PartyManager.CountActiveParties();
            int activeMapCount = Server.Maps.MapManager.CountActiveMaps();
            int activeNetworkClients = Server.Network.ClientManager.CountActiveClients();

            Process serverProcess = Process.GetCurrentProcess();
            long workingSet = serverProcess.WorkingSet64;
            long elapsedHours = (long)Globals.LiveTime.Elapsed.TotalHours;

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                dbConnection.Database.AddRow("stats_performance", new IDataColumn[] {
                    dbConnection.Database.CreateColumn(false, "Time", DateTime.UtcNow.ToString()),
                    dbConnection.Database.CreateColumn(false, "PlayerCount", playerCount.ToString()),
                    dbConnection.Database.CreateColumn(false, "StaffCount", staffCount.ToString()),
                    dbConnection.Database.CreateColumn(false, "ActiveNetworkClients", activeNetworkClients.ToString()),
                    dbConnection.Database.CreateColumn(false, "ActiveMaps", activeMapCount.ToString()),
                    dbConnection.Database.CreateColumn(false, "ActiveParties", partyCount.ToString()),
                    dbConnection.Database.CreateColumn(false, "WorkingSet", workingSet.ToString()),
                    dbConnection.Database.CreateColumn(false, "ElapsedHours", elapsedHours.ToString())
                });
            }
        }
    }
}
