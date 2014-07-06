//using System;
//using System.Collections.Generic;
//using System.Text;
//using Server.Items;
//using Server.Npcs;
//using Server.Scripting;
//using Server.Missions;
//using Server.Maps;
//using Server.RDungeons;
//using Server.CustomMenus;
//using System.Xml;
//using Server.Network;
//using Server.Moves;
//using PMU.Core;
//using PMU.Sockets;
//using Server.Players.Parties;
//using Server.Stories;
//using Server.WonderMails;

//namespace Server.Players
//{
//    public partial class PlayerOld
//    {
//        #region Variables

//        internal Statistics.Statistics statistics;
//        public Statistics.Statistics Statistics {
//            get { return statistics; }
//        }

//        internal bool uninstancedWarp = false;

//        public long DataTransfered { get; set; }

//        public string Login { get { return mLogin; } }
//        // Account Data
//        internal string mLogin = "";
//        internal string mPassword = "";

        //// Guild Data
        //public string GuildName { get; set; }
        //public Enums.GuildRank GuildAccess { get; set; }

//        // Inv/Bank Data
//        public Dictionary<int, InventoryItem> Inventory { get; set; }
//        public Dictionary<int, InventoryItem> Bank { get; set; }

//        int mMaxInv;
//        int mMaxBank;
//        public int MaxInv { get { return mMaxInv; } }
//        public int MaxBank { get { return mMaxBank; } }

//        // Story Data
//        public Dictionary<int, bool> Stories { get; set; } // True = Locked, False = Unlocked
//        public Story CurrentChapter { get { return mCurrentChapter; } }
//        public int CurrentSegment { get { return mCurrentSegment; } }
//        internal Story mCurrentChapter = null;
//        internal int mCurrentSegment = -1;
//        internal int mSegmentToStart = -1;
//        internal bool loadingStory = false;
//        StoryHelper storyHelper= null;
//        public StoryHelper StoryHelper {
//            get { return storyHelper; }
//        }

//        // Mission Data
//        public Dictionary<int, bool> Missions { get; set; }// True = Completed, False = Incomplete
//        public int MissionExp { get; set; }
//        public Enums.ExplorerRank ExplorerRank { get; set; }

//        // Team Data
//        public int ActiveSlot { get; set; }
//        public Recruit[] Team { get; set; }
//        internal Recruit mRequestedRecruit;

//        // Friends Data
//        internal List<string> mFriends;

//        // Location Data
//        public string MapID { get; set; }
//        public int X { get; set; }
//        public int Y { get; set; }
//        public Enums.Direction Direction { get; set; }
//        //public Enums.Direction PreDirection { get; set; }
//        public IMap Map { get { return MapManager.RetrieveActiveMap(MapID); } }

//        //Pokedex Data
//        internal ListPair<int, Enums.PokedexStat> mPokedex;

//        // Dungeon Completion Counts
//        internal ListPair<int, int> mDungeonCompletionCounts;

//        // Misc Data
//        public bool PK { get; set; }
//        public bool Solid { get; set; }
//        public string Status { get; set; }
//        public bool Hunted { get; set; }
//        public string Name { get; set; }
//        public Enums.Sex Sex { get; set; }

//        // Party
//        internal string partyID = null;
//        public string PartyID {
//            get { return partyID; }
//        }

//        // Random Dungeons
//        //internal int dungeonFloor;
//        //internal int dungeonIndex;

//        //public int DungeonIndex {
//        //    get { return dungeonIndex; }
//        //}
//        //public int DungeonFloor {
//        //    get { return dungeonFloor; }
//        //}

//        //internal Maps.InstancedMapManager mInstancedMapManager;

//        internal CustomMenuManager mCustomMenuManager;
//        public CustomMenuManager CustomMenuManager {
//            get { return mCustomMenuManager; }
//        }

//        // Internal Data
//        internal Client client;
//        public Client Client { get { return client; } }
//        string id;
//        public string CharID { get { return id; } }
//        internal bool mLoaded = false;
//        internal string mBuffer = "";
//        internal int mDataSize;
//        internal bool mLoggedIn = false;
//        internal bool mInGame = false;
//        internal int mCharNum;
//        public int CharNum { get { return mCharNum; } }
//        public int BellyStepCounter { get; set; }
//        public int HPStepCounter { get; set; }
//        public int ConfusionStepCounter { get; set; }
//        public Enums.Speed SpeedLimit { get; set; }
//        public bool CanSwapActiveRecruit { get; set; }
//        //public Enums.StatusAilment StatusAilment { get; set; }// moved to recruits
//        //public int StatusAilmentCounter { get; set; }//moved to recruits

//        internal bool mLocked = false;
//        internal bool mGettingMap;

//        public TickCount AttackTimer { get; set; }
//        public TickCount PauseTimer { get; set; }
//        //internal TickCount mStatusTimer;

//        public bool Muted { get; set; }
//        internal string mQuestionID;
//        internal bool mInScriptEditor = false;

//        //  System Data
//        internal string mOSVersion;
//        internal Version mPlatformVersion;
//        internal string mClientEdition;

//        // Account Data
//        public Enums.Rank Access = Enums.Rank.Normal;
//        internal string[] mCharName;

//        // Tracker Data
//        internal List<string> mSubmitedTickets;

//        internal Scripting.TimerCollection mTimers;
//        public Scripting.TimerCollection Timers {
//            get { return mTimers; }
//        }
//        public IExPlayer ExPlayer { get; set; }

//        // Wonder Mail Data
//        public WonderMailTasks JobList { get; set; }
//        public WonderMails.MissionBoard MissionBoard { get; set; }
//        public Mission ActiveMission { get; set; }
//        internal bool missionComplete;

//        // ExpKit Data
//        public AvailableExpKitModuleCollection AvailableExpKitModules { get; set; }
//        internal Enums.ExpKitModules activeExpKitModule;
//        public Enums.ExpKitModules ActiveExpKitModule {
//            get { return activeExpKitModule; }
//        }

//        bool usingTempStats;
//        public bool UsingTempStats {
//            get { return usingTempStats; }
//        }

//        internal string tradePartner;
//        public string TradePartner {
//            get { return tradePartner; }
//        }
//        internal int setTradeItem;
//        public int SetTradeItem {
//            get { return setTradeItem; }
//        }
//        internal bool readyToTrade;

//        internal bool inMapEditor;
//        public bool InMapEditor {
//            get { return inMapEditor; }
//        }

//        internal List<Stories.Story> storyPlaybackCache;
//        //public List<Combat.BattleMessage> WaitingBattleMsg;
//        internal List<Events.Player.TriggerEvents.ITriggerEvent> triggerEvents;

//        internal bool veteran;
//        public bool Veteran {
//            get { return veteran; }
//        }

//        internal Tournaments.MatchUp tournamentMatchUp;
//        public Tournaments.MatchUp TournamentMatchUp {
//            get { return tournamentMatchUp; }
//        }
//        internal Tournaments.Tournament tournament;
//        public Tournaments.Tournament Tournament {
//            get { return tournament; }
//        }
//        internal Tournaments.MatchUp tournamentSpectatingMatchUp;
//        public Tournaments.MatchUp TournamentSpectatingMatchUp {
//            get { return tournamentSpectatingMatchUp; }
//        }

        ////Seen Characters Data
        //private SeenCharacter[] seenNpcs { get; set; }
        //private ListPair<string, SeenCharacter> seenPlayers { get; set; }

        //internal Player(Client client) {
        //    this.client = client;
        //    mBuffer = "";
        //    mDataSize = 0;
        //    mCharName = new string[4];
        //    Inventory = new Dictionary<int, InventoryItem>();
        //    Bank = new Dictionary<int, InventoryItem>();
        //    mFriends = new List<string>();
        //    Stories = new Dictionary<int, bool>();
        //    storyHelper = new Players.StoryHelper(this);
        //    Missions = new Dictionary<int, bool>();
        //    Team = new Recruit[Constants.MAX_ACTIVETEAM];
        //    mCustomMenuManager = new CustomMenuManager(client);
        //    mSubmitedTickets = new List<string>();
        //    mPokedex = new ListPair<int, Enums.PokedexStat>();
        //    mDungeonCompletionCounts = new ListPair<int, int>();
        //    mTimers = new Scripting.TimerCollection();
        //    mOSVersion = "";
        //    mPlatformVersion = null;
        //    mClientEdition = "";
        //    GuildName = "";
        //    SpeedLimit = Enums.Speed.Running;
        //    CanSwapActiveRecruit = true;
        //    Status = "";
        //    Name = "";
        //    JobList = new WonderMailTasks(this);
        //    MissionBoard = new WonderMails.MissionBoard(this);
        //    ActiveMission = null;
        //    id = null;
        //    AvailableExpKitModules = new AvailableExpKitModuleCollection();

        //    storyPlaybackCache = new List<Stories.Story>();
        //    //WaitingBattleMsg = new List<Combat.BattleMessage>();
        //    triggerEvents = new List<Events.Player.TriggerEvents.ITriggerEvent>();

        //    seenNpcs = new SeenCharacter[Constants.MAX_MAP_NPCS];
        //    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
        //        seenNpcs[i] = new SeenCharacter();
        //    }
        //    seenPlayers = new ListPair<string, SeenCharacter>();
        //}

//        #endregion

//        #region Saving

//        internal string GetCharFolder() {
//            return IO.IO.ProcessPath(IO.Paths.AccountFolder + mLogin + "\\Char" + mCharNum.ToString() + "\\");
//        }

//        internal void SetPassword(string login, string password) {
//            mLogin = login;
//#if !DEBUG
//            mPassword = password;
//#endif
//        }

//        internal void UpdatePassword() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(IO.Paths.AccountFolder + mLogin + "\\info.xml"))) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("AccountData");

//                writer.WriteElementString("Login", mLogin);
//                writer.WriteElementString("Password", mPassword);

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        private void CheckFolders() {
//            if (System.IO.Directory.Exists(GetCharFolder()) == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder());
//            }
//            if (System.IO.Directory.Exists(GetCharFolder() + "Recruits") == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder() + "Recruits");
//            }
//            if (System.IO.Directory.Exists(GetCharFolder() + "Bank") == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder() + "Bank");
//            }
//            if (System.IO.Directory.Exists(GetCharFolder() + "Story") == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder() + "Story");
//            }
//            if (System.IO.Directory.Exists(GetCharFolder() + "Missions") == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder() + "Missions");
//            }
//            if (System.IO.Directory.Exists(GetCharFolder() + "WonderMail") == false) {
//                System.IO.Directory.CreateDirectory(GetCharFolder() + "WonderMail");
//            }
//        }

//        public void Save() {
//            if (PlayerManager.SavingEnabled) {
//                if (string.IsNullOrEmpty(mLogin)) {
//                    return;
//                }
//                if (!mLoaded) {
//                    return;
//                }

//                CheckFolders();

//                SaveMissions();
//                SaveStories();
//                SaveInfoFile();
//                SaveInventory();
//                SaveBank();
//                SaveWonderMail();
//                SaveTriggerEvents();
//                storyHelper.SaveSettings();

//                if (Map != null && (Map.MapType == Enums.MapType.Instanced || Map.MapType == Enums.MapType.RDungeonMap)) {
//                    Map.Save();
//                    //mInstancedMapManager.SaveActiveMap();
//                }
//            } else {
//                Messenger.PlayerMsg(client, "Saving is currently disabled.", Text.BrightRed);
//            }
//        }

//        public void SaveBank() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "Bank\\bank.xml"), Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("Bank");
//                writer.WriteElementString("MaxBank", client.Player.MaxBank.ToString());

//                for (int i = 1; i <= mMaxBank; i++) {
//                    if (Bank.ContainsKey(i) == false) {
//                        Bank.Add(i, new InventoryItem());
//                    }
//                    Bank[i].Save(writer, "Item");
//                }

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        public void SaveStories() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "Story\\data.xml"), Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("StoryData");

//                for (int i = 0; i <= Stories.StoryManager.Stories.MaxStories; i++) {
//                    if (Stories.ContainsKey(i)) {
//                        writer.WriteStartElement("Chapter");
//                        writer.WriteAttributeString("num", i.ToString());
//                        writer.WriteElementString("Completed", Stories[i].ToString());
//                        writer.WriteEndElement();
//                    }
//                }

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        public void SaveMissions() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "Missions\\data.xml"), Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("MissionData");

//                writer.WriteElementString("MissionExp", MissionExp.ToString());

//                writer.WriteStartElement("Premade");
//                for (int i = 0; i < Missions.Count; i++) {
//                    if (Missions.ContainsKey(i)) {
//                        writer.WriteStartElement("Mission");
//                        writer.WriteAttributeString("num", i.ToString());
//                        writer.WriteElementString("Completed", Missions[i].ToString());
//                        writer.WriteEndElement();
//                    }
//                }
//                writer.WriteEndElement();

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        public void SaveInventory() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "inventory.xml"), Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("Inventory");
//                writer.WriteElementString("MaxInv", client.Player.MaxInv.ToString());

