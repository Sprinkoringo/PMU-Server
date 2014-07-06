namespace Script {
    using System;
    using System.Collections.Generic;
    using System.Text;

    using PMU.DatabaseConnector;
    using PMU.DatabaseConnector.SQLite;

    using Server.IO;

    public class DatabaseManager {
        //#region Fields

        //static SettingsDatabase optionsDB;

        //#endregion Fields

        //#region Properties

        //public static SettingsDatabase OptionsDB {
        //    get { return optionsDB; }
        //}

        //#endregion Properties

        #region Methods

        public static void InitOptionsDB() {
            //string dbPath = Paths.ScriptsIOFolder + "OptionsDB.sqlite";
            //optionsDB = new SettingsDatabase(new SQLite("Data Source=" + dbPath + ";Version=3;"));
        }

        #endregion Methods
    }
}