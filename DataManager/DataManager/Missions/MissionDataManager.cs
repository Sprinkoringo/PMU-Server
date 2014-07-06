using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;
using PMU.DatabaseConnector;
using PMU.DatabaseConnector.MySql;

namespace DataManager.Missions {
    public class MissionDataManager {

        public static IEnumerable<MissionRewardData> LoadMissionRewardData(MySql database, int difficulty) {
            string query = "SELECT mission_reward.ItemNum, mission_reward.ItemAmount, mission_reward.ItemTag " +
                "FROM mission_reward " +
                "WHERE mission_reward.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_reward.RewardIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query)) {
                MissionRewardData data = new MissionRewardData();
                data.ItemNum = column["ItemNum"].ValueString.ToInt();
                data.ItemAmount = column["ItemAmount"].ValueString.ToInt();
                data.ItemTag = column["ItemTag"].ValueString;
                yield return data;
            }
        }

        public static IEnumerable<MissionClientData> LoadMissionClientData(MySql database, int difficulty) {
            string query = "SELECT mission_client.DexNum, mission_client.FormNum " +
                "FROM mission_client " +
                "WHERE mission_client.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_client.ClientIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query)) {
                MissionClientData data = new MissionClientData();
                data.DexNum = column["DexNum"].ValueString.ToInt();
                data.FormNum = column["FormNum"].ValueString.ToInt();
                yield return data;
            }
        }

        public static IEnumerable<MissionEnemyData> LoadMissionEnemyData(MySql database, int difficulty) {
            string query = "SELECT mission_enemy.NpcNum " +
                "FROM mission_enemy " +
                "WHERE mission_enemy.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_enemy.EnemyIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query)) {
                MissionEnemyData data = new MissionEnemyData();
                data.NpcNum = column["NpcNum"].ValueString.ToInt();
                yield return data;
            }
        }



        public static void ClearMissionRewardData(MySql database, int difficulty) {
            database.ExecuteNonQuery("DELETE FROM mission_reward WHERE Rank = \'" + difficulty + "\'");
        }

        public static void ClearMissionClientData(MySql database, int difficulty) {
            database.ExecuteNonQuery("DELETE FROM mission_client WHERE Rank = \'" + difficulty + "\'");
        }

        public static void ClearMissionEnemyData(MySql database, int difficulty) {
            database.ExecuteNonQuery("DELETE FROM mission_enemy WHERE Rank = \'" + difficulty + "\'");
        }



        public static void AddMissionRewardData(MySql database, int difficulty, int index, MissionRewardData data) {
            database.AddRow("mission_reward", new IDataColumn[] {
                database.CreateColumn(false, "Rank", difficulty.ToString()),
                database.CreateColumn(false, "RewardIndex", index.ToString()),
                database.CreateColumn(false, "ItemNum", data.ItemNum.ToString()),
                database.CreateColumn(false, "ItemAmount", data.ItemAmount.ToString()),
                database.CreateColumn(false, "ItemTag", data.ItemTag)
            });
        }

        public static void AddMissionClientData(MySql database, int difficulty, int index, MissionClientData data) {
            database.AddRow("mission_client", new IDataColumn[] {
                database.CreateColumn(false, "Rank", difficulty.ToString()),
                database.CreateColumn(false, "ClientIndex", index.ToString()),
                database.CreateColumn(false, "DexNum", data.DexNum.ToString()),
                database.CreateColumn(false, "FormNum", data.FormNum.ToString())
            });
        }

        public static void AddMissionEnemyData(MySql database, int difficulty, int index, MissionEnemyData data) {
            database.AddRow("mission_enemy", new IDataColumn[] {
                database.CreateColumn(false, "Rank", difficulty.ToString()),
                database.CreateColumn(false, "EnemyIndex", index.ToString()),
                database.CreateColumn(false, "NpcNum", data.NpcNum.ToString())
            });
        }
    }
}
