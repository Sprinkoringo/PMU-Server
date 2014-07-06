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
using System.Text;
using PMU.DatabaseConnector.MySql;
using PMU.DatabaseConnector;
using Server.Database;

namespace Server.Dungeons
{
    public class DungeonManagerBase
    {
        static DungeonCollection dungeons;

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        public static void Initialize() {
            dungeons = new DungeonCollection();
        }

        public static DungeonCollection Dungeons {
            get { return dungeons; }
        }

        public static void LoadDungeons(object object1) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                string query = "SELECT COUNT(num) FROM dungeon";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                int count = row["COUNT(num)"].ValueString.ToInt();
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        LoadDungeon(i, dbConnection.Database);
                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(i, count - 1));
                    }
                    catch (Exception ex)
                    {
                        Exceptions.ErrorLogger.WriteToErrorLog(ex, "Loading dungeon #" + i.ToString());
                    }
                }
                if (LoadComplete != null)
                    LoadComplete(null, null);
            }
        }

        public static void LoadDungeon(int dungeonNum, MySql database)
        {
            Dungeon dungeon = new Dungeon();

            string query = "SELECT name, " +
                "rescue " +
                "FROM dungeon WHERE dungeon.num = \'" + dungeonNum + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                dungeon.Name = row["name"].ValueString;
                dungeon.AllowsRescue = row["rescue"].ValueString.ToBool();
            }

            query = "SELECT map_id, " +
                "difficulty, " +
                "boss_map " +
                "FROM dungeon_smap WHERE dungeon_smap.num = \'" + dungeonNum + "\'";

            foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
            {
                int mapID = columnCollection["map_id"].ValueString.ToInt();

                StandardDungeonMap sMap = new StandardDungeonMap();
                sMap.MapNum = mapID;
                sMap.Difficulty = (Enums.JobDifficulty)columnCollection["difficulty"].ValueString.ToInt();
                sMap.IsBadGoalMap = columnCollection["boss_map"].ValueString.ToBool();

                dungeon.StandardMaps.Add(sMap);
            }

            query = "SELECT rdungeon_num, " +
                "rdungeon_floor, " +
                "difficulty, " +
                "boss_map " +
                "FROM dungeon_rmap WHERE dungeon_rmap.num = \'" + dungeonNum + "\'";

            foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
            {
                int dungeonIndex = columnCollection["rdungeon_num"].ValueString.ToInt();
                int dungeonFloor = columnCollection["rdungeon_floor"].ValueString.ToInt();

                RandomDungeonMap rMap = new RandomDungeonMap();
                rMap.RDungeonIndex = dungeonIndex;
                rMap.RDungeonFloor = dungeonFloor;
                rMap.Difficulty = (Enums.JobDifficulty)columnCollection["difficulty"].ValueString.ToInt();
                rMap.IsBadGoalMap = columnCollection["boss_map"].ValueString.ToBool();

                dungeon.RandomMaps.Add(rMap);
            }

            query = "SELECT script_key, " +
                "script_args " +
                "FROM dungeon_script WHERE dungeon_script.num = \'" + dungeonNum + "\'";

            foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
            {
                int scriptKey = columnCollection["script_key"].ValueString.ToInt();
                string scriptArgs = columnCollection["script_args"].ValueString;

                dungeon.ScriptList.Add(scriptKey, scriptArgs);
            }

            dungeons.Dungeons.Add(dungeonNum, dungeon);
        }

        public static void AddDungeon() {
            SaveDungeon(dungeons.Count);
        }

        public static void SaveDungeon(int dungeonNum)
        {
            if (dungeons.Dungeons.ContainsKey(dungeonNum) == false)
                dungeons.Dungeons.Add(dungeonNum, new Dungeon());
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                database.BeginTransaction();

                database.ExecuteNonQuery("DELETE FROM dungeon WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM dungeon_smap WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM dungeon_rmap WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM dungeon_script WHERE num = \'" + dungeonNum + "\'");

                database.UpdateOrInsert("dungeon", new IDataColumn[] {
                database.CreateColumn(false, "num", dungeonNum.ToString()),
                database.CreateColumn(false, "name", dungeons[dungeonNum].Name),
                database.CreateColumn(false, "rescue", dungeons[dungeonNum].AllowsRescue.ToIntString())
            });

                for (int i = 0; i < dungeons[dungeonNum].StandardMaps.Count; i++)
                {
                    database.UpdateOrInsert("dungeon_smap", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "map_id", dungeons[dungeonNum].StandardMaps[i].MapNum.ToString()),
                    database.CreateColumn(false, "difficulty", ((int)dungeons[dungeonNum].StandardMaps[i].Difficulty).ToString()),
                    database.CreateColumn(false, "boss_map", dungeons[dungeonNum].StandardMaps[i].IsBadGoalMap.ToIntString())
                });
                }

                for (int i = 0; i < dungeons[dungeonNum].RandomMaps.Count; i++)
                {
                    database.UpdateOrInsert("dungeon_rmap", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "rdungeon_num", dungeons[dungeonNum].RandomMaps[i].RDungeonIndex.ToString()),
                    database.CreateColumn(false, "rdungeon_floor", dungeons[dungeonNum].RandomMaps[i].RDungeonFloor.ToString()),
                    database.CreateColumn(false, "difficulty", ((int)dungeons[dungeonNum].RandomMaps[i].Difficulty).ToString()),
                    database.CreateColumn(false, "boss_map", dungeons[dungeonNum].RandomMaps[i].IsBadGoalMap.ToIntString())
                });
                }

                for (int i = 0; i < dungeons[dungeonNum].ScriptList.Count; i++)
                {
                    database.UpdateOrInsert("dungeon_script", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "script_key", dungeons[dungeonNum].ScriptList.KeyByIndex(i).ToString()),
                    database.CreateColumn(false, "script_args", dungeons[dungeonNum].ScriptList.ValueByIndex(i).ToString())
                });
                }
                database.EndTransaction();
            }
        }
    }
}