//                for (int i = 1; i <= mMaxInv; i++) {
//                    if (Inventory.ContainsKey(i) == false) {
//                        Inventory.Add(i, new InventoryItem());
//                    }
//                    Inventory[i].Save(writer, "InvItem");
//                }

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        public void SaveInfoFile() {
//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "info.xml"), Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("Player");

//                writer.WriteStartElement("General");

//                writer.WriteElementString("ID", CharID);
//                writer.WriteElementString("Name", Name);
//                writer.WriteElementString("Access", ((int)Access).ToString());
//                //writer.WriteElementString("Sex", ((int)Sex).ToString());
//                writer.WriteElementString("PK", PK.ToString());
//                writer.WriteElementString("Solid", Solid.ToString());
//                writer.WriteElementString("Map", MapID);
//                writer.WriteElementString("X", X.ToString());
//                writer.WriteElementString("Y", Y.ToString());
//                writer.WriteElementString("Status", Status);
//                //writer.WriteElementString("RDungeonNum", dungeonIndex.ToString());
//                //writer.WriteElementString("RDungeonFloor", dungeonFloor.ToString());
//                string availableExpKitModules = "";
//                for (int i = 0; i < AvailableExpKitModules.Count; i++) {
//                    if (AvailableExpKitModules[i].Temporary == false) {
//                        availableExpKitModules += (int)(AvailableExpKitModules[i].Type) + ";";
//                    }
//                }
//                writer.WriteElementString("AvailableExpKitModules", availableExpKitModules);
//                writer.WriteElementString("InTempMode", usingTempStats.ToString());
//                writer.WriteElementString("Veteran", veteran.ToString());

//                writer.WriteEndElement();
//                //writer.WriteStartElement("Equips");

//                //writer.WriteElementString("Armor", Armor.ToString());
//                //writer.WriteElementString("Weapon", Weapon.ToString());
//                //writer.WriteElementString("Helmet", Helmet.ToString());
//                //writer.WriteElementString("Shield", Shield.ToString());
//                //writer.WriteElementString("Leg", Legs.ToString());
//                //writer.WriteElementString("Necklace", Necklace.ToString());
//                //writer.WriteElementString("Ring", Ring.ToString());

//                //writer.WriteEndElement();
//                writer.WriteStartElement("Team");

//                writer.WriteElementString("ActiveSlot", ActiveSlot.ToString());
//                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                    writer.WriteStartElement("TeamSlot");
//                    writer.WriteAttributeString("num", i.ToString());
//                    if (Team[i] != null) {
//                        writer.WriteString(Team[i].RecruitIndex.ToString());
//                        if (Team[i].RecruitIndex != -1) {
//                            Team[i].Save(GetCharFolder(), i);
//                        }
//                    } else {
//                        writer.WriteString("-1");
//                    }
//                    writer.WriteEndElement();
//                }

//                writer.WriteEndElement();
//                writer.WriteStartElement("Guild");

                //writer.WriteElementString("GuildName", GuildName);
                //writer.WriteElementString("GuildAccess", ((int)GuildAccess).ToString());

//                writer.WriteEndElement();
//                writer.WriteStartElement("StoryData");

//                if (mCurrentChapter != null) {
//                    if (!string.IsNullOrEmpty(mCurrentChapter.ID.ToString())) {
//                        writer.WriteElementString("CurrentChapter", mCurrentChapter.ID.ToString());
//                    } else {
//                        writer.WriteElementString("CurrentChapter", "-1");
//                    }
//                } else {
//                    writer.WriteElementString("CurrentChapter", "-1");
//                }
//                writer.WriteElementString("CurrentSegment", mCurrentSegment.ToString());

//                writer.WriteEndElement();

//                writer.WriteStartElement("Friends");

//                for (int i = 0; i < mFriends.Count; i++) {
//                    writer.WriteElementString("Friend", mFriends[i]);
//                }

//                writer.WriteEndElement();
//                writer.WriteStartElement("DungeonData");

//                for (int i = 0; i < mDungeonCompletionCounts.Count; i++) {
//                    int id = mDungeonCompletionCounts.Keys[i];
//                    writer.WriteStartElement("DungeonInfo");
//                    writer.WriteAttributeString("id", id.ToString());
//                    writer.WriteElementString("CompletionCount", mDungeonCompletionCounts.GetValue(id).ToString());
//                    writer.WriteEndElement();
//                }

//                writer.WriteEndElement();

//                writer.WriteStartElement("PokedexData");

//                for (int i = 0; i < mPokedex.Count; i++)
//                {
//                    int id = mPokedex.Keys[i];
//                    writer.WriteStartElement("Pokemon");
//                    writer.WriteAttributeString("id", id.ToString());
//                    writer.WriteElementString("Status", ((int)mPokedex.GetValue(id)).ToString());
//                    writer.WriteEndElement();
//                }

//                writer.WriteEndElement();

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        public void SaveWonderMail() {
//            JobList.SaveCompletedMail();
//            JobList.SaveJobList();
//            MissionBoard.SaveMissionBoardData();
//        }

//        public void SaveTriggerEvents() {
//            using (XmlWriter writer = XmlWriter.Create(GetCharFolder() + "triggerevents.xml", Settings.XmlWriterSettings)) {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("TriggerEvents");

//                for (int i = 0; i < triggerEvents.Count; i++) {
//                    triggerEvents[i].Save(writer);
//                }

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        internal void AddChar(string name, Enums.Sex sex, int charNum, bool silent, bool generateID) {
//            mCharNum = charNum;
//            ActiveSlot = 0;

//            Team[0] = new Recruit(client);
//            Team[0].RecruitIndex = 0;

//            Name = name;
//            //Sex = sex;

//            if (generateID) {
//                id = PlayerID.GeneratePlayerID();
//            }

//            Team[0].Name = name;
//            Team[0].Sex = sex;
//            Team[0].Species = 0;
//            Team[0].Sprite = Settings.NewCharSprite;
//            Team[0].Level = 1;
//            Team[0].CalculateOriginalStats();
//            //mTeam[0].Str = Classes.ClassManager.Classes[classNum].Str;
//            //mTeam[0].Def = Classes.ClassManager.Classes[classNum].Def;
//            //mTeam[0].Spd = Classes.ClassManager.Classes[classNum].Speed;
//            //mTeam[0].SpclAttack = Classes.ClassManager.Classes[classNum].PP;
//            Team[0].HP = Team[0].MaxHP;

//            MapID = MapManager.GenerateMapID(Settings.StartMap);
//            X = Settings.StartX;
//            Y = Settings.StartY;

//            mMaxInv = 20;
//            mMaxBank = 100;

//            // if (Map <= 0) Map = 1;
//            if (X < 0 || X > Settings.MaxMapX) X = Settings.MaxMapX / 2;
//            if (Y < 0 || Y > Settings.MaxMapY) Y = Settings.MaxMapY / 2;



//            Scripting.ScriptManager.InvokeSub("CharAdded", client, name, charNum);

//            mLoaded = true;
//            Save();
//            mLoaded = false;

//            if (silent == false) {
//                PlayerManager.AddToCharList(name, mLogin, charNum, CharID);
//                LoadChars();
//                Messenger.SendChars(client);
//                Messenger.PlainMsg(client, "Character has been created!", Enums.PlainMsgType.Chars);
//            }
//        }

//        public void DeleteChar(int charNum) {
//            if (System.IO.Directory.Exists(IO.Paths.AccountFolder + mLogin + "\\Char" + charNum.ToString())) {
//                try {
//                    System.IO.Directory.Delete(IO.Paths.AccountFolder + mLogin + "\\Char" + charNum.ToString(), true);
//                } catch { }
//                PlayerManager.DeleteFromCharList(mCharName[charNum]);
//                AddChar("", Enums.Sex.Male, charNum, true, false);
//            }
//        }

//        #endregion

//        #region Loading

//        public bool CharExists(int charNum) {
//            if (mCharName[charNum].Trim() != "")
//                return true;
//            else
//                return false;
//        }

//        internal bool PasswordOK(string login, string password) {
//            if (IO.IO.DirectoryExists(IO.Paths.AccountFolder + login) == true) {
//                string loginGood = login + "blah";
//                string passGood = password + "blah";
//                using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(IO.Paths.AccountFolder + login + "\\info.xml"))) {
//                    while (reader.Read()) {
//                        if (reader.IsStartElement()) {
//                            switch (reader.Name) {
//                                case "Login": {
//                                        loginGood = reader.ReadString();
//                                    }
//                                    break;
//                                case "Password": {
//                                        passGood = reader.ReadString();
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                }
////#if DEBUG
//                //return true;
////#else
//                if ((login.ToLower() == loginGood.ToLower()) && (password == passGood))
//                    return true;
//                else
//                    return false;
////#endif
//            } else
//                return false;
//        }

//        internal void LoadChars() {
//            for (int i = 1; i <= 3; i++) {
//                if (IO.IO.DirectoryExists(IO.Paths.AccountFolder + mLogin + "\\Char" + i.ToString()) && IO.IO.FileExists(IO.Paths.AccountFolder + mLogin + "\\Char" + i.ToString() + "\\info.xml")) {
//                    bool nameFound = false;
//                    using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(IO.Paths.AccountFolder + mLogin + "\\Char" + i.ToString() + "\\info.xml"))) {
//                        while (reader.Read() && nameFound == false) {
//                            if (reader.IsStartElement()) {
//                                switch (reader.Name) {
//                                    case "Name": {
//                                            mCharName[i] = reader.ReadString();
//                                            nameFound = true;
//                                        }
//                                        break;
//                                }
//                            }
//                        }
//                    }
//                } else {
//                    AddChar("", Enums.Sex.Male, i, true, false);
//                    mCharName[i] = "";
//                }
//            }
//        }

//        public void LoadStories() {
//            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "Story\\data.xml"))) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "Chapter": {
//                                    int num = reader["num"].ToInt(-1);
//                                    if (num > -1 && reader.Read()) {
//                                        if (Stories.ContainsKey(num)) {
//                                            Stories[num] = reader.ReadElementString("Completed").ToBool();
//                                        } else {
//                                            Stories.Add(num, reader.ReadElementString("Completed").ToBool());
//                                        }
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//        }

//        public void LoadMissions() {
//            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "Missions\\data.xml"))) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "Mission": {
//                                    int num = reader["num"].ToInt(-1);
//                                    if (num > -1 && reader.Read()) {
//                                        if (Missions.ContainsKey(num)) {
//                                            Missions[num] = reader.ReadElementString("Completed").ToBool();
//                                        } else {
//                                            Missions.Add(num, reader.ReadElementString("Completed").ToBool());
//                                        }
//                                    }
//                                }
//                                break;
//                            case "MissionExp": {
//                                    MissionExp = reader.ReadString().ToInt();
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//            while (ExplorerRank != Enums.ExplorerRank.Guildmaster && MissionExp >= MissionManager.DetermineMissionExpRequirement(ExplorerRank + 1)) {
//                ExplorerRank++;
//            }
//        }

//        public void LoadInventory() {
//            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "inventory.xml"))) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "MaxInv": {
//                                    mMaxInv = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "InvItem": {
//                                    if (Inventory.Count < mMaxInv + 1) {
//                                        InventoryItem invItem = new InventoryItem();
//                                        using (XmlReader subReader = reader.ReadSubtree()) {
//                                            invItem.Load(subReader);
//                                        }
//                                        Inventory.Add(Inventory.Count + 1, invItem);
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//        }

//        public void LoadBank() {
//            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "Bank\\bank.xml"))) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "MaxBank": {
//                                    mMaxBank = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "Item": {
//                                    if (Bank.Count <= mMaxBank) {
//                                        InventoryItem invItem = new InventoryItem();
//                                        using (XmlReader subReader = reader.ReadSubtree()) {
//                                            invItem.Load(subReader);
//                                        }
//                                        Bank.Add(Bank.Count + 1, invItem);
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//        }

        //public void LoadInfoFile() {
        //    using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "info.xml"))) {
        //        while (reader.Read()) {
        //            if (reader.IsStartElement()) {
        //                switch (reader.Name) {
        //                    #region General
        //                    case "ID": {
        //                            id = reader.ReadString();
        //                        }
        //                        break;
        //                    case "Name": {
        //                            Name = reader.ReadString();
        //                        }
        //                        break;
        //                    case "Access": {
        //                            Access = (Enums.Rank)(reader.ReadString().ToInt());
        //                        }
        //                        break;
        //                    //case "Sex": {
        //                    //        Sex = (Enums.Sex)(reader.ReadString().ToInt());
        //                    //    }
        //                    //    break;
        //                    case "PK": {
        //                            PK = reader.ReadString().ToBool();
        //                        }
        //                        break;
        //                    case "Solid": {
        //                            Solid = reader.ReadString().ToBool();
        //                        }
        //                        break;
        //                    case "Map": {
        //                            MapID = reader.ReadString();
        //                        }
        //                        break;
        //                    case "X": {
        //                            X = reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    case "Y": {
        //                            Y = reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    case "Status": {
        //                            Status = reader.ReadString();
        //                        }
        //                        break;
        //                    //case "RDungeonNum": {
        //                    //        dungeonIndex = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "RDungeonFloor": {
        //                    //        dungeonFloor = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    case "AvailableExpKitModules": {
        //                            string[] modules = reader.ReadString().Split(';');
        //                            AvailableExpKitModules = new AvailableExpKitModuleCollection(modules.Length);
        //                            AddEnabledExpKitModules();
        //                            for (int i = 0; i < modules.Length; i++) {
        //                                if (modules[i].IsNumeric()) {
        //                                    if (AvailableExpKitModules.Contains((Enums.ExpKitModules)modules[i].ToInt()) == false) {
        //                                        AvailableExpKitModules.Add(new AvailableExpKitModule((Enums.ExpKitModules)modules[i].ToInt(), false));
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    case "InTempMode": {
        //                            usingTempStats = reader.ReadString().ToBool();
        //                        }
        //                        break;
        //                    case "Veteran": {
        //                            veteran = reader.ReadString().ToBool();
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Equips
        //                    //case "Armor": {
        //                    //        Armor = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Weapon": {
        //                    //        Weapon = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Helmet": {
        //                    //        Helmet = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Shield": {
        //                    //        Shield = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Leg": {
        //                    //        Legs = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Necklace": {
        //                    //        Necklace = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    //case "Ring": {
        //                    //        Ring = reader.ReadString().ToInt();
        //                    //    }
        //                    //    break;
        //                    #endregion
        //                    #region Team
        //                    case "ActiveSlot": {
        //                            ActiveSlot = reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    case "TeamSlot": {
        //                            int num = reader["num"].ToInt(-1);
        //                            int recruitNum = reader.ReadString().ToInt(-1);
        //                            if (num > -1 && (recruitNum > -1 || recruitNum == -2)) {
        //                                Team[num] = new Recruit(client);
        //                                AddToTeam(recruitNum, num);
        //                            }
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Guild
        //                    case "GuildName": {
        //                            GuildName = reader.ReadString();
        //                        }
        //                        break;
        //                    case "GuildAccess": {
        //                            GuildAccess = (Enums.GuildRank)reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Story
        //                    case "CurrentChapter": {
        //                            int chapter = reader.ReadString().ToInt(-1);
        //                            if (chapter > -1) {
        //                                mCurrentChapter = StoryManager.Stories[chapter];
        //                            } else {
        //                                mCurrentChapter = null;
        //                            }
        //                        }
        //                        break;
        //                    case "CurrentSegment": {
        //                            mCurrentSegment = reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Friends
        //                    case "Friend": {
        //                            mFriends.Add(reader.ReadString());
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Dungeon Data
        //                    case "DungeonInfo": {
        //                            int id = reader["id"].ToInt(-1);
        //                            if (id > -1 && reader.Read()) {
        //                                mDungeonCompletionCounts.Add(id, reader.ReadElementString("CompletionCount").ToInt());
        //                            }
        //                        }
        //                        break;
        //                    #endregion
        //                    #region Pokedex Data
        //                    case "Pokemon":
        //                        {
        //                            int id = reader["id"].ToInt(-1);
        //                            if (id > -1 && reader.Read())
        //                            {
        //                                mPokedex.Add(id, (Enums.PokedexStat)reader.ReadElementString("Status").ToInt());
        //                            }
        //                        }
        //                        break;
        //                    #endregion
        //                }
        //            }
        //        }
        //    }

//            #region Data Verification
//            // Verify the recruits
//            bool found = false;
//            for (int n = 0; n < Constants.MAX_ACTIVETEAM; n++) {
//                if (Team[n] == null) {
//                    Team[n] = new Recruit(client);
//                }
//                if (Team[n].Loaded == true)
//                    found = true;
//            }
//            if (found == false)
//                AddToTeam(0, 0);

//            if (Team[ActiveSlot].Loaded == false) {
//                ActiveSlot = 0;
//            }

//            //Check to make sure that they aren't on map 0, if so reset'm
//            //if (Map <= 0 && Map != -2) {
//            //    Map = 1;
//            //    X = 8;
//            //    Y = 8;
//            //}
//            if (MapID.IsNumeric()) {
//                int mapNum = MapID.ToInt();
//                if (mapNum == -2) {
//                    MapID = MapManager.GenerateMapID(Settings.Crossroads);
//                } else {
//                    MapID = "s" + MapID;
//                }
//            }
//            //if (Armor == 0) Armor = -1;
//            //if (Shield == 0) Shield = -1;
//            //if (Helmet == 0) Helmet = -1;
//            //if (Weapon == 0) Weapon = -1;
//            //if (Legs == 0) Legs = -1;
//            //if (Necklace == 0) Necklace = -1;
//            //if (Ring == 0) Ring = -1;
//            if (string.IsNullOrEmpty(this.id)) {
//                id = PlayerID.GeneratePlayerID();
//            }
//            PlayerID.AddPlayerToIndexList(id, client.TcpID);

//            #endregion
//        }

//        public void LoadWonderMail() {
//            JobList.LoadData();
//            MissionBoard.LoadMissionBoardData();
//        }

//        public void LoadTriggerEvents() {
//            if (IO.IO.FileExists(GetCharFolder() + "triggerevents.xml") == false) {
//                SaveTriggerEvents();
//            }
//            using (XmlReader reader = XmlReader.Create(GetCharFolder() + "triggerevents.xml")) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "TriggerEvent": {
//                                    Server.Events.Player.TriggerEvents.TriggerEventTrigger type = (Server.Events.Player.TriggerEvents.TriggerEventTrigger)Enum.Parse(typeof(Server.Events.Player.TriggerEvents.TriggerEventTrigger), reader["type"], true);
//                                    Server.Events.Player.TriggerEvents.ITriggerEvent triggerEvent = Events.Player.TriggerEvents.TriggerEventHelper.CreateTriggerEventInstance(type);
//                                    triggerEvent.Load(reader.ReadSubtree(), client);
//                                    triggerEvents.Add(triggerEvent);
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//        }

//        public void Load(int charNum) {
//            mCharNum = charNum;

//            CheckFolders();

//            LoadInventory();
//            LoadInfoFile();
//            LoadMissions();
//            LoadStories();
//            LoadBank();
//            LoadWonderMail();
//            LoadTriggerEvents();
//            storyHelper.LoadSettings();

//            statistics = new Statistics.Statistics();
//            statistics.HandleLogin(this.mOSVersion, this.GetDotNetVersion(), this.client.MacAddress, this.client.IP);
//            statistics.Load(GetCharFolder() + "statistics.xml");

//            mLoaded = true;
//        }

//        #endregion

//        #region Recruits

//        public Recruit GetActiveRecruit() {
//            return Team[ActiveSlot];
//        }

//        public Recruit GetRecruit(int teamSlot) {
//            return Team[teamSlot];
//        }

//        public int GetRecruitIndex(int teamSlot) {
//            return Team[teamSlot].RecruitIndex;
//        }

//        public void SwapActiveRecruit(int teamSlot) {
//            SwapActiveRecruit(teamSlot, false);
//        }

        //public void SwapActiveRecruit(int teamSlot, bool ignoreChecks) {
        //    if (CanSwapRecruit(teamSlot) || ignoreChecks) {
        //        if (Team[teamSlot].Loaded && teamSlot != ActiveSlot) {
        //            int oldSlot = ActiveSlot;
        //            ScriptManager.InvokeSub("BeforeRecruitActiveCharSwap", client, oldSlot, teamSlot);
        //            ActiveSlot = teamSlot;
        //            PacketHitList hitList = null;
        //            PacketHitList.MethodStart(ref hitList);
        //            hitList.AddPacket(client, TcpPacket.CreatePacket("activecharswap", teamSlot.ToString()));
        //            PacketBuilder.AppendBelly(client, hitList);
        //            PacketBuilder.AppendStats(client, hitList);
        //            PacketBuilder.AppendPlayerData(client, hitList);
        //            PacketBuilder.AppendPlayerMoves(client, hitList);
        //            Party playerParty = PartyManager.FindPlayerParty(client);
        //            if (playerParty != null) {
        //                PacketBuilder.AppendPartyMemberData(client, hitList, playerParty.Members.GetMemberSlot(CharID));
        //            }
        //            PacketHitList.MethodEnded(ref hitList);
        //            // *** Old code, without PacketHitList
        //            //Messenger.SendDataTo(client, TcpPacket.CreatePacket("activecharswap", teamSlot.ToString()));
        //            //Messenger.SendBelly(client);
        //            //Messenger.SendStats(client);
        //            //Messenger.SendPlayerData(client);
        //            //Messenger.SendPlayerMoves(client);
        //            //Party playerParty = PartyManager.FindPlayerParty(client);
        //            //if (playerParty != null) {
        //            //    Messenger.SendPartyMemberData(playerParty, playerParty.Members.FindMember(CharID), playerParty.Members.GetMemberSlot(CharID));
        //            //}
        //            // *** End old code, without PacketHitList
        //            ScriptManager.InvokeSub("RecruitActiveCharSwap", client, oldSlot, teamSlot);
        //            PauseTimer = new TickCount(Core.GetTickCount().Tick + 1000);

//                }
//            } else if (!Core.GetTickCount().Elapsed(PauseTimer, 0)) {
//                Messenger.PlayerMsg(client, "You can't switch out right now.", Text.BrightRed);
//            } else {
//                Messenger.PlayerMsg(client, "You can't switch out!", Text.BrightRed);
//            }
//        }

//        public void SwapLeader(int teamSlot) {
//            if (client.Player.Team[teamSlot].Loaded) {
//                if (client.Player.Team[teamSlot].RecruitIndex != -2) {
//                    Recruit oldLeader = Team[0];
//                    Recruit newLeader = Team[teamSlot];
//                    Scripting.ScriptManager.InvokeSub("BeforeRecruitLeaderChanged", client, oldLeader, newLeader);
//                    client.Player.Team[0] = newLeader;
//                    client.Player.Team[teamSlot] = oldLeader;
//                    Messenger.SendActiveTeam(client);
//                    Messenger.SendStats(client);
//                    Messenger.SendPlayerMoves(client);
//                    Messenger.SendPlayerData(client);
//                    Messenger.SendAllRecruits(client);
//                    Scripting.ScriptManager.InvokeSub("RecruitLeaderChanged", client, teamSlot);
//                } else {
//                    Messenger.PlayerMsg(client, "You cannot make temporary Pokemon your team leader!", Text.BrightRed);
//                }
//            } else {
//                Messenger.PlayerMsg(client, "Error: Pokémon not loaded.", Text.BrightRed);
//            }
//        }

//        public bool CanSwapRecruit(int teamSlot) {
//            if (!CanSwapActiveRecruit) {
//                return false;
//            }
//            if (partyID != null) {
//                Party party = PartyManager.FindParty(partyID);
//                if (party != null) {
//                    switch (party.Members.Count) {
//                        case 2: {
//                                if (teamSlot == 2 || teamSlot == 3) {
//                                    return false;
//                                }
//                            }
//                            break;
//                        case 3:
//                        case 4: {
//                                if (teamSlot != 0) {
//                                    return false;
//                                }
//                            }
//                            break;
//                    }
//                }
//            }
//            return true;
//        }

//        public int GetActiveRecruitSlot() {
//            return ActiveSlot;
//        }

//        public bool IsInTeam(int recruitIndex) {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null) {
//                    if (Team[i].RecruitIndex == recruitIndex)
//                        return true;
//                }
//            }
//            return false;
//        }

//        public int FindTeamSlot(int recruitIndex) {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null) {
//                    if (Team[i].RecruitIndex == recruitIndex)
//                        return i;
//                }
//            }
//            return -1;
//        }

//        public int FindOpenTeamSlot() {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] == null || Team[i].RecruitIndex == -1) {
//                    return i;
//                }
//            }
//            return -1;
//        }

//        public bool RecruitExists(int recruitIndex) {
//            return System.IO.File.Exists(GetCharFolder() + "Recruits\\" + recruitIndex.ToString() + ".xml");
//        }

//        public int GetTeamSize() {
//            int total = 0;
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null) {
//                    if (Team[i].RecruitIndex > -1) {
//                        total += Team[i].Size;
//                    }
//                }
//            }
//            return total;
//        }

//        public void AddToTeam(int recruitIndex, int teamSlot) {
//            if (((recruitIndex > -1 && IsInTeam(recruitIndex) == false && RecruitExists(recruitIndex)) || recruitIndex == -2) && (!(teamSlot < 0) || !(teamSlot > 3))) {
//                Team[teamSlot] = new Recruit(client);
//                Team[teamSlot].Load(GetCharFolder(), recruitIndex, teamSlot);
//                if (recruitIndex != -2) {
//                    ScriptManager.InvokeSub("RecruitAddedToTeam", client, teamSlot, recruitIndex);
//                }
//            }
//        }

        //public void RemoveFromTeam(int teamSlot) {
        //    if (teamSlot > 0 && teamSlot < 4 && Team[teamSlot].Loaded) {
        //        ScriptManager.InvokeSub("BeforeRecruitRemovedFromTeam", client, teamSlot);
        //        if (ActiveMission != null) {
        //            if (ActiveMission.WonderMail.MissionType == Enums.MissionType.Escort) {
        //                if (Team[teamSlot].RecruitIndex == -2 && !missionComplete) {
        //                    Messenger.PlayerMsg(client, Team[teamSlot].Name + " has went home. The mission has ended.", Text.Yellow);
        //                    if (Settings.Scripting) {
        //                        Scripting.ScriptManager.InvokeSub("OnMissionFailed", client, ActiveMission);
        //                    }
        //                    for (int i = 0; i < JobList.JobList.Count; i++) {
        //                        JobList.JobList[i].Accepted = false;
        //                    }
        //                    ActiveMission = null;
        //                    Messenger.SendMissionComplete(client);
        //                    Messenger.SendJobList(client);
        //                }
        //            }
        //        }
        //        if (teamSlot == ActiveSlot) {
        //            SwapActiveRecruit(0, true);
        //        }
        //        if (Team[teamSlot].RecruitIndex > -1) {
        //            if (Team[teamSlot].InTempMode) {
        //                RestoreRecruitStats(teamSlot);
        //            } else {
        //                Team[teamSlot].Save(GetCharFolder(), teamSlot);
        //            }
        //        }
        //        Team[teamSlot] = new Recruit(client);
        //        Messenger.SendActiveTeam(client);
        //    }
        //}

//        internal void AddToRequested(MapNpc npc) {
//            if (mRequestedRecruit == null) {
//                mRequestedRecruit = new Recruit(client);
//            }
//            mRequestedRecruit.Name = npc.Name;
//            mRequestedRecruit.Species = npc.Species;
//            Pokedex.Pokemon pokemon = Pokedex.Pokedex.GetPokemon(npc.Species);

//            //if (pokemon != null) {
//            //    if (pokemon.ID != NpcRec.Species) {
//            //        mRequestedRecruit.SpeciesOverride = NpcRec.Species;
//            //    }
//            //}
//            mRequestedRecruit.Sprite = NpcManager.Npcs[npc.Num].Sprite;
//            if (npc.Level <= 0) {
//                mRequestedRecruit.Level = 1;//NpcRec.RecruitLevel;
//            } else {
//                mRequestedRecruit.Level = npc.Level;
//            }
//            mRequestedRecruit.Exp = 0;
//            mRequestedRecruit.IQ = 0;
//            mRequestedRecruit.NpcBase = npc.Num;
//            mRequestedRecruit.AtkBonus = npc.AtkBonus;
//            mRequestedRecruit.DefBonus = npc.DefBonus;
//            mRequestedRecruit.SpclAtkBonus = npc.SpclAtkBonus;
//            mRequestedRecruit.SpclDefBonus = npc.SpclDefBonus;
//            mRequestedRecruit.SpdBonus = npc.SpdBonus;
//            mRequestedRecruit.MaxHPBonus = npc.MaxHPBonus;
//            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
//                mRequestedRecruit.Moves[i].MoveNum = npc.Moves[i].MoveNum;
//                mRequestedRecruit.Moves[i].MaxPP = npc.Moves[i].MaxPP;
//            }
            
            
//            if (pokemon != null) {
//                Messenger.AskQuestion(client, "RecruitPokemon", mRequestedRecruit.Name + " wants to join your team! Will you accept it?", pokemon.Mugshot);
//            } else {
//                Messenger.AskQuestion(client, "RecruitPokemon", mRequestedRecruit.Name + " wants to join your team! Will you accept it?", -1);
//            }
//            //Program.mResponder.SendRecruitAddRequest(client, mRequestedRecruit);
//        }

//        private int FindOpenRecruitmentSlot() {
//            string Folder = GetCharFolder() + "Recruits\\";
//            for (int i = 1; i <= Constants.MAX_RECRUITMENTS; i++) {
//                if (System.IO.File.Exists(Folder + i.ToString() + ".xml") == false) {
//                    return i;
//                }
//            }
//            return -1;
//        }

//        public int AddToRecruitmentBank(Recruit requestedRecruit) {
//            string Folder = GetCharFolder() + "Recruits\\";
//            int index = FindOpenRecruitmentSlot();
//            string FileName = Folder + index + ".xml";
//            if (index > -1 && index <= Constants.MAX_RECRUITMENTS) {
                
//                PokemonCaught(requestedRecruit.Species);

//                Recruit recruit = new Recruit(null);
//                recruit.RecruitIndex = index;
//                recruit.Sprite = requestedRecruit.Sprite;
//                recruit.Level = requestedRecruit.Level;
//                recruit.Species = requestedRecruit.Species;
//                recruit.CalculateOriginalStats();
//                recruit.Exp = requestedRecruit.Exp;
//                recruit.IQ = requestedRecruit.IQ;
//                //recruit.HP = recruit.MaxHP;
//                recruit.Name = requestedRecruit.Name;
//                recruit.NpcBase = requestedRecruit.NpcBase;
//                recruit.AtkBonus = requestedRecruit.AtkBonus;
//                    recruit.DefBonus = requestedRecruit.DefBonus;
//                    recruit.SpclAtkBonus = requestedRecruit.SpclAtkBonus;
//                    recruit.SpclDefBonus = requestedRecruit.SpclDefBonus;
//                    recruit.SpdBonus = requestedRecruit.SpdBonus;
//                    recruit.MaxHPBonus = requestedRecruit.MaxHPBonus;
            		
//                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
//                        recruit.Moves[i].MoveNum = requestedRecruit.Moves[i].MoveNum;
//                        recruit.Moves[i].MaxPP = requestedRecruit.Moves[i].MaxPP;
//                        recruit.Moves[i].CurrentPP = recruit.Moves[i].MaxPP;
//                    }
//                recruit.Save(GetCharFolder(), -1);
//                //Return the index of the recruited player
//                return index;
//            }
//            //Return -1 so we know we're full
//            return -1;
//        }

//        public void ReleaseRecruit(int recruitIndex) {
//            if (recruitIndex > 0) {
//                string Folder = GetCharFolder() + "Recruits\\";
//                string File = Folder + recruitIndex + ".xml";
//                if (System.IO.File.Exists(File)) {
//                    if (IsInTeam(recruitIndex)) {
//                        Messenger.PlayerMsg(client, "This pokemon is in your team! You can't release it!", Text.BrightRed);
//                        return;
//                    }
//                    DeleteFromRecruitmentBank(recruitIndex, true);
//                    Messenger.PlayerMsg(client, "Pokemon released!", Text.BrightRed);
//                    ScriptManager.InvokeSub("RecruitReleased", client, recruitIndex);
//                    Messenger.SendAllRecruits(client);
//                    //Messenger.OpenAssembly(client);
//                }
//            }
//        }

//        public void DeleteFromRecruitmentBank(int recruitIndex, bool RemoveFromTeamSlot) {
//            if (recruitIndex > 0) {
//                if (RemoveFromTeamSlot) {
//                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                        if (Team[i].RecruitIndex == recruitIndex) {
//                            RemoveFromTeam(i);
//                        }
//                    }
//                }
//                string FileName = GetCharFolder() + "Recruits\\" + recruitIndex + ".xml";
//                if (System.IO.File.Exists(FileName)) {
//                    System.IO.File.Delete(FileName);
//                }
//            } else if (recruitIndex == 0) {
//                Messenger.PlayerMsg(client, "You can't release your starter pokemon!", Text.BrightRed);
//            } else {
//                Messenger.PlayerMsg(client, "Error: Unable to release selected pokemon", Text.BrightRed);
//            }
//        }

//        public void ChangeRecruitName(int teamSlot, string newName) {

//            if (teamSlot > -1 && teamSlot < 4) {
//                if (newName != "") {
//                    if (Team[teamSlot].RecruitIndex > -1) {
//                        Team[teamSlot].Name = newName;
//                        Team[teamSlot].Nickname = true;
//                        //Messenger.OpenAssembly(client);

//                    }
//                } else {
//                    if (Team[teamSlot].RecruitIndex > -1) {
//                        Team[teamSlot].Name = Pokedex.Pokedex.GetPokemon(Team[teamSlot].Species).Name;
//                        Team[teamSlot].Nickname = false;
//                        //Messenger.OpenAssembly(client);

//                    }
//                }
//                Messenger.SendActiveTeam(client);
//            }
//            /*else {
//                if (newName != "")
//                {
//                    using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "Recruits\\" + recruitIndex.ToString() + ".xml"), Settings.XmlWriterSettings))
//                    {
//                        writer.
//                        writer.WriteElementString("Name", newName);
//                        writer.WriteElementString("IsNickname", "true");


//                    }
//                }
//                else
//                {
//                    int sprite = 0;
//                    int speciesOverride = 0;
//                    using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(GetCharFolder() + "Recruits\\" + recruitIndex.ToString() + ".xml")))
//                    {
//                        while (reader.Read())
//                        {
//                            if (reader.IsStartElement())
//                            {
//                                switch (reader.Name)
//                                {
//                                    case "Sprite":
//                                        {
//                                            sprite = reader.ReadString().ToInt();

//                                        }
//                                        break;
//                                    case "SpeciesOverride":
//                                        {
//                                            speciesOverride = reader.ReadString().ToInt();
//                                        }
//                                        break;

//                                }
//                            }
//                        }
//                    }

//                    using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(GetCharFolder() + "Recruits\\" + recruitIndex.ToString() + ".xml"), Settings.XmlWriterSettings))
//                    {
//                        writer.WriteElementString("Name", Pokedex.Pokedex.GetPokemon(Recruit.DetermineSpecies(sprite, speciesOverride)).Name);
//                        writer.WriteElementString("IsNickname", "false");
//                    }
//                }
//            }
//            */

//            Messenger.SendAllRecruits(client);

//        }


//        public int GetLevelBonus() {
//            int level = GetActiveRecruit().Level;
//            if (level >= 1 && level < 30) {
//                return 0;
//            } else if (level > 29 && level < 40) {
//                return 50;
//            } else if (level > 39 && level < 50) {
//                return 75;
//            } else if (level > 49 && level < 60) {
//                return 100;
//            } else if (level > 59 && level < 70) {
//                return 125;
//            } else if (level > 69 && level < 80) {
//                return 150;
//            } else if (level > 79 && level < 90) {
//                return 175;
//            } else if (level > 89 && level < 101) {
//                return 250;
//            } else {
//                return 0;
//            }
//        }

//        public int GetRecruitBonus() {
//            return GetActiveRecruit().RecruitBoost + GetLevelBonus();
//        }

//        public void AddToTeamTemp(int teamSlot, int npcNum, int level) {
//            if (teamSlot > -1 && teamSlot < 4 && npcNum > 0 && npcNum <= NpcManager.Npcs.MaxNpcs) {
//                Team[teamSlot].RecruitIndex = -2;
//                Team[teamSlot].Loaded = true;
//                Pokedex.Pokemon pokemon = Pokedex.Pokedex.FindBySprite(Npcs.NpcManager.Npcs[npcNum].Sprite);
//                if (pokemon != null) {
//                    if (pokemon.ID != Npcs.NpcManager.Npcs[npcNum].Species) {
//                        Team[teamSlot].SpeciesOverride = Npcs.NpcManager.Npcs[npcNum].Species;
//                    }
//                }
//                Team[teamSlot].Sprite = Npcs.NpcManager.Npcs[npcNum].Sprite;
//                Team[teamSlot].Species = pokemon.ID;
//                if (level == -1) {
//                    client.Player.Team[teamSlot].Level = 1;// Npcs.NpcManager.Npcs[npcNum].RecruitLevel;
//                } else {
//                    client.Player.Team[teamSlot].Level = level;
//                }
//                client.Player.Team[teamSlot].Name = NpcManager.Npcs[npcNum].Name;
//                client.Player.Team[teamSlot].Nickname = true;
//                client.Player.Team[teamSlot].Size = 1;
//                //client.Player.Team[teamSlot].Level = NpcManager.Npcs[npcNum].RecruitLevel;
//                client.Player.Team[teamSlot].Exp = 0;
//                client.Player.Team[teamSlot].IQ = 0;
//                client.Player.Team[teamSlot].CalculateOriginalStats();
//                client.Player.Team[teamSlot].HP = client.Player.Team[teamSlot].MaxHP;
//            }
//        }

//        #endregion

//        #region Inventory/Equips

//        public bool IsInvFull() {
//            if (FindInvSlot(-1) == -1) {
//                return true;
//            } else {
//                return false;
//            }
//        }

//        //public bool IsEquiptable(int itemNum) {
//        //    if (ItemManager.Items.ContainsItem(itemNum)) {
//        //        switch (ItemManager.Items[itemNum].Type) {
//        //            case Enums.ItemType.Armor:
//        //            case Enums.ItemType.Weapon:
//        //            case Enums.ItemType.Helmet:
//        //            case Enums.ItemType.Shield:
//        //            case Enums.ItemType.Legs:
//        //            case Enums.ItemType.Ring:
//        //            case Enums.ItemType.Necklace:
//        //                return true;
//        //            default:
//        //                return false;
//        //        }
//        //    } else
//        //        return false;
//        //}

//        public void CheckAllHeldItems() {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null) {
//                    Team[i].CheckHeldItem();
//                }
//            }
//        }

//        public int GetItemSlotHolder(int itemSlot) {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null && Team[i].HeldItemSlot == itemSlot) {
//                    return i;
//                }
//            }

//            return -1;
//        }

//        public bool HasItemHeld(int itemNum) {
//            return HasItemHeld(itemNum, false);
//        }

//        public bool HasItemHeld(int itemNum, bool stickyCheck) {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null && Team[i].HeldItem != null && Team[i].HeldItem.Num == itemNum && (!stickyCheck || !Team[i].HeldItem.Sticky)) {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public bool HasItemHeldBy(int itemNum, Enums.PokemonType type) {
//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null && Team[i].HeldItem != null && Team[i].HeldItem.Num == itemNum && (Team[i].Type1 == type || Team[i].Type2 == type)) {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public void TakeItem(int itemnum, int ItemVal) {
//            TakeItem(itemnum, ItemVal, true);

//        }

//        public void TakeItem(int itemnum, int ItemVal, bool ignoreSticky) {
//            int i = 0;
//            //Enums.ItemType n = Enums.ItemType.None;
            


//            // Check for subscript out of range
//            if (client.IsPlaying() == false | itemnum <= 0 | itemnum > ItemManager.Items.MaxItems) {
//                return;
//            }

//            for (i = 1; i <= Inventory.Count; i++) {
//                if (Inventory.ContainsKey(i)) {
//                    // Check to see if the player has the item
//                    if (Inventory[i].Num == itemnum) {

//                        TakeItemSlot(i, ItemVal, ignoreSticky);

//                        return;
//                    }
//                }
//            }
//        }

//        public void TakeItemSlot(int slot, int itemVal, bool ignoreSticky) {
//            if (Inventory.ContainsKey(slot) && Inventory[slot].Num > 0)
//            {
//                bool TakeItem = false;
//                int itemNum = Inventory[slot].Num;
//                if (ignoreSticky || !Inventory[slot].Sticky)
//                {
//                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.Currency || ItemManager.Items[itemNum].Stackable == true)
//                    {
//                        // Is what we are trying to take away more then what they have?  If so just set it to zero
//                        if (itemVal >= Inventory[slot].Val)
//                        {
//                            TakeItem = true;
//                        }
//                        else
//                        {
//                            Inventory[slot].Val = Inventory[slot].Val - itemVal;
//                            Messenger.SendInventoryUpdate(client, slot);
//                        }
//                    }
//                    else
//                    {


//                        TakeItem = true;

//                        //n = ItemManager.Items[Inventory[i].Num].Type;
//                        // Check if its not an equipable weapon, and if it isn't then take it away
//                        //if ((n != Enums.ItemType.Weapon) && (n != Enums.ItemType.Armor) && (n != Enums.ItemType.Helmet) && (n != Enums.ItemType.Shield) && (n != Enums.ItemType.Legs) & (n != Enums.ItemType.Ring) & (n != Enums.ItemType.Necklace)) {
//                        //    TakeItem = true;
//                        //}
//                    }

//                    if (TakeItem == true)
//                    {

//                        // Check to see if its held by anyone

//                        if (GetItemSlotHolder(slot) > -1)//remove from all equippers
//                        {
//                            RemoveItem(slot, true, false);
//                            Messenger.SendWornEquipment(client);
//                        }

//                        Inventory[slot].Num = 0;
//                        Inventory[slot].Val = 0;
//                        Inventory[slot].Sticky = false;
//                        Inventory[slot].Tag = "";
//                        //mInventory[i].Dur = 0;

//                        //check if it's held-in-bag, and if it was the last functional copy
//                        if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0)
//                        {
//                            for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++)
//                            {
//                                if (Team[j] != null)
//                                {
//                                    //the function auto-checks if it's in the activeItemlist to begin with
//                                    Team[j].RemoveFromActiveItemList(itemNum);
//                                }

//                            }
//                        }



//                        // Send the inventory update
//                        Messenger.SendInventoryUpdate(client, slot);

//                    }
//                }
//            }
//        }

//        public void GiveItem(int itemnum, int ItemVal) {
//            // Check for subscript out of range
//            GiveItem(itemnum, ItemVal, false);
//        }
        
//        public void GiveItem(int itemnum, int itemVal, string tag) {
//            // Check for subscript out of range
//            GiveItem(itemnum, itemVal, tag, false);
//        }
        
//        public void GiveItem(int itemnum, int itemVal, bool sticky) {
//            // Check for subscript out of range
//            GiveItem(itemnum, itemVal, "", sticky);
//        }

//        public void GiveItem(int itemnum, int ItemVal, string tag, bool sticky) {
//            // Check for subscript out of range
//            if (client.IsPlaying() == false | itemnum <= 0 | itemnum > ItemManager.Items.MaxItems) {
//                return;
//            }
//            int i = FindInvSlot(itemnum);
//            if (i == -1) {
//                // Check to see if inventory is full
//                //if (IsInvFull() == false) {
//                //    InventoryItem inv = new InventoryItem();
//                //    inv.Num = itemnum;
//                //    inv.Val = ItemVal;
//                //    Inventory.Add(Inventory.Count, inv);
//                //    i = FindInvSlot(itemnum);
//                //} else {
//                Messenger.PlayerMsg(client, "Your inventory is full.", Text.BrightRed);
//                //}
//            } else {
//                Inventory[i].Num = itemnum;
//                if (ItemManager.Items[itemnum].Stackable || ItemManager.Items[itemnum].Type == Enums.ItemType.Currency) {
//                    Inventory[i].Val += ItemVal;
//                } else {
//                    Inventory[i].Val = 0;
//                }

//                Inventory[i].Sticky = sticky;
//                Inventory[i].Tag = tag;

//                if (ItemManager.Items[itemnum].Type == Enums.ItemType.HeldInBag && !sticky) {
//                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                        if (Team[j] != null && Team[j].MeetsReqs(itemnum)) {
//                            //the function auto-checks if it's in the activeItemlist to begin with
//                            Team[j].AddToActiveItemList(itemnum);
//                        }

//                    }
//                }
//                Messenger.SendInventoryUpdate(client, i);
//            }
//            //if (IsEquiptable(itemnum) && i > -1) {
//            //    mInventory[i].Dur = Program.ClassMan.mItems[itemnum].Data1;
//            //}
//        }


//        public int FindInvSlot(int itemNum) {
//            if (itemNum != -1) {
//                if (ItemManager.Items[itemNum].Stackable || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
//                    for (int i = 1; i <= Inventory.Count; i++) {
//                        if (Inventory[i].Num == itemNum) {
//                            return i;
//                        }
//                    }
//                }
//            }

//            //search for any empty space
//            for (int i = 1; i <= Inventory.Count; i++) {
//                if (Inventory[i].Num == 0) {
//                    return i;
//                }
//            }
//            return -1;
//        }

//        public int FindBankSlot(int itemNum) {
//            if (itemNum != -1) {
//                if (ItemManager.Items[itemNum].Stackable || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
//                    for (int i = 1; i <= Bank.Count; i++) {
//                        if (Bank[i].Num == itemNum) {
//                            return i;
//                        }
//                    }
//                }
//            }

//            for (int i = 1; i <= Bank.Count; i++) {
//                if (Bank[i].Num == 0) {
//                    return i;
//                }
//            }
//            return -1;
//        }

//        public int HasItem(int itemNum) {
//            return HasItem(itemNum, false);
//        }

//        public int HasItem(int itemNum, bool stickyCheck) {
//            if (client.IsPlaying() == false || itemNum < 0 || itemNum > ItemManager.Items.MaxItems) {
//                return 0;
//            }

//            for (int i = 1; i <= Inventory.Count; i++) {
//                if (Inventory[i].Num == itemNum && (!stickyCheck || !Inventory[i].Sticky)) {
//                    if (ItemManager.Items[Inventory[i].Num].Type == Enums.ItemType.Currency || ItemManager.Items[Inventory[i].Num].Stackable == true) {
//                        return Inventory[i].Val;
//                    } else {
//                        return 1;
//                    }
//                }
//            }
//            return 0;
//        }

//        public void HoldItem(int invNum)//only for active recruit
//        {
//            int ownItem = -1;
//            if (GetItemSlotHolder(invNum) > -1) {
//                if (Inventory[invNum].Sticky) {

//                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[invNum].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
//                    return;
//                } else {
//                    RemoveItem(invNum, false, false);

//                }
//            }



//            //remove any item the active recruit may already be holding
//            if (GetActiveRecruit().HeldItemSlot != -1) {
//                //make sure the already held item isn't sticky
//                if (GetActiveRecruit().HeldItem.Sticky) {
//                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[GetActiveRecruit().HeldItem.Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
//                    return;
//                } else {
//                    ownItem = GetActiveRecruit().HeldItemSlot;
//                    RemoveItem(GetActiveRecruit().HeldItemSlot, false, false);

//                }
//            }
//            GetActiveRecruit().HeldItemSlot = invNum;

//            if (!Inventory[invNum].Sticky) {

//                int itemNum = Inventory[invNum].Num;

//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetActiveRecruit().MeetsReqs(itemNum)) {
//                    GetActiveRecruit().AddToActiveItemList(itemNum);

//                }

//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty) {
//                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                        if (Team[i] != null && Team[i].MeetsReqs(itemNum)) {

//                            Team[i].AddToActiveItemList(itemNum);
//                        }
//                    }
//                }

//            }
//            Messenger.SendWornEquipment(client);
//            if (ownItem != -1) {
//                Messenger.SendInventoryUpdate(client, ownItem);
//            }
//            Messenger.SendInventoryUpdate(client, invNum);

//        }

//        public void RemoveItem(int invNum) {

//            RemoveItem(invNum, false);
//        }

//        public void RemoveItem(int invNum, bool ignoreSticky) {

//            RemoveItem(invNum, ignoreSticky, true);
//        }

//        public void RemoveItem(int invNum, bool ignoreSticky, bool updateItem)//has an extra check to dequip item from EVERY recruit, just in case multiple recruits somehow equipped the same invNum
//        {
//            // Prevent hacking
//            if (invNum < 1 || invNum > mMaxInv) {
//                Messenger.HackingAttempt(client, "Invalid InvNum");
//                return;
//            }


//            if (Inventory[invNum].Sticky && !ignoreSticky) {
//                Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[invNum].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
//                return;
//            }

//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (Team[i] != null && Team[i].HeldItemSlot == invNum) {

//                    Team[i].HeldItemSlot = -1;
//                    int itemNum = Inventory[invNum].Num;

//                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held) {
//                        Team[i].RemoveFromActiveItemList(itemNum);

//                    }

//                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty && !HasItemHeld(itemNum, true)) {
//                        for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                            if (Team[j] != null) {

//                                Team[j].RemoveFromActiveItemList(itemNum);
//                            }
//                        }
//                    }

//                }
//            }
//            if (updateItem) {
//                Messenger.SendWornEquipment(client);
//                Messenger.SendInventoryUpdate(client, invNum);
//            }

//        }

//        public void SetItemSticky(int invNum, bool sticky) {
//            // Prevent hacking
//            if (invNum < 1 || invNum > mMaxInv) {
//                Messenger.HackingAttempt(client, "Invalid InvNum");
//                return;
//            }

//            if (sticky && !Inventory[invNum].Sticky) {
//                Inventory[invNum].Sticky = true;

//                int itemNum = Inventory[invNum].Num;
//                //held item
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetItemSlotHolder(invNum) > -1) {

//                    Team[GetItemSlotHolder(invNum)].RemoveFromActiveItemList(itemNum);

//                }

//                //held-in-party, check for remaining functional copies
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty) {


//                    if (!HasItemHeld(itemNum, true)) {
//                        for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {


//                            Team[i].RemoveFromActiveItemList(itemNum);

//                        }
//                    }
//                }

//                //held-in-bag item, check for remaining functional copies
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0) {
//                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                        //the following function auto-checks if it's in the activeItemlist to begin with
//                        Team[j].RemoveFromActiveItemList(itemNum);


//                    }
//                }

//            } else if (!sticky && Inventory[invNum].Sticky)//cleanse
//            {
//                Inventory[invNum].Sticky = false;
//                int itemNum = Inventory[invNum].Num;
//                //held item
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetItemSlotHolder(invNum) > -1) {
//                    if (Team[GetItemSlotHolder(invNum)].MeetsReqs(itemNum)) {

//                        Team[GetItemSlotHolder(invNum)].AddToActiveItemList(itemNum);
//                    }
//                }

//                //held-in-party
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty && GetItemSlotHolder(invNum) > -1) {
//                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                        if (Team[i] != null && Team[i].MeetsReqs(itemNum)) {

//                            Team[i].AddToActiveItemList(itemNum);
//                        }
//                    }
//                }

//                //held-in-bag item
//                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag) {
//                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                        if (Team[j] != null && Team[j].MeetsReqs(itemNum)) {
//                            //the following function auto-checks if it's in the activeItemlist to begin with
//                            Team[j].AddToActiveItemList(itemNum);
//                        }

//                    }
//                }

//            }

//            Messenger.SendInventoryUpdate(client, invNum);

//        }

        

//        public void SetMaxInv(int max) {
//            mMaxInv = max;
//            for (int i = 1; i <= mMaxInv; i++) {
//                if (!Inventory.ContainsKey(i)) {
//                    Inventory.Add(i, new InventoryItem());
//                }
//            }
//            Messenger.SendInventory(client);
//            //really that easy?
//        }

//        #endregion

//        #region Bank

//        public void TakeBankItem(int itemnum, int ItemVal) {
//            int i = 0;
//            //int n = 0;
//            bool TakeBankItem = false;

//            // Check for subscript out of range
//            if (client.IsPlaying() == false | itemnum <= 0 | itemnum > ItemManager.Items.MaxItems) {
//                return;
//            }

//            for (i = 1; i <= mMaxBank; i++) {
//                // Check to see if the player has the item
//                if (Bank[i].Num == itemnum) {
//                    if (ItemManager.Items[itemnum].Type == Enums.ItemType.Currency || ItemManager.Items[itemnum].Stackable == true) {
//                        // Is what we are trying to take away more then what they have? If so just set it to zero
//                        if (ItemVal >= Bank[i].Val) {
//                            TakeBankItem = true;
//                        } else {
//                            Bank[i].Val -= ItemVal;
//                            Messenger.SendBankUpdate(client, i);
//                        }
//                    } else {
//                        // Check to see if its any sort of ArmorSlot/WeaponSlot ~unneeded for takebankitem




//                        // Check if its not an equipable weapon, and if it isn't then take it away ~ irrelevant

//                        TakeBankItem = true;

//                    }

//                    if (TakeBankItem == true) {
//                        Bank[i].Num = 0;
//                        Bank[i].Val = 0;
//                        Bank[i].Sticky = false;
//                        Bank[i].Tag = "";
//                        //mBank[i].Dur = 0;

//                        // Send the Bank update
//                        Messenger.SendBankUpdate(client, i);
//                        return;
//                    }
//                }
//            }
//        }

//        public void GiveBankItem(int itemnum, int ItemVal, int BankSlot) {
//            int i = 0;

//            // Check for subscript out of range
//            if (itemnum <= 0 || itemnum > ItemManager.Items.MaxItems) {
//                return;
//            }

//            i = BankSlot;

//            // Check to see if Bankentory is full
//            if (i != -1) {
//                Bank[i].Num = itemnum;
//                Bank[i].Val += ItemVal;
//            } else {
//                Messenger.StorageMessage(client, "Storage full!");
//            }
//        }

//        public void SetMaxBank(int max) {
//            mMaxBank = max;
//            for (int i = 1; i <= mMaxBank; i++) {
//                if (!Bank.ContainsKey(i)) {
//                    Bank.Add(i, new InventoryItem());
//                }
//            }
//            //no need to send bank
//            //really that easy?
//        }


//        #endregion

//        #region Stats

//        public int GetMaxHP() {
//            return GetActiveRecruit().MaxHP;
//        }





//        public void SetHP(int HP) {
//            SetHP(ActiveSlot, HP);
//        }

//        public void SetHP(int teamSlot, int HP) {
//            Team[teamSlot].HP = HP;

//        }

        //public void ProcessHunger(PacketHitList hitlist) {
        //    if (BellyStepCounter >= 100) {
        //        if (GetActiveRecruit().Belly > 20) {
        //            GetActiveRecruit().Belly -= (BellyStepCounter / 100);
        //            if (GetActiveRecruit().Belly <= 20) {
        //                hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Getting hungry...", Text.Red));
        //            }
        //            PacketBuilder.AppendBelly(client, hitlist);

        //        } else if (GetActiveRecruit().Belly > 10) {
        //            GetActiveRecruit().Belly -= (BellyStepCounter / 100);
        //            if (GetActiveRecruit().Belly <= 10) {
        //                hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Getting dizzy from hunger...", Text.Red));
        //            }
        //            PacketBuilder.AppendBelly(client, hitlist);

        //        } else if (GetActiveRecruit().Belly > 0) {
        //            GetActiveRecruit().Belly -= (BellyStepCounter / 100);
        //            if (GetActiveRecruit().Belly <= 0) {
        //                GetActiveRecruit().Belly = 0;
        //                hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Oh no!  Your belly is empty!", Text.Red));
        //            }
        //            PacketBuilder.AppendBelly(client, hitlist);
        //        }
        //        BellyStepCounter = BellyStepCounter % 100;
        //    }
        //}


        //#endregion

//        #region Friends List

        //public void SendFriendsList() {
        //    TcpPacket packet = new TcpPacket("friendslist");
        //    packet.AppendParameter(mFriends.Count);
        //    {
        //        for (int i = 0; i <= mFriends.Count - 1; i++) {
        //            Client clientIsOn = ClientManager.FindClient(mFriends[i]);
        //            int ison;
        //            if (clientIsOn != null) {
        //                ison = 1;
        //            } else {
        //                ison = 0;
        //            }
        //            packet.AppendParameters(mFriends[i], ison.ToString());
        //        }
        //    }
        //    packet.FinalizePacket();
        //    Messenger.SendDataTo(client, packet);
        //    if (Settings.Scripting == true) {
        //        ScriptManager.InvokeSub("OnSendFriendsList", client);
        //    }
        //}

        //public void AppendFriendsList(PacketHitList hitlist) {
        //    TcpPacket packet = new TcpPacket("friendslist");
        //    packet.AppendParameter(mFriends.Count);
        //    {
        //        for (int i = 0; i <= mFriends.Count - 1; i++) {
        //            Client clientIsOn = ClientManager.FindClient(mFriends[i]);
        //            int ison;
        //            if (clientIsOn != null) {
        //                ison = 1;
        //            } else {
        //                ison = 0;
        //            }
        //            packet.AppendParameters(mFriends[i], ison.ToString());
        //        }
        //    }
        //    packet.FinalizePacket();
        //    hitlist.AddPacket(client, packet);
        //    if (Settings.Scripting == true) {
        //        ScriptManager.InvokeSub("OnSendFriendsList", client);
        //    }
        //}

//        public void AddFriend(string friendName) {
//            Client pIndex = ClientManager.FindClient(friendName.Trim());
//            string pName;
//            if (pIndex != null) {
//                pName = pIndex.Player.Name.Trim();
//                if (pName.Trim().ToLower() == Name.Trim().ToLower()) {
//                    Messenger.PlayerMsg(client, "You can't add yourself!", Text.BrightRed);
//                    return;
//                }
//            } else {
//                Messenger.PlayerMsg(client, "Player is not online", Text.White);
//                return;
//            }

//            if (mFriends.Count < 20) {
//                if (mFriends.Contains(pName) == false) {
//                    mFriends.Add(pName);
//                    SendFriendsList();
//                    Messenger.PlayerMsg(client, pName + " has been added!", Text.Yellow);
//                    if (Settings.Scripting == true) {
//                        ScriptManager.InvokeSub("FriendAdded", client, pName);
//                    }
//                } else {
//                    Messenger.PlayerMsg(client, "Player is already on your friends list!", Text.BrightRed);
//                }
//            } else {
//                Messenger.PlayerMsg(client, "Friends list full", Text.BrightRed);
//            }
//        }

//        public void RemoveFriend(string friendName) {
//            if (mFriends.Contains(friendName)) {
//                mFriends.Remove(friendName);
//                SendFriendsList();
//                Messenger.PlayerMsg(client, friendName + " has been removed!", Text.Yellow);
//                if (Settings.Scripting == true) {
//                    ScriptManager.InvokeSub("FriendRemoved", client, friendName);
//                }
//            } else {
//                Messenger.PlayerMsg(client, "Unable to find " + friendName + " on your friends list", Text.BrightRed);
//            }
//        }

//        public bool IsFriend(string friendName) {
//            return mFriends.Contains(friendName.Trim());
//        }


//        #endregion

//        #region Missions

//        public void CancelActiveJob() {
//            if (ActiveMission != null) {
//                int activeSlot = -1;
//                for (int i = 0; i < JobList.JobList.Count; i++) {
//                    if (JobList.JobList[i].Accepted) {
//                        activeSlot = i;
//                        break;
//                    }
//                }
//                if (activeSlot > -1) {
//                    JobList.JobList[activeSlot].Accepted = false;
//                }
//                if (ActiveMission.WonderMail.MissionType == Enums.MissionType.Escort) {
//                    RemoveFromTeam(FindTeamSlot(-2));
//                    Messenger.SendActiveTeam(client);
//                }
//                ActiveMission = null;
//                Messenger.SendCancelJob(client);
//                Messenger.SendJobList(client);
//                Messenger.PlayerMsg(client, "You have cancelled your job!", Text.Yellow);
//            }
//        }

//        #endregion

//        #region Maps

//        public IMap GetCurrentMap() {
//            return Map;
//        }
        
//        public void MapDropItem(int InvNum, int Amount) {
//            MapDropItem(InvNum, Amount, null);
//        }

//        public void MapDropItem(int InvNum, int Amount, Client playerFor) {
//            int i = 0;
//            IMap map = GetCurrentMap();
//            bool TakeItem = false;

//            // Check for subscript out of range
//            if (InvNum < 0 | InvNum > mMaxInv) {
//                return;
//            }



//            if ((Inventory[InvNum].Num > 0) && (Inventory[InvNum].Num <= ItemManager.Items.MaxItems)) {

//                //check to make sure player is not trying to drop an equipped sticky item
//                if (Inventory[InvNum].Sticky && GetItemSlotHolder(InvNum) > -1) {
//                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[InvNum].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
//                    return;
//                }

//                i = map.FindOpenItemSlot();

//                if (i != -1) {

//                    // Check to see if its any sort of ArmorSlot/WeaponSlot
//                    //if (GetItemSlotHolder(InvNum) > -1)
//                    //{
//                    //    Team[GetItemSlotHolder(i)].HeldItemSlot = -1;
//                    //    Messenger.SendWornEquipment(client);

//                    //}
//                    int itemNum = Inventory[InvNum].Num;

//                    if (ItemManager.Items[itemNum].Bound) {
//                        Messenger.PlayerMsg(client, "This item cannot be dropped.", Text.BrightRed);
//                        return;
//                    }
//                    //map.ActiveItem[i].Num = itemNum;
//                    //map.ActiveItem[i].X = X;
//                    //map.ActiveItem[i].Y = Y;
//                    //map.ActiveItem[i].PlayerFor = "";
//                    //map.ActiveItem[i].TimeDropped = new TickCount(0);
//                    bool sticky = Inventory[InvNum].Sticky;
//                    string tag = Inventory[InvNum].Tag;

//                    string Msg;

//                    if (Inventory[InvNum].Sticky) {
//                        Msg = "x" + ItemManager.Items[itemNum].Name.Trim();
//                    } else {
//                        Msg = ItemManager.Items[itemNum].Name.Trim();
//                    }

//                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.Currency || ItemManager.Items[itemNum].Stackable == true) {
//                        // Check if its more then they have and if so drop it all
//                        if (Amount >= Inventory[InvNum].Val) {
//                            map.ActiveItem[i].Value = Inventory[InvNum].Val;
//                            Messenger.MapMsg(MapID, Name + " drops " + Inventory[InvNum].Val + " " + Msg + ".", Text.Yellow);
//                            TakeItem = true;
//                        } else {
//                            map.ActiveItem[i].Value = Amount;
//                            Messenger.MapMsg(MapID, Name + " drops " + Amount + " " + Msg + ".", Text.Yellow);
//                            Inventory[InvNum].Val = Inventory[InvNum].Val - Amount;
//                        }
//                    } else {
//                        // Its not a currency object so this is easy
//                        map.ActiveItem[i].Value = 0;
//                        Messenger.MapMsg(MapID, Name + " drops a " + Msg + ".", Text.Yellow);

//                        TakeItem = true;
//                    }

//                    if (TakeItem == true) {

//                        // Check to see if its held by anyone

//                        if (GetItemSlotHolder(InvNum) > -1)//remove from all equippers
//                        {
//                            RemoveItem(InvNum, false, false);
//                            Messenger.SendWornEquipment(client);
//                        }



//                        Inventory[InvNum].Num = 0;
//                        Inventory[InvNum].Val = 0;
//                        Inventory[InvNum].Sticky = false;
//                        Inventory[InvNum].Tag = "";
//                        //mInventory[i].Dur = 0;

//                        //check for held-in-bag

//                        //check to see if there aren't any functional copies left in bag
//                        if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0) {
//                            for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                                if (Team[j] != null) {
//                                    //the function auto-checks if it's in the activeItemlist to begin with
//                                    Team[j].RemoveFromActiveItemList(itemNum);
//                                }

//                            }
//                        }
//                    }

//                    // Send inventory update
//                    Messenger.SendInventoryUpdate(client, InvNum);

//                    // Spawn the item before we set the num or we'll get a different free map item slot
//                    map.SpawnItemSlot(i, itemNum, Amount, sticky, false, tag, X, Y, playerFor);
//                } else {
//                    Messenger.PlayerMsg(client, "Too many items already on the ground.", Text.BrightRed);
//                }
//            }
//        }

//        public void MapGetItem() {
//            int i = 0;
//            int n = 0;
//            string Msg = null;

//            IMap map = GetCurrentMap();

//            for (i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
//                // See if theres even an item here
//                if ((map.ActiveItem[i].Num > 0) & (map.ActiveItem[i].Num <= Items.ItemManager.Items.MaxItems)) {
//                    // Check if item is at the same location as the player
//                    if ((map.ActiveItem[i].X == X) & (map.ActiveItem[i].Y == Y)) {
//                        if (string.IsNullOrEmpty(map.ActiveItem[i].PlayerFor) || CharID == map.ActiveItem[i].PlayerFor || Core.GetTickCount().Elapsed(map.ActiveItem[i].TimeDropped, 20000)) {
//                            // Find open slot
//                            if (ItemManager.Items[map.ActiveItem[i].Num].Rarity <= (int)ExplorerRank + 1) {
//                                n = FindInvSlot(map.ActiveItem[i].Num);

//                                // Open slot available?
//                                if (n != -1) {
//                                    // Set item in players inventory
//                                    Inventory[n].Num = map.ActiveItem[i].Num;
//                                    Inventory[n].Sticky = map.ActiveItem[i].Sticky;
//                                    Inventory[n].Tag = map.ActiveItem[i].Tag;

//                                    if (Inventory[n].Sticky) {
//                                        Msg = "x" + ItemManager.Items[Inventory[n].Num].Name.Trim();
//                                    } else {
//                                        Msg = ItemManager.Items[Inventory[n].Num].Name.Trim();
//                                    }
//                                    if (ItemManager.Items[Inventory[n].Num].Type == Enums.ItemType.Currency || ItemManager.Items[Inventory[n].Num].Stackable == true) {
//                                        Inventory[n].Val += map.ActiveItem[i].Value;
//                                        Msg = "You picked up " + map.ActiveItem[i].Value + " " + Msg + ".";
//                                    } else {
//                                        Inventory[n].Val = 0;

//                                        Msg = "You picked up a " + Msg + ".";
//                                    }
//                                    //mInventory[n].Dur = map.ActiveItem[i].Dur;

//                                    if (ItemManager.Items[Inventory[n].Num].Type == Enums.ItemType.HeldInBag) {
//                                        for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
//                                            if (Team[j] != null && Team[j].MeetsReqs(Inventory[n].Num)) {
//                                                //the function auto-checks if it's in the activeItemlist to begin with
//                                                Team[j].AddToActiveItemList(Inventory[n].Num);
//                                            }

//                                        }
//                                    }

//                                    // Erase item from the map ~ done in spawnitemslot

//                                    Messenger.SendInventoryUpdate(client, n);
//                                    map.SpawnItemSlot(i, 0, 0, false, false, "", X, Y, null);
//                                    Messenger.PlayerMsg(client, Msg, Text.Yellow);

                            //        if (Settings.Scripting) {
                            //            Scripting.ScriptManager.InvokeSub("OnPickupItem", GetActiveRecruit(), n, Inventory[n]);
                            //        }
                            //        return;
                            //    } else {
                            //        Messenger.PlayerMsg(client, "Your inventory is full.", Text.BrightRed);
                            //        return;
                            //    }
                            //} else {
                            //    Messenger.PlayerMsg(client, "You do not have a high enough Explorer Rank to receive this item.", Text.BrightRed);

//                                return;
//                            }
//                        } else {
//                            Messenger.PlayerMsg(client, "This item does not belong to you. Wait " + System.Math.Round((decimal)(map.ActiveItem[i].TimeDropped.Tick + 20000 - Core.GetTickCount().Tick) / 1000, 0) + " seconds before trying again.", Text.BrightRed);
//                            return;
//                        }

//                    }
//                }
//            }
//        }

        //public void RecreateSeenCharacters() {
        //    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
        //        seenNpcs[i] = new SeenCharacter();
        //        if (AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), Map.ActiveNpc[i])) {
        //            seenNpcs[i].InSight = true;
        //        }
        //        //new seencharacter
        //        //if canbeseen, then isseen
        //        //server cannot unsee
        //    }

        //    //new listpair
        //    seenPlayers = new ListPair<string, SeenCharacter>();
        //    foreach(Client i in Map.GetClients()) {
        //        seenPlayers.Add(i.Player.CharID, new SeenCharacter());
        //        if (AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), i.Player.GetActiveRecruit())) {
        //            seenPlayers[i.Player.CharID].InSight = true;
        //        }
        //        //add new seencharacter based on player
        //        //if canbeseen, then isseen
        //    }
        //}

        //public void RemoveUnseenCharacters() {
        //    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
        //        if (!AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), Map.ActiveNpc[i])) {
        //            seenNpcs[i].InSight = false;
        //        }
        //    }

        //    foreach (Client i in Map.GetClients()) {
        //        if (!AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), i.Player.GetActiveRecruit())) {
        //            seenPlayers[i.Player.CharID].InSight = false;
        //        }
        //    }
        //}

        

        //public void RefreshSeenCharacters(PacketHitList hitlist) {
        //    AddSeenCharacters(hitlist);
        //    RemoveUnseenCharacters();
        //}

        //public void SeeNewCharacter(Combat.ICharacter target) {
        //    if (target.CharacterType == Enums.CharacterType.Recruit) {
        //        if (!seenPlayers.ContainsKey(((Recruit)target).Owner.Player.CharID)) {
        //            seenPlayers.Add(((Recruit)target).Owner.Player.CharID, new SeenCharacter());
        //        }
        //        seenPlayers[((Recruit)target).Owner.Player.CharID].InSight = AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), target);
        //        seenPlayers[((Recruit)target).Owner.Player.CharID].LocationOutdated = false;
        //        seenPlayers[((Recruit)target).Owner.Player.CharID].ConditionOutdated = false;
        //    } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
        //        seenNpcs[((MapNpc)target).MapSlot].InSight = AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), target);
        //        seenNpcs[((MapNpc)target).MapSlot].LocationOutdated = false;
        //        seenNpcs[((MapNpc)target).MapSlot].ConditionOutdated = false;
        //    }
        //}

        //public void AddActivation(PacketHitList hitlist, Combat.ICharacter target) {
        //    SeenCharacter seencharacter;
        //    if (target.CharacterType == Enums.CharacterType.Recruit) {
        //        if (!seenPlayers.ContainsKey(((Recruit)target).Owner.Player.CharID)) {
        //            seenPlayers.Add(((Recruit)target).Owner.Player.CharID, new SeenCharacter());
        //            //SeenPlayers[((Recruit)target).Owner.Player.CharID].LocationOutdated = true;
        //            //SeenPlayers[((Recruit)target).Owner.Player.CharID].ConditionOutdated = true;
        //        }
        //        seencharacter = seenPlayers[((Recruit)target).Owner.Player.CharID];
        //        if (seencharacter.LocationOutdated) {
        //            //add location data
        //            hitlist.AddPacket(client, PacketBuilder.CreatePlayerXY(((Recruit)target).Owner));
        //            hitlist.AddPacket(client, PacketBuilder.CreatePlayerDir(((Recruit)target).Owner));
        //            seencharacter.LocationOutdated = false;
        //        }
        //        if (seencharacter.ConditionOutdated) {
        //            //add condition data
        //            hitlist.AddPacket(client, PacketBuilder.CreatePlayerStatusAilment(((Recruit)target).Owner));
        //            hitlist.AddPacket(client, PacketBuilder.CreatePlayerConfusion(((Recruit)target).Owner));
        //            hitlist.AddPacket(client, PacketBuilder.CreatePlayerVolatileStatus(((Recruit)target).Owner));
        //            seencharacter.ConditionOutdated = false;
        //        }
        //        //add activation param
        //        hitlist.AddPacket(client, PacketBuilder.CreatePlayerActivation(((Recruit)target).Owner));
        //        //set isseen to true
        //        seencharacter.InSight = true;
        //    } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
        //        seencharacter = seenNpcs[((MapNpc)target).MapSlot];
        //        if (seencharacter.LocationOutdated) {
        //            //add location data
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcXY(MapID, ((MapNpc)target).MapSlot));
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcDir(MapID, ((MapNpc)target).MapSlot));
        //            seencharacter.LocationOutdated = false;
        //        }
        //        if (seencharacter.ConditionOutdated) {
        //            //add condition data
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcHP(Map, ((MapNpc)target).MapSlot));
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcStatusAilment(MapID, ((MapNpc)target).MapSlot));
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcConfusion(MapID, ((MapNpc)target).MapSlot));
        //            hitlist.AddPacket(client, PacketBuilder.CreateNpcVolatileStatus(MapID, ((MapNpc)target).MapSlot));
        //            seencharacter.ConditionOutdated = false;
        //        }
        //        //add activation param
        //        hitlist.AddPacket(client, PacketBuilder.CreateNpcActivation(MapID, ((MapNpc)target).MapSlot));
        //        //set isseen to true
        //        seencharacter.InSight = true;
        //    }
        //}
        ////public void AddUpdate(PacketHitList hitlist, Combat.ICharacter target, Enums.OutdateType updateType) {
        ////    if (AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), target)) {
        
        ////    } else {

        ////    }
        ////}

        //public bool IsSeenCharacterSeen(Combat.ICharacter target) {
        //    if (target.CharacterType == Enums.CharacterType.Recruit) {
        //        if (!seenPlayers.ContainsKey(((Recruit)target).Owner.Player.CharID)) {
        //            return false;
        //        }
        //        if (!seenPlayers[((Recruit)target).Owner.Player.CharID].InSight) {
        //            return false;
        //        }
        //    } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
        //        if (Map.ActiveNpc[((MapNpc)target).MapSlot] != target) {
        //            return false;
        //        }
        //        if (!seenNpcs[((MapNpc)target).MapSlot].InSight) {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public void OutdateCharacter(Combat.ICharacter target, Enums.OutdateType outdateType) {
        //    SeenCharacter seencharacter = null;
        //    if (target.CharacterType == Enums.CharacterType.Recruit) {
        //        if (!seenPlayers.ContainsKey(((Recruit)target).Owner.Player.CharID)) {
        //            seenPlayers.Add(((Recruit)target).Owner.Player.CharID, new SeenCharacter());
        //        }
        //        seencharacter = seenPlayers[((Recruit)target).Owner.Player.CharID];
        //    } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
        //        //if (Map.ActiveNpc[((MapNpc)target).MapSlot] != target) {
        //        //    return false;
        //        //}
        //        seencharacter = seenNpcs[((MapNpc)target).MapSlot];
        //    }

        //    if (outdateType == Enums.OutdateType.Location) {
        //        seencharacter.LocationOutdated = true;
        //    } else if (outdateType == Enums.OutdateType.Condition) {
        //        seencharacter.ConditionOutdated = true;
        //    }
        //}
        ////when outdating something, be sure it exists

        //#endregion

