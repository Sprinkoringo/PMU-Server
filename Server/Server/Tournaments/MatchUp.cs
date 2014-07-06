using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Network;
using Server.Maps;

namespace Server.Tournaments
{
    public class MatchUp
    {
        Tournament tournament;
        TournamentMember playerOne;
        TournamentMember playerTwo;
        string id;

        MatchUpRules rules;

        public TournamentMember PlayerOne {
            get { return playerOne; }
        }

        public TournamentMember PlayerTwo {
            get { return playerTwo; }
        }

        public MatchUpRules Rules {
            get { return rules; }
            internal set {
                this.rules = value;
            }
        }

        public string ID {
            get { return id; }
        }

        IMap combatMap;

        public MatchUp(string id, Tournament tournament, TournamentMember playerOne, TournamentMember playerTwo) {
            this.id = id;
            this.tournament = tournament;
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;
        }

        public void StartMatchUp(IMap combatMap) {
            playerOne.Client.Player.TournamentMatchUp = this;
            playerTwo.Client.Player.TournamentMatchUp = this;

            this.combatMap = combatMap;
        }

        public void WarpToCombatMap(Client client) {
            if (client.Player.CharID == playerOne.Client.Player.CharID) {
                Messenger.PlayerWarp(client, combatMap, 10, 10);
            } else if (client.Player.CharID == playerTwo.Client.Player.CharID) {
                Messenger.PlayerWarp(client, combatMap, 11, 10);
            }
        }

        public TournamentMember SelectOtherMember(Client client) {
            if (client.Player.CharID == playerOne.Client.Player.CharID) {
                return playerTwo;
            } else if (client.Player.CharID == playerTwo.Client.Player.CharID) {
                return playerOne;
            } else {
                return null;
            }
        }

        public void EndMatchUp(string winnerPlayerID) {
            TournamentMember winner = null;
            TournamentMember loser = null;
            if (winnerPlayerID == playerOne.Client.Player.CharID) {
                winner = playerOne;
                loser = playerTwo;
            } else if (winnerPlayerID == playerTwo.Client.Player.CharID) {
                winner = playerTwo;
                loser = playerOne;
            }
            if (winner != null && loser != null) {
                // Warp both players to the hub
                tournament.WarpToHub(playerOne.Client);
                tournament.WarpToHub(playerTwo.Client);
                // Allow the winner to continue, prevent the loser from continuing
                winner.Active = true;
                loser.Active = false;

                winner.Client.Player.TournamentMatchUp = null;
                loser.Client.Player.TournamentMatchUp = null;

                tournament.MatchUpComplete(this);
            }
        }
    }
}
