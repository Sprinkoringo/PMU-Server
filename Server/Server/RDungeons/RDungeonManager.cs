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
using Server.Maps;
using PMU.DatabaseConnector.MySql;
using PMU.DatabaseConnector;
using Server.Database;

namespace Server.RDungeons
{
    public class RDungeonManager
    {
        static RDungeonCollection rdungeons;

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        public static void Initialize() {
            rdungeons = new RDungeonCollection();
        }

        public static RDungeonCollection RDungeons {
            get { return rdungeons; }
        }

        public static void LoadRDungeons(Object object1) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                try
                {
                    //method for getting count
                    string query = "SELECT COUNT(num) FROM rdungeon";
                    DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                    int count = row["COUNT(num)"].ValueString.ToInt();
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            LoadRDungeon(i, dbConnection.Database);
                            if (LoadUpdate != null)
                                LoadUpdate(null, new LoadingUpdateEventArgs(i, count));
                        }
                    }
                    if (LoadComplete != null)
                        LoadComplete(null, null);
                }
                catch (Exception ex)
                {
                    Exceptions.ErrorLogger.WriteToErrorLog(ex);
                }
            }
        }

        public static void LoadRDungeon(int dungeonNum, MySql database)
        {
            RDungeon dungeon = new RDungeon(dungeonNum);

            string query = "SELECT name, " +
                "up, " +
                "allow_recruit, " +
                "allow_exp, " +
                "time_limit " +
                "FROM rdungeon WHERE rdungeon.num = \'" + dungeonNum + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                dungeon.DungeonName = row["name"].ValueString;
                bool isUp = row["up"].ValueString.ToBool();
                if (isUp)
                {
                    dungeon.Direction = Enums.Direction.Up;
                }
                else
                {
                    dungeon.Direction = Enums.Direction.Down;
                }
                dungeon.Recruitment = row["allow_recruit"].ValueString.ToBool();
                dungeon.Exp = row["allow_exp"].ValueString.ToBool();
                dungeon.WindTimer = row["time_limit"].ValueString.ToInt();
            }

            query = "SELECT floor_num, " +
                "trap_min, " +
                "trap_max, " +
                "item_min, " +
                "item_max, " +
                "intricacy, " +
                "room_width_min, " +
                "room_width_max, " +
                "room_length_min, " +
                "room_length_max, " +
                "hall_turn_min, " +
                "hall_turn_max, " +
                "hall_var_min, " +
                "hall_var_max, " +
                "water_chance, " +
                "craters, " +
                "crater_min, " +
                "crater_max, " +
                "crater_fuzz, " +
                "npc_respawn, " +
                "npc_min, " +
                "npc_max, " +
                "stairs, " +
                "ground, " +
                "ground_alt1, " +
                "ground_alt2, " +
                "top_left, " +
                "top_center, " +
                "top_right, " +
                "center_left, " +
                "center_center, " +
                "center_right, " +
                "bottom_left, " +
                "bottom_center, " +
                "bottom_right, " +
                "top_left_in, " +
                "top_right_in, " +
                "bottom_left_in, " +
                "bottom_right_in, " +
                "col_top, " +
                "col_center, " +
                "col_bottom, " +
                "row_left, " +
                "row_center, " +
                "row_right, " +
                "isolated, " +
                "stairs_set, " +
                "ground_set, " +
                "ground_alt1_set, " +
                "ground_alt2_set, " +
                "top_left_set, " +
                "top_center_set, " +
                "top_right_set, " +
                "center_left_set, " +
                "center_center_set, " +
                "center_right_set, " +
                "bottom_left_set, " +
                "bottom_center_set, " +
                "bottom_right_set, " +
                "top_left_in_set, " +
                "top_right_in_set, " +
                "bottom_left_in_set, " +
                "bottom_right_in_set, " +
                "col_top_set, " +
                "col_center_set, " +
                "col_bottom_set, " +
                "row_left_set, " +
                "row_center_set, " +
                "row_right_set, " +
                "isolated_set, " +
                "top_left_alt, " +
                "top_center_alt, " +
                "top_right_alt, " +
                "center_left_alt, " +
                "center_center_alt1, " +
                "center_center_alt2, " +
                "center_right_alt, " +
                "bottom_left_alt, " +
                "bottom_center_alt, " +
                "bottom_right_alt, " +
                "top_left_in_alt, " +
                "top_right_in_alt, " +
                "bottom_left_in_alt, " +
                "bottom_right_in_alt, " +
                "col_top_alt, " +
                "col_center_alt, " +
                "col_bottom_alt, " +
                "row_left_alt, " +
                "row_center_alt, " +
                "row_right_alt, " +
                "isolated_alt, " +
                "top_left_alt_set, " +
                "top_center_alt_set, " +
                "top_right_alt_set, " +
                "center_left_alt_set, " +
                "center_center_alt1_set, " +
                "center_center_alt2_set, " +
                "center_right_alt_set, " +
                "bottom_left_alt_set, " +
                "bottom_center_alt_set, " +
                "bottom_right_alt_set, " +
                "top_left_in_alt_set, " +
                "top_right_in_alt_set, " +
                "bottom_left_in_alt_set, " +
                "bottom_right_in_alt_set, " +
                "col_top_alt_set, " +
                "col_center_alt_set, " +
                "col_bottom_alt_set, " +
                "row_left_alt_set, " +
                "row_center_alt_set, " +
                "row_right_alt_set, " +
                "isolated_alt_set, " +
                "water, " +
                "shore_top_left, " +
                "shore_top_right, " +
                "shore_bottom_left, " +
                "shore_bottom_right, " +
                "shore_diag_forth, " +
                "shore_diag_back, " +
                "shore_top, " +
                "shore_left, " +
                "shore_right, " +
                "shore_bottom, " +
                "shore_vert, " +
                "shore_horiz, " +
                "shore_top_left_in, " +
                "shore_top_right_in, " +
                "shore_bottom_left_in, " +
                "shore_bottom_right_in, " +
                "shore_top_in, " +
                "shore_left_in, " +
                "shore_right_in, " +
                "shore_bottom_in, " +
                "shore_isolated, " +
                "water_set, " +
                "shore_top_left_set, " +
                "shore_top_right_set, " +
                "shore_bottom_left_set, " +
                "shore_bottom_right_set, " +
                "shore_diag_forth_set, " +
                "shore_diag_back_set, " +
                "shore_top_set, " +
                "shore_left_set, " +
                "shore_right_set, " +
                "shore_bottom_set, " +
                "shore_vert_set, " +
                "shore_horiz_set, " +
                "shore_top_left_in_set, " +
                "shore_top_right_in_set, " +
                "shore_bottom_left_in_set, " +
                "shore_bottom_right_in_set, " +
                "shore_top_in_set, " +
                "shore_left_in_set, " +
                "shore_right_in_set, " +
                "shore_bottom_in_set, " +
                "shore_isolated_set, " +
                "water_anim, " +
                "shore_top_left_anim, " +
                "shore_top_right_anim, " +
                "shore_bottom_left_anim, " +
                "shore_bottom_right_anim, " +
                "shore_diag_forth_anim, " +
                "shore_diag_back_anim, " +
                "shore_top_anim, " +
                "shore_left_anim, " +
                "shore_right_anim, " +
                "shore_bottom_anim, " +
                "shore_vert_anim, " +
                "shore_horiz_anim, " +
                "shore_top_left_in_anim, " +
                "shore_top_right_in_anim, " +
                "shore_bottom_left_in_anim, " +
                "shore_bottom_right_in_anim, " +
                "shore_top_in_anim, " +
                "shore_left_in_anim, " +
                "shore_right_in_anim, " +
                "shore_bottom_in_anim, " +
                "shore_isolated_anim, " +
                "water_anim_set, " +
                "shore_top_left_anim_set, " +
                "shore_top_right_anim_set, " +
                "shore_bottom_left_anim_set, " +
                "shore_bottom_right_anim_set, " +
                "shore_diag_forth_anim_set, " +
                "shore_diag_back_anim_set, " +
                "shore_top_anim_set, " +
                "shore_left_anim_set, " +
                "shore_right_anim_set, " +
                "shore_bottom_anim_set, " +
                "shore_vert_anim_set, " +
                "shore_horiz_anim_set, " +
                "shore_top_left_in_anim_set, " +
                "shore_top_right_in_anim_set, " +
                "shore_bottom_left_in_anim_set, " +
                "shore_bottom_right_in_anim_set, " +
                "shore_top_in_anim_set, " +
                "shore_left_in_anim_set, " +
                "shore_right_in_anim_set, " +
                "shore_bottom_in_anim_set, " +
                "shore_isolated_anim_set, " +
                "ground_type, " +
                "ground_data1, " +
                "ground_data2, " +
                "ground_data3, " +
                "ground_string1, " +
                "ground_string2, " +
                "ground_string3, " +
                "hall_type, " +
                "hall_data1, " +
                "hall_data2, " +
                "hall_data3, " +
                "hall_string1, " +
                "hall_string2, " +
                "hall_string3, " +
                "water_type, " +
                "water_data1, " +
                "water_data2, " +
                "water_data3, " +
                "water_string1, " +
                "water_string2, " +
                "water_string3, " +
                "wall_type, " +
                "wall_data1, " +
                "wall_data2, " +
                "wall_data3, " +
                "wall_string1, " +
                "wall_string2, " +
                "wall_string3, " +
                "goal_type, " +
                "goal_map, " +
                "goal_x, " +
                "goal_y, " +
                "darkness, " +
                "music " +
                "FROM rdungeon_floor WHERE rdungeon_floor.num = \'" + dungeonNum + "\'";

            List<DataColumnCollection> columnCollections = database.RetrieveRows(query);
            if (columnCollections == null) columnCollections = new List<DataColumnCollection>();
            foreach (DataColumnCollection columnCollection in columnCollections)
            {
                RDungeonFloor floor = new RDungeonFloor();

                int floorNum = columnCollection["floor_num"].ValueString.ToInt();
                floor.Options.TrapMin = columnCollection["trap_min"].ValueString.ToInt();
                floor.Options.TrapMax = columnCollection["trap_max"].ValueString.ToInt();
                floor.Options.ItemMin = columnCollection["item_min"].ValueString.ToInt();
                floor.Options.ItemMax = columnCollection["item_max"].ValueString.ToInt();
                floor.Options.Intricacy = columnCollection["intricacy"].ValueString.ToInt();
                floor.Options.RoomWidthMin = columnCollection["room_width_min"].ValueString.ToInt();
                floor.Options.RoomWidthMax = columnCollection["room_width_max"].ValueString.ToInt();
                floor.Options.RoomLengthMin = columnCollection["room_length_min"].ValueString.ToInt();
                floor.Options.RoomLengthMax = columnCollection["room_length_max"].ValueString.ToInt();
                floor.Options.HallTurnMin = columnCollection["hall_turn_min"].ValueString.ToInt();
                floor.Options.HallTurnMax = columnCollection["hall_turn_max"].ValueString.ToInt();
                floor.Options.HallVarMin = columnCollection["hall_var_min"].ValueString.ToInt();
                floor.Options.HallVarMax = columnCollection["hall_var_max"].ValueString.ToInt();
                floor.Options.WaterFrequency = columnCollection["water_chance"].ValueString.ToInt();
                floor.Options.Craters = columnCollection["craters"].ValueString.ToInt();
                floor.Options.CraterMinLength = columnCollection["crater_min"].ValueString.ToInt();
                floor.Options.CraterMaxLength = columnCollection["crater_max"].ValueString.ToInt();
                floor.Options.CraterFuzzy = columnCollection["crater_fuzz"].ValueString.ToBool();

                floor.NpcSpawnTime = columnCollection["npc_respawn"].ValueString.ToInt();
                floor.NpcMin = columnCollection["npc_min"].ValueString.ToInt();
                floor.NpcMax = columnCollection["npc_max"].ValueString.ToInt();

                floor.StairsX = columnCollection["stairs"].ValueString.ToInt();
                floor.mGroundX = columnCollection["ground"].ValueString.ToInt();
                floor.mGroundAltX = columnCollection["ground_alt1"].ValueString.ToInt();
                floor.mGroundAlt2X = columnCollection["ground_alt2"].ValueString.ToInt();
                floor.mTopLeftX = columnCollection["top_left"].ValueString.ToInt();
                floor.mTopCenterX = columnCollection["top_center"].ValueString.ToInt();
                floor.mTopRightX = columnCollection["top_right"].ValueString.ToInt();
                floor.mCenterLeftX = columnCollection["center_left"].ValueString.ToInt();
                floor.mCenterCenterX = columnCollection["center_center"].ValueString.ToInt();
                floor.mCenterRightX = columnCollection["center_right"].ValueString.ToInt();
                floor.mBottomLeftX = columnCollection["bottom_left"].ValueString.ToInt();
                floor.mBottomCenterX = columnCollection["bottom_center"].ValueString.ToInt();
                floor.mBottomRightX = columnCollection["bottom_right"].ValueString.ToInt();
                floor.mInnerTopLeftX = columnCollection["top_left_in"].ValueString.ToInt();
                floor.mInnerTopRightX = columnCollection["top_right_in"].ValueString.ToInt();
                floor.mInnerBottomLeftX = columnCollection["bottom_left_in"].ValueString.ToInt();
                floor.mInnerBottomRightX = columnCollection["bottom_right_in"].ValueString.ToInt();
                floor.mColumnTopX = columnCollection["col_top"].ValueString.ToInt();
                floor.mColumnCenterX = columnCollection["col_center"].ValueString.ToInt();
                floor.mColumnBottomX = columnCollection["col_bottom"].ValueString.ToInt();
                floor.mRowLeftX = columnCollection["row_left"].ValueString.ToInt();
                floor.mRowCenterX = columnCollection["row_center"].ValueString.ToInt();
                floor.mRowRightX = columnCollection["row_right"].ValueString.ToInt();
                floor.mIsolatedWallX = columnCollection["isolated"].ValueString.ToInt();

                floor.StairsSheet = columnCollection["stairs_set"].ValueString.ToInt();
                floor.mGroundSheet = columnCollection["ground_set"].ValueString.ToInt();
                floor.mGroundAltSheet = columnCollection["ground_alt1_set"].ValueString.ToInt();
                floor.mGroundAlt2Sheet = columnCollection["ground_alt2_set"].ValueString.ToInt();
                floor.mTopLeftSheet = columnCollection["top_left_set"].ValueString.ToInt();
                floor.mTopCenterSheet = columnCollection["top_center_set"].ValueString.ToInt();
                floor.mTopRightSheet = columnCollection["top_right_set"].ValueString.ToInt();
                floor.mCenterLeftSheet = columnCollection["center_left_set"].ValueString.ToInt();
                floor.mCenterCenterSheet = columnCollection["center_center_set"].ValueString.ToInt();
                floor.mCenterRightSheet = columnCollection["center_right_set"].ValueString.ToInt();
                floor.mBottomLeftSheet = columnCollection["bottom_left_set"].ValueString.ToInt();
                floor.mBottomCenterSheet = columnCollection["bottom_center_set"].ValueString.ToInt();
                floor.mBottomRightSheet = columnCollection["bottom_right_set"].ValueString.ToInt();
                floor.mInnerTopLeftSheet = columnCollection["top_left_in_set"].ValueString.ToInt();
                floor.mInnerTopRightSheet = columnCollection["top_right_in_set"].ValueString.ToInt();
                floor.mInnerBottomLeftSheet = columnCollection["bottom_left_in_set"].ValueString.ToInt();
                floor.mInnerBottomRightSheet = columnCollection["bottom_right_in_set"].ValueString.ToInt();
                floor.mColumnTopSheet = columnCollection["col_top_set"].ValueString.ToInt();
                floor.mColumnCenterSheet = columnCollection["col_center_set"].ValueString.ToInt();
                floor.mColumnBottomSheet = columnCollection["col_bottom_set"].ValueString.ToInt();
                floor.mRowLeftSheet = columnCollection["row_left_set"].ValueString.ToInt();
                floor.mRowCenterSheet = columnCollection["row_center_set"].ValueString.ToInt();
                floor.mRowRightSheet = columnCollection["row_right_set"].ValueString.ToInt();
                floor.mIsolatedWallSheet = columnCollection["isolated_set"].ValueString.ToInt();

                floor.mTopLeftAltX = columnCollection["top_left_alt"].ValueString.ToInt();
                floor.mTopCenterAltX = columnCollection["top_center_alt"].ValueString.ToInt();
                floor.mTopRightAltX = columnCollection["top_right_alt"].ValueString.ToInt();
                floor.mCenterLeftAltX = columnCollection["center_left_alt"].ValueString.ToInt();
                floor.mCenterCenterAltX = columnCollection["center_center_alt1"].ValueString.ToInt();
                floor.mCenterCenterAlt2X = columnCollection["center_center_alt2"].ValueString.ToInt();
                floor.mCenterRightAltX = columnCollection["center_right_alt"].ValueString.ToInt();
                floor.mBottomLeftAltX = columnCollection["bottom_left_alt"].ValueString.ToInt();
                floor.mBottomCenterAltX = columnCollection["bottom_center_alt"].ValueString.ToInt();
                floor.mBottomRightAltX = columnCollection["bottom_right_alt"].ValueString.ToInt();
                floor.mInnerTopLeftAltX = columnCollection["top_left_in_alt"].ValueString.ToInt();
                floor.mInnerTopRightAltX = columnCollection["top_right_in_alt"].ValueString.ToInt();
                floor.mInnerBottomLeftAltX = columnCollection["bottom_left_in_alt"].ValueString.ToInt();
                floor.mInnerBottomRightAltX = columnCollection["bottom_right_in_alt"].ValueString.ToInt();
                floor.mColumnTopAltX = columnCollection["col_top_alt"].ValueString.ToInt();
                floor.mColumnCenterAltX = columnCollection["col_center_alt"].ValueString.ToInt();
                floor.mColumnBottomAltX = columnCollection["col_bottom_alt"].ValueString.ToInt();
                floor.mRowLeftAltX = columnCollection["row_left_alt"].ValueString.ToInt();
                floor.mRowCenterAltX = columnCollection["row_center_alt"].ValueString.ToInt();
                floor.mRowRightAltX = columnCollection["row_right_alt"].ValueString.ToInt();
                floor.mIsolatedWallAltX = columnCollection["isolated_alt"].ValueString.ToInt();

                floor.mTopLeftAltSheet = columnCollection["top_left_alt_set"].ValueString.ToInt();
                floor.mTopCenterAltSheet = columnCollection["top_center_alt_set"].ValueString.ToInt();
                floor.mTopRightAltSheet = columnCollection["top_right_alt_set"].ValueString.ToInt();
                floor.mCenterLeftAltSheet = columnCollection["center_left_alt_set"].ValueString.ToInt();
                floor.mCenterCenterAltSheet = columnCollection["center_center_alt1_set"].ValueString.ToInt();
                floor.mCenterCenterAlt2Sheet = columnCollection["center_center_alt2_set"].ValueString.ToInt();
                floor.mCenterRightAltSheet = columnCollection["center_right_alt_set"].ValueString.ToInt();
                floor.mBottomLeftAltSheet = columnCollection["bottom_left_alt_set"].ValueString.ToInt();
                floor.mBottomCenterAltSheet = columnCollection["bottom_center_alt_set"].ValueString.ToInt();
                floor.mBottomRightAltSheet = columnCollection["bottom_right_alt_set"].ValueString.ToInt();
                floor.mInnerTopLeftAltSheet = columnCollection["top_left_in_alt_set"].ValueString.ToInt();
                floor.mInnerTopRightAltSheet = columnCollection["top_right_in_alt_set"].ValueString.ToInt();
                floor.mInnerBottomLeftAltSheet = columnCollection["bottom_left_in_alt_set"].ValueString.ToInt();
                floor.mInnerBottomRightAltSheet = columnCollection["bottom_right_in_alt_set"].ValueString.ToInt();
                floor.mColumnTopAltSheet = columnCollection["col_top_alt_set"].ValueString.ToInt();
                floor.mColumnCenterAltSheet = columnCollection["col_center_alt_set"].ValueString.ToInt();
                floor.mColumnBottomAltSheet = columnCollection["col_bottom_alt_set"].ValueString.ToInt();
                floor.mRowLeftAltSheet = columnCollection["row_left_alt_set"].ValueString.ToInt();
                floor.mRowCenterAltSheet = columnCollection["row_center_alt_set"].ValueString.ToInt();
                floor.mRowRightAltSheet = columnCollection["row_right_alt_set"].ValueString.ToInt();
                floor.mIsolatedWallAltSheet = columnCollection["isolated_alt_set"].ValueString.ToInt();

                floor.mWaterX = columnCollection["water"].ValueString.ToInt();
                floor.mShoreTopLeftX = columnCollection["shore_top_left"].ValueString.ToInt();
                floor.mShoreTopRightX = columnCollection["shore_top_right"].ValueString.ToInt();
                floor.mShoreBottomLeftX = columnCollection["shore_bottom_left"].ValueString.ToInt();
                floor.mShoreBottomRightX = columnCollection["shore_bottom_right"].ValueString.ToInt();
                floor.mShoreDiagonalForwardX = columnCollection["shore_diag_forth"].ValueString.ToInt();
                floor.mShoreDiagonalBackX = columnCollection["shore_diag_back"].ValueString.ToInt();
                floor.mShoreTopX = columnCollection["shore_top"].ValueString.ToInt();
                floor.mShoreLeftX = columnCollection["shore_left"].ValueString.ToInt();
                floor.mShoreRightX = columnCollection["shore_right"].ValueString.ToInt();
                floor.mShoreBottomX = columnCollection["shore_bottom"].ValueString.ToInt();
                floor.mShoreVerticalX = columnCollection["shore_vert"].ValueString.ToInt();
                floor.mShoreHorizontalX = columnCollection["shore_horiz"].ValueString.ToInt();
                floor.mShoreInnerTopLeftX = columnCollection["shore_top_left_in"].ValueString.ToInt();
                floor.mShoreInnerTopRightX = columnCollection["shore_top_right_in"].ValueString.ToInt();
                floor.mShoreInnerBottomLeftX = columnCollection["shore_bottom_left_in"].ValueString.ToInt();
                floor.mShoreInnerBottomRightX = columnCollection["shore_bottom_right_in"].ValueString.ToInt();
                floor.mShoreInnerTopX = columnCollection["shore_top_in"].ValueString.ToInt();
                floor.mShoreInnerLeftX = columnCollection["shore_left_in"].ValueString.ToInt();
                floor.mShoreInnerRightX = columnCollection["shore_right_in"].ValueString.ToInt();
                floor.mShoreInnerBottomX = columnCollection["shore_bottom_in"].ValueString.ToInt();
                floor.mShoreSurroundedX = columnCollection["shore_isolated"].ValueString.ToInt();

                floor.mWaterSheet = columnCollection["water_set"].ValueString.ToInt();
                floor.mShoreTopLeftSheet = columnCollection["shore_top_left_set"].ValueString.ToInt();
                floor.mShoreTopRightSheet = columnCollection["shore_top_right_set"].ValueString.ToInt();
                floor.mShoreBottomLeftSheet = columnCollection["shore_bottom_left_set"].ValueString.ToInt();
                floor.mShoreBottomRightSheet = columnCollection["shore_bottom_right_set"].ValueString.ToInt();
                floor.mShoreDiagonalForwardSheet = columnCollection["shore_diag_forth_set"].ValueString.ToInt();
                floor.mShoreDiagonalBackSheet = columnCollection["shore_diag_back_set"].ValueString.ToInt();
                floor.mShoreTopSheet = columnCollection["shore_top_set"].ValueString.ToInt();
                floor.mShoreLeftSheet = columnCollection["shore_left_set"].ValueString.ToInt();
                floor.mShoreRightSheet = columnCollection["shore_right_set"].ValueString.ToInt();
                floor.mShoreBottomSheet = columnCollection["shore_bottom_set"].ValueString.ToInt();
                floor.mShoreVerticalSheet = columnCollection["shore_vert_set"].ValueString.ToInt();
                floor.mShoreHorizontalSheet = columnCollection["shore_horiz_set"].ValueString.ToInt();
                floor.mShoreInnerTopLeftSheet = columnCollection["shore_top_left_in_set"].ValueString.ToInt();
                floor.mShoreInnerTopRightSheet = columnCollection["shore_top_right_in_set"].ValueString.ToInt();
                floor.mShoreInnerBottomLeftSheet = columnCollection["shore_bottom_left_in_set"].ValueString.ToInt();
                floor.mShoreInnerBottomRightSheet = columnCollection["shore_bottom_right_in_set"].ValueString.ToInt();
                floor.mShoreInnerTopSheet = columnCollection["shore_top_in_set"].ValueString.ToInt();
                floor.mShoreInnerLeftSheet = columnCollection["shore_left_in_set"].ValueString.ToInt();
                floor.mShoreInnerRightSheet = columnCollection["shore_right_in_set"].ValueString.ToInt();
                floor.mShoreInnerBottomSheet = columnCollection["shore_bottom_in_set"].ValueString.ToInt();
                floor.mShoreSurroundedSheet = columnCollection["shore_isolated_set"].ValueString.ToInt();

                floor.mWaterAnimX = columnCollection["water_anim"].ValueString.ToInt();
                floor.mShoreTopLeftAnimX = columnCollection["shore_top_left_anim"].ValueString.ToInt();
                floor.mShoreTopRightAnimX = columnCollection["shore_top_right_anim"].ValueString.ToInt();
                floor.mShoreBottomLeftAnimX = columnCollection["shore_bottom_left_anim"].ValueString.ToInt();
                floor.mShoreBottomRightAnimX = columnCollection["shore_bottom_right_anim"].ValueString.ToInt();
                floor.mShoreDiagonalForwardAnimX = columnCollection["shore_diag_forth_anim"].ValueString.ToInt();
                floor.mShoreDiagonalBackAnimX = columnCollection["shore_diag_back_anim"].ValueString.ToInt();
                floor.mShoreTopAnimX = columnCollection["shore_top_anim"].ValueString.ToInt();
                floor.mShoreLeftAnimX = columnCollection["shore_left_anim"].ValueString.ToInt();
                floor.mShoreRightAnimX = columnCollection["shore_right_anim"].ValueString.ToInt();
                floor.mShoreBottomAnimX = columnCollection["shore_bottom_anim"].ValueString.ToInt();
                floor.mShoreVerticalAnimX = columnCollection["shore_vert_anim"].ValueString.ToInt();
                floor.mShoreHorizontalAnimX = columnCollection["shore_horiz_anim"].ValueString.ToInt();
                floor.mShoreInnerTopLeftAnimX = columnCollection["shore_top_left_in_anim"].ValueString.ToInt();
                floor.mShoreInnerTopRightAnimX = columnCollection["shore_top_right_in_anim"].ValueString.ToInt();
                floor.mShoreInnerBottomLeftAnimX = columnCollection["shore_bottom_left_in_anim"].ValueString.ToInt();
                floor.mShoreInnerBottomRightAnimX = columnCollection["shore_bottom_right_in_anim"].ValueString.ToInt();
                floor.mShoreInnerTopAnimX = columnCollection["shore_top_in_anim"].ValueString.ToInt();
                floor.mShoreInnerLeftAnimX = columnCollection["shore_left_in_anim"].ValueString.ToInt();
                floor.mShoreInnerRightAnimX = columnCollection["shore_right_in_anim"].ValueString.ToInt();
                floor.mShoreInnerBottomAnimX = columnCollection["shore_bottom_in_anim"].ValueString.ToInt();
                floor.mShoreSurroundedAnimX = columnCollection["shore_isolated_anim"].ValueString.ToInt();

                floor.mWaterAnimSheet = columnCollection["water_anim_set"].ValueString.ToInt();
                floor.mShoreTopLeftAnimSheet = columnCollection["shore_top_left_anim_set"].ValueString.ToInt();
                floor.mShoreTopRightAnimSheet = columnCollection["shore_top_right_anim_set"].ValueString.ToInt();
                floor.mShoreBottomLeftAnimSheet = columnCollection["shore_bottom_left_anim_set"].ValueString.ToInt();
                floor.mShoreBottomRightAnimSheet = columnCollection["shore_bottom_right_anim_set"].ValueString.ToInt();
                floor.mShoreDiagonalForwardAnimSheet = columnCollection["shore_diag_forth_anim_set"].ValueString.ToInt();
                floor.mShoreDiagonalBackAnimSheet = columnCollection["shore_diag_back_anim_set"].ValueString.ToInt();
                floor.mShoreTopAnimSheet = columnCollection["shore_top_anim_set"].ValueString.ToInt();
                floor.mShoreLeftAnimSheet = columnCollection["shore_left_anim_set"].ValueString.ToInt();
                floor.mShoreRightAnimSheet = columnCollection["shore_right_anim_set"].ValueString.ToInt();
                floor.mShoreBottomAnimSheet = columnCollection["shore_bottom_anim_set"].ValueString.ToInt();
                floor.mShoreVerticalAnimSheet = columnCollection["shore_vert_anim_set"].ValueString.ToInt();
                floor.mShoreHorizontalAnimSheet = columnCollection["shore_horiz_anim_set"].ValueString.ToInt();
                floor.mShoreInnerTopLeftAnimSheet = columnCollection["shore_top_left_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerTopRightAnimSheet = columnCollection["shore_top_right_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerBottomLeftAnimSheet = columnCollection["shore_bottom_left_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerBottomRightAnimSheet = columnCollection["shore_bottom_right_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerTopAnimSheet = columnCollection["shore_top_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerLeftAnimSheet = columnCollection["shore_left_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerRightAnimSheet = columnCollection["shore_right_in_anim_set"].ValueString.ToInt();
                floor.mShoreInnerBottomAnimSheet = columnCollection["shore_bottom_in_anim_set"].ValueString.ToInt();
                floor.mShoreSurroundedAnimSheet = columnCollection["shore_isolated_anim_set"].ValueString.ToInt();

                floor.GroundTile.Type = (Enums.TileType)columnCollection["ground_type"].ValueString.ToInt();
                floor.GroundTile.Data1 = columnCollection["ground_data1"].ValueString.ToInt();
                floor.GroundTile.Data2 = columnCollection["ground_data2"].ValueString.ToInt();
                floor.GroundTile.Data3 = columnCollection["ground_data3"].ValueString.ToInt();
                floor.GroundTile.String1 = columnCollection["ground_string1"].ValueString;
                floor.GroundTile.String2 = columnCollection["ground_string2"].ValueString;
                floor.GroundTile.String3 = columnCollection["ground_string3"].ValueString;

                floor.HallTile.Type = (Enums.TileType)columnCollection["hall_type"].ValueString.ToInt();
                floor.HallTile.Data1 = columnCollection["hall_data1"].ValueString.ToInt();
                floor.HallTile.Data2 = columnCollection["hall_data2"].ValueString.ToInt();
                floor.HallTile.Data3 = columnCollection["hall_data3"].ValueString.ToInt();
                floor.HallTile.String1 = columnCollection["hall_string1"].ValueString;
                floor.HallTile.String2 = columnCollection["hall_string2"].ValueString;
                floor.HallTile.String3 = columnCollection["hall_string3"].ValueString;

                floor.WaterTile.Type = (Enums.TileType)columnCollection["water_type"].ValueString.ToInt();
                floor.WaterTile.Data1 = columnCollection["water_data1"].ValueString.ToInt();
                floor.WaterTile.Data2 = columnCollection["water_data2"].ValueString.ToInt();
                floor.WaterTile.Data3 = columnCollection["water_data3"].ValueString.ToInt();
                floor.WaterTile.String1 = columnCollection["water_string1"].ValueString;
                floor.WaterTile.String2 = columnCollection["water_string2"].ValueString;
                floor.WaterTile.String3 = columnCollection["water_string3"].ValueString;

                floor.WallTile.Type = (Enums.TileType)columnCollection["wall_type"].ValueString.ToInt();
                floor.WallTile.Data1 = columnCollection["wall_data1"].ValueString.ToInt();
                floor.WallTile.Data2 = columnCollection["wall_data2"].ValueString.ToInt();
                floor.WallTile.Data3 = columnCollection["wall_data3"].ValueString.ToInt();
                floor.WallTile.String1 = columnCollection["wall_string1"].ValueString;
                floor.WallTile.String2 = columnCollection["wall_string2"].ValueString;
                floor.WallTile.String3 = columnCollection["wall_string3"].ValueString;

                floor.GoalType = (Enums.RFloorGoalType)columnCollection["goal_type"].ValueString.ToInt();
                floor.GoalMap = columnCollection["goal_map"].ValueString.ToInt();
                floor.GoalX = columnCollection["goal_x"].ValueString.ToInt();
                floor.GoalY = columnCollection["goal_y"].ValueString.ToInt();
                floor.Darkness = columnCollection["darkness"].ValueString.ToInt();
                floor.Music = columnCollection["music"].ValueString;

                string query2 = "SELECT item_index, " +
                    "item_num, " +
                    "min_val, " +
                    "max_val, " +
                    "chance, " +
                    "sticky_chance, " +
                    "tag, " +
                    "hidden, " +
                    "on_ground, " +
                    "on_water, " +
                    "on_wall " +
                    "FROM rdungeon_item WHERE rdungeon_item.num = \'" + dungeonNum + "\' AND rdungeon_item.floor_num = \'" + floorNum + "\'";

                List<DataColumnCollection> columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {
                    RDungeonItem item = new RDungeonItem();
                    int itemInd = columnCollection2["item_index"].ValueString.ToInt();
                    item.ItemNum = columnCollection2["item_num"].ValueString.ToInt();
                    item.MinAmount = columnCollection2["min_val"].ValueString.ToInt();
                    item.MaxAmount = columnCollection2["max_val"].ValueString.ToInt();
                    item.AppearanceRate = columnCollection2["chance"].ValueString.ToInt();
                    item.StickyRate = columnCollection2["sticky_chance"].ValueString.ToInt();
                    item.Tag = columnCollection2["tag"].ValueString;
                    item.Hidden = columnCollection2["hidden"].ValueString.ToBool();
                    item.OnGround = columnCollection2["on_ground"].ValueString.ToBool();
                    item.OnWater = columnCollection2["on_water"].ValueString.ToBool();
                    item.OnWall = columnCollection2["on_wall"].ValueString.ToBool();

                    floor.Items.Add(item);
                }

                query2 = "SELECT npc_index, " +
                    "npc_num, " +
                    "min_level, " +
                    "max_level, " +
                    "chance, " +
                    "status, " +
                    "status_counter, " +
                    "status_chance " +
                    "FROM rdungeon_npc WHERE rdungeon_npc.num = \'" + dungeonNum + "\' AND rdungeon_npc.floor_num = \'" + floorNum + "\'";

                columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {
                    MapNpcPreset npc = new MapNpcPreset();
                    int npcInd = columnCollection2["npc_index"].ValueString.ToInt();
                    npc.NpcNum = columnCollection2["npc_num"].ValueString.ToInt();
                    npc.MinLevel = columnCollection2["min_level"].ValueString.ToInt();
                    npc.MaxLevel = columnCollection2["max_level"].ValueString.ToInt();
                    npc.AppearanceRate = columnCollection2["chance"].ValueString.ToInt();
                    npc.StartStatus = (Enums.StatusAilment)columnCollection2["status"].ValueString.ToInt();
                    npc.StartStatusCounter = columnCollection2["status_counter"].ValueString.ToInt();
                    npc.StartStatusChance = columnCollection2["status_chance"].ValueString.ToInt();

                    floor.Npcs.Add(npc);
                }

                query2 = "SELECT tile_index, " +
                    "tile_type, " +
                    "data1, " +
                    "data2, " +
                    "data3, " +
                    "string1, " +
                    "string2, " +
                    "string3, " +
                    "ground, " +
                    "mask1, " +
                    "mask2, " +
                    "fringe1, " +
                    "fringe2, " +
                    "ground_set, " +
                    "mask1_set, " +
                    "mask2_set, " +
                    "fringe1_set, " +
                    "fringe2_set, " +
                    "ground_anim, " +
                    "mask1_anim, " +
                    "mask2_anim, " +
                    "fringe1_anim, " +
                    "fringe2_anim, " +
                    "ground_anim_set, " +
                    "mask1_anim_set, " +
                    "mask2_anim_set, " +
                    "fringe1_anim_set, " +
                    "fringe2_anim_set, " +
                    "rdungeon_value, " +
                    "chance " +
                    "FROM rdungeon_tile WHERE rdungeon_tile.num = \'" + dungeonNum + "\' AND rdungeon_tile.floor_num = \'" + floorNum + "\'";

                columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {
                    RDungeonTrap tile = new RDungeonTrap();
                    int tileInd = columnCollection2["tile_index"].ValueString.ToInt();
                    tile.Type = (Enums.TileType)columnCollection2["tile_type"].ValueString.ToInt();
                    tile.Data1 = columnCollection2["data1"].ValueString.ToInt();
                    tile.Data2 = columnCollection2["data2"].ValueString.ToInt();
                    tile.Data3 = columnCollection2["data3"].ValueString.ToInt();
                    tile.String1 = columnCollection2["string1"].ValueString;
                    tile.String2 = columnCollection2["string2"].ValueString;
                    tile.String3 = columnCollection2["string3"].ValueString;
                    tile.Ground = columnCollection2["ground"].ValueString.ToInt();
                    tile.Mask = columnCollection2["mask1"].ValueString.ToInt();
                    tile.Mask2 = columnCollection2["mask2"].ValueString.ToInt();
                    tile.Fringe = columnCollection2["fringe1"].ValueString.ToInt();
                    tile.Fringe2 = columnCollection2["fringe2"].ValueString.ToInt();
                    tile.GroundSet = columnCollection2["ground_set"].ValueString.ToInt();
                    tile.MaskSet = columnCollection2["mask1_set"].ValueString.ToInt();
                    tile.Mask2Set = columnCollection2["mask2_set"].ValueString.ToInt();
                    tile.FringeSet = columnCollection2["fringe1_set"].ValueString.ToInt();
                    tile.Fringe2Set = columnCollection2["fringe2_set"].ValueString.ToInt();
                    tile.GroundAnim = columnCollection2["ground_anim"].ValueString.ToInt();
                    tile.Anim = columnCollection2["mask1_anim"].ValueString.ToInt();
                    tile.M2Anim = columnCollection2["mask2_anim"].ValueString.ToInt();
                    tile.FAnim = columnCollection2["fringe1_anim"].ValueString.ToInt();
                    tile.F2Anim = columnCollection2["fringe2_anim"].ValueString.ToInt();
                    tile.GroundAnimSet = columnCollection2["ground_anim_set"].ValueString.ToInt();
                    tile.AnimSet = columnCollection2["mask1_anim_set"].ValueString.ToInt();
                    tile.M2AnimSet = columnCollection2["mask2_anim_set"].ValueString.ToInt();
                    tile.FAnimSet = columnCollection2["fringe1_anim_set"].ValueString.ToInt();
                    tile.F2AnimSet = columnCollection2["fringe2_anim_set"].ValueString.ToInt();
                    tile.RDungeonMapValue = columnCollection2["rdungeon_value"].ValueString.ToInt();
                    tile.AppearanceRate = columnCollection2["chance"].ValueString.ToInt();

                    floor.SpecialTiles.Add(tile);
                }

                query2 = "SELECT weather_index, " +
                    "weather_type " +
                    "FROM rdungeon_weather WHERE rdungeon_weather.num = \'" + dungeonNum + "\' AND rdungeon_weather.floor_num = \'" + floorNum + "\'";

                columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {

                    int weatherInd = columnCollection2["weather_index"].ValueString.ToInt();
                    Enums.Weather weather = (Enums.Weather)columnCollection2["weather_type"].ValueString.ToInt();

                    floor.Weather.Add(weather);
                }

                query2 = "SELECT chamber_index, " +
                    "chamber_num, " +
                    "string1, " +
                    "string2, " +
                    "string3 " +
                    "FROM rdungeon_chamber WHERE rdungeon_chamber.num = \'" + dungeonNum + "\' AND rdungeon_chamber.floor_num = \'" + floorNum + "\'";

                columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {
                    RDungeonPresetChamber chamber = new RDungeonPresetChamber();
                    int chamberInd = columnCollection2["chamber_index"].ValueString.ToInt();
                    chamber.ChamberNum = columnCollection2["chamber_num"].ValueString.ToInt();
                    chamber.String1 = columnCollection2["string1"].ValueString;
                    chamber.String2 = columnCollection2["string2"].ValueString;
                    chamber.String3 = columnCollection2["string3"].ValueString;

                    floor.Options.Chambers.Add(chamber);
                }

                dungeon.Floors.Add(floor);
            }

            rdungeons.RDungeons.Add(dungeonNum, dungeon);
        }

        #region Saving

        public static int AddRDungeon() {
            int index = rdungeons.Count;
            SaveRDungeon(index);
            return index;
        }

        public static void SaveRDungeon(int dungeonNum)
        {
            if (rdungeons.RDungeons.ContainsKey(dungeonNum) == false)
                rdungeons.RDungeons.Add(dungeonNum, new RDungeon(dungeonNum));
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                database.BeginTransaction();

                database.ExecuteNonQuery("DELETE FROM rdungeon WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_floor WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_item WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_npc WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_tile WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_weather WHERE num = \'" + dungeonNum + "\'");
                database.ExecuteNonQuery("DELETE FROM rdungeon_chamber WHERE num = \'" + dungeonNum + "\'");

                database.UpdateOrInsert("rdungeon", new IDataColumn[] {
                database.CreateColumn(false, "num", dungeonNum.ToString()),
                database.CreateColumn(false, "name", rdungeons[dungeonNum].DungeonName),
                database.CreateColumn(false, "up", (rdungeons[dungeonNum].Direction == Enums.Direction.Up).ToIntString()),
                database.CreateColumn(false, "allow_recruit", rdungeons[dungeonNum].Recruitment.ToIntString()),
                database.CreateColumn(false, "allow_exp", rdungeons[dungeonNum].Exp.ToIntString()),
                database.CreateColumn(false, "time_limit", rdungeons[dungeonNum].WindTimer.ToString()),
            });

                for (int i = 0; i < rdungeons[dungeonNum].Floors.Count; i++)
                {
                    database.UpdateOrInsert("rdungeon_floor", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),

                    database.CreateColumn(false, "trap_min", rdungeons[dungeonNum].Floors[i].Options.TrapMin.ToString()),
                    database.CreateColumn(false, "trap_max", rdungeons[dungeonNum].Floors[i].Options.TrapMax.ToString()),
                    database.CreateColumn(false, "item_min", rdungeons[dungeonNum].Floors[i].Options.ItemMin.ToString()),
                    database.CreateColumn(false, "item_max", rdungeons[dungeonNum].Floors[i].Options.ItemMax.ToString()),
                    database.CreateColumn(false, "intricacy", rdungeons[dungeonNum].Floors[i].Options.Intricacy.ToString()),
                    database.CreateColumn(false, "room_width_min", rdungeons[dungeonNum].Floors[i].Options.RoomWidthMin.ToString()),
                    database.CreateColumn(false, "room_width_max", rdungeons[dungeonNum].Floors[i].Options.RoomWidthMax.ToString()),
                    database.CreateColumn(false, "room_length_min", rdungeons[dungeonNum].Floors[i].Options.RoomLengthMin.ToString()),
                    database.CreateColumn(false, "room_length_max", rdungeons[dungeonNum].Floors[i].Options.RoomLengthMax.ToString()),
                    database.CreateColumn(false, "hall_turn_min", rdungeons[dungeonNum].Floors[i].Options.HallTurnMin.ToString()),
                    database.CreateColumn(false, "hall_turn_max", rdungeons[dungeonNum].Floors[i].Options.HallTurnMax.ToString()),
                    database.CreateColumn(false, "hall_var_min", rdungeons[dungeonNum].Floors[i].Options.HallVarMin.ToString()),
                    database.CreateColumn(false, "hall_var_max", rdungeons[dungeonNum].Floors[i].Options.HallVarMax.ToString()),
                    database.CreateColumn(false, "water_chance", rdungeons[dungeonNum].Floors[i].Options.WaterFrequency.ToString()),
                    database.CreateColumn(false, "craters", rdungeons[dungeonNum].Floors[i].Options.Craters.ToString()),
                    database.CreateColumn(false, "crater_min", rdungeons[dungeonNum].Floors[i].Options.CraterMinLength.ToString()),
                    database.CreateColumn(false, "crater_max", rdungeons[dungeonNum].Floors[i].Options.CraterMaxLength.ToString()),
                    database.CreateColumn(false, "crater_fuzz", rdungeons[dungeonNum].Floors[i].Options.CraterFuzzy.ToIntString()),
                    
                    database.CreateColumn(false, "npc_respawn", rdungeons[dungeonNum].Floors[i].NpcSpawnTime.ToString()),
                    database.CreateColumn(false, "npc_min", rdungeons[dungeonNum].Floors[i].NpcMin.ToString()),
                    database.CreateColumn(false, "npc_max", rdungeons[dungeonNum].Floors[i].NpcMax.ToString()),

                    database.CreateColumn(false, "stairs", rdungeons[dungeonNum].Floors[i].StairsX.ToString()),
                    database.CreateColumn(false, "ground", rdungeons[dungeonNum].Floors[i].mGroundX.ToString()),
                    database.CreateColumn(false, "ground_alt1", rdungeons[dungeonNum].Floors[i].mGroundAltX.ToString()),
                    database.CreateColumn(false, "ground_alt2", rdungeons[dungeonNum].Floors[i].mGroundAlt2X.ToString()),
                    database.CreateColumn(false, "top_left", rdungeons[dungeonNum].Floors[i].mTopLeftX.ToString()),
                    database.CreateColumn(false, "top_center", rdungeons[dungeonNum].Floors[i].mTopCenterX.ToString()),
                    database.CreateColumn(false, "top_right", rdungeons[dungeonNum].Floors[i].mTopRightX.ToString()),
                    database.CreateColumn(false, "center_left", rdungeons[dungeonNum].Floors[i].mCenterLeftX.ToString()),
                    database.CreateColumn(false, "center_center", rdungeons[dungeonNum].Floors[i].mCenterCenterX.ToString()),
                    database.CreateColumn(false, "center_right", rdungeons[dungeonNum].Floors[i].mCenterRightX.ToString()),
                    database.CreateColumn(false, "bottom_left", rdungeons[dungeonNum].Floors[i].mBottomLeftX.ToString()),
                    database.CreateColumn(false, "bottom_center", rdungeons[dungeonNum].Floors[i].mBottomCenterX.ToString()),
                    database.CreateColumn(false, "bottom_right", rdungeons[dungeonNum].Floors[i].mBottomRightX.ToString()),
                    database.CreateColumn(false, "top_left_in", rdungeons[dungeonNum].Floors[i].mInnerTopLeftX.ToString()),
                    database.CreateColumn(false, "top_right_in", rdungeons[dungeonNum].Floors[i].mInnerTopRightX.ToString()),
                    database.CreateColumn(false, "bottom_left_in", rdungeons[dungeonNum].Floors[i].mInnerBottomLeftX.ToString()),
                    database.CreateColumn(false, "bottom_right_in", rdungeons[dungeonNum].Floors[i].mInnerBottomRightX.ToString()),
                    database.CreateColumn(false, "col_top", rdungeons[dungeonNum].Floors[i].mColumnTopX.ToString()),
                    database.CreateColumn(false, "col_center", rdungeons[dungeonNum].Floors[i].mColumnCenterX.ToString()),
                    database.CreateColumn(false, "col_bottom", rdungeons[dungeonNum].Floors[i].mColumnBottomX.ToString()),
                    database.CreateColumn(false, "row_left", rdungeons[dungeonNum].Floors[i].mRowLeftX.ToString()),
                    database.CreateColumn(false, "row_center", rdungeons[dungeonNum].Floors[i].mRowCenterX.ToString()),
                    database.CreateColumn(false, "row_right", rdungeons[dungeonNum].Floors[i].mRowRightX.ToString()),
                    database.CreateColumn(false, "isolated", rdungeons[dungeonNum].Floors[i].mIsolatedWallX.ToString()),

                    database.CreateColumn(false, "stairs_set", rdungeons[dungeonNum].Floors[i].StairsSheet.ToString()),
                    database.CreateColumn(false, "ground_set", rdungeons[dungeonNum].Floors[i].mGroundSheet.ToString()),
                    database.CreateColumn(false, "ground_alt1_set", rdungeons[dungeonNum].Floors[i].mGroundAltSheet.ToString()),
                    database.CreateColumn(false, "ground_alt2_set", rdungeons[dungeonNum].Floors[i].mGroundAlt2Sheet.ToString()),
                    database.CreateColumn(false, "top_left_set", rdungeons[dungeonNum].Floors[i].mTopLeftSheet.ToString()),
                    database.CreateColumn(false, "top_center_set", rdungeons[dungeonNum].Floors[i].mTopCenterSheet.ToString()),
                    database.CreateColumn(false, "top_right_set", rdungeons[dungeonNum].Floors[i].mTopRightSheet.ToString()),
                    database.CreateColumn(false, "center_left_set", rdungeons[dungeonNum].Floors[i].mCenterLeftSheet.ToString()),
                    database.CreateColumn(false, "center_center_set", rdungeons[dungeonNum].Floors[i].mCenterCenterSheet.ToString()),
                    database.CreateColumn(false, "center_right_set", rdungeons[dungeonNum].Floors[i].mCenterRightSheet.ToString()),
                    database.CreateColumn(false, "bottom_left_set", rdungeons[dungeonNum].Floors[i].mBottomLeftSheet.ToString()),
                    database.CreateColumn(false, "bottom_center_set", rdungeons[dungeonNum].Floors[i].mBottomCenterSheet.ToString()),
                    database.CreateColumn(false, "bottom_right_set", rdungeons[dungeonNum].Floors[i].mBottomRightSheet.ToString()),
                    database.CreateColumn(false, "top_left_in_set", rdungeons[dungeonNum].Floors[i].mInnerTopLeftSheet.ToString()),
                    database.CreateColumn(false, "top_right_in_set", rdungeons[dungeonNum].Floors[i].mInnerTopRightSheet.ToString()),
                    database.CreateColumn(false, "bottom_left_in_set", rdungeons[dungeonNum].Floors[i].mInnerBottomLeftSheet.ToString()),
                    database.CreateColumn(false, "bottom_right_in_set", rdungeons[dungeonNum].Floors[i].mInnerBottomRightSheet.ToString()),
                    database.CreateColumn(false, "col_top_set", rdungeons[dungeonNum].Floors[i].mColumnTopSheet.ToString()),
                    database.CreateColumn(false, "col_center_set", rdungeons[dungeonNum].Floors[i].mColumnCenterSheet.ToString()),
                    database.CreateColumn(false, "col_bottom_set", rdungeons[dungeonNum].Floors[i].mColumnBottomSheet.ToString()),
                    database.CreateColumn(false, "row_left_set", rdungeons[dungeonNum].Floors[i].mRowLeftSheet.ToString()),
                    database.CreateColumn(false, "row_center_set", rdungeons[dungeonNum].Floors[i].mRowCenterSheet.ToString()),
                    database.CreateColumn(false, "row_right_set", rdungeons[dungeonNum].Floors[i].mRowRightSheet.ToString()),
                    database.CreateColumn(false, "isolated_set", rdungeons[dungeonNum].Floors[i].mIsolatedWallSheet.ToString()),

                    database.CreateColumn(false, "top_left_alt", rdungeons[dungeonNum].Floors[i].mTopLeftAltX.ToString()),
                    database.CreateColumn(false, "top_center_alt", rdungeons[dungeonNum].Floors[i].mTopCenterAltX.ToString()),
                    database.CreateColumn(false, "top_right_alt", rdungeons[dungeonNum].Floors[i].mTopRightAltX.ToString()),
                    database.CreateColumn(false, "center_left_alt", rdungeons[dungeonNum].Floors[i].mCenterLeftAltX.ToString()),
                    database.CreateColumn(false, "center_center_alt1", rdungeons[dungeonNum].Floors[i].mCenterCenterAltX.ToString()),
                    database.CreateColumn(false, "center_center_alt2", rdungeons[dungeonNum].Floors[i].mCenterCenterAlt2X.ToString()),
                    database.CreateColumn(false, "center_right_alt", rdungeons[dungeonNum].Floors[i].mCenterRightAltX.ToString()),
                    database.CreateColumn(false, "bottom_left_alt", rdungeons[dungeonNum].Floors[i].mBottomLeftAltX.ToString()),
                    database.CreateColumn(false, "bottom_center_alt", rdungeons[dungeonNum].Floors[i].mBottomCenterAltX.ToString()),
                    database.CreateColumn(false, "bottom_right_alt", rdungeons[dungeonNum].Floors[i].mBottomRightAltX.ToString()),
                    database.CreateColumn(false, "top_left_in_alt", rdungeons[dungeonNum].Floors[i].mInnerTopLeftAltX.ToString()),
                    database.CreateColumn(false, "top_right_in_alt", rdungeons[dungeonNum].Floors[i].mInnerTopRightAltX.ToString()),
                    database.CreateColumn(false, "bottom_left_in_alt", rdungeons[dungeonNum].Floors[i].mInnerBottomLeftAltX.ToString()),
                    database.CreateColumn(false, "bottom_right_in_alt", rdungeons[dungeonNum].Floors[i].mInnerBottomRightAltX.ToString()),
                    database.CreateColumn(false, "col_top_alt", rdungeons[dungeonNum].Floors[i].mColumnTopAltX.ToString()),
                    database.CreateColumn(false, "col_center_alt", rdungeons[dungeonNum].Floors[i].mColumnCenterAltX.ToString()),
                    database.CreateColumn(false, "col_bottom_alt", rdungeons[dungeonNum].Floors[i].mColumnBottomAltX.ToString()),
                    database.CreateColumn(false, "row_left_alt", rdungeons[dungeonNum].Floors[i].mRowLeftAltX.ToString()),
                    database.CreateColumn(false, "row_center_alt", rdungeons[dungeonNum].Floors[i].mRowCenterAltX.ToString()),
                    database.CreateColumn(false, "row_right_alt", rdungeons[dungeonNum].Floors[i].mRowRightAltX.ToString()),
                    database.CreateColumn(false, "isolated_alt", rdungeons[dungeonNum].Floors[i].mIsolatedWallAltX.ToString()),

                    database.CreateColumn(false, "top_left_alt_set", rdungeons[dungeonNum].Floors[i].mTopLeftAltSheet.ToString()),
                    database.CreateColumn(false, "top_center_alt_set", rdungeons[dungeonNum].Floors[i].mTopCenterAltSheet.ToString()),
                    database.CreateColumn(false, "top_right_alt_set", rdungeons[dungeonNum].Floors[i].mTopRightAltSheet.ToString()),
                    database.CreateColumn(false, "center_left_alt_set", rdungeons[dungeonNum].Floors[i].mCenterLeftAltSheet.ToString()),
                    database.CreateColumn(false, "center_center_alt1_set", rdungeons[dungeonNum].Floors[i].mCenterCenterAltSheet.ToString()),
                    database.CreateColumn(false, "center_center_alt2_set", rdungeons[dungeonNum].Floors[i].mCenterCenterAlt2Sheet.ToString()),
                    database.CreateColumn(false, "center_right_alt_set", rdungeons[dungeonNum].Floors[i].mCenterRightAltSheet.ToString()),
                    database.CreateColumn(false, "bottom_left_alt_set", rdungeons[dungeonNum].Floors[i].mBottomLeftAltSheet.ToString()),
                    database.CreateColumn(false, "bottom_center_alt_set", rdungeons[dungeonNum].Floors[i].mBottomCenterAltSheet.ToString()),
                    database.CreateColumn(false, "bottom_right_alt_set", rdungeons[dungeonNum].Floors[i].mBottomRightAltSheet.ToString()),
                    database.CreateColumn(false, "top_left_in_alt_set", rdungeons[dungeonNum].Floors[i].mInnerTopLeftAltSheet.ToString()),
                    database.CreateColumn(false, "top_right_in_alt_set", rdungeons[dungeonNum].Floors[i].mInnerTopRightAltSheet.ToString()),
                    database.CreateColumn(false, "bottom_left_in_alt_set", rdungeons[dungeonNum].Floors[i].mInnerBottomLeftAltSheet.ToString()),
                    database.CreateColumn(false, "bottom_right_in_alt_set", rdungeons[dungeonNum].Floors[i].mInnerBottomRightAltSheet.ToString()),
                    database.CreateColumn(false, "col_top_alt_set", rdungeons[dungeonNum].Floors[i].mColumnTopAltSheet.ToString()),
                    database.CreateColumn(false, "col_center_alt_set", rdungeons[dungeonNum].Floors[i].mColumnCenterAltSheet.ToString()),
                    database.CreateColumn(false, "col_bottom_alt_set", rdungeons[dungeonNum].Floors[i].mColumnBottomAltSheet.ToString()),
                    database.CreateColumn(false, "row_left_alt_set", rdungeons[dungeonNum].Floors[i].mRowLeftAltSheet.ToString()),
                    database.CreateColumn(false, "row_center_alt_set", rdungeons[dungeonNum].Floors[i].mRowCenterAltSheet.ToString()),
                    database.CreateColumn(false, "row_right_alt_set", rdungeons[dungeonNum].Floors[i].mRowRightAltSheet.ToString()),
                    database.CreateColumn(false, "isolated_alt_set", rdungeons[dungeonNum].Floors[i].mIsolatedWallAltSheet.ToString()),

                    database.CreateColumn(false, "water", rdungeons[dungeonNum].Floors[i].mWaterX.ToString()),
                    database.CreateColumn(false, "shore_top_left", rdungeons[dungeonNum].Floors[i].mShoreTopLeftX.ToString()),
                    database.CreateColumn(false, "shore_top_right", rdungeons[dungeonNum].Floors[i].mShoreTopRightX.ToString()),
                    database.CreateColumn(false, "shore_bottom_left", rdungeons[dungeonNum].Floors[i].mShoreBottomLeftX.ToString()),
                    database.CreateColumn(false, "shore_bottom_right", rdungeons[dungeonNum].Floors[i].mShoreBottomRightX.ToString()),
                    database.CreateColumn(false, "shore_diag_forth", rdungeons[dungeonNum].Floors[i].mShoreDiagonalForwardX.ToString()),
                    database.CreateColumn(false, "shore_diag_back", rdungeons[dungeonNum].Floors[i].mShoreDiagonalBackX.ToString()),
                    database.CreateColumn(false, "shore_top", rdungeons[dungeonNum].Floors[i].mShoreTopX.ToString()),
                    database.CreateColumn(false, "shore_left", rdungeons[dungeonNum].Floors[i].mShoreLeftX.ToString()),
                    database.CreateColumn(false, "shore_right", rdungeons[dungeonNum].Floors[i].mShoreRightX.ToString()),
                    database.CreateColumn(false, "shore_bottom", rdungeons[dungeonNum].Floors[i].mShoreBottomX.ToString()),
                    database.CreateColumn(false, "shore_vert", rdungeons[dungeonNum].Floors[i].mShoreVerticalX.ToString()),
                    database.CreateColumn(false, "shore_horiz", rdungeons[dungeonNum].Floors[i].mShoreHorizontalX.ToString()),
                    database.CreateColumn(false, "shore_top_left_in", rdungeons[dungeonNum].Floors[i].mShoreInnerTopLeftX.ToString()),
                    database.CreateColumn(false, "shore_top_right_in", rdungeons[dungeonNum].Floors[i].mShoreInnerTopRightX.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_in", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomLeftX.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_in", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomRightX.ToString()),
                    database.CreateColumn(false, "shore_top_in", rdungeons[dungeonNum].Floors[i].mShoreInnerTopX.ToString()),
                    database.CreateColumn(false, "shore_left_in", rdungeons[dungeonNum].Floors[i].mShoreInnerLeftX.ToString()),
                    database.CreateColumn(false, "shore_right_in", rdungeons[dungeonNum].Floors[i].mShoreInnerRightX.ToString()),
                    database.CreateColumn(false, "shore_bottom_in", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomX.ToString()),
                    database.CreateColumn(false, "shore_isolated", rdungeons[dungeonNum].Floors[i].mShoreSurroundedX.ToString()),

                    database.CreateColumn(false, "water_set", rdungeons[dungeonNum].Floors[i].mWaterSheet.ToString()),
                    database.CreateColumn(false, "shore_top_left_set", rdungeons[dungeonNum].Floors[i].mShoreTopLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_top_right_set", rdungeons[dungeonNum].Floors[i].mShoreTopRightSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_set", rdungeons[dungeonNum].Floors[i].mShoreBottomLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_set", rdungeons[dungeonNum].Floors[i].mShoreBottomRightSheet.ToString()),
                    database.CreateColumn(false, "shore_diag_forth_set", rdungeons[dungeonNum].Floors[i].mShoreDiagonalForwardSheet.ToString()),
                    database.CreateColumn(false, "shore_diag_back_set", rdungeons[dungeonNum].Floors[i].mShoreDiagonalBackSheet.ToString()),
                    database.CreateColumn(false, "shore_top_set", rdungeons[dungeonNum].Floors[i].mShoreTopSheet.ToString()),
                    database.CreateColumn(false, "shore_left_set", rdungeons[dungeonNum].Floors[i].mShoreLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_right_set", rdungeons[dungeonNum].Floors[i].mShoreRightSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_set", rdungeons[dungeonNum].Floors[i].mShoreBottomSheet.ToString()),
                    database.CreateColumn(false, "shore_vert_set", rdungeons[dungeonNum].Floors[i].mShoreVerticalSheet.ToString()),
                    database.CreateColumn(false, "shore_horiz_set", rdungeons[dungeonNum].Floors[i].mShoreHorizontalSheet.ToString()),
                    database.CreateColumn(false, "shore_top_left_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_top_right_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopRightSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomRightSheet.ToString()),
                    database.CreateColumn(false, "shore_top_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopSheet.ToString()),
                    database.CreateColumn(false, "shore_left_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerLeftSheet.ToString()),
                    database.CreateColumn(false, "shore_right_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerRightSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_in_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomSheet.ToString()),
                    database.CreateColumn(false, "shore_isolated_set", rdungeons[dungeonNum].Floors[i].mShoreSurroundedSheet.ToString()),

                    database.CreateColumn(false, "water_anim", rdungeons[dungeonNum].Floors[i].mWaterAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_left_anim", rdungeons[dungeonNum].Floors[i].mShoreTopLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_right_anim", rdungeons[dungeonNum].Floors[i].mShoreTopRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_anim", rdungeons[dungeonNum].Floors[i].mShoreBottomLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_anim", rdungeons[dungeonNum].Floors[i].mShoreBottomRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_diag_forth_anim", rdungeons[dungeonNum].Floors[i].mShoreDiagonalForwardAnimX.ToString()),
                    database.CreateColumn(false, "shore_diag_back_anim", rdungeons[dungeonNum].Floors[i].mShoreDiagonalBackAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_anim", rdungeons[dungeonNum].Floors[i].mShoreTopAnimX.ToString()),
                    database.CreateColumn(false, "shore_left_anim", rdungeons[dungeonNum].Floors[i].mShoreLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_right_anim", rdungeons[dungeonNum].Floors[i].mShoreRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_anim", rdungeons[dungeonNum].Floors[i].mShoreBottomAnimX.ToString()),
                    database.CreateColumn(false, "shore_vert_anim", rdungeons[dungeonNum].Floors[i].mShoreVerticalAnimX.ToString()),
                    database.CreateColumn(false, "shore_horiz_anim", rdungeons[dungeonNum].Floors[i].mShoreHorizontalAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_left_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerTopLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_right_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerTopRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_top_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerTopAnimX.ToString()),
                    database.CreateColumn(false, "shore_left_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerLeftAnimX.ToString()),
                    database.CreateColumn(false, "shore_right_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerRightAnimX.ToString()),
                    database.CreateColumn(false, "shore_bottom_in_anim", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomAnimX.ToString()),
                    database.CreateColumn(false, "shore_isolated_anim", rdungeons[dungeonNum].Floors[i].mShoreSurroundedAnimX.ToString()),

                    database.CreateColumn(false, "water_anim_set", rdungeons[dungeonNum].Floors[i].mWaterAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_left_anim_set", rdungeons[dungeonNum].Floors[i].mShoreTopLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_right_anim_set", rdungeons[dungeonNum].Floors[i].mShoreTopRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_anim_set", rdungeons[dungeonNum].Floors[i].mShoreBottomLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_anim_set", rdungeons[dungeonNum].Floors[i].mShoreBottomRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_diag_forth_anim_set", rdungeons[dungeonNum].Floors[i].mShoreDiagonalForwardAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_diag_back_anim_set", rdungeons[dungeonNum].Floors[i].mShoreDiagonalBackAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_anim_set", rdungeons[dungeonNum].Floors[i].mShoreTopAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_left_anim_set", rdungeons[dungeonNum].Floors[i].mShoreLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_right_anim_set", rdungeons[dungeonNum].Floors[i].mShoreRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_anim_set", rdungeons[dungeonNum].Floors[i].mShoreBottomAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_vert_anim_set", rdungeons[dungeonNum].Floors[i].mShoreVerticalAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_horiz_anim_set", rdungeons[dungeonNum].Floors[i].mShoreHorizontalAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_left_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_right_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_left_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_right_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_top_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerTopAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_left_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerLeftAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_right_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerRightAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_bottom_in_anim_set", rdungeons[dungeonNum].Floors[i].mShoreInnerBottomAnimSheet.ToString()),
                    database.CreateColumn(false, "shore_isolated_anim_set", rdungeons[dungeonNum].Floors[i].mShoreSurroundedAnimSheet.ToString()),

                    database.CreateColumn(false, "ground_type", ((int)rdungeons[dungeonNum].Floors[i].GroundTile.Type).ToString()),
                    database.CreateColumn(false, "ground_data1", ((int)rdungeons[dungeonNum].Floors[i].GroundTile.Data1).ToString()),
                    database.CreateColumn(false, "ground_data2", ((int)rdungeons[dungeonNum].Floors[i].GroundTile.Data2).ToString()),
                    database.CreateColumn(false, "ground_data3", ((int)rdungeons[dungeonNum].Floors[i].GroundTile.Data3).ToString()),
                    database.CreateColumn(false, "ground_string1", rdungeons[dungeonNum].Floors[i].GroundTile.String1),
                    database.CreateColumn(false, "ground_string2", rdungeons[dungeonNum].Floors[i].GroundTile.String2),
                    database.CreateColumn(false, "ground_string3", rdungeons[dungeonNum].Floors[i].GroundTile.String3),
                    database.CreateColumn(false, "hall_type", ((int)rdungeons[dungeonNum].Floors[i].HallTile.Type).ToString()),
                    database.CreateColumn(false, "hall_data1", ((int)rdungeons[dungeonNum].Floors[i].HallTile.Data1).ToString()),
                    database.CreateColumn(false, "hall_data2", ((int)rdungeons[dungeonNum].Floors[i].HallTile.Data2).ToString()),
                    database.CreateColumn(false, "hall_data3", ((int)rdungeons[dungeonNum].Floors[i].HallTile.Data3).ToString()),
                    database.CreateColumn(false, "hall_string1", rdungeons[dungeonNum].Floors[i].HallTile.String1),
                    database.CreateColumn(false, "hall_string2", rdungeons[dungeonNum].Floors[i].HallTile.String2),
                    database.CreateColumn(false, "hall_string3", rdungeons[dungeonNum].Floors[i].HallTile.String3),
                    database.CreateColumn(false, "water_type", ((int)rdungeons[dungeonNum].Floors[i].WaterTile.Type).ToString()),
                    database.CreateColumn(false, "water_data1", ((int)rdungeons[dungeonNum].Floors[i].WaterTile.Data1).ToString()),
                    database.CreateColumn(false, "water_data2", ((int)rdungeons[dungeonNum].Floors[i].WaterTile.Data2).ToString()),
                    database.CreateColumn(false, "water_data3", ((int)rdungeons[dungeonNum].Floors[i].WaterTile.Data3).ToString()),
                    database.CreateColumn(false, "water_string1", rdungeons[dungeonNum].Floors[i].WaterTile.String1),
                    database.CreateColumn(false, "water_string2", rdungeons[dungeonNum].Floors[i].WaterTile.String2),
                    database.CreateColumn(false, "water_string3", rdungeons[dungeonNum].Floors[i].WaterTile.String3),
                    database.CreateColumn(false, "wall_type", ((int)rdungeons[dungeonNum].Floors[i].WallTile.Type).ToString()),
                    database.CreateColumn(false, "wall_data1", ((int)rdungeons[dungeonNum].Floors[i].WallTile.Data1).ToString()),
                    database.CreateColumn(false, "wall_data2", ((int)rdungeons[dungeonNum].Floors[i].WallTile.Data2).ToString()),
                    database.CreateColumn(false, "wall_data3", ((int)rdungeons[dungeonNum].Floors[i].WallTile.Data3).ToString()),
                    database.CreateColumn(false, "wall_string1", rdungeons[dungeonNum].Floors[i].WallTile.String1),
                    database.CreateColumn(false, "wall_string2", rdungeons[dungeonNum].Floors[i].WallTile.String2),
                    database.CreateColumn(false, "wall_string3", rdungeons[dungeonNum].Floors[i].WallTile.String3),
                    database.CreateColumn(false, "goal_type", ((int)rdungeons[dungeonNum].Floors[i].GoalType).ToString()),
                    database.CreateColumn(false, "goal_map", rdungeons[dungeonNum].Floors[i].GoalMap.ToString()),
                    database.CreateColumn(false, "goal_x", rdungeons[dungeonNum].Floors[i].GoalX.ToString()),
                    database.CreateColumn(false, "goal_y", rdungeons[dungeonNum].Floors[i].GoalY.ToString()),
                    database.CreateColumn(false, "darkness", rdungeons[dungeonNum].Floors[i].Darkness.ToString()),
                    database.CreateColumn(false, "music", rdungeons[dungeonNum].Floors[i].Music)
                });

                    for (int j = 0; j < rdungeons[dungeonNum].Floors[i].Items.Count; j++)
                    {
                        database.UpdateOrInsert("rdungeon_item", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),
                    database.CreateColumn(false, "item_index", j.ToString()),
                    database.CreateColumn(false, "item_num", rdungeons[dungeonNum].Floors[i].Items[j].ItemNum.ToString()),
                    database.CreateColumn(false, "min_val", rdungeons[dungeonNum].Floors[i].Items[j].MinAmount.ToString()),
                    database.CreateColumn(false, "max_val", rdungeons[dungeonNum].Floors[i].Items[j].MaxAmount.ToString()),
                    database.CreateColumn(false, "chance", rdungeons[dungeonNum].Floors[i].Items[j].AppearanceRate.ToString()),
                    database.CreateColumn(false, "sticky_chance", rdungeons[dungeonNum].Floors[i].Items[j].StickyRate.ToString()),
                    database.CreateColumn(false, "tag", rdungeons[dungeonNum].Floors[i].Items[j].Tag),
                    database.CreateColumn(false, "hidden", rdungeons[dungeonNum].Floors[i].Items[j].Hidden.ToIntString()),
                    database.CreateColumn(false, "on_ground", rdungeons[dungeonNum].Floors[i].Items[j].OnGround.ToIntString()),
                    database.CreateColumn(false, "on_water", rdungeons[dungeonNum].Floors[i].Items[j].OnWater.ToIntString()),
                    database.CreateColumn(false, "on_wall", rdungeons[dungeonNum].Floors[i].Items[j].OnWall.ToIntString()),
                    });
                    }

                    for (int j = 0; j < rdungeons[dungeonNum].Floors[i].Npcs.Count; j++)
                    {
                        database.UpdateOrInsert("rdungeon_npc", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),
                    database.CreateColumn(false, "npc_index", j.ToString()),
                    database.CreateColumn(false, "npc_num", rdungeons[dungeonNum].Floors[i].Npcs[j].NpcNum.ToString()),
                    database.CreateColumn(false, "min_level", rdungeons[dungeonNum].Floors[i].Npcs[j].MinLevel.ToString()),
                    database.CreateColumn(false, "max_level", rdungeons[dungeonNum].Floors[i].Npcs[j].MaxLevel.ToString()),
                    database.CreateColumn(false, "chance", rdungeons[dungeonNum].Floors[i].Npcs[j].AppearanceRate.ToString()),
                    database.CreateColumn(false, "status", ((int)rdungeons[dungeonNum].Floors[i].Npcs[j].StartStatus).ToString()),
                    database.CreateColumn(false, "status_counter", rdungeons[dungeonNum].Floors[i].Npcs[j].StartStatusCounter.ToString()),
                    database.CreateColumn(false, "status_chance", rdungeons[dungeonNum].Floors[i].Npcs[j].StartStatusChance.ToString()),
                    });
                    }

                    for (int j = 0; j < rdungeons[dungeonNum].Floors[i].SpecialTiles.Count; j++)
                    {
                        database.UpdateOrInsert("rdungeon_tile", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),
                    database.CreateColumn(false, "tile_index", j.ToString()),
                    database.CreateColumn(false, "tile_type", ((int)rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Type).ToString()),
                    database.CreateColumn(false, "data1", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Data1.ToString()),
                    database.CreateColumn(false, "data2", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Data2.ToString()),
                    database.CreateColumn(false, "data3", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Data3.ToString()),
                    database.CreateColumn(false, "string1", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].String1),
                    database.CreateColumn(false, "string2", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].String2),
                    database.CreateColumn(false, "string3", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].String3),
                    database.CreateColumn(false, "ground", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Ground.ToString()),
                    database.CreateColumn(false, "mask1", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Mask.ToString()),
                    database.CreateColumn(false, "mask2", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Mask2.ToString()),
                    database.CreateColumn(false, "fringe1", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Fringe.ToString()),
                    database.CreateColumn(false, "fringe2", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Fringe2.ToString()),
                    database.CreateColumn(false, "ground_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].GroundSet.ToString()),
                    database.CreateColumn(false, "mask1_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].MaskSet.ToString()),
                    database.CreateColumn(false, "mask2_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Mask2Set.ToString()),
                    database.CreateColumn(false, "fringe1_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].FringeSet.ToString()),
                    database.CreateColumn(false, "fringe2_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Fringe2Set.ToString()),
                    database.CreateColumn(false, "ground_anim", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].GroundAnim.ToString()),
                    database.CreateColumn(false, "mask1_anim", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].Anim.ToString()),
                    database.CreateColumn(false, "mask2_anim", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].M2Anim.ToString()),
                    database.CreateColumn(false, "fringe1_anim", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].FAnim.ToString()),
                    database.CreateColumn(false, "fringe2_anim", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].F2Anim.ToString()),
                    database.CreateColumn(false, "ground_anim_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].GroundAnimSet.ToString()),
                    database.CreateColumn(false, "mask1_anim_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].AnimSet.ToString()),
                    database.CreateColumn(false, "mask2_anim_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].M2AnimSet.ToString()),
                    database.CreateColumn(false, "fringe1_anim_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].FAnimSet.ToString()),
                    database.CreateColumn(false, "fringe2_anim_set", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].F2AnimSet.ToString()),
                    database.CreateColumn(false, "rdungeon_value", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].RDungeonMapValue.ToString()),
                    database.CreateColumn(false, "chance", rdungeons[dungeonNum].Floors[i].SpecialTiles[j].AppearanceRate.ToString()),
                    });
                    }

                    for (int j = 0; j < rdungeons[dungeonNum].Floors[i].Weather.Count; j++)
                    {
                        database.UpdateOrInsert("rdungeon_weather", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),
                    database.CreateColumn(false, "weather_index", j.ToString()),
                    database.CreateColumn(false, "weather_type", ((int)rdungeons[dungeonNum].Floors[i].Weather[j]).ToString()),
                    });
                    }

                    for (int j = 0; j < rdungeons[dungeonNum].Floors[i].Options.Chambers.Count; j++)
                    {
                        database.UpdateOrInsert("rdungeon_chamber", new IDataColumn[] {
                    database.CreateColumn(false, "num", dungeonNum.ToString()),
                    database.CreateColumn(false, "floor_num", i.ToString()),
                    database.CreateColumn(false, "chamber_index", j.ToString()),
                    database.CreateColumn(false, "chamber_num", rdungeons[dungeonNum].Floors[i].Options.Chambers[j].ChamberNum.ToString()),
                    database.CreateColumn(false, "string1", rdungeons[dungeonNum].Floors[i].Options.Chambers[j].String1),
                    database.CreateColumn(false, "string2", rdungeons[dungeonNum].Floors[i].Options.Chambers[j].String2),
                    database.CreateColumn(false, "string3", rdungeons[dungeonNum].Floors[i].Options.Chambers[j].String3),
                    });
                    }

                }

                database.EndTransaction();
            }
        }

        #endregion

    }
}
