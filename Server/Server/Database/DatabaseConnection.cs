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
