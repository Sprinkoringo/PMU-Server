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
using PMU.DatabaseConnector;
using PMU.DatabaseConnector.MySql;
using Server.Database;

namespace Server.WonderMails {
    public class WonderMailManager {

        static MissionPoolCollection missionPools;


        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        public static void Initialize() {
            missionPools = new MissionPoolCollection();
        }

        public static MissionPoolCollection Missions {
            get { return missionPools; }
        }

        public static void LoadMissionPools(object object1) {
            int count = Enum.GetValues(typeof(Enums.JobDifficulty)).Length;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                for (int i = 0; i < count; i++) {
                    try {
                        LoadMissionPool(dbConnection, i);
                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(i, count - 1));
                    } catch (Exception ex) {
                        Exceptions.ErrorLogger.WriteToErrorLog(ex, "Loading MissionPool #" + i.ToString());
                    }
                }
            }
            if (LoadComplete != null)
                LoadComplete(null, null);
        }

        public static void LoadMissionPool(DatabaseConnection dbConnection, int difficulty) {
            MissionPool missionPool = new MissionPool();
            MySql database = dbConnection.Database;

            string query = "SELECT mission_client.DexNum, mission_client.FormNum " +
                "FROM mission_client " +
                "WHERE mission_client.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_client.ClientIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                MissionClientData data = new MissionClientData();
                data.Species = column["DexNum"].ValueString.ToInt();
                data.Form = column["FormNum"].ValueString.ToInt();
                missionPool.MissionClients.Add(data);
            }

            query = "SELECT mission_enemy.NpcNum " +
                "FROM mission_enemy " +
                "WHERE mission_enemy.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_enemy.EnemyIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                MissionEnemyData data = new MissionEnemyData();
                data.NpcNum = column["NpcNum"].ValueString.ToInt();
                missionPool.Enemies.Add(data);
            }
            
            query = "SELECT mission_reward.ItemNum, mission_reward.ItemAmount, mission_reward.ItemTag " +
                "FROM mission_reward " +
                "WHERE mission_reward.Rank = \'" + difficulty + "\'  " +
                "ORDER BY mission_reward.RewardIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                MissionRewardData data = new MissionRewardData();
                data.ItemNum = column["ItemNum"].ValueString.ToInt();
                data.Amount = column["ItemAmount"].ValueString.ToInt();
                data.Tag = column["ItemTag"].ValueString;
                missionPool.Rewards.Add(data);
            }


            missionPools.MissionPools.Add(missionPool);
        }

        public static void SaveMissionPool(DatabaseConnection dbConnection, int difficulty)
        {
            MySql database = dbConnection.Database;

            database.ExecuteNonQuery("DELETE FROM mission_client WHERE Rank = \'" + difficulty + "\'");
            
            for (int i = 0; i < missionPools.MissionPools[difficulty].MissionClients.Count; i++) {
                MissionClientData data = missionPools.MissionPools[difficulty].MissionClients[i];
                
            }

            database.ExecuteNonQuery("DELETE FROM mission_enemy WHERE Rank = \'" + difficulty + "\'");
            for (int i = 0; i < missionPools.MissionPools[difficulty].Enemies.Count; i++) {
                MissionEnemyData data = missionPools.MissionPools[difficulty].Enemies[i];

                database.AddRow("mission_enemy", new IDataColumn[] {
                    database.CreateColumn(false, "Rank", difficulty.ToString()),
                    database.CreateColumn(false, "EnemyIndex", i.ToString()),
                    database.CreateColumn(false, "NpcNum", data.NpcNum.ToString())
                });
            }

            database.ExecuteNonQuery("DELETE FROM mission_reward WHERE Rank = \'" + difficulty + "\'");
            for (int i = 0; i < missionPools.MissionPools[difficulty].Rewards.Count; i++) {
                MissionRewardData data = missionPools.MissionPools[difficulty].Rewards[i];


                database.AddRow("mission_reward", new IDataColumn[] {
                    database.CreateColumn(false, "Rank", difficulty.ToString()),
                    database.CreateColumn(false, "RewardIndex", i.ToString()),
                    database.CreateColumn(false, "ItemNum", data.ItemNum.ToString()),
                    database.CreateColumn(false, "ItemAmount", data.Amount.ToString()),
                    database.CreateColumn(false, "ItemTag", data.Tag)
                });
            }
        }
    }
}
