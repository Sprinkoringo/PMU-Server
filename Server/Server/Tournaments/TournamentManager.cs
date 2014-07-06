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
