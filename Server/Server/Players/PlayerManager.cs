//namespace Server.Players
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//    using System.Xml;
//    using Server.Maps;

//    using PMU.DatabaseConnector;
//    using PMU.DatabaseConnector.SQLite;
//    using Server.Network;

//    public class PlayerManagerOld
//    {
//        #region Fields

//        static bool savingEnabled;
//        static IDatabase charListDB;

//        #endregion Fields

//        #region Properties

//        public static bool SavingEnabled {
//            get { return savingEnabled; }
//            set { savingEnabled = value; }
//        }

//        #endregion Properties

//        #region Methods

//        public static bool AccountExists(string login) {
//            return IO.IO.DirectoryExists(IO.Paths.AccountFolder + login);
//        }

//        public static void AddToCharList(string name, string account, int characterSlot, string charID) {
//            charListDB.UpdateOrInsert("tblCharList", new IDataColumn[] { 
//                charListDB.CreateColumn("name", name),
//                charListDB.CreateColumn("account", account),
//                charListDB.CreateColumn("slot", characterSlot.ToString()),
//                charListDB.CreateColumn("id", charID)
//            }, "name=\"" + name + "\"");
//        }

//        public static void DeleteFromCharList(string name) {
//            if (name.ToLower().Trim() == "")
//                return;
//            charListDB.DeleteRow("tblCharList", "name=\"" + name + "\"");
//        }

//        public static bool CharExists(string charName) {
//            IDataColumn[] columns = charListDB.RetrieveRow("tblCharList", "name", "name=\"" + charName + "\"");
//            return (columns != null);
//        }

//        public static void Initialize() {
//            //players = new PlayerCollection(maxPlayers);
//            string charListDBPath = IO.Paths.AccountFolder + "charlist.sqlite";
//            charListDB = new SQLite("Data Source=" + charListDBPath + ";Version=3;");
//            Bans.Initialize();
//            savingEnabled = true;
//            Server.Players.Parties.PartyManager.Initialize();
//            VeteranCompensationHelper.InitializeDB();
//        }

//        public static void LoadCharList() {
//            if (IO.IO.FileExists(IO.Paths.AccountFolder + "charlist.sqlite") == false) {
//                System.IO.File.Create(IO.Paths.AccountFolder + "charlist.sqlite").Close();
//                charListDB.CreateTable("tblCharList", new IDataField[] { 
//                    charListDB.CreateField("name", "TEXT"),
//                    charListDB.CreateField("account", "TEXT"),
//                    charListDB.CreateField("slot", "TEXT"),
//                    charListDB.CreateField("id", "TEXT")
//                });
//            }
//        }

//        public static string FindCharacterID(string characterName) {
//            IDataColumn[] columns = charListDB.RetrieveRow("tblCharList", "id", "name=\"" + characterName + "\"");
//            if (columns != null) {
//                return (string)columns[0].Value;
//            } else {
//                return null;
//            }
//        }

//        public static string FindCharacterName(string characterID) {
//            IDataColumn[] columns = charListDB.RetrieveRow("tblCharList", "name", "id=\"" + characterID + "\"");
//            if (columns != null) {
//                return (string)columns[0].Value;
//            } else {
//                return null;
//            }
//        }

//        public static CharacterInformation RetrieveCharacterInformation(string column, string value) {
//            IDataColumn[] columns = charListDB.RetrieveRow("tblCharList", "name, account, slot, id", column + "=\"" + value + "\"");
//            if (columns != null) {

//                CharacterInformation characterInfo = new CharacterInformation(columns[0].Value.ToString(),
//                    columns[1].Value.ToString(), columns[2].Value.ToString().ToInt(),
//                    columns[3].Value.ToString());

//                return characterInfo;
//            } else {
//                return null;
//            }
//        }

//        public static void LoadPlayer(Client client, int charNum) {
//            if (client.Player != null)
//                client.Player.Load(charNum);
//        }

//        public static void LoadPlayerChars(Client client) {
//            if (client.Player != null)
//                client.Player.LoadChars();
//        }

//        public static void SavePlayer(Client client) {
//            if (client.Player != null)
//                client.Player.Save();
//        }

//        public static void CreateAccount(Client client, string name, string password) {
//            client.Player.SetPassword(name, password);

//            if (System.IO.Directory.Exists(IO.Paths.AccountFolder + name) == false) {
//                System.IO.Directory.CreateDirectory(IO.Paths.AccountFolder + name);
//            }

//            for (int i = 1; i <= 3; i++) {
//                client.Player.mCharNum = i;
//                client.Player.MapID = MapManager.GenerateMapID(Settings.Crossroads);
//                client.Player.X = 8;
//                client.Player.Y = 8;
//                client.Player.Save();
//            }

//            client.Player.UpdatePassword();

//            client.Player = new Player(client);
//        }

//        #endregion Methods
//    }
//}