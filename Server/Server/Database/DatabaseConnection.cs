using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PMU.DatabaseConnector.MySql;
using Server.Network;

namespace Server.Database
{
    public class DatabaseConnection : IDisposable
    {
        MySql database;
        Client client;
        DatabaseID databaseID;

        public MySql Database {
            get {
                if (!disposed) {
                    return database;
                } else {
                    throw new ObjectDisposedException("database", "Database connection has been disposed.");
                }
            }
        }

        public DatabaseConnection(DatabaseID databaseID) {
            this.databaseID = databaseID;

            string databaseName = DetermineDatabaseName(databaseID);
            if (!string.IsNullOrEmpty(databaseName)) {
#if !DEBUG
                database = new MySql(Settings.DatabaseIP/*"pmuniverse.servegame.com"*/, Settings.DatabasePort, databaseName, Settings.DatabaseUser, Settings.DatabasePassword);
                 
#else
                database = new MySql("localhost", Settings.DatabasePort, databaseName, Settings.DatabaseUser, Settings.DatabasePassword);
#endif
            }

            database.OpenConnection();
        }

        private string DetermineDatabaseName(DatabaseID databaseID) {
            switch (databaseID) {
                case DatabaseID.Players: {
                        return "pmu_players";
                    }
                case DatabaseID.Data: {
                        return "pmu_data";
                    }
                default: {
                        return null;
                    }
            }
        }

        bool disposed;
        public void Dispose() {
            if (!disposed) {

                if (database != null) {
                    database.CloseConnection();
                }

                disposed = true;
            }
        }
    }
}
