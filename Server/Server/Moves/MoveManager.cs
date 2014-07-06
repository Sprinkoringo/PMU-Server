namespace Server.Moves
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class MoveManager
    {
        #region Fields

        protected static MoveCollection moves;

        static Move standardAttack;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static MoveCollection Moves {
            get { return moves; }
        }

        public static Move StandardAttack
        {
            get
            {
                return standardAttack;
            }

        }

        #endregion Properties

        #region Methods
        /*
        public static int FindMoveName(string moveName){
            for (int i = 1; i <= moves.MaxMoves; i++) {
                if (Moves.Moves[i].Name == moveName) {
                    return i;
                }
            }
            return -1;
        }
        */


        public static void Initialize() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                //method for getting count
                string query = "SELECT COUNT(num) FROM move";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                int count = row["COUNT(num)"].ValueString.ToInt();
                moves = new MoveCollection(count);

            }
            standardAttack = new Move();

            //Set standard attack
            standardAttack.Name = "its regular attack";
            standardAttack.RangeType = Enums.MoveRange.FrontOfUser;
            standardAttack.Range = 1;
            standardAttack.Accuracy = 90;
            standardAttack.TargetType = Enums.MoveTarget.Foes;
            standardAttack.Element = Enums.PokemonType.None;
            standardAttack.MoveCategory = Enums.MoveCategory.Physical;
            standardAttack.EffectType = Enums.MoveType.SubHP;
            standardAttack.HitTime = 900;
            standardAttack.HitFreeze = false;
            standardAttack.Data1 = Server.Constants.STANDARDATTACKBASEPOWER;
            standardAttack.AttackerAnim.AnimationIndex = -1;
            standardAttack.TravelingAnim.AnimationIndex = -1;
            standardAttack.DefenderAnim.AnimationIndex = -1;
            standardAttack.Sound = -1;
        }

        public static void LoadMove(int moveNum, MySql database)
        {
            if (moves.Moves.ContainsKey(moveNum) == false)
                moves.Moves.Add(moveNum, new Move());

            string query = "SELECT name, " +
                "max_pp, " +
                "effect_type, " +
                "element, " +
                "category, " +
                "range_type, " +
                "range_dist, " +
                "target, " +
                "data1, " +
                "data2, " +
                "data3, " +
                "effect_data1, " +
                "effect_data2, " +
                "effect_data3, " +
                "accuracy, " +
                "hit_time, " +
                "hit_freeze, " +
                "per_player, " +
                "key_effect, " +
                "sound, " +
                "attack_anim_type, " +
                "attack_anim_index, " +
                "attack_anim_speed, " +
                "attack_anim_cycle, " +
                "travel_anim_type, " +
                "travel_anim_index, " +
                "travel_anim_speed, " +
                "travel_anim_cycle, " +
                "landed_anim_type, " +
                "landed_anim_index, " +
                "landed_anim_speed, " +
                "landed_anim_cycle " +
                "FROM move WHERE move.num = \'" + moveNum + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                moves[moveNum].Name = row["name"].ValueString;
                moves[moveNum].MaxPP = row["max_pp"].ValueString.ToInt();
                moves[moveNum].EffectType = (Enums.MoveType)row["effect_type"].ValueString.ToInt();
                moves[moveNum].Element = (Enums.PokemonType)row["element"].ValueString.ToInt();
                moves[moveNum].MoveCategory = (Enums.MoveCategory)row["category"].ValueString.ToInt();
                moves[moveNum].RangeType = (Enums.MoveRange)row["range_type"].ValueString.ToInt();
                moves[moveNum].Range = row["range_dist"].ValueString.ToInt();
                moves[moveNum].TargetType = (Enums.MoveTarget)row["target"].ValueString.ToInt();
                moves[moveNum].Data1 = row["data1"].ValueString.ToInt();
                moves[moveNum].Data2 = row["data2"].ValueString.ToInt();
                moves[moveNum].Data3 = row["data3"].ValueString.ToInt();
                moves[moveNum].AdditionalEffectData1 = row["effect_data1"].ValueString.ToInt();
                moves[moveNum].AdditionalEffectData2 = row["effect_data2"].ValueString.ToInt();
                moves[moveNum].AdditionalEffectData3 = row["effect_data3"].ValueString.ToInt();
                moves[moveNum].Accuracy = row["accuracy"].ValueString.ToInt();
                moves[moveNum].HitTime = row["hit_time"].ValueString.ToInt();
                moves[moveNum].HitFreeze = row["hit_freeze"].ValueString.ToBool();
                moves[moveNum].PerPlayer = row["per_player"].ValueString.ToBool();
                moves[moveNum].KeyItem = row["key_effect"].ValueString.ToInt();
                moves[moveNum].Sound = row["sound"].ValueString.ToInt();
                moves[moveNum].AttackerAnim.AnimationType = (Enums.MoveAnimationType)row["attack_anim_type"].ValueString.ToInt();
                moves[moveNum].AttackerAnim.AnimationIndex = row["attack_anim_index"].ValueString.ToInt();
                moves[moveNum].AttackerAnim.FrameSpeed = row["attack_anim_speed"].ValueString.ToInt();
                moves[moveNum].AttackerAnim.Repetitions = row["attack_anim_cycle"].ValueString.ToInt();
                moves[moveNum].TravelingAnim.AnimationType = (Enums.MoveAnimationType)row["travel_anim_type"].ValueString.ToInt();
                moves[moveNum].TravelingAnim.AnimationIndex = row["travel_anim_index"].ValueString.ToInt();
                moves[moveNum].TravelingAnim.FrameSpeed = row["travel_anim_speed"].ValueString.ToInt();
                moves[moveNum].TravelingAnim.Repetitions = row["travel_anim_cycle"].ValueString.ToInt();
                moves[moveNum].DefenderAnim.AnimationType = (Enums.MoveAnimationType)row["landed_anim_type"].ValueString.ToInt();
                moves[moveNum].DefenderAnim.AnimationIndex = row["landed_anim_index"].ValueString.ToInt();
                moves[moveNum].DefenderAnim.FrameSpeed = row["landed_anim_speed"].ValueString.ToInt();
                moves[moveNum].DefenderAnim.Repetitions = row["landed_anim_cycle"].ValueString.ToInt();
            }
        }

        public static void LoadMoves(object object1) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                try
                {
                    for (int i = 1; i <= moves.MaxMoves; i++)
                    {
                        LoadMove(i, dbConnection.Database);
                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(i, moves.MaxMoves));
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

        public static void SaveMove(int moveNum)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                database.BeginTransaction();

                database.ExecuteNonQuery("DELETE FROM move WHERE num = \'" + moveNum + "\'");

                database.UpdateOrInsert("move", new IDataColumn[] {
                    database.CreateColumn(false, "num", moveNum.ToString()),
                    database.CreateColumn(false, "name", moves[moveNum].Name),
                    database.CreateColumn(false, "max_pp", moves[moveNum].MaxPP.ToString()),
                    database.CreateColumn(false, "effect_type", ((int)moves[moveNum].EffectType).ToString()),
                    database.CreateColumn(false, "element", ((int)moves[moveNum].Element).ToString()),
                    database.CreateColumn(false, "category", ((int)moves[moveNum].MoveCategory).ToString()),
                    database.CreateColumn(false, "range_type", ((int)moves[moveNum].RangeType).ToString()),
                    database.CreateColumn(false, "range_dist", moves[moveNum].Range.ToString()),
                    database.CreateColumn(false, "target", ((int)moves[moveNum].TargetType).ToString()),
                    database.CreateColumn(false, "data1", moves[moveNum].Data1.ToString()),
                    database.CreateColumn(false, "data2", moves[moveNum].Data2.ToString()),
                    database.CreateColumn(false, "data3", moves[moveNum].Data3.ToString()),
                    database.CreateColumn(false, "effect_data1", moves[moveNum].AdditionalEffectData1.ToString()),
                    database.CreateColumn(false, "effect_data2", moves[moveNum].AdditionalEffectData2.ToString()),
                    database.CreateColumn(false, "effect_data3", moves[moveNum].AdditionalEffectData3.ToString()),
                    database.CreateColumn(false, "accuracy", moves[moveNum].Accuracy.ToString()),
                    database.CreateColumn(false, "hit_time", moves[moveNum].HitTime.ToString()),
                    database.CreateColumn(false, "hit_freeze", moves[moveNum].HitFreeze.ToIntString()),
                    database.CreateColumn(false, "per_player", moves[moveNum].PerPlayer.ToIntString()),
                    database.CreateColumn(false, "key_effect", moves[moveNum].KeyItem.ToString()),
                    database.CreateColumn(false, "sound", moves[moveNum].Sound.ToString()),
                    database.CreateColumn(false, "attack_anim_type", ((int)moves[moveNum].AttackerAnim.AnimationType).ToString()),
                    database.CreateColumn(false, "attack_anim_index", moves[moveNum].AttackerAnim.AnimationIndex.ToString()),
                    database.CreateColumn(false, "attack_anim_speed", moves[moveNum].AttackerAnim.FrameSpeed.ToString()),
                    database.CreateColumn(false, "attack_anim_cycle", moves[moveNum].AttackerAnim.Repetitions.ToString()),
                    database.CreateColumn(false, "travel_anim_type", ((int)moves[moveNum].TravelingAnim.AnimationType).ToString()),
                    database.CreateColumn(false, "travel_anim_index", moves[moveNum].TravelingAnim.AnimationIndex.ToString()),
                    database.CreateColumn(false, "travel_anim_speed", moves[moveNum].TravelingAnim.FrameSpeed.ToString()),
                    database.CreateColumn(false, "travel_anim_cycle", moves[moveNum].TravelingAnim.Repetitions.ToString()),
                    database.CreateColumn(false, "landed_anim_type", ((int)moves[moveNum].DefenderAnim.AnimationType).ToString()),
                    database.CreateColumn(false, "landed_anim_index", moves[moveNum].DefenderAnim.AnimationIndex.ToString()),
                    database.CreateColumn(false, "landed_anim_speed", moves[moveNum].DefenderAnim.FrameSpeed.ToString()),
                    database.CreateColumn(false, "landed_anim_cycle", moves[moveNum].DefenderAnim.Repetitions.ToString()),
                });

                database.EndTransaction();
            }
        }

        #endregion Methods
    }
}