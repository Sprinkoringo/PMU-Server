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


namespace Server.Players.Parties
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Server.Database;

    using Server.Network;

    public class Party
    {

        #region Constructors

        public Party(string partyID, Client leader) {
            PartyID = partyID;

            Members = new List<string>();
            Members.Add(leader.Player.CharID);
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                PartyManager.SaveParty(dbConnection.Database, this);
            }
        }

        public Party(string partyID) {
            PartyID = partyID;
            Members = new List<string>();
        }

        #endregion Constructors

        #region Properties

        public List<string> Members { get; set; }

        public int MemberCount {
            get { return Members.Count; }
        }

        public string PartyID
        {
            get;
            private set;
        }

        #endregion Properties

        public void HandoutExp(Client giver, ulong exp, PacketHitList hitlist) {
        	PacketHitList.MethodStart(ref hitlist);
        	bool totalForAll = true;
            ulong newExp;
            if (totalForAll)
            {
                newExp = exp;
            } else {

                newExp = exp * (ulong)(Members.Count + 3) / 4 / (ulong)Members.Count;
            }
            
            
            if (!(giver.Player.GetActiveRecruit().Level == Exp.ExpManager.Exp.MaxLevels) && giver.Player.Hunted && !giver.Player.Dead) {
                Scripting.ScriptManager.InvokeSub("PlayerEXP", hitlist, giver, newExp);
                                
                Combat.BattleProcessor.CheckPlayerLevelUp(hitlist, giver);
            }
            
            foreach (Client i in GetOnlineMemberClients()) {
                if (i.IsPlaying() && i != giver) {
                    //PartyMember member = members.FindMember(i.Player.CharID);
                    //if (member != null) {
                        if (giver.Player.Map == i.Player.Map) {
                            if (!(i.Player.GetActiveRecruit().Level == Exp.ExpManager.Exp.MaxLevels) && i.Player.Hunted && !i.Player.Dead) {
                                int lowestBound = 8000;
                                int chosenMiddle = 8000;
                                if (i.Player.GetActiveRecruit().Level < giver.Player.GetActiveRecruit().Level) {
                                    int diff = giver.Player.GetActiveRecruit().Level - i.Player.GetActiveRecruit().Level;
                                    lowestBound -= diff * diff * 100 / 5;
                                    int distance = Math.Max(Math.Abs(giver.Player.X - i.Player.X), Math.Abs(giver.Player.Y - i.Player.Y));
                                    
                                    if (distance > 40)
                                        distance = 40;

                                    if (distance < 8)
                                        distance = 8;
                                    
                                    chosenMiddle = lowestBound * (distance - 8) / 40 + chosenMiddle * (40 - (distance - 8)) / 40;

                                    if (chosenMiddle < 0) chosenMiddle = 0;
                                }

                                ulong moddedExp = newExp * (ulong)chosenMiddle / 8000;

                                if (moddedExp < 1) moddedExp = 1;

                                Scripting.ScriptManager.InvokeSub("PlayerEXP", hitlist, i, moddedExp);

                                Combat.BattleProcessor.CheckPlayerLevelUp(hitlist, i);
                            } else {
                                // The party member is at the max level, so do nothing
                            }
                        }
                    //}
                }
            }
            
            PacketHitList.MethodEnded(ref hitlist);
        }

        public bool AddToParty(Client client) {
            if (CanAddToParty(client)) {
                Add(client.Player.CharID);
                //SwitchOutExtraMembers();

                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PartyManager.SaveParty(dbConnection.Database, this);
                }
                return true;
            } else {
                return false;
            }
        }

        public void SwitchOutExtraMembers() {
            foreach (Client client in GetOnlineMemberClients()) {
                switch (Members.Count) {
                    case 2: {
                            if (client.Player.ActiveSlot == 2 || client.Player.ActiveSlot == 3) {
                                client.Player.SwapActiveRecruit(0, true);
                            }
                        }
                        break;
                    case 3:
                    case 4: {
                            if (client.Player.ActiveSlot != 0) {
                                client.Player.SwapActiveRecruit(0, true);
                            }
                        }
                        break;
                }
            }
        }

        public bool CanAddToParty(Client client) {
            // If there are less than 4 members
            if (Members.Count < 4) {
                return true;
            } else {
                return false;
            }
        }

        public bool CanRemoveFromParty(Client client) {
            return true;
        }

        public IEnumerable<Client> GetOnlineMemberClients() {
            for (int i = 0; i < Members.Count; i++) {
                Client client = ClientManager.FindClientFromCharID(Members[i]);
                if (client != null) {
                    yield return client;
                }
            }
        }

        public IEnumerable<string> GetMemberCharacters() {
            for (int i = 0; i < Members.Count; i++) {
                //Client client = ClientManager.GetClient(PlayerID.FindTcpID(Members[i]));
                //if (client != null) {
                    yield return Members[i];
                //}
            }
        }

        public Client GetLeader() {
            return ClientManager.FindClientFromCharID(Members[0]);
        }

        public bool IsLeaderInParty() {
            if (Members.Count < 1 || string.IsNullOrEmpty(Members[0])) {
                return false;
            }
            return IsPlayerInParty(Members[0]);
        }

        public bool Remove(Client client) {
            if (CanRemoveFromParty(client)) {
                Remove(client.Player.CharID);

                return true;
            } else {
                return false;
            }
        }

        private void Add(string playerID) {
            if (IsPlayerInParty(playerID) == false) {
                Members.Add(playerID);
                if (string.IsNullOrEmpty(Members[0])) {
                    Members[0] = playerID;
                }
            }
        }

        public bool IsPlayerInParty(string playerID) {
            for (int i = 0; i < Members.Count; i++) {
                if (Members[i] == playerID) {
                    return true;
                }
            }
            return false;
        }

        //public PartyMember FindMember(string playerID) {
        //    for (int i = 0; i < Members.Count; i++) {
        //        if (Members[i] == playerID) {
        //            return members[i];
        //        }
        //    }
        //    return null;
        //}

        public int GetMemberSlot(string playerID) {
            for (int i = 0; i < Members.Count; i++) {
                if (Members[i] == playerID) {
                    return i;
                }
            }
            return -1;
        }

        //public int GetMemberSlot(PartyMember member) {
        //    return Members.IndexOf(member);
        //}

        public void Remove(string playerID) {
            if (IsPlayerInParty(playerID)) {
                for (int i = 0; i < Members.Count; i++) {
                    if (Members[i] == playerID) {
                        Members.RemoveAt(i);
                        break;
                    }
                }

                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PartyManager.SaveParty(dbConnection.Database, this);
                }
            }
        }
    }
}