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
