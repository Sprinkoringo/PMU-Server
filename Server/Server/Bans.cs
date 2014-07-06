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