//        #region Stories

//        public bool GetStoryState(int storyNum) {
//            return Stories[storyNum];
//        }

//        public void SetStoryState(int storyNum, bool value) {
//            Stories[storyNum] = value;
//        }

//        #endregion

//        #region System Info

//        public string GetOSVersion() {
//            return mOSVersion;
//        }

//        public string GetDotNetVersion() {
//            return mPlatformVersion.ToString();
//        }

//        public string GetClientEdition() {
//            return mClientEdition;
//        }

//        #endregion

//        #region Dungeons

//        public void SetDungeonCompletionCount(int dungeonIndex, int completionCount) {
//            if (mDungeonCompletionCounts.ContainsKey(dungeonIndex) == false) {
//                mDungeonCompletionCounts.Add(dungeonIndex, completionCount);
//            } else {
//                mDungeonCompletionCounts.SetValue(dungeonIndex, completionCount);
//            }
//        }

//        public int GetDungeonCompletionCount(int dungeonIndex) {
//            if (mDungeonCompletionCounts.ContainsKey(dungeonIndex)) {
//                return mDungeonCompletionCounts.GetValue(dungeonIndex);
//            } else {
//                return 0;
//            }
//        }

//        public void IncrementDungeonCompletionCount(int dungeonIndex, int incrementAmount) {
//            SetDungeonCompletionCount(dungeonIndex, GetDungeonCompletionCount(dungeonIndex) + incrementAmount);

