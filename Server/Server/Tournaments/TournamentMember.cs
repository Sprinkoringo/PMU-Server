using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Network;

namespace Server.Tournaments
{
    public class TournamentMember
    {
        string tournament;
        Client client;
        bool active;
        bool admin;

        public bool Admin {
            get { return admin; }
            set { admin = value; }
        }

        public Client Client {
            get { return client; }
        }

        public bool Active {
            get { return active; }
            set {
                active = value;
            }
        }

        public TournamentMember() {
            this.active = true;
            this.admin = false;
        }

        public Tournament GetTournamentInstance() {
            return TournamentManager.Tournaments[tournament];
        }

        public TournamentMember(Tournament tournament, Client client) {
            this.tournament = tournament.ID;
            this.client = client;
        }
    }
}
