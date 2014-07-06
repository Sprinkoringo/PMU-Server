using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;
using Characters = DataManager.Characters;
using DataManager.Players;
using Server.Missions;

using PMU.Core;
using Server.Stories;
using Server.Maps;
using Server.Tournaments;
using Server.Statistics;
using System.Threading;
using Server.Database;

namespace Server.Players
{
    public partial class Player
    {
        #region Fields

        string accountName;
        PlayerData playerData;
        Client client;
        Recruit[] team;
        string partyID;
        List<GoalPoint> goalPoints;
        WonderMailTasks jobList;
        Recruit requestedRecruit;
        Enums.ExplorerRank explorerRank;
        string osVersion;
        string platformVersion;
        string clientEdition;
        bool uninstancedWarp;
        AvailableExpKitModuleCollection availableExpKitModules;
        Story currentChapter;
        bool loadingStory = false;
        StoryPlaybackCache storyPlaybackCache;
        StoryHelper storyHelper;
        Inventory inventory;
        Inventory bank;
        bool loggedIn;
        bool movementLocked;
        int segmentToStart;
        bool readyToTrade;
        string tradePartner;
        int setTradeItem;
        int setTradeAmount;
        string questionID;
        Server.Statistics.PlayerStatistics statistics;
        Server.WonderMails.MissionBoard missionBoard;
        Scripting.TimerCollection timers;
        CustomMenus.CustomMenuManager customMenuManager;
        Enums.ExpKitModules activeExpKitModule;
        List<Events.Player.TriggerEvents.ITriggerEvent> triggerEvents;
        ReaderWriterLockSlim dataLock;
        bool loaded;
        IMap cachedMap;

        Dictionary<string, Dictionary<int, SeenCharacter>> seenNpcs { get; set; }
        Dictionary<string, SeenCharacter> seenPlayers { get; set; }

        List<string> mapsToDelete = new List<string>();


        #endregion Fields

        #region Properties

        public bool SavingLocked {
            get;
            set;
        }

        public StoryHelper StoryHelper {
            get { return storyHelper; }
        }

        public bool Loaded {
            get { return loaded; }
        }

        public string Email {
            get {
                return playerData.Email;
            }
            set { playerData.Email = value; }
        }

        public bool LoggingOut {
            get;
            set;
        }

        public Enums.ExpKitModules ActiveExpKitModule {
            get { return activeExpKitModule; }
            internal set {
                activeExpKitModule = value;
            }
        }

        public CustomMenus.CustomMenuManager CustomMenuManager {
            get {
                return customMenuManager;
            }
        }

        public Recruit RequestedRecruit {
            get { return requestedRecruit; }
            internal set {
                requestedRecruit = value;
            }
        }

        public IExPlayer ExPlayer {
            get;
            set;
        }

        public Scripting.TimerCollection Timers {
            get { return timers; }
            internal set {
                timers = value;
            }
        }

        public bool InUninstancedWarp {
            get { return uninstancedWarp; }
            internal set {
                uninstancedWarp = value;
            }
        }

        public Server.WonderMails.MissionBoard MissionBoard {
            get { return missionBoard; }
        }

        public Server.Statistics.PlayerStatistics Statistics {
            get { return statistics; }
        }

        internal PlayerData PlayerData {
            get { return playerData; }
        }

        public AvailableExpKitModuleCollection AvailableExpKitModules {
            get { return availableExpKitModules; }
        }

        public string QuestionID {
            get { return questionID; }
            internal set {
                questionID = value;
            }
        }

        MatchUp tournamentMatchUp;
        public MatchUp TournamentMatchUp {
            get { return tournamentMatchUp; }
            internal set {
                tournamentMatchUp = value;
            }
        }
        Tournament tournament;
        public Tournaments.Tournament Tournament {
            get { return tournament; }
            internal set {
                tournament = value;
            }
        }
        MatchUp tournamentSpectatingMatchUp;
        public MatchUp TournamentSpectatingMatchUp {
            get { return tournamentSpectatingMatchUp; }
            set {
                tournamentSpectatingMatchUp = value;
            }
        }