//            if (ActiveMission != null) {
//                if (ActiveMission.WonderMail.MissionType == Enums.MissionType.CompleteDungeon) {
//                    if (ActiveMission.WonderMail.DungeonIndex == dungeonIndex) {
//                        HandleMissionComplete();
//                    }
//                }
//            }
//        }

//        public void AddDungeonAttempt(int dungeonIndex) {
//            if (mDungeonCompletionCounts.ContainsKey(dungeonIndex) == false) {
//                mDungeonCompletionCounts.Add(dungeonIndex, 0);
//            }
//        }

//        public int GetTotalDungeonCompletionCount()
//        {
//            int total = 0;

//            for (int i = 0; i < mDungeonCompletionCounts.Count; i++)
//            {
//                total += mDungeonCompletionCounts.ValueByIndex(i);
//            }

//            return total;
//        }

//        #endregion

//        #region Pokedex

//        public void PokemonSeen(int id)
//        {
//            if (id <= 0 || id > Constants.TOTAL_POKEMON) return;
//            if (mPokedex.ContainsKey(id) == false)
//            {
//                mPokedex.Add(id, Enums.PokedexStat.Defeated);
//            }
//        }

//        public void PokemonCaught(int id)
//        {
//            if (id <= 0 || id > Constants.TOTAL_POKEMON) return;
//            if (mPokedex.ContainsKey(id) == false)
//            {
//                mPokedex.Add(id, Enums.PokedexStat.Joined);
//            }
//            else if (mPokedex[id] == Enums.PokedexStat.Defeated)
//            {
//                mPokedex.SetValue(id, Enums.PokedexStat.Joined);
//            }
//        }

