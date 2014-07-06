using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;
using PMU.DatabaseConnector.MySql;

namespace DataManager.Players
{
    public class PlayerData
    {
        public string Email { get; set; }
        public string CharID { get; set; }
        public string Name { get; set; }
        public byte Access { get; set; }
        public bool PK { get; set; }
        public bool Solid { get; set; }
        public string Status { get; set; }
        public bool Veteran { get; set; }
        public bool InTempMode { get; set; }
        public bool Dead { get; set; }

        public string AvailableModules { get; set; }

        public string Map { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte Direction { get; set; }

        public int ActiveSlot { get; set; }
        public PlayerDataTeamMember[] TeamMembers { get; set; }

        public string GuildName { get; set; }
        public byte GuildAccess { get; set; }

        public string CurrentChapter { get; set; }
        public int CurrentSegment { get; set; }

        public PlayerFriendsList Friends { get; set; }

        public ListPair<int, int> DungeonCompletionCounts { get; set; }
        public ListPair<int, byte> RecruitList { get; set; }

        public int MaxInv { get; set; }
        public int MaxBank { get; set; }

        public ListPair<int, Characters.InventoryItem> Inventory { get; set; }
        public ListPair<int, Characters.InventoryItem> Bank { get; set; }

        public int MissionExp { get; set; }

        public ListPair<int, bool> StoryChapters { get; set; }

        public int MissionCompletions { get; set; }
        public List<PlayerDataJobListItem> JobList { get; set; }

        public int LastGenTime { get; set; }

        public List<PlayerDataJobListItem> MissionBoardMissions { get; set; }

        public ListPair<string, string> StoryHelperStateSettings { get; set; }

        public List<PlayerDataTriggerEvent> TriggerEvents { get; set; }

        public string LastMacAddressUsed { get; set; }
        public string LastIPAddressUsed { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastLogout { get; set; }
        public string LastOS { get; set; }
        public string LastDotNetVersion { get; set; }
        public TimeSpan TotalPlayTime { get; set; }
        public TimeSpan LastPlayTime { get; set; }

        public bool DungeonCompletionCountsLoaded { get; internal set; }
        public bool RecruitListLoaded { get; internal set; }

        public PlayerData(string charID) {
            this.CharID = charID;
            TeamMembers = new PlayerDataTeamMember[4];
            for (int i = 0; i < TeamMembers.Length; i++) {
                TeamMembers[i] = new PlayerDataTeamMember();
            }
            Friends = new PlayerFriendsList(charID);
            DungeonCompletionCounts = new ListPair<int, int>();
            RecruitList = new ListPair<int, byte>();
            Inventory = new ListPair<int, Characters.InventoryItem>();
            Bank = new ListPair<int, Characters.InventoryItem>();
            StoryChapters = new ListPair<int, bool>();
            JobList = new List<PlayerDataJobListItem>();
            MissionBoardMissions = new List<PlayerDataJobListItem>();
            StoryHelperStateSettings = new ListPair<string, string>();
            TriggerEvents = new List<PlayerDataTriggerEvent>();
            LastLogin = DateTime.MinValue;
            LastLogout = DateTime.MinValue;
            Email = "";
        }
    }
}
