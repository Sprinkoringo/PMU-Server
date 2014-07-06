namespace Server.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class NpcManagerBase
    {
        #region Fields

        protected static NpcCollection npcs;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static NpcCollection Npcs {
            get { return npcs; }
        }

        #endregion Properties

        #region Methods



        public static void Initialize() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                //method for getting count
                string query = "SELECT COUNT(num) FROM npc";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                int count = row["COUNT(num)"].ValueString.ToInt();
                npcs = new NpcCollection(count);
            }
        }

        public static void LoadNpc(int npcNum, MySql database)
        {
            if (npcs.Npcs.ContainsKey(npcNum) == false)
                npcs.Npcs.Add(npcNum, new Npc());

            string query = "SELECT name, " +
                "attack_say, " +
                "species, " +
                "form, " +
                "behavior, " +
                "shiny_chance, " +
                "dawn_spawn, " +
                "day_spawn, " +
                "dusk_spawn, " +
                "night_spawn, " +
                "ai_script, " +
                "recruit_rate, " +
                "move1, " +
                "move2, " +
                "move3, " +
                "move4 " +
                "FROM npc WHERE npc.num = \'" + npcNum + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                npcs[npcNum].Name = row["name"].ValueString;
                npcs[npcNum].AttackSay = row["attack_say"].ValueString;
                npcs[npcNum].Species = row["species"].ValueString.ToInt();
                npcs[npcNum].Form = row["form"].ValueString.ToInt();
                npcs[npcNum].Behavior = (Enums.NpcBehavior)row["behavior"].ValueString.ToInt();
                npcs[npcNum].ShinyChance = row["shiny_chance"].ValueString.ToInt();
                npcs[npcNum].SpawnsAtDawn = row["dawn_spawn"].ValueString.ToBool();
                npcs[npcNum].SpawnsAtDay = row["day_spawn"].ValueString.ToBool();
                npcs[npcNum].SpawnsAtDusk = row["dusk_spawn"].ValueString.ToBool();
                npcs[npcNum].SpawnsAtNight = row["night_spawn"].ValueString.ToBool();
                npcs[npcNum].AIScript = row["ai_script"].ValueString;
                npcs[npcNum].RecruitRate = row["recruit_rate"].ValueString.ToInt();
                npcs[npcNum].Moves[0] = row["move1"].ValueString.ToInt();
                npcs[npcNum].Moves[1] = row["move2"].ValueString.ToInt();
                npcs[npcNum].Moves[2] = row["move3"].ValueString.ToInt();
                npcs[npcNum].Moves[3] = row["move4"].ValueString.ToInt();
            }

            query = "SELECT drop_num, " +
                "item_num, " +
                "item_val, " +
                "tag, " +
                "chance " +
                "FROM npc_drop WHERE npc_drop.npc_num = \'" + npcNum + "\'";

            foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
            {
                int dropNum = columnCollection["drop_num"].ValueString.ToInt();

                npcs[npcNum].Drops[dropNum].ItemNum = columnCollection["item_num"].ValueString.ToInt();
                npcs[npcNum].Drops[dropNum].ItemValue = columnCollection["item_val"].ValueString.ToInt();
                npcs[npcNum].Drops[dropNum].Tag = columnCollection["tag"].ValueString;
                npcs[npcNum].Drops[dropNum].Chance = columnCollection["chance"].ValueString.ToInt();
            }
        }

        public static void LoadNpcs(object object1) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                for (int i = 1; i <= npcs.MaxNpcs; i++)
                {
                    try
                    {
                        LoadNpc(i, dbConnection.Database);
                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(i, npcs.MaxNpcs));
                    }
                    catch (Exception ex)
                    {
                        Exceptions.ErrorLogger.WriteToErrorLog(ex, "Npc #" + i.ToString());
                    }
                }
                if (LoadComplete != null)
                    LoadComplete(null, null);
            }
        }

        public static void SaveNpc(int npcNum)
        {
            if (npcs.Npcs.ContainsKey(npcNum) == false)
                npcs.Npcs.Add(npcNum, new Npc());
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                database.BeginTransaction();
                database.ExecuteNonQuery("DELETE FROM npc WHERE num = \'" + npcNum + "\'");
                database.ExecuteNonQuery("DELETE FROM npc_drop WHERE npc_num = \'" + npcNum + "\'");

                database.UpdateOrInsert("npc", new IDataColumn[] {
                database.CreateColumn(false, "num", npcNum.ToString()),
                database.CreateColumn(false, "name", npcs[npcNum].Name),
                database.CreateColumn(false, "attack_say", npcs[npcNum].AttackSay),
                database.CreateColumn(false, "species", npcs[npcNum].Species.ToString()),
                database.CreateColumn(false, "form", npcs[npcNum].Form.ToString()),
                database.CreateColumn(false, "behavior", ((int)npcs[npcNum].Behavior).ToString()),
                database.CreateColumn(false, "shiny_chance", npcs[npcNum].ShinyChance.ToString()),
                database.CreateColumn(false, "dawn_spawn", npcs[npcNum].SpawnsAtDawn.ToIntString()),
                database.CreateColumn(false, "day_spawn", npcs[npcNum].SpawnsAtDay.ToIntString()),
                database.CreateColumn(false, "dusk_spawn", npcs[npcNum].SpawnsAtDusk.ToIntString()),
                database.CreateColumn(false, "night_spawn", npcs[npcNum].SpawnsAtNight.ToIntString()),
                database.CreateColumn(false, "ai_script", npcs[npcNum].AIScript),
                database.CreateColumn(false, "recruit_rate", npcs[npcNum].RecruitRate.ToString()),
                database.CreateColumn(false, "move1", npcs[npcNum].Moves[0].ToString()),
                database.CreateColumn(false, "move2", npcs[npcNum].Moves[1].ToString()),
                database.CreateColumn(false, "move3", npcs[npcNum].Moves[2].ToString()),
                database.CreateColumn(false, "move4", npcs[npcNum].Moves[3].ToString()),
            });

                for (int i = 0; i < npcs[npcNum].Drops.Length; i++)
                {
                    database.UpdateOrInsert("npc_drop", new IDataColumn[] {
                database.CreateColumn(false, "npc_num", npcNum.ToString()),
                database.CreateColumn(false, "drop_num", i.ToString()),
                database.CreateColumn(false, "item_num", npcs[npcNum].Drops[i].ItemNum.ToString()),
                database.CreateColumn(false, "item_val", npcs[npcNum].Drops[i].ItemValue.ToString()),
                database.CreateColumn(false, "tag", npcs[npcNum].Drops[i].Tag),
                database.CreateColumn(false, "chance", npcs[npcNum].Drops[i].Chance.ToString()),
            });
                }
                database.EndTransaction();
            }

        }

        public static int AddNpc()
        {
            int index = npcs.MaxNpcs;
            SaveNpc(index);
            return index;
        }

        #endregion Methods
    }
}