//        #endregion Pokedex

//        #region RDungeons

//        public void WarpToDungeon(int dungeonNum, int floor) {
//            //Program.ClassMan.mThreads.StartOnThread(new System.Threading.WaitCallback(WarpToDungeonBackground), new CParamObject(new object[] { dungeonNum, floor }));
//            //WarpToDungeonBackground(new ParamObject(dungeonNum, floor, RDungeonManager.RDungeons[dungeonNum].Options));
//            if (dungeonNum > -1 && floor > -1) {
//                //dungeonIndex = dungeonNum;
//                //dungeonFloor = floor;
//                RDungeonMap map = null;

//                if (IsInParty()) {
//                    foreach (Client i in PartyManager.Parties[partyID].Members.GetMemberClients()) {
//                        if (i != client && i.Player.Map.MapType == Enums.MapType.RDungeonMap) {
//                            if (((RDungeonMap)i.Player.Map).RDungeonIndex == dungeonNum && ((RDungeonMap)i.Player.Map).RDungeonFloor == floor) {

//                                map = (RDungeonMap)i.Player.Map;

//                                break;
//                            }
//                        }
//                    }
//                }
//                if (map == null) {
//                    map = RDungeonFloorGen.GenerateFloor(client, dungeonNum, floor, RDungeonManager.RDungeons[dungeonNum].Floors[floor].Options);
//                }

