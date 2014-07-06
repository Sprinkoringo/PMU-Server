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

using Server.Network;

using PMU.DatabaseConnector;
using PMU.DatabaseConnector.MySql;
using Server.Database;

namespace Server
{
    public class Bans
    {
    // Bans a player
        public static void BanPlayer(DatabaseConnection dbConnection, string ip, string bannedID, string bannedAccount,
            string bannedMac, string bannerID, string bannerIP, string unbanDate, Enums.BanType banType) {
            IDataColumn[] columns = new IDataColumn[] {
                dbConnection.Database.CreateColumn(true, "BannedPlayerID", bannedID),
                dbConnection.Database.CreateColumn(false, "BannedPlayerAccount", bannedAccount),
                dbConnection.Database.CreateColumn(false, "BannedPlayerIP", ip),
                dbConnection.Database.CreateColumn(false, "BannedPlayerMac", bannedMac),
                dbConnection.Database.CreateColumn(false, "BannerPlayerID", bannerID),
                dbConnection.Database.CreateColumn(false, "BannerPlayerIP",bannerIP),
                dbConnection.Database.CreateColumn(false, "BannedDate", DateTime.Now.ToString()),
                dbConnection.Database.CreateColumn(false, "UnbanDate", unbanDate),
                dbConnection.Database.CreateColumn(false, "BanType", ((int)banType).ToString())
            };
            dbConnection.Database.AddRow("bans", columns);
        }

        public static Enums.BanType IsIPBanned(DatabaseConnection dbConnection, Client client) {
            string ipToTest = client.IP.ToString();
            return IsBanned(dbConnection, "BannedPlayerIP", ipToTest);
        }

        public static Enums.BanType IsMacBanned(DatabaseConnection dbConnection, Client client) {
            if (client.MacAddress == "") return Enums.BanType.None;
            return IsBanned(dbConnection, "BannedPlayerMac", client.MacAddress);
        }

        public static Enums.BanType IsCharacterBanned(DatabaseConnection dbConnection, string id) {
            return IsBanned(dbConnection, "BannedPlayerID", id);
        }

        public static Enums.BanType IsBanned(DatabaseConnection dbConnection, string column, string value) {
            IDataColumn[] columns = RetrieveField(dbConnection, "UnbanDate", column, value);
            IDataColumn[] dataColumns = RetrieveField(dbConnection, "BanType", column, value);
            if (columns != null) {
                string unbanDate = (string)columns[0].Value;
                if (unbanDate == "-----") {
                    // It's a permanent ban.
                    return (Enums.BanType)((int)dataColumns[0].Value);
                } else {
                    // It's a temp ban
                    DateTime dtUnbanDate = DateTime.Parse(unbanDate);
                    if (DateTime.Now > dtUnbanDate) {
                        RemoveBan(dbConnection, column, value);
                        return Enums.BanType.None;
                    } else {
                        return (Enums.BanType)((int)dataColumns[0].Value);
                    }
                }
            } else {
                // columns was null, which means their entry was not found
                return Enums.BanType.None;
            }
        }

        public static void RemoveBan(DatabaseConnection dbConnection, string column, string value) {
            dbConnection.Database.DeleteRow("bans", column + "=\"" + value + "\"");
        }

        private static IDataColumn[] RetrieveField(DatabaseConnection dbConnection, string fieldToRetrieve, string columnToSearch, string valueToSearch) {
            IDataColumn[] columns = dbConnection.Database.RetrieveRow("bans", fieldToRetrieve, columnToSearch + "=\"" + valueToSearch + "\"");
            if (columns != null) {
                return columns;
            } else {
                return null;
            }
        }


    }
}
