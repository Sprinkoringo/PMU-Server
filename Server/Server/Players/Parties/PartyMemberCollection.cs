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