//                map.ProcessingPaused = true;
//                Messenger.SendDataTo(client, TcpPacket.CreatePacket("floorchangedisplay", map.Name, "1500"));
//                Messenger.PlayerWarp(client, map, map.StartX, map.StartY);
//                //map.SpawnItems();
//                //map.SpawnNpcs();
//                //  SendDungeonLoadedFloor(RDungeonFloorGen.GenerateFloor(client, Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]), (GeneratorOptions)param.Param[2]), Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]));
//                if (Settings.Scripting) {
//                    ScriptManager.InvokeSub("EnterRDungeon", client, dungeonNum, floor);
//                }
//            }
//        }

//        //private void WarpToDungeonBackground(Object data) {
//        //    try {
//        //        ParamObject param = data as ParamObject;
//        //        if (param != null) {
//        //            Messenger.SendDataTo(client, TcpPacket.CreatePacket("rdungeonfloorchange", RDungeonManager.RDungeons[dungeonNum].DungeonName, dungeonFloor, "1500"));
//        //            RDungeonMap map = RDungeonFloorGen.GenerateFloor(client, Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]), (GeneratorOptions)param.Param[2]);
//        //            Messenger.PlayerWarp(client, map, map.StartX, map.StartY);
//        //            //  SendDungeonLoadedFloor(RDungeonFloorGen.GenerateFloor(client, Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]), (GeneratorOptions)param.Param[2]), Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]));
//        //            if (Settings.Scripting) {
//        //                ScriptManager.InvokeSub("EnterRDungeon", client, Convert.ToInt32(param.Param[0]), Convert.ToInt32(param.Param[1]));
//        //            }
//        //        }
//        //    } catch (Exception ex) {
//        //        Exceptions.ErrorLogger.WriteToErrorLog(ex, "Sending Random Dungeon");
//        //    }
//        //}

        //public void SendDungeonLoadedFloor(InstancedMap map, int dungeonNum, int floor) {