        public int SetTradeItem {
            get { return setTradeItem; }
            internal set {
                setTradeItem = value;
            }
        }

        public int SetTradeAmount {
            get { return setTradeAmount; }
            internal set {
                setTradeAmount = value;
            }
        }

        public string TradePartner {
            get { return tradePartner; }
            internal set {
                tradePartner = value;
            }
        }

        public bool ReadyToTrade {
            get { return readyToTrade; }
            internal set {
                readyToTrade = value;
            }
        }

        //public int Confusion {
        //    get;
        //    set;
        //}

        public int BellyStepCounter {
            get;
            set;
        }

        public int HPStepCounter {
            get;
            set;
        }

        public bool GettingMap {
            get;
            set;
        }

        public Enums.Speed SpeedLimit { get { return GetActiveRecruit().SpeedLimit; } }

        public bool Veteran {
            get { return playerData.Veteran; }
            set { playerData.Veteran = value; }
        }

        public bool Solid {
            get { return playerData.Solid; }
            set { playerData.Solid = value; }
        }

        public int SegmentToStart {
            get { return segmentToStart; }
            internal set {
                segmentToStart = value;
            }
        }

        public bool MovementLocked {
            get { return movementLocked; }
            set { movementLocked = value; }
        }

        public Enums.GuildRank GuildAccess {
            get { return (Enums.GuildRank)playerData.GuildAccess; }
            set {
                playerData.GuildAccess = (byte)value;
            }
        }

        public string Status {
            get { return playerData.Status; }
            set {
                playerData.Status = value;
            }
        }

        //public bool PK {
        //    get { return playerData.PK; }
        //    set {
        //        playerData.PK = value;
        //    }
        //}

        public IMap Map {
            get {
                if (cachedMap == null || cachedMap.MapID != MapID) {
                    cachedMap = MapManager.RetrieveActiveMap(MapID);
                }

                return cachedMap;
            }
        }

        public bool Hunted {
            get;
            set;
        }

        public bool ProtectionOff {
            get;
            set;
        }

        public bool Dead {
            get { return playerData.Dead; }
            set { playerData.Dead = value; }
        }

        public bool LoggedIn {
            get { return loggedIn; }
            internal set { loggedIn = value; }
        }

        public string GuildName {
            get {
                return playerData.GuildName;
            }
            set {
                playerData.GuildName = value;
            }
        }

        public string PartyID {
            get { return partyID; }
            internal set {
                partyID = value;
            }
        }

        public bool Muted { get; set; }

        public StoryPlaybackCache StoryPlaybackCache {
            get { return storyPlaybackCache; }
        }

        public bool LoadingStory {
            get { return loadingStory; }
            internal set {
                loadingStory = value;
            }
        }

        public Story CurrentChapter {
            get {
                return currentChapter;
            }
            set {
                if (value != null) {
                    playerData.CurrentChapter = value.ID;
                } else {
                    playerData.CurrentChapter = "-1";
                }
                currentChapter = value;
            }
        }

        public int CurrentSegment {
            get { return playerData.CurrentSegment; }
            set { playerData.CurrentSegment = value; }
        }

        public Client Client {
            get { return client; }
        }

        public string Name {
            get { return playerData.Name; }
            set { playerData.Name = value; }
        }

        public int MaxInv {
            get { return playerData.MaxInv; }
            internal set {
                playerData.MaxInv = value;
            }
        }

        public int MaxBank {
            get { return playerData.MaxBank; }
            internal set {
                playerData.MaxBank = value;
            }
        }

        public bool CanSwapActiveRecruit { get; set; }

        public string AccountName {
            get { return accountName; }
            internal set {
                accountName = value;
            }
        }

        public string CharID {
            get { return playerData.CharID; }
        }

        public int MissionExp {
            get { return playerData.MissionExp; }
            set { playerData.MissionExp = value; }
        }

