namespace Server.Players.Parties
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using PMU.Core;
    using PMU.DatabaseConnector;
    using PMU.DatabaseConnector.MySql;

    using Server.Network;
    using System.Threading;
    using Server.Database;
    using Server.Maps;

    public class PartyManager
    {
        #region Fields

        static ListPair<string, Party> parties = new ListPair<string, Party>();
        static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        #endregion Fields

        #region Methods

        public static int CountActiveParties() {
            int count = 0;
            rwLock.EnterReadLock();
            try {
                count = parties.Count;
            } finally {
                rwLock.ExitReadLock();
            }
            return count;
        }

        public static void CreateNewParty(Client leader) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);
            List<string> partyIDs = new List<string>();
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                partyIDs = LoadPartyIDList(dbConnection.Database);
            }

            if (leader.Player.PartyID == null) {
                string partyID = null;
                rwLock.EnterWriteLock();
                try {
                    while (true) {
                        string tempID = Security.PasswordGen.Generate(10, 15);
                        if (partyIDs.Contains(tempID) == false) {
                            partyID = tempID;
                            break;
                        }
                    }
                    Party party = new Party(partyID, leader);
                    leader.Player.PartyID = partyID;
                    parties.Add(partyID, party);
                } finally {
                    rwLock.ExitWriteLock();
                }
                leader.Player.AddExpKitModule(new AvailableExpKitModule(Enums.ExpKitModules.Party, true));
                PacketBuilder.AppendPartyMemberDataFor(leader, hitlist, 0);
                hitlist.AddPacket(leader, PacketBuilder.CreateChatMsg("You have created a new party!", Text.BrightGreen));
            } else {
                hitlist.AddPacket(leader, PacketBuilder.CreateChatMsg("You are already in a party!", Text.BrightRed));
            }

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void DisbandParty(Party party) {
            rwLock.EnterWriteLock();
            try {
                int partyIndex = parties.IndexOfValue(party);
                if (partyIndex > -1) {
                    parties.RemoveAt(partyIndex);
                }
            } finally {
                rwLock.ExitWriteLock();
            }
            
            foreach (Client client in party.GetOnlineMemberClients()) {
                if (client.IsPlaying()) {
                    client.Player.RemoveExpKitModule(Enums.ExpKitModules.Party);
                    client.Player.PartyID = null;
                    Messenger.SendDisbandPartyTo(client);
                    Messenger.PlayerMsg(client, "Your party has been disbanded!", Text.BrightRed);
                }
            }
        }

        public static Party FindParty(string partyID) {
            Party party = null;
            rwLock.EnterReadLock();
            try {
                int partyIndex = parties.IndexOfKey(partyID);
                if (partyIndex > -1) {
                    party = parties.ValueByIndex(partyIndex);
                }
            } finally {
                rwLock.ExitReadLock();
            }
            return party;
        }

        public static Party FindPlayerParty(Client client) {
            if (client != null) {
                if (!string.IsNullOrEmpty(client.Player.PartyID)) {
                    return FindParty(client.Player.PartyID);
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        public static void Initialize() {
            parties = new ListPair<string, Party>();
        }

        public static void JoinParty(string partyID, Client client) {
            JoinParty(FindParty(partyID), client);
        }

        public static void JoinParty(Party party, Client client) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            if (party != null) {
                Client leader = party.GetLeader();
                if (Combat.MoveProcessor.IsInAreaRange(1, client.Player.X, client.Player.Y, leader.Player.X, leader.Player.Y)) {
                    if (party.AddToParty(client)) {
                        client.Player.PartyID = party.PartyID;
                        client.Player.AddExpKitModule(new AvailableExpKitModule(Enums.ExpKitModules.Party, true));
                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You have joined the party!", Text.BrightGreen));
                        //PartyMember member = party.FindMember(client.Player.CharID);
                        int slot = party.GetMemberSlot(client.Player.CharID);
                        //Messenger.SendPartyMemberData(client, member, slot);
                        foreach (Client i in party.GetOnlineMemberClients()) {
                            if (i.IsPlaying() && i != client) {
                                //PartyMember teamMember = party.Members.FindMember(i.Player.CharID);
                                //int teamMemberSlot = party.Members.GetMemberSlot(i.Player.CharID);
                                //Messenger.SendPartyMemberData(client, teamMember, teamMemberSlot);
                                //Messenger.SendPartyMemberData(i, member, slot);
                                hitlist.AddPacket(i, PacketBuilder.CreateChatMsg(client.Player.Name + " has joined the party!", Text.BrightGreen));
                            }
                            PacketBuilder.AppendPartyMemberData(i, hitlist, slot);
                        }
                    } else {
                        hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You couldn't join the party!", Text.BrightRed));
                    }
                } else {
                    Messenger.PlayerMsg(client, "You need to stand next to the leader to join the party!", Text.BrightRed);
                }
            }

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RemoveFromParty(string clientID) {
            Party partyData = null;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                partyData = LoadCharacterParty(dbConnection.Database, clientID);
            }

            if (partyData != null) {

                partyData.Remove(clientID);

                PerformPartyChecks(partyData);
            }
        }

        public static void RemoveFromParty(string partyID, Client client) {
            RemoveFromParty(FindParty(partyID), client);
        }

        public static void RemoveFromParty(Party party, Client client) {
            if (party != null) {
                //if (CanBeRemovedFromParty(party, client)) {
                    int slot = party.GetMemberSlot(client.Player.CharID);
                    
                    party.Remove(client);
                    client.Player.PartyID = null;
                    client.Player.RemoveExpKitModule(Enums.ExpKitModules.Party);
                    Messenger.PlayerMsg(client, "You have been removed from your party!", Text.Black);
                    Messenger.SendLeftParty(party, slot);
                    Messenger.PartyMsg(party, client.Player.Name + " has been removed from the party!", Text.BrightGreen);
                    if (slot == 0 && party.IsLeaderInParty()) {
                        Client client2 = party.GetLeader();
                        if (client2 != null) {
                            Messenger.PartyMsg(party, client2.Player.Name + " is now the party leader!", Text.BrightGreen);
                        }
                    }
                    PerformPartyChecks(party);
                //} else {
                //    Messenger.PlayerMsg(client, "You can't be removed from your party here!", Text.Black);
                //}
            } else {
                Messenger.PlayerMsg(client, "You are not in a party!", Text.Black);
            }
        }

        //public static bool CanBeRemovedFromParty(Party party, Client client) {
        
        //    if (client.Player.Map.Moral == Enums.MapMoral.None) return false;

        //    return true;
        //}

        private static void PerformPartyChecks(Party party) {
            //bool shouldDisband = false;
            if (party.MemberCount == 0) {
                //shouldDisband = true;
                rwLock.EnterWriteLock();
                try {
                    int partyIndex = parties.IndexOfValue(party);
                    if (partyIndex > -1) {
                        parties.RemoveAt(partyIndex);
                    }
                } finally {
                    rwLock.ExitWriteLock();
                }
            }
            //if (party.IsLeaderInParty() == false) {
            //    shouldDisband = true;
            //}

            //if (shouldDisband) {
            //    DisbandParty(party);
            //}

            HandleMemberLogout(party.PartyID, null);
        }

        public static void LoadCharacterParty(Client client) {
            
            Party partyData = null;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                partyData = LoadCharacterParty(dbConnection.Database, client.Player.CharID);
            }

            if (partyData != null) {
                if (!parties.ContainsKey(partyData.PartyID)) {
                    parties.Add(partyData.PartyID, partyData);
                }
                client.Player.PartyID = partyData.PartyID;
                //party.SwitchOutExtraMembers();
            }

            
            
        }

        public static void HandleMemberLogout(string partyID, string leavingChar) {
            Party party = FindParty(partyID);
            int onlineClients = 0;
            if (party != null) {
                foreach (Client client in party.GetOnlineMemberClients()) {
                    if (client.Player.CharID != leavingChar) {
                        onlineClients++;
                    }
                }
                if (onlineClients <= 0) {
                    parties.RemoveAtKey(partyID);
                }
            }
        }

        public static List<string> LoadPartyIDList(MySql database)
        {
            List<string> idList = new List<string>();

            string query = "SELECT parties.PartyID " +
                "FROM parties;";
            List<DataColumnCollection> rows = database.RetrieveRows(query);
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (!idList.Contains(rows[i]["PartyID"].ValueString))
                    {
                        idList.Add(rows[i]["PartyID"].ValueString);
                    }
                }
            }

            return idList;
        }

        public static Party LoadParty(MySql database, string partyID)
        {
            Party partyData = new Party(partyID);

            string query = "SELECT parties.CharID " +
                "FROM parties " +
                "WHERE parties.PartyID = \'" + partyData.PartyID + "\';";
            List<DataColumnCollection> rows = database.RetrieveRows(query);
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    partyData.Members.Add(rows[i]["CharID"].ValueString);
                }
            }
            return partyData;
        }

        public static Party LoadCharacterParty(MySql database, string charID)
        {
            string partyID = "";

            string query = "SELECT parties.PartyID " +
                "FROM parties " +
                "WHERE parties.CharID = \'" + charID + "\';";
            List<DataColumnCollection> rows = database.RetrieveRows(query);
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    partyID = rows[i]["PartyID"].ValueString;
                }
            }
            else
            {
                return null;
            }

            Party partyData = new Party(partyID);

            query = "SELECT parties.CharID " +
                "FROM parties " +
                "WHERE parties.PartyID = \'" + partyData.PartyID + "\';";
            rows = database.RetrieveRows(query);
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    partyData.Members.Add(rows[i]["CharID"].ValueString);
                }
            }
            return partyData;
        }

        public static void LoadParty(MySql database, Party partyData)
        {
            string query = "SELECT parties.CharID " +
                "FROM parties " +
                "WHERE parties.PartyID = \'" + partyData.PartyID + "\';";
            List<DataColumnCollection> rows = database.RetrieveRows(query);
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    partyData.Members.Add(rows[i]["CharID"].ValueString);
                }
            }

        }

        public static void SaveParty(MySql database, Party partyData)
        {
            database.ExecuteNonQuery("DELETE FROM parties WHERE PartyID = \'" + partyData.PartyID + "\'");
            //database.DeleteRow("friends", "CharID = \'" + playerData.CharID + "\'");

            for (int i = 0; i < partyData.Members.Count; i++)
            {
                database.UpdateOrInsert("parties", new IDataColumn[] {
                    database.CreateColumn(false, "PartyID", partyData.PartyID),
                    database.CreateColumn(false, "PartySlot", i.ToString()),
                    database.CreateColumn(false, "CharID", partyData.Members[i])
                });
            }

        }

        public static void DeleteParty(MySql database, Party partyData)
        {
            database.ExecuteNonQuery("DELETE FROM parties WHERE PartyID = \'" + partyData.PartyID + "\'");
        }

        public delegate void PlayerWarpAction(Client client);

        public static bool AttemptPartyWarp(Client client, PlayerWarpAction action)
        {
            if (client.Player.PartyID == null)
            {
                action(client);
                return true;
            }
            else
            {
                bool warp = true;
                IMap sourceMap = client.Player.Map;
                Party party = PartyManager.FindPlayerParty(client);
                foreach (Client member in party.GetOnlineMemberClients())
                {
                    if (/*!member.Player.Dead &&*/ member.Player.MapID == client.Player.MapID && (member.Player.X != client.Player.X || member.Player.Y != client.Player.Y))
                    {
                        warp = false;
                    }
                }

                if (warp)
                {
                    foreach (Client member in party.GetOnlineMemberClients())
                    {
                        if (member.Player.Map != sourceMap) continue;

                        action(member);
                    }
                }
                else
                {
                    Messenger.PlayerMsg(client, "All surviving players on the floor must be on the goal in order to continue.", Text.WhiteSmoke);
                }
                return warp;
            }
        }

        #endregion Methods
    }
}