//        //    if (map != null && dungeonNum != -1) {
//        //        if (Map != -2) {
//        //            int oldMap = Map;
//        //            Messenger.SendLeaveMap(client, oldMap);
//        //            // Now we check if there were any players left on the map the player just left, and if not stop processing npcs
//        //            if (MapManager.Maps[oldMap].GetTotalPlayers() == 0) {
//        //                MapManager.Maps[oldMap].PlayersOnMap = false;
//        //            }
//        //        }
//        //        DungeonFloor = floor;
//        //        DungeonIndex = dungeonNum;
//        //        Map = -2;
//        //        if (map.StartX != 0) {
//        //            X = map.StartX;
//        //        }
//        //        if (map.StartY != 0) {
//        //            Y = map.StartY;
//        //        }
//        //        mInstancedMapManager.ActiveMap = (InstancedMap)map;
//        //        string dungeonFloor;
//        //        if (RDungeonManager.RDungeons[dungeonNum].Direction == Enums.Direction.Up) {
//        //            dungeonFloor = (floor + 1).ToString() + "F";
//        //        } else {
//        //            dungeonFloor = "B" + (floor + 1).ToString() + "F";
//        //        }
//        //        Messenger.SendDataTo(client, Tcp.Packet.CreatePacket("rdungeonfloorchange", RDungeonManager.RDungeons[dungeonNum].DungeonName, dungeonFloor, "1500"));
//        //        map.SpawnNpcs();
//        //        map.SpawnItems();
//        //        Messenger.SendMap(client, map);
//        //        Messenger.SendMapNpcsTo(client);
//        //        Messenger.SendMapItemsTo(client);
//        //        Messenger.SendPlayerData(client);
//        //        Messenger.SendDataTo(client, Tcp.Packet.CreatePacket("mapdone"));
//        //        // Send map npc HP
//        //        for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
//        //            if (map.ActiveNpc[i].Num > 0) {
//        //                Messenger.SendDataTo(client, Tcp.Packet.CreatePacket("npchp", i, map.ActiveNpc[i].HP, NpcManager.GetMaxHP(map.ActiveNpc[i].Num, map.Npc[i].Level)));
//        //            }
//        //        }
//        //        Messenger.SendDataTo(client, Tcp.Packet.CreatePacket("WEATHER", ((int)Globals.ServerWeather).ToString(), RDungeonManager.RDungeons[dungeonNum].Floors[floor].WeatherIntensity.ToString()));
//        //        Messenger.SendPlayerXY(client);
//        //    } else {
//        //        Messenger.PlayerWarp(client, 1, 8, 8);
//        //    }
//        //}

