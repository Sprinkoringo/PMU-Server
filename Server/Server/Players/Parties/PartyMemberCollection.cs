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
    using System.Linq;
    using System.Text;

    using Server.Network;

    public class PartyMemberCollection
    {
        #region Fields

        string leader;
        List<PartyMember> members;

        #endregion Fields

        #region Constructors

        public PartyMemberCollection() {
            members = new List<PartyMember>();
            leader = null;
        }

        #endregion Constructors

        #region Properties

        public int Count {
            get { return members.Count; }
        }

        #endregion Properties

        #region Methods

        public bool Add(Client client) {
            if (CanAddToParty(client)) {
                Add(client.Player.CharID);
                SwitchOutExtraMembers();
                return true;
            } else {
                return false;
            }
        }

        public void SwitchOutExtraMembers() {
            foreach (Client client in GetMemberClients()) {
                switch (members.Count) {
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
            if (members.Count < 4) {
                return true;
            } else {
                return false;
            }
        }

        public bool CanRemoveFromParty(Client client) {
            return true;
        }

        public IEnumerable<Client> GetMemberClients() {
            for (int i = 0; i < members.Count; i++) {
                Client client = ClientManager.FindClientFromCharID(members[i].PlayerID);
                if (client != null) {
                    yield return client;
                }
            }
        }
        
        public Client GetLeader() {
        	return ClientManager.FindClientFromCharID(leader);
        }

        public bool IsLeaderInParty() {
            if (string.IsNullOrEmpty(leader)) {
                return false;
            }
            return IsPlayerInParty(leader);
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
                members.Add(new PartyMember(playerID));
                if (string.IsNullOrEmpty(leader)) {
                    leader = playerID;
                }
            }
        }

        public bool IsPlayerInParty(string playerID) {
            for (int i = 0; i < members.Count; i++) {
                if (members[i].PlayerID == playerID) {
                    return true;
                }
            }
            return false;
        }

        public PartyMember FindMember(string playerID) {
            for (int i = 0; i < members.Count; i++) {
                if (members[i].PlayerID == playerID) {
                    return members[i];
                }
            }
            return null;
        }

        public int GetMemberSlot(string playerID) {
            for (int i = 0; i < members.Count; i++) {
                if (members[i].PlayerID == playerID) {
                    return i;
                }
            }
            return -1;
        }

        public int GetMemberSlot(PartyMember member) {
            return members.IndexOf(member);
        }

        private void Remove(string playerID) {
            if (IsPlayerInParty(playerID)) {
                for (int i = 0; i < members.Count; i++) {
                    if (members[i].PlayerID == playerID) {
                        members.RemoveAt(i);
                        break;
                    }
                }

            }
        }

        #endregion Methods
    }
}