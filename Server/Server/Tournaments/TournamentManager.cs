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
using Server.Maps;

namespace Server.Tournaments
{
    public class TournamentManager
    {
        static TournamentCollection tournaments;

        public static TournamentCollection Tournaments {
            get { return tournaments; }
        }

        public static void Initialize() {
            tournaments = new TournamentCollection();
        }

        public static Tournament CreateTournament(Client manager, string name, string hubMap, int hubStartX, int hubStartY) {
            Tournament tournament = new Tournament(GenerateUniqueID(), name,  new WarpDestination(hubMap, hubStartX, hubStartY));
            // Add the manager
            TournamentMember member = new TournamentMember(tournament, manager);
            member.Admin = true;
            member.Active = true;
            tournament.RegisterPlayer(member);

            tournaments.AddTournament(tournament);

            return tournament;
        }

        public static void RemoveTournament(Tournament tournament) {
            tournaments.Remove(tournament);
        }

        private static string GenerateUniqueID() {
            string testID;
            while (true) {
                // Generate a new ID
                testID = Security.PasswordGen.Generate(16);
                // Check if the same ID is already in use
                if (!tournaments.IsIDInUse(testID)) {
                    // If it isn't, our generated ID is useable!
                    return testID;
                }
                // If the same ID is in use, try to generate a new ID
            }
        }
    }
}