        public int ActiveSlot {
            get { return playerData.ActiveSlot; }
            set { playerData.ActiveSlot = value; }
        }

        public Inventory Inventory {
            get {
                return inventory;
            }
        }

        public Inventory Bank {
            get {
                return bank;
            }
        }

        public WonderMailTasks JobList {
            get { return jobList; }
        }

        public bool InMapEditor {
            get;
            set;
        }

        public bool InScriptEditor {
            get;
            set;
        }

        public TickCount AttackTimer { get; set; }
        public TickCount PauseTimer { get; set; }

        public Recruit[] Team {
            get { return team; }
        }

        public string MapID {
            get { return playerData.Map; }
            set {
                playerData.Map = value;
            }
        }

        public PlayerFriendsList Friends {
            get {
                // Load the friends list on demand!
                if (playerData.Friends.Loaded == false) {
                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                        PlayerDataManager.LoadPlayerFriendsList(dbConnection.Database, playerData);
                    }
                }
                return playerData.Friends;
            }
        }

        public int X {
            get { return playerData.X; }
            set { playerData.X = value; }
        }

        public int Y {
            get { return playerData.Y; }
            set { playerData.Y = value; }
        }

        public Enums.Direction Direction {
            get { return (Enums.Direction)playerData.Direction; }
            set { playerData.Direction = (byte)value; }
        }

        public Enums.ExplorerRank ExplorerRank {
            get { return explorerRank; }
            set { explorerRank = value; }
        }

        private ListPair<int, bool> StoryChapters {
            get {
                return playerData.StoryChapters;
            }
        }

        public List<GoalPoint> ActiveGoalPoints {
            get { return goalPoints; }
            internal set { goalPoints = value; }
        }

        public List<Events.Player.TriggerEvents.ITriggerEvent> TriggerEvents {
            get {
                return triggerEvents;
            }
        }

        internal ListPair<int, byte> RecruitList {
            get {
                return playerData.RecruitList;
            }
        }

        internal int MissionCompletions {
            get { return playerData.MissionCompletions; }
            set { playerData.MissionCompletions = value; }
        }

        internal ListPair<int, int> DungeonCompletionCounts {
            get {
                return playerData.DungeonCompletionCounts;
            }
        }

        public Enums.Rank Access {
            get { return (Enums.Rank)playerData.Access; }
            set {
                playerData.Access = (byte)value;
            }
        }

        #endregion Properties