//        #endregion RDungeons

//        #region Wonder Mail

//        public int SelectWonderMailDifficulty() {
//            if (mDungeonCompletionCounts.Count > 0) {
//                int rand = Server.Math.Rand(0, mDungeonCompletionCounts.Count);
//                int dungeonNum = mDungeonCompletionCounts.KeyByIndex(rand);
//                if (dungeonNum >= Dungeons.DungeonManager.Dungeons.Count) {
//                    return SelectWonderMailDifficulty();
//                }
//                return WonderMails.Generator.CalculateDungeonDifficulty(dungeonNum);
//            } else {
//                return 0;
//            }
//        }

//        internal void HandleMissionComplete() {
//            if (ActiveMission.WonderMail.EndStoryIndex > 0) {
//                StoryManager.PlayStory(client, ActiveMission.WonderMail.EndStoryIndex - 1);
//            } else {
//                Messenger.PlayerMsg(client, "You have completed the mission!", Text.Yellow);
//            }
//            JobList.CompletedMail.Add(ActiveMission.WonderMail.Code);
//            JobList.RemoveJob(ActiveMission.WonderMail.Code);
//            List<RewardItem> rewards = WonderMailSystem.RewardInfo[ActiveMission.WonderMail.RewardIndex].Rewards;
//            for (int i = 0; i < rewards.Count; i++) {
//                GiveItem(rewards[i].ItemNum, rewards[i].Amount);
//            }
//            if (ActiveMission.WonderMail.MissionExp == -1) {
//                MissionExp += MissionManager.DetermineMissionExpReward(ActiveMission.WonderMail.Difficulty);
//            } else {
//                MissionExp += ActiveMission.WonderMail.MissionExp;
//            }
//            MissionManager.ExplorerRankUp(client);
//            if (Settings.Scripting == true) {
//                Scripting.ScriptManager.InvokeSub("OnMissionComplete", client, ActiveMission);
//            }
//            ActiveMission = null;
//            Messenger.SendJobList(client);
//            Messenger.SendPlayerMissionExp(client);
//            Messenger.SendDataTo(client, TcpPacket.CreatePacket("missioncomplete"));
//        }

//        #endregion Wonder Mail

//        #region ExpKit

//        public void AddEnabledExpKitModules() {
//            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.Chat, false));
//            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.Counter, false));
//            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.FriendsList, false));

//            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
//                AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.MapReport, true));
//            }
//        }

//        public void SetActiveExpKitModule(Enums.ExpKitModules module) {
//            if (AvailableExpKitModules.Contains(module)) {
//                Messenger.SendDataTo(client, TcpPacket.CreatePacket("setactivekitmodule", (int)(module)));
//            }
//        }

//        public void AddExpKitModule(AvailableExpKitModule module) {
//            if (AvailableExpKitModules.Contains(module.Type) == false) {
//                AvailableExpKitModules.Add(module);
//                Messenger.SendAvailableExpKitModules(client);
//            }
//        }

//        public void RemoveExpKitModule(Enums.ExpKitModules module) {
//            if (AvailableExpKitModules.Contains(module)) {
//                AvailableExpKitModules.Remove(module);
//                Messenger.SendAvailableExpKitModules(client);
//            }
//        }

//        #endregion

//        #region Level x system

        //public void BeginTempStatMode(int level, bool keepMoves) {
        //    PacketHitList hitlist = null;
        //    PacketHitList.MethodStart(ref hitlist);

        //    if (usingTempStats == false) {
        //        usingTempStats = true;
        //        // First, save the current team
        //        for (int i = 0; i < Team.Length; i++) {
        //            if (Team[i].Loaded) {
        //                Team[i].InTempMode = true;
        //                Team[i].Save(GetCharFolder(), i);
        //            }
        //        }
        //        // Create the cache folder
        //        string tempStatModeCache = GetCharFolder() + "TempModeCache/";
        //        if (System.IO.Directory.Exists(tempStatModeCache)) {
        //            System.IO.Directory.Delete(tempStatModeCache, true);
        //        }
        //        System.IO.Directory.CreateDirectory(tempStatModeCache);
        //        // Now, copy the recruit files to the temp stat mode folder
        //        for (int i = 0; i < Team.Length; i++) {
        //            if (Team[i].Loaded) {
        //                string teamMemberPath;
        //                if (Team[i].RecruitIndex != -2) {
        //                    teamMemberPath = GetCharFolder() + "Recruits/" + Team[i].RecruitIndex + ".xml";
        //                } else {
        //                    teamMemberPath = GetCharFolder() + "Recruits/TempTeam/" + i + ".xml";
        //                }
        //                System.IO.File.Copy(teamMemberPath, tempStatModeCache + "team" + i + ".xml", true);
        //            }
        //        }
        //        // Now that the team is saved... We can set all members to level x
        //        for (int i = 0; i < Team.Length; i++) {
        //            if (Team[i].Loaded) {
        //                Team[i].Level = level;
        //                Team[i].Exp = 0;
        //                Team[i].CalculateOriginalStats();
        //                Team[i].HP = Team[i].MaxHP;
        //                if (!keepMoves)
        //                {
        //                    for (int move = 0; move < Team[i].Moves.Length; move++)
        //                    {
        //                        Team[i].Moves[move] = new RecruitMove();
        //                    }
        //                    Team[i].GenerateMoveset();
        //                }
        //            }
        //        }
        //        PacketBuilder.AppendPlayerData(client, hitlist);
        //        Messenger.SendStats(client);
        //        PacketBuilder.AppendEXP(client, hitlist);
                
        //        Messenger.SendPlayerMoves(client);
        //        Messenger.SendActiveTeam(client);
        //    }

        //    PacketHitList.MethodEnded(ref hitlist);
        //}


        //public void EndTempStatMode() {
        //    PacketHitList hitlist = null;
        //    PacketHitList.MethodStart(ref hitlist);

        //    if (usingTempStats) {
        //        string tempStatModeCache = GetCharFolder() + "TempModeCache/";
        //        for (int i = 0; i < Team.Length; i++) {
        //            if (Team[i].InTempMode) {
        //                if (System.IO.File.Exists(tempStatModeCache + "team" + i + ".xml")) {
        //                    int recruitIndex = Team[i].RecruitIndex;
        //                    Team[i] = new Recruit(client);
        //                    Team[i].LoadDirect(tempStatModeCache + "team" + i + ".xml", recruitIndex);
        //                    System.IO.File.Delete(tempStatModeCache + "team" + i + ".xml");
        //                    Team[i].InTempMode = false;
        //                    Team[i].Save(GetCharFolder(), i);
        //                }
        //            }
        //        }
        //        usingTempStats = false;
        //        SaveInfoFile();
        //        PacketBuilder.AppendPlayerData(client, hitlist);
        //        Messenger.SendStats(client);
        //        PacketBuilder.AppendEXP(client, hitlist);

        //        Messenger.SendPlayerMoves(client);
        //        Messenger.SendActiveTeam(client);
        //    }
            
        //    PacketHitList.MethodEnded(ref hitlist);
        //}

//        public void RestoreRecruitStats(int slot) {
//            if (Team[slot].InTempMode) {
//                string tempStatModeCache = GetCharFolder() + "TempModeCache/";
//                if (System.IO.File.Exists(tempStatModeCache + "team" + slot + ".xml")) {
//                    int recruitIndex = Team[slot].RecruitIndex;
//                    Team[slot] = new Recruit(client);
//                    Team[slot].LoadDirect(tempStatModeCache + "team" + slot + ".xml", recruitIndex);
//                    System.IO.File.Delete(tempStatModeCache + "team" + slot + ".xml");
//                    Team[slot].InTempMode = false;
//                    Team[slot].Save(GetCharFolder(), slot);
//                }
//            }
//        }

//        #endregion

//        #region Trading

//        public void RequestTrade(Client playerToTradeWith) {
//            if (playerToTradeWith.Player.CharID != this.CharID) {
//                playerToTradeWith.Player.tradePartner = this.CharID;
//                Messenger.AskQuestion(playerToTradeWith, "PlayerTradeRequest", "Would you like to trade with player " + this.Name + "?", Pokedex.Pokedex.GetPokemon(this.GetActiveRecruit().Species).Mugshot);
//            }
//        }

//        internal void EndTrade(bool manualEnd) {
//            client.Player.readyToTrade = false;
//            client.Player.setTradeItem = -1;
//            if (!string.IsNullOrEmpty(client.Player.tradePartner)) {
//                Client tradePartner = ClientManager.GetClient(PlayerID.FindTcpID(client.Player.tradePartner));
//                if (tradePartner != null && tradePartner.Player.tradePartner == client.Player.CharID) {
//                    tradePartner.Player.readyToTrade = false;
//                    tradePartner.Player.setTradeItem = -1;
//                    tradePartner.Player.tradePartner = null;
//                    Messenger.SendEndTrade(tradePartner);
//                }
//            }
//            client.Player.tradePartner = null;
//            if (manualEnd == false) {
//                Messenger.SendEndTrade(client);
//            }
//        }

//        public void EndTrade() {
//            EndTrade(false);
//        }

//        #endregion

//        #region Trigger Events

//        public void AddTriggerEvent(Events.Player.TriggerEvents.ITriggerEvent triggerEvent) {
//            if (triggerEvents.Contains(triggerEvent) == false) {
//                triggerEvents.Add(triggerEvent);
//            }
//        }

//        public void RemoveTriggerEvent(Events.Player.TriggerEvents.ITriggerEvent triggerEvent) {
//            if (triggerEvents.Contains(triggerEvent)) {
//                triggerEvents.Remove(triggerEvent);
//            }
//        }

//        public List<Events.Player.TriggerEvents.ITriggerEvent> TriggerEvents {
//            get { return this.triggerEvents; }
//        }

//        #endregion

//        #region Mail

//        #region Fields

//        Mail.Mailbox mailbox;

//        #endregion

//        #region Properties

//        public Mail.Mailbox Mailbox {
//            get { return mailbox; }
//        }

//        #endregion

//        #region Methods

//        public void LoadMailbox() {
//            this.mailbox = Mail.Mailbox.LoadMailbox(GetCharFolder() + "Mail/Mailbox.xml");
//        }

//        #endregion

//        #endregion

//        public bool IsInParty() {
//            return !string.IsNullOrEmpty(partyID);
//        }

//        public bool IsInSight(int x, int y) {
//            return (x >= X - 10 && x <= X + 10 &&
//                    y >= Y - 8 && y <= Y + 7);
//        }

//        public void BeginUninstancedWarp() {
//            uninstancedWarp = true;
//        }

//        public void EndUninstancedWarp() {
//            uninstancedWarp = false;
//        }


//    }
//}
