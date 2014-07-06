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
using System.Linq;
using System.Text;
using Server.Maps;
using Server.Items;
using Server.Stories;
using Server.Missions;
using Server.RDungeons;
using Server.Evolutions;
using Server.Players;
using Server.Combat;
using Server.WonderMails;
using Server.Network;
using PMU.Sockets;
using Server.Players.Parties;
using Server.Database;
using PMU.Core;

namespace Server.Guilds {
    public class GuildManager {

        public const Enums.ExplorerRank MIN_RANK = Enums.ExplorerRank.Master;
        public const int RECRUIT_PRICE = 1000;
        public const int PROMOTE_PRICE = 10000;
        public const int CREATE_PRICE = 100000;

        public static void RegisterGuild(Client client, string name) {

            if (client.Player.HasItem(1) < CREATE_PRICE) {
                Messenger.PlayerMsg(client, "You do not have enough " + ItemManager.Items[1].Name + " to make a guild!", Text.BrightRed);
                return;
            }

            // Check if they are already in a guild
            if (!string.IsNullOrEmpty(client.Player.GuildName) && client.Player.GuildAccess > 0) {
                Messenger.PlayerMsg(client, "You are already in a guild!", Text.BrightRed);
                return;
            }

            for (int i = 0; i < name.Length; i++) {
                int n = (int)Convert.ToChar(name.Substring(i, 1));

                if (n == 32 && (i == 0 || i == name.Length - 1)) {
                    Messenger.PlayerMsg(client, "Names cannot begin or end with spaces.", Text.BrightRed);
                    return;
                }

                if ((n >= 65 && n <= 90) || (n >= 97 && n <= 122) || (n == 95) || (n == 32) || (n >= 48 && n <= 57)) {
                } else {
                    Messenger.PlayerMsg(client, "Invalid name, only letters, numbers, spaces, and _ are allowed.", Text.BrightRed);
                    return;
                }
            }

            if (name.Length < 3 || name.Length > 24) {
                Messenger.PlayerMsg(client, "Guild name must be between 3 and 24 letters long.", Text.BrightRed);
                return;
            }


            client.Player.GuildName = name;

            Party party = PartyManager.FindPlayerParty(client);

            if (party != null) {
                Messenger.AskQuestion(client, "CreateGuild", "Waiting for other party members to register...", -1, new string[1] { "Cancel" });
            }

            if (party != null) {
                bool matches = true;
                foreach (Client member in party.GetOnlineMemberClients()) {
                    if (member.Player.QuestionID != "CreateGuild") {
                        return;
                    }
                }

                foreach (Client member in party.GetOnlineMemberClients()) {
                    if (member.Player.GuildName != client.Player.GuildName) {
                        matches = false;
                        return;
                    }
                }
                if (!matches) {
                    foreach (Client member in party.GetOnlineMemberClients()) {
                        Messenger.PlayerMsg(client, "The guild names don't agree.  All party members must agree on the same guild name.", Text.BrightRed);
                        member.Player.GuildName = "";
                        Messenger.ForceEndStoryTo(member);
                    }
                    return;
                }
            }

            
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                ListPair<string, int> members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, name);

                if (party != null) {
                    foreach (Client member in party.GetOnlineMemberClients()) {
                        Messenger.ForceEndStoryTo(member);
                    }
                }

                if (members.Count > 0) {
                    if (party != null) {
                        foreach (Client member in party.GetOnlineMemberClients()) {
                            Messenger.PlayerMsg(member, "That guild name already exists!", Text.BrightRed);
                            member.Player.GuildName = "";
                        }
                    } else {
                        Messenger.PlayerMsg(client, "That guild name already exists!", Text.BrightRed);
                        client.Player.GuildName = "";
                    }
                    return;
                }

                if (party != null) {
                    foreach (Client member in party.GetOnlineMemberClients()) {
                        member.Player.TakeItem(1, CREATE_PRICE, true);
                        DataManager.Players.PlayerDataManager.AddGuildMember(dbConnection.Database, name, member.Player.CharID);
                        DataManager.Players.PlayerDataManager.SetGuildAccess(dbConnection.Database, member.Player.CharID, (int)Enums.GuildRank.Founder);
                        member.Player.GuildName = name;
                        member.Player.GuildAccess = Enums.GuildRank.Founder;
                        Messenger.SendPlayerGuild(member);
                        Messenger.PlayerMsg(client, "Guild created!", Text.BrightGreen);
                    }
                } else {
                    client.Player.TakeItem(1, CREATE_PRICE, true);
                    DataManager.Players.PlayerDataManager.AddGuildMember(dbConnection.Database, name, client.Player.CharID);
                    DataManager.Players.PlayerDataManager.SetGuildAccess(dbConnection.Database, client.Player.CharID, (int)Enums.GuildRank.Founder);
                    client.Player.GuildName = name;
                    client.Player.GuildAccess = Enums.GuildRank.Founder;
                    Messenger.SendPlayerGuild(client);
                    Messenger.PlayerMsg(client, "Guild created!", Text.BrightGreen);
                }
                
            }

            
            
        }

        public static void GuildStepDown(Client client) {
            if (string.IsNullOrEmpty(client.Player.GuildName)) {
                Messenger.PlayerMsg(client, "You are not in a guild.", Text.BrightRed);
                return;
            }
            bool stepDown = false;
            if (client.Player.GuildAccess > Enums.GuildRank.Member) {
                stepDown = true;
            }

            int index = -1;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                ListPair<string, int> members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                index = members.IndexOfKey(client.Player.CharID);

                if (stepDown) {
                    DataManager.Players.PlayerDataManager.SetGuildAccess(dbConnection.Database, members.KeyByIndex(index), (int)client.Player.GuildAccess - 1);
                    Messenger.PlayerMsg(client, "You have stepped down from your position in the guild.", Text.Blue);
                } else {
                    DataManager.Players.PlayerDataManager.RemoveGuildMember(dbConnection.Database, client.Player.CharID);
                    Messenger.PlayerMsg(client, "You have left your guild.", Text.Blue);
                }
            }

            if (stepDown) {
                client.Player.GuildAccess = (Enums.GuildRank)((int)client.Player.GuildAccess - 1);
            } else {
                client.Player.GuildName = "";
                client.Player.GuildAccess = Enums.GuildRank.None;
            }
            //send the guild access data to all
            Messenger.SendPlayerGuild(client);

            //send the update to all possible guild members
            if (index > -1) {
                Messenger.SendFullGuildUpdate(client.Player.GuildName);
            }
        }

        public static void DisbandGuild(Client client) {
            if (client.Player.GuildAccess < Enums.GuildRank.Founder) {
                Messenger.PlayerMsg(client, "You are not the owner of this guild!", Text.BrightRed);
                return;
            }

            string guildName = client.Player.GuildName;

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                DataManager.Players.PlayerDataManager.RemoveGuild(dbConnection.Database, guildName);
            }
            
            foreach (Client member in ClientManager.GetClients()) {
                if (member.Player.GuildName == guildName) {
                    member.Player.GuildName = "";
                    member.Player.GuildAccess = Enums.GuildRank.None;
                    Messenger.SendPlayerGuild(member);
                    Messenger.PlayerMsg(client, "Your guild has been disbanded.", Text.BrightRed);
                }
            }
        }

    }
}
