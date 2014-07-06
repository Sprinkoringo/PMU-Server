using System;
using System.Collections.Generic;
using System.Text;
using PMU.DatabaseConnector.MySql;
using PMU.DatabaseConnector;

namespace DataManager.Parties {
    public class PartyData {

        public List<string> Members { get; set; }
        public string PartyID { get; set; }

        public PartyData() {
            Members = new List<string>();
        }
        
    }
}
