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
using Server.Network;
using PMU.Core;
using Server.Maps;

namespace Server.Tournaments
{
    public class Tournament
    {
        string id;
        bool tournamentStarted;
        WarpDestination hub;
        string name;
        TournamentRules rules;

        public string Name {
            get { return name; }
        }

        public WarpDestination Hub {
            get { return hub; }
        }

        public string ID {
            get { return id; }
        }

        public bool TournamentStarted {
            get { return tournamentStarted; }
        }

        TournamentMemberCollection registeredMembers;

        List<MatchUp> activeMatchups;
        List<string> combatMaps;
        InstancedMap hubMap;

        int playersNeeded = 2;

        public int PlayersNeeded {
            get { return playersNeeded; }
            set {
                if (value < 2) {
                    value = 2;
                }
                playersNeeded = value;
            }
        }

        public List<MatchUp> ActiveMatchups {
            get { return activeMatchups; }
        }

        public TournamentMemberCollection RegisteredMembers {
            get { return registeredMembers; }
        }

        public TournamentRules Rules {
            get { return rules; }
        }

        public Tournament(string id, string name, WarpDestination hub) {
            this.id = id;
            this.name = name;
            this.hub = hub;

            registeredMembers = new TournamentMemberCollection();
            activeMatchups = new List<MatchUp>();
            combatMaps = new List<string>();

            rules = new TournamentRules();
        }

        public void AddCombatMap(string mapID) {
            combatMaps.Add(mapID);
        }

        public void WarpToHub(Client client) {
            if (hubMap == null) {
                hubMap = MapCloner.CreateInstancedMap(MapManager.RetrieveMap(hub.MapID));
            }
            Messenger.PlayerWarp(client, hubMap, hub.X, hub.Y);
        }

        public void CancelTournament(string reason) {
            Scripting.ScriptManager.InvokeSub("OnTournamentCanceled", this, reason);

            for (int i = registeredMembers.Count - 1; i >= 0; i--) {
                WarpToHub(registeredMembers[i].Client);
                registeredMembers[i].Client.Player.Tournament = null;
                registeredMembers[i].Client.Player.TournamentMatchUp = null;
                registeredMembers.RemoveAt(i);
            }
            TournamentManager.RemoveTournament(this);
        }

        public void RemoveRegisteredPlayer(Client client) {
            if (registeredMembers.Contains(client.Player.CharID)) {
                registeredMembers.Remove(client);
                client.Player.Tournament = null;

                bool tournyHasAdmin = false;
                for (int i = 0; i < RegisteredMembers.Count; i++) {
                    if (RegisteredMembers[i].Admin) {
                        tournyHasAdmin = true;
                        break;
                    }
                }
                if (tournyHasAdmin == false) {
                    CancelTournament("All tournament organizers have left!");
                }
            }
        }

        public void RegisterSpectator(Client client) {
            if (registeredMembers.Contains(client.Player.CharID) == false) {
                TournamentMember member = new TournamentMember(this, client);
                client.Player.Tournament = this;
                member.Active = false;
                registeredMembers.Add(member);
                OnPlayerRegistered(member);
            }
        }

        public void RegisterPlayer(Client client) {
            if (tournamentStarted == false) {
                if (registeredMembers.Count < playersNeeded && registeredMembers.Contains(client.Player.CharID) == false) {
                    TournamentMember member = new TournamentMember(this, client);
                    client.Player.Tournament = this;
                    member.Active = true;
                    registeredMembers.Add(member);
                    OnPlayerRegistered(member);
                }
            }
        }

        public void RegisterPlayer(TournamentMember member) {
            if (tournamentStarted == false) {
                if (registeredMembers.Count < playersNeeded) {
                    member.Client.Player.Tournament = this;
                    registeredMembers.Add(member);
                    OnPlayerRegistered(member);
                }
            }
        }

        private void OnPlayerRegistered(TournamentMember member) {
            Scripting.ScriptManager.InvokeSub("OnPlayerRegisteredInTournament", this, member, !member.Active);
        }

