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
