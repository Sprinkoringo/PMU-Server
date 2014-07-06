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