        public bool LoadCharacter(DatabaseConnection dbConnection, int characterSlot) {
            dataLock.EnterReadLock();
            bool result = false;
            string characterID = PlayerDataManager.RetrieveAccountCharacterID(dbConnection.Database, accountName, characterSlot);
            if (!string.IsNullOrEmpty(characterID)) {
                playerData = new PlayerData(characterID);
                PlayerDataManager.LoadPlayerData(dbConnection.Database, playerData);
                PlayerDataManager.LoadPlayerStatistics(dbConnection.Database, playerData);
                PlayerDataManager.LoadPlayerMissionBoardMissions(dbConnection.Database, playerData);
                PlayerDataManager.LoadPlayerStoryHelperStateSettings(dbConnection.Database, playerData);
                PlayerDataManager.LoadPlayerJobList(dbConnection.Database, playerData);
                PlayerDataManager.LoadPlayerTriggerEvents(dbConnection.Database, playerData);

                inventory = new Inventory(playerData.Inventory);
                bank = new Inventory(playerData.Bank);

                // Load available ExpKit modules
                if (string.IsNullOrEmpty(playerData.AvailableModules)) {
                    AddEnabledExpKitModules();
                } else {
                    string[] modules = playerData.AvailableModules.Split(';');
                    availableExpKitModules = new AvailableExpKitModuleCollection(modules.Length);
                    AddEnabledExpKitModules();
                    for (int i = 0; i < modules.Length; i++) {
                        if (modules[i].IsNumeric()) {
                            if (availableExpKitModules.Contains((Enums.ExpKitModules)modules[i].ToInt()) == false) {
                                availableExpKitModules.Add(new AvailableExpKitModule((Enums.ExpKitModules)modules[i].ToInt(), false));
                            }
                        }
                    }
                }

                // Load statistics - Statistics data is inherited from PlayerData
                statistics = new Statistics.PlayerStatistics(this);

                // Load mission board data
                missionBoard = new WonderMails.MissionBoard(this);
                missionBoard.LoadMissionBoardData();
                missionBoard.UpdateMissionBoard();

                // Load joblist
                jobList = new WonderMailTasks(this);
                jobList.LoadJobList(playerData.JobList);

                //load mission goals [HIGH]
                goalPoints = new List<GoalPoint>();

                // Load story helper
                storyHelper = new StoryHelper(this);

                // Load trigger events [HIGH]
                triggerEvents = new List<Events.Player.TriggerEvents.ITriggerEvent>();

                // Load player team
                team = new Recruit[playerData.TeamMembers.Length];
                for (int i = 0; i < playerData.TeamMembers.Length; i++) {
                    team[i] = new Recruit(client);
                }
                for (int i = 0; i < playerData.TeamMembers.Length; i++) {
                    if (playerData.TeamMembers[i].RecruitIndex != -1) {
                        RecruitData recruitData = PlayerDataManager.LoadPlayerRecruit(dbConnection.Database, CharID, playerData.TeamMembers[i].RecruitIndex, playerData.TeamMembers[i].UsingTempStats);
                        if (recruitData != null) {
                            team[i].LoadFromRecruitData(recruitData, playerData.TeamMembers[i].RecruitIndex);
                        } else {
                            playerData.TeamMembers[i].RecruitIndex = -1;
                        }
                    }
                }

                for (int i = 0; i < playerData.TeamMembers.Length; i++) {
                    if (team[i].RecruitIndex != -1) {
                        team[i].LoadActiveItemList();
                    }
                }

                // Set the explorer rank
                while (ExplorerRank != Enums.ExplorerRank.Guildmaster && MissionExp >= MissionManager.DetermineMissionExpRequirement(ExplorerRank + 1)) {
                    ExplorerRank++;
                }

                if (MapID.IsNumeric()) {
                    int mapNum = MapID.ToInt();
                    if (mapNum == -2) {
                        MapID = MapManager.GenerateMapID(Settings.Crossroads);
                    } else {
                        MapID = "s" + MapID;
                    }
                }
                //            if (string.IsNullOrEmpty(this.id)) {
                //                id = PlayerID.GeneratePlayerID();
                //            }
                //PlayerID.AddPlayerToIndexList(CharID, client.TcpID);

                loaded = true;
            }

            dataLock.ExitReadLock();
            return result;
        }

        private void LoadTriggerEvents() {
            for (int i = 0; i < playerData.TriggerEvents.Count; i++) {
                Server.Events.Player.TriggerEvents.TriggerEventTrigger type = (Server.Events.Player.TriggerEvents.TriggerEventTrigger)playerData.TriggerEvents[i].Items.GetValue("Type").ToInt();
                Server.Events.Player.TriggerEvents.ITriggerEvent triggerEvent = Events.Player.TriggerEvents.TriggerEventHelper.CreateTriggerEventInstance(type);
                triggerEvent.Load(playerData.TriggerEvents[i], client);
                triggerEvents.Add(triggerEvent);
            }

        }