        private void StartTournamentIfReady() {
            if (IsReadyToStart()) {
                StartRound(new MatchUpRules());
            }
        }

        public int CountRemainingPlayers() {
            int activePlayerCount = 0;
            for (int i = 0; i < registeredMembers.Count; i++) {
                if (registeredMembers[i].Active) {
                    activePlayerCount++;
                }
            }
            return activePlayerCount;
        }

        public bool IsReadyToStart() {
            if (registeredMembers.Count == playersNeeded) {
                return true;
            } else {
                return false;
            }
        }

        public void StartRound(MatchUpRules matchUpRules) {
            if (!tournamentStarted) {
                tournamentStarted = true;
            }
            bool evenPlayerCount = (CountRemainingPlayers() % 2 == 0);
            TournamentMemberCollection membersWaitList = registeredMembers.Clone() as TournamentMemberCollection;
            // Remove inactive players from wait list
            for (int i = membersWaitList.Count - 1; i >= 0; i--) {
                if (membersWaitList[i].Active == false) {
                    membersWaitList.RemoveAt(i);
                }
            }
            if (!evenPlayerCount) {
                int skipIndex = MathFunctions.Rand(0, membersWaitList.Count);
                membersWaitList.RemoveAt(skipIndex);
            }
            this.activeMatchups.Clear();
            // Continue making match-ups until all players have been accounted for
            while (membersWaitList.Count > 0) {
                int playerOneIndex = MathFunctions.Rand(0, membersWaitList.Count);
                TournamentMember playerOne = membersWaitList[playerOneIndex];
                membersWaitList.RemoveAt(playerOneIndex);

                int playerTwoIndex = MathFunctions.Rand(0, membersWaitList.Count);
                TournamentMember playerTwo = membersWaitList[playerTwoIndex];
                membersWaitList.RemoveAt(playerTwoIndex);

                MatchUp matchUp = new MatchUp(GenerateUniqueMatchUpID(), this, playerOne, playerTwo);
                matchUp.Rules = matchUpRules;

                this.activeMatchups.Add(matchUp);

                int combatMapIndex = MathFunctions.Rand(0, combatMaps.Count);
                InstancedMap iMap = MapCloner.CreateInstancedMap(MapManager.RetrieveMap(combatMaps[combatMapIndex]));
                matchUp.StartMatchUp(iMap);
            }
        }

        internal void MatchUpComplete(MatchUp matchUp) {
            this.activeMatchups.Remove(matchUp);

            if (this.activeMatchups.Count == 0) {
                // All match-ups have been completed, and winners were determined

                int remainingPlayersCount = CountRemainingPlayers() + 1;
                if (remainingPlayersCount == 1) {
                    // Only one player left, so this player wins the tournament!
                    TournamentMember winner = null;
                    // Find the winning player instance
                    for (int i = 0; i < registeredMembers.Count; i++) {
                        if (registeredMembers[i].Active) {
                            winner = registeredMembers[i];
                        }
                    }
                    if (winner != null) {
                        TournamentComplete(winner);
                    }
                } else if (remainingPlayersCount > 1) {
                    // We have more than one player, continue the match-ups

                    Scripting.ScriptManager.InvokeSub("OnTournamentRoundComplete", this);
                }

            }
        }

        internal void TournamentComplete(TournamentMember winner) {
            Scripting.ScriptManager.InvokeSub("OnTournamentComplete", this, winner);
        }

        private string GenerateUniqueMatchUpID() {
            string testID;
            while (true) {
                // Generate a new ID
                testID = Security.PasswordGen.Generate(16);
                // Check if the same ID is already in use
                if (!IsMatchUpIDInUse(testID)) {
                    // If it isn't, our generated ID is useable!
                    return testID;
                }
                // If the same ID is in use, try to generate a new ID
            }
        }

        private bool IsMatchUpIDInUse(string idToTest) {
            for (int i = 0; i < activeMatchups.Count; i++) {
                if (activeMatchups[i].ID == idToTest) {
                    return true;
                }
            }
            return false;
        }

    }
}
