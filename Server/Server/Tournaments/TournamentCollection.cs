using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PMU.Core;

namespace Server.Tournaments
{
    public class TournamentCollection
    {
        ListPair<string, Tournament> tournaments;

        public TournamentCollection() {
            tournaments = new ListPair<string, Tournament>();
        }

        public void AddTournament(Tournament tournament) {
            if (tournaments.Keys.Contains(tournament.ID) == false) {
                tournaments.Add(tournament.ID, tournament);
            }
        }

        public void Remove(string tournamentID) {
            tournaments.RemoveAtKey(tournamentID);
        }

        public void Remove(Tournament tournament) {
            tournaments.RemoveAtValue(tournament);
        }

        public bool IsIDInUse(string idToTest) {
            return (this.tournaments.Keys.Contains(idToTest));
        }

        public Tournament this[string tournamentID] {
            get {
                if (tournaments.Keys.Contains(tournamentID)) {
                    return tournaments[tournamentID];
                } else {
                    return null;
                }
            }
        }

        public Tournament this[int index] {
            get { return tournaments.ValueByIndex(index); }
        }

        public int Count {
            get { return tournaments.Values.Count; }
        }
    }
}