        public void SaveCharacterData(DatabaseConnection dbConnection) {
            if (loaded && !SavingLocked) {
                dataLock.EnterWriteLock();

                try {
                    if (playerData.AvailableModules == null) {
                        playerData.AvailableModules = "";
                    }
                    PlayerDataManager.SavePlayerAvailableExpKitModules(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerCharacteristics(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerGuild(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerItemGenerals(dbConnection.Database, playerData);
                    JobList.SaveJobList(dbConnection);
                    //PlayerDataManager.SavePlayerJobList(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerLocation(dbConnection.Database, playerData);
                    //PlayerDataManager.SavePlayerMissionBoardGenerals(dbConnection.Database, playerData);
                    missionBoard.SaveMissionBoardData();
                    PlayerDataManager.SavePlayerMissionBoardMissions(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerMissionGenerals(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerStoryGenerals(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerStoryHelperStateSettings(dbConnection.Database, playerData);
                    PlayerDataManager.SavePlayerStatistics(dbConnection.Database, playerData);

                    // Save inventory
                    ListPair<int, Characters.InventoryItem> updatedInventory = new ListPair<int, Characters.InventoryItem>();
                    for (int i = 1; i <= Inventory.Count; i++) {
                        if (Inventory[i].Updated) {
                            updatedInventory.Add(i, Inventory[i].BaseInventoryItem);
                            Inventory[i].Updated = false;
                        }
                    }
                    PlayerDataManager.SavePlayerInventoryUpdates(dbConnection.Database, CharID, updatedInventory);

                    // Save bank
                    ListPair<int, Characters.InventoryItem> updatedBank = new ListPair<int, Characters.InventoryItem>();
                    for (int i = 1; i <= Bank.Count; i++) {
                        if (Bank[i].Updated) {
                            updatedBank.Add(i, Bank[i].BaseInventoryItem);
                            Bank[i].Updated = false;
                        }
                    }

                    PlayerDataManager.SavePlayerBankUpdates(dbConnection.Database, CharID, updatedBank);

                    // Save trigger events
                    playerData.TriggerEvents.Clear();
                    for (int i = 0; i < triggerEvents.Count; i++) {
                        PlayerDataTriggerEvent triggerEvent = new PlayerDataTriggerEvent();
                        triggerEvents[i].Save(triggerEvent);
                        playerData.TriggerEvents.Add(triggerEvent);
                    }
                    PlayerDataManager.SavePlayerTriggerEvents(dbConnection.Database, playerData);

                    // Save team
                    for (int i = 0; i < team.Length; i++) {
                        playerData.TeamMembers[i].RecruitIndex = team[i].RecruitIndex;
                        playerData.TeamMembers[i].UsingTempStats = team[i].InTempMode;
                        if (team[i].Loaded) {
                            team[i].Save(dbConnection);
                        }
                    }
                    PlayerDataManager.SavePlayerTeam(dbConnection.Database, playerData);

                    IMap map = GetCurrentMap();
                    if (map != null && (map.MapType == Enums.MapType.Instanced || map.MapType == Enums.MapType.RDungeonMap || map.MapType == Enums.MapType.House)) {
                        map.Save();
                        //mInstancedMapManager.SaveActiveMap();
                    }
                    lock (mapsToDelete) {
                        AI.AIProcessor.mapGC.AddMaps(mapsToDelete);
                        mapsToDelete.Clear();
                    }
                } catch (Exception ex) {
                    System.Windows.Forms.MessageBox.Show("Map: " + MapID + System.Environment.NewLine + ex.ToString());
                } finally {
                    dataLock.ExitWriteLock();
                }
            }
        }

        public void AddMapToDelete(string mapID) {
            lock (mapsToDelete) {
                mapsToDelete.Add(mapID);
            }
        }

        public void SaveInventory(DatabaseConnection dbConnection) {
            PlayerDataManager.SavePlayerInventory(dbConnection.Database, playerData);
        }

        public void SaveBank(DatabaseConnection dbConnection) {
            PlayerDataManager.SavePlayerBank(dbConnection.Database, playerData);
        }

        public Player(Client client) {
            this.client = client;

            dataLock = new ReaderWriterLockSlim();
            storyPlaybackCache = new StoryPlaybackCache();
            timers = new Scripting.TimerCollection();
            customMenuManager = new CustomMenus.CustomMenuManager(client);
            CanSwapActiveRecruit = true;
            partyID = null;

            seenNpcs = new Dictionary<string, Dictionary<int, SeenCharacter>>();
            seenPlayers = new Dictionary<string, SeenCharacter>();

            mapsToDelete = new List<string>();
        }
    }
}
