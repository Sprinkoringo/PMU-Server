namespace Server.Exp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class ExpManager
    {
        #region Fields

        static ExpCollection exp;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static ExpCollection Exp {
            get { return exp; }
        }

        #endregion Properties

        #region Methods


        public static void Initialize() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                //method for getting count
                string query = "SELECT COUNT(level_num) FROM experience";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                int count = row["COUNT(level_num)"].ValueString.ToInt();
                exp = new ExpCollection(count);
            }
        }

        public static void LoadExps(object object1) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                try
                {
                    MySql database = dbConnection.Database;

                    string query = "SELECT * " +
                        "FROM experience";

                    foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
                    {
                        int level = columnCollection["level_num"].ValueString.ToInt();

                        exp[level - 1] = columnCollection["med_slow"].ValueString.ToUlng();

                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(level, exp.MaxLevels));
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

        #endregion Methods
    }
}