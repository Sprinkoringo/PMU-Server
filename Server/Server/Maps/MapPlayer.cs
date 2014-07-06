using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;

namespace Server.Maps
{
    public class MapPlayer
    {
        Client client;

        public string PlayerID { get; private set; }
        public Client Client {
            get {
                if (client == null) {
                    client = ClientManager.FindClientFromCharID(PlayerID);
                }
                return client;
            }
            set { client = value; }
        }
            
        public MapPlayer(string playerID, Client client) {
            this.PlayerID = playerID;
            this.Client = client;
        }
    